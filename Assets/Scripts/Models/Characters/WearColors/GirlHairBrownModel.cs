using Assets.Helpers;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters.WearColors
{
    internal class GirlHairBrownModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "1e977d16541002ee82f9ded10cf1d586";

        /// <inheritdoc/>
        public List<MaterialColor> MaterialColors => new()
        {
            new MaterialColor(materialIndex: 2, color: ColorHelper.FromHex("#594938")),
        };

        /// <inheritdoc/>
        public int Price => 2400;

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
