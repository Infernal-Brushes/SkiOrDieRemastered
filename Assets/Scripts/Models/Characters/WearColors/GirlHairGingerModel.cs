using Assets.Helpers;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters.WearColors
{
    internal class GirlHairGingerModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "35f3c83720b12353b1e72828c3e8654b";

        /// <inheritdoc/>
        public List<MaterialColor> MaterialColors => new()
        {
            new MaterialColor(materialIndex: 2, color: ColorHelper.FromHex("#C36443")),
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
