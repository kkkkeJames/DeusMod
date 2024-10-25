using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeusMod.Helpers
{
    public static class DeusModMathHelper
    {
        public static float EucilidDistanceHelper(Vector2 a, Vector2 b)
        {
            return (float)Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
        }
        public static float PythagoreanHelper(float a, float b)
        {
            //勾股定律，不必多说，a和b代表边长
            return (float)Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
        }
        public static float EllipseRadiusHelper(float a, float b, float rot)
        {
            //a是长半径
            //b是短半径
            //inirot为需要计算半径的目标角度
            return (float)Math.Sqrt(Math.Pow(a, 2) * Math.Pow(b, 2) / (Math.Pow(a, 2) * Math.Pow((Math.Sin(rot)), 2) + Math.Pow(b, 2) * Math.Pow((Math.Cos(rot)), 2)));
        }
        public static float LogisticHelper(float a, float b, float pow, float red, float x)
        {
            return (float)((a / (1 + ((a / b) - 1) * Math.Pow(Math.E, pow * x))) - red);
        }
    }
}
