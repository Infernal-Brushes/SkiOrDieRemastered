using Assets.Helpers;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters.WearColors
{
    internal class WoodmanSkiBlackGoldModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "0a150c1d97474e4f370f6264f2c5f312";

        /// <inheritdoc/>
        public List<MaterialColor> MaterialColors => new()
        {
            new MaterialColor(materialIndex: 0, color: ColorHelper.FromHex("#C0BA53")),
            new MaterialColor(materialIndex: 2, color: ColorHelper.FromHex("#141414"))
        };

        /// <inheritdoc/>
        public int Price => 1700;

        public override bool Equals(object obj)
        {
            if (obj is not YoungManJacketBlueModel comparingObj)
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
