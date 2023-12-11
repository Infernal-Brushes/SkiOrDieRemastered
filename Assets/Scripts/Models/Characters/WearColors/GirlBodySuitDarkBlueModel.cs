using Assets.Helpers;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters.WearColors
{
    internal class GirlBodySuitDarkBlueModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "9841137169772e0a3efcc9e1fef583d7";

        /// <inheritdoc/>
        public List<MaterialColor> MaterialColors => new()
        {
            new MaterialColor(materialIndex: 0, color: ColorHelper.FromHex("#BBC5CD")),
            new MaterialColor(materialIndex: 3, color: ColorHelper.FromHex("#3BD4D1")),
            new MaterialColor(materialIndex: 4, color: ColorHelper.FromHex("#43689F")),
        };

        /// <inheritdoc/>
        public int Price => 1100;

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
