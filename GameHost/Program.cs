using System;

namespace GameHost
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (var game = new Game.Game())
            {
                game.Run();
            }
        }
    }
#endif
}

