using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

public class Program
{
    public static void Main(string[] args)
    {
        GameEngine engine = new GameEngine();
        engine.Run();
    }
}

public class StoryScene
{
    public int SceneId;
    public string Title;
    public string StoryText;
    public string[] Choices;
    public int[] NextSceneIDs;

    public StoryScene(int SceneId, string Title, string StoryText, string[] Choices, int[] NextSceneIDs)
    {
        this.SceneId = SceneId;
        this.Title = Title;
        this.StoryText = StoryText;
        this.Choices = Choices;
        this.NextSceneIDs = NextSceneIDs;
    }

    public void Display(List<int> availableIndices)
    {
        if (!string.IsNullOrEmpty(Title))
        {
            Console.WriteLine(Title);
            Console.WriteLine();
        }
        PrintWrapped(StoryText);

        for (int i = 0; i < availableIndices.Count; i++)
        {
            int originalIndex = availableIndices[i];
            string choiceText = Choices[originalIndex];

            if (choiceText.Length > 3 && char.IsDigit(choiceText[0]) && choiceText[1] == '.' && choiceText[2] == ' ')
            {
                choiceText = choiceText.Substring(3);
            }

            Console.WriteLine($"{i + 1}. {choiceText}");
        }
    }

    public static void PrintWrapped(string text, int lineWidth = 120)
    {
        string[] words = text.Split(' ');
        int currentLineLength = 0;

        foreach (string word in words)
        {
            if (currentLineLength + word.Length + 1 > lineWidth)
            {
                Console.WriteLine();
                currentLineLength = 0;
            }

            Console.Write(word + " ");
            currentLineLength += word.Length + 1;
        }
        Console.WriteLine();
        Console.WriteLine();
    }
}

public class GameEngine
{
    private Dictionary<int, StoryScene> keyValuePairs = new Dictionary<int, StoryScene>();
    private string playerName = "Traveler";
    private int knifeNotches = 1;

    private HashSet<int> attemptedTreeChoices = new HashSet<int>();
    private HashSet<int> attemptedWitchChoices = new HashSet<int>();
    private HashSet<int> attemptedMerchantChoices = new HashSet<int>();

    public void Run()
    {
        SetupScenes();
        GameStart();
    }

    private void UpdateSceneTexts()
    {
        if (keyValuePairs.ContainsKey(0))
        {
            keyValuePairs[0].StoryText = "You awaken, disorientated, and try to get yourself up and dust yourself off, but you cannot remember anything. " +
            "You look around to try to find any clues and see an unrecognisable name carved into a tree nearby. " +
            "As you move your hand, you realise a knife is entangled with your hand in thorn vines, " +
            $"featuring {knifeNotches} scorched notch{(knifeNotches > 1 ? "es" : "")} burned into the steel. " +
            "Further around you, you see a smoke trail through the tops of the trees, a cobble path leading down with wagon wheel tracks, " +
            "and a faint ethereal glow from deeper within the forest.";
        }
    }

