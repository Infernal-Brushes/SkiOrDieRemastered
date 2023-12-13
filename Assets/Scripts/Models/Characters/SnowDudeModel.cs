using Assets.Scripts.Models.Characters.WearColors;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters
{
    public class SnowDudeModel : ICharacterModel
    {
        /// <inheritdoc/>
        public string Name => "Снежа";

        /// <inheritdoc/>
        public string Description => "Веселый снеговик, обожает наваливать боком! Рассеян, не всегда легко собраться";

        /// <inheritdoc/>
        public int Index => 3;

        /// <inheritdoc/>
        public string Key => "43e128885f5855035682e1eb313b9373";

        /// <inheritdoc/>
        public int Price => 6500;

        /// <inheritdoc/>
        public List<IWearColorModel> BodyPartColorsDefault => new()
        {
        };

        /// <inheritdoc/>
        public List<IWearColorModel> SkiColorsDefault => new()
        {
        };

        /// <inheritdoc/>
        public List<IWearColorModel> BodyPartColors => new()
        {
        };

        /// <inheritdoc/>
        public List<IWearColorModel> SkiColors => new()
        {
        };
    }
}
