using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugConsole
{
    /// <summary>
    /// Holds the Command handler parameters
    /// </summary>
    public class ExecuteCommandArgs : EventArgs
    {
        public object[] Args;

        /// <summary>
        /// Initzializes a new ExecuteCOmmandArgs
        /// </summary>
        /// <param name="args">Command handler parameters</param>
        public ExecuteCommandArgs(params object[] args)
        {
            Args = args;
        }
    }
}
