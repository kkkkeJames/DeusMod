using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DeusMod.Projs.Melee;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using System.Collections.Generic;
using Terraria.GameContent;

namespace DeusMod.Items.Weapon.Melee.Spears
{
    public class DeusGlobalSpear : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.Spear;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            var Cline1 = new TooltipLine(Mod, "Tooltip1", "\"长按右键以蓄力，松开右键则向鼠标方向投矛。\"");
            var Cline2 = new TooltipLine(Mod, "Tooltip2", "\"蓄力时间越长，投出的矛下坠速度越慢，移速越快。\"");
            tooltips.Add(Cline1);
            tooltips.Add(Cline2);
        }
        public override bool AltFunctionUse(Item item, Player player)
        {
            return true;
        }
    }

    public abstract class GlobalThrowSpear : ModProjectile
    {
        public Texture2D tex;
        private bool lift = false; //是否已经放开鼠标右键
        private Vector2 aim; //鼠标方向决定的扔出目标
        private float velocity; //飞行速度
        public float minvelocity; //最小飞行速度
        private float fall; //下落速度
        public float maxfall; //最小下落速度
        public int maxtimer; //最大蓄力时长
        public override void SetDefaults()
        {
            tex = ModContent.Request<Texture2D>(Texture).Value;
            Projectile.width = tex.Width;
            Projectile.height = tex.Height;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 60;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
        }
        public override void AI()
        {
            //ai[0]代表计时器，且主要表示蓄力时的计时器
            //ai[1]表示状态机
            Player player = Main.player[Projectile.owner];
            if (!Main.mouseRight)
                lift = true;
            if ((!lift || Projectile.ai[0] <= 60) && Projectile.ai[0] < maxtimer)
            {
                Projectile.ai[1] = 0;
                Projectile.ai[0]++;
                Projectile.timeLeft = 2;
                aim = Main.MouseWorld - player.Center;
                player.direction = aim.X > 0 ? 1 : -1;
                player.itemTime = player.itemAnimation = 2;
                float pi = player.direction == -1 ? (float)Math.PI : 0;
                Projectile.rotation = (float)Math.Atan(aim.Y / aim.X) + (float)Math.PI / 4 + pi;
                Projectile.Center = player.Center + new Vector2((float)Math.Sqrt(Projectile.width * Projectile.width + Projectile.height * Projectile.height) / 4, 0).RotatedBy(Projectile.rotation - (float)Math.PI / 4);
                velocity = MathHelper.Lerp(minvelocity, minvelocity * 3, Projectile.ai[0] / maxtimer);
                fall = MathHelper.Lerp(maxfall, 0.2f * maxfall, Projectile.ai[0] / maxtimer);
            }
            if (((Projectile.ai[0] > 60 && lift) || Projectile.ai[0] >= maxtimer) && Projectile.ai[1] != 2) Projectile.ai[1] = 1;

            if (Projectile.ai[1] == 1)
            {
                velocity = MathHelper.Lerp(minvelocity, minvelocity * 3, Projectile.ai[0] / maxtimer);
                fall = MathHelper.Lerp(maxfall, 0.2f * maxfall, Projectile.ai[0] / maxtimer);
                Projectile.velocity = aim / aim.Length() * velocity;
                Projectile.timeLeft = 600;
                Projectile.tileCollide = true;
                Projectile.penetrate = 1;
                Projectile.ai[1] = 2;
            }
            if (Projectile.ai[1] == 2)
            {
                Projectile.velocity.Y += fall;
                float pi = Projectile.velocity.X < 0 ? (float)Math.PI : 0;
                Projectile.rotation = (float)Math.Atan(Projectile.velocity.Y / Projectile.velocity.X) + (float)Math.PI / 4 + pi;
            }
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            Main.instance.DrawCacheProjsOverPlayers.Add(index);
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[1] == 0) return false;
            else return true;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            float radius = (float)Math.Sqrt(Projectile.width * Projectile.width + Projectile.height * Projectile.height) / 2;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + new Vector2(-radius, 0).RotatedBy(Projectile.rotation - (float)Math.PI / 4), Projectile.Center + new Vector2(radius, 0).RotatedBy(Projectile.rotation - (float)Math.PI / 4), 4, ref point);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[1] == 0)
            {
                Player player = Main.player[Projectile.owner];
                List<VertexInfo2> draw = new List<VertexInfo2>();
                List<VertexInfo2> draw2 = new List<VertexInfo2>();
                List<VertexInfo2> draw3 = new List<VertexInfo2>();
                float height = 48;
                Color color = Color.Red;
                Vector2[] drawpos = new Vector2[11];
                //求接下来扔出的矛的斜率，根据斜率计算角度，把半径放上去
                for (int i = 1; i <= 10; i++)
                {
                    Vector2 tempaim = aim / aim.Length() * velocity;
                    if (i == 1) drawpos[i] = player.Center - Main.screenPosition;
                    else drawpos[i] = drawpos[i - 1] + tempaim + new Vector2(0, fall * (i - 1));
                    float sloperot = (float)Math.Atan((tempaim.Y + fall * (i - 1)) / tempaim.X);
                    draw.Add(new VertexInfo2(drawpos[i] + new Vector2(0, height * -0.5f).RotatedBy(sloperot), new Vector3(i / 10, 0f, 1), color * ((float)(255 - Projectile.alpha) / 255)));
                    draw.Add(new VertexInfo2(drawpos[i] + new Vector2(0, height * -0.4f).RotatedBy(sloperot), new Vector3(i / 10, 0.1f, 1), color * ((float)(255 - Projectile.alpha) / 255)));
                    draw2.Add(new VertexInfo2(drawpos[i] + new Vector2(0, height * -0.4f).RotatedBy(sloperot), new Vector3(i / 10, 0.1f, 1), color * MathHelper.Lerp(0.8f, 0.4f, i / 10f) * ((float)(255 - Projectile.alpha) / 255)));
                    draw2.Add(new VertexInfo2(drawpos[i] + new Vector2(0, height * 0.4f).RotatedBy(sloperot), new Vector3(i / 10, 0.9f, 1), color * MathHelper.Lerp(0.8f, 0.4f, i / 10f) * ((float)(255 - Projectile.alpha) / 255)));
                    draw3.Add(new VertexInfo2(drawpos[i] + new Vector2(0, height * 0.4f).RotatedBy(sloperot), new Vector3(i / 10, 0.9f, 1), color * ((float)(255 - Projectile.alpha) / 255)));
                    draw3.Add(new VertexInfo2(drawpos[i] + new Vector2(0, height * 0.5f).RotatedBy(sloperot), new Vector3(i / 10, 1f, 1), color * ((float)(255 - Projectile.alpha) / 255)));
                }
                #region 顶点绘制配件
                //顶点绘制的固定一套语句
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>("Terraria/Images/Extra_210").Value;
                if (draw.Count >= 3)
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, draw.ToArray(), 0, draw.Count - 2);
                if (draw2.Count >= 3)
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, draw2.ToArray(), 0, draw3.Count - 2);
                if (draw3.Count >= 3)
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, draw3.ToArray(), 0, draw3.Count - 2);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                #endregion
            }
            return true;
        }
    }
}
