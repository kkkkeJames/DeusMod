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
    public class LineWarn : ModProjectile
    {
        //以设置的center为预警线center，设置长、宽、角度和颜色
        public Color color = Color.Black;
        public int timeMax = 60;
        public override string Texture => "Terraria/Images/Extra_210";
        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.alpha = 255;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
        }
        public override void AI()
        {
            if (Projectile.timeLeft > timeMax - 5) Projectile.alpha -= 51;
            if (Projectile.timeLeft < 5) Projectile.alpha += 51;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 pos = Projectile.Center - Main.screenPosition;
            List<VertexInfo2> line = new List<VertexInfo2>();
            List<VertexInfo2> line2 = new List<VertexInfo2>();
            List<VertexInfo2> arrow = new List<VertexInfo2>();
            List<VertexInfo2> arrow2 = new List<VertexInfo2>();
            for (int i = 0; i <= 64; i++)
            {
                line.Add(new VertexInfo2(pos + new Vector2(Projectile.width * i / 64f - Projectile.width * 0.5f, Projectile.height * -0.5f).RotatedBy(Projectile.rotation), new Vector3(0, 0, 1), color * ((float)(255 - Projectile.alpha) / 255)));
                line.Add(new VertexInfo2(pos + new Vector2(Projectile.width * i / 64f - Projectile.width * 0.5f, 0).RotatedBy(Projectile.rotation), new Vector3(1, 1, 1), color * 0.6f * ((float)(255 - Projectile.alpha) / 255)));
                line2.Add(new VertexInfo2(pos + new Vector2(Projectile.width * i / 64f - Projectile.width * 0.5f, 0).RotatedBy(Projectile.rotation), new Vector3(0, 0, 1), color * 0.6f * ((float)(255 - Projectile.alpha) / 255)));
                line2.Add(new VertexInfo2(pos + new Vector2(Projectile.width * i / 64f - Projectile.width * 0.5f, Projectile.height * 0.5f).RotatedBy(Projectile.rotation), new Vector3(1, 1, 1), color * ((float)(255 - Projectile.alpha) / 255)));
            }
            arrow.Add(new VertexInfo2(pos + new Vector2(Projectile.width * (timeMax - Projectile.timeLeft) / (timeMax + 2) - Projectile.width * 0.5f, Projectile.height * -0.5f).RotatedBy(Projectile.rotation), new Vector3(0, 0, 1), color * ((float)(255 - Projectile.alpha) / 255)));
            arrow.Add(new VertexInfo2(pos + new Vector2(Projectile.width * (timeMax - Projectile.timeLeft + 1) / (timeMax + 2) - Projectile.width * 0.5f, Projectile.height * -0.5f).RotatedBy(Projectile.rotation), new Vector3(0, 1, 1), color * ((float)(255 - Projectile.alpha) / 255)));
            arrow.Add(new VertexInfo2(pos + new Vector2(Projectile.width * (timeMax - Projectile.timeLeft + 1) / (timeMax + 2) - Projectile.width * 0.5f, 0).RotatedBy(Projectile.rotation), new Vector3(0, 1, 1), color * ((float)(255 - Projectile.alpha) / 255)));
            arrow.Add(new VertexInfo2(pos + new Vector2(Projectile.width * (timeMax - Projectile.timeLeft + 2) / (timeMax + 2) - Projectile.width * 0.5f, 0).RotatedBy(Projectile.rotation), new Vector3(1, 1, 1), color * ((float)(255 - Projectile.alpha) / 255)));

            arrow2.Add(new VertexInfo2(pos + new Vector2(Projectile.width * (timeMax - Projectile.timeLeft) / (timeMax + 2) - Projectile.width * 0.5f, Projectile.height * 0.5f).RotatedBy(Projectile.rotation), new Vector3(0, 0, 1), color * ((float)(255 - Projectile.alpha) / 255)));
            arrow2.Add(new VertexInfo2(pos + new Vector2(Projectile.width * (timeMax - Projectile.timeLeft + 1) / (timeMax + 2) - Projectile.width * 0.5f, Projectile.height * 0.5f).RotatedBy(Projectile.rotation), new Vector3(0, 1, 1), color * ((float)(255 - Projectile.alpha) / 255)));
            arrow2.Add(new VertexInfo2(pos + new Vector2(Projectile.width * (timeMax - Projectile.timeLeft + 1) / (timeMax + 2) - Projectile.width * 0.5f, 0).RotatedBy(Projectile.rotation), new Vector3(0, 1, 1), color * ((float)(255 - Projectile.alpha) / 255)));
            arrow2.Add(new VertexInfo2(pos + new Vector2(Projectile.width * (timeMax - Projectile.timeLeft + 2) / (timeMax + 2) - Projectile.width * 0.5f, 0).RotatedBy(Projectile.rotation), new Vector3(1, 1, 1), color * ((float)(255 - Projectile.alpha) / 255)));
            #region 顶点绘制配件
            //顶点绘制的固定一套语句
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            if (line.Count >= 3)
            {
                Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.Projectile[Projectile.type].Value;
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, line.ToArray(), 0, line.Count - 2);
            }
            if (line2.Count >= 3)
            {
                Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.Projectile[Projectile.type].Value;
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, line2.ToArray(), 0, line2.Count - 2);
            }
            if (arrow.Count >= 3)
            {
                Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.Projectile[Projectile.type].Value;
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, arrow.ToArray(), 0, arrow.Count - 2);
            }
            if (arrow2.Count >= 3)
            {
                Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.Projectile[Projectile.type].Value;
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, arrow2.ToArray(), 0, arrow2.Count - 2);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            #endregion
            return false;
        }
    }
    public class TargetedLineWarn : ModProjectile
    {
        public static Color color = Color.Black;
        public static Player target = null;
        public override string Texture => "DeusMod/Assets/Projs/LineWarn";
        public override void SetDefaults()
        {
            Projectile.width = 2880;
            Projectile.height = 192;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.ai[0];
            if (target != null)
                Projectile.Center = target.Center + new Vector2(Projectile.ai[1], 0);
            else
            {
                if (Projectile.localAI[0] == 0)
                    Projectile.Center += new Vector2(Projectile.ai[1], 0);
            }
            Projectile.localAI[0]++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            List<VertexInfo2> line = new List<VertexInfo2>();
            for (int i = 1; i <= 64; i++)
            {
                Vector2 pos0 = Projectile.Center - Main.screenPosition;
                float dis = (((float)i - Projectile.localAI[0]) / 64);
                if (dis < 0) dis++;
                line.Add(new VertexInfo2(pos0 + new Vector2(Projectile.width * i / 64f - Projectile.width * 0.5f, Projectile.height * -0.5f * Projectile.scale).RotatedBy(Projectile.rotation), new Vector3(dis, 0, 1), color * ((float)(255 - Projectile.alpha) / 255)));
                line.Add(new VertexInfo2(pos0 + new Vector2(Projectile.width * i / 64f - Projectile.width * 0.5f, Projectile.height * 0.5f * Projectile.scale).RotatedBy(Projectile.rotation), new Vector3(dis, 1, 1), color * ((float)(255 - Projectile.alpha) / 255)));
            }
            #region 顶点绘制配件
            //顶点绘制的固定一套语句
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.Projectile[Projectile.type].Value;
            if (line.Count >= 3)
            {
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, line.ToArray(), 0, line.Count - 2);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            #endregion
            return false;
        }
    }
}
