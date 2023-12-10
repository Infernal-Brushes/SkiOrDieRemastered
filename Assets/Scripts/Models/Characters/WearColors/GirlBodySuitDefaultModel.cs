using Assets.Helpers;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters.WearColors
{
    internal class GirlBodySuitDefaultModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "3b4f0a11c2d190151c8f4884e1addfb2";

        /// <inheritdoc/>
        public List<MaterialColor> MaterialColors => new()
        {
            new MaterialColor(materialIndex: 0, color: ColorHelper.FromHex("#807CE7")),
            new MaterialColor(materialIndex: 3, color: ColorHelper.FromHex("#E7E7E7")),
            new MaterialColor(materialIndex: 4, color: ColorHelper.FromHex("#88ADFF")),
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
