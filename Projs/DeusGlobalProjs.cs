using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using DeusMod;
using Microsoft.Xna.Framework;

namespace DeusMod.Projs
{
    public class DeusGlobalProjs : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public int ClashType = 0; //0代表常规不穿透，可以被近战格挡，1代表触发性射弹，近战攻击直接触发kill里的后续射弹，2代表扩散/爆炸，无法格挡，3代表装饰性，无法格挡
        /*public override void OnHitPlayer(Projectile projectile, Player target, int damage, bool crit)
        {
            if (projectile.type == ProjectileID.DemonSickle || projectile.type == ProjectileID.UnholyTridentHostile || projectile.type == ProjectileID.DemonScythe || projectile.type == ProjectileID.ImpFireball)
            {
                target.AddBuff(ModContent.BuffType<ProfanedFlame>(), 60000000);
            }
        }*/
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            switch (projectile.type)
            {
                case ProjectileID.SlimeGun:
                    target.AddBuff(BuffID.Slimed, 1);
                    break;
            }
            base.OnHitNPC(projectile, target, hit, damageDone);
        }
    }
}
