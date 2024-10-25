using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DeusMod.Projs.Melee.SkillUI
{
    public class MeleeSkill_Accumulate : ModProjectile
    {
        public override string Texture => "DeusMod/Assets/Projs/MeleeSkillUI/MeleeSkill_Accumulate";
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 30;
            Projectile.alpha = 255;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.ai[0]++;
            if (Projectile.ai[0] <= 5)
            {
                Projectile.Center = player.Center + new Vector2(-10 + Projectile.ai[0] * 2, -40);
                Projectile.alpha -= 51;
            }
            else if (Projectile.ai[0] <= 25)
            {
                Projectile.Center = player.Center + new Vector2(0, -40);
            }
            else
            {
                Projectile.Center = player.Center + new Vector2((Projectile.ai[0] - 25) * 2, -40);
                Projectile.alpha += 51;
            }
        }
    }
}