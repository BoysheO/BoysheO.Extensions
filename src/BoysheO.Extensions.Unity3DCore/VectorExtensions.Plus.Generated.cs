//auto generate,don't edit
using System;
using UnityEngine;

namespace BoysheO.Extensions.Unity3DCore
{
    public static partial class VectorExtensions
    {
        public static Vector2 Plus(this Vector2 src, float x,float y)
        {
            return new Vector2(src.x + x,src.y + y);
        }
        public static Vector3 Plus(this Vector3 src, float x,float y,float z)
        {
            return new Vector3(src.x + x,src.y + y,src.z + z);
        }
        public static Vector4 Plus(this Vector4 src, float x,float y,float z,float w)
        {
            return new Vector4(src.x + x,src.y + y,src.z + z,src.w + w);
        }
        public static Vector2Int Plus(this Vector2Int src, int x,int y)
        {
            return new Vector2Int(src.x + x,src.y + y);
        }
        public static Vector3Int Plus(this Vector3Int src, int x,int y,int z)
        {
            return new Vector3Int(src.x + x,src.y + y,src.z + z);
        }
    }
}