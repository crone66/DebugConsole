using System;

namespace DebugConsole
{
    /// <summary>
    /// Describes a Command
    /// </summary>
    public struct CommandDescriptor
    {
        public string Command;
        public string Description;
        public bool UseOwnThread;
        public event EventHandler<ExecuteCommandArgs> CommandHandler;

        public object[] Args;
        public bool RepeatAllowed;
        public float Sleep;
        public bool IgnoreSleep;

        /// <summary>
        /// Initzializes a new command
        /// </summary>
        /// <param name="command">Command text case sensetive</param>
        /// <param name="description">Command desciption</param>
        /// <param name="useOwnThread">Indicates whenther a command handler should run in it's own thread or not</param>
        /// <param name="commandHandler">Reference to command handler</param>
        public CommandDescriptor(string command, string description, bool useOwnThread, EventHandler<ExecuteCommandArgs> commandHandler)
        {
            Command = command;
            Description = description;
            UseOwnThread = useOwnThread;
            CommandHandler = commandHandler;
            Args = null;
            RepeatAllowed = false;
            Sleep = 0f;
            IgnoreSleep = true;
        }

        /// <summary>
        /// Initzializes a new command
        /// </summary>
        /// <param name="command">Command text case sensetive</param>
        /// <param name="description">Command desciption</param>
        /// <param name="useOwnThread">Indicates whenther a command handler should run in it's own thread or not</param>
        /// <param name="repeatAllowed">Allows multiply command executions of the same command per update</param>
        /// <param name="sleep">Time delay between command execution</param>
        /// <param name="ignoreSleep">Indicates whenther the command ignores the time delay or not</param>
        /// <param name="commandHandler">Reference to command handler</param>
        /// <param name="args">Command execution parameters</param>
        public CommandDescriptor(string command, string description, bool useOwnThread, bool repeatAllowed, float sleep, bool ignoreSleep, EventHandler<ExecuteCommandArgs> commandHandler, params object[] args)
        {
            Command = command;
            Description = description;
            UseOwnThread = useOwnThread;
            Args = args;
            RepeatAllowed = repeatAllowed;
            Sleep = sleep;
            IgnoreSleep = ignoreSleep;
            CommandHandler = commandHandler;
        }

        /// <summary>
        /// Initzializes a new command
        /// </summary>
        /// <param name="command">Command text case sensetive</param>
        /// <param name="description">Command desciption</param>
        /// <param name="useOwnThread">Indicates whenther a command handler should run in it's own thread or not</param>
        /// <param name="repeatAllowed">Allows multiply command executions of the same command per update</param>
        /// <param name="sleep">Time delay between command execution</param>
        /// <param name="ignoreSleep">Indicates whenther the command ignores the time delay or not</param>
        /// <param name="commandHandler">Reference to command handler</param>
        public CommandDescriptor(string command, string description, bool useOwnThread, bool repeatAllowed, float sleep, bool ignoreSleep, EventHandler<ExecuteCommandArgs> commandHandler)
        {
            Command = command;
            Description = description;
            UseOwnThread = useOwnThread;
            Args = null;
            RepeatAllowed = repeatAllowed;
            Sleep = sleep;
            IgnoreSleep = ignoreSleep;
            CommandHandler = commandHandler;
        }

        /// <summary>
        /// Calls the command handler with the given args
        /// </summary>
        /// <param name="args">Command handler parameters</param>
        internal void ExecuteCommand(ExecuteCommandArgs args)
        {
            CommandHandler?.Invoke(this, args);
        }

        /// <summary>
        /// Calls the command handler with the given args on initzialization
        /// </summary>
        internal void ExecuteCommand()
        {
            CommandHandler?.Invoke(this, new ExecuteCommandArgs(Args));
        }
    }
}
