using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace DeusMod.Items.Accessories
{
    public class ShiningGem : ModItem
    {
        public override string Texture => "DeusMod/Assets/Items/Accessories/ShiningGem";
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Shining Gem");
            //DisplayName.AddTranslation(7, "璀璨宝石");
            //Tooltip.SetDefault("Increase 20 life max\n'Why finding diamonds? Boredom kills me...'");
            //Tooltip.AddTranslation(7, "增加20最大生命上限\n“为啥找钻石？因为我要无聊死了...”");
        }
        // 物品基本信息设置
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.accessory = true;
            Item.defense = 2;
            Item.rare = 1;
            Item.value = Item.sellPrice(0, 0, 30, 0);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statLifeMax2 += 20;
        }
        // 物品合成方式设置
        public override void AddRecipes()
        {
            var recipe = CreateRecipe(); 
            recipe.AddIngredient(ItemID.GoldBar, 2);
            recipe.AddIngredient(ItemID.Diamond, 4);
            recipe.AddIngredient(ItemID.Ruby, 4);
            recipe.AddTile(TileID.Tables);
            recipe.AddTile(TileID.Chairs);
            recipe.Register();

            var recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.PlatinumBar, 2);
            recipe2.AddIngredient(ItemID.Diamond, 4);
            recipe2.AddIngredient(ItemID.Ruby, 4);
            recipe2.AddTile(TileID.Tables);
            recipe2.AddTile(TileID.Chairs);
            recipe2.Register();
        }
    }
}