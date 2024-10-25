using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using DeusMod.Items.Weapon.Melee;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using DeusMod.Projs;

namespace DeusMod
{
    public class DeusPlayer : ModPlayer
    {
        #region 剑意变量
        public float SwordPowerMax = 0; //剑意槽的最大值
        public float SwordPower = 0; //当前剑意值
        #endregion

        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();
            if (SwordPowerMax > 0)
                SwordPowerUI.visible = true;
            else SwordPowerUI.visible = false;
        }
        public bool ShieldEquipped; //是否装备盾牌，只能装备一件格挡盾/冲刺盾
        public static bool AODNbuff;
        public float AODNtime = 0;
        public static bool sticktoground;

        public bool reload;
        public static bool shootS;
        public override void ResetEffects()
        {
            ShieldEquipped = false; 
            if(AODNtime > 0) AODNbuff = true;
            else AODNbuff = false;
            AODNtime -= 1;
            sticktoground = false;
            reload = false;
            shootS = false;
        }
        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            base.OnHitByNPC(npc, hurtInfo);
        }
        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            base.OnHitByProjectile(proj, hurtInfo);
        }
        public override void OnHurt(Player.HurtInfo info)
        {
            AODNtime = 600;
            base.OnHurt(info);
        }
        public override void SetControls()
        {
            if (sticktoground)
            {
                Player.controlJump = false;
                Player.controlDown = false;
                Player.controlHook = false;
                Player.stairFall = false;
            }
        }
    }
}