using Assets.Helpers;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters.WearColors
{
    internal class GirlHairBlueModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "66120d86c88312352a8ece9c3844e4ae";

        /// <inheritdoc/>
        public List<MaterialColor> MaterialColors => new()
        {
            new MaterialColor(materialIndex: 2, color: ColorHelper.FromHex("#5D7DC8")),
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
