namespace TranslocatorLocator.ModSystem
{
    using Vintagestory.API.Common;
    using Vintagestory.API.Server;
    using ModConfig;

    public class TranslocatorLocatorSystem : ModSystem
    {

        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return true;
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            api.Logger.Debug("[TranslocatorLocator] StartServer");
            base.Start(api);

            ModConfig.Load(api);
        }

        public override void Start(ICoreAPI api)
        {
            api.Logger.Debug("[TranslocatorLocator] Start");
            base.Start(api);

            api.RegisterItemClass("ItemTranslocatorLocator", typeof(ItemTranslocatorLocator));
            api.RegisterItemClass("ItemAgedWoodLocator", typeof(ItemAgedWoodLocator));
        }
    }
}
