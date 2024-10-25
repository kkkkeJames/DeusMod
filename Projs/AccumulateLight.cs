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
using System;

namespace DeusMod.Projs
{
    public class AccumulateLight : ModProjectile
    {
        public override string Texture => "DeusMod/Effects/Textures/AccumulateLight";
        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.scale = 2;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.alpha = 240;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.height = (int)(Projectile.height / Main.rand.NextFloat(6f, 8f));
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.ai[1]++;
            Projectile.Center = player.Center + new Vector2(0, Projectile.ai[0]) + new Vector2(160 - Projectile.ai[1] * 8, 0).RotatedBy(Projectile.rotation); //初始位置+后续位移
            Projectile.scale -= 0.05f;
            if (Projectile.ai[1] <= 4) Projectile.alpha -= 60;
            else Projectile.alpha += 12;
            if (Projectile.alpha >= 255 || Projectile.scale <= 0) Projectile.Kill();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            List<VertexInfo2> circle = new List<VertexInfo2>();
            for (int i = 0; i <= 1; i++)
            {
                Vector2 pos0 = Projectile.Center - Main.screenPosition;
                circle.Add(new VertexInfo2(pos0 + new Vector2(Projectile.width * (i - 0.5f) * Projectile.scale, -Projectile.height * 0.5f * Projectile.scale).RotatedBy(Projectile.rotation), new Vector3(0 + i, 0, 1), Color.White * ((float)(255 - Projectile.alpha) / 255)));
                circle.Add(new VertexInfo2(pos0 + new Vector2(Projectile.width * (i - 0.5f) * Projectile.scale, Projectile.height * 0.5f * Projectile.scale).RotatedBy(Projectile.rotation), new Vector3(0 + i, 1, 1), Color.White * ((float)(255 - Projectile.alpha) / 255)));
            }
            #region 顶点绘制配件
            //顶点绘制的固定一套语句
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.Projectile[Projectile.type].Value;
            if (circle.Count >= 3)
            {
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            #endregion
            return false;
        }
    }
}
