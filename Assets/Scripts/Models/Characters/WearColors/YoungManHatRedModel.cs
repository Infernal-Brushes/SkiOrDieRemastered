using Assets.Helpers;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters.WearColors
{
    /// <inheritdoc/>
    internal class YoungManHatRedModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "28823d57f6637b2ff975bcfbdb77fb5e";

        /// <inheritdoc/>
        public List<MaterialColor> MaterialColors => new()
        {
            new MaterialColor(materialIndex: 6, color: ColorHelper.FromHex("#8E1816"))
        };

        /// <inheritdoc/>
        public int Price => 600;

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
