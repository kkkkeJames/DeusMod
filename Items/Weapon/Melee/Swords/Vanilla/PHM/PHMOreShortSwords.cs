using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using DeusMod.Projs;
using System.Collections.Generic;
using DeusMod.Core;
using System;

namespace DeusMod.Items.Weapon.Melee.Swords.Vanilla.PHM
{
    public class CopperShortSwordRE : GlobalItem
    {
        public bool normalend = false;
        public int phase = 0;
        public float timer;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.CopperShortsword;
        }
        public override void SetDefaults(Item item)
        {
            item.noUseGraphic = true;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.Shoot;
            item.shoot = ModContent.ProjectileType<CopperShortSwordSlash>();
            item.useTime = item.useAnimation = 21;
            item.channel = true;
            item.autoReuse = false;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            var Cline1 = new TooltipLine(Mod, "Tooltip1", "\"矿石制造的短剑。短小的样式使得短剑更便于使用。\"");
            tooltips.Add(Cline1);
        }
        public override void HoldItem(Item item, Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<CopperShortSwordSlash>()] < 1)
                Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.Center, Vector2.Zero, ModContent.ProjectileType<CopperShortSwordSlash>(), item.damage, item.knockBack, player.whoAmI);
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
                if (proj.type == ModContent.ProjectileType<CopperShortSwordSlash>() && proj.owner == player.whoAmI && proj != null)
                {
                    switch (phase)
                    {
                        case 0:
                            ((CopperShortSwordSlash)proj.ModProjectile).StingerTrigger(true, 0.2f);
                            break;
                        case 1:
                            ((CopperShortSwordSlash)proj.ModProjectile).StingerTrigger(true, 0.2f);
                            break;
                        case 2:
                            ((CopperShortSwordSlash)proj.ModProjectile).StingerTrigger(true, -0.4f, 1.2f);
                            break;
                    }
                }
            }
            return false;
        }
    }
    public class TinShortSwordRE : GlobalItem
    {
        public bool normalend = false;
        public int phase = 0;
        public float timer;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.TinShortsword;
        }
        public override void SetDefaults(Item item)
        {
            item.noUseGraphic = true;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.Shoot;
            item.shoot = ModContent.ProjectileType<TinShortSwordSlash>();
            item.useTime = item.useAnimation = 21;
            item.channel = true;
            item.autoReuse = false;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            var Cline1 = new TooltipLine(Mod, "Tooltip1", "\"矿石制造的短剑。短小的样式使得短剑更便于使用。\"");
            tooltips.Add(Cline1);
        }
        public override void HoldItem(Item item, Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<TinShortSwordSlash>()] < 1)
                Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.Center, Vector2.Zero, ModContent.ProjectileType<TinShortSwordSlash>(), item.damage, item.knockBack, player.whoAmI);
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
                if (proj.type == ModContent.ProjectileType<TinShortSwordSlash>() && proj.owner == player.whoAmI && proj != null)
                {
                    switch (phase)
                    {
                        case 0:
                            ((TinShortSwordSlash)proj.ModProjectile).StingerTrigger(true, 0.2f);
                            break;
                        case 1:
                            ((TinShortSwordSlash)proj.ModProjectile).StingerTrigger(true, 0.2f);
                            break;
                        case 2:
                            ((TinShortSwordSlash)proj.ModProjectile).StingerTrigger(true, -0.4f, 1.2f);
                            break;
                    }
                }
            }
            return false;
        }
    }
    public class IronShortSwordRE : GlobalItem
    {
        public bool normalend = false;
        public int phase = 0;
        public float timer;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.IronShortsword;
        }
        public override void SetDefaults(Item item)
        {
            item.noUseGraphic = true;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.Shoot;
            item.shoot = ModContent.ProjectileType<IronShortSwordSlash>();
            item.useTime = item.useAnimation = 21;
            item.channel = true;
            item.autoReuse = false;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            var Cline1 = new TooltipLine(Mod, "Tooltip1", "\"矿石制造的短剑。短小的样式使得短剑更便于使用。\"");
            tooltips.Add(Cline1);
        }
        public override void HoldItem(Item item, Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<IronShortSwordSlash>()] < 1)
                Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.Center, Vector2.Zero, ModContent.ProjectileType<IronShortSwordSlash>(), item.damage, item.knockBack, player.whoAmI);
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
                if (proj.type == ModContent.ProjectileType<IronShortSwordSlash>() && proj.owner == player.whoAmI && proj != null)
                {
                    switch (phase)
                    {
                        case 0:
                            ((IronShortSwordSlash)proj.ModProjectile).StingerTrigger(true, 0.2f);
                            break;
                        case 1:
                            ((IronShortSwordSlash)proj.ModProjectile).StingerTrigger(true, 0.2f);
                            break;
                        case 2:
                            ((IronShortSwordSlash)proj.ModProjectile).StingerTrigger(true, -0.4f, 1.2f);
                            break;
                    }
                }
            }
            return false;
        }
    }
    public class LeadShortSwordRE : GlobalItem
    {
        public bool normalend = false;
        public int phase = 0;
        public float timer;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.LeadShortsword;
        }
        public override void SetDefaults(Item item)
        {
            item.noUseGraphic = true;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.Shoot;
            item.shoot = ModContent.ProjectileType<LeadShortSwordSlash>();
            item.useTime = item.useAnimation = 21;
            item.channel = true;
            item.autoReuse = false;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            var Cline1 = new TooltipLine(Mod, "Tooltip1", "\"矿石制造的短剑。短小的样式使得短剑更便于使用。\"");
            tooltips.Add(Cline1);
        }
        public override void HoldItem(Item item, Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<LeadShortSwordSlash>()] < 1)
                Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.Center, Vector2.Zero, ModContent.ProjectileType<LeadShortSwordSlash>(), item.damage, item.knockBack, player.whoAmI);
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
                if (proj.type == ModContent.ProjectileType<LeadShortSwordSlash>() && proj.owner == player.whoAmI && proj != null)
                {
                    switch (phase)
                    {
                        case 0:
                            ((LeadShortSwordSlash)proj.ModProjectile).StingerTrigger(true, 0.2f);
                            break;
                        case 1:
                            ((LeadShortSwordSlash)proj.ModProjectile).StingerTrigger(true, 0.2f);
                            break;
                        case 2:
                            ((LeadShortSwordSlash)proj.ModProjectile).StingerTrigger(true, -0.4f, 1.2f);
                            break;
                    }
                }
            }
            return false;
        }
    }
    public class SilverShortSwordRE : GlobalItem
    {
        public bool normalend = false;
        public int phase = 0;
        public float timer;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.SilverShortsword;
        }
        public override void SetDefaults(Item item)
        {
            item.noUseGraphic = true;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.Shoot;
            item.shoot = ModContent.ProjectileType<SilverShortSwordSlash>();
            item.useTime = item.useAnimation = 21;
            item.channel = true;
            item.autoReuse = false;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            var Cline1 = new TooltipLine(Mod, "Tooltip1", "\"矿石制造的短剑。短小的样式使得短剑更便于使用。\"");
            tooltips.Add(Cline1);
        }
        public override void HoldItem(Item item, Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SilverShortSwordSlash>()] < 1)
                Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.Center, Vector2.Zero, ModContent.ProjectileType<SilverShortSwordSlash>(), item.damage, item.knockBack, player.whoAmI);
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
                if (proj.type == ModContent.ProjectileType<SilverShortSwordSlash>() && proj.owner == player.whoAmI && proj != null)
                {
                    switch (phase)
                    {
                        case 0:
                            ((SilverShortSwordSlash)proj.ModProjectile).StingerTrigger(true, 0.2f);
                            break;
                        case 1:
                            ((SilverShortSwordSlash)proj.ModProjectile).StingerTrigger(true, 0.2f);
                            break;
                        case 2:
                            ((SilverShortSwordSlash)proj.ModProjectile).StingerTrigger(true, -0.4f, 1.2f);
                            break;
                    }
                }
            }
            return false;
        }
    }
    public class TungstenShortSwordRE : GlobalItem
    {
        public bool normalend = false;
        public int phase = 0;
        public float timer;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.TungstenShortsword;
        }
        public override void SetDefaults(Item item)
        {
            item.noUseGraphic = true;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.Shoot;
            item.shoot = ModContent.ProjectileType<TungstenShortSwordSlash>();
            item.useTime = item.useAnimation = 21;
            item.channel = true;
            item.autoReuse = false;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            var Cline1 = new TooltipLine(Mod, "Tooltip1", "\"矿石制造的短剑。短小的样式使得短剑更便于使用。\"");
            tooltips.Add(Cline1);
        }
        public override void HoldItem(Item item, Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<TungstenShortSwordSlash>()] < 1)
                Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.Center, Vector2.Zero, ModContent.ProjectileType<TungstenShortSwordSlash>(), item.damage, item.knockBack, player.whoAmI);
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
                if (proj.type == ModContent.ProjectileType<TungstenShortSwordSlash>() && proj.owner == player.whoAmI && proj != null)
                {
                    switch (phase)
                    {
                        case 0:
                            ((TungstenShortSwordSlash)proj.ModProjectile).StingerTrigger(true, 0.2f);
                            break;
                        case 1:
                            ((TungstenShortSwordSlash)proj.ModProjectile).StingerTrigger(true, 0.2f);
                            break;
                        case 2:
                            ((TungstenShortSwordSlash)proj.ModProjectile).StingerTrigger(true, -0.4f, 1.2f);
                            break;
                    }
                }
            }
            return false;
        }
    }
    public class GoldShortSwordRE : GlobalItem
    {
        public bool normalend = false;
        public int phase = 0;
        public float timer;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.GoldShortsword;
        }
        public override void SetDefaults(Item item)
        {
            item.noUseGraphic = true;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.Shoot;
            item.shoot = ModContent.ProjectileType<GoldShortSwordSlash>();
            item.useTime = item.useAnimation = 21;
            item.channel = true;
            item.autoReuse = false;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            var Cline1 = new TooltipLine(Mod, "Tooltip1", "\"矿石制造的短剑。短小的样式使得短剑更便于使用。\"");
            tooltips.Add(Cline1);
        }
        public override void HoldItem(Item item, Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<GoldShortSwordSlash>()] < 1)
                Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.Center, Vector2.Zero, ModContent.ProjectileType<GoldShortSwordSlash>(), item.damage, item.knockBack, player.whoAmI);
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
                if (proj.type == ModContent.ProjectileType<GoldShortSwordSlash>() && proj.owner == player.whoAmI && proj != null)
                {
                    switch (phase)
                    {
                        case 0:
                            ((GoldShortSwordSlash)proj.ModProjectile).StingerTrigger(true, 0.2f);
                            break;
                        case 1:
                            ((GoldShortSwordSlash)proj.ModProjectile).StingerTrigger(true, 0.2f);
                            break;
                        case 2:
                            ((GoldShortSwordSlash)proj.ModProjectile).StingerTrigger(true, -0.4f, 1.2f);
                            break;
                    }
                }
            }
            return false;
        }
    }
    public class PlatinumShortSwordRE : GlobalItem
    {
        public bool normalend = false;
        public int phase = 0;
        public float timer;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.PlatinumShortsword;
        }
        public override void SetDefaults(Item item)
        {
            item.noUseGraphic = true;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.Shoot;
            item.shoot = ModContent.ProjectileType<PlatinumShortSwordSlash>();
            item.useTime = item.useAnimation = 21;
            item.channel = true;
            item.autoReuse = false;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            var Cline1 = new TooltipLine(Mod, "Tooltip1", "\"矿石制造的短剑。短小的样式使得短剑更便于使用。\"");
            tooltips.Add(Cline1);
        }
        public override void HoldItem(Item item, Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<PlatinumShortSwordSlash>()] < 1)
                Projectile.NewProjectileDirect(player.GetSource_ItemUse(item), player.Center, Vector2.Zero, ModContent.ProjectileType<PlatinumShortSwordSlash>(), item.damage, item.knockBack, player.whoAmI);
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
                if (proj.type == ModContent.ProjectileType<PlatinumShortSwordSlash>() && proj.owner == player.whoAmI && proj != null)
                {
                    switch (phase)
                    {
                        case 0:
                            ((PlatinumShortSwordSlash)proj.ModProjectile).StingerTrigger(true, 0.2f);
                            break;
                        case 1:
                            ((PlatinumShortSwordSlash)proj.ModProjectile).StingerTrigger(true, 0.2f);
                            break;
                        case 2:
                            ((PlatinumShortSwordSlash)proj.ModProjectile).StingerTrigger(true, -0.4f, 1.2f);
                            break;
                    }
                }
            }
            return false;
        }
    }

    public class CopperShortSwordSlash: DeusGlobalSwordSlash
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.CopperShortsword;
        public override void RegisterVariables()
        {
            SwordDust1 = DustID.Copper;
            SlashColor = new Color(235, 166, 135);
            IsShortSword = true;
        }
        public override void Appear()
        {
            
        }
    }
    public class TinShortSwordSlash : DeusGlobalSwordSlash
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.TinShortsword;
        public override void RegisterVariables()
        {
            SwordDust1 = DustID.Tin;
            SlashColor = new Color(187, 165, 124);
            IsShortSword = true;
        }
        public override void Appear()
        {

        }
    }
    public class IronShortSwordSlash : DeusGlobalSwordSlash
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.IronShortsword;
        public override void RegisterVariables()
        {
            SwordDust1 = DustID.Iron;
            SlashColor = new Color(189, 159, 139);
            IsShortSword = true;
        }
        public override void Appear()
        {

        }
    }
    public class LeadShortSwordSlash : DeusGlobalSwordSlash
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.LeadShortsword;
        public override void RegisterVariables()
        {
            SwordDust1 = DustID.Lead;
            SlashColor = new Color(104, 140, 150) * 1.5f;
            IsShortSword = true;
        }
        public override void Appear()
        {

        }
    }
    public class SilverShortSwordSlash : DeusGlobalSwordSlash
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.SilverShortsword;
        public override void RegisterVariables()
        {
            SwordDust1 = DustID.Silver;
            SlashColor = Color.Silver;
            IsShortSword = true;
        }
        public override void Appear()
        {

        }
    }
    public class TungstenShortSwordSlash : DeusGlobalSwordSlash
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.TungstenShortsword;
        public override void RegisterVariables()
        {
            SwordDust1 = DustID.Tungsten;
            SlashColor = Color.LightGreen;
            IsShortSword = true;
        }
        public override void Appear()
        {

        }
    }
    public class GoldShortSwordSlash : DeusGlobalSwordSlash
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.GoldShortsword;
        public override void RegisterVariables()
        {
            SwordDust1 = DustID.Gold;
            SlashColor = Color.Gold;
            IsShortSword = true;
        }
        public override void Appear()
        {

        }
    }
    public class PlatinumShortSwordSlash : DeusGlobalSwordSlash
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.PlatinumShortsword;
        public override void RegisterVariables()
        {
            SwordDust1 = DustID.Platinum;
            SlashColor = Color.Silver * 1.5f;
            IsShortSword = true;
        }
        public override void Appear()
        {

        }
    }
}
