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
        public int Price => 1200;
    }
}
