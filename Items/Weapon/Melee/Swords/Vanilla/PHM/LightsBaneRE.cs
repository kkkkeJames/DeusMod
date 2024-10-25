using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using DeusMod.Projs;
using DeusMod.Projs.Melee.SkillUI;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.GameContent;
using System;
using DeusMod.Core;
using DeusMod.Helpers;
using DeusMod.Projs.NPCs;
using DeusMod.Dusts;
using System.Linq;
using DeusMod.Core.Systems;

namespace DeusMod.Items.Weapon.Melee.Swords.Vanilla.PHM
{
    public class LightsBaneRE : GlobalItem
    {
        public bool special = false;
        public bool normalend = false;
        public int phase = 0;
        public int damage;
        public float timer;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.LightsBane;
        }
        public override void SetDefaults(Item item)
        {
            item.noUseGraphic = true;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.Shoot;
            item.useTime = item.useAnimation = 30;
            item.shoot = ModContent.ProjectileType<LightsBaneSlash>();
            item.channel = true;
            item.autoReuse = false;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            var Cline0 = new TooltipLine(Mod, "Tooltip0", "剑意上限：1.6");
            var Cline1 = new TooltipLine(Mod, "Tooltip1", "右键可以在任何情况下切换挥剑模式/特殊模式");
            var Cline2 = new TooltipLine(Mod, "Tooltip2", "挥剑模式：第二段攻击后稍等片刻攻击会向前挥砍");
            var Cline3 = new TooltipLine(Mod, "Tooltip2", "向前挥砍的同时持续消耗剑意，消耗剑意时免疫碰撞伤害");
            var Cline4 = new TooltipLine(Mod, "Tooltip2", "特殊模式：消耗0.8剑意，像目标方向释放一个暗影球");
            var Cline5 = new TooltipLine(Mod, "Tooltip3", "暗影球会逐渐加速，接触敌人后会释放暗影斩击");
            var Cline6 = new TooltipLine(Mod, "Tooltip4", "用任意剑格挡暗影球可以增加斩击的伤害，重置暗影球的速度并使其带有追踪效果");
            var Cline7 = new TooltipLine(Mod, "Tooltip5", "第一次用射出暗影球的剑格挡暗影球还可以恢复消耗的剑意");
            var Cline8 = new TooltipLine(Mod, "Tooltip5", "暗影球最多可以格挡三次");
            var Cline9 = new TooltipLine(Mod, "Tooltip5", "挥剑模式第三段攻击后使用特殊能力会消耗0.8剑意同时释放被判定为格挡一次的暗影球");
            tooltips.Add(Cline0);
            tooltips.Add(Cline1);
            tooltips.Add(Cline2);
            tooltips.Add(Cline3);
            tooltips.Add(Cline4);
            tooltips.Add(Cline5);
            tooltips.Add(Cline6);
            tooltips.Add(Cline7);
            tooltips.Add(Cline8);
            tooltips.Add(Cline9);
        }
        public bool mouseright = false;
        public override void HoldItem(Item item, Player player)
        {
            player.GetModPlayer<DeusPlayer>().SwordPowerMax = 1.6f;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<LightsBaneSlash>()] < 1)
            {
                var proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.Center, Vector2.Zero, ModContent.ProjectileType<LightsBaneSlash>(), item.damage, item.knockBack, player.whoAmI);
            }
            if (normalend == true) //在一斩结束后开始计算后摇
            {
                timer++; //计时器增加后摇时间
                if (timer >= 60)
                {
                    normalend = false; //六十f是极限时间，过去了就是重新开始
                }
            }
            else timer = 0;

            if (Main.mouseRightRelease && mouseright)
            {
                mouseright = false;
                special = !special;
            }
            if (Main.mouseRight)
                mouseright = true;
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            timer = 0;
            if (normalend)
            {
                phase++; //在60f的攻击后摇之内再次攻击的话，段数指示器加一，表示这一打是下一打
                if (!special)
                {
                    if (phase == 4)
                        phase = 0; //如果第四段打完则重置到第一段 
                }
                else
                {
                    if (phase == 5)
                        phase = 0; //如果瞬移打击打完则重置到第一段 
                }
            }
            else
            {
                phase = 0; //60f后摇接受后攻击，则直接返回第一段
            }
            normalend = true;
            if (!special)
            {
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.type == ModContent.ProjectileType<LightsBaneSlash>() && proj.owner == player.whoAmI && proj != null)
                    {
                        switch (phase)
                        {
                            case 0:
                                ((LightsBaneSlash)proj.ModProjectile).WieldTrigger(true, 1.7f * item.scale, 0.7f, -2.1f, 1.8f, 0.3f, 8);
                                break;
                            case 1:
                                ((LightsBaneSlash)proj.ModProjectile).StingerTrigger(true, 0.3f);
                                break;
                            case 2:
                                ((LightsBaneSlash)proj.ModProjectile).WieldTrigger(true, 2.2f * item.scale, 0.5f, -2.6f, 2.4f, -0.2f, 8);
                                break;
                            case 3:
                                ((LightsBaneSlash)proj.ModProjectile).WieldTrigger(true, 2.2f * item.scale, 0.5f, -2.6f, 2.4f, -0.2f, 8);
                                break;
                        }
                    }
                }
            }
            else
            {
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.type == ModContent.ProjectileType<LightsBaneSlash>() && proj.owner == player.whoAmI && proj != null)
                    {
                        if (phase <= 3)
                        {
                            ((LightsBaneSlash)proj.ModProjectile).LightsBaneSpecialTrigger(false);
                            phase = 3;
                        }
                        else ((LightsBaneSlash)proj.ModProjectile).LightsBaneSpecialTrigger(true);
                    }
                }
            }
            return false;
        }
    }
    public class LightsBaneSlash : DeusGlobalSwordSlash
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.LightsBane;
        public LightsBaneSlash()
        {

        }
        public override void Appear()
        {
        }
        public override void Initialize()
        {
            base.Initialize();
            RegisterState(new LBStinger());
            RegisterState(new LBWield());
        }
        public override void RegisterVariables()
        {
            Player player = Main.player[Projectile.owner];
            SwordDust1 = DustID.Demonite;
            SlashColor = Color.BlueViolet;
        }
        public bool SpecialSlash = false;
        public void LightsBaneSpecialTrigger(bool specialslash)
        {
            SpecialSlash = specialslash;
            Player player = Main.player[Projectile.owner];
            if(SpecialSlash) //空放：丢出暗影球
            {
                ((LightsBaneSlash)Projectile.ModProjectile).LBStingerTrigger(false, 0);
            }
            else //1A后放：斩击并产生1档暗影球
            {
                ((LightsBaneSlash)Projectile.ModProjectile).LBWieldTrigger(false, 2f, 0.8f, 1.8f, -1.7f, 0.4f, 1, 6);
            }
        }

        private int LBStingerSpecial = 0;
        private int LBStingerSpecialTimer = 0;
        public void LBStingerTrigger(bool shouldcountmouse, int special)
        {
            LBStingerSpecial = special;
            if (LBStingerSpecial == 2) LBStingerSpecialTimer = 80;
            Player player = Main.player[Projectile.owner];
            StingerDrawArmBefore = ShouldDrawArm;
            Timer = 0;
            IniSet.Set(SwordArmPosAdd, Projectile.rotation, ArmRotation, Projectile.scale); //获取iniset
            MousePos = Main.MouseWorld - player.Center;
            float exrot = MousePos.X > 0 ? 0 : (float)Math.PI;
            if (shouldcountmouse)
            {
                StingerStartPosAdd = new Vector2(-ProjRadius / 2, 0).RotatedBy((float)Math.Atan(MousePos.Y / MousePos.X) + exrot);
                StingerEndPosAdd = new Vector2(0, 0).RotatedBy((float)Math.Atan(MousePos.Y / MousePos.X) + exrot);
                TargetSet.Set(StingerStartPosAdd, (float)Math.Atan(MousePos.Y / MousePos.X) + exrot, (float)Math.Atan(MousePos.Y / MousePos.X) + exrot - (float)Math.PI / 2, 1.6f);
            }
            else
            {
                StingerStartPosAdd = new Vector2(-ProjRadius / 2, 0).RotatedBy(exrot);
                StingerEndPosAdd = new Vector2(0, 0).RotatedBy(exrot);
                TargetSet.Set(StingerStartPosAdd, exrot, exrot - (float)Math.PI / 2, 1.6f);
            }
            ((DeusGlobalSwordSlash)Projectile.ModProjectile).SetState<LBStinger>();
        }
        private class LBStinger : ProjState
        {
            public override void AI(ProjStateMachine projectile)
            {
                #region 基础设置
                Projectile proj = projectile.Projectile;
                LightsBaneSlash projmod = (LightsBaneSlash)proj.ModProjectile;
                Player player = Main.player[proj.owner]; //基础设置
                player.itemAnimation = player.itemTime = 2; //使得玩家始终处于使用武器的状态
                projmod.TimeMax = (player.HeldItem.useTime / player.GetAttackSpeed(DamageClass.Melee)) * 4; //状态总时长
                int HoldupTimeMax = (int)(projmod.TimeMax / 2); //举剑的时长等于使用武器时间的1/2
                int StingerTimeMax = (int)(projmod.TimeMax / 4); //刺剑的时长等于使用武器时间的1/4
                int RecoverTimeMax = (int)projmod.TimeMax - HoldupTimeMax - StingerTimeMax; //收剑的时长等于使用武器时间的1/4
                projmod.Timer++; //射弹计时器
                #endregion
                #region 举剑
                if (projmod.Timer <= HoldupTimeMax)
                {
                    if (!projmod.StingerDrawArmBefore)
                    {
                        if (projmod.Timer <= HoldupTimeMax / 3f)
                        {
                            projmod.DrawBehindPlayer = true;
                            projmod.DrawArmRotation = MathHelper.Lerp(0, (float)Math.PI, (float)(projmod.Timer / (HoldupTimeMax / 3f)));
                        }
                        else
                        {
                            projmod.PosSetTarget(proj, (float)(projmod.Timer - HoldupTimeMax / 3f) / (2f * HoldupTimeMax / 3f), true, true);
                            projmod.DrawArmRotation = projmod.ArmRotation;
                        }
                    }
                    else
                    {
                        projmod.PosSetTarget(proj, (float)projmod.Timer / HoldupTimeMax, true, true);
                        projmod.DrawArmRotation = projmod.ArmRotation;
                    }
                }
                #endregion
                #region 刺剑
                else if (projmod.Timer <= HoldupTimeMax + StingerTimeMax)
                {
                    if (projmod.Timer == HoldupTimeMax + 1)
                    {
                        if (projmod.LBStingerSpecial == 0)
                        {
                            player.velocity.X += 14 * player.direction;
                        }
                        if (projmod.LBStingerSpecial == 2)
                        {
                            Projectile hookproj = Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, new Vector2(12 * player.direction, 0), ModContent.ProjectileType<LightsBaneHook>(), proj.damage, 0, player.whoAmI, RecoverTimeMax / 4);
                            hookproj.timeLeft = (StingerTimeMax + RecoverTimeMax) / 4 + 20;
                            player.velocity.X -= 14 * player.direction;
                        }
                    }
                    if (projmod.LBStingerSpecial == 0) projmod.StingerAttack = true;
                    projmod.CouldHit = true;
                    projmod.DrawTimer = projmod.Timer - HoldupTimeMax;
                    projmod.DrawTimeMax = StingerTimeMax;
                    if (projmod.Timer == HoldupTimeMax + 1)
                    {
                        projmod.IniSet.Set(projmod.SwordArmPosAdd, proj.rotation, projmod.ArmRotation, proj.scale); //获取iniset
                        projmod.TargetSet.Set(projmod.StingerEndPosAdd, proj.rotation, projmod.ArmRotation, proj.scale);
                    }
                    projmod.DrawArmRotation = projmod.ArmRotation;
                    projmod.PosSetTarget(proj, (projmod.Timer - HoldupTimeMax) / (float)StingerTimeMax);
                    if (projmod.LBStingerSpecial == 2)
                    {
                        if (projmod.Timer == HoldupTimeMax + StingerTimeMax)
                        {
                            if (projmod.LBStingerSpecialTimer > 0)
                            {
                                projmod.LBStingerSpecialTimer--;
                                projmod.Timer--;
                            }
                        }
                    }
                }
                #endregion
                #region 收剑
                else
                {
                    if (projmod.LBStingerSpecial == 0) projmod.StingerAttack = false;
                    if (projmod.LBStingerSpecial == 0) projmod.StingerAttackStopDraw = true;
                    projmod.DrawTimer = projmod.Timer - HoldupTimeMax - StingerTimeMax;
                    projmod.DrawTimeMax = RecoverTimeMax;
                    if (projmod.Timer == HoldupTimeMax + StingerTimeMax + 1)
                    {
                        projmod.IniSet.Set(projmod.SwordArmPosAdd, proj.rotation, projmod.ArmRotation, proj.scale); //获取iniset
                        projmod.TargetSet.Set(projmod.StingerStartPosAdd, proj.rotation, projmod.ArmRotation, proj.scale);
                    }
                    projmod.DrawArmRotation = projmod.ArmRotation;
                    projmod.PosSetTarget(proj, (projmod.Timer - HoldupTimeMax - StingerTimeMax) / (float)RecoverTimeMax);
                }
                #endregion
                #region 状态机变化
                if (projmod.Timer > projmod.TimeMax) //结束
                {
                    if (projmod.LBStingerSpecial == 0) projmod.StingerAttackStopDraw = false;
                    projmod.IniSet.Set(projmod.SwordArmPosAdd, proj.rotation, projmod.ArmRotation, proj.scale); //设置iniset
                    if (!projmod.IsShortSword)
                        projmod.TargetSet.Set(new Vector2(-8 * player.direction, -16), 0.5f * (float)Math.PI, player.direction * (float)Math.PI, 1.2f);
                    else projmod.TargetSet.Set(new Vector2(-4 * player.direction, -4), 0.5f * (float)Math.PI - 1.2f * player.direction, 0, 1.2f);
                    projmod.Timer = 0; //复原计时器
                    projmod.TimeMax = 240; //设置最大时间
                    projmod.SetState<Recover>(); //设置AI
                    return;
                }
                #endregion
            }
        }

        private int LBWieldSpecial = 1;
        private bool LBWieldChangeDir = false;
        public void LBWieldTrigger(bool shouldcountmouse, float standardscale, float thinscale, float holdrot, float targrot, float swordpoweradd, int special = 1, float handlelength = 0, float stoptime = 0, float damscale = 1f, int projtype = 0, float projdamscale = 1f)
        {
            LBWieldSpecial = special;
            LBWieldChangeDir = false;
            Player player = Main.player[Projectile.owner];
            WieldDrawArmBefore = ShouldDrawArm;
            Timer = 0; //重置计时器
            IniSet.Set(SwordArmPosAdd, Projectile.rotation, ArmRotation, Projectile.scale); //获取iniset
            ShouldCountMouse = shouldcountmouse; //获取此次挥剑角度是否由鼠标角度决定，也即是否此时需要获取鼠标位置
            if (shouldcountmouse) MousePos = Main.MouseWorld - player.Center; //若需要，获取此时的鼠标位置
            WieldHoldRot = holdrot; //获取举剑的目标角度
            WieldFinalRot = targrot; //获取挥剑的目标角度
            DrawInverse = player.direction < 0 ? true : false; //改变射弹绘制方向
            if (targrot < holdrot) DrawInverse = !DrawInverse; //如果从下向上挥则改变射弹方向
            if (player.direction < 0) //如果玩家为负方向
            {
                WieldHoldRot = (float)Math.PI - WieldHoldRot; //改变两个角度使得角度变成正向
                WieldFinalRot = (float)Math.PI - WieldFinalRot;
            }
            float scl = DeusModMathHelper.EllipseRadiusHelper(standardscale, standardscale * thinscale, WieldHoldRot);
            if (shouldcountmouse) //加入鼠标角度的影响
            {
                WieldHoldRot += (float)Math.Atan(MousePos.Y / MousePos.X);
                WieldFinalRot += (float)Math.Atan(MousePos.Y / MousePos.X);
            }
            WieldStandardScale = standardscale; //解决半径问题
            WieldThinScale = thinscale;
            WieldHandleLength = handlelength;
            TargetSet.Set(new Vector2(-WieldHandleLength, 0).RotatedBy(WieldHoldRot), WieldHoldRot, WieldHoldRot - (float)Math.PI / 2f, scl);
            ((DeusGlobalSwordSlash)Projectile.ModProjectile).SetState<LBWield>();
            DamageScale = damscale;
            ShootProj = projtype;
            SwordPowerAdd = swordpoweradd;
            player.GetModPlayer<LBSpecialPlayer>().LBDodge = false;
        }
        private class LBWield : ProjState
        {
            public override void AI(ProjStateMachine projectile)
            {
                #region 基础设置
                Projectile proj = projectile.Projectile;
                LightsBaneSlash projmod = (LightsBaneSlash)proj.ModProjectile;
                Player player = Main.player[proj.owner]; //基础设置
                projmod.ShouldDrawArm = true; //自定义手臂角度
                player.itemAnimation = player.itemTime = 2; //使得玩家始终处于使用武器的状态
                int HoldupTimeMax = (int)(projmod.TimeMax / 3); //举剑的时长等于使用武器时间的1/3
                if(projmod.LBWieldSpecial == 3)
                {
                    projmod.TimeMax = (player.HeldItem.useTime / player.GetAttackSpeed(DamageClass.Melee)) * 20f / 3f;
                    HoldupTimeMax = (int)(projmod.TimeMax / 5f);
                }
                else projmod.TimeMax = (player.HeldItem.useTime / player.GetAttackSpeed(DamageClass.Melee)) * 4; //状态总时长
                int WieldTimeMax = (int)projmod.TimeMax - HoldupTimeMax; //挥剑的时长等于使用武器时间的2/3
                if (projmod.WieldStuckTimer > 0) //如果卡肉
                    projmod.WieldStuckTimer--; //在计时器减少之前减少卡肉计时器
                else projmod.Timer++; //然后再减少射弹计时器
                #endregion
                #region 总时长的前1/3：举剑
                if (projmod.Timer <= HoldupTimeMax) //如果总时长小于举剑的时长
                {
                    projmod.WieldAttack = false;
                    if (!projmod.WieldDrawArmBefore)
                    {
                        if (projmod.Timer <= HoldupTimeMax / 3f)
                        {
                            projmod.DrawBehindPlayer = true;
                            projmod.DrawArmRotation = MathHelper.Lerp(0, (float)Math.PI, (float)(projmod.Timer / (HoldupTimeMax / 3f)));
                        }
                        else
                        {
                            projmod.PosSetTarget(proj, (float)(projmod.Timer - HoldupTimeMax / 3f) / (2f * HoldupTimeMax / 3f), true, true);
                            projmod.DrawArmRotation = projmod.ArmRotation;
                        }
                    }
                    else
                    {
                        projmod.PosSetTarget(proj, (float)projmod.Timer / HoldupTimeMax, true, true);
                        projmod.DrawArmRotation = projmod.ArmRotation;
                    }
                    if (projmod.LBWieldSpecial == 4)
                    {
                        player.fullRotationOrigin = new Vector2(player.width, player.height) / 2;
                        if (projmod.Timer > projmod.TimeMax * 2 / 9f)
                        {
                            player.headPosition += new Vector2(-0.15f * player.direction, 0);
                            player.legPosition += new Vector2(0.15f * player.direction, 0);
                        }
                    }
                }
                #endregion
                #region 后2/3：挥剑
                else
                {
                    projmod.Clashable = true;
                    projmod.WieldAttack = true;
                    projmod.CouldHit = true;
                    int WieldTimer;
                    WieldTimer = projmod.DrawTimer = projmod.Timer - HoldupTimeMax;
                    if (projmod.LBWieldSpecial == 4)
                    {
                        player.fullRotationOrigin = new Vector2(player.width, player.height) / 2;
                        if (projmod.Timer > projmod.TimeMax * 8 / 9f)
                        {
                            player.headPosition += new Vector2(-0.15f * player.direction, 0);
                            player.legPosition += new Vector2(0.15f * player.direction, 0);
                        }
                        else
                        {
                            player.headPosition = new Vector2(2.1f * player.direction, 0);
                            player.legPosition = new Vector2(-2.1f * player.direction, 0);
                        }
                        if ((projmod.Timer - HoldupTimeMax) % (WieldTimeMax / 16) == 0f && (player.GetModPlayer<DeusPlayer>().SwordPower > 0))
                        {
                            player.GetModPlayer<DeusPlayer>().SwordPower -= 0.1f;
                            player.GetModPlayer<DeusPlayer>().SwordPower = (float)Math.Round(player.GetModPlayer<DeusPlayer>().SwordPower, 1);
                            if (player.GetModPlayer<DeusPlayer>().SwordPower < 0) player.GetModPlayer<DeusPlayer>().SwordPower = 0;
                        }
                        if (player.GetModPlayer<DeusPlayer>().SwordPower > 0)
                            player.GetModPlayer<LBSpecialPlayer>().LBSpecialDodge = true;
                        else player.GetModPlayer<LBSpecialPlayer>().LBSpecialDodge = false;
                    }
                    #region 记录初始数据
                    if (WieldTimer == 1) //当时间到了刚开始计算挥剑的位置
                    {
                        if (projmod.LBWieldSpecial == 1)
                        {
                            player.velocity.X -= 10 * player.direction;
                            if (player.velocity.Y == 0) player.velocity.Y -= 3f;
                        }
                        if (projmod.LBWieldSpecial == 4)
                        {
                            player.velocity.X += 24 * player.direction;
                        }
                        projmod.IniSet.Set(projmod.SwordArmPosAdd, proj.rotation, projmod.ArmRotation, proj.scale); //重新记录Ini类原始数据
                        #region 一部分需要计算得出的数据
                        //通过挥剑标准倍率、挥剑缩小倍率、挥剑初始角度、挥剑最终角度、挥剑角度是否考虑鼠标和鼠标位置等计算得出最终的倍率大小
                        int targscaleflag = projmod.ShouldCountMouse ? 1 : 0;
                        float targscale = DeusModMathHelper.EllipseRadiusHelper(projmod.WieldStandardScale, projmod.WieldStandardScale * projmod.WieldThinScale, projmod.WieldFinalRot - (float)Math.Atan(projmod.MousePos.Y / projmod.MousePos.X) * targscaleflag);
                        projmod.TargetSet.Set(Vector2.Zero, projmod.WieldFinalRot, projmod.WieldFinalRot - (float)Math.PI / 2, targscale);//重新记录targ数据
                        #endregion
                    }
                    #endregion
                    #region 角度、位置、大小变化，不是平滑变化
                    projmod.RotSetTargetLogistic(proj, projmod.Timer - HoldupTimeMax, (float)(projmod.TimeMax - HoldupTimeMax)); //角度变化
                    projmod.SwordArmPosAdd = new Vector2(-projmod.WieldHandleLength, 0).RotatedBy(proj.rotation);
                    proj.scale = DeusModMathHelper.EllipseRadiusHelper(projmod.WieldStandardScale, projmod.WieldStandardScale * projmod.WieldThinScale, projmod.Projectile.rotation - (projmod.ShouldCountMouse ? (float)Math.Atan(projmod.MousePos.Y / projmod.MousePos.X) : 0));
                    projmod.DrawArmRotation = projmod.ArmRotation = proj.rotation - (float)Math.PI / 2;
                    projmod.WieldDrawRadius[WieldTimer] = projmod.ProjRadius; //像oldpos一样记录绘制半径
                    if (projmod.LBWieldSpecial == 3)
                    {
                        player.velocity.X = -6 * player.direction;
                    }
                    if (projmod.LBWieldSpecial == 4)
                    {
                        if (!projmod.DrawInverse)
                        {
                            if (proj.rotation > Math.PI / 2 && proj.rotation < Math.PI * 3 / 2 && projmod.Timer >= HoldupTimeMax + WieldTimeMax / 2f)
                            {
                                player.velocity.X *= 0.97f;
                                projmod.LBWieldChangeDir = true;
                            }
                            if (projmod.LBWieldChangeDir) player.direction = -1;
                        }
                        else
                        {
                            if (proj.rotation < Math.PI / 2 && proj.rotation > -Math.PI / 2 && projmod.Timer >= HoldupTimeMax + WieldTimeMax / 2f)
                            {
                                player.velocity.X *= 0.97f;
                                projmod.LBWieldChangeDir = true;
                            }
                            if (projmod.LBWieldChangeDir) player.direction = 1;
                        }
                    }
                    #endregion
                    #region 挥剑到正中间位置（也就是挥剑的1/4时间点）时发射弹幕
                    if (projmod.ShootProj != 0 && projmod.Timer == HoldupTimeMax + WieldTimeMax / 4)
                    {
                        Projectile shootproj = Projectile.NewProjectileDirect(proj.GetSource_FromThis(), player.Center, Vector2.Zero, projmod.ShootProj, (int)(proj.damage * projmod.ShootProjDamScale), proj.knockBack, Main.myPlayer);
                        shootproj.direction = player.direction;
                    }
                    #endregion
                    #region 格挡射弹
                    //时间在最长帧的前后范围内，大约6帧
                    if (projmod.Timer >= -12 + HoldupTimeMax + WieldTimeMax / 4 && projmod.Timer <= 12 + HoldupTimeMax + WieldTimeMax / 4)
                    {
                        projmod.PerfectClashable = true;
                        foreach (Projectile counterproj in Main.projectile)
                        {
                            Rectangle projhitbox = new Rectangle((int)(proj.Center.X + new Vector2(projmod.ProjRadius, 0).RotatedBy(proj.rotation).X) - 10, (int)(proj.Center.Y + new Vector2(projmod.ProjRadius, 0).RotatedBy(proj.rotation).Y) - 10, 20, 20);
                            Rectangle counterhitbox = counterproj.Hitbox;
                            //敌对射弹、可以拼刀、重合
                            if (counterproj.hostile == true && counterproj.GetGlobalProjectile<DeusGlobalProjs>().ClashType <= 1 && projhitbox.Intersects(counterhitbox))
                            {
                                counterproj.Kill();
                                SoundEngine.PlaySound(DeusModSoundHelper.Sword_Clash_Normal, player.Center);
                            }
                        }
                    }
                    #endregion
                    #region 拼刀
                    //完美拼刀帧在最长帧的前后范围内，大约6帧，其它时间拼刀均为非完美拼刀
                    if (projmod.Timer >= -12 + HoldupTimeMax + WieldTimeMax / 4 && projmod.Timer <= 12 + HoldupTimeMax + WieldTimeMax / 4)
                        projmod.PerfectClashable = true;
                    foreach (Projectile counterproj in Main.projectile)
                    {
                        Rectangle projhitbox = proj.Hitbox;
                        Rectangle counterhitbox = counterproj.Hitbox;
                        //敌对剑、重合
                        if (counterproj.ModProjectile is DeusGlobalNPCSlash && projhitbox.Intersects(counterhitbox))
                        {
                            if ((counterproj.ModProjectile as DeusGlobalNPCSlash).Clashable)
                            {
                                Rectangle Intersect = Rectangle.Intersect(projhitbox, counterhitbox);
                                //尘埃
                                Dust dust = Dust.NewDustDirect(new Vector2(Intersect.Center.X, Intersect.Center.Y) - new Vector2(21, 5), 0, 0, ModContent.DustType<SwordSlash>());
                                dust.velocity = Vector2.Zero;
                                dust.scale = Main.rand.NextFloat(0.3f, 0.4f);
                                dust.rotation = proj.rotation + (float)Math.PI / 2;
                                if (projmod.PerfectClashable)
                                {
                                    SoundEngine.PlaySound(DeusModSoundHelper.Sword_Clash_Perfect, player.Center);
                                    (counterproj.ModProjectile as DeusGlobalNPCSlash).PerfectClash();
                                }
                                else
                                {
                                    SoundEngine.PlaySound(DeusModSoundHelper.Sword_Clash_Normal, player.Center);
                                    player.velocity.X -= 6 * player.direction; //玩家向后退
                                    projmod.Timer = 0; //复原计时器
                                    projmod.TimeMax = 120; //设置最大时间
                                    projmod.SetState<Clash>();
                                    (counterproj.ModProjectile as DeusGlobalNPCSlash).ImperfectClash();
                                }
                            }
                        }
                    }
                    #endregion
                    #region 状态机的处理
                    if (projmod.Timer >= projmod.TimeMax) //结束
                    {
                        player.GetModPlayer<LBSpecialPlayer>().LBSpecialDodge = false;
                        projmod.IniSet.Set(projmod.SwordArmPosAdd, proj.rotation, projmod.ArmRotation, proj.scale); //设置iniset
                        projmod.TargetSet.Set(new Vector2(-8 * player.direction, -16), 0.5f * (float)Math.PI, player.direction * (float)Math.PI, 1.2f);
                        projmod.Timer = 0; //复原计时器
                        projmod.TimeMax = 240; //设置最大时间
                        projmod.SetState<Recover>(); //设置AI
                        return;
                    }
                    #endregion
                }
                #endregion
            }
        }
    }
    public class LBSpecialPlayer : ModPlayer
    {
        public bool LBSpecialDodge;
        public bool LBDodge = false;
        public override void PreUpdate()
        {
            base.PreUpdate();
            if (LBSpecialDodge)
            {
                LBDodge = true;
                Player.GiveImmuneTimeForCollisionAttack(1);
            }
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            base.DrawEffects(drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
            if (LBSpecialDodge)
            {
                r = Color.BlueViolet.R / 255f;
                g = Color.BlueViolet.G / 255f;
                b = Color.BlueViolet.B / 255f;
            }
        }
    }

    public class LightsBane_ShadowOrb : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_18";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18000;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.1f, 0.4f, 0.2f);

            Projectile.velocity *= 1.1f;

            NPC finalTarget = null;
            float maxdistance = 1000f;
            bool targeted = false;
            if (Projectile.ai[1] > 0)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC target = Main.npc[i];
                    if (!target.CanBeChasedBy())
                        continue;

                    Vector2 distance = target.Center - Projectile.Center;
                    float distanceTo = (float)Math.Sqrt(distance.X * distance.X + distance.Y * distance.Y);
                    if (distanceTo < maxdistance)
                    {
                        finalTarget = target;
                        maxdistance = distanceTo;
                        targeted = true;
                    }
                }
                if (targeted)
                {
                    Projectile.Move
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            //Redraw the Projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);

                Main.spriteBatch.Draw(Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            return true;
        }
    }




    
}
