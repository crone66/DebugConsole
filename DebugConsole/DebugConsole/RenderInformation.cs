
namespace DebugConsole
{
    public class RenderInformation
    {
        public int CursorIndex;
        public int MarkStartedIndex;
        public bool CursorVisable;
        public string CommandLine;
        public string[] VisableOutputs;
        public int[][] VisableOutputColors;

        public RenderInformation(int cursorIndex, int markStartedIndex, bool cursorVisable, string commandLine, string[] visableOutputs, int[][] visableOutputColors)
        {
            CursorIndex = cursorIndex;
            MarkStartedIndex = markStartedIndex;
            CursorVisable = cursorVisable;
            CommandLine = commandLine;
            VisableOutputs = visableOutputs;
            VisableOutputColors = visableOutputColors;
        }
    }
}
