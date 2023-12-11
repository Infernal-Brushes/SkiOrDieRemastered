using Assets.Helpers;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters.WearColors
{
    internal class GirlSkiIcyModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "601a02a1310a28bad9cc8d9f78ef8b76";

        /// <inheritdoc/>
        public List<MaterialColor> MaterialColors => new()
        {
            new MaterialColor(materialIndex: 0, color: ColorHelper.FromHex("#1F2D4B")),
            new MaterialColor(materialIndex: 1, color: ColorHelper.FromHex("#3DB7A0"))
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
