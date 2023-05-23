using System.Collections.ObjectModel;
using LongBow.Common.Interfaces.File;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace LongBow
{
	public interface IShellViewModel
	{
		DelegateCommand<object> ClosingCommand { get; }
		DelegateCommand ToggleFullScreenModeCommand { get; }
		InteractionRequest<INotification> NotificationRequest { get; }
		bool FullScreen { get; }
		bool IsMenuMinimized { get; set; }
	}
}
