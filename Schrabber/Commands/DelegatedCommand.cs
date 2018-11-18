using System;
using System.Windows.Input;

namespace Schrabber.Commands
{
	internal class DelegatedCommand<T> : ICommand
	{

		private readonly Func<T, Boolean> _canExecute = null;
		private readonly Action<T> _execute;

		internal DelegatedCommand(Action<T> execute)
			=> this._execute = execute;
		internal DelegatedCommand(Func<T, Boolean> canExecute, Action<T> execute) : this(execute)
			=> this._canExecute = canExecute;

		#region ICommand
#pragma warning disable 67
		public event EventHandler CanExecuteChanged;
#pragma warning restore 67
		public Boolean CanExecute(Object parameter) => this._canExecute?.Invoke((T)parameter) ?? true;
		public void Execute(Object parameter) => this._execute((T)parameter);
		#endregion ICommand
	}
}
