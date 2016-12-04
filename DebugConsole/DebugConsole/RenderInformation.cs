
namespace DebugConsole
{
    /// <summary>
    /// Holds all information to render a debugging console
    /// </summary>
    public class RenderInformation
    {
        public int CursorIndex;
        public int MarkStartedIndex;
        public bool CursorVisable;
        public string CommandLine;
        public string[] Lines;
        public Color[] LineColors;
        public string[] AutoComplete;

        /// <summary>
        /// Initzializes render information
        /// </summary>
        /// <param name="cursorIndex">Current cursor index in the command line</param>
        /// <param name="markStartedIndex">Start index of the command line selection</param>
        /// <param name="cursorVisable">Indicates whenther the cursor is visable or not</param>
        /// <param name="commandLine">Command line string</param>
        /// <param name="lines">Console output lines</param>
        /// <param name="lineColors">Console output lines color</param>
        /// <param name="autoComplete">Command suggestions that matches with the current command line</param>
        public RenderInformation(int cursorIndex, int markStartedIndex, bool cursorVisable, string commandLine, string[] lines, Color[] lineColors, string[] autoComplete)
        {
            CursorIndex = cursorIndex;
            MarkStartedIndex = markStartedIndex;
            CursorVisable = cursorVisable;
            CommandLine = commandLine;
            Lines = lines;
            LineColors = lineColors;
            AutoComplete = autoComplete;
        }
    }
}
