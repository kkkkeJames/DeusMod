using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace DeusMod.Core
{
    public abstract class ProjState //一个状态模式，放在射弹内部
    {
        public abstract void AI(ProjStateMachine projectile); //对ProjStateMachine类型的射弹实现的AI模式
    }
    public abstract class ProjStateMachine : ModProjectile
    {
        private List<ProjState> projStates = new List<ProjState>(); //所有AI模式的列表
        private Dictionary<string, int> stateDict = new Dictionary<string, int>(); //对应状态的字典
        public ProjState currentState => projStates[State - 1]; //当前的具体状态模式

        public int Timer //计时器
        {
            get { return (int)Projectile.ai[0]; }
            set { Projectile.ai[0] = value; }
        }
        private int State //当前的状态模式的序列
        {
            get { return (int)Projectile.ai[1]; }
            set { Projectile.ai[1] = value; }
        }

        public void SetState<T>() where T : ProjState //将射弹设置为一个已有状态
        {
            var name = typeof(T).FullName; //状态的类型名称
            if (!stateDict.ContainsKey(name)) throw new ArgumentException("这个状态并不存在"); //如果没有状态则throw exception
            State = stateDict[name]; //如果有则将当前的ai模式设置成对应序列
        }
        protected void RegisterState<T>(T state) where T : ProjState //注册一个新状态
        {
            var name = typeof(T).FullName; //状态的具体名称
            if (stateDict.ContainsKey(name)) throw new ArgumentException("这个状态已经注册过了"); //如果已经注册则throw exception
            projStates.Add(state); //在list里加入新的state
            stateDict.Add(name, projStates.Count); //如果没有则加入新的字典
        }
        public abstract void Initialize(); //初始化，注册所有的ProjState

        public sealed override void AI()
        {
            if (State == 0) //初始的state大多是0
            {
                Initialize(); //初始化并接入第一个ai
                State = 1;
            }

            AIBefore();
            currentState.AI(this); //执行currentstate所指向的ai
            AIAfter();
        }

        public virtual void AIBefore() { } //状态机前的ai

        public virtual void AIAfter() { } //状态机后的ai
    }
}
