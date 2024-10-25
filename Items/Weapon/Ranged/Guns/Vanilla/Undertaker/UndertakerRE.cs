using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.DataStructures;
using System;
using System.Collections.Generic;
using DeusMod.Dusts;
using DeusMod.Core.Systems;

namespace DeusMod.Items.Weapon.Ranged.Guns.Vanilla.Undertaker
{
    public class UndertakerRE : GlobalItem
    {
        public bool specialbullet;
        public bool used = false;
        public int useTime = 0;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            //这个GlobalItem只改动火枪
            return entity.type == ItemID.TheUndertaker;
        }
        public override void SetDefaults(Item item)//所有的重置枪械的装载弹和射击数据在globalgun里
        {
            item.autoReuse = false;
            item.UseSound = null;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            var line1 = new TooltipLine(Mod, "Tooltip1", "Mod Shooting: Hold mouse right key to load special bullet that drain life from enemies");
            var Cline1 = new TooltipLine(Mod, "Tooltip1", "模组射击：长按右键装载汲取敌人生命的特殊子弹");
            tooltips.Add(Cline1);
        } 
        public override void HoldItem(Item item, Player player)
        {
            if (used)
            {
                if (useTime < item.useTime) useTime++;
                else
                {
                    useTime = 0;
                    used = false;
                }

                int recoiltime = item.useTime / 8;
                if (useTime <= recoiltime) player.itemRotation -= player.direction * 0.11f;
                else if (useTime <= recoiltime * 2) player.itemRotation += player.direction * 0.11f;
            }
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                return false;
            }
            else
            {
                player.GetModPlayer<ScreenShake>().ScreenShakeShort(24, (float)(Math.Atan(velocity.Y / velocity.X) + Math.PI));
                used = true;
                return true;
            }
        }
    }






    public class UndertakerSpecialBullet : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 2;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            AIType = ProjectileID.Bullet;
        }
        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan(Projectile.velocity.Y / Projectile.velocity.X);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.position, Vector2.Zero, ProjectileID.VampireHeal, 0, 0, player.whoAmI, 0, 5);
        }
    }





    
    public class UndertakerLoadBullet : ModProjectile
    {
        public override string Texture => "DeusMod/Assets/Projs/BulletShell1";
        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 2;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 10;
            Projectile.aiStyle = -1;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.ai[0]++;
            Projectile.Center = player.Center + new Vector2(0, 33) - new Vector2(0, 40).RotatedBy(Projectile.ai[0] * player.direction / 10);
            Projectile.rotation = 1.1f * player.direction;
        }
    }





    public class UndertakerLoadBulletSpecial : ModProjectile
    {
        public override string Texture => "DeusMod/Assets/Projs/BulletShell1";
        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 2;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 10;
            Projectile.aiStyle = -1;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.ai[0]++;
            Projectile.Center = player.Center + new Vector2(0, 33) - new Vector2(0, 40).RotatedBy(Projectile.ai[0] * player.direction / 10);
            Projectile.rotation = 1.1f * player.direction;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Tex = ModContent.Request<Texture2D>("Terraria/Images/Projectile_927").Value;
            //Rectangle Rect = new Rectangle(0, 0, Tex.Width, Tex.Height);
            //Main.EntitySpriteDraw(Tex, Projectile.Center - Main.screenPosition, Rect, Color.DarkRed, Projectile.rotation, new Vector2(0, 0), 1, SpriteEffects.None, 0);
            List<VertexInfo2> circle = new List<VertexInfo2>();
            for(int i = 0; i <= 1; i++)
            {
                circle.Add(new VertexInfo2(Projectile.Center - Main.screenPosition + new Vector2(24 * (i - 0.5f) * Projectile.scale, -12 * Projectile.scale).RotatedBy(Projectile.rotation), new Vector3(0 + i, 0, 1), Color.Red * 2));
                circle.Add(new VertexInfo2(Projectile.Center - Main.screenPosition + new Vector2(24 * (i - 0.5f) * Projectile.scale, 12 * Projectile.scale).RotatedBy(Projectile.rotation), new Vector3(0 + i, 1, 1), Color.Red * 2));
            }
            #region 顶点绘制配件
            //顶点绘制的固定一套语句，但是additive和alphablend有不同的适配情况
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.Projectile[927].Value;
            if (circle.Count >= 3)
            {
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            #endregion
            return true;
        }
    }
}
