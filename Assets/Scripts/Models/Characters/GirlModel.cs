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
            "Ее дом - это горнолыжные трассы. " +
            "У нее всегда найдется смешная шутка или неожиданный трюк на склоне";

        /// <inheritdoc/>
        public int Index => 2;

        /// <inheritdoc/>
        public string Key => "12bbeda666a1f262272cc598708033bf";

        /// <inheritdoc/>
        public int Price => 20;// 2800;
    }
}
