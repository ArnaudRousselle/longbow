using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace LongBow.Common.Regions
{
	public class NotificationWindowRegionBehavior : WindowRegionBehavior
	{
		public const string Key = "NotificationWindowRegionBehavior";

		private const int NotificationHeight = 150;
		private const int NotificationWidth = 250;
		private const int Delta = 10;

		private readonly int _maxIndexLocation;
		private int _currentLocationIndex = -1;
		private int _nbNotifications;

		public NotificationWindowRegionBehavior()
		{
			_maxIndexLocation = ((int) SystemParameters.FullPrimaryScreenHeight/(NotificationHeight + Delta)) - 1;
		}

		protected override void OnViewAddedToRegion(object view)
		{
			base.OnViewAddedToRegion(view);
			_nbNotifications++;
		}

		protected override void OnViewRemovedFromRegion(object view)
		{
			base.OnViewRemovedFromRegion(view);

			_nbNotifications--;

			if (_nbNotifications < 0)
				_nbNotifications = 0;
		}

		protected override Window GetNewWindow()
		{
			return new Window();
		}

		protected override void WindowShowing(Window sender, object dataContext)
		{
			_currentLocationIndex = _nbNotifications <= 0 ? 0 : _currentLocationIndex + 1;

			if (_currentLocationIndex > _maxIndexLocation)
				_currentLocationIndex = _maxIndexLocation;

			sender.ShowInTaskbar = false;
			sender.WindowStyle = WindowStyle.None;
			sender.AllowsTransparency = true;

			sender.SizeToContent = SizeToContent.Manual;
			sender.Height = NotificationHeight;
			sender.Width = NotificationWidth;

			sender.Top = (SystemParameters.FullPrimaryScreenHeight - sender.Height*(_currentLocationIndex + 1) - Delta) -
			             (_currentLocationIndex*Delta);
			sender.Left = SystemParameters.FullPrimaryScreenWidth - sender.Width - Delta - 30;

			sender.ShowActivated = false;

			var thread = new Thread(
				() =>
				{
					Thread.Sleep(5000);
					sender.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
						new Action(sender.Close));
				})
			{ CurrentCulture = CultureInfo.InvariantCulture };

			thread.Start();
		}

		protected override void WindowShowed(Window sender, object dataContext)
		{

		}
	}
}
