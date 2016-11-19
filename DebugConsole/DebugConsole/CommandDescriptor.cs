using System;

namespace DebugConsole
{
    public class CommandDescriptor
    {
        public string Command;
        public string Description;
        public bool UseOwnThread;
        public event EventHandler<ExecuteCommandArgs> CommandHandler;

        public CommandDescriptor(string command, string description, bool useOwnThread, EventHandler<ExecuteCommandArgs> commandHandler)
        {
            Command = command;
            Description = description;
            UseOwnThread = useOwnThread;
            CommandHandler = commandHandler;
        }

        internal void ExecuteCommand(ExecuteCommandArgs args)
        {
            if (CommandHandler != null)
                CommandHandler(this, args);
        }
    }
}
