using System;
using UnityEngine;

namespace BoysheO.Extensions.Unity3DCore
{
    public static partial class VectorExtensions
    {
        public static float SqrDistance(this Vector2 a, Vector2 b)
        {
            return Vector2.SqrMagnitude(b - a);
        }

        public static float SqrDistance(this Vector3 a, Vector3 b)
        {
            return Vector3.SqrMagnitude(b - a);
        }

        /// <summary>
        /// v视作区间，求线性插值结果
        /// </summary>
        public static float Lerp(this Vector2 v, float t)
        {
            return v.x + v.y * t;
        }

        /// <summary>
        /// v视作区间，求线性插值结果
        /// </summary>
        public static float Lerp(this Vector2Int v, float t)
        {
            return v.x + v.y * t;
        }

        #region 解构函数

        // ReSharper disable once UseDeconstructionOnParameter
        public static void Deconstruct(this Vector3 v, out float x, out float y, out float z)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        // ReSharper disable once UseDeconstructionOnParameter
        public static void Deconstruct(this Vector3Int v, out int x, out int y, out int z)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        // ReSharper disable once UseDeconstructionOnParameter
        public static void Deconstruct(this Vector2 v, out float x, out float y)
        {
            x = v.x;
            y = v.y;
        }

        // ReSharper disable once UseDeconstructionOnParameter
        public static void Deconstruct(this Vector2Int v, out int x, out int y)
        {
            x = v.x;
            y = v.y;
        }


        // ReSharper disable once UseDeconstructionOnParameter
        public static void Deconstruct(this Vector4 v, out float x, out float y, out float z, out float w)
        {
            x = v.x;
            y = v.y;
            z = v.z;
            w = v.w;
        }

        // ReSharper disable once UseDeconstructionOnParameter
        public static void Deconstruct(this Color v, out float r, out float g, out float b, out float a)
        {
            r = v.r;
            g = v.g;
            b = v.b;
            a = v.a;
        }

        // ReSharper disable once UseDeconstructionOnParameter
        public static void Deconstruct(this Color32 v, out float r, out float g, out float b, out float a)
        {
            r = v.r;
            g = v.g;
            b = v.b;
            a = v.a;
        }

        #endregion


        public static float Dot(this Vector2 lhs, Vector2 rhs)
        {
            return Vector2.Dot(lhs, rhs);
        }

        public static float Dot(this Vector3 lhs, Vector3 rhs)
        {
            return Vector3.Dot(lhs, rhs);
        }

        // ReSharper disable once UseDeconstructionOnParameter
        public static Vector2 Dot(this Vector2 lhs, float factor)
        {
            return new Vector2(lhs.x * factor, lhs.y * factor);
        }

        // ReSharper disable once UseDeconstructionOnParameter
        public static Vector3 Dot(this Vector3 lhs, float factor)
        {
            return new Vector3(lhs.x * factor, lhs.y * factor, lhs.z * factor);
        }

        public static Vector2 ToVector2(this Vector3 vector3)
        {
            return vector3;
        }

        public static Vector3 ToVector3(this Vector2 vector2)
        {
            return vector2;
        }
    }
}