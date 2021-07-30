namespace Locator
{
    using System;
    using Vintagestory.API.Client;
    using Vintagestory.API.Common;
    using Vintagestory.API.Config;
    using Vintagestory.API.Server;
    using Vintagestory.API.Util;
    using System.Text;
    using Vintagestory.API.MathTools;
    using Cairo;
#pragma warning disable IDE0005
    using System.Linq;
#pragma warning restore IDE0005

    public abstract class ItemBaseLocator : Item
    {
        private SkillItem[] toolModes;

        protected abstract int GetConeRange(ModConfig.ModConfig modConfig);
        protected abstract int GetConeCost(ModConfig.ModConfig modConfig);
        protected abstract int GetSmallCubeRange(ModConfig.ModConfig modConfig);
        protected abstract int GetSmallCubeCost(ModConfig.ModConfig modConfig);
        protected abstract int GetMediumCubeRange(ModConfig.ModConfig modConfig);
        protected abstract int GetMediumCubeCost(ModConfig.ModConfig modConfig);
        protected abstract int GetLargeCubeRange(ModConfig.ModConfig modConfig);
        protected abstract int GetLargeCubeCost(ModConfig.ModConfig modConfig);
        protected abstract bool IsSearchedBlock(Block block);
        protected abstract void PrintCubeSearchResults(int count, int range, ICoreClientAPI capi);
        protected abstract void PrintConeSearchResults(int count, int range, int direction, ICoreClientAPI capi);
        protected abstract bool ShouldSearchReturnEarly();
        protected abstract string GetFlavorText();


        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {

            if (byEntity is EntityPlayer)
            {
                if (blockSel != null && firstEvent)
                {
                    var range = 0;
                    var cost = 0;
                    var player = byEntity.World.PlayerByUid((byEntity as EntityPlayer).PlayerUID);
                    var toolMode = this.GetToolMode(slot, player, blockSel);
                    var modConfig = ModConfig.ModConfig.Current;

                    switch (toolMode)
                    {
                        case 0:
                            range = this.GetConeRange(modConfig);
                            cost = this.GetConeCost(modConfig);
                            break;
                        case 1:
                            range = this.GetSmallCubeRange(modConfig);
                            cost = this.GetSmallCubeCost(modConfig);
                            break;
                        case 2:
                            range = this.GetMediumCubeRange(modConfig);
                            cost = this.GetMediumCubeCost(modConfig);
                            break;
                        case 3:
                            range = this.GetLargeCubeRange(modConfig);
                            cost = this.GetLargeCubeCost(modConfig);
                            break;
                        default:
                            break;
                    }

                    if (byEntity.Api.Side == EnumAppSide.Server)
                    {
                        this.ApplyDurabilityDamageServer(cost, slot, byEntity);
                    }
                    else
                    {
                        switch (toolMode)
                        {
                            case 0:
                                this.ExecuteConeSearch(range, cost, slot, byEntity, blockSel);
                                break;
                            case 1:
                            case 2:
                            case 3:
                                this.ExecuteCubeSearch(range, cost, slot, byEntity, blockSel);
                                break;
                            default:
                                var capi = byEntity.Api as ICoreClientAPI;
                                capi.ShowChatMessage("Unsupported tool mode!");
                                break;
                        }
                        this.SpawnParticles(blockSel, byEntity);
                    }
                }
                handling = EnumHandHandling.PreventDefaultAction;
            }
        }

        public override void OnLoaded(ICoreAPI api)
        {
            this.toolModes = ObjectCacheUtil.GetOrCreate(api, "translocatorLocatorToolModes", () =>
            {
                SkillItem[] modes;

                modes = new SkillItem[4];
                modes[0] = new SkillItem() { Code = new AssetLocation("cone"), Name = Lang.Get("translocatorlocator:directionalmode") };
                modes[1] = new SkillItem() { Code = new AssetLocation("smallcube"), Name = Lang.Get("translocatorlocator:smallcubemode") };
                modes[2] = new SkillItem() { Code = new AssetLocation("mediumcube"), Name = Lang.Get("translocatorlocator:mediumcubemode") };
                modes[3] = new SkillItem() { Code = new AssetLocation("largecube"), Name = Lang.Get("translocatorlocator:largecubemode") };

                if (api is ICoreClientAPI capi)
                {
                    modes[0].WithIcon(capi, (cr, x, y, w, h, c) => DrawIcons(cr, x, y, w, h, c, 0));
                    modes[1].WithIcon(capi, (cr, x, y, w, h, c) => DrawIcons(cr, x, y, w, h, c, 1));
                    modes[2].WithIcon(capi, (cr, x, y, w, h, c) => DrawIcons(cr, x, y, w, h, c, 2));
                    modes[3].WithIcon(capi, (cr, x, y, w, h, c) => DrawIcons(cr, x, y, w, h, c, 3));
                }

                return modes;
            });
        }

        public override SkillItem[] GetToolModes(ItemSlot slot, IClientPlayer forPlayer, BlockSelection blockSel)
        {
            return this.toolModes;
        }

        public override int GetToolMode(ItemSlot slot, IPlayer byPlayer, BlockSelection blockSel)
        {
            return Math.Min(this.toolModes.Length - 1, slot.Itemstack.Attributes.GetInt("toolMode"));
        }

        public override void SetToolMode(ItemSlot slot, IPlayer byPlayer, BlockSelection blockSel, int toolMode)
        {
            slot.Itemstack.Attributes.SetInt("toolMode", toolMode);
        }

        public override void OnUnloaded(ICoreAPI api)
        {
            for (var i = 0; this.toolModes != null && i < this.toolModes.Length; i++)
            {
                this.toolModes[i]?.Dispose();
            }
        }

        private void ExecuteConeSearch(int range, int durabilityDamage, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel)
        {
            var direction = blockSel.Face.Index;
            var count = 0;

            var xMin = blockSel.Position.X;
            var xMax = blockSel.Position.X;
            var yMin = blockSel.Position.Y;
            var yMax = blockSel.Position.Y;
            var zMin = blockSel.Position.Z;
            var zMax = blockSel.Position.Z;

            for (var distance = 0; distance <= range && !(this.ShouldSearchReturnEarly() && count > 0); distance++)
            {
                var diameter = distance; // just for readability

                switch (direction)
                {
                    // North
                    case 0:
                        xMin = blockSel.Position.X - diameter;
                        xMax = blockSel.Position.X + diameter;
                        yMin = blockSel.Position.Y - diameter;
                        yMax = blockSel.Position.Y + diameter;
                        zMin = blockSel.Position.Z + distance;
                        zMax = blockSel.Position.Z + distance;
                        break;
                    // East
                    case 1:
                        xMin = blockSel.Position.X - distance;
                        xMax = blockSel.Position.X - distance;
                        yMin = blockSel.Position.Y - diameter;
                        yMax = blockSel.Position.Y + diameter;
                        zMin = blockSel.Position.Z - diameter;
                        zMax = blockSel.Position.Z + diameter;
                        break;
                    // South
                    case 2:
                        xMin = blockSel.Position.X - diameter;
                        xMax = blockSel.Position.X + diameter;
                        yMin = blockSel.Position.Y - diameter;
                        yMax = blockSel.Position.Y + diameter;
                        zMin = blockSel.Position.Z - distance;
                        zMax = blockSel.Position.Z - distance;
                        break;
                    // West
                    case 3:
                        xMin = blockSel.Position.X + distance;
                        xMax = blockSel.Position.X + distance;
                        yMin = blockSel.Position.Y - diameter;
                        yMax = blockSel.Position.Y + diameter;
                        zMin = blockSel.Position.Z - diameter;
                        zMax = blockSel.Position.Z + diameter;
                        break;
                    // Up
                    case 4:
                        xMin = blockSel.Position.X - diameter;
                        xMax = blockSel.Position.X + diameter;
                        yMin = blockSel.Position.Y - distance;
                        yMax = blockSel.Position.Y - distance;
                        zMin = blockSel.Position.Z - diameter;
                        zMax = blockSel.Position.Z + diameter;
                        break;
                    // Down
                    case 5:
                        xMin = blockSel.Position.X - diameter;
                        xMax = blockSel.Position.X + diameter;
                        yMin = blockSel.Position.Y + distance;
                        yMax = blockSel.Position.Y + distance;
                        zMin = blockSel.Position.Z - diameter;
                        zMax = blockSel.Position.Z + diameter;
                        break;
                    default:
                        break;
                }
                count += this.SearchCubeArea(byEntity.World, new BlockPos(xMin, yMin, zMin), new BlockPos(xMax, yMax, zMax));
            }

            var capi = byEntity.Api as ICoreClientAPI;
            this.PrintConeSearchResults(count, range, blockSel.Face.Index, capi);
            this.PlaySound(count > 0, byEntity);

            this.ApplyDurabilityDamageClient(durabilityDamage, slot, byEntity);
        }

        private int SearchCubeArea(IWorldAccessor world, BlockSelection blockSel, int range)
        {
            return this.SearchCubeArea(world,
                                                new BlockPos(blockSel.Position.X - range,
                                                            blockSel.Position.Y - range,
                                                            blockSel.Position.Z - range),
                                                new BlockPos(blockSel.Position.X + range,
                                                            blockSel.Position.Y + range,
                                                            blockSel.Position.Z + range));
        }

        protected int SearchCubeArea(IWorldAccessor world, BlockPos lowerCorner, BlockPos upperCorner)
        {
            var count = 0;
            world.BlockAccessor.SearchBlocks(lowerCorner, upperCorner, (block, pos) =>
            {
                if (this.IsSearchedBlock(block))
                {
                    count++;
                }
                return true;
            });
            return count;
        }

        private void ExecuteCubeSearch(int range, int durabilityDamage, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel)
        {
            var capi = byEntity.Api as ICoreClientAPI;
            var count = this.SearchCubeArea(byEntity.World, blockSel, range);

            this.PrintCubeSearchResults(count, range, capi);
            this.PlaySound(count > 0, byEntity);

            this.ApplyDurabilityDamageClient(durabilityDamage, slot, byEntity);
        }

        private void ApplyDurabilityDamageServer(int durabilityDamage, ItemSlot slot, EntityAgent byEntity)
        {
            if (slot != null && slot.Itemstack != null)
            {
                var capi = byEntity.Api as ICoreServerAPI;
                var world = capi.World as IWorldAccessor;
                var serverplayer = world.PlayerByUid((byEntity as EntityPlayer).PlayerUID) as IServerPlayer;

                slot.Itemstack.Collectible.DamageItem(world, byEntity, serverplayer.InventoryManager.ActiveHotbarSlot, durabilityDamage);
                slot.MarkDirty();
            }
        }

        private void ApplyDurabilityDamageClient(int durabilityDamage, ItemSlot slot, EntityAgent byEntity)
        {
            if (slot != null && slot.Itemstack != null)
            {
                var capi = byEntity.Api as ICoreClientAPI;
                var world = capi.World as IWorldAccessor;
                var player = world.PlayerByUid((byEntity as EntityPlayer).PlayerUID);

                slot.Itemstack.Collectible.DamageItem(world, byEntity, player.InventoryManager.ActiveHotbarSlot, durabilityDamage);
                slot.MarkDirty();
            }
        }

        private void SpawnParticles(BlockSelection blockSel, EntityAgent byEntity)
        {
            var byPlayer = (byEntity as EntityPlayer).Player;
            var pos = blockSel.Position.ToVec3d().Add(blockSel.HitPosition.ToVec3f().ToVec3d());
            byEntity.World.SpawnCubeParticles(blockSel.Position, pos, 0.5f, 8, 0.7f, byPlayer);
        }

        private void PlaySound(bool found, EntityAgent byEntity)
        {
            if (found)
            {
                this.PlaySoundFound(byEntity);
            }
            else
            {
                this.PlaySoundNotFound(byEntity);
            }
        }

        private void PlaySoundNotFound(EntityAgent byEntity)
        {
            var pos = byEntity.Pos;
            byEntity.World.PlaySoundAt(new AssetLocation("sounds/tool/padlock"), pos.X, pos.Y, pos.Z, null);
        }

        private void PlaySoundFound(EntityAgent byEntity)
        {
            var pos = byEntity.Pos;
            byEntity.World.PlaySoundAt(new AssetLocation("sounds/tool/reinforce"), pos.X, pos.Y, pos.Z, null);
        }

        public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);

            dsc.AppendLine("\n" + this.GetFlavorText());
        }

#pragma warning disable IDE0060
        private static void DrawIcons(Context cr, int x, int y, float width, float height, double[] rgba, int toolMode)
#pragma warning restore IDE0060
        {
            cr.SetSourceRGB(1D, 1D, 1D);
            switch (toolMode)
            {
                case 0:
                    cr.MoveTo(11, 24);
                    cr.LineTo(37, 10);
                    cr.LineTo(37, 38);
                    cr.LineTo(11, 24);
                    cr.Fill();
                    break;
                case 1:
                    cr.Rectangle(16, 16, 16, 16);
                    cr.Fill();
                    break;
                case 2:
                    cr.Rectangle(10, 10, 28, 28);
                    cr.Fill();
                    break;
                case 3:
                default:
                    cr.Rectangle(4, 4, 40, 40);
                    cr.Fill();
                    break;
            }
        }
    }
}
