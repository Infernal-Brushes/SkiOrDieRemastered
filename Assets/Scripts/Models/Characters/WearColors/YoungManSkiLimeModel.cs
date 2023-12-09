using Assets.Helpers;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters.WearColors
{
    internal class YoungManSkiLimeModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "3cba338b308b6cc7100252cef977d5f3";

        /// <inheritdoc/>
        public List<MaterialColor> MaterialColors => new()
        {
            new MaterialColor(materialIndex: 1, color: ColorHelper.FromHex("#59CA70")),
            new MaterialColor(materialIndex: 2, color: ColorHelper.FromHex("#A9E04F"))
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
    }
}
