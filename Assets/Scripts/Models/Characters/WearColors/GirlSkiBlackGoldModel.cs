using Assets.Helpers;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters.WearColors
{
    internal class GirlSkiBlackGoldModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "7a88f74ec630eba19ec62e4050b2dc88";

        /// <inheritdoc/>
        public List<MaterialColor> MaterialColors => new()
        {
            new MaterialColor(materialIndex: 0, color: ColorHelper.FromHex("#464317")),
            new MaterialColor(materialIndex: 1, color: ColorHelper.FromHex("#0A0A0A"))
        };

        /// <inheritdoc/>
        public int Price => 2000;

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
