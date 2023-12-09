﻿using Assets.Helpers;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters.WearColors
{
    internal class YoungManSkiWhiteRedModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "5cbd256cea88da1adf57ddccc669c194";

        /// <inheritdoc/>
        public List<MaterialColor> MaterialColors => new()
        {
            new MaterialColor(materialIndex: 1, color: ColorHelper.FromHex("#C1C1C1")),
            new MaterialColor(materialIndex: 2, color: ColorHelper.FromHex("#890400"))
        };

        /// <inheritdoc/>
        public int Price => 1300;

        public override bool Equals(object obj)
        {
            if (obj is not YoungManJacketBlueModel comparingObj)
            {
                return false;
            }

            return comparingObj.Key == Key;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
        //1: 3AE0D9
        //2: 5A76A4
    }
}
