using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DeusMod.Projs.Melee;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using System.Collections.Generic;
using Terraria.GameInput;
using Terraria.Localization;

namespace DeusMod.PlayerControl
{
    public class DeusPlayerBlock : ModPlayer
    {
        public float blocktime = 0; //正在格挡时不断增加，在数据大于5之前格挡算作完美格挡
        public bool HasBlockShield = false; //是否持有可以格挡的盾牌
        public bool IsBlock = false; //是否正在格挡
        public bool Parry = false; //是否完美格挡
        public bool Recover = false; //是否有后摇，如果有后摇则blocktime从30开始减到0
        public override void ResetEffects()
        {
            HasBlockShield = false;
            if (Recover)
            {
                if(blocktime >= 0) 
                    blocktime--;
                if (blocktime < 0)
                    Recover = false;
            }
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (HasBlockShield && (Player.itemTime == 0 && Player.itemAnimation == 0))
            {
                if ((BlockKeybindSystem.BlockKeybind.Current || ((IsBlock && blocktime < 30) && !Parry)) && !Recover)
                {
                    blocktime++;
                    IsBlock = true;
                }
                else
                {
                    if (IsBlock)
                    {
                        if (!Parry)
                        {
                            Recover = true;
                            blocktime = 30;
                        }
                        else
                        {
                            blocktime = 0;
                            Parry = false;
                        }
                    }
                    IsBlock = false;
                }
            }
        }
        public override bool CanUseItem(Item item)
        {
            if (BlockKeybindSystem.BlockKeybind.Current || IsBlock || (Recover && blocktime > 0))
                return false;
            else return base.CanUseItem(item);
        }
        public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
        {
            if (IsBlock && blocktime > 0 && blocktime <= 6)
            {
                string text = "Parry!";
                CombatText.NewText(new Rectangle((int)Player.Center.X, (int)Player.Center.Y, Player.width, Player.height), Color.Yellow, Language.GetTextValue(text), false, false);
                Player.immune = true;
                Player.immuneTime = 60;
                Parry = true;
                return true;
            }
            else return base.ImmuneTo(damageSource, cooldownCounter, dodgeable);
        }
    }

    public class BlockKeybindSystem : ModSystem
    {
        public static ModKeybind BlockKeybind { get; private set; }
        public override void Load()
        {
            BlockKeybind = KeybindLoader.RegisterKeybind(Mod, "Block", "G");
        }
        public override void Unload()
        {
            BlockKeybind = null;
        }
    }
    public abstract class ShieldBlockProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.hide = true;
            Projectile.alpha = 75;
            base.SetDefaults();
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center + new Vector2(Projectile.width * Projectile.scale / 2 + player.width * player.direction / 2, 0);
            Projectile.direction = player.direction;
            if (player.GetModPlayer<DeusPlayerBlock>().IsBlock && player.GetModPlayer<DeusPlayerBlock>().blocktime <= 6)
                Projectile.ai[1] = 0;
            else if (player.GetModPlayer<DeusPlayerBlock>().Recover)
                Projectile.ai[1] = 2;
            else Projectile.ai[1] = 1;

            if (Projectile.ai[1] == 0)
                Projectile.scale += 0.2f;
            if (Projectile.ai[1] == 2)
            {
                Projectile.alpha += 6;
                Projectile.scale -= 0.04f;
            }

            if (Projectile.alpha >= 255 || Projectile.scale <= 1) Projectile.Kill();
            if (player.GetModPlayer<DeusPlayerBlock>().blocktime == 0 && !BlockKeybindSystem.BlockKeybind.Current)
                Projectile.Kill();
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            Main.instance.DrawCacheProjsOverPlayers.Add(index);
        }
    }
}
