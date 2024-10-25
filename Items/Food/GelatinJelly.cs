using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DeusMod.Items.Food
{
    public class GelatinJelly : ModItem
    {
        public override string Texture => "DeusMod/Assets/Items/Food/GelatinJelly";
        public override void SetStaticDefaults()
        {
            
        }
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 14;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.value = Item.sellPrice(0, 0, 0, 36);
            Item.rare = ItemRarityID.White;
            Item.useStyle = ItemUseStyleID.EatFood;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Gel, 6)
                .AddIngredient(ItemID.FoodPlatter, 1)
                .AddTile(TileID.Pots)
                .Register();
        }
        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item2);
            player.AddBuff(BuffID.WellFed, 18000);
            player.GetItem(Main.myPlayer, new Item(ItemID.FoodPlatter), GetItemSettings.ItemCreatedFromItemUsage);
            return true;
        }
    }
}
