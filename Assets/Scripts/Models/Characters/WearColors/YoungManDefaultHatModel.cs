using Assets.Helpers;
using UnityEngine;

namespace Assets.Scripts.Models.Characters.WearColors
{
    internal class YoungManDefaultHatModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "8165d9eb8e53faf9e5baa100be37c09a";

        /// <inheritdoc/>
        public int MaterialIndex => 6;

        /// <inheritdoc/>
        public Color Color => ColorHelper.FromHex("#58C529");

        /// <inheritdoc/>
        public int Price => 0;

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
