using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DeusMod.Helpers;
using Terraria.Audio;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using DeusMod.Items.Weapon.Melee.Scythes;

namespace DeusMod.Items.Weapon.Melee.Swords
{
    public class DeusGlobalSword : GlobalItem
    {
        public static bool[] ItemIsSword = ItemID.Sets.Factory.CreateBoolSet(ItemID.CopperBroadsword, ItemID.TinBroadsword, ItemID.IronBroadsword, ItemID.LeadBroadsword, ItemID.SilverBroadsword, ItemID.TungstenBroadsword, ItemID.GoldBroadsword, ItemID.PlatinumBroadsword, ItemID.EnchantedSword, ItemID.LightsBane, ItemID.BloodButcherer, ItemID.BladeofGrass, ItemID.FieryGreatsword, ItemID.Starfury, ItemID.Muramasa,
            ItemID.CopperShortsword, ItemID.TinShortsword, ItemID.IronShortsword, ItemID.LeadShortsword, ItemID.SilverShortsword, ItemID.TungstenShortsword, ItemID.GoldShortsword, ItemID.PlatinumShortsword,
            ItemID.Spear, ItemID.Trident,
            ModContent.ItemType<ScytheOfRetribution>()
            );
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return ItemIsSword[entity.type];
        }
        public override void SetDefaults(Item entity)
        {
            //SoundStyle[] SwordSound = new SoundStyle[3]{DeusModSoundHelper.Sword_Wield, DeusModSoundHelper.Sword_Wield2, DeusModSoundHelper.Sword_Wield3 };
            //entity.UseSound = SwordSound[Main.rand.Next(3)]; //先让每一把武器的使用音效都是mod的
            entity.UseSound = null; //ban掉近战武器的使用音效，在各自的AI里写
            base.SetDefaults(entity);
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //SoundStyle[] SwordSound = new SoundStyle[3] { DeusModSoundHelper.Sword_Wield, DeusModSoundHelper.Sword_Wield2, DeusModSoundHelper.Sword_Wield3 };
            //item.UseSound = SwordSound[Main.rand.Next(3)];
            item.UseSound = null;
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }
    }
}
