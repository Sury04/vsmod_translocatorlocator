namespace TranslocatorLocator
{
    using Locator;
    using Vintagestory.API.Client;
    using Vintagestory.API.Common;
    using Vintagestory.API.Config;
    using Vintagestory.GameContent;

    public class ItemTranslocatorLocator : ItemBaseLocator
    {
        protected override int GetConeRange(ModConfig.ModConfig modConfig)
        {
            return modConfig.TranslocatorLocatorConeRange;
        }
        protected override int GetConeCost(ModConfig.ModConfig modConfig)
        {
            return modConfig.TranslocatorLocatorConeCost;
        }
        protected override int GetSmallCubeRange(ModConfig.ModConfig modConfig)
        {
            return modConfig.TranslocatorLocatorCubeSmallRange;
        }
        protected override int GetSmallCubeCost(ModConfig.ModConfig modConfig)
        {
            return modConfig.TranslocatorLocatorCubeSmallCost;
        }
        protected override int GetMediumCubeRange(ModConfig.ModConfig modConfig)
        {
            return modConfig.TranslocatorLocatorCubeMediumRange;
        }
        protected override int GetMediumCubeCost(ModConfig.ModConfig modConfig)
        {
            return modConfig.TranslocatorLocatorCubeMediumCost;
        }
        protected override int GetLargeCubeRange(ModConfig.ModConfig modConfig)
        {
            return modConfig.TranslocatorLocatorCubeLargeRange;
        }
        protected override int GetLargeCubeCost(ModConfig.ModConfig modConfig)
        {
            return modConfig.TranslocatorLocatorCubeLargeCost;
        }

        protected override bool IsSearchedBlock(Block block)
        {
            return block is BlockStaticTranslocator transBlock && !transBlock.Repaired;
        }

        protected override void PrintCubeSearchResults(int count, int range, ICoreClientAPI capi)
        {
            var noOfTLs = (count > 0) ? "" + count : Lang.Get("translocatorlocator:translocator_no");
            string message;
            if (count <= 1)
            {
                message = Lang.Get("translocatorlocator:translocator_cubefoundlessthantwo");
            }
            else
            {
                message = Lang.Get("translocatorlocator:translocator_cubefoundmorethanone");
            }
            capi.ShowChatMessage(message.Replace("#no", noOfTLs).Replace("#range", "" + range));
        }

        protected override void PrintConeSearchResults(int count, int range, int direction, ICoreClientAPI capi)
        {
            string adjustedDirection;
            switch (direction)
            {
                // wording in opposite direction of face clicked
                case 0:
                    adjustedDirection = Lang.Get("translocatorlocator:translocator_southofthatblock");
                    break;
                case 1:
                    adjustedDirection = Lang.Get("translocatorlocator:translocator_westofthatblock");
                    break;
                case 2:
                    adjustedDirection = Lang.Get("translocatorlocator:translocator_northofthatblock");
                    break;
                case 3:
                    adjustedDirection = Lang.Get("translocatorlocator:translocator_eastofthatblock");
                    break;
                case 4:
                    adjustedDirection = Lang.Get("translocatorlocator:translocator_belowthatblock");
                    break;
                case 5:
                    adjustedDirection = Lang.Get("translocatorlocator:translocator_abovethatblock");
                    break;
                default:
                    adjustedDirection = Lang.Get("translocatorlocator:translocator_somewherearoundthatblock");
                    break;
            }
            var noOfTLs = (count > 0) ? "" + count : Lang.Get("translocatorlocator:translocator_no");
            string message;
            if (count <= 1)
            {
                message = Lang.Get("translocatorlocator:translocator_conefoundlessthantwo");
            }
            else
            {
                message = Lang.Get("translocatorlocator:translocator_conefoundmorethanone");
            }
            capi.ShowChatMessage(message.Replace("#no", noOfTLs).Replace("#range", "" + range).Replace("#direction", adjustedDirection));
        }

        protected override string GetFlavorText()
        {
            return Lang.Get("translocatorlocator:translocator_flavortext");
        }

        protected override bool ShouldSearchReturnEarly()
        {
            return false;
        }
    }
}

