using Assets.Helpers;
using UnityEngine;

namespace Assets.Scripts.Models.Characters.WearColors
{
    /// <inheritdoc/>
    internal class YoungManBlueJacketModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "285b74257287e725376d2785ad2411ec";

        /// <inheritdoc/>
        public int MaterialIndex => 4;

        /// <inheritdoc/>
        public Color Color => ColorHelper.FromRGB(24, 131, 254);

        /// <inheritdoc/>
        public int Price => 200;

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
