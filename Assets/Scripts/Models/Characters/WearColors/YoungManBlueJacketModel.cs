﻿using Assets.Helpers;
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
        public Color Color => ColorHelper.FromHex("#1883FE");

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
