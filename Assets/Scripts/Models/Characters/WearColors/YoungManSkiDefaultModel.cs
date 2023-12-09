using Assets.Helpers;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters.WearColors
{
    internal class YoungManSkiDefaultModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "4c086a04587341b6872f749abeebbc81";

        /// <inheritdoc/>
        public List<MaterialColor> MaterialColors => new()
        {
            new MaterialColor(materialIndex: 1, color: ColorHelper.FromHex("#1B2E2E")),
            new MaterialColor(materialIndex: 2, color: ColorHelper.FromHex("#3B6766"))
        };

        /// <inheritdoc/>
        public int Price => 0;

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
    }
}