    private void SetupScenes()
    {
        StoryScene scenePrologue = new StoryScene(
            -1,
            "Prologue",
            "You walk into a dense and ancient forest and make your way to a tree that catches your eye. " +
            "You take out your personal pocket knife and decide to carve your name into the tree in front of you.",
            new string[] { "Type a name to carve into the tree and press Enter." },
            new int[] { 0 }
        );
        keyValuePairs.Add(-1, scenePrologue);

        StoryScene scene0 = new StoryScene(
            0,
            "Scene 0: Awaken",
            "",
            new string[] {
                "1. Enter deeper into the forest to investigate the ethereal glow.",
                "2. Make your way to the origin of the smoke trail.",
                "3. Follow the cobble path."
            },
            new int[] { 1, 2, 3 }
        );
        keyValuePairs.Add(0, scene0);

        StoryScene scene1 = new StoryScene(
            1,
            "Scene 1: Tree of Wisdom",
            "As you enter deeper into the forest, weaving in and out of trees, the glow becomes brighter and brighter " +
            "until you reach the source of the glow: a tree as huge as a building and with a canopy large enough to shade an entire neighbourhood. " +
            "As you get closer, the giant tree's gnawed bark twists and turns into a vengeful face. Before you have a chance... " +
            "The tree speaks in a booming, omnipotent voice: 'I AM THE TREE OF WISDOM. BEFORE THE FOREST WAS, I WAS JUST A SAPLING. " +
            "BEFORE THE FOREST WAS, I WATCHED COUNTLESS WARS AMONG MEN. AND YOU WHO DEFACED MY BRETHREN, MARKING THEM YOURSELF AND LEAVING YOUR BRAND. " +
            "NOW, EXPLAIN YOURSELF AND PERHAPS I SHALL FORGIVE YOU.'",
            new string[] {
                "1. Try to plead with the tree for forgiveness.",
                "2. Fight with the tree for its absurd accusations."
            },
            new int[] { 4, 103 }
        );
        keyValuePairs.Add(1, scene1);

        StoryScene scene2 = new StoryScene(
            2,
            "Scene 2: The Witch's Hut",
            "You make your way to the origin of the smoke trail. " +
            "As you edge closer to the smoke, you see a small, dishevelled hut. " +
            "A wicked looking witch appears in front of you. " +
            "She is wearing a ragged cloak and holding a broomstick and a potion bottle. " +
            "You desperately ask her, 'What is going on here, can you please help me remember?' " +
            "In a squealing voice, she replies, 'I can help you, if you can answer my riddle correctly; if not, I will make decorations out of you.'",
            new string[] {
                "1. Try to answer her riddle.",
                "2. Steal the potion and drink it.",
                "3. Try to steal her broom and escape."
            },
            new int[] { 5, 106, 107 }
        );
        keyValuePairs.Add(2, scene2);

        StoryScene scene3 = new StoryScene(
            3,
            "Scene 3: Merchant Cabin",
            "You follow the cobble path and find yourself approaching a log cabin with a wagon stationed outside. " +
            "You knock on the door of the cabin, and a merchant asks through the door, 'What do you seek?' " +
            "You answer, 'I need your help, please. I can't remember anything.' " +
            "He appears, cracking open the door to peer at his guest. " +
            "'If you want my advice, you'll have to earn it with some muscle.' " +
            "He points to a massive, awkward iron anvil sitting off center in his workshop. 'Help me heave that heavy anvil into the corner first.'",
            new string[] {
                "1. Help him move the heavy iron anvil.",
                "2. Try to force or trade your thorn entangled knife onto him.",
                "3. Rob the merchant with the knife."
            },
            new int[] { 31, 109, 110 }
        );
        keyValuePairs.Add(3, scene3);

        StoryScene scene3B = new StoryScene(
            31,
            "Scene 3B: The Anvil Heavy Lift",
            "You step inside to grip the cold, massive iron anvil. " +
            "MINI GAME RULE: Smash the [SPACEBAR] 25 times within 10 seconds to lift the anvil into the corner! " +
            "Press enter to start.",
            new string[] { "1. Begin lifting" },
            new int[] { 32 }
        );
        keyValuePairs.Add(31, scene3B);

        StoryScene scene3C = new StoryScene(
            32,
            "Scene 3C: Merchant Reward",
            "The merchant nods approvingly, wiping sweat from his brow. 'Not bad at all, traveler. You've got grit. " +
            "Take this advice: head deeper into the forest. The ancient guardian there holds answers.'",
            new string[] { "1. Head deeper into the forest toward the glow" },
            new int[] { 1 }
        );
        keyValuePairs.Add(32, scene3C);

        StoryScene scene4 = new StoryScene(
            4,
            "Scene 4: Plead for Forgiveness",
            "You decided to plead with the trees for forgiveness. " +
            "Type a short sentence, to plead with the trees for forgiveness, then press Enter.",
            new string[] { "(Type your response)" },
            new int[] { 104 }
        );
        keyValuePairs.Add(4, scene4);

        StoryScene scene5 = new StoryScene(
            5,
            "Scene 5: The Witch's Riddle",
            "You decided to try to answer the witch's riddle. " +
            "The witch's riddle: 'I carry all your yesterdays, I bring back the lost and tears, I am as light as dust, and am lost in a moment... What am I?' " +
            "Type your answer as one word and press Enter.",
            new string[] { "(Type your response)" },
            new int[] { 105 }
        );
        keyValuePairs.Add(5, scene5);

        StoryScene endingRiddleSuccess = new StoryScene(
            113,
            "Ending 13: Riddle Master",
            "The witch gasps in shock as your correct answer pierces the magical veil. She hands you a vial of pure memory before fleeing into the trees. You reclaim your past and escape the forest triumphant!",
            new string[] { "1. Play again from the beginning", "2. Quit game" },
            new int[] { -1, 999 }
        );
        keyValuePairs.Add(113, endingRiddleSuccess);

        StoryScene ending1 = new StoryScene(
            101,
            "Ending 1: Unharmed",
            "You never carved your name into the tree. You turn around and leave the forest completely intact. You win!",
            new string[] { "1. Play again from the beginning", "2. Quit game" },
            new int[] { -1, 999 }
        );
        keyValuePairs.Add(101, ending1);

        StoryScene ending2 = new StoryScene(
            102,
            "Ending 2: Pardoned",
            "The Tree of Wisdom accepts your sincere plea for forgiveness. 'Thank you, " + playerName + ". Your answer was pardoned. I forgive you.' The ancient guardian says your name and suddenly you remember everything, lifting the curse.",
            new string[] { "1. Play again from the beginning", "2. Quit game" },
            new int[] { -1, 999 }
        );
        keyValuePairs.Add(102, ending2);

        StoryScene ending3 = new StoryScene(
            103,
            "Ending 3: Crushed",
            "The giant tree lashes out with heavy roots, ending your journey abruptly.",
            new string[] { "1. Loop back", "2. Quit game" },
            new int[] { 0, 999 }
        );
        keyValuePairs.Add(103, ending3);

        StoryScene ending4 = new StoryScene(
            104,
            "Ending 4: Entangled",
            "The tree rejects your apology and the curse continues.",
            new string[] { "1. Loop back", "2. Quit game" },
            new int[] { 0, 999 }
        );
        keyValuePairs.Add(104, ending4);

        StoryScene ending5 = new StoryScene(
            105,
            "Ending 5: Transformed",
            "The witch cackles wildly as green smoke billows around you, turning you permanently into a decorative toad on her mantelpiece.",
            new string[] { "1. Loop back", "2. Quit game" },
            new int[] { 0, 999 }
        );
        keyValuePairs.Add(105, ending5);

        StoryScene ending6 = new StoryScene(
            106,
            "Ending 6: Consumed",
            "The bubbling brew floods your senses with terrifying visions of your past crimes before darkness claims you once again.",
            new string[] { "1. Loop back", "2. Quit game" },
            new int[] { 0, 999 }
        );
        keyValuePairs.Add(106, ending6);

        StoryScene ending7 = new StoryScene(
            107,
            "Ending 7: Ash",
            "Before your fingers even touch the wood, a powerful hex blasts you backward, reducing you to ash.",
            new string[] { "1. Play again from the beginning", "2. Quit game" },
            new int[] { -1, 999 }
        );
        keyValuePairs.Add(107, ending7);

        StoryScene ending8 = new StoryScene(
            108,
            "Ending 8: Repelled",
            "The merchant fights back but he overpowers your blade to pierce yourself.",
            new string[] { "1. Play again from the beginning", "2. Quit game" },
            new int[] { -1, 999 }
        );
        keyValuePairs.Add(108, ending8);

        StoryScene ending9 = new StoryScene(
            109,
            "Ending 9: Trapped Assistant",
            "The merchant recoils in horror from the cursed iron, slamming the heavy door shut as his cabin wards spring to life, trapping you outside in the dark as his unwanted assistant.",
            new string[] { "1. Play again from the beginning", "2. Quit game" },
            new int[] { -1, 999 }
        );
        keyValuePairs.Add(109, ending9);

        StoryScene ending10 = new StoryScene(
            110,
            "Ending 10: Shot Down",
            "The merchant draws a hidden, enchanted crossbow and fires instantly, ending your journey right on his threshold.",
            new string[] { "1. Play again from the beginning", "2. Quit game" },
            new int[] { -1, 999 }
        );
        keyValuePairs.Add(110, ending10);

        StoryScene ending12 = new StoryScene(
            112,
            "Ending 12: Shattered Loop",
            "The blade's weight and accumulated notches cause the metal to fracture and shatter entirely, breaking the cursed loop open and freeing you from the forest at last.",
            new string[] { "1. Play again from the beginning", "2. Quit game" },
            new int[] { -1, 999 }
        );
        keyValuePairs.Add(112, ending12);
    }

