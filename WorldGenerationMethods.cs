using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
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
using Terraria.WorldBuilding;
using StructureHelper;

namespace DeusMod
{
    public class WorldGenerationMethods : ModSystem
    {
        public static void MoonAltar()
        {
            Point16 pos = new Point16(3000, 1800);
            Generator.GenerateStructure("Structures/MoonAltar", pos, DeusMod.Instance);
        }
    }
}
