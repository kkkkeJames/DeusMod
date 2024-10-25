using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using ReLogic.Graphics;
using Terraria.GameInput;
using DeusMod.Items.Ammo.Magnum;
using DeusMod.Items.Weapon.Ranged.Guns.LighteningHawk;
using Terraria.DataStructures;

namespace DeusMod.Items.Weapon.Ranged
{
    //装备枪械武器时水平端，开火时手臂对齐武器角度
    public class PlayerHoldGun : ModPlayer
    {
        public override void Load()
        {
            if (Main.dedServ)
                return;

            On_Player.ItemCheck_ApplyHoldStyle_Inner += (orig, player, mountOffset, sItem, heldItemFrame) => {
                if (player.HeldItem.DamageType == DamageClass.Ranged && (player.HeldItem.useAmmo == AmmoID.Bullet || player.HeldItem.useAmmo == ModContent.ItemType<MagnumBullet>()))
                {
                    player.ItemCheck_ApplyUseStyle(mountOffset, sItem, heldItemFrame);

                    return;
                }

                orig(player, mountOffset, sItem, heldItemFrame);
            };
            //修正持握枪械和枪械开火的角度，使得正常水平端，开火时手臂对齐武器
            On_Player.ItemCheck_ApplyUseStyle_Inner += (orig, player, mountOffset, sItem, heldItemFrame) => {
                if (player.itemTime == 0) player.itemRotation = 0;
                else player.SetCompositeArmFront(true, 0, player.itemRotation + (player.direction == 1 ? -MathHelper.Pi / 2 : MathHelper.Pi / 2));
                orig(player, mountOffset, sItem, heldItemFrame);

                if (player.HeldItem.DamageType == DamageClass.Ranged && (player.HeldItem.useAmmo == AmmoID.Bullet || player.HeldItem.useAmmo == ModContent.ItemType<MagnumBullet>()))
                {
                    // Fix rotation range.
                    if (player.itemRotation > MathHelper.Pi)
                    {
                        player.itemRotation -= MathHelper.TwoPi;
                    }

                    if (player.itemRotation < -MathHelper.Pi)
                    {
                        player.itemRotation += MathHelper.TwoPi;
                    }
                }
            };
            On_Player.PlayerFrame += (orig, player) => {
                if (player.HeldItem.DamageType == DamageClass.Ranged && (player.HeldItem.useAmmo == AmmoID.Bullet || player.HeldItem.useAmmo == ModContent.ItemType<MagnumBullet>()) && player.itemAnimation <= 0)
                {
                    InvokeWithForcedAnimation(player, () => orig(player));
                    return;
                }

                orig(player);
            };
            On_PlayerDrawLayers.DrawPlayer_27_HeldItem += (On_PlayerDrawLayers.orig_DrawPlayer_27_HeldItem orig, ref PlayerDrawSet drawInfo) => {
                var player = drawInfo.drawPlayer;
                
                //端持武器的判定——造成远程伤害，使用子弹或者马格南子弹
                if (player.HeldItem.DamageType == DamageClass.Ranged && (player.HeldItem.useAmmo == AmmoID.Bullet || player.HeldItem.useAmmo == ModContent.ItemType<MagnumBullet>()) && player.itemAnimation <= 0)
                {
                    //先强制进动画，然后恢复
                    ForceAnim(player, out int itemAnim, out int itemAnimMax);

                    orig(ref drawInfo);

                    RestoreAnim(player, itemAnim, itemAnimMax);

                    return;
                }

                orig(ref drawInfo);
            };
        }
        private static void ForceAnim(Player player, out int itemAnim, out int itemAnimMax)
        {
            itemAnim = player.itemAnimation;
            itemAnimMax = player.itemAnimationMax;

            player.itemAnimation = 1;
            player.itemAnimationMax = 2;
        }
        private static void RestoreAnim(Player player, int itemAnim, int itemAnimMax)
        {
            player.itemAnimation = itemAnim;
            player.itemAnimationMax = itemAnimMax;
        }
        private static void InvokeWithForcedAnimation(Player player, Action action)
        {
            ForceAnim(player, out int itemAnim, out int itemAnimMax);

            action();

            RestoreAnim(player, itemAnim, itemAnimMax);
        }
    }
    public abstract class GlobalGun : GlobalItem
    {
        public bool needreload = false;
        public int reloadTimeMax;
        public int reloadTime;
        public int bulletnum;
        public int bulletcapacity;
        public bool used = false;
        public int useTime = 0;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            //这个GlobalItem只对消耗普通子弹和马格南子弹的枪生效
            return entity.DamageType == DamageClass.Ranged && (entity.useAmmo == AmmoID.Bullet || entity.useAmmo == ModContent.ItemType<MagnumBullet>());
        }
        public override void SetDefaults(Item entity)
        {
            NeedReload(entity);
            base.SetDefaults(entity);
        }
        public override void HoldItem(Item item, Player player)
        {
            if (reloadTime > 0) reloadTime--; //如果有换弹时间则逐渐衰减
            if (reloadTime == 0 && bulletnum < bulletcapacity) //如果换弹时间结束则增加子弹并且视情况重置换弹时间
            {
                bulletnum++;
                if (bulletnum < bulletcapacity) reloadTime = reloadTimeMax;
            }
            if (used)
            {
                if (useTime < item.useTime) useTime++;
                else
                {
                    useTime = 0;
                    used = false;
                }
                SetRecoil(item, player);
            }
            base.HoldItem(item, player);
        }
        public override bool CanUseItem(Item item, Player player) //需要换弹的情况下没子弹无法使用
        {
            return !needreload || bulletnum > 0;
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.mouseRight) ModShooting(item);
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }
        public virtual void NeedReload(Item item)
        { 
        }
        public virtual void SetRecoil(Item item, Player player)
        {
        }
        public virtual void ModShooting(Item item)
        { 
        }
    }
}
