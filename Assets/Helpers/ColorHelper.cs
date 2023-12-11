using System;
using UnityEngine;

namespace Assets.Helpers
{
    public static class ColorHelper
    {
        /// <summary>
        /// Получить цвет из целочисленных цветов красного, зелёного, синего, и альфа-канала
        /// </summary>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <param name="alpha"></param>
        /// <returns>Color</returns>
        public static Color FromRGB(int red, int green, int blue, int alpha = 255) => new(red / 255f, green / 255f, blue / 255f, alpha / 255f);

        public static Color FromHex(string hexColor)
        {
            if (ColorUtility.TryParseHtmlString(hexColor, out Color unityColor))
            {
                return unityColor;
            }
            else
            {
                throw new ArgumentException($"Некорректный HEX цвета: {hexColor}", nameof(hexColor));
            }
        }
    }
}
