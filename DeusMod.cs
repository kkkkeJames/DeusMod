using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Terraria.Graphics.Effects;
using ReLogic.Content;
using Terraria.UI;
using DeusMod.Projs;
using DeusMod.Core.Systems;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;

namespace DeusMod
{
	public class DeusMod : Mod
	{
        //��Ϊmod��ʵ��������shaderͨ����Ҫ��
		public static DeusMod Instance { get; set; }
        private List<IOrderedLoadable> loadCache;
        public DeusMod()
        {
			Instance = this;
        }
        public override void Load()
        {
            loadCache = new List<IOrderedLoadable>();

            foreach (Type type in Code.GetTypes())
            {
                if (!type.IsAbstract && type.GetInterfaces().Contains(typeof(IOrderedLoadable))) //�жϼ�����ӿ�
                {
                    var instance = Activator.CreateInstance(type);//����ʵ��
                    loadCache.Add(instance as IOrderedLoadable);//���ӿ���ӵ��б���
                }

                loadCache.Sort((n, t) => n.Priority.CompareTo(t.Priority));//�������ȶ�����
            }

            for (int k = 0; k < loadCache.Count; k++)
            {
                loadCache[k].Load(); //��������loadCache������
                SetLoadingText("Loading " + loadCache[k].GetType().Name); //������ʾ��������
            }
        }

        public override void Unload()
        {
            if (loadCache is not null)
            {
                foreach (var loadable in loadCache)
                {
                    loadable.Unload();
                }

                loadCache = null;
            }
        }

        public static void SetLoadingText(string text)
        {
            var Interface_loadMods = typeof(Mod).Assembly.GetType("Terraria.ModLoader.UI.Interface")!.GetField("loadMods", BindingFlags.NonPublic | BindingFlags.Static)!;
            var UIProgress_set_SubProgressText = typeof(Mod).Assembly.GetType("Terraria.ModLoader.UI.UIProgress")!.GetProperty("SubProgressText", BindingFlags.Public | BindingFlags.Instance)!.GetSetMethod()!;

            UIProgress_set_SubProgressText.Invoke(Interface_loadMods.GetValue(null), new object[] { text });
        }
    }
}