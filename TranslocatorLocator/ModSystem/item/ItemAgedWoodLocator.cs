namespace TranslocatorLocator
{
    using Locator;
    using Vintagestory.API.Client;
    using Vintagestory.API.Common;
    using Vintagestory.API.Config;
    using Vintagestory.GameContent;

    public class ItemAgedWoodLocator : ItemBaseLocator
    {
        protected override int GetConeRange(ModConfig.ModConfig modConfig)
        {
            return modConfig.AgedWoodLocatorConeRange;
        }
        protected override int GetConeCost(ModConfig.ModConfig modConfig)
        {
            return modConfig.AgedWoodLocatorConeCost;
        }
        protected override int GetSmallCubeRange(ModConfig.ModConfig modConfig)
        {
            return modConfig.AgedWoodLocatorCubeSmallRange;
        }
        protected override int GetSmallCubeCost(ModConfig.ModConfig modConfig)
        {
            return modConfig.AgedWoodLocatorCubeSmallCost;
        }
        protected override int GetMediumCubeRange(ModConfig.ModConfig modConfig)
        {
            return modConfig.AgedWoodLocatorCubeMediumRange;
        }
        protected override int GetMediumCubeCost(ModConfig.ModConfig modConfig)
        {
            return modConfig.AgedWoodLocatorCubeMediumCost;
        }
        protected override int GetLargeCubeRange(ModConfig.ModConfig modConfig)
        {
            return modConfig.AgedWoodLocatorCubeLargeRange;
        }
        protected override int GetLargeCubeCost(ModConfig.ModConfig modConfig)
        {
            return modConfig.AgedWoodLocatorCubeLargeCost;
        }
        protected override bool IsSearchedBlock(Block block)
        {
            // class check of the block seems to be way faster than checking the code string

            return (
                // early returns
                !(block is BlockSoil) &&
                !block.BlockMaterial.Equals(EnumBlockMaterial.Gravel) &&
                !block.BlockMaterial.Equals(EnumBlockMaterial.Stone) &&
                !block.BlockMaterial.Equals(EnumBlockMaterial.Sand) &&
                  // checks
                  ((block is BlockBed bedBlock && bedBlock.Code.Path.Contains("woodaged"))
                || (block is BlockTapestry)
                || (block is BlockLog logBlock && logBlock.Code.Path.Contains("aged"))
                || (block is BlockContainer containerBlock && containerBlock.Code.Path.Contains("collapsed"))
                || (block.BlockMaterial.Equals(EnumBlockMaterial.Wood) && block.Code.Path.Contains("planks-aged"))
                ));
        }

        protected override void PrintCubeSearchResults(int count, int range, ICoreClientAPI capi)
        {
            string message;
            if (count < 1)
            {
                message = Lang.Get("translocatorlocator:agedwood_cubefoundnothing");
            }
            else
            {
                message = Lang.Get("translocatorlocator:agedwood_cubefoundsomething");
            }
            capi.ShowChatMessage(message.Replace("#range", "" + range));
        }

        protected override void PrintConeSearchResults(int count, int range, int direction, ICoreClientAPI capi)
        {
            string adjustedDirection;
            switch (direction)
            {
                // wording in opposite direction of face clicked
                case 0:
                    adjustedDirection = Lang.Get("translocatorlocator:agedwood_southofthatblock");
                    break;
                case 1:
                    adjustedDirection = Lang.Get("translocatorlocator:agedwood_westofthatblock");
                    break;
                case 2:
                    adjustedDirection = Lang.Get("translocatorlocator:agedwood_northofthatblock");
                    break;
                case 3:
                    adjustedDirection = Lang.Get("translocatorlocator:agedwood_eastofthatblock");
                    break;
                case 4:
                    adjustedDirection = Lang.Get("translocatorlocator:agedwood_belowthatblock");
                    break;
                case 5:
                    adjustedDirection = Lang.Get("translocatorlocator:agedwood_abovethatblock");
                    break;
                default:
                    adjustedDirection = Lang.Get("translocatorlocator:agedwood_somewherearoundthatblock");
                    break;
            }
            string message;
            if (count < 1)
            {
                message = Lang.Get("translocatorlocator:agedwood_conefoundnothing");
            }
            else
            {
                message = Lang.Get("translocatorlocator:agedwood_conefoundsomething");
            }
            capi.ShowChatMessage(message.Replace("#range", "" + range).Replace("#direction", adjustedDirection));
        }
        protected override string GetFlavorText()
        {
            return Lang.Get("translocatorlocator:agedwood_flavortext");
        }

        protected override bool ShouldSearchReturnEarly()
        {
            return true;
        }
    }
}

