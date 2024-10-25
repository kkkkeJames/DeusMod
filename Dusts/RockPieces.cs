using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DeusMod.Dusts
{
    public class RockPieces : ModDust
    {
        public override string Texture => "DeusMod/Assets/Dusts/RockPieces";
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = false;
            dust.frame = new Rectangle(0, 0, 32, 28);
        }
        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.velocity.Y += 0.2f;
            if(dust.velocity.Y > 0)
                dust.alpha += 5;
            if (dust.alpha >= 255)
                dust.active = false;
            if (dust.rotation > 0) 
                dust.rotation += 0.01f;
            else dust.rotation -= 0.01f;
            return false;
        }
    }
}
