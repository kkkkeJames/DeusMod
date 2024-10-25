using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace DeusMod.Dusts
{
    public class Gunfire_L : ModDust
    {
        public override string Texture => "DeusMod/Assets/Dusts/Gunfire_L";
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 22, 12);
            dust.noLight = false;
        }
        public override bool Update(Dust dust)
        {
            dust.fadeIn++;
            if (dust.fadeIn >= 2) dust.active = false; 
            if (!dust.noLight)
                Lighting.AddLight(dust.position, dust.color.ToVector3());
            return false;
        }
        public override bool PreDraw(Dust dust)
        {
            Texture2D BloomTex = ModContent.Request<Texture2D>("DeusMod/Assets/Projs/Bloom").Value;
            Color BloomColor = new Color(255, 217, 95);
            BloomColor.A = 0;
            Main.EntitySpriteDraw(BloomTex, dust.position - Main.screenPosition, null, BloomColor * 0.66f, 0, BloomTex.Size() * 0.5f, dust.scale, SpriteEffects.None);
            return base.PreDraw(dust);
        }
    }
}
