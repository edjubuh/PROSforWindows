using System.Windows.Input;

namespace PROSforWindows.Commands
{
    public class ButtonCommand
    {
        public ICommand Command { get; set; }

        public string Header { get; set; }

        public ButtonCommand() { }

        public ButtonCommand(ICommand command, string header)
        {
            Command = command;
            Header = header;
        }
    }
}
