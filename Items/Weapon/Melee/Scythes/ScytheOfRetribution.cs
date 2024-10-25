using DeusMod.Core;
using DeusMod.Helpers;
using DeusMod.Projs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DeusMod.Items.Weapon.Melee.Scythes
{
    public class ScytheOfRetribution : ModItem
    {
        public bool special = false;
        public int damage;
        public override string Texture => "DeusMod/Assets/Items/Weapon/Melee/Scythe/ScytheOfRetribution";
		public override void SetStaticDefaults()
        {
        }
		public override void SetDefaults()
		{
			Item.damage = 28;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Melee;
			Item.width = 64;
			Item.height = 60;
			Item.useTime = 40;
			Item.useAnimation = 40;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6;
			Item.value = 30000;
			Item.rare = ItemRarityID.Blue;
			Item.channel = true;
			Item.shoot = ModContent.ProjectileType<ScytheOfRetributionSlash>();
		}
        public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ScytheOfRetributionSlash>()] < 1)
            {
                var proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<ScytheOfRetributionSlash>(), Item.damage, Item.knockBack, player.whoAmI);
            }
        }
        public override bool AltFunctionUse(Player player)//右键切换模式
        {
            return true;
        }
        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
                special = !special;
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 0)
            {
                if (!special)
                {
                    foreach (Projectile proj in Main.projectile)
                    {
                        if (proj.type == ModContent.ProjectileType<ScytheOfRetributionSlash>() && proj.owner == player.whoAmI && proj != null)
                        {
                            ((ScytheOfRetributionSlash)proj.ModProjectile).ScytheOfRetributionSpecialTrigger(false);
                        }
                    }
                }
                else
                {
                    foreach (Projectile proj in Main.projectile)
                    {
                        if (proj.type == ModContent.ProjectileType<ScytheOfRetributionSlash>() && proj.owner == player.whoAmI && proj != null)
                        {
                            ((ScytheOfRetributionSlash)proj.ModProjectile).ScytheOfRetributionSpecialTrigger(true);
                        }
                    }
                }
            }
            return false;
        }
    }
    public class ScytheOfRetributionSlash : DeusGlobalSwordSlash
    {
        public override string Texture => "DeusMod/Assets/Items/Weapon/Melee/Scythe/ScytheOfRetribution";
        public ScytheOfRetributionSlash()
        {
        }
        public override void Appear()
        {
        }
        public override void Initialize()
        {
            base.Initialize();
            RegisterState(new ScytheOfRetributionSpecial());
            RegisterState(new ScytheOfRetributionSpecial2());
        }
        public override void RegisterVariables()
        {
            SwordDust1 = DustID.Iron;
            SlashColor = Color.DarkGray;
            WieldDrawLessLength = 12;
        }
        public void ScytheOfRetributionSpecialTrigger(bool special)
        {
            Player player = Main.player[Projectile.owner];
            Timer = 0; //重置计时器
            IniSet.Set(SwordArmPosAdd, Projectile.rotation, ArmRotation, Projectile.scale); //获取iniset
            if (!special)
            {
                TargetSet.Set(new Vector2(-26, 0).RotatedBy(player.direction == 1 ? -2.2f : 2.2f - (float)Math.PI), player.direction == 1 ? -2.2f : 2.2f - (float)Math.PI, player.direction == 1 ? -2.2f - (float)Math.PI / 2 : 2.2f + (float)Math.PI / 2, 1.6f);
                ((ScytheOfRetributionSlash)Projectile.ModProjectile).SetState<ScytheOfRetributionSpecial>();
            }
            else
            {
                TargetSet.Set(new Vector2(-26, 0).RotatedBy(player.direction == 1 ? -3f : 3f - (float)Math.PI), player.direction == 1 ? -3f : 3f - (float)Math.PI, player.direction == 1 ? -3f - (float)Math.PI / 2 : 3f + (float)Math.PI / 2, 1.6f);
                ((ScytheOfRetributionSlash)Projectile.ModProjectile).SetState<ScytheOfRetributionSpecial2>();
            }
        }
        public class ScytheOfRetributionSpecial : ProjState
        {
            public bool Charging = true;
            public override void AI(ProjStateMachine projectile)
            {
                #region 基础设置
                Projectile proj = projectile.Projectile;
                ScytheOfRetributionSlash projmod = (ScytheOfRetributionSlash)proj.ModProjectile;
                Player player = Main.player[proj.owner]; //基础设置
                #endregion
                #region 此AI数据
                if (projmod.Timer == 0)
                    Charging = true;
                projmod.Timer++;
                projmod.WieldAttack = false;
                projmod.ShouldDrawArm = true;
                projmod.DrawInverse = player.direction < 0;
                player.itemTime = player.itemAnimation = 2;
                //停止长按后不可恢复地停止蓄力
                if (!player.channel)
                    Charging = false;
                //停止蓄力
                #endregion
                #region 手臂位置改变/镰刀举到蓄力位置
                if (projmod.Timer <= 30)
                {
                    projmod.DrawBehindPlayer = true;
                    projmod.DrawArmRotation = MathHelper.Lerp(0, (float)Math.PI * player.direction, projmod.Timer / 30f);
                }
                else if (projmod.Timer <= 120)
                {
                    projmod.PosSetTarget(proj, (projmod.Timer - 30) / 90f, true, true);
                    projmod.DrawArmRotation = projmod.ArmRotation;
                }
                else projmod.ChargeShader = true;
                #endregion
                if (!Charging)
                {
                    #region 只有在蓄力大于30帧的情况下才能停止蓄力
                    if (projmod.Timer > 120)
                    {
                        projmod.ChargeShader = false;
                        projmod.WieldTrigger(false, 1.6f, 1f, -2.2f, 2.2f, 24, 0f, MathHelper.Lerp(1, 2.5f, (projmod.Timer - 120f) / 600f));
                    }
                    #endregion
                }
                else 
                {
                    #region 蓄力超过3秒，无论是否蓄力都停止动作
                    if (projmod.Timer > 720)
                    {
                        projmod.ChargeShader = false;
                        projmod.WieldTrigger(false, 1.6f, 1f, -2.2f, 2.2f, 24, 0f, MathHelper.Lerp(1, 2.5f, (projmod.Timer - 120f) / 600f));
                    }
                    #endregion
                }
            }
        }
        public class ScytheOfRetributionSpecial2 : ProjState
        {
            public override void AI(ProjStateMachine projectile)
            {
                #region 基础设置
                Projectile proj = projectile.Projectile;
                ScytheOfRetributionSlash projmod = (ScytheOfRetributionSlash)proj.ModProjectile;
                Player player = Main.player[proj.owner]; //基础设置
                #endregion
                #region 此AI数据
                projmod.Timer++;
                projmod.WieldAttack = false;
                projmod.ShouldDrawArm = true;
                projmod.ChargeShader = true;
                projmod.DrawInverse = player.direction < 0;
                player.itemTime = player.itemAnimation = 2;
                //停止蓄力
                #endregion
                #region 手臂位置改变/镰刀举到蓄力位置
                if (projmod.Timer <= 30)
                {
                    projmod.DrawBehindPlayer = true;
                    projmod.DrawArmRotation = MathHelper.Lerp(0, (float)Math.PI * player.direction, projmod.Timer / 30f);
                }
                else if (projmod.Timer <= 120)
                {
                    projmod.PosSetTarget(proj, (projmod.Timer - 30) / 90f, true, true);
                    projmod.DrawArmRotation = projmod.ArmRotation;
                }
                #endregion
                #region 超过60帧，停止动作
                if (projmod.Timer > 240)
                {
                    projmod.WieldTrigger(false, 1.6f, 0.5f, -3f, 3f, 0, 16, 0, 1, ModContent.ProjectileType<ScytheOfRetributionProjFriendly>(), 0.5f);
                }
                #endregion
            }
        }
    }
    public class ScytheOfRetributionProjFriendly : ModProjectile
    {
        public override string Texture => "DeusMod/Assets/Items/Weapon/Melee/Scythe/ScytheOfRetribution";
        public static Asset<Texture2D> SlashTexture;
        private float rotinc;
        public override void Load()
        {
            SlashTexture = ModContent.Request<Texture2D>("DeusMod/Effects/Textures/Slash");
        }
        public override void Unload()
        {
            SlashTexture = null;
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 2 * (int)DeusModMathHelper.PythagoreanHelper(ModContent.Request<Texture2D>(Texture).Value.Width, ModContent.Request<Texture2D>(Texture).Value.Height);
            Projectile.height = (int)DeusModMathHelper.PythagoreanHelper(ModContent.Request<Texture2D>(Texture).Value.Width, ModContent.Request<Texture2D>(Texture).Value.Height);
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.penetrate = 3;
            Projectile.aiStyle = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 1440;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 90;
        }
        public override void AI()
        {
            Projectile.velocity = new Vector2(2 * Projectile.direction, 0);
            Projectile.ai[0]++;
            rotinc = (float)Math.PI / 45 * Math.Sign(Projectile.velocity.X);
            Projectile.ai[1] += rotinc;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            List<VertexInfo2> slash = new List<VertexInfo2>();
            List<VertexInfo2> line = new List<VertexInfo2>();
            int iTimer = Projectile.ai[0] < 39 ? (int)Projectile.ai[0] + 1 : Projectile.oldPos.Length;
            for (int i = 0; i < iTimer; i++) //挥砍时长小于31时，挥砍时长小于oldpos记录长度，则oldpos和oldrot的最大值不能被记录，大于31就可以直接回溯最远的oldpos了
            {
                Vector2 pos = Projectile.Center - Main.screenPosition; //挥砍的中心都是以剑柄为中心
                slash.Add(new VertexInfo2(pos + new Vector2(DeusModMathHelper.EllipseRadiusHelper(Projectile.width, Projectile.height, Projectile.ai[1] - rotinc * i) / 2, 0).RotatedBy(Projectile.ai[1] - rotinc * i), new Vector3(1 - (float)i / iTimer, 0, 1), Color.DarkGray * MathHelper.Lerp(1, 0, (float)i / iTimer)));
                slash.Add(new VertexInfo2(pos + new Vector2(DeusModMathHelper.EllipseRadiusHelper(Projectile.width, Projectile.height, Projectile.ai[1] - rotinc * i) / 4, 0).RotatedBy(Projectile.ai[1] - rotinc * i), new Vector3(1 - (float)i / iTimer, 0.12f, 1), Color.DarkGray * MathHelper.Lerp(1, 0, (float)i / iTimer)));
            }
            for (int i = 0; i < 2; i++)
            {
                Vector2 pos = Projectile.Center - Main.screenPosition; //挥砍的中心都是以剑柄为中心
                line.Add(new VertexInfo2(pos + new Vector2(DeusModMathHelper.EllipseRadiusHelper(Projectile.width, Projectile.height, Projectile.ai[1]) / 2, -1 + i * 2).RotatedBy(Projectile.ai[1]), new Vector3(0.8f, 0, 1), Color.DarkGray * 0.8f));
                line.Add(new VertexInfo2(pos + new Vector2(0, -1 + i * 2).RotatedBy(Projectile.ai[1]), new Vector3(0.8f, 0.01f, 1), Color.DarkGray * 0.8f));
            }
            #region 顶点绘制配件
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            if (slash.Count >= 3)
            {
                Main.graphics.GraphicsDevice.Textures[0] = SlashTexture.Value;
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, slash.ToArray(), 0, slash.Count - 2);
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, line.ToArray(), 0, line.Count - 2);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            #endregion
            return false;
        }
    }
}
