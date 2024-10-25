using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.DataStructures;
using System;
using System.Collections.Generic;
using DeusMod.Dusts;
using System.Linq;
using DeusMod.Items.Weapon.Ranged.Guns.Vanilla.Musket;

namespace DeusMod.NPCs
{
    public class TargetNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool Targetstucked
        {
            get => targetstucked; set => targetstucked = value;
        }
        public Vector2 Targetstuckedposition
        {
            get => targetstuckposition; set => targetstuckposition = value;
        }
        public Projectile Targetitself
        {
            get => targetitself; set => targetitself = value;
        }
        public bool targetstucked;
        public Vector2 targetstuckposition;
        public Projectile targetitself;
        /*public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.type == ModContent.ProjectileType<MusketSpecialBullet>() && targetstucked && (targetstuckposition - projectile.Center).Length() <= 30 && projectile.ai[1] != 1)
            {
                Player player = Main.player[projectile.owner];
                projectile.penetrate += 2;
                projectile.velocity *= 0.1f;
                projectile.timeLeft = 20;
                projectile.ai[1] = 1;
                targetitself.Kill();
                bool flag1 = Main.rand.NextBool(), flag2 = Main.rand.NextBool();
                int proj = Projectile.NewProjectile(player.GetSource_FromThis(), npc.Center + new Vector2(flag1 ? Main.rand.NextFloat(npc.width * 0.25f, npc.width * 0.3f) : Main.rand.NextFloat(-npc.width * 0.3f, -npc.width * 0.25f), flag2 ? Main.rand.NextFloat(npc.height * 0.25f, npc.height * 0.3f) : Main.rand.NextFloat(-npc.height * 0.3f, -npc.height * 0.25f)),
                    Vector2.Zero, ModContent.ProjectileType<MusketTarget>(), 0, 0, player.whoAmI, npc.whoAmI);
            }
            base.OnHitByProjectile(npc, projectile, hit, damageDone);
        }*/
    }
}