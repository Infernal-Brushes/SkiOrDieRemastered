using Assets.Helpers;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters.WearColors
{
    internal class GirlBodySuitKillBillModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "c51a750f9b1560f724d9be650722277c";

        /// <inheritdoc/>
        public List<MaterialColor> MaterialColors => new()
        {
            new MaterialColor(materialIndex: 0, color: ColorHelper.FromHex("#3A3A3A")),
            new MaterialColor(materialIndex: 3, color: ColorHelper.FromHex("#3A3A3A")),
            new MaterialColor(materialIndex: 4, color: ColorHelper.FromHex("#AD9E20")),
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
