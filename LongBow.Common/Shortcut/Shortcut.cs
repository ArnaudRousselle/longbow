using System.Windows.Input;
using LongBow.Common.Menu;

namespace LongBow.Common.Shortcut
{
	public static class Shortcut
	{
		public static void AddShortcuts(InputBindingCollection inputBindings)
		{
			inputBindings.Add(new KeyBinding
			{
				Command = MenuCommands.NewBillingCommand,
				Key = Key.F1,
			});

			inputBindings.Add(new KeyBinding
			{
				Command = MenuCommands.OpenListingCommand,
				Key = Key.F2,
			});

			inputBindings.Add(new KeyBinding
			{
				Command = MenuCommands.OpenCalendarListingCommand,
				Key = Key.F3,
			});

			inputBindings.Add(new KeyBinding
			{
				Command = MenuCommands.OpenFileImportCommand,
				Key = Key.F4,
			});

			inputBindings.Add(new KeyBinding
			{
				Command = MenuCommands.OpenReportingCommand,
				Key = Key.F5,
			});

            inputBindings.Add(new KeyBinding
            {
                Command = MenuCommands.OpenLineChartCommand,
                Key = Key.F6,
            });

            inputBindings.Add(new KeyBinding
			{
				Command = MenuCommands.OpenCalculator,
				Key = Key.F7,
			});

			inputBindings.Add(new KeyBinding
			{
				Command = MenuCommands.OpenCommand,
				Key = Key.O,
				Modifiers = ModifierKeys.Control,
			});

			inputBindings.Add(new KeyBinding
			{
				Command = MenuCommands.SaveCommand,
				Key = Key.S,
				Modifiers = ModifierKeys.Control,
			});

			inputBindings.Add(new KeyBinding
			{
				Command = MenuCommands.SaveAsCommand,
				Key = Key.S,
				Modifiers = ModifierKeys.Control | ModifierKeys.Shift,
			});

		}
	}
}
