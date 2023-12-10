using Assets.Helpers;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters.WearColors
{
    internal class GirlSkiDefaultModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "1e3b469038d3dd10d293b21aef2ead36";

        /// <inheritdoc/>
        public List<MaterialColor> MaterialColors => new()
        {
            new MaterialColor(materialIndex: 0, color: ColorHelper.FromHex("#000000")),
            new MaterialColor(materialIndex: 1, color: ColorHelper.FromHex("#675A91"))
        };

        /// <inheritdoc/>
        public int Price => 0;

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
