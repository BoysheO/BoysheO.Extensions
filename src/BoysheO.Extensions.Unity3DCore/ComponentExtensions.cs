using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace BoysheO.Extensions.Unity3DCore
{
    public static class ComponentExtensions
    {
        /// <summary>
        /// 如果没有指定组件，则抛异常
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        public static T GetRequireComponent<T>(this Component component)
            where T:Component
        {
            var com = component.GetComponent<T>();
            if (com is null) throw new Exception($"{component.name} is missing component {typeof(T).Name}");
            return com;
        }

        /// <summary>
        /// 如果没有指定组件，则抛异常
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        public static T GetRequireComponentInChildren<T>(this Component o)
            where T : Component
        {
            return o.GetComponentInChildren<T>() ??
                   throw new Exception($"{o.name} missing child component {typeof(T).Name}");
        }

        /// <summary>
        /// 判断是否具有指定组件（引用判空，非==null判定）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        public static bool HasComponent<T>(this Component com) where T : Component
        {
            return !(com.GetComponent<T>() is null);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        public static bool IsDestroyed(this Component component)
        {
            return component == null;
        }
    }
}