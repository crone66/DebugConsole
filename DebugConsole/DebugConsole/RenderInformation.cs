
namespace DebugConsole
{
    public class RenderInformation
    {
        public int CursorIndex;
        public int MarkStartedIndex;
        public bool CursorVisable;
        public string CommandLine;
        public string[] Lines;
        public Color[] LineColors;
        public string[] AutoComplete;

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
