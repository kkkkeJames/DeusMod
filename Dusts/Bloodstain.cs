using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DeusMod.Dusts
{
    public class Bloodstain : ModDust
    {
        public override string Texture => "DeusMod/Assets/Dusts/Bloodstain";
        private float timer = 0;
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 42, 44);
            timer = 0;
        }
        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color col = Color.Lerp(Color.Red, Color.Black, timer / 68);
            return col * ((255 - dust.alpha) / 255f);
        }
        public override bool Update(Dust dust)
        {
            timer++;
            if (dust.color == Color.Black)
                dust.alpha += 1;
            if (dust.alpha >= 255)
            {
                dust.active = false; 
            }
            return false;
        }
    }
}
