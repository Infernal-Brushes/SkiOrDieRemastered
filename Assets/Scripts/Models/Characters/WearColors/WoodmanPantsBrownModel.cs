using Assets.Helpers;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters.WearColors
{
    internal class WoodmanPantsBrownModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "d8a46ef8de8f7ee2e09a044805dd56b1";

        /// <inheritdoc/>
        public List<MaterialColor> MaterialColors => new()
        {
            new MaterialColor(materialIndex: 5, color: ColorHelper.FromHex("#63584C"))
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
