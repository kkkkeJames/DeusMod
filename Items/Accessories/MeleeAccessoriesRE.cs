using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using DeusMod.Projs;
using DeusMod.Projs.Melee.SkillUI;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.GameContent;
using System;
using DeusMod.Helpers;
using DeusMod.Core;

namespace DeusMod.Items.Accessories
{
    public class MeleeAccessoriesRE : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.FeralClaws;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            switch (item.type)
            {
                case ItemID.FeralClaws:
                    var Cline0 = new TooltipLine(Mod, "Tooltip0", "近战速度提高8%");
                    tooltips.Add(Cline0);
                    break;
            }
        }
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            switch (item.type)
            {
                case ItemID.FeralClaws:
                    player.GetAttackSpeed(DamageClass.Melee) -= 0.04f;
                    player.autoReuseGlove = false;
                    break;
            }
        }
    }
}
