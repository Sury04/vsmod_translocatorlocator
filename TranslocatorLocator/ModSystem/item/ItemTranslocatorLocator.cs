namespace TranslocatorLocator
{
    using System.Globalization;
    using TranslocatorLocator.ModSystem.item;
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
            //var noOfTLs = (count > 0) ? "" + count : Lang.Get("translocatorlocator:translocator_no");
            var message = count switch
            {
                0 => Lang.Get("translocatorlocator:translocator_cubenofound"),
                1 => Lang.Get("translocatorlocator:translocator_cubefoundlessthantwo"),
                _ => Lang.Get("translocatorlocator:translocator_cubefoundmorethanone"),
            };

            capi.ShowChatMessage(message.Replace("#no", count.ToString(CultureInfo.InvariantCulture)).Replace("#range", "" + range));
        }

        protected override void PrintConeSearchResults(int count, int range, int direction, ICoreClientAPI capi)
        {
            var adjustedDirection = direction switch
            {
                // wording in opposite direction of face clicked
                0 => Lang.Get("translocatorlocator:translocator_southofthatblock"),
                1 => Lang.Get("translocatorlocator:translocator_westofthatblock"),
                2 => Lang.Get("translocatorlocator:translocator_northofthatblock"),
                3 => Lang.Get("translocatorlocator:translocator_eastofthatblock"),
                4 => Lang.Get("translocatorlocator:translocator_belowthatblock"),
                5 => Lang.Get("translocatorlocator:translocator_abovethatblock"),
                _ => Lang.Get("translocatorlocator:translocator_somewherearoundthatblock"),
            };

            var message = count switch
            {
                0 => Lang.Get("translocatorlocator:translocator_conenofound"),
                1 => Lang.Get("translocatorlocator:translocator_conefoundlessthantwo"),
                _ => Lang.Get("translocatorlocator:translocator_conefoundmorethanone"),
            };

            capi.ShowChatMessage(message.Replace("#no", count.ToString(CultureInfo.InvariantCulture)).Replace("#range", "" + range).Replace("#direction", adjustedDirection));
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

