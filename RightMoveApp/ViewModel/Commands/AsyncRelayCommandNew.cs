using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RightMoveApp.ViewModel.Commands
{
    public class AsyncRelayCommandNew : IAsyncRelayCommand
    {
        public bool IsExecuting => this.executionCount > 0;

        protected readonly Func<Task> ExecuteAsyncNoParam;
        protected readonly Action ExecuteNoParam;
        protected readonly Func<bool> CanExecuteNoParam;

        private readonly Func<object, CancellationTokenSource, Task> _executeAsync;
        private readonly Action<object> execute;
        private readonly Predicate<object> canExecute;
        private EventHandler canExecuteChangedDelegate;
        private int executionCount;
        private CancellationTokenSource _cancellationTokenSource;

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                this.canExecuteChangedDelegate = (EventHandler)Delegate.Combine(this.canExecuteChangedDelegate, value);
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
                this.canExecuteChangedDelegate = (EventHandler)Delegate.Remove(this.canExecuteChangedDelegate, value);
            }
        }

        #region Constructors

        public AsyncRelayCommandNew(Action<object> execute)
          : this(execute, param => true)
        {
        }

        public AsyncRelayCommandNew(Action executeNoParam)
          : this(executeNoParam, () => true)
        {
        }

        public AsyncRelayCommandNew(Func<object, CancellationTokenSource, Task> executeAsync)
          : this(executeAsync, param => true)
        {
        }

        public AsyncRelayCommandNew(Func<Task> executeAsyncNoParam)
          : this(executeAsyncNoParam, () => true)
        {
        }

        public AsyncRelayCommandNew(Action executeNoParam, Func<bool> canExecuteNoParam)
        {
            this.ExecuteNoParam = executeNoParam ?? throw new ArgumentNullException(nameof(executeNoParam));
            this.CanExecuteNoParam = canExecuteNoParam ?? (() => true);
        }

        public AsyncRelayCommandNew(Action<object> execute, Predicate<object> canExecute)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute ?? (param => true); ;
        }

        public AsyncRelayCommandNew(Func<Task> executeAsyncNoParam, Func<bool> canExecuteNoParam)
        {
            this.ExecuteAsyncNoParam = executeAsyncNoParam ?? throw new ArgumentNullException(nameof(executeAsyncNoParam));
            this.CanExecuteNoParam = canExecuteNoParam ?? (() => true);
        }

        public AsyncRelayCommandNew(Func<object, CancellationTokenSource, Task> executeAsync, Predicate<object> canExecute)
        {
	        _cancellationTokenSource = new CancellationTokenSource();

            this._executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            this.canExecute = canExecute ?? (param => true);
        }
        

        #endregion Constructors


        public bool CanExecute() => CanExecute(null);

        public bool CanExecute(object parameter) => this.canExecute?.Invoke(parameter)
                                                    ?? this.CanExecuteNoParam?.Invoke()
                                                    ?? true;

        async void ICommand.Execute(object parameter) => await ExecuteAsync(parameter, CancellationToken.None);

        public async Task ExecuteAsync() => await ExecuteAsync(null, CancellationToken.None);

        public async Task ExecuteAsync(CancellationToken cancellationToken) => await ExecuteAsync(null, cancellationToken);

        public async Task ExecuteAsync(object parameter) => await ExecuteAsync(parameter, CancellationToken.None);

        public async Task ExecuteAsync(object parameter, CancellationToken cancellationToken)
        {
            try
            {
                Interlocked.Increment(ref this.executionCount);
                cancellationToken.ThrowIfCancellationRequested();

                if (_executeAsync != null)
                {
                    // await _executeAsync.Invoke(parameter, cancellationToken).ConfigureAwait(false);
                    return;
                }
                if (ExecuteAsyncNoParam != null)
                {
                    await ExecuteAsyncNoParam.Invoke().ConfigureAwait(false);
                    return;
                }
                if (this.ExecuteNoParam != null)
                {
                    this.ExecuteNoParam.Invoke();
                    return;
                }

                this.execute?.Invoke(parameter);
            }
            finally
            {
                Interlocked.Decrement(ref this.executionCount);
            }
        }

        public void InvalidateCommand() => OnCanExecuteChanged();

        protected virtual void OnCanExecuteChanged() => this.canExecuteChangedDelegate?.Invoke(this, EventArgs.Empty);
    }
}
