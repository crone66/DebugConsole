using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugConsole
{
    /// <summary>
    /// The command manager manages all available game commands (Command Pattern)
    /// </summary>
    public class CommandManager
    {
        private Dictionary<string, CommandDescriptor> registeredCommands;

        private CommandDescriptor[] commands;
        private Queue<CommandDescriptor> commandsToExecute;

        /// <summary>
        /// Initzializes a new command manager
        /// </summary>
        public CommandManager()
        {
            commandsToExecute = new Queue<CommandDescriptor>();
            registeredCommands = new Dictionary<string, CommandDescriptor>();
        }

        /// <summary>
        /// Initzializes a new command manager
        /// </summary>
        public CommandManager(CommandDescriptor[] registCommands)
        {
            registeredCommands = new Dictionary<string, CommandDescriptor>();
            commandsToExecute = new Queue<CommandDescriptor>();
            for (int i = 0; i < registCommands.Length; i++)
            {
                AddCommand(registCommands[i]);
            }
        }

        /// <summary>
        /// Adds a new command to the command manager
        /// </summary>
        /// <param name="command"></param>
        public bool AddCommand(CommandDescriptor command)
        {
            if (!registeredCommands.ContainsKey(command.Command))
            {
                registeredCommands.Add(command.Command, command);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Enqueues add command which will be executed on the next update
        /// </summary>
        /// <param name="commandName">Command name</param>
        public bool EnqueueCommand(string commandName)
        {
            if (registeredCommands.ContainsKey(commandName))
            {
                CommandDescriptor cmd = registeredCommands[commandName];
                if (cmd.RepeatAllowed || !commandsToExecute.Contains(cmd))
                {
                    commandsToExecute.Enqueue(cmd);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Clears the list of command to execute
        /// </summary>
        public void Clear()
        {
            commandsToExecute.Clear();
            commands = null;
        }

        /// <summary>
        /// Executes all commands since the last command execution (Update)
        /// </summary>
        /// <param name="elapsedSeconds">ElapsedSeconds since last update</param>
        public void Update(float elapsedSeconds)
        {
            commands = commandsToExecute.ToArray();
            commandsToExecute.Clear();

            float stackedSleep = 0;
            for (int i = 0; i < commands.Length; i++)
            {
                CommandDescriptor cmd = commands[i];
                if (cmd.IgnoreSleep || stackedSleep < elapsedSeconds)
                {
                    stackedSleep += cmd.Sleep;
                    cmd.ExecuteCommand();
                }
                else
                {
                    commandsToExecute.Enqueue(cmd);
                }
            }
        }

        /// <summary>
        /// Indicates whenther a command is registered or not
        /// </summary>
        /// <param name="commandName">Command name</param>
        /// <returns>Returns true when the command is registered</returns>
        public bool CommandExists(string commandName)
        {
            return registeredCommands.ContainsKey(commandName);
        }
    }
}
