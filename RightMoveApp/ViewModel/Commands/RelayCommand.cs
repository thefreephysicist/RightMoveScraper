using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace RightMoveApp.ViewModel.Commands
{
	public class RelayCommand : ICommand
	{
		Action<object> _execteMethod;
		Func<object, bool> _canexecuteMethod;

		public RelayCommand(Action<object> execteMethod, Func<object, bool> canexecuteMethod)
		{
			_execteMethod = execteMethod;
			_canexecuteMethod = canexecuteMethod;
		}

		public event EventHandler CanExecuteChanged
		{
			add
			{
				CommandManager.RequerySuggested += value;
			}
			remove
			{
				CommandManager.RequerySuggested -= value;
			}
		}

		public bool CanExecute(object parameter)
		{
			if (_canexecuteMethod != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public void Execute(object parameter)
		{
			_execteMethod(parameter);
		}
	}
}
