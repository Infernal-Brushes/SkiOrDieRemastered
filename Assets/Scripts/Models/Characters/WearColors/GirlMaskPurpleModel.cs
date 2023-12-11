using Assets.Helpers;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters.WearColors
{
    internal class GirlMaskPurpleModel : IWearColorModel
    {
        /// <inheritdoc/>
        public string Key => "c3d1d4699a2f4843ab4c9e45934a521d";

        /// <inheritdoc/>
        public List<MaterialColor> MaterialColors => new()
        {
            new MaterialColor(materialIndex: 5, color: ColorHelper.FromHex("#EC61CE")),
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
