using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGW
{
    public static class Utils
    {
        public static bool IntersectOrContains(this RectInt a, RectInt b)
        {
            FlipNegative(ref a);
            FlipNegative(ref b);
            bool c1 = a.xMin < b.xMax;
            bool c2 = a.xMax > b.xMin;
            bool c3 = a.yMin < b.yMax;
            bool c4 = a.yMax > b.yMin;
            bool isIntersect = (c1 && c2 && c3 && c4);
            return isIntersect;
        }
        public static void FlipNegative(ref RectInt r)
        {
            if (r.width < 0)
                r.x -= (r.width *= -1);

            if (r.height < 0)
                r.y -= (r.height *= -1);
        }
        public static float GetRandomNumberInRange(this System.Random random, double minNumber, double maxNumber)
        {
            return (float)(random.NextDouble() * (maxNumber - minNumber) + minNumber);
        }

        public static(int x, int y) XYFromIndex(int index, int size)
        {
            var y = index / size;
            var x = index % size;
            return (x: x, y: y);
        }

        public static void DebugDrawColoredRectangle(RectInt rect, Color color)
        {
            Debug.DrawLine(new Vector3(rect.xMin, rect.yMin), new Vector3(rect.xMax, rect.yMin), color);
            Debug.DrawLine(new Vector3(rect.xMin, rect.yMin), new Vector3(rect.xMin, rect.yMax), color);
            Debug.DrawLine(new Vector3(rect.xMax, rect.yMax), new Vector3(rect.xMax, rect.yMin), color);
            Debug.DrawLine(new Vector3(rect.xMax, rect.yMax), new Vector3(rect.xMin, rect.yMax), color);
        }
        public static void Shuffle<T>(this IList<T> list, System.Random rng)
        {
            int n = list.Count;

            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}

