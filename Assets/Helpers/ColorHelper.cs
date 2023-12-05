using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// <returns></returns>
        public static Color FromRGB(int red, int green, int blue, int alpha = 255) => new(red / 255f, green / 255f, blue / 255f, alpha / 255f);
    }
}
