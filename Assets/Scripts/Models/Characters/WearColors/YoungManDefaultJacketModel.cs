using Assets.Helpers;
using UnityEngine;

namespace Assets.Scripts.Models.Characters.WearColors
{
    /// <summary>
    /// Базовый цвет куртки пацана
    /// </summary>
    internal class YoungManDefaultJacketModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "c07afa99ac8b2c6455464b03e3c8c251";

        /// <inheritdoc/>
        public int MaterialIndex => 4;

        /// <inheritdoc/>
        public Color Color => ColorHelper.FromHex("#FE6517");

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
