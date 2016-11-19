using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugConsole
{
    public class ExecuteCommandArgs : EventArgs
    {
        public CommandDescriptor Command;
        public string[] Args;

        public ExecuteCommandArgs(CommandDescriptor command, string[] args)
        {
            Command = command;
            Args = args;
        }
    }
}
