using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace BoysheO.Extensions.Unity3DCore
{
    public static class GameObjectExtensions
    {
        public static T AddOrGetComponent<T>(this GameObject go) where T : Component
        {
            var t = go.GetComponent<T>();
            if (t == null) t = go.AddComponent<T>();

            return t;
        }
        
        /// <summary>
        /// 如果没有指定组件，则抛异常
        /// </summary>
        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetRequireComponent<T>(this GameObject o)
            where T : Component
        {
            var com = o.GetComponent<T>();
            if (com is null) throw new Exception($"{o.name} is missing component {typeof(T).Name}");
            return com;
        }

        /// <summary>
        /// 如果没有指定组件，则抛异常
        /// </summary>
        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetRequireComponentInChildren<T>(this GameObject o)
            where T : Component
        {
            return o.GetComponentInChildren<T>() ??
                   throw new Exception($"{o.name} missing child component {typeof(T).Name}");
        }

        /// <summary>
        /// 判断go下是否具有指定组件（引用判空，非==null判定）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasComponent<T>(this GameObject go) where T : Component
        {
            return !(go.GetComponent<T>() is null);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDestroyed(this GameObject component)
        {
            return component == null;
        }
    }
}