using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using DeusMod.Projs;
using System.Collections.Generic;
using System;

namespace DeusMod.Items.Weapon.Melee.Swords.Vanilla.PHM
{
    public class CopperBroadSwordRE : GlobalItem
    {
        public bool normalend = false;
        public int phase = 0;
        public float timer;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.CopperBroadsword;
        }
        public override void SetDefaults(Item item)
        {
            item.noUseGraphic = true;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.Shoot;
            item.shoot = ModContent.ProjectileType<CopperBroadswordSlash>();
            item.useTime = item.useAnimation = 33;
            item.channel = true;
            item.autoReuse = false;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            var Cline0 = new TooltipLine(Mod, "Tooltip0", "剑意上限：1");
            var Cline1 = new TooltipLine(Mod, "Tooltip1", "\"矿石制造的剑。普通意味着稳定。\"");
            tooltips.Add(Cline0);
            tooltips.Add(Cline1);
        }
        public override void HoldItem(Item item, Player player)
        {
            player.GetModPlayer<DeusPlayer>().SwordPowerMax = 1f;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<CopperBroadswordSlash>()] < 1)
                Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.Center, Vector2.Zero, ModContent.ProjectileType<CopperBroadswordSlash>(), item.damage, item.knockBack, player.whoAmI);
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
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
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
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.type == ModContent.ProjectileType<CopperBroadswordSlash>() && proj.owner == player.whoAmI && proj != null)
                {
                    switch (phase)
                    {
                        case 0:
                            ((CopperBroadswordSlash)proj.ModProjectile).WieldTrigger(true, 2f * item.scale, 0.7f, -1.9f, 1.9f, 0.2f, 6);
                            break;
                        case 1:
                            ((CopperBroadswordSlash)proj.ModProjectile).WieldTrigger(true, 2f * item.scale, 0.8f, 1.8f, -1.7f, 0.2f, 6);
                            break;
                        case 2:
                            ((CopperBroadswordSlash)proj.ModProjectile).WieldTrigger(true, 2f * item.scale, 0.7f, -2.5f, 2.3f, -0.4f, 6, 0, 1, 0, 1.2f);
                            break;
                    }
                }
            }
            return false;
        }
    }
    public class TinBroadSwordRE : GlobalItem
    {
        public bool normalend = false;
        public int phase = 0;
        public float timer;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.TinBroadsword;
        }
        public override void SetDefaults(Item item)
        {
            item.noUseGraphic = true;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.Shoot;
            item.shoot = ModContent.ProjectileType<TinBroadswordSlash>();
            item.useTime = item.useAnimation = 33;
            item.channel = true;
            item.autoReuse = false;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            var Cline0 = new TooltipLine(Mod, "Tooltip0", "剑意上限：1");
            var Cline1 = new TooltipLine(Mod, "Tooltip1", "\"矿石制造的剑。普通意味着稳定。\"");
            tooltips.Add(Cline0);
            tooltips.Add(Cline1);
        }
        public override void HoldItem(Item item, Player player)
        {
            player.GetModPlayer<DeusPlayer>().SwordPowerMax = 1f;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<TinBroadswordSlash>()] < 1)
                Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.Center, Vector2.Zero, ModContent.ProjectileType<TinBroadswordSlash>(), item.damage, item.knockBack, player.whoAmI);
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
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
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
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.type == ModContent.ProjectileType<TinBroadswordSlash>() && proj.owner == player.whoAmI && proj != null)
                {
                    switch (phase)
                    {
                        case 0:
                            ((TinBroadswordSlash)proj.ModProjectile).WieldTrigger(true, 2f * item.scale, 0.7f, -1.9f, 1.9f, 0.2f, 6);
                            break;
                        case 1:
                            ((TinBroadswordSlash)proj.ModProjectile).WieldTrigger(true, 2f * item.scale, 0.8f, 1.8f, -1.7f, 0.2f, 6);
                            break;
                        case 2:
                            ((TinBroadswordSlash)proj.ModProjectile).WieldTrigger(true, 2f * item.scale, 0.7f, -2.5f, 2.3f, -0.4f, 6, 0, 1, 0, 1.2f);
                            break;
                    }
                }
            }
            return false;
        }
    }
    public class IronBroadSwordRE : GlobalItem
    {
        public bool normalend = false;
        public int phase = 0;
        public float timer;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.IronBroadsword;
        }
        public override void SetDefaults(Item item)
        {
            item.noUseGraphic = true;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.Shoot;
            item.shoot = ModContent.ProjectileType<IronBroadswordSlash>();
            item.useTime = item.useAnimation = 33;
            item.channel = true;
            item.autoReuse = false;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            var Cline0 = new TooltipLine(Mod, "Tooltip0", "剑意上限：1");
            var Cline1 = new TooltipLine(Mod, "Tooltip1", "\"矿石制造的剑。普通意味着稳定。\"");
            tooltips.Add(Cline0);
            tooltips.Add(Cline1);
        }
        public override void HoldItem(Item item, Player player)
        {
            player.GetModPlayer<DeusPlayer>().SwordPowerMax = 1f;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<IronBroadswordSlash>()] < 1)
                Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.Center, Vector2.Zero, ModContent.ProjectileType<IronBroadswordSlash>(), item.damage, item.knockBack, player.whoAmI);
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
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
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
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.type == ModContent.ProjectileType<IronBroadswordSlash>() && proj.owner == player.whoAmI && proj != null)
                {
                    switch (phase)
                    {
                        case 0:
                            ((IronBroadswordSlash)proj.ModProjectile).WieldTrigger(true, 2f * item.scale, 0.7f, -1.9f, 1.9f, 0.2f, 6);
                            break;
                        case 1:
                            ((IronBroadswordSlash)proj.ModProjectile).WieldTrigger(true, 2f * item.scale, 0.8f, 1.8f, -1.7f, 0.2f, 6);
                            break;
                        case 2:
                            ((IronBroadswordSlash)proj.ModProjectile).WieldTrigger(true, 2f * item.scale, 0.7f, -2.5f, 2.3f, -0.4f, 6, 0, 1, 0, 1.2f);
                            break;
                    }
                }
            }
            return false;
        }
    }
    public class LeadBroadSwordRE : GlobalItem
    {
        public bool normalend = false;
        public int phase = 0;
        public float timer;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.LeadBroadsword;
        }
        public override void SetDefaults(Item item)
        {
            item.noUseGraphic = true;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.Shoot;
            item.shoot = ModContent.ProjectileType<LeadBroadswordSlash>();
            item.useTime = item.useAnimation = 33;
            item.channel = true;
            item.autoReuse = false;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            var Cline0 = new TooltipLine(Mod, "Tooltip0", "剑意上限：1");
            var Cline1 = new TooltipLine(Mod, "Tooltip1", "\"矿石制造的剑。普通意味着稳定。\"");
            tooltips.Add(Cline0);
            tooltips.Add(Cline1);
        }
        public override void HoldItem(Item item, Player player)
        {
            player.GetModPlayer<DeusPlayer>().SwordPowerMax = 1f;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<LeadBroadswordSlash>()] < 1)
                Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.Center, Vector2.Zero, ModContent.ProjectileType<LeadBroadswordSlash>(), item.damage, item.knockBack, player.whoAmI);
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
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
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
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.type == ModContent.ProjectileType<LeadBroadswordSlash>() && proj.owner == player.whoAmI && proj != null)
                {
                    switch (phase)
                    {
                        case 0:
                            ((LeadBroadswordSlash)proj.ModProjectile).WieldTrigger(true, 2f * item.scale, 0.7f, -1.9f, 1.9f, 0.2f, 6);
                            break;
                        case 1:
                            ((LeadBroadswordSlash)proj.ModProjectile).WieldTrigger(true, 2f * item.scale, 0.8f, 1.8f, -1.7f, 0.2f, 6);
                            break;
                        case 2:
                            ((LeadBroadswordSlash)proj.ModProjectile).WieldTrigger(true, 2f * item.scale, 0.7f, -2.5f, 2.3f, -0.4f, 6, 0, 1, 0, 1.2f);
                            break;
                    }
                }
            }
            return false;
        }
    }
    public class SilverBroadSwordRE : GlobalItem
    {
        public bool normalend = false;
        public int phase = 0;
        public float timer;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.SilverBroadsword;
        }
        public override void SetDefaults(Item item)
        {
            item.noUseGraphic = true;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.Shoot;
            item.shoot = ModContent.ProjectileType<SilverBroadswordSlash>();
            item.useTime = item.useAnimation = 33;
            item.channel = true;
            item.autoReuse = false;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            var Cline0 = new TooltipLine(Mod, "Tooltip0", "剑意上限：1");
            var Cline1 = new TooltipLine(Mod, "Tooltip1", "\"矿石制造的剑。普通意味着稳定。\"");
            tooltips.Add(Cline0);
            tooltips.Add(Cline1);
        }
        public override void HoldItem(Item item, Player player)
        {
            player.GetModPlayer<DeusPlayer>().SwordPowerMax = 1f;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SilverBroadswordSlash>()] < 1)
                Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.Center, Vector2.Zero, ModContent.ProjectileType<SilverBroadswordSlash>(), item.damage, item.knockBack, player.whoAmI);
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
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
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
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.type == ModContent.ProjectileType<SilverBroadswordSlash>() && proj.owner == player.whoAmI && proj != null)
                {
                    switch (phase)
                    {
                        case 0:
                            ((SilverBroadswordSlash)proj.ModProjectile).WieldTrigger(true, 2f * item.scale, 0.7f, -1.9f, 1.9f, 0.2f, 6);
                            break;
                        case 1:
                            ((SilverBroadswordSlash)proj.ModProjectile).WieldTrigger(true, 2f * item.scale, 0.8f, 1.8f, -1.7f, 0.2f, 6);
                            break;
                        case 2:
                            ((SilverBroadswordSlash)proj.ModProjectile).WieldTrigger(true, 2f * item.scale, 0.7f, -2.5f, 2.3f, -0.4f, 6, 0, 1, 0, 1.2f);
                            break;
                    }
                }
            }
            return false;
        }
    }
    public class TungstenBroadSwordRE : GlobalItem
    {
        public bool normalend = false;
        public int phase = 0;
        public float timer;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.TungstenBroadsword;
        }
        public override void SetDefaults(Item item)
        {
            item.noUseGraphic = true;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.Shoot;
            item.shoot = ModContent.ProjectileType<TungstenBroadswordSlash>();
            item.useTime = item.useAnimation = 33;
            item.channel = true;
            item.autoReuse = false;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            var Cline0 = new TooltipLine(Mod, "Tooltip0", "剑意上限：1");
            var Cline1 = new TooltipLine(Mod, "Tooltip1", "\"矿石制造的剑。普通意味着稳定。\"");
            tooltips.Add(Cline0);
            tooltips.Add(Cline1);
        }
        public override void HoldItem(Item item, Player player)
        {
            player.GetModPlayer<DeusPlayer>().SwordPowerMax = 1f;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<TungstenBroadswordSlash>()] < 1)
                Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.Center, Vector2.Zero, ModContent.ProjectileType<TungstenBroadswordSlash>(), item.damage, item.knockBack, player.whoAmI);
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
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
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
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.type == ModContent.ProjectileType<TungstenBroadswordSlash>() && proj.owner == player.whoAmI && proj != null)
                {
                    switch (phase)
                    {
                        case 0:
                            ((TungstenBroadswordSlash)proj.ModProjectile).WieldTrigger(true, 2f * item.scale, 0.7f, -1.9f, 1.9f, 0.2f, 6);
                            break;
                        case 1:
                            ((TungstenBroadswordSlash)proj.ModProjectile).WieldTrigger(true, 2f * item.scale, 0.8f, 1.8f, -1.7f, 0.2f, 6);
                            break;
                        case 2:
                            ((TungstenBroadswordSlash)proj.ModProjectile).WieldTrigger(true, 2f * item.scale, 0.7f, -2.5f, 2.3f, -0.4f, 6, 0, 1, 0, 1.2f);
                            break;
                    }
                }
            }
            return false;
        }
    }
    public class GoldBroadSwordRE : GlobalItem
    {
        public bool normalend = false;
        public int phase = 0;
        public float timer;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.GoldBroadsword;
        }
        public override void SetDefaults(Item item)
        {
            item.noUseGraphic = true;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.Shoot;
            item.shoot = ModContent.ProjectileType<GoldBroadswordSlash>();
            item.useTime = item.useAnimation = 33;
            item.channel = true;
            item.autoReuse = false;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            var Cline0 = new TooltipLine(Mod, "Tooltip0", "剑意上限：1");
            var Cline1 = new TooltipLine(Mod, "Tooltip1", "\"矿石制造的剑。普通意味着稳定。\"");
            tooltips.Add(Cline0);
            tooltips.Add(Cline1);
        }
        public override void HoldItem(Item item, Player player)
        {
            player.GetModPlayer<DeusPlayer>().SwordPowerMax = 1f;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<GoldBroadswordSlash>()] < 1)
                Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.Center, Vector2.Zero, ModContent.ProjectileType<GoldBroadswordSlash>(), item.damage, item.knockBack, player.whoAmI);
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
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
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
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.type == ModContent.ProjectileType<GoldBroadswordSlash>() && proj.owner == player.whoAmI && proj != null)
                {
                    switch (phase)
                    {
                        case 0:
                            ((GoldBroadswordSlash)proj.ModProjectile).WieldTrigger(true, 2f * item.scale, 0.7f, -1.9f, 1.9f, 0.2f, 6);
                            break;
                        case 1:
                            ((GoldBroadswordSlash)proj.ModProjectile).WieldTrigger(true, 2f * item.scale, 0.8f, 1.8f, -1.7f, 0.2f, 6);
                            break;
                        case 2:
                            ((GoldBroadswordSlash)proj.ModProjectile).WieldTrigger(true, 2f * item.scale, 0.7f, -2.5f, 2.3f, -0.4f, 6, 0, 1, 0, 1.2f);
                            break;
                    }
                }
            }
            return false;
        }
    }
    public class PlatinumBroadSwordRE : GlobalItem
    {
        public bool normalend = false;
        public int phase = 0;
        public float timer;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.PlatinumBroadsword;
        }
        public override void SetDefaults(Item item)
        {
            item.noUseGraphic = true;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.Shoot;
            item.shoot = ModContent.ProjectileType<PlatinumBroadswordSlash>();
            item.useTime = item.useAnimation = 33;
            item.channel = true;
            item.autoReuse = false;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            var Cline0 = new TooltipLine(Mod, "Tooltip0", "剑意上限：1");
            var Cline1 = new TooltipLine(Mod, "Tooltip1", "\"矿石制造的剑。普通意味着稳定。\"");
            tooltips.Add(Cline0);
            tooltips.Add(Cline1);
        }
        public override void HoldItem(Item item, Player player)
        {
            player.GetModPlayer<DeusPlayer>().SwordPowerMax = 1f;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<PlatinumBroadswordSlash>()] < 1)
                Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.Center, Vector2.Zero, ModContent.ProjectileType<PlatinumBroadswordSlash>(), item.damage, item.knockBack, player.whoAmI);
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
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
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
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.type == ModContent.ProjectileType<PlatinumBroadswordSlash>() && proj.owner == player.whoAmI && proj != null)
                {
                    switch (phase)
                    {
                        case 0:
                            ((PlatinumBroadswordSlash)proj.ModProjectile).WieldTrigger(true, 2f * item.scale, 0.7f, -1.9f, 1.9f, 0.2f, 6);
                            break;
                        case 1:
                            ((PlatinumBroadswordSlash)proj.ModProjectile).WieldTrigger(true, 2f * item.scale, 0.8f, 1.8f, -1.7f, 0.2f, 6);
                            break;
                        case 2:
                            ((PlatinumBroadswordSlash)proj.ModProjectile).WieldTrigger(true, 2f * item.scale, 0.7f, -2.5f, 2.3f, -0.4f, 6, 0, 1, 0, 1.2f);
                            break;
                    }
                }
            }
            return false;
        }
    }

    public class CopperBroadswordSlash : DeusGlobalSwordSlash
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.CopperBroadsword;
        public override void RegisterVariables()
        {
            SwordDust1 = DustID.Copper;
            SlashColor = new Color(235, 166, 135);
        }
        public override void Appear()
        {

        }
    }
    public class TinBroadswordSlash : DeusGlobalSwordSlash
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.TinBroadsword;
        public override void RegisterVariables()
        {
            SwordDust1 = DustID.Tin;
            SlashColor = new Color(187, 165, 124);
        }
        public override void Appear()
        {

        }
    }
    public class IronBroadswordSlash : DeusGlobalSwordSlash
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.IronBroadsword;
        public override void RegisterVariables()
        {
            SwordDust1 = DustID.Iron;
            SlashColor = new Color(189, 159, 139);
        }
        public override void Appear()
        {

        }
    }
    public class LeadBroadswordSlash : DeusGlobalSwordSlash
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.LeadBroadsword;
        public override void RegisterVariables()
        {
            SwordDust1 = DustID.Lead;
            SlashColor = new Color(104, 140, 150) * 1.5f;
        }
        public override void Appear()
        {

        }
    }
    public class SilverBroadswordSlash : DeusGlobalSwordSlash
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.SilverBroadsword;
        public override void RegisterVariables()
        {
            SwordDust1 = DustID.Silver;
            SlashColor = Color.Silver;
        }
        public override void Appear()
        {

        }
    }
    public class TungstenBroadswordSlash : DeusGlobalSwordSlash
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.TungstenBroadsword;
        public override void RegisterVariables()
        {
            SwordDust1 = DustID.Tungsten;
            SlashColor = Color.LightGreen;
        }
        public override void Appear()
        {

        }
    }
    public class GoldBroadswordSlash : DeusGlobalSwordSlash
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.GoldBroadsword;
        public override void RegisterVariables()
        {
            SwordDust1 = DustID.Gold;
            SlashColor = Color.Gold;
        }
        public override void Appear()
        {

        }
    }
    public class PlatinumBroadswordSlash : DeusGlobalSwordSlash
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.PlatinumBroadsword;
        public override void RegisterVariables()
        {
            SwordDust1 = DustID.Platinum;
            SlashColor = Color.Silver * 1.5f;
        }
        public override void Appear()
        {

        }
    }
}
