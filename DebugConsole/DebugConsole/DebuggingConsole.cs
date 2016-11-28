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

        public DebuggingConsole()
        {
            commands = new Dictionary<string, CommandDescriptor>();
            Reset();
        }

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

        public void Reset()
        {
            output = new List<string>();
            colors = new List<Color>();
            matchingCommands = new List<string>();
            cursorIndex = 0;
            currentCommand = "";
            isOpen = false;
            isCursorVisable = false;
            elapsedBlinking = 0f;
            elapsedArrowKeys = arrowMoveInterval;
        }

        public bool AddCommand(CommandDescriptor command)
        {
            if (Commands.ContainsKey(command.Command))
                return false;

            Commands.Add(command.Command, command);
            return true;
        }

        public bool RemoveCommand(CommandDescriptor command)
        {
            if (Commands.ContainsKey(command.Command))
            {
                Commands.Remove(command.Command);
                return true;
            }

            return false;
        }

        public bool RemoveCommand(string command)
        {
            if (Commands.ContainsKey(command))
            {
                Commands.Remove(command);
                return true;
            }

            return false;
        }

        public void Insert(string insert)
        {
            for (int i = 0; i < insert.Length; i++)
            {
                Update(0, insert[i], false, false);
            }
        }

        public void Update(float elapsedMilliseconds, char c, bool arrowLeftPressed, bool arrowRightPressed)
        {
            CheckChar(c);
            Update(elapsedMilliseconds, arrowLeftPressed, arrowRightPressed);
        }

        public void Update(float elapsedMilliseconds, bool arrowLeftPressed, bool arrowRightPressed)
        {
            if(IsOpen)
            {
                //cursor blinking
                elapsedBlinking += elapsedMilliseconds;
                elapsedArrowKeys += elapsedMilliseconds;
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
                        cursorIndex += (cursorIndex >= currentCommand.Length ? currentCommand.Length -1 : 1);
                        elapsedArrowKeys = 0f;
                    }
                }

                lastRenderingInfo = renderingInfo;
                renderingInfo = new RenderInformation(cursorIndex, -1, isCursorVisable, currentCommand, output.ToArray(), colors.ToArray(), matchingCommands.ToArray());
            }
        }

        public void Open()
        {
            isOpen = true;
        }

        public void Close()
        {
            isOpen = false;
        }

        public void Clear()
        {
            output.Clear();
            colors.Clear();
        }

        public void WriteLine(string text)
        {
            output.Add(text);
            colors.Add(defaultColor);
        }

        public void WriteLine(string text, byte r, byte g, byte b)
        {
            output.Add(text);
            colors.Add(new Color(r, g, b));
        }

        public void WriteLines(string[] lines, byte r, byte g, byte b)
        {
            output.AddRange(lines);
            Color color = new Color(r, g, b);
            for (int i = 0; i < lines.Length; i++)
            {
                colors.Add(color);
            }
        }

        public void WriteLines(string[] lines, byte[] r, byte[] g, byte[] b)
        {
            output.AddRange(lines);
            for (int i = 0; i < lines.Length; i++)
            {
                colors.Add(new Color(r[i], g[i], b[i]));
            }
        }

        public void ExecuteCommand(string command, string[] args)
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

        public void ExecuteCommand(string commandLine)
        {
            BuildCommand(commandLine);
        }

        private void TryExecute()
        {
            string command = currentCommand;
            currentCommand = "";
            matchingCommands.Clear();
            cursorIndex = 0;
            BuildCommand(command);
        }

        private void BuildCommand(string commandLine)
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

        private void AddChar(char c)
        {
            if (c >= 32 && c <= 125)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(currentCommand.Substring(0, cursorIndex));
                sb.Append(c);
                if(cursorIndex + 1 < currentCommand.Length)
                    sb.Append(currentCommand.Substring(cursorIndex + 1));

                currentCommand = sb.ToString();
                cursorIndex++;
                SearchMatches();
            }
        }

        private void RemoveChar(bool backspace)
        {
            if (backspace)
            {
                if (cursorIndex > 0)
                {
                    currentCommand = currentCommand.Remove(cursorIndex - 1, 1);
                    cursorIndex--;
                }
            }
            else
            {
                if(cursorIndex + 1 < currentCommand.Length)
                    currentCommand = currentCommand.Remove(cursorIndex, 1);
            }
            SearchMatches();
        }

        private void SearchMatches()
        {
            matchingCommands.Clear();
            foreach (KeyValuePair<string, CommandDescriptor> cmd in commands)
            {
                if(cmd.Key.StartsWith(currentCommand))
                {
                    matchingCommands.Add(cmd.Key);
                }
            }
        }
    }
}
