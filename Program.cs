/*****************************************************************************
 // File Name:         Program.cs
 // Author:            Hongseok Kim
 // Creation Date:     Oct 20, 2026
 //
 // Brief Description: Runs the text adventure game loop and calls the functions
 //                    that set up, display, update, and end the game.
 *****************************************************************************/
namespace GAME140_TextAdventure
{
    internal class Program
    {
        /// <summary>
        /// Starts the game and runs the game loop.
        /// </summary>
        static void Main(string[] args) 
        {
            PlayerState playerState = new PlayerState();
            WorldState worldState = new WorldState();

            GameCore.InitializeGame(ref playerState, ref worldState);

            while (!playerState.WantsToExit)
            {
                GameCore.RenderGame(playerState, worldState);
                GameCore.GetInput(ref playerState, worldState);
                GameCore.UpdateGame(ref playerState, ref worldState);
            }

            GameCore.CleanupGame(ref playerState, ref worldState);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}