﻿using Assets.Scripts.Models.Characters.WearColors;
using System.Collections.Generic;

namespace Assets.Scripts.Models.Characters
{
    /// <summary>
    /// Модель пацана
    /// </summary>
    internal class YoungManModel : ICharacterModel
    {
        /// <inheritdoc/>
        public string Name => "Ричард";

        /// <inheritdoc/>
        public string Description => "Весельчак, который обожает лыжные курорты. Однако он все же предпочитает сноубординг. Но ему выдали лыжи... Сволочи!";

        /// <inheritdoc/>
        public int Index => 0;

        /// <inheritdoc/>
        public string Key => "fb7df1cf4762c4f98935c2b2e6bb8fb3";

        /// <inheritdoc/>
        public int Price => 0;

        /// <inheritdoc/>
        public List<IWearColorModel> BodyPartColorsDefault => new()
        {
            new YoungManJacketDefaultModel(),
            new YoungManHatDefaultModel(),
        };

        /// <inheritdoc/>
        public List<IWearColorModel> SkiColorsDefault => new()
        {
            new YoungManSkiDefaultModel(),
        };

        /// <inheritdoc/>
        public List<IWearColorModel> BodyPartColors => new() {
            new YoungManJacketDefaultModel(),
            new YoungManJacketBlueModel(),
            new YoungManJacketAcidModel(),

            new YoungManHatDefaultModel(),
            new YoungManHatRedModel(),
            new YoungManHatCyanModel(),
        };

        /// <inheritdoc/>
        public List<IWearColorModel> SkiColors => new()
        {
            new YoungManSkiDefaultModel(),
            new YoungManSkiWhiteRedModel(),
            new YoungManSkiMintModel(),
            new YoungManSkiLimeModel(),
        };
    }
}
