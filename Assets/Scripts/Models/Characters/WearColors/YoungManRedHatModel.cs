using Assets.Helpers;
using UnityEngine;

namespace Assets.Scripts.Models.Characters.WearColors
{
    /// <inheritdoc/>
    internal class YoungManRedHatModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "28823d57f6637b2ff975bcfbdb77fb5e";

        /// <inheritdoc/>
        public int MaterialIndex => 6;

        /// <inheritdoc/>
        public Color Color => ColorHelper.FromHex("#8E1816");

        /// <inheritdoc/>
        public int Price => 600;

        public override bool Equals(object obj)
        {
            if (obj is not YoungManBlueJacketModel comparingObj)
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
