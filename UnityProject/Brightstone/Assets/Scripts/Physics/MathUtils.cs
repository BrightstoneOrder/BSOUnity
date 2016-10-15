using UnityEngine;
using System.Collections;
namespace Brightstone
{
    public static class MathUtils
    {
        public const float EPSILON = 0.000001f;

        public static bool Equals(float a, float b)
        {
            return a > b ? ((a - b) < EPSILON) : ((b - a) < EPSILON);
        }

        public static bool Equals(Vector3 a, Vector3 b)
        {
            return Equals(a.x, b.x) && Equals(a.y, b.y) && Equals(a.z, b.z);
        }

        public static Vector3 RandomVector(Vector3 min, Vector3 max)
        {
            return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
        }

        public static Vector3 ClampVector(Vector3 value, Vector3 min, Vector3 max)
        {
            return new Vector3(Mathf.Clamp(value.x, min.x, max.x),
                                Mathf.Clamp(value.y, min.y, max.y),
                                Mathf.Clamp(value.z, min.z, max.z));
        }

        public static float ClampAngle(float val)
        {
            if (val < -360)
                val += 360;
            if (val > 360)
                val -= 360;
            return val;
        }

        public static float ClampAngle(float val, float min, float max)
        {
            if (val < -360)
                val += 360;
            if (val > 360)
                val -= 360;
            return Mathf.Clamp(val, min, max);
        }


        public static Vector3 Direction(Vector3 to, Vector3 from)
        {
            return (to - from).normalized;
        }

        public static Vector2 Direction(Vector2 to, Vector2 from)
        {
            return (to - from).normalized;
        }

        public static Vector2 MidPoint(Vector2 a, Vector2 b)
        {
            return new Vector2((a.x + b.x) * 0.5f, (a.y + b.y) * 0.5f);
        }

        public static Vector3 MidPoint(Vector3 a, Vector3 b)
        {
            return new Vector3((a.x + b.x) * 0.5f, (a.y + b.y) * 0.5f, (a.z + b.z) * 0.5f);
        }

        public static Vector3 ScalarDistance(Vector3 a, Vector3 b)
        {
            return new Vector3(
                Mathf.Abs(a.x - b.x),
                Mathf.Abs(a.y - b.y),
                Mathf.Abs(a.z - b.z)
                );
        }

        public static Vector2 Abs(Vector2 a)
        {
            return new Vector2(Mathf.Abs(a.x), Mathf.Abs(a.y));
        }

        public static Vector3 Abs(Vector3 a)
        {
            return new Vector3(Mathf.Abs(a.x), Mathf.Abs(a.y), Mathf.Abs(a.z));
        }

        public static float DistanceSqr(Vector2 a, Vector2 b)
        {
            return (a - b).sqrMagnitude;
        }

        public static float DistanceSqr(Vector3 a, Vector3 b)
        {
            return (a - b).sqrMagnitude;
        }

        public static float DistanceXZ(Vector3 a, Vector3 b)
        {
            a.y = b.y;
            return Vector3.Distance(a, b);
        }

        public static float DistanceXZSqr(Vector3 a, Vector3 b)
        {
            a.y = b.y;
            return DistanceSqr(a, b);
        }

        public static int SafeAdd(int a, int b)
        {
            int value = a + b;
            if (value < a || value < b)
            {
                return int.MaxValue;
            }
            return value;
        }

        public static int SafeSubtract(int a, int b)
        {
            int value = a - b;
            if (value < 0)
            {
                return 0;
            }
            return value;
        }
    }

}