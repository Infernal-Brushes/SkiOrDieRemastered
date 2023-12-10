using Assets.Helpers;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters.WearColors
{
    internal class WoodmanShirtDefaultModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "6a4b1b12dac56f9b2921c7b27e0d45d6";

        /// <inheritdoc/>
        public List<MaterialColor> MaterialColors => new()
        {
            new MaterialColor(materialIndex: 6, color: ColorHelper.FromHex("#840000"))
        };

        /// <inheritdoc/>
        public int Price => 0;

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
