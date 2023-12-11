using Assets.Helpers;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters.WearColors
{
    internal class GirlMaskCyanModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "c225b232305ce62bca83da0f0fc6665e";

        /// <inheritdoc/>
        public List<MaterialColor> MaterialColors => new()
        {
            new MaterialColor(materialIndex: 5, color: ColorHelper.FromHex("#85F1B1")),
        };

        /// <inheritdoc/>
        public int Price => 1600;

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
