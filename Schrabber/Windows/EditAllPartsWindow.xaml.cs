using Schrabber.Commands;
using Schrabber.Models;
using System;
using System.Windows;
using System.Windows.Input;

namespace Schrabber.Windows
{
	/// <summary>
	/// Interaction logic for EditAllPartsWindow.xaml
	/// </summary>
	public partial class EditAllPartsWindow : Window
	{
		public Part Part => (Part)this.PartControl.DataContext;

		public static readonly DependencyProperty ConfirmCommandDependencyProperty = DependencyProperty.Register(
			nameof(ConfirmCommand),
			typeof(ICommand),
			typeof(EditAllPartsWindow),
			new PropertyMetadata(null)
		);

		public ICommand ConfirmCommand
		{
			get => (ICommand)this.GetValue(ConfirmCommandDependencyProperty);
			set => this.SetValue(ConfirmCommandDependencyProperty, value);
		}

		public EditAllPartsWindow(Media media)
		{
			this.InitializeComponent();

			this.ConfirmCommand = new DelegatedCommand<Object>(_ =>
			{
				this.DialogResult = true;
				this.Close();
			});
			this.PartControl.DataContext = new Part(media);
		}
	}
}
