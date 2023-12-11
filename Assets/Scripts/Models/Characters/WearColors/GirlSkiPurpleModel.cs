using Assets.Helpers;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters.WearColors
{
    internal class GirlSkiPurpleModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "bba0d32f12293809826c4696ab4bd8b2";

        /// <inheritdoc/>
        public List<MaterialColor> MaterialColors => new()
        {
            new MaterialColor(materialIndex: 0, color: ColorHelper.FromHex("#241533")),
            new MaterialColor(materialIndex: 1, color: ColorHelper.FromHex("#841D58"))
        };

        /// <inheritdoc/>
        public int Price => 2000;

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
