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
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using Terraria.WorldBuilding;
using Terraria.GameContent.Generation;
using StructureHelper;

namespace DeusMod
{

    public class DeusWorld : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int RuinsOfTheElder = tasks.FindIndex((GenPass genpass) => genpass.Name.Equals("Micro Biomes"));
            if (RuinsOfTheElder != -1)
            {
                tasks.Insert(RuinsOfTheElder + 1, new PassLegacy("Ruins of the Elder", delegate (GenerationProgress progress, GameConfiguration configuration)
                {
                    progress.Message = "Ruins of the Elder";
                    WorldGenerationMethods.MoonAltar();
                }));
            }
        }
    }
}
