using System.Collections.Generic;
using System.Text;
using System.Threading;

/*TODO:
 * Add Mouse interaction
 * -> Change cursorindex
 * -> mark text
 * Fix entf key
 */

namespace DebugConsole
{
    /// <summary>
    /// Debugging console with command parser
    /// </summary>
    public class DebuggingConsole
    {
        private const float cursorBlinkInverval = 1000f;
        private const float arrowMoveInterval = 500f;

        private Dictionary<string, CommandDescriptor> commands;
        private string currentCommand;
        private int cursorIndex;
        private List<string> output;
        private List<Color> colors;
        private bool isOpen;
        private bool isCursorVisable;
        private float elapsedBlinking;
        private float elapsedArrowKeys;
        private Color defaultColor;

        private RenderInformation renderingInfo;
        private RenderInformation lastRenderingInfo;

        private List<string> matchingCommands;

        public Dictionary<string, CommandDescriptor> Commands
        {
            get
            {
                return commands;
            }
        }

        public bool IsOpen
        {
            get
            {
                return isOpen;
            }
        }

        public RenderInformation RenderingInfo
        {
            get
            {
                return renderingInfo;
            }
        }

        public RenderInformation LastRenderingInfo
        {
            get
            {
                return lastRenderingInfo;
            }
        }

        public Color DefaultColor
        {
            get
            {
                return defaultColor;
            }

            set
            {
                if (value.A > 10)
                {
                    defaultColor = value;
                }
                else
                {
                    throw new System.ArgumentException("Value of alpha channel must be greater than 10");
                }
            }
        }

        public string CurrentCommand
        {
            get
            {
                return currentCommand;
            }

            set
            {
                if (value != null)
                {
                    currentCommand = value;
                    cursorIndex = currentCommand.Length;
                    SearchMatches();
                }
                else
                    throw new System.NullReferenceException();
            }
        }

        /// <summary>
        /// Initzializes a new debugging console
        /// </summary>
        public DebuggingConsole()
        {
            commands = new Dictionary<string, CommandDescriptor>();
            Reset();
        }

        /// <summary>
        /// Initzializes a new debugging console
        /// </summary>
        /// <param name="commands">Adds available commands</param>
        /// <param name="defaultColor">Sets the default text color</param>
        public DebuggingConsole(CommandDescriptor[] commands, Color defaultColor)
        {
            this.commands = new Dictionary<string, CommandDescriptor>();
            DefaultColor = defaultColor;
            Reset();
            for (int i = 0; i < commands.Length; i++)
            {
                AddCommand(commands[i]);
            }
        }

        /// <summary>
        /// Resets the whole console
        /// </summary>
        public void Reset()
        {
            output = new List<string>();
            colors = new List<Color>();
            matchingCommands = new List<string>();
            cursorIndex = 0;
            CurrentCommand = "";
            isOpen = false;
            isCursorVisable = false;
            elapsedBlinking = 0f;
            elapsedArrowKeys = arrowMoveInterval;
        }

        /// <summary>
        /// Adds new commands to the debugging console
        /// </summary>
        /// <param name="command">Command description</param>
        /// <returns>Retruns false if the command already exists</returns>
        public bool AddCommand(CommandDescriptor command)
        {
            if (Commands.ContainsKey(command.Command))
                return false;

            Commands.Add(command.Command, command);
            return true;
        }

