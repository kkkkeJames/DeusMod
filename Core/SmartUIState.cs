using System.Collections.Generic;
using Terraria.UI;

namespace DeusMod.Core
{
    public abstract class SmartUIState : UIState
    {
        // 用于描述UI显示在哪个图层上
        public abstract int UILayer(List<GameInterfaceLayer> layers);
        // 缩放
        public virtual InterfaceScaleType Scale { get; set; } = InterfaceScaleType.UI;
        // 可见
        public virtual bool Visible { get; set; } = false;
        public virtual void Unload() { }
    }
}
