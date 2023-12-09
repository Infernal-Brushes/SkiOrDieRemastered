using Assets.Helpers;
using UnityEngine;

namespace Assets.Scripts.Models.Characters.WearColors
{
    /// <inheritdoc/>
    internal class YoungManCyanHatModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "b72615785c06803207d7f3ed4dcf2660";

        /// <inheritdoc/>
        public int MaterialIndex => 6;

        /// <inheritdoc/>
        public Color Color => ColorHelper.FromHex("#3FB79F");

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
