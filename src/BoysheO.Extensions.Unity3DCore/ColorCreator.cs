using System;
using UnityEngine;

namespace BoysheO.Extensions.Unity3DCore
{
    public static class ColorCreator
    {
        public static Color32 Build(string hex)
        {
            return ColorUtility.TryParseHtmlString(hex, out var color)
                ? color
                : throw new Exception($"invalid hex={hex}");
        }
    }
}