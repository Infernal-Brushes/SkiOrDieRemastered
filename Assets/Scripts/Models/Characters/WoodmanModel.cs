using Assets.Scripts.Models.Characters.WearColors;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters
{
    /// <inheritdoc/>
    public class WoodmanModel : ICharacterModel
    {
        /// <inheritdoc/>
        public string Name => "Хэнк";

        /// <inheritdoc/>
        public string Description => "Харизматичный гуру горных вершин. " +
            "Он может и не молодой, но он точно знает, как покорять горы. " +
            "Как Чак Норрис, только на лыжах";

        /// <inheritdoc/>
        public int Index => 1;

        /// <inheritdoc/>
        public string Key => "4b0c364c6db11ddeced087ac1dfe56d5";

        /// <inheritdoc/>
        public int Price => 3200;

        /// <inheritdoc/>
        public List<IWearColorModel> BodyPartColorsDefault => new()
        {
            new WoodmanJacketDefaultModel(),
            new WoodmanShirtDefaultModel(),
            new WoodmanPantsDefaultModel(),
            new WoodmanPantsDefaultModel(),
        };

        /// <inheritdoc/>
        public List<IWearColorModel> SkiColorsDefault => new()
        {
            new WoodmanSkiDefaultModel(),
        };

        /// <inheritdoc/>
        public List<IWearColorModel> BodyPartColors => new()
        {
            new WoodmanJacketDefaultModel(),
            new WoodmanJacketLightModel(),
            new WoodmanJacketBlackModel(),
            new WoodmanShirtDefaultModel(),
            new WoodmanShirtBlueModel(),
            new WoodmanShirtWhiteMode(),
            new WoodmanPantsDefaultModel(),
            new WoodmanPantsBrownModel(),
        };

        /// <inheritdoc/>
        public List<IWearColorModel> SkiColors => new()
        {
            new WoodmanSkiDefaultModel(),
            new WoodmanSkiBlueModel(),
            new WoodmanSkiBlackGoldModel(),
        };
    }
}
