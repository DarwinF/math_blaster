using System;

namespace Math_Blaster
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (MathBlaster game = new MathBlaster())
            {
                game.Run();
            }
        }
    }
}

