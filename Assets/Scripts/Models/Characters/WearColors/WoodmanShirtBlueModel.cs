using Assets.Helpers;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters.WearColors
{
    internal class WoodmanShirtBlueModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "cf4246214aff44e728204b7ede69283a";

        /// <inheritdoc/>
        public List<MaterialColor> MaterialColors => new()
        {
            new MaterialColor(materialIndex: 6, color: ColorHelper.FromHex("#160084"))
        };

        /// <inheritdoc/>
        public int Price => 500;

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
