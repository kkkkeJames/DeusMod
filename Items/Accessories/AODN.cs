using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using DeusMod.Core.Systems;

namespace DeusMod.Items.Accessories
{
    public class AODN : ModItem
    {
        public override string Texture => "DeusMod/Assets/Items/Accessories/AODN";
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Apocalypse Of Dreary Night");
            //DisplayName.AddTranslation(7, "诡月厄劫");
            //Tooltip.SetDefault("When picking up hearts or hit, increase speed for some time\n'Ugh...'");
            //Tooltip.AddTranslation(7, "当拾取心或者被攻击后在一段时间内增加移动速度\n‘呃...’");
        }
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = 1;
            Item.value = Item.buyPrice(0, 1, 0, 0);
        }
        public float timer = 0f;
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if(timer <= 180) timer++;
            if (timer == 1)
            {
                string text = "...Arrrrrrrrgh!";
                CombatText.NewText(new Rectangle((int)player.Center.X, (int)player.Center.Y, player.width, player.height), Color.Red, Language.GetTextValue(text), false, false);
            }
            if(timer <= 60)
            {
                Main.GameZoomTarget = 1 + 0.01f * timer;
            }
            if (timer <= 120)
            {
                player.GetModPlayer<ScreenShake>().ScreenShakeContinue(60, 16);
                if (!Filters.Scene["DarkDepress"].IsActive())
                {
                    Filters.Scene.Activate("DarkDepress");
                }
            }
            else
            {
                if (Filters.Scene["DarkDepress"].IsActive())
                {
                    Filters.Scene.Deactivate("DarkDepress");
                }
            }
            if (timer > 120 && timer <= 180)
            {
                Main.GameZoomTarget = 1.6f - 0.01f * (timer - 120);
            }
            /*if (DeusPlayer.AODNbuff)
            {
                player.moveSpeed += 1.5f;
                player.jumpSpeedBoost += 1.5f;
            }*/
        }
    }
}
