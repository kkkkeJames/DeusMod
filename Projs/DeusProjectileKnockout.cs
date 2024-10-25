using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Terraria.Graphics.Effects;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace DeusMod.Projs
{
    public class DeusProjectileKnockout : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public int knockout = 0; //弹幕的击晕值
    }
}
