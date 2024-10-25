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
using System.Collections.Generic;

namespace DeusMod.Projs
{
    public class CircleWarn : ModProjectile
    {
        public static Color color = Color.Black;
        public override string Texture => "DeusMod/Effects/Textures/Circleshock";
        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 96;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
        }
        public override void AI()
        {
            Projectile.scale = Projectile.ai[0];
            Projectile.localAI[0]++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            List<VertexInfo2> circle = new List<VertexInfo2>();
            List<VertexInfo2> subcircle = new List<VertexInfo2>();
            for (int i = 0; i <= 1; i++) //前15帧绘制基础圆心
            {
                Vector2 pos0 = Projectile.Center - Main.screenPosition;
                if (Projectile.localAI[0] <= 15)
                {
                    circle.Add(new VertexInfo2(pos0 + new Vector2(Projectile.width * (i - 0.5f) * MathHelper.Lerp(0, Projectile.scale, Projectile.localAI[0] / 15), -Projectile.height * 0.5f * MathHelper.Lerp(0, Projectile.scale, Projectile.localAI[0] / 15)), new Vector3(0 + i, 0, 1), color * ((float)(255 - Projectile.alpha) / 255)));
                    circle.Add(new VertexInfo2(pos0 + new Vector2(Projectile.width * (i - 0.5f) * MathHelper.Lerp(0, Projectile.scale, Projectile.localAI[0] / 15), Projectile.height * 0.5f * MathHelper.Lerp(0, Projectile.scale, Projectile.localAI[0] / 15)), new Vector3(0 + i, 1, 1), color * ((float)(255 - Projectile.alpha) / 255)));
                }
                else
                {
                    circle.Add(new VertexInfo2(pos0 + new Vector2(Projectile.width * (i - 0.5f) * Projectile.scale, -Projectile.height * 0.5f * Projectile.scale), new Vector3(0 + i, 0, 1), color * ((float)(255 - Projectile.alpha) / 255)));
                    circle.Add(new VertexInfo2(pos0 + new Vector2(Projectile.width * (i - 0.5f) * Projectile.scale, Projectile.height * 0.5f * Projectile.scale), new Vector3(0 + i, 1, 1), color * ((float)(255 - Projectile.alpha) / 255)));
                }
            }
            if (Projectile.localAI[0] > 30)
            {
                for (int i = 0; i <= 1; i++) //前15帧绘制基础圆心
                {
                    Vector2 pos0 = Projectile.Center - Main.screenPosition;
                    if (Projectile.localAI[0] <= 45)
                    {
                        subcircle.Add(new VertexInfo2(pos0 + new Vector2(Projectile.width * (i - 0.5f) * MathHelper.Lerp(0, Projectile.scale, (Projectile.localAI[0] - 30) / 15), -Projectile.height * 0.5f * MathHelper.Lerp(0, Projectile.scale, (Projectile.localAI[0] - 30) / 15)), new Vector3(0 + i, 0, 1), color * ((float)(255 - Projectile.alpha) / 255)));
                        subcircle.Add(new VertexInfo2(pos0 + new Vector2(Projectile.width * (i - 0.5f) * MathHelper.Lerp(0, Projectile.scale, (Projectile.localAI[0] - 30) / 15), Projectile.height * 0.5f * MathHelper.Lerp(0, Projectile.scale, (Projectile.localAI[0] - 30) / 15)), new Vector3(0 + i, 1, 1), color * ((float)(255 - Projectile.alpha) / 255)));
                    }
                    else
                    {
                        subcircle.Add(new VertexInfo2(pos0 + new Vector2(Projectile.width * (i - 0.5f) * Projectile.scale, -Projectile.height * 0.5f * Projectile.scale), new Vector3(0 + i, 0, 1), color * ((float)(255 - Projectile.alpha) / 255)));
                        subcircle.Add(new VertexInfo2(pos0 + new Vector2(Projectile.width * (i - 0.5f) * Projectile.scale, Projectile.height * 0.5f * Projectile.scale), new Vector3(0 + i, 1, 1), color * ((float)(255 - Projectile.alpha) / 255)));
                    }
                }
            }
            #region 顶点绘制配件
            //顶点绘制的固定一套语句
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.Projectile[Projectile.type].Value;
            if (circle.Count >= 3)
            {
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
                if (Projectile.localAI[0] > 30)
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, subcircle.ToArray(), 0, circle.Count - 2);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            #endregion
            return false;
        }
    }
}
