using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LongBow.Controls.Annotations;

namespace LongBow.Controls.Menus
{
	public class SmallLongBowMenuItem : ContentControl, INotifyPropertyChanged
	{
		static SmallLongBowMenuItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SmallLongBowMenuItem), new FrameworkPropertyMetadata(typeof(SmallLongBowMenuItem)));
			StyleProperty.OverrideMetadata(typeof(SmallLongBowMenuItem), new FrameworkPropertyMetadata(GetDefautlStyle()));
		}

		private static Style _defaultStyle;
		private static Style GetDefautlStyle()
		{
			return _defaultStyle ??
				   (_defaultStyle = Application.Current.FindResource(typeof(SmallLongBowMenuItem)) as Style);
		}

		#region Label

		public string Label
		{
			get { return (string)GetValue(LabelProperty); }
			set { SetValue(LabelProperty, value); }
		}

		public static readonly DependencyProperty LabelProperty =
			DependencyProperty.Register("Label", typeof(string), typeof(SmallLongBowMenuItem));

		#endregion

		#region Command

		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.Register("Command", typeof(ICommand), typeof(SmallLongBowMenuItem));

        #endregion

        #region CanExecute

        public bool CanExecute
	    {
	        get { return (bool) GetValue(CanExecuteProperty); }
	        set { SetValue(CanExecuteProperty, value); }
	    }

	    public static readonly DependencyProperty CanExecuteProperty = DependencyProperty.Register(
	        "CanExecute", typeof (bool), typeof (SmallLongBowMenuItem), new PropertyMetadata(default(bool)));

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

	    [NotifyPropertyChangedInvocator]
	    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	    {
	        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	    }

	    #endregion

	}
}
