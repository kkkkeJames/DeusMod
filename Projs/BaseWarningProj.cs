using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace DeusMod.Projs
{

    public abstract class BaseWarningProj : ModProjectile
    {
        public static Asset<Texture2D> DrawTex;
        private bool Warning = true; 
        public Vector2 WarningPos;
        public int WarningWidth;
        public int WarningHeight;
        public float WarningRot;
        public int WarningTimeMax;
        public Color WarningColor;
        private int WarningOp = 255;
        private int WarningTimer;
        public override void Load()
        {
            if (Main.dedServ)
                return;

            DrawTex = ModContent.Request<Texture2D>("Terraria/Images/Extra_210");
        }
        public override void Unload()
        {
            if (Main.dedServ)
                return;

            DrawTex = null;
        }
        public override void AI()
        {
            WarningTimer++;
            if (WarningTimer <= 5) WarningOp -= 51;
            if (WarningTimeMax - WarningTimer <= 5) WarningOp += 51;
            if (WarningTimer > WarningTimeMax) Warning = false;
        }
        public void DrawWarningLine()
        {
            if (Warning)
            {
                Vector2 pos = WarningPos - Main.screenPosition;
                List<VertexInfo2> line = new List<VertexInfo2>();
                List<VertexInfo2> line2 = new List<VertexInfo2>();
                List<VertexInfo2> arrow = new List<VertexInfo2>();
                List<VertexInfo2> arrow2 = new List<VertexInfo2>();
                for (int i = 0; i <= 64; i++)
                {
                    line.Add(new VertexInfo2(pos + new Vector2(WarningWidth * i / 64f, WarningHeight * -0.5f).RotatedBy(WarningRot), new Vector3(0, 0, 1), WarningColor * ((float)(255 - WarningOp) / 255)));
                    line.Add(new VertexInfo2(pos + new Vector2(WarningWidth * i / 64f, 0).RotatedBy(WarningRot), new Vector3(1, 1, 1), WarningColor * 0.6f * ((float)(255 - WarningOp) / 255)));
                    line2.Add(new VertexInfo2(pos + new Vector2(WarningWidth * i / 64f, 0).RotatedBy(WarningRot), new Vector3(0, 0, 1), WarningColor * 0.6f * ((float)(255 - WarningOp) / 255)));
                    line2.Add(new VertexInfo2(pos + new Vector2(WarningWidth * i / 64f, WarningHeight * 0.5f).RotatedBy(WarningRot), new Vector3(1, 1, 1), WarningColor * ((float)(255 - WarningOp) / 255)));
                }
                arrow.Add(new VertexInfo2(pos + new Vector2(WarningWidth * (WarningTimer) / (WarningTimeMax + 2), WarningHeight * -0.5f).RotatedBy(WarningRot), new Vector3(0, 0, 1), WarningColor * ((float)(255 - WarningOp) / 255)));
                arrow.Add(new VertexInfo2(pos + new Vector2(WarningWidth * (WarningTimer + 1) / (WarningTimeMax + 2), WarningHeight * -0.5f).RotatedBy(WarningRot), new Vector3(0, 1, 1), WarningColor * ((float)(255 - WarningOp) / 255)));
                arrow.Add(new VertexInfo2(pos + new Vector2(WarningWidth * (WarningTimer + 1) / (WarningTimeMax + 2), 0).RotatedBy(WarningRot), new Vector3(0, 1, 1), WarningColor * ((float)(255 - WarningOp) / 255)));
                arrow.Add(new VertexInfo2(pos + new Vector2(WarningWidth * (WarningTimer + 2) / (WarningTimeMax + 2), 0).RotatedBy(WarningRot), new Vector3(1, 1, 1), WarningColor * ((float)(255 - WarningOp) / 255)));

                arrow2.Add(new VertexInfo2(pos + new Vector2(WarningWidth * (WarningTimer) / (WarningTimeMax + 2), WarningHeight * 0.5f).RotatedBy(WarningRot), new Vector3(0, 0, 1), WarningColor * ((float)(255 - WarningOp) / 255)));
                arrow2.Add(new VertexInfo2(pos + new Vector2(WarningWidth * (WarningTimer + 1) / (WarningTimeMax + 2), WarningHeight * 0.5f).RotatedBy(WarningRot), new Vector3(0, 1, 1), WarningColor * ((float)(255 - WarningOp) / 255)));
                arrow2.Add(new VertexInfo2(pos + new Vector2(WarningWidth * (WarningTimer + 1) / (WarningTimeMax + 2), 0).RotatedBy(WarningRot), new Vector3(0, 1, 1), WarningColor * ((float)(255 - WarningOp) / 255)));
                arrow2.Add(new VertexInfo2(pos + new Vector2(WarningWidth * (WarningTimer + 2) / (WarningTimeMax + 2), 0).RotatedBy(WarningRot), new Vector3(1, 1, 1), WarningColor * ((float)(255 - WarningOp) / 255)));
                #region 顶点绘制配件
                //顶点绘制的固定一套语句
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                if (line.Count >= 3)
                {
                    Main.graphics.GraphicsDevice.Textures[0] = DrawTex.Value;
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, line.ToArray(), 0, line.Count - 2);
                }
                if (line2.Count >= 3)
                {
                    Main.graphics.GraphicsDevice.Textures[0] = DrawTex.Value;
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, line2.ToArray(), 0, line2.Count - 2);
                }
                if (arrow.Count >= 3)
                {
                    Main.graphics.GraphicsDevice.Textures[0] = DrawTex.Value;
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, arrow.ToArray(), 0, arrow.Count - 2);
                }
                if (arrow2.Count >= 3)
                {
                    Main.graphics.GraphicsDevice.Textures[0] = DrawTex.Value;
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, arrow2.ToArray(), 0, arrow2.Count - 2);
                }
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                #endregion
            }
        }
    }
}
