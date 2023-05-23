using Microsoft.Practices.Prism.Commands;

namespace LongBow.Common.Interfaces.Tabulation
{
	public interface ITab
	{
		string HeaderTab { get; }
		DelegateCommand CloseTabCommand { get; }
	}
}
