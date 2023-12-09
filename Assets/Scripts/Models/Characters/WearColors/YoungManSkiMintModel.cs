using Assets.Helpers;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters.WearColors
{
    internal class YoungManSkiMintModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "8d23eab7b2b0cbe21efe3c4f3741d6f8";

        /// <inheritdoc/>
        public List<MaterialColor> MaterialColors => new()
        {
            new MaterialColor(materialIndex: 1, color: ColorHelper.FromHex("#3AE0D9")),
            new MaterialColor(materialIndex: 2, color: ColorHelper.FromHex("#5A76A4"))
        };

        /// <inheritdoc/>
        public int Price => 1300;

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
        //1: 59CA70
        //2: A9E04F
    }
}
