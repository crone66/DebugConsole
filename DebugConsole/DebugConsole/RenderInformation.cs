
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

        public RenderInformation(int cursorIndex, int markStartedIndex, bool cursorVisable, string commandLine, string[] lines, Color[] lineColors)
        {
            CursorIndex = cursorIndex;
            MarkStartedIndex = markStartedIndex;
            CursorVisable = cursorVisable;
            CommandLine = commandLine;
            Lines = lines;
            LineColors = lineColors;
        }
    }
}