        /// <summary>
        /// Removes a given command
        /// </summary>
        /// <param name="command">Command that should be removed</param>
        /// <returns>Returns false when the given command doesn't exists</returns>
        public bool RemoveCommand(CommandDescriptor command)
        {
            if (Commands.ContainsKey(command.Command))
            {
                Commands.Remove(command.Command);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes a given command
        /// </summary>
        /// <param name="command">Command that should be removed</param>
        /// <returns>Returns false when the given command doesn't exists</returns>
        public bool RemoveCommand(string command)
        {
            if (Commands.ContainsKey(command))
            {
                Commands.Remove(command);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Inserts a string on the current cursor position
        /// </summary>
        /// <param name="insert">string to insert</param>
        public void Insert(string insert)
        {
            for (int i = 0; i < insert.Length; i++)
            {
                Update(0, insert[i], false, false);
            }
        }

        /// <summary>
        /// Updates the debugging console
        /// </summary>
        /// <param name="elapsedSeconds">Elapsed seconds since last update</param>
        /// <param name="c">Pressed char</param>
        /// <param name="arrowLeftPressed">State of the left arrow key</param>
        /// <param name="arrowRightPressed">State of the right arrow key</param>
        public void Update(float elapsedSeconds, char c, bool arrowLeftPressed, bool arrowRightPressed)
        {
            CheckChar(c);
            Update(elapsedSeconds, arrowLeftPressed, arrowRightPressed);
        }

        /// <summary>
        /// Updates the debugging console
        /// </summary>
        /// <param name="elapsedSeconds">Elapsed seconds since last update</param>
        /// <param name="arrowLeftPressed">State of the left arrow key</param>
        /// <param name="arrowRightPressed">State of the right arrow key</param>
        public void Update(float elapsedSeconds, bool arrowLeftPressed, bool arrowRightPressed)
        {
            if(IsOpen)
            {
                //cursor blinking
                elapsedBlinking += elapsedSeconds;
                elapsedArrowKeys += elapsedSeconds;
                if(elapsedBlinking >= cursorBlinkInverval)
                {
                    elapsedBlinking = 0f;
                    isCursorVisable = !isCursorVisable;
                }

                if (elapsedArrowKeys >= arrowMoveInterval)
                {
                    if (arrowLeftPressed)
                    {
                        cursorIndex -= (cursorIndex - 1 < 0 ? 0 : 1);
                        elapsedArrowKeys = 0f;
                    }

                    if (arrowRightPressed)
                    {
                        cursorIndex += (cursorIndex >= CurrentCommand.Length ? CurrentCommand.Length -1 : 1);
                        elapsedArrowKeys = 0f;
                    }
                }

                lastRenderingInfo = renderingInfo;
                renderingInfo = new RenderInformation(cursorIndex, -1, isCursorVisable, CurrentCommand, output.ToArray(), colors.ToArray(), matchingCommands.ToArray());
            }
        }

        /// <summary>
        /// Opens the console 
        /// </summary>
        public void Open()
        {
            isOpen = true;
        }

        /// <summary>
        /// Closes the console
        /// </summary>
        public void Close()
        {
            isOpen = false;
        }

        /// <summary>
        /// Clears the console output
        /// </summary>
        public void Clear()
        {
            output.Clear();
            colors.Clear();
        }

        /// <summary>
        /// Adds a new line to the output
        /// </summary>
        /// <param name="text">New output line</param>
        public void WriteLine(string text)
        {
            output.Add(text);
            colors.Add(defaultColor);
        }

        /// <summary>
        /// Adds a new line to the output
        /// </summary>
        /// <param name="text">New output line</param>
        /// <param name="r">Red color channel (0-255)</param>
        /// <param name="g">Green color channel (0-255)</param>
        /// <param name="b">Blue color channel (0-255)</param>
        public void WriteLine(string text, byte r, byte g, byte b)
        {
            output.Add(text);
            colors.Add(new Color(r, g, b));
        }

        /// <summary>
        /// Adds multiply lines to the output
        /// </summary>
        /// <param name="lines">New output lines</param>
        /// <param name="r">Red color channel (0-255)</param>
        /// <param name="g">Green color channel (0-255)</param>
        /// <param name="b">Blue color channel (0-255)</param>
        public void WriteLines(string[] lines, byte r, byte g, byte b)
        {
            output.AddRange(lines);
            Color color = new Color(r, g, b);
            for (int i = 0; i < lines.Length; i++)
            {
                colors.Add(color);
            }
        }

        /// <summary>
        /// Adds multiply lines to the output
        /// </summary>
        /// <param name="lines">New output lines</param>
        /// <param name="r">Red color channel (0-255)</param>
        /// <param name="g">Green color channel (0-255)</param>
        /// <param name="b">Blue color channel (0-255)</param>
        public void WriteLines(string[] lines, byte[] r, byte[] g, byte[] b)
        {
            output.AddRange(lines);
            for (int i = 0; i < lines.Length; i++)
            {
                colors.Add(new Color(r[i], g[i], b[i]));
            }
        }

        /// <summary>
        /// Executes a given command
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <param name="args">Command execution parameters</param>
        public void ExecuteCommand(string command, params object[] args)
        {
            if (commands.ContainsKey(command))
            {
                CommandDescriptor cmd = commands[command];

                if (cmd.UseOwnThread)
                {
                    Thread t = new Thread(() =>
                    {
                        cmd.ExecuteCommand(new ExecuteCommandArgs(cmd, args));
                    });
                    t.IsBackground = true;
                    t.Start();
                }
                else
                {
                    cmd.ExecuteCommand(new ExecuteCommandArgs(cmd, args));
                }
            }
            else
            {
                if (command != null)
                    WriteLine(string.Format("Command \"{0}\" doesn't exist!", command), 255, 0, 0);
            }
        }

        /// <summary>
        /// Executes a given command
        /// </summary>
        /// <param name="commandLine"></param>
        public void ExecuteCommand(string commandLine)
        {
            ParseCommand(commandLine);
        }

        /// <summary>
        /// Tries to execute the current command line
        /// </summary>
        private void TryExecute()
        {
            string command = CurrentCommand;
            CurrentCommand = "";
            matchingCommands.Clear();
            cursorIndex = 0;
            ParseCommand(command);
        }

        /// <summary>
        /// Parses a given command line and executes the command
        /// </summary>
        /// <param name="commandLine">Command line</param>
        private void ParseCommand(string commandLine)
        {
            List<List<string>> commandParts = SplitCommand(commandLine);
            for (int i = 0; i < commandParts.Count; i++)
            {
                if (commandParts[i].Count > 0)
                {
                    string command = commandParts[i][0];
                    commandParts[i].RemoveAt(0);

                    ExecuteCommand(command, commandParts[i].ToArray());
                }
            }
        }

        /// <summary>
        /// Splits the command line into commands and args
        /// </summary>
        /// <param name="command">command line</param>
        /// <returns>Returns all command parts</returns>
        private List<List<string>> SplitCommand(string command)
        {
            command = command.Trim();
            List<List<string>> commands = new List<List<string>>();
            StringBuilder sb = new StringBuilder();
            bool waitForEscape = false;
            List<string> parts = new List<string>();
            bool finishCommand = false;
            bool finishPart = false;
            for (int i = 0; i < command.Length; i++)
            {
                bool isComment = i + 1 < command.Length && command[i] == 47 && command[i + 1] == 47;
                finishCommand = !waitForEscape && (command[i] == 59 || isComment);
                finishPart = !waitForEscape && (finishCommand || command[i] == 32);
                if (!waitForEscape && isComment) //ignore comments
                {
                    i = command.Length;
                }

                if (finishCommand || finishPart)
                {
                    if (finishPart)
                    {
                        finishPart = false;
                        if (sb.Length > 0)
                        {
                            parts.Add(sb.ToString());
                            sb.Clear();
                        }
                    }

                    if (finishCommand)
                    {
                        finishCommand = false;
                        if (parts.Count > 0)
                        {
                            commands.Add(parts);
                            parts = new List<string>();
                        }
                    }
                }
                else
                {
                    if (command[i] == 34 && (i - 1 < 0 || command[i - 1] != 92)) //command == " and command != \"
                    {
                        waitForEscape = !waitForEscape;
                        continue;
                    }

                    sb.Append(command[i]);
                }
            }

            if(sb.Length > 0)
                parts.Add(sb.ToString());
            if (parts.Count > 0)
            {
                commands.Add(parts);
                parts = new List<string>();
            }
            return commands;
        }

        /// <summary>
        /// Handles a char depending on it's value
        /// </summary>
        /// <param name="c">Input char</param>
        private void CheckChar(char c)
        {
            switch (c)
            {
                case (char)8: //Backspace
                    RemoveChar(true);
                    break;
                case (char)13://Enter
                    TryExecute();
                    break;
                case (char)46: //Del
                    RemoveChar(false);
                    break;
                case (char)27: //ESC
                    Close();
                    break;
                default:
                    AddChar(c);
                    break;
            }
        }

        /// <summary>
        /// Adds a new char to the current conmand line
        /// </summary>
        /// <param name="c"></param>
        private void AddChar(char c)
        {
            if (c >= 32 && c <= 125)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(CurrentCommand.Substring(0, cursorIndex));
                sb.Append(c);
                if(cursorIndex + 1 < CurrentCommand.Length)
                    sb.Append(CurrentCommand.Substring(cursorIndex + 1));

                CurrentCommand = sb.ToString();
                SearchMatches();
            }
        }

        /// <summary>
        /// Removes a char from the current command line
        /// </summary>
        /// <param name="backspace">Indicates whenther the backspace is pressed or not</param>
        private void RemoveChar(bool backspace)
        {
            if (backspace)
            {
                if (cursorIndex > 0)
                {
                    CurrentCommand = CurrentCommand.Remove(cursorIndex - 1, 1);
                }
            }
            else
            {
                if(cursorIndex + 1 < CurrentCommand.Length)
                    CurrentCommand = CurrentCommand.Remove(cursorIndex, 1);
            }
            SearchMatches();
        }

        /// <summary>
        /// Searchs matching commands to update the auto suggestion
        /// </summary>
        private void SearchMatches()
        {
            matchingCommands.Clear();
            foreach (KeyValuePair<string, CommandDescriptor> cmd in commands)
            {
                if(cmd.Key.StartsWith(CurrentCommand))
                {
                    matchingCommands.Add(cmd.Key);
                }
            }
        }
    }
}
