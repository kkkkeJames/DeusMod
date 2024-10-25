using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DeusMod.Projs.Melee;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using System.Collections.Generic;
using DeusMod.PlayerControl;

namespace DeusMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class WoodenShield : ModItem
    {
        public override string Texture => "DeusMod/Assets/Items/Accessories/WoodenShield";
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Wooden Shield");
            //DisplayName.AddTranslation(7, "木盾");
            //Tooltip.SetDefault("Press G to block, increasing 2 defense\nBlock right before being hurt could be completely immune to it and cancel the backswing");
            //Tooltip.AddTranslation(7, "按下G键举盾防御，提升2防御力\n在将要受到攻击之前防御可以直接免疫此次攻击并取消防御的后摇");
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.accessory = true;
            Item.defense = 1;
            Item.rare = 0;
            Item.value = Item.sellPrice(0, 0, 30, 0);
        }
        public override void AddRecipes()
        {
            var recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Wood, 8);
            recipe.AddIngredient(ItemID.IronBar, 4);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();

            var recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.Wood, 8);
            recipe2.AddIngredient(ItemID.LeadBar, 4);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<DeusPlayerBlock>().HasBlockShield = true;
            if (player.GetModPlayer<DeusPlayerBlock>().IsBlock)
            {
                player.statDefense += 2;
            }
            if (player.GetModPlayer<DeusPlayerBlock>().IsBlock && player.ownedProjectileCounts[ModContent.ProjectileType<WoodenShieldBlock>()] == 0)
            {
                Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<WoodenShieldBlock>(), 0, 0, player.whoAmI);
            }
        }

    }
    public class WoodenShieldBlock : ShieldBlockProjectile
    {
        public override string Texture => "DeusMod/Assets/Items/Accessories/WoodenShield_Block";
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 24;
            base.SetDefaults();
        }
    }
}
