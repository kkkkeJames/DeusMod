using DeusMod.Core.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.UI;

namespace DeusMod.Core.Loaders
{
    class UILoader : IOrderedLoadable
    {
        public float Priority { get => 1.1f; }

        public static List<UserInterface> UserInterfaces;
        public static List<SmartUIState> UIStates;

        public void Load()
        {
            if (Main.dedServ)
                return;

            Mod Mod = DeusMod.Instance;

            UserInterfaces = new List<UserInterface>();
            UIStates = new List<SmartUIState>();

            foreach (Type t in AssemblyManager.GetLoadableTypes(Mod.Code))
            {
                if (t.IsSubclassOf(typeof(SmartUIState)))
                {
                    var state = (SmartUIState)Activator.CreateInstance(t, null);
                    var userInterface = new UserInterface();
                    userInterface.SetState(state);

                    UIStates?.Add(state);
                    UserInterfaces?.Add(userInterface);
                }
            }
        }

        public void Unload()
        {
            if (UIStates != null)
                UIStates.ForEach(n => n.Unload());
            UserInterfaces = null;
            UIStates = null;
        }

        public static void AddLayer(List<GameInterfaceLayer> layers, UserInterface userInterface, UIState state, int index, bool visible, InterfaceScaleType scale)
        {
            string name = state == null ? "Unknown" : state.ToString();
            layers.Insert(index, new LegacyGameInterfaceLayer("DeusMod: " + name,
                delegate
                {
                    if (visible)
                    {
                        userInterface.Update(Main._drawInterfaceGameTime);
                        state.Draw(Main.spriteBatch);
                    }
                    return true;
                }, scale));
        }

        public static T GetUIState<T>() where T : SmartUIState => UIStates.FirstOrDefault(n => n is T) as T;

        public static UserInterface GetUserInterface<T>() where T : SmartUIState => UserInterfaces.FirstOrDefault(n => n.CurrentState is T);
    }

    class UISystem : ModSystem
    {
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            for (int k = 0; k < UILoader.UIStates.Count; k++)
            {
                var state = UILoader.UIStates[k];
                UILoader.AddLayer(layers, UILoader.UserInterfaces[k], state, state.UILayer(layers), state.Visible, state.Scale);
            }
        }
    }
}
