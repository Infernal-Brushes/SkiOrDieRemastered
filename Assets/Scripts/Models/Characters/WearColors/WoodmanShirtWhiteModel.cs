using Assets.Helpers;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters.WearColors
{
    internal class WoodmanShirtWhiteMode : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "1186271d7423c24d3c8de44f1d02083c";

        /// <inheritdoc/>
        public List<MaterialColor> MaterialColors => new()
        {
            new MaterialColor(materialIndex: 6, color: ColorHelper.FromHex("#DDDDDD"))
        };

        /// <inheritdoc/>
        public int Price => 500;

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
