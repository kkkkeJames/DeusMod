using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeusMod.Core.Systems
{
    public interface IDrawWarp
    {
        void DrawWarp();
    }
    public interface IOrderedLoadable
    {
        void Load();
        void Unload();
        float Priority { get; }
    }
}
