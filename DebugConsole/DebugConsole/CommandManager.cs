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
        private CommandDescriptor[] commands;
        private Queue<CommandDescriptor> commandsToExecute;

        /// <summary>
        /// Initzializes a new command manager
        /// </summary>
        public CommandManager()
        {
            commandsToExecute = new Queue<CommandDescriptor>();
        }

        /// <summary>
        /// Adds a new command to the command manager
        /// </summary>
        /// <param name="command"></param>
        public void AddCommand(CommandDescriptor command)
        {
            if(command.RepeatAllowed || !commandsToExecute.Contains(command))
                commandsToExecute.Enqueue(command);
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
    }
}
