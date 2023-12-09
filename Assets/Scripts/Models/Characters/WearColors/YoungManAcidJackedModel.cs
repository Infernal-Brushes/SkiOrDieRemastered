using Assets.Helpers;
using UnityEngine;

namespace Assets.Scripts.Models.Characters.WearColors
{
    /// <inheritdoc/>
    internal class YoungManAcidJackedModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "414d8c46a77dbc268d41fedf2da1f439";

        /// <inheritdoc/>
        public int MaterialIndex => 4;

        /// <inheritdoc/>
        public Color Color => ColorHelper.FromHex("#90DE25");

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
