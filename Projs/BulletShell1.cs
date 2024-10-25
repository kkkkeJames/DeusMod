using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.DataStructures;

namespace DeusMod.Projs
{
    public class BulletShell1 : ModProjectile
    {
        public override string Texture => "DeusMod/Assets/Projs/BulletShell1";
        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 2;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.aiStyle = 14;
        }
    }
}
