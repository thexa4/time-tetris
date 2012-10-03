using System;

namespace TimeTetris
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (TetrisGame game = new TetrisGame())
            {
                game.Run();
            }
        }
    }
#endif
}

