namespace DebugConsole
{
    /// <summary>
    /// Representation of a Color in RGB 
    /// </summary>
    public struct Color
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        /// <summary>
        /// Initzializes a Color
        /// </summary>
        /// <param name="r">Red color channel (0-255)</param>
        /// <param name="g">Green color channel (0-255)</param>
        /// <param name="b">Blue color channel (0-255)</param>
        public Color(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
            A = 255;
        }

        /// <summary>
        /// Initzializes a Color
        /// </summary>
        /// <param name="r">Red color channel (0-255)</param>
        /// <param name="g">Green color channel (0-255)</param>
        /// <param name="b">Blue color channel (0-255)</param>
        /// <param name="a">Alpha channel (0-255)</param>
        public Color(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }
}
