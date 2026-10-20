/******************************************************************************
 // File Name:         CommandParser.cs
 // Author:            Hongseok Kim
 // Creation Date:     Oct 20, 2026
 //
 // Brief Description: Reads the player's input and splits it into an action
 //                    and a target for the text adventure game.
******************************************************************************/
using System;
using System.Collections.Generic;

namespace GAME140_TextAdventure
{
    /// <summary>
    /// Stores the action and target from the player's input.
    /// </summary>
    struct TextAdventureCommand  
    {
        /// <summary>
        /// The action the player types, like "go" or "look".
        /// </summary>
        public string Verb;

        /// <summary>
        /// The target after the action, like "north" or "key".
        /// </summary>
        public string Parameter;
    }

    /// <summary>
    /// Splits the player's input into an action and a target.
    /// </summary>
    static class CommandParser
    {
        /// <summary>
        /// Reads the input and gets the action and target.
        /// </summary>
        /// <returns>The action and target from the input.</returns>
        public static TextAdventureCommand ParseAdventureCommand()
        {
            // Read the input in lowercase. Use an empty string if there is no input.
            string inputStr = Console.ReadLine()?.ToLower() ?? "";

            // Split the input into words using spaces or tabs.
            string[] tokens = inputStr.Split(
                new char[] { ' ', '\t' },
                StringSplitOptions.RemoveEmptyEntries
            );

            // Create a command to store the input.
            TextAdventureCommand command = new TextAdventureCommand();

            // Use the first word as the action.
            if (tokens.Length >= 1)
            {
                command.Verb = tokens[0];

                // Use the second word as the target if there is one.
                if (tokens.Length >= 2)
                {
                    command.Parameter = tokens[1];
                }
            }

            return command;
        }
    }
}
