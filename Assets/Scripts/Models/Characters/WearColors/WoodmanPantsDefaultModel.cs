using Assets.Helpers;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters.WearColors
{
    internal class WoodmanPantsDefaultModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "1f9204ec8245db04d6759af6cbf31375";

        /// <inheritdoc/>
        public List<MaterialColor> MaterialColors => new()
        {
            new MaterialColor(materialIndex: 5, color: ColorHelper.FromHex("#64708E"))
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
