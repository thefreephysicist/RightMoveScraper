using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RightMoveApp.ViewModel.Commands
{
	public interface IAsyncRelayCommand : ICommand
	{
		bool IsExecuting { get; }

		bool CanExecute();
		Task ExecuteAsync();
		Task ExecuteAsync(CancellationToken cancellationToken);
		Task ExecuteAsync(object parameter);
		Task ExecuteAsync(object parameter, CancellationToken cancellationToken);

		void InvalidateCommand();
	}
}
