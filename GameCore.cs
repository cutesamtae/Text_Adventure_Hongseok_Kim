/******************************************************************************
 // File Name:         GameCore.cs
 // Author:            Hongseok Kim
 // Creation Date:     Oct 20, 2026
 //
 // Brief Description: Stores the room and player data and handles movement,
 //                    picking up the key, locked exits, and the win condition.
******************************************************************************/
using System;

namespace GAME140_TextAdventure
{
    struct RoomExitData
    {
        /// <summary>
        /// The name the player types to use this exit.
        /// </summary>
        public string Name;

        /// <summary>
        /// The room index this exit leads to.
        /// </summary>
        public int TargetRoomIndex;


        public bool Locked;

        /// <summary>
        /// Sets the exit name, destination, and lock status.
        /// </summary>
        /// <param name="name">The name used to choose this exit.</param>
        /// <param name="targetRoomIndex">The index of the room this exit leads to.</param>
        /// <param name="isLocked">Whether this exit needs a keycard.</param>
        public RoomExitData(string name, int targetRoomIndex, bool isLocked)
        {
            Name = name;
            TargetRoomIndex = targetRoomIndex;
            Locked = isLocked;
        }
    }


    /// <summary>
    /// Stores the information for one room.
    /// </summary>
    struct RoomData
    {
        public string Name;
        public string Description;
        public List<RoomExitData> Exits;
        /// <summary>
        /// Whether this room has a keycard.
        /// </summary>
        public bool HasKeycard;
        public bool CanEscape;

        /// <summary>
        /// Sets the room name, description, items, and exits.
        /// </summary>
        /// <param name="name">The room name.</param>
        /// <param name="description">The room description.</param>
        /// <param name="hasKeycard">Whether this room has a keycard.</param>
        /// <param name="canEscape">Whether the player can escape here.</param>
        public RoomData(string name, string description, bool hasKeycard = false, bool canEscape = false)
        {
            Name = name;
            Description = description;
            Exits = new List<RoomExitData>();
            HasKeycard = hasKeycard;
            CanEscape = canEscape;
        }

        /// <summary>
        /// Creates an exit and adds it to this room.
        /// </summary>
        /// <param name="exitName">The name used to choose this exit.</param>
        /// <param name="targetIndex">The index of the room this exit leads to.</param>
        /// <param name="isLocked">Whether the player needs a key to use this exit.</param>
        public void AddExitToRoom(string exitName, int targetIndex, bool isLocked = false)
        {
            RoomExitData roomExit = new RoomExitData(exitName, targetIndex, isLocked);
            Exits.Add(roomExit);
        }
    }


    /// <summary>
    /// Stores the player's current information and actions.
    /// </summary>
    struct PlayerState
    {
        /// <summary>
        /// Whether the player has the key for locked exits.
        /// </summary>
        public bool HasKeycard;

        /// <summary>
        /// Whether the game should end.
        /// </summary>
        public bool WantsToExit;

        /// <summary>
        /// Whether to show the current room.
        /// </summary>
        public bool WantsToLook;

        /// <summary>
        /// The index of the room the player is in.
        /// </summary>
        public int CurrentRoomIndex;

        /// <summary>
        /// The exit the player wants to use.
        /// </summary>
        public string DesiredExit;

        /// <summary>
        /// The item the player wants to pick up.
        /// </summary>
        public string DesiredPickup;
        
        /// <summary>
        /// Whether the player wants to escape.
        /// </summary>
        public bool WantsToEscape;
    } 

    /// <summary>
    /// Stores the information for the game world.
    /// </summary>
    struct WorldState
    {
        /// <summary>
        /// The list of rooms in the game.
        /// </summary>
        public List<RoomData> Rooms;
    }

