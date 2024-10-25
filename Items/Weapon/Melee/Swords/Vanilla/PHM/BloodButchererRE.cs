using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using DeusMod.Projs;
using DeusMod.Projs.Melee.SkillUI;
using System.Collections.Generic;
using System;
using DeusMod.Helpers;
using DeusMod.Core;
using DeusMod.Projs.NPCs;
using DeusMod.Dusts;

namespace DeusMod.Items.Weapon.Melee.Swords.Vanilla.PHM
{
    public class BloodButchererRE : GlobalItem
    {
        public bool special = false;
        public bool normalend = false;
        public int phase = 0;
        public int damage;
        public float timer;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.BloodButcherer;
        }
        public override void SetDefaults(Item item)
        {
            item.noUseGraphic = true;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.Shoot;
            item.useTime = item.useAnimation = 36;
            item.shoot = ModContent.ProjectileType<BloodButchererSlash>();
            item.channel = true;
            item.autoReuse = false;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            var Cline1 = new TooltipLine(Mod, "Tooltip1", "右键可以在任何情况下切换挥剑模式/特技模式");
            var Cline2 = new TooltipLine(Mod, "Tooltip2", "特技模式：长按可以持续蓄力");
            var Cline3 = new TooltipLine(Mod, "Tooltip3", "停止蓄力后会根据蓄力时长不断转剑");
            var Cline4 = new TooltipLine(Mod, "Tooltip4", "转剑期间受到伤害降低50%且可以以50%最大速度为速度上限移动");
            var Cline5 = new TooltipLine(Mod, "Tooltip5", "转剑期间击倒敌人可以回复一定血量");
            var Cline6 = new TooltipLine(Mod, "Tooltip6", "\"以猩红矿锭制成的大剑，兼具厚重和锋利，可以将敌人轻松切开。\"");
            var Cline7 = new TooltipLine(Mod, "Tooltip6", "\"猩红矿锭的增殖能量赋予其使用者汲取敌人生命力的能力。\"");
            tooltips.Add(Cline1);
            tooltips.Add(Cline2);
            tooltips.Add(Cline3);
            tooltips.Add(Cline4);
            tooltips.Add(Cline5);
            tooltips.Add(Cline6);
            tooltips.Add(Cline7);
        }
        public bool mouseright = false;
        public override void HoldItem(Item item, Player player)
        {
            player.GetModPlayer<DeusPlayer>().SwordPowerMax = 2.4f;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<BloodButchererSlash>()] < 1)
            {
                var proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.Center, Vector2.Zero, ModContent.ProjectileType<BloodButchererSlash>(), item.damage, item.knockBack, player.whoAmI);
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
        public override bool AltFunctionUse(Item item, Player player)//右键切换斩击模式和蓄力模式
        {
            return true;
        }
        public override bool CanUseItem(Item item, Player player)
        {
            #region InAir判断玩家是否在空中
            Point pos = (player.Bottom / 16).ToPoint();
            bool InAir = !Main.tile[pos].HasTile && !Main.tile[pos].HasUnactuatedTile && Main.tile[pos].TileType == 0;
            #endregion
            if (special == false) return true;
            else
                if (!InAir) return true;
                else return false;
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 0)
            {
                timer = 0;
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
                if(!special)
                { 
                    foreach (Projectile proj in Main.projectile)
                    {
                        if (proj.type == ModContent.ProjectileType<BloodButchererSlash>() && proj.owner == player.whoAmI && proj != null)
                        {
                            switch (phase)
                            {
                                case 0:
                                    ((BloodButchererSlash)proj.ModProjectile).WieldTrigger(true, 2.4f * item.scale, 0.7f, -1.9f, 1.9f, 0.3f, 10);
                                    break;
                                case 1:
                                    ((BloodButchererSlash)proj.ModProjectile).WieldTrigger(true, 2.4f * item.scale, 0.8f, 1.8f, -1.7f, 0.3f, 10);
                                    break;
                                case 2:
                                    ((BloodButchererSlash)proj.ModProjectile).SpecialTrigger(true, 2.6f * item.scale, 0.7f, -2.5f, 2.3f, -0.6f, 12, 0, 2f);
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    foreach (Projectile proj in Main.projectile)
                    {
                        if (proj.type == ModContent.ProjectileType<BloodButchererSlash>() && proj.owner == player.whoAmI && proj != null)
                        {
                            ((BloodButchererSlash)proj.ModProjectile).Special2Trigger();
                        }
                    }
                }
            }
            return false;
        }
    }
    public class BloodButchererSlash : DeusGlobalSwordSlash
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.BloodButcherer;
        public BloodButchererSlash()
        {

        }
        public override void Appear()
        {
        }
        public override void Initialize()
        {
            base.Initialize();
            RegisterState(new Special());
            RegisterState(new Special2());
            RegisterState(new Special3());
        }
        public override void RegisterVariables()
        {
            Player player = Main.player[Projectile.owner];
            SwordDust1 = DustID.Blood;
            SlashColor = Color.DarkRed * 2f;
        }
        public void SpecialTrigger(bool shouldcountmouse, float standardscale, float thinscale, float holdrot, float targrot, float swordpoweradd, float handlelength = 0, float stoptime = 0, float damscale = 1f, int projtype = 0, float projdamscale = 1f)
        {
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
            ((DeusGlobalSwordSlash)Projectile.ModProjectile).SetState<Special>();
            DamageScale = damscale;
            ShootProj = projtype;
            ApplyStuck = true;
            ApplySlashDust = true;
            SwordPowerAdd = swordpoweradd;
        }
        private class Special : ProjState
        {
            public override void AI(ProjStateMachine projectile)
            {
                #region 基础设置
                Projectile proj = projectile.Projectile;
                BloodButchererSlash projmod = (BloodButchererSlash)proj.ModProjectile;
                Player player = Main.player[proj.owner]; //基础设置
                projmod.ShouldDrawArm = true; //自定义手臂角度
                player.itemAnimation = player.itemTime = 2; //使得玩家始终处于使用武器的状态
                projmod.TimeMax = (player.HeldItem.useTime / player.GetAttackSpeed(DamageClass.Melee)) * 16 / 3; //状态总时长
                int HoldupTimeMax = (int)(projmod.TimeMax * 3 / 4); //举剑的时长等于使用武器时间的1/3
                int WieldTimeMax = (int)projmod.TimeMax - HoldupTimeMax; //挥剑的时长等于使用武器时间的2/3
                if (projmod.WieldStuckTimer > 0) //如果卡肉
                    projmod.WieldStuckTimer--; //在计时器减少之前减少卡肉计时器
                else projmod.Timer++; //然后再减少射弹计时器
                #endregion
                #region 总时长的前1/2：举剑并且蓄力
                if (projmod.Timer <= HoldupTimeMax / 2) //如果总时长小于举剑的时长（仅对举剑前1/2时间生效）
                {
                    projmod.WieldAttack = false;
                    if (!projmod.WieldDrawArmBefore)
                    {
                        if (projmod.Timer <= HoldupTimeMax / 6f)
                        {
                            projmod.DrawBehindPlayer = true;
                            projmod.DrawArmRotation = MathHelper.Lerp(0, (float)Math.PI, (float)(projmod.Timer / (HoldupTimeMax / 6f)));
                        }
                        else
                        {
                            projmod.PosSetTarget(proj, (float)(projmod.Timer - HoldupTimeMax / 6f) / (2f * HoldupTimeMax / 6f), true, true);
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
                #region 后1/2：挥剑
                else if(projmod.Timer > HoldupTimeMax)
                {
                    projmod.Clashable = true;
                    projmod.WieldAttack = true;
                    projmod.CouldHit = true;
                    player.GetModPlayer<LBSpecialPlayer>().LBSpecialDodge = false;
                    int WieldTimer;
                    WieldTimer = projmod.DrawTimer = projmod.Timer - HoldupTimeMax;
                    #region 记录初始数据
                    if (WieldTimer == 1) //当时间到了刚开始计算挥剑的位置
                    {
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
        public void Special2Trigger()
        {
            Player player = Main.player[Projectile.owner];
            Timer = 0;
            IniSet.Set(SwordArmPosAdd, Projectile.rotation, ArmRotation, Projectile.scale); //获取iniset
            TargetSet.Set(new Vector2(-12, 0).RotatedBy(player.direction == 1 ? -(float)Math.PI : 0), player.direction == 1 ? -(float)Math.PI : 0, player.direction == 1 ? - (float)Math.PI * 3 / 2 : (float)Math.PI * 3 / 2, 2.4f);
            ((BloodButchererSlash)Projectile.ModProjectile).SetState<Special2>();
        }
        private class Special2 : ProjState
        {
            public bool Charging = true;
            public override void AI(ProjStateMachine projectile)
            {
                #region 基础设置
                Projectile proj = projectile.Projectile;
                BloodButchererSlash projmod = (BloodButchererSlash)proj.ModProjectile;
                Player player = Main.player[proj.owner]; //基础设置
                #endregion
                #region 此AI数据
                if (projmod.Timer == 0)
                    Charging = true;
                projmod.Timer++;
                if (projmod.Timer % 30 == 1)
                {
                    player.GetModPlayer<DeusPlayer>().SwordPower += 0.1f;
                    if (player.GetModPlayer<DeusPlayer>().SwordPower > player.GetModPlayer<DeusPlayer>().SwordPowerMax)
                        player.GetModPlayer<DeusPlayer>().SwordPower = player.GetModPlayer<DeusPlayer>().SwordPowerMax;
                    if (player.GetModPlayer<DeusPlayer>().SwordPower < 0)
                        player.GetModPlayer<DeusPlayer>().SwordPower = 0;
                    player.GetModPlayer<DeusPlayer>().SwordPower = (float)Math.Round(player.GetModPlayer<DeusPlayer>().SwordPower, 1);
                }
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
                        projmod.TimeMax = player.GetModPlayer<DeusPlayer>().SwordPower * 600; //最大时间为蓄力时间的2倍
                        projmod.Timer = 0; //复原计时器
                        projmod.SetState<Special3>(); //设置AI
                    }
                    #endregion
                }
                else
                {
                    #region 蓄力超过3秒，强行取消蓄力
                    if (projmod.Timer > 720)
                    {
                        projmod.ChargeShader = false; 
                        projmod.Timer = 0; //复原计时器
                        projmod.TimeMax = player.GetModPlayer<DeusPlayer>().SwordPower * 600; //最大时间为6s
                        projmod.SetState<Special3>(); //设置AI
                    }
                    #endregion
                }
            }
        }
        private class Special3 : ProjState
        {
            public bool Charging = true;
            public int ChargeDirection = 1;
            public override void AI(ProjStateMachine projectile)
            {
                #region 基础设置
                Projectile proj = projectile.Projectile;
                BloodButchererSlash projmod = (BloodButchererSlash)proj.ModProjectile;
                Player player = Main.player[proj.owner]; //基础设置
                player.GetModPlayer<BBSpecialPlayer>().BBSpecial = true;
                proj.localNPCHitCooldown = 8;
                projmod.WieldStandardScale = 2.4f; //解决半径问题
                projmod.WieldThinScale = 0.4f;
                projmod.WieldHandleLength = 12;
                projmod.DamageScale = 0.2f;
                projmod.ShouldDrawArm = true; //自定义手臂角度
                player.itemAnimation = player.itemTime = 2; //使得玩家始终处于使用武器的状态
                if (projmod.Timer == 0) ChargeDirection = player.direction;
                projmod.Timer++; //然后再减少射弹计时器
                projmod.DrawTimer = projmod.Timer;
                #endregion
                #region 转剑
                projmod.Clashable = true;
                projmod.WieldAttack = true;
                projmod.CouldHit = true;
                if (projmod.Timer % 60 == 1)
                {
                    player.GetModPlayer<DeusPlayer>().SwordPower -= 0.1f;
                    if (player.GetModPlayer<DeusPlayer>().SwordPower > player.GetModPlayer<DeusPlayer>().SwordPowerMax)
                        player.GetModPlayer<DeusPlayer>().SwordPower = player.GetModPlayer<DeusPlayer>().SwordPowerMax;
                    if (player.GetModPlayer<DeusPlayer>().SwordPower < 0)
                        player.GetModPlayer<DeusPlayer>().SwordPower = 0;
                    player.GetModPlayer<DeusPlayer>().SwordPower = (float)Math.Round(player.GetModPlayer<DeusPlayer>().SwordPower, 1);
                }
                #region 角度（平滑变化）、位置、大小、玩家方向变化
                if (projmod.TimeMax - projmod.Timer > 20) proj.rotation += 0.2f * ChargeDirection;
                else proj.rotation += MathHelper.Lerp(0, 0.2f, (projmod.TimeMax - projmod.Timer) / 20f) * ChargeDirection;
                while (proj.rotation > Math.PI * 2) proj.rotation -= (float)Math.PI * 2;
                while (proj.rotation < -Math.PI * 2) proj.rotation += (float)Math.PI * 2;
                projmod.SwordArmPosAdd = new Vector2(-projmod.WieldHandleLength, 0).RotatedBy(proj.rotation);
                proj.scale = DeusModMathHelper.EllipseRadiusHelper(projmod.WieldStandardScale, projmod.WieldStandardScale * projmod.WieldThinScale, projmod.Projectile.rotation);
                projmod.DrawArmRotation = projmod.ArmRotation = proj.rotation - (float)Math.PI / 2;
                projmod.WieldDrawRadius[projmod.Timer] = projmod.ProjRadius; //像oldpos一样记录绘制半径
                if (proj.rotation > -Math.PI / 2 && proj.rotation < Math.PI / 2) player.direction = 1;
                else player.direction = -1;
                #endregion
                #region 格挡射弹（大剑护体，所以只要碰到就能挡）
                //时间在最长帧的前后范围内，大约6帧
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
                #endregion
                #region 拼刀（大剑护体，所以只能普通拼刀）
                //完美拼刀帧在最长帧的前后范围内，大约6帧，其它时间拼刀均为非完美拼刀
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
                            SoundEngine.PlaySound(DeusModSoundHelper.Sword_Clash_Normal, player.Center);
                            player.velocity.X -= 6 * player.direction; //玩家向后退
                            projmod.Timer = 0; //复原计时器
                            projmod.TimeMax = 120; //设置最大时间
                            projmod.SetState<Clash>();
                            (counterproj.ModProjectile as DeusGlobalNPCSlash).ImperfectClash();
                        }
                    }
                }
                #endregion
                #region 状态机的处理
                if (projmod.Timer >= projmod.TimeMax) //结束
                {
                    player.GetModPlayer<DeusPlayer>().SwordPower = 0;
                    player.GetModPlayer<BBSpecialPlayer>().BBSpecial = false;
                    proj.localNPCHitCooldown = (int)(player.HeldItem.useTime / player.GetAttackSpeed(DamageClass.Melee)) * 4;
                    projmod.IniSet.Set(projmod.SwordArmPosAdd, proj.rotation, projmod.ArmRotation, proj.scale); //设置iniset
                    projmod.TargetSet.Set(new Vector2(-8 * player.direction, -16), 0.5f * (float)Math.PI, player.direction * (float)Math.PI, 1.2f);
                    projmod.Timer = 0; //复原计时器
                    projmod.TimeMax = 240; //设置最大时间
                    projmod.SetState<Recover>(); //设置AI
                    return;
                }
                #endregion
                #endregion
            }
        }
    }
    public class BBSpecialPlayer : ModPlayer
    {
        public bool BBSpecial = false;
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            base.ModifyHurt(ref modifiers);
            if (BBSpecial) Player.endurance += 0.5f;
        }
    }
}