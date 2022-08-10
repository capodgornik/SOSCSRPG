﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Engine.Factories;
using Engine.Models;
using Engine.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Engine.Services
{
    public static class SaveGameService
    {
        private const string SAVE_GAME_FILE_NAME = "SOSCRPG.json";

        public static void Save(GameSession gameSession)
        {
            File.WriteAllText(SAVE_GAME_FILE_NAME,
                              JsonConvert.SerializeObject(gameSession, Formatting.Indented));
        }

        public static GameSession LoadLastSaveOrCreateNew()
        {
            if (!File.Exists(SAVE_GAME_FILE_NAME))
            {
                return new GameSession();
            }
            
            // Save game file exists, so create the GameSession object from it.
            try
            {
                JObject data = JObject.Parse(File.ReadAllText(SAVE_GAME_FILE_NAME));
                // Populate Player object
                Player player = CreatePlayer(data);
                int x = (int)data[nameof(GameSession.CurrentLocation)][nameof(Location.XCoordinate)];
                int y = (int)data[nameof(GameSession.CurrentLocation)][nameof(Location.YCoordinate)];

                // Create GameSession object with saved game data
                return new GameSession(player, x, y);
            }

            catch (Exception ex)
            {
                // If there was an error loading/deserializing the saved game, 
                // create a brand new GameSession object.
                return new GameSession();
            }
        }

        private static Player CreatePlayer(JObject data)
        {
            string fileVersion = FileVersion(data);
            Player player;

            switch (fileVersion)
            {
                case "0.1.000":
                    player =
                        new Player((string)data[nameof(GameSession.CurrentPlayer)][nameof(Player.Name)],
                                   (string)data[nameof(GameSession.CurrentPlayer)][nameof(Player.CharacterClass)],
                                   (int)data[nameof(GameSession.CurrentPlayer)][nameof(Player.ExperiencePoints)],
                                   (int)data[nameof(GameSession.CurrentPlayer)][nameof(Player.MaximumHitPoints)],
                                   (int)data[nameof(GameSession.CurrentPlayer)][nameof(Player.CurrentHitPoints)],
                                   (int)data[nameof(GameSession.CurrentPlayer)][nameof(Player.Dexterity)],
                                   (int)data[nameof(GameSession.CurrentPlayer)][nameof(Player.Gold)]);
                    break;
                default:
                    throw new InvalidDataException($"File version '{fileVersion}' not recognized");
            }

            PopulatePlayerInventory(data, player);
            PopulatePlayerQuests(data, player);
            PopulatePlayerRecipes(data, player);

            return player;
        }

        private static void PopulatePlayerInventory(JObject data, Player player)
        {
            string fileVersion = FileVersion(data);

            switch (fileVersion)
            {
                case "0.1.000":
                    foreach (JToken itemToken in (JArray)data[nameof(GameSession.CurrentPlayer)]
                        [nameof(Player.Inventory)]
                        [nameof(Inventory.Items)])
                    {
                        int itemId = (int)itemToken[nameof(GameItem.ItemTypeID)];
                        player.AddItemToInventory(ItemFactory.CreateGameItem(itemId));
                    }
                    break;
                default:
                    throw new InvalidDataException($"File version '{fileVersion}' not recognized");
            }
        }

        private static void PopulatePlayerQuests(JObject data, Player player)
        {
            string fileVersion = FileVersion(data);

            switch (fileVersion)
            {
                case "0.1.000":
                    foreach (JToken questToken in (JArray)data[nameof(GameSession.CurrentPlayer)]
                        [nameof(Player.Quests)])
                    {
                        int questId =
                            (int)questToken[nameof(QuestStatus.PlayerQuest)][nameof(QuestStatus.PlayerQuest.ID)];
                        Quest quest = QuestFactory.GetQuestByID(questId);
                        QuestStatus questStatus = new QuestStatus(quest);
                        questStatus.IsCompleted = (bool)questToken[nameof(QuestStatus.IsCompleted)];
                        player.Quests.Add(questStatus);
                    }
                    break;
                default:
                    throw new InvalidDataException($"File version '{fileVersion}' not recognized");
            }
        }

        private static void PopulatePlayerRecipes(JObject data, Player player)
        {
            string fileVersion = FileVersion(data);

            switch (fileVersion)
            {
                case "0.1.000":
                    foreach (JToken recipeToken in
                        (JArray)data[nameof(GameSession.CurrentPlayer)][nameof(Player.Recipes)])
                    {
                        int recipeId = (int)recipeToken[nameof(Recipe.ID)];
                        Recipe recipe = RecipeFactory.RecipeById(recipeId);
                        player.Recipes.Add(recipe);
                    }
                    break;
                default:
                    throw new InvalidDataException($"File version '{fileVersion}' not recognized");
            }
        }

        private static string FileVersion(JObject data)
        {
            return (string)data[nameof(GameSession.Version)];
        }
    }
}