    public void GameStart()
    {
        int currentSceneID = -1;
        bool playing = true;

        while (playing)
        {
            Console.Clear();

            if (!keyValuePairs.ContainsKey(currentSceneID))
            {
                Console.WriteLine($"Error: Scene ID {currentSceneID} not found!");
                break;
            }

            UpdateSceneTexts();
            StoryScene currentScene = keyValuePairs[currentSceneID];

            List<int> availableIndices = new List<int>();
            for (int i = 0; i < currentScene.Choices.Length; i++)
            {
                if (currentSceneID == 1 && attemptedTreeChoices.Contains(i)) continue;
                if (currentSceneID == 2 && attemptedWitchChoices.Contains(i)) continue;
                if (currentSceneID == 3 && attemptedMerchantChoices.Contains(i)) continue;
                availableIndices.Add(i);
            }

            currentScene.Display(availableIndices);

            if (currentSceneID == -1)
            {
                Console.Write("Enter name to carve: ");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    currentSceneID = 101;
                }
                else
                {
                    playerName = input.Trim();
                    knifeNotches = 1;
                    attemptedTreeChoices.Clear();
                    attemptedWitchChoices.Clear();
                    attemptedMerchantChoices.Clear();

                    if (keyValuePairs.ContainsKey(102))
                    {
                        keyValuePairs[102].StoryText = "The Tree of Wisdom accepts your sincere plea for forgiveness. 'Thank you, " + playerName + ". Your answer was pardoned. I forgive you.' The ancient guardian says your name and suddenly you remember everything, lifting the curse.";
                    }
                    currentSceneID = currentScene.NextSceneIDs[0];
                }
            }
            else if (currentSceneID == 0)
            {
                Console.Write("\nYour choice: ");
                string input = Console.ReadLine()?.Trim();

                if (int.TryParse(input, out int choiceNumber) && choiceNumber >= 1 && choiceNumber <= 3)
                {
                    if (choiceNumber == 1) currentSceneID = 1;
                    else if (choiceNumber == 2) currentSceneID = 2;
                    else if (choiceNumber == 3) currentSceneID = 3;
                }
                else
                {
                    Console.WriteLine("Invalid choice. Press Enter.");
                    Console.ReadLine();
                }
            }
            else if (currentSceneID == 1 || currentSceneID == 2 || currentSceneID == 3)
            {
                if (availableIndices.Count == 0)
                {
                    Console.WriteLine("\nYou have exhausted all paths here. Press Enter to return to the clearing.");
                    Console.ReadLine();
                    currentSceneID = 0;
                    continue;
                }

                Console.Write("\nYour choice: ");
                string input = Console.ReadLine()?.Trim();

                if (int.TryParse(input, out int displayIndex) && displayIndex >= 1 && displayIndex <= availableIndices.Count)
                {
                    int originalChoiceIndex = availableIndices[displayIndex - 1];

                    if (currentSceneID == 1) attemptedTreeChoices.Add(originalChoiceIndex);
                    else if (currentSceneID == 2) attemptedWitchChoices.Add(originalChoiceIndex);
                    else if (currentSceneID == 3) attemptedMerchantChoices.Add(originalChoiceIndex);

                    currentSceneID = currentScene.NextSceneIDs[originalChoiceIndex];
                }
                else
                {
                    Console.WriteLine("Invalid choice. Press Enter.");
                    Console.ReadLine();
                }
            }
            else if (currentSceneID == 31)
            {
                bool success = RunAnvilMiniGame();
                if (success)
                {
                    currentSceneID = currentScene.NextSceneIDs[0];
                }
                else
                {
                    knifeNotches++;
                    if (knifeNotches >= 4)
                    {
                        currentSceneID = 112;
                    }
                    else
                    {
                        currentSceneID = 0;
                    }
                }
            }
            else if (currentSceneID == 4)
            {
                Console.Write("Your plea: ");
                string plea = Console.ReadLine()?.Trim() ?? string.Empty;

                bool containsSorry = plea.Contains("sorry", StringComparison.OrdinalIgnoreCase);
                bool containsAmNot = plea.Contains("am not", StringComparison.OrdinalIgnoreCase);

                if (containsSorry && !containsAmNot)
                {
                    currentSceneID = 102;
                }
                else
                {
                    currentSceneID = 104;
                }
            }
            else if (currentSceneID == 5)
            {
                Console.Write("Your answer: ");
                string answer = Console.ReadLine()?.Trim() ?? string.Empty;

                bool containsMemory = answer.Contains("memory", StringComparison.OrdinalIgnoreCase) ||
                                      answer.Contains("memories", StringComparison.OrdinalIgnoreCase);

                if (containsMemory)
                {
                    currentSceneID = 113;
                }
                else
                {
                    currentSceneID = 105;
                }
            }
            else
            {
                if (currentScene.Choices.Length == 0)
                {
                    Console.WriteLine("Press Enter to exit...");
                    Console.ReadLine();
                    break;
                }

                Console.Write("\nYour choice: ");
                string input = Console.ReadLine()?.Trim();

                if (int.TryParse(input, out int choiceNumber) && choiceNumber >= 1 && choiceNumber <= currentScene.Choices.Length)
                {
                    int targetID = currentScene.NextSceneIDs[choiceNumber - 1];

                    if (targetID == 0)
                    {
                        knifeNotches++;
                        if (knifeNotches >= 4)
                        {
                            targetID = 112;
                        }
                    }

                    if (targetID == 999)
                    {
                        Console.WriteLine("\nThanks for playing!");
                        playing = false;
                    }
                    else
                    {
                        currentSceneID = targetID;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid choice. Press Enter to try again.");
                    Console.ReadLine();
                }
            }
        }
    }

    private bool RunAnvilMiniGame()
    {
        Console.WriteLine("\n[MINI-GAME ACTIVE] Smash the [SPACEBAR] 25 times within 10 seconds!");
        Console.WriteLine("Press Enter when you are ready to start...");
        Console.ReadLine();

        int presses = 0;
        DateTime startTime = DateTime.Now;
        TimeSpan timeLimit = TimeSpan.FromSeconds(10);

        while (Console.KeyAvailable) Console.ReadKey(true);

        while ((DateTime.Now - startTime) < timeLimit)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Spacebar)
                {
                    presses++;
                    Console.Write($"\rPresses: {presses}/25 [Time left: {(int)(10 - (DateTime.Now - startTime).TotalSeconds)}s]   ");
                }
            }
        }

        Console.WriteLine();
        if (presses >= 25)
        {
            Console.WriteLine("Success! You hoisted the heavy iron anvil into the corner.");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            return true;
        }
        else
        {
            Console.WriteLine($"You only managed {presses} presses. The anvil slips and you collapse in exhaustion!");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            return false;
        }
    }
}