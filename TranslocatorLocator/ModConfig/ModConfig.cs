#pragma warning disable IDE1006 // Naming Styles
namespace ModConfig
{
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;
    using Vintagestory.API.Common;
    using Vintagestory.API.Config;

    public class ModConfig
    {
        public static ModConfig Current { get; set; }

        // translocators
        public string TranslocatorLocatorConeRangeDesc = "The range of the directional mode in blocks.";
        public int TranslocatorLocatorConeRange = 20;
        public string TranslocatorLocatorConeCostDesc = "The durability cost of the directional mode.";
        public int TranslocatorLocatorConeCost = 1;

        public string TranslocatorLocatorCubeSmallRangeDesc = "The radius of the short range mode in blocks.";
        public int TranslocatorLocatorCubeSmallRange = 20;
        public string TranslocatorLocatorCubeSmallCostDesc = "The durability cost of the short range mode.";
        public int TranslocatorLocatorCubeSmallCost = 1;

        public string TranslocatorLocatorCubeMediumRangeDesc = "The radius of the medium range mode in blocks.";
        public int TranslocatorLocatorCubeMediumRange = 50;
        public string TranslocatorLocatorCubeMediumCostDesc = "The durability cost of the medium range mode.";
        public int TranslocatorLocatorCubeMediumCost = 5;

        public string TranslocatorLocatorCubeLargeRangeDesc = "The radius of the long range mode in blocks.";
        public int TranslocatorLocatorCubeLargeRange = 100;
        public string TranslocatorLocatorCubeLargeCostDesc = "The durability cost of the long range mode.";
        public int TranslocatorLocatorCubeLargeCost = 20;

        // aged wood items
        public string AgedWoodLocatorConeRangeDesc = "The range of the directional mode in blocks.";
        public int AgedWoodLocatorConeRange = 20;
        public string AgedWoodLocatorConeCostDesc = "The durability cost of the directional mode.";
        public int AgedWoodLocatorConeCost = 1;

        public string AgedWoodLocatorCubeSmallRangeDesc = "The radius of the short range mode in blocks.";
        public int AgedWoodLocatorCubeSmallRange = 20;
        public string AgedWoodLocatorCubeSmallCostDesc = "The durability cost of the short range mode.";
        public int AgedWoodLocatorCubeSmallCost = 1;

        public string AgedWoodLocatorCubeMediumRangeDesc = "The radius of the medium range mode in blocks.";
        public int AgedWoodLocatorCubeMediumRange = 50;
        public string AgedWoodLocatorCubeMediumCostDesc = "The durability cost of the medium range mode.";
        public int AgedWoodLocatorCubeMediumCost = 5;

        public string AgedWoodLocatorCubeLargeRangeDesc = "The radius of the long range mode in blocks.";
        public int AgedWoodLocatorCubeLargeRange = 100;
        public string AgedWoodLocatorCubeLargeCostDesc = "The durability cost of the long range mode.";
        public int AgedWoodLocatorCubeLargeCost = 20;

        public static string filename = "TranslocatorLocator.json";
        public static void Load(ICoreAPI api)
        {
            ModConfig config = null;
            var logname = "translocatorlocator-mod-logs.txt";

            try
            {
                for (var attempts = 1; attempts < 4; attempts++)
                {
                    try
                    {
                        config = api.LoadModConfig<ModConfig>(filename);
                    }
                    catch (JsonReaderException e)
                    {
                        var badLineNum = e.LineNumber;
                        api.Logger.Error($"[TranslocatorLocatorMod Error] Unable to parse config JSON. Attempt {attempts} to salvage the file...");
                        var configFilepath = Path.Combine(GamePaths.ModConfig, filename);
                        var badConfigFilepath = Path.Combine(GamePaths.Logs, "ERROR_" + filename);
                        var translocatorlocatorLogFilepath = Path.Combine(GamePaths.Logs, logname);
                        if (attempts == 1)
                        {
                            if (File.Exists(badConfigFilepath))
                            {
                                File.Delete(badConfigFilepath);
                            }
                            File.Copy(configFilepath, badConfigFilepath);
                            File.WriteAllText(translocatorlocatorLogFilepath, e.ToString());
                        }
                        if (attempts != 3)
                        {
                            var lines = new List<string>(File.ReadAllLines(configFilepath));
                            lines.RemoveAt(badLineNum - 1);
                            File.WriteAllText(configFilepath, string.Join("\n", lines.ToArray()));
                        }
                    }
                }
                try
                {
                    config = api.LoadModConfig<ModConfig>(filename);
                }
                catch (JsonReaderException)
                {
                    api.Logger.Error("[TranslocatorLocatorMod Error] Unable to salvage config.");
                }
            }
            catch (System.Exception e)
            {
                api.Logger.Error("[TranslocatorLocatorMod Error] Something went really wrong with reading the config file.");
                File.WriteAllText(Path.Combine(GamePaths.Logs, logname), e.ToString());
            }

            if (config == null)
            {
                api.Logger.Warning("[TranslocatorLocatorMod Warning] Unable to load valid config file. Generating default config.");
                config = new ModConfig();
            }
            Save(api, config);
            Current = config;
        }
        public static void Save(ICoreAPI api, ModConfig config)
        {
            api.StoreModConfig(config, filename);
        }
    }
}

