using Assets.Scripts.Models.Characters.WearColors;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters
{
    /// <inheritdoc/>
    public class GirlModel : ICharacterModel
    {
        /// <inheritdoc/>
        public string Name => "Оливия";

        /// <inheritdoc/>
        public string Description => "Заразительная энергия внутри маленькой обертки. " +
            "В ее горнолыжном костюме затаилась не только грация, но и доля шалости. " +
            "У нее всегда найдется смешная шутка или неожиданный трюк на склоне";

        /// <inheritdoc/>
        public int Index => 2;

        /// <inheritdoc/>
        public string Key => "12bbeda666a1f262272cc598708033bf";

        /// <inheritdoc/>
        public int Price => 4700;

        /// <inheritdoc/>
        public List<IWearColorModel> BodyPartColorsDefault => new()
        {
        };

        /// <inheritdoc/>
        public List<IWearColorModel> SkiColorsDefault => new()
        {
        };

        /// <inheritdoc/>
        public List<IWearColorModel> BodyPartColors => new();

        /// <inheritdoc/>
        public List<IWearColorModel> SkiColors => new()
        {
        };
    }
}
