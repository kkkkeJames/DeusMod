using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace DeusMod.Items.Accessories
{
    public class GlobalShield : GlobalItem
    {
        // Shields can be categorized into three kinds
        // Blocking shield provides extra defense, and can hold to increase defense, blocking accurately can parry projectiles
        // Dashing shield allows player to dash, they can rather dash, dash and slam, or dash and bonk (including tabi/masterninjagear)
        // Other shields can both block and dash, or cannot do either but only provide stats
        // A player can only equip one shield in the accessory slot
        public static bool[] AccessoryShield = ItemID.Sets.Factory.CreateBoolSet(ItemID.EoCShield, ItemID.CobaltShield, ItemID.ObsidianShield, ItemID.AnkhShield, ItemID.FrozenShield, ItemID.HeroShield, ItemID.PaladinsShield, ItemID.Tabi, ItemID.MasterNinjaGear, ModContent.ItemType<WoodenShield>());
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return AccessoryShield[entity.type];
        }
        public override void SetDefaults(Item entity)
        {
            base.SetDefaults(entity);

        }
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            base.UpdateAccessory(item, player, hideVisual);
            player.GetModPlayer<DeusPlayer>().ShieldEquipped = true;
        }
        public override bool CanEquipAccessory(Item item, Player player, int slot, bool modded)
        {
            if (player.GetModPlayer<DeusPlayer>().ShieldEquipped) return false;
            return base.CanEquipAccessory(item, player, slot, modded);
        }
    }
}
