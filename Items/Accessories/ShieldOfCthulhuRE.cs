using DeusMod.NPCs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DeusMod.Items.Accessories
{
    public class ShieldOfCthulhuRE : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.EoCShield;
        }
        public override void SetDefaults(Item entity)
        {
            base.SetDefaults(entity);
        }
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            base.UpdateAccessory(item, player, hideVisual);
            player.dashType = 0;
            player.GetModPlayer<ShieldOfCthulhuModPlayer>().SocEquipped = true;
        }
    }
    public class ShieldOfCthulhuModPlayer : ModPlayer
    {
        // Direction
        public const int DashRight = 2;
        public const int DashLeft = 3;

        public const int DashCooldown = 50; // The cooldown of a dash. If less than duration, it is possible to initiate another dash while dashing
        public const int DashDuration = 35; // The duration of a dash. 

        public const float DashVelocity = 12f;

        // -1 = not tapping, also can be dashright/dashleft
        public int DashDir = -1;

        public bool SocEquipped = false; // Whether the player equipped SoC
        public int DashDelay = 0; // The cooldown of cooldown of dash, or how many frames left before the player can dash
        public int DashTimer = 0; // How many frames left for dashing
        public int DashHit = -1;// Target of dashing

        public override void ResetEffects()
        {
            // Reset SocEquipped to false, but if the player equip SoC, it will still be true
            SocEquipped = false;

            // Reset dash direction by times of and time between tappings
            DashDir = Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[DashRight] < 15 ? DashRight : (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[DashLeft] < 15 ? DashLeft : -1);
        }

        // Dash
        public override void PreUpdateMovement()
        {
            if (CanUseDash() && DashDir != -1 && DashDelay == 0) // Can use dash && have a dash direction && with no dash cooldown
            {
                Vector2 newVelocity = Player.velocity;

                switch (DashDir)
                {
                    case DashLeft when Player.velocity.X > -DashVelocity:
                    case DashRight when Player.velocity.X < DashVelocity:
                        {
                            // dash direction and speed
                            float dashDirection = DashDir == DashRight ? 1 : -1;
                            newVelocity.X = dashDirection * DashVelocity;
                            break;
                        }
                    default:
                        return; // not dashing if current speed exceeds dash full speed
                }
                
                // After examining if the spped exceeds, there will be a dash
                // Set the timers for dash
                DashDelay = DashCooldown; // Reset the dash delay to max cd (so in next frame this function will not be visited)
                DashTimer = DashDuration; // Reset the dash timer to duration
                Player.velocity = newVelocity; // Set the velocity to dashing

                DashHit = -1; // Set the dash hit target to default
            }

            if (DashDelay > 0)
                DashDelay--; // Calculating dash cooldown

            if (DashTimer > 0) // When dashing...
            {
                Player.eocDash = DashTimer - 1;
                Player.armorEffectDrawShadowEOCShield = true; // Since we are trying to mimic SoC, we want visual effects of SoC

                DashTimer--;

                // Check if the player slams into an enemy
                if (DashHit < 0) // if not...
                {
                    Rectangle rectangle = new((int)(Player.position.X + (Player.velocity.X * 0.5) - 4.0), (int)(Player.position.Y + (Player.velocity.Y * 0.5) - 4.0), Player.width + 8, Player.height + 8);
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (!npc.active || npc.dontTakeDamage || npc.friendly || (npc.aiStyle == 112 && !(npc.ai[2] <= 1f)) || ((npc.CountsAsACritter || npc.lifeMax <= 5) && Player.dontHurtCritters) || !rectangle.Intersects(npc.Hitbox) || !npc.noTileCollide && !Collision.CanHit(Player.position, Player.width, Player.height, npc.position, npc.width, npc.height) || !Player.CanNPCBeHitByPlayerOrPlayerProjectile(npc))
                        {
                            continue;
                        }

                        Rectangle rect = npc.getRect();
                        if (rectangle.Intersects(rect) && (npc.noTileCollide || Player.CanHit(npc)))
                        {
                            float dmg = Player.GetDamage(DamageClass.Melee).ApplyTo(30);
                            float kb = Player.GetTotalKnockback(DamageClass.Melee).ApplyTo(9);
                            bool crit = false;

                            if (Player.kbGlove)
                                kb *= 2f;

                            if (Player.kbBuff)
                                kb *= 1.5f;

                            if (Main.rand.Next(100) < Player.GetTotalCritChance(DamageClass.Melee))
                            {
                                crit = true;
                            }

                            int hitDir = Player.velocity.X < 0f ? -1 : (Player.velocity.X > 0f ? 1 : Player.direction);

                            // Apply damage and knockout
                            if (Player.whoAmI == Main.myPlayer)
                            {
                                Player.ApplyDamageToNPC(npc, (int)dmg, kb, hitDir, crit);
                                npc.GetGlobalNPC<DeusGlobalBuffNPC>().knockoutgauge += 1500;
                            }
                            // Set SoC visual, set recoil velocity, set immunity, set dashhit, set dashdelay (as we don't want to dash during recoil), most of them are from source code
                            Player.eocDash = 10; // This is purely for visual
                            DashTimer = 10;
                            DashDelay = 30;
                            Player.velocity.X = -hitDir * 9;
                            Player.velocity.Y = -4f;
                            Player.GiveImmuneTimeForCollisionAttack(10);
                            DashHit = i;
                        }
                    }
                }
                // When recoil/near the end of a dash, the player needs to slow down
                if ((!Player.controlLeft || !(Player.velocity.X < 0f)) && (!Player.controlRight || !(Player.velocity.X > 0f)) && DashTimer <= 10)
                {
                    Player.velocity.X *= 0.95f;
                }
            }
            if (DashTimer == 0) // Reset dash hit when timer is 0
                DashHit = -1;
        }
        private bool CanUseDash()
        { // 判断能不能用该冲刺的方法
            return Player.GetModPlayer<DeusPlayer>().ShieldEquipped
                && SocEquipped
                && Player.dashType == 0 // 玩家没有忍者或克盾冲刺 (优先让它们冲)
                && !Player.setSolar // 玩家没有穿日耀套
                && !Player.mount.Active; // 玩家没在坐骑上, 因为一边骑一边冲真的很怪欸
        }
    }
}
