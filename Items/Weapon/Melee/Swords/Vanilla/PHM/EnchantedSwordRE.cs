using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using DeusMod.Projs.Melee;
using DeusMod.Projs.Melee.SkillUI;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using DeusMod.Dusts;
using DeusMod.Projs;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using ReLogic.Content;
using DeusMod.Core.Systems;

namespace DeusMod.Items.Weapon.Melee.Swords.Vanilla
{
    public class EnchantedSwordRE : GlobalItem
    {
        public bool special = false;
        public bool normalend = false;
        public int phase = 0;
        public int damage;
        public float timer;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.EnchantedSword;
        }
        public override void SetDefaults(Item item)
        {
            item.noUseGraphic = true;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.Shoot;
            item.useTime = item.useAnimation = 40;
            item.shoot = ModContent.ProjectileType<EnchantedSwordSlash>();
            item.channel = true;
            item.autoReuse = false;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            var Cline1 = new TooltipLine(Mod, "Tooltip1", "挥剑模式：每次挥剑会射出附魔剑光束");
            var Cline2 = new TooltipLine(Mod, "Tooltip2", "蓄能模式：长按左键蓄力");
            var Cline3 = new TooltipLine(Mod, "Tooltip3", "蓄力结束后释放一束过载附魔光束");
            var Cline4 = new TooltipLine(Mod, "Tooltip4", "\"地下埋藏的宝剑，以具有优秀魔力传导能力的材料制成。\"");
            var Cline5 = new TooltipLine(Mod, "Tooltip5", "\"至今人们仍对其知之甚少，包括其魔能的来源。\"");
            var Cline6 = new TooltipLine(Mod, "Tooltip5", "\"最近泰拉多地侦测到不明的魔能来源。经过比对，其魔能是泰拉目前最接近附魔剑魔能的。\"");
            tooltips.Add(Cline1);
            tooltips.Add(Cline2);
            tooltips.Add(Cline3);
            tooltips.Add(Cline4);
            tooltips.Add(Cline5);
            tooltips.Add(Cline6);
        }
        public override void HoldItem(Item item, Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<EnchantedSwordSlash>()] < 1)
            {
                var proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.Center, Vector2.Zero, ModContent.ProjectileType<EnchantedSwordSlash>(), item.damage, item.knockBack, player.whoAmI);
            }
            if (normalend == true) //在一斩结束后开始计算后摇
            {
                timer++; //后摇计时器
                if (timer >= 60) //后摇超过60帧了
                {
                    normalend = false; //重置攻击段数
                }
            }
            else timer = 0; //后摇计时器不动
        }
        public override bool AltFunctionUse(Item item, Player player)//右键切换模式
        {
            return true;
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 0)
            {
                timer = 0;
                if (!special)
                {
                    if (normalend)
                    {
                        phase++; //在60f的攻击后摇之内再次攻击的话，段数指示器加一，表示这一打是下一打
                        if (phase == 3)
                            phase = 0; //如果第三段打完则重置到第一段
                    }
                    else
                    {
                        phase = 0; //60f后摇接受后攻击，则直接返回第一段
                    }
                    normalend = true;
                    foreach (Projectile proj in Main.projectile)
                    {
                        if (proj.type == ModContent.ProjectileType<EnchantedSwordSlash>() && proj.owner == player.whoAmI && proj != null)
                        {
                            switch (phase)
                            {
                                case 0:
                                    //((EnchantedSwordSlash)proj.ModProjectile).Wield_enterstats(false, true, -1.9f, 1.9f, 2f * item.scale, 0.7f, 1f);
                                    break;
                                case 1:
                                    //((EnchantedSwordSlash)proj.ModProjectile).Wield_enterstats(true, true, 1.8f, -1.7f, 2f * item.scale, 0.8f, 1f);
                                    break;
                                case 2:
                                    //((EnchantedSwordSlash)proj.ModProjectile).Wield_enterstats(false, true, -2.5f, 2.3f, 2f * item.scale, 0.7f, 1.1f, 0, true);
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.Center, Vector2.Zero, ModContent.ProjectileType<SwordEnchant>(), item.damage, item.knockBack, player.whoAmI);
                }
            }
            else if(player.altFunctionUse == 2)
            {
                if(!special)
                {
                    Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.Center, Vector2.Zero, ModContent.ProjectileType<MeleeSkill_Accumulate>(), item.damage, item.knockBack, player.whoAmI);
                    special = true;
                }
                else
                {
                    Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.Center, Vector2.Zero, ModContent.ProjectileType<MeleeSkill_Slash>(), item.damage, item.knockBack, player.whoAmI);
                    special = false;
                }
            }
            return false;
        }
    }
    public class EnchantedSwordEffect
    {
        public static Effect Whiten = ModContent.Request<Effect>("DeusMod/Effects/Whiten", AssetRequestMode.ImmediateLoad).Value;
    }
    public class SwordEnchant : ModProjectile
    {
        public override string Texture => "DeusMod/Items/Weapon/Melee/Swords/Vanilla/PHM/EnchantedSwordEnchant";
        public float damscale = 1;
        public int drawtimer = 0;
        public bool flying = false;
        public bool rotating = false;
        public Vector2 mousepos;
        public override void SetDefaults()
        {
            Projectile.width = ModContent.Request<Texture2D>(Texture).Value.Width;
            Projectile.height = ModContent.Request<Texture2D>(Texture).Value.Height;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.noEnchantments = true;
            Projectile.friendly = true;
            Projectile.extraUpdates = 3;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            Projectile.NewProjectileDirect(player.GetSource_FromThis(), Projectile.Center + new Vector2(0, -42), Vector2.Zero, ModContent.ProjectileType<EnchantedSwordCircle>(), 0, 0, player.whoAmI);
        }
        public float Wield_logisticformula(float x) //挥剑的逻辑斯蒂
        {
            return (float)((3.14 / (1 + ((3.14 / 1.57) - 1) * Math.Pow(Math.E, -0.23f * x / 4))) - 1.57);
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!flying) //是否脱手
            {
                player.itemTime = 8;
                player.itemAnimation = 8;
                if (!rotating) //如果没脱手，是否在跟着剑转
                {
                    if (player.channel) //如果还在蓄力
                    {
                        player.direction = Main.MouseWorld.X - player.Center.X > 0 ? 1 : -1; //玩家随着鼠标改变方向
                        foreach (Projectile proj in Main.projectile)
                        {
                            if (proj.type == ModContent.ProjectileType<EnchantedSwordSlash>() && proj.owner == player.whoAmI && proj != null)
                            {
                                //((EnchantedSwordSlash)proj.ModProjectile).Special1(); //开特殊状态
                            }
                        }

                        Projectile.timeLeft = 2400; //不让射弹消失
                        Projectile.ai[0]++; //计时器
                        Projectile.rotation = -(float)Math.PI / 4; //射弹固定角度
                        Projectile.Center = player.Center + new Vector2(0, -(float)Math.Sqrt(Projectile.width * Projectile.width + Projectile.height * Projectile.height) * Projectile.scale / 2); //射弹固定中心
                        if (Projectile.ai[0] <= 1440) //蓄力的6秒钟之内
                        {
                            Projectile.scale = MathHelper.Lerp(1, 2, Projectile.ai[0] / 1440); //弹幕最终能变成4倍大小
                            damscale = 1 + Projectile.ai[0] / 480; //伤害同理
                            bool flag = Main.rand.NextBool(8); //蓄力粒子
                            if (flag)
                            {
                                float rot = Main.rand.NextFloat((float)Math.PI * 2);
                                Projectile proj = Projectile.NewProjectileDirect(player.GetSource_FromThis(), Projectile.Center + new Vector2(160, 0).RotatedBy(rot), new Vector2(-8f, 0).RotatedBy(rot), ModContent.ProjectileType<AccumulateLight>(), 0, 0, player.whoAmI, -80); //蓄力的粒子
                                proj.scale = Main.rand.NextFloat(0.6f, 1.2f);
                                proj.rotation = rot;
                            }
                        }
                    }
                    if (!player.channel || Projectile.ai[0] > 1440)
                    {
                        Projectile.ai[0] = 0;
                        rotating = true;
                        Projectile.damage = (int)(Projectile.damage * damscale);
                        Projectile.penetrate = 1;
                        mousepos = Main.screenPosition + Main.MouseScreen - player.Center;
                        bool dir = player.direction == 1 ? true : false;
                        float end = (float)Math.PI / 3 * 2;
                        foreach (Projectile proj in Main.projectile)
                        {
                            if (proj.type == ModContent.ProjectileType<EnchantedSwordSlash>() && proj.owner == player.whoAmI && proj != null)
                            {
                                //((EnchantedSwordSlash)proj.ModProjectile).Wield_enterstats(dir, false, -(float)Math.PI / 2, end, Projectile.scale * 2 - 0.2f, 1, 1);
                                ((EnchantedSwordSlash)proj.ModProjectile).Draw_SwordShouldDraw = true;
                            }
                        }
                    }
                }
                else
                {
                    Projectile.ai[0]++;
                    int maxtime = (int)(player.HeldItem.useTime / player.GetAttackSpeed(DamageClass.Melee)) * 8 / 3;
                    Func<float, float> f = Wield_logisticformula;
                    if (player.direction == 1)
                    {
                        Projectile.rotation = -(float)Math.PI / 4 + ((float)Math.PI / 3 * 2 + (float)Math.PI / 2) * (f(Projectile.ai[0]) / f(maxtime));
                        Projectile.Center = player.Center + new Vector2((float)Math.Sqrt(Projectile.width * Projectile.width + Projectile.height * Projectile.height) * Projectile.scale / 2, 0).RotatedBy(Projectile.rotation -(float)Math.PI / 4);
                    }
                    else
                    {
                        Projectile.rotation = -(float)Math.PI / 4 - ((float)Math.PI / 3 * 2 + (float)Math.PI / 2) * (f(Projectile.ai[0]) / f(maxtime));
                        Projectile.Center = player.Center + new Vector2((float)Math.Sqrt(Projectile.width * Projectile.width + Projectile.height * Projectile.height) * Projectile.scale / 2, 0).RotatedBy(Projectile.rotation - (float)Math.PI / 4);
                    }
                    if(Math.Abs(Projectile.rotation - (float)Math.PI / 4 - Math.Atan(mousepos.Y / mousepos.X)) <= 0.06f || Math.Abs((Projectile.rotation - (float)Math.PI / 4 + Math.PI) - Math.Atan(mousepos.Y / mousepos.X)) <= 0.06f)
                    {
                        flying = true;
                        rotating = false;
                    }
                }
            }
            else
            {
                Projectile.ai[1]++;
                Projectile.ignoreWater = false;
                Projectile.velocity = Vector2.UnitX.RotatedBy(Projectile.rotation - (float)Math.PI / 4) * 16 * (float)Math.Pow(0.97f, Projectile.ai[1]);
                if (Projectile.velocity.Length() <= 0.2f) Projectile.Kill();
            }
        }
        public override bool? CanDamage()
        {
            return (rotating || flying);
        }
        public override bool? CanCutTiles()
        {
            return (flying || rotating);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            drawtimer++;
            float specialtimer = drawtimer % 30;
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Color color = new Color(255, 255, 255) * MathHelper.Lerp(0.5f, 0, specialtimer / 30);
            float scalenum = MathHelper.Lerp(1, 2.5f, specialtimer / 30);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, new Vector2(tex.Width / 2, tex.Height / 2), Projectile.scale * scalenum, SpriteEffects.None, default);
            if (flying)
            {
                Main.spriteBatch.End();//先要关闭画布
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                EnchantedSwordEffect.Whiten.Parameters["uTime"].SetValue(Projectile.ai[1] / 3f);
                EnchantedSwordEffect.Whiten.CurrentTechnique.Passes[0].Apply();
            }
            return true;
        }
        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();//先要关闭画布
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
        }
        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            player.GetModPlayer<ScreenShake>().ScreenShakeShort(48, Projectile.rotation);
            Projectile.NewProjectileDirect(player.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<EnchantedSwordExplosion>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
            #region 爆炸粒子
            Projectile.NewProjectileDirect(player.GetSource_FromThis(), Projectile.Center, new Vector2(Main.rand.NextFloat(36, 54), Main.rand.NextFloat(9, 18)), ModContent.ProjectileType<EnchantedSwordDust>(), 0, 0, player.whoAmI, 1);
            Projectile.NewProjectileDirect(player.GetSource_FromThis(), Projectile.Center, new Vector2(Main.rand.NextFloat(18, 36), Main.rand.NextFloat(-15, -9)), ModContent.ProjectileType<EnchantedSwordDust>(), 0, 0, player.whoAmI, 1);
            Projectile.NewProjectileDirect(player.GetSource_FromThis(), Projectile.Center, new Vector2(Main.rand.NextFloat(-45, -36f), Main.rand.NextFloat(6, 18)), ModContent.ProjectileType<EnchantedSwordDust>(), 0, 0, player.whoAmI, 1);
            Projectile.NewProjectileDirect(player.GetSource_FromThis(), Projectile.Center, new Vector2(Main.rand.NextFloat(-36, -18f), Main.rand.NextFloat(-9, -6)), ModContent.ProjectileType<EnchantedSwordDust>(), 0, 0, player.whoAmI, 1);
            Projectile.NewProjectileDirect(player.GetSource_FromThis(), Projectile.Center, new Vector2(Main.rand.NextFloat(-24f, -15f), Main.rand.NextFloat(-27, -15)), ModContent.ProjectileType<EnchantedSwordDust>(), 0, 0, player.whoAmI, 1);
            Projectile.NewProjectileDirect(player.GetSource_FromThis(), Projectile.Center, new Vector2(45, 0).RotatedBy((float)Math.PI / 3), ModContent.ProjectileType<EnchantedSwordDust>(), 0, 0, player.whoAmI, 2);
            Projectile.NewProjectileDirect(player.GetSource_FromThis(), Projectile.Center, new Vector2(45, 0).RotatedBy((float)Math.PI), ModContent.ProjectileType<EnchantedSwordDust>(), 0, 0, player.whoAmI, 2);
            Projectile.NewProjectileDirect(player.GetSource_FromThis(), Projectile.Center, new Vector2(45, 0).RotatedBy((float)Math.PI * 5 / 3), ModContent.ProjectileType<EnchantedSwordDust>(), 0, 0, player.whoAmI, 2);
            Projectile.NewProjectileDirect(player.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<EnchantedSwordDust>(), 0, 0, player.whoAmI, 3);
            Projectile.NewProjectileDirect(player.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<EnchantedSwordDust>(), 0, 0, player.whoAmI, 3, (float)Math.PI * 2 / 3);
            Projectile.NewProjectileDirect(player.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<EnchantedSwordDust>(), 0, 0, player.whoAmI, 3, (float)Math.PI * 4 / 3);
            #endregion
        }
    }
    public class EnchantedSwordCircle : ModProjectile
    {
        public override string Texture => "DeusMod/Effects/Textures/Circleshock";
        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 96;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 30;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
        }
        public override void AI()
        {
            Projectile.scale = MathHelper.Lerp(0f, 4f, (30f - Projectile.timeLeft) / 30f);
            Projectile.alpha = (int)MathHelper.Lerp(60, 255, (30f - Projectile.timeLeft) / 30f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            List<VertexInfo2> circle = new List<VertexInfo2>();
            Color color = new Color(69, 96, 233);
            for (int i = 0; i <= 1; i++) //前15帧绘制基础圆心
            {
                Vector2 pos0 = Projectile.Center - Main.screenPosition;
                circle.Add(new VertexInfo2(pos0 + new Vector2(Projectile.width * (i - 0.5f) * Projectile.scale, -Projectile.height * 0.5f * Projectile.scale), new Vector3(0 + i, 0, 1), color * ((float)(255 - Projectile.alpha) / 255)));
                circle.Add(new VertexInfo2(pos0 + new Vector2(Projectile.width * (i - 0.5f) * Projectile.scale, Projectile.height * 0.5f * Projectile.scale), new Vector3(0 + i, 1, 1), color * ((float)(255 - Projectile.alpha) / 255)));
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
    public class EnchantedSwordExplosion : ModProjectile
    {
        public override string Texture => "DeusMod/Effects/Textures/CircleBlast";
        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 96;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 15;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
        }
        private int rippleCount = 3;
        private int rippleSize = 10;
        private int rippleSpeed = 30;
        private float distortStrength = 100f;
        public override void AI()
        {
            Projectile.scale = MathHelper.Lerp(1f, 2f, (15f - Projectile.timeLeft) / 15f);
            Projectile.alpha = (int)MathHelper.Lerp(0, 255, (15f - Projectile.timeLeft) / 15f);
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[0] = 1; // Set state to exploded
                Projectile.alpha = 255; // Make the Projectile invisible.
                Projectile.friendly = false; // Stop the bomb from hurting enemies.

                if (Main.netMode != NetmodeID.Server && !Filters.Scene["SwordWarp"].IsActive())
                {
                    Filters.Scene.Activate("SwordWarp", Projectile.Center).GetShader().UseColor(rippleCount, rippleSize, rippleSpeed).UseTargetPosition(Projectile.Center);
                }
            }

            if (Main.netMode != NetmodeID.Server && Filters.Scene["SwordWarp"].IsActive())
            {
                float progress = (15f - Projectile.timeLeft) / 60f;
                Filters.Scene["SwordWarp"].GetShader().UseProgress(progress).UseOpacity(distortStrength * (1 - progress / 3f));
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            List<VertexInfo2> circle = new List<VertexInfo2>();
            Color color = new Color(69, 96, 233);
            for (int i = 0; i <= 1; i++) //前15帧绘制基础圆心
            {
                Vector2 pos0 = Projectile.Center - Main.screenPosition;
                circle.Add(new VertexInfo2(pos0 + new Vector2(Projectile.width * (i - 0.5f) * Projectile.scale, -Projectile.height * 0.5f * Projectile.scale), new Vector3(0 + i, 0, 1), color * ((float)(255 - Projectile.alpha) / 255)));
                circle.Add(new VertexInfo2(pos0 + new Vector2(Projectile.width * (i - 0.5f) * Projectile.scale, Projectile.height * 0.5f * Projectile.scale), new Vector3(0 + i, 1, 1), color * ((float)(255 - Projectile.alpha) / 255)));
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
        public override void Kill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.Server && Filters.Scene["SwordWarp"].IsActive())
            {
                Filters.Scene["SwordWarp"].Deactivate();
            }
        }
    }
    public class EnchantedSwordDust : ModProjectile
    {
        public Vector2 offset;
        public Vector2[] posrec = new Vector2[180];
        public int trailnum = 0;
        private float w = 0;
        public override string Texture => "Terraria/Images/Extra_210";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 90;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[0] == 3)
                offset = Projectile.position;
        }
        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan(Projectile.velocity.Y / Projectile.velocity.X);
            if (Projectile.ai[0] == 1)
            {
                Projectile.velocity.Y += 0.05f;
                Projectile.velocity *= 0.9f;
                if (Projectile.timeLeft < 63)
                    Projectile.velocity = Vector2.Zero;
                Projectile.timeLeft -= 2;
                if (Projectile.timeLeft < 15)
                    Projectile.alpha += 34;
            }
            if (Projectile.ai[0] == 2)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(0.1f);
                Projectile.velocity *= 0.9f;
                if (Projectile.timeLeft < 63)
                    Projectile.velocity = Vector2.Zero;
                Projectile.timeLeft -= 2;
                if (Projectile.timeLeft < 15)
                    Projectile.alpha += 34;
            }
            if (Projectile.ai[0] == 3)
            {
                w += 0.06f;
                float a = 48f;
                float b = 8f;
                float r = (float)Math.Sqrt(Math.Pow(a, 2) * Math.Pow(b, 2) / (Math.Pow(a, 2) * Math.Pow(Math.Sin(w), 2) + Math.Pow(b, 2) * Math.Pow(Math.Cos(w), 2)));
                Projectile.position = offset + new Vector2(r, 0).RotatedBy(w + Projectile.ai[1]);
                posrec[trailnum] = Projectile.position;
                trailnum++;
            }
            if (Projectile.timeLeft < 15) 
                Projectile.alpha += 17;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            List<VertexInfo2> trail = new List<VertexInfo2>();
            if (Projectile.ai[0] == 1 || Projectile.ai[0] == 2)
            {
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    if (Projectile.oldPos[i] == Vector2.Zero) break;
                    Vector2 pos0 = Projectile.oldPos[i] - Main.screenPosition;
                    Color color = Color.Lerp(Color.White, Color.Black, (float)i / (Projectile.oldPos.Length - 1));
                    trail.Add(new VertexInfo2(pos0 + new Vector2(0, -Projectile.height * 0.5f * Projectile.scale).RotatedBy(Projectile.oldRot[i]), new Vector3(0 + i, 0, 1), color * ((float)(255 - Projectile.alpha) / 255)));
                    trail.Add(new VertexInfo2(pos0 + new Vector2(0, Projectile.height * 0.5f * Projectile.scale).RotatedBy(Projectile.oldRot[i]), new Vector3(0 + i, 1, 1), color * ((float)(255 - Projectile.alpha) / 255)));
                }
            }
            if (Projectile.ai[0] == 3)
            {
                for(int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    if (trailnum - 1 < i) break;
                    Vector2 pos0 = posrec[trailnum - 1 - i] - Main.screenPosition;
                    Color color = Color.Lerp(Color.White, Color.Black, (float)i / (Projectile.oldPos.Length - 1));
                    trail.Add(new VertexInfo2(pos0 + new Vector2(0, -Projectile.height * 0.5f * Projectile.scale), new Vector3(0 + i, 0, 1), color * ((float)(255 - Projectile.alpha) / 255)));
                    trail.Add(new VertexInfo2(pos0 + new Vector2(0, Projectile.height * 0.5f * Projectile.scale), new Vector3(0 + i, 1, 1), color * ((float)(255 - Projectile.alpha) / 255)));
                }
            }
            #region 顶点绘制配件
            //顶点绘制的固定一套语句
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.Projectile[Projectile.type].Value;
            if (trail.Count >= 3)
            {
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, trail.ToArray(), 0, trail.Count - 2);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            #endregion
            return false;
        }
    }
    public class EnchantedSwordSlash : DeusGlobalSwordSlash
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.EnchantedSword;
        public EnchantedSwordSlash()
        {

        }
        public override void RegisterVariables()
        {
            SlashColor = new Color(69, 96, 233);
        }
        public override void Appear()
        {
            throw new NotImplementedException();
        }
        /*public override void Special1()
{
   Player player = Main.player[Projectile.owner];
   Projectile.rotation = 1.5f * (float)Math.PI;
   Projectile.Center = player.Center + new Vector2(0, -Global_Radius);
   Draw_SwordShouldDraw = false;
}

public override void Special2()
{
   throw new NotImplementedException();
}

public override void Special3()
{
   throw new NotImplementedException();
}

public override void Wield()
{
   base.Wield();
   Player player = Main.player[Projectile.owner];
   Item item = player.HeldItem;
   if (!item.GetGlobalItem<EnchantedSwordRE>().special && (Math.Abs(Projectile.rotation - Math.Atan(Holdup_mousePos.Y / Holdup_mousePos.X)) <= 0.04f || Math.Abs((Projectile.rotation + Math.PI) - Math.Atan(Holdup_mousePos.Y / Holdup_mousePos.X)) <= 0.04f))
   {
       Projectile.NewProjectile(item.GetSource_FromThis(), Projectile.Center - new Vector2(Global_Radius, 0).RotatedBy(Projectile.rotation), new Vector2(item.shootSpeed, 0).RotatedBy(Projectile.rotation), ProjectileID.EnchantedBeam, item.damage, item.knockBack, player.whoAmI);
   }
}*/
    }
}
