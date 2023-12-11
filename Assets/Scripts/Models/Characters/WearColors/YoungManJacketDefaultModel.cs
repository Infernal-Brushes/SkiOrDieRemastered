using Assets.Helpers;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters.WearColors
{
    /// <summary>
    /// Базовый цвет куртки пацана
    /// </summary>
    internal class YoungManJacketDefaultModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "c07afa99ac8b2c6455464b03e3c8c251";

        /// <inheritdoc/>
        public List<MaterialColor> MaterialColors => new()
        {
            new MaterialColor(materialIndex: 4, color: ColorHelper.FromHex("#FE6517"))
        };

        /// <inheritdoc/>
        public int Price => 0;

        public override bool Equals(object obj)
        {
            if (obj is not IWearColorModel comparingObj)
            {
                return false;
            }

            return comparingObj.Key == Key;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }
}
