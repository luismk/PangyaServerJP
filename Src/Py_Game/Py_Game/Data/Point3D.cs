using PangyaAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Py_Game.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Point3D
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }


        public static Point3D operator -(Point3D PosA, Point3D PosB)
        {
            Point3D result = new Point3D()
            {
                X = PosA.X - PosB.X,
                Y = PosA.Y - PosB.Y,
                Z = PosA.Z - PosB.Z,
            };
            return result;
        }
        public static Point3D operator +(Point3D PosA, Point3D PosB)
        {
            Point3D result = new Point3D()
            {
                X = PosA.X + PosB.X,
                Y = PosA.Y + PosB.Y,
                Z = PosA.Z + PosB.Z,
            };
            return result;
        }
        public float Distance(Point3D PlayerPos)
        {
            return (this - PlayerPos).Length();
        }

        public float Length()
        {
            return Convert.ToSingle(Math.Sqrt(X * X + Y * Y));
        }

        public float HoleDistance(Point3D PosB)
        {
            return Convert.ToSingle(Math.Sqrt(Math.Pow(X - PosB.X, 2) + Math.Pow(Z - PosB.Z, 2)));
        }
    }
}
