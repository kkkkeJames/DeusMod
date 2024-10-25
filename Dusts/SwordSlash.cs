using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace DeusMod.Dusts
{
    public class SwordSlash : ModDust
    {
        private float timer = 0;
        public override string Texture => "Terraria/Images/Projectile_927";
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 72, 72);
            dust.shader = new Terraria.Graphics.Shaders.ArmorShaderData(new Ref<Effect>(DeusMod.Instance.Assets.Request<Effect>("Effects/WeakGlowDust", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value), "WeakGlowDustPass");
            timer = 0;
        }
        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color;
        }
        public override bool Update(Dust dust)
        {
            dust.shader.UseColor(dust.color);
            dust.color *= 0.8f;
            dust.position += new Vector2(3, 0).RotatedBy(dust.rotation);
            if (!dust.noLight)
                Lighting.AddLight(dust.position, dust.color.ToVector3());
            timer++;
            if (timer > 30)
                dust.active = false;
            return false;
        }
    }
}