    /// <summary>
    /// Sets up the game and handles player actions.
    /// </summary>
    static class GameCore
    {
        /// <summary>
        /// Sets up the rooms, exits, and starting player state.
        /// </summary>
        public static void InitializeGame(ref PlayerState playerState,
                                          ref WorldState worldState)
        {
            // Show the starting room when the game begins.
            playerState.WantsToLook = true;
            playerState.HasKeycard = false;

            Console.WriteLine(
                "The ship is badly damaged. Find a keycard and reach " +
                "the escape shuttle.\n" +
                "Type escape inside the shuttle to leave the ship.\n" +
                "Type help to see all commands.\n");

            worldState.Rooms = new List<RoomData>();

            // Create the starting room with three exits.
            RoomData mainHub = new RoomData(
                "Main Hub",
                "Red warning lights flash across the central hub. " +
                "Passages lead north, east, and west.");

            mainHub.AddExitToRoom("west", 1);
            mainHub.AddExitToRoom("east", 2);
            mainHub.AddExitToRoom("north", 4, true);
            worldState.Rooms.Add(mainHub); // Room index 0

            // Place the key in the storage bay.
            RoomData storageBay = new RoomData(
                "Storage Bay",
                "Loose supplies cover the floor of the storage bay. " +
                "A ladder leads north to the observation deck.",
                hasKeycard: true);

            storageBay.AddExitToRoom("east", 0);
            storageBay.AddExitToRoom("north", 3);
            worldState.Rooms.Add(storageBay); // Room index 1

            // Create an optional room for the player to explore.
            RoomData engineRoom = new RoomData(
                "Engine Room",
                "The damaged engine shakes and sends sparks into the air. " +
                "The passage back to the main hub is west.");

            engineRoom.AddExitToRoom("west", 0);
            worldState.Rooms.Add(engineRoom); // Room index 2

            // Connect the observation deck to the storage bay.
            RoomData observationDeck = new RoomData(
                "Observation Deck",
                "Through the window, you see broken pieces of the ship " +
                "drifting into space. A ladder leads south to the storage bay.");

            observationDeck.AddExitToRoom("south", 1);
            worldState.Rooms.Add(observationDeck); // Room index 3

            // Require the key to enter this room from the main hub.
            RoomData controlDeck = new RoomData(
                "Control Deck",
                "Most of the control screens are dark. " +
                "A sign points north toward the escape shuttle.");

            controlDeck.AddExitToRoom("south", 0);
            controlDeck.AddExitToRoom("north", 5);
            worldState.Rooms.Add(controlDeck); // Room index 4

            // Create the room where the player can escape.
            RoomData escapeShuttle = new RoomData(
                "Escape Shuttle",
                "A small escape shuttle waits in the launch bay. " +
                "Its control panel is still working.",
                canEscape: true);

            escapeShuttle.AddExitToRoom("south", 4);
            worldState.Rooms.Add(escapeShuttle); // Room index 5

            // Start the player in the main hub.
            playerState.CurrentRoomIndex = 0;
        }

