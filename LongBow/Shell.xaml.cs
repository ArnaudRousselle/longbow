using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Data;
using LongBow.Common.Shortcut;

namespace LongBow
{
	[Export]
	public partial class Shell
	{
		[Import]
		public IShellViewModel ViewModel
		{
			get { return DataContext as IShellViewModel; }
			set { DataContext = value; }
		}

		public Shell()
		{
			InitializeComponent();
			Loaded += ShellLoaded;
		}

		private void ShellLoaded(object sender, RoutedEventArgs e)
		{
			Shortcut.AddShortcuts(InputBindings);
		}
	}
}
