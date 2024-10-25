using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using System;
using System.Collections.Generic;
using DeusMod.Dusts;
using DeusMod.NPCs;
using DeusMod.Projs;
using DeusMod.Core.Systems;

namespace DeusMod.Items.Weapon.Ranged.Guns.Vanilla.Musket
{
    public class BoomstickRE : GlobalGun
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.Boomstick;
        }
        public override void SetDefaults(Item item)//所有的重置枪械的装载弹和射击数据在globalgun里
        {
            item.autoReuse = false;
            base.SetDefaults(item);
        }
        public override void NeedReload(Item item)
        {
            needreload = false;
        }
        public override void HoldItem(Item item, Player player)
        {
            base.HoldItem(item, player);
        }
        public override bool CanUseItem(Item item, Player player)
        {
            return base.CanUseItem(item, player);
        }
        public override void SetRecoil(Item item, Player player)
        {
            int recoiltime = item.useTime / 8;
            if (useTime <= recoiltime) player.itemRotation -= player.direction * 0.14f;
            else if (useTime <= recoiltime * 2) player.itemRotation += player.direction * 0.14f;
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            base.Shoot(item, player, source, position, velocity, type, damage, knockback);
            float rot = player.direction == 1 ? 0 : (float)Math.PI;
            Dust Gunfire = Dust.NewDustPerfect(position + new Vector2(item.width, 0).RotatedBy(player.itemRotation + rot), ModContent.DustType<Gunfire_L>());
            Gunfire.rotation = player.itemRotation + rot;
            used = true;
            return true;
        }
    }

}