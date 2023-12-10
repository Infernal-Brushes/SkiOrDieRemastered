using Assets.Helpers;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters.WearColors
{
    internal class WoodmanJacketBlackModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "e605d943406a6af600cfaf72215ac5e9";

        /// <inheritdoc/>
        public List<MaterialColor> MaterialColors => new()
        {
            new MaterialColor(materialIndex: 4, color: ColorHelper.FromHex("#1D1D1D"))
        };

        /// <inheritdoc/>
        public int Price => 800;

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