        /// <summary>
        /// Reads a command and records what the player wants to do.
        /// </summary>
        public static void GetInput(ref PlayerState playerState,
                                    WorldState worldState)
        {
            // Clear the look, movement, and pickup requests before reading input.
            playerState.WantsToLook = false;
            playerState.WantsToEscape = false;
            playerState.DesiredExit = "";
            playerState.DesiredPickup = "";

            Console.WriteLine("\nWhat do you do?");
            Console.Write("> ");

            // Get the action and target from the input.
            TextAdventureCommand command = CommandParser.ParseAdventureCommand();

            // Check which command the player entered.
            if (command.Verb == "quit")
            {
                playerState.WantsToExit = true;
            }
            else if (command.Verb == "look")
            {
                playerState.WantsToLook = true;
            }
            else if (command.Verb == "escape")
            {
                playerState.WantsToEscape = true;
            }
            else if (command.Verb == "go")
            {
                playerState.DesiredExit = command.Parameter;
            }
            else if (command.Verb == "get")
            {
                playerState.DesiredPickup = command.Parameter;
            }
            else if (command.Verb == "inspect")
            {
                if (command.Parameter == "keycard")
                {
                    if (playerState.HasKeycard)
                    {
                        Console.WriteLine(
                            "The keycard has a blue stripe and the words " +
                            "'Control Deck Access' printed on it.");
                        Console.WriteLine(
                            "It opens the locked door north of the Main Hub.");
                    }
                    else
                    {
                        Console.WriteLine(
                            "I need to pick up the keycard before inspecting it.");
                    }
                }
                else
                {
                    Console.WriteLine("Try inspect keycard.");
                }
            }
            else if (command.Verb == "help")
            {
                Console.WriteLine(
                    "Command List: help, look, quit, go [direction], " +
                    "get keycard, inspect keycard, escape");
            }
            else
            {
                Console.WriteLine("I don't understand.");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Shows the room, key, inventory, and exits when needed.
        /// </summary>
        public static void RenderGame(PlayerState playerState,
                                      WorldState worldState)
        {
            if (playerState.WantsToLook)
            {
                // Get the room the player is currently in.
                RoomData currentRoom = worldState.Rooms[playerState.CurrentRoomIndex];
                Console.WriteLine("*************************************");
                Console.WriteLine("LOCATION: " + currentRoom.Name);
                Console.WriteLine(currentRoom.Description + "\n");

                if (currentRoom.HasKeycard)
                {
                    Console.WriteLine("There is a keycard in this room!");
                    Console.WriteLine(
                        "Type get keycard to pick it up, then inspect keycard " +
                        "to read it.\n");
                }

                if (playerState.HasKeycard)
                {
                    // Show the keycard in the inventory.
                    Console.WriteLine("INVENTORY: ");
                    Console.WriteLine("- Keycard\n");
                }
                
                Console.WriteLine("EXITS: ");
                for (int i = 0; i < currentRoom.Exits.Count; i++)
                {
                    // Get the next exit to display.
                    RoomExitData currentExit = currentRoom.Exits[i];
                    Console.WriteLine("- " + currentExit.Name);

                }
            }
        }

        /// <summary>
        /// Handles movement, picking up the keycard, and escaping.
        /// </summary>
        public static void UpdateGame(ref PlayerState playerState, 
                                      ref WorldState worldState)
        {
            RoomData currentRoom = worldState.Rooms[playerState.CurrentRoomIndex];
            // Check whether the player chose an exit.
            if (playerState.DesiredExit != "")
            {                
                bool foundExit = false;

                foreach(RoomExitData exit in currentRoom.Exits)
                {
                    if(exit.Name == playerState.DesiredExit)
                    {
                        foundExit = true;

                        // Stop the player if the exit is locked and they have no key.
                        if (exit.Locked && !playerState.HasKeycard)
                        {
                            Console.WriteLine("The door is locked! I need a keycard to open it.\n");
                        }
                        else
                        {
                            // Move to the next room and show it.
                            playerState.CurrentRoomIndex = exit.TargetRoomIndex;
                            playerState.WantsToLook = true;
                        }                        
                    }
                }

                if (!foundExit)
                {
                    Console.WriteLine($"I could not find the exit '{playerState.DesiredExit}'\n");
                }
            }
            else if (playerState.DesiredPickup != "")
            {
                if (playerState.DesiredPickup == "keycard")
                {
                    if (currentRoom.HasKeycard)
                    {
                        Console.WriteLine("I pick up the keycard.");
                        playerState.HasKeycard = true;
                        currentRoom.HasKeycard = false;

                        // Save the room after taking the keycard.
                        worldState.Rooms[playerState.CurrentRoomIndex] = currentRoom;
                    }
                    else
                    {
                        Console.WriteLine("I don't see a keycard here.");
                    }
                }
                else
                {
                    Console.WriteLine(
                        "I don't know what a '" +
                        playerState.DesiredPickup + "' is.");
                }
            }
            else if (playerState.WantsToEscape)
            {
                if (currentRoom.CanEscape)
                {
                    Console.WriteLine(
                        "You start the shuttle and launch away from the ship.");
                    Console.WriteLine("You escaped! You win!\n");

                    // End the game after escaping.
                    playerState.WantsToExit = true;
                }
                else
                {
                    // Tell the player to find the escape shuttle.
                    Console.WriteLine(
                        "I can't escape here. I need to reach the escape shuttle.");
                }
            }
        }
    

        /// <summary>
        /// Prints a message when the game ends.
        /// </summary>
        public static void CleanupGame(ref PlayerState playerState,
                                       ref WorldState worldState)
        {
            Console.WriteLine("Quitting...\n");
        }
    }
}
