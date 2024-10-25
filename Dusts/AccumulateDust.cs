using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DeusMod.Dusts
{
    public class AccumulateDust : ModDust
    {
        public override string Texture => "DeusMod/Effects/Textures/AccumulateLight";
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
			dust.frame = new Rectangle(0, 0, 3780, 1890);
		}
		public override bool Update(Dust dust)
		{
			dust.position += dust.velocity;
			dust.velocity *= 0.95f;
			dust.scale *= 0.99f;

			float light = 0.35f * dust.scale;

			Lighting.AddLight(dust.position, light, light, light);

			if (dust.scale < 0.5f)
			{
				dust.active = false;
			}

			return false;
		}
	}
}
