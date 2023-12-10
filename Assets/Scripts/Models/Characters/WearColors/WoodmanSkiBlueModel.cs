using Assets.Helpers;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters.WearColors
{
    internal class WoodmanSkiBlueModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "c9eae95c2496ac8fbd27d4c24e2b9a99";

        /// <inheritdoc/>
        public List<MaterialColor> MaterialColors => new()
        {
            new MaterialColor(materialIndex: 0, color: ColorHelper.FromHex("#9FEEFB")),
            new MaterialColor(materialIndex: 2, color: ColorHelper.FromHex("#434D6C"))
        };

        /// <inheritdoc/>
        public int Price => 1700;

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
