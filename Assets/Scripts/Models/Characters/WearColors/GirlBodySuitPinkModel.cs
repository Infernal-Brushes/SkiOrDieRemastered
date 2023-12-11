using Assets.Helpers;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters.WearColors
{
    internal class GirlBodySuitPinkModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "b1c130be230477c7383aba589ee0325a";

        /// <inheritdoc/>
        public List<MaterialColor> MaterialColors => new()
        {
            new MaterialColor(materialIndex: 0, color: ColorHelper.FromHex("#F8BDFF")),
            new MaterialColor(materialIndex: 3, color: ColorHelper.FromHex("#B4479D")),
            new MaterialColor(materialIndex: 4, color: ColorHelper.FromHex("#CA96BC")),
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
