namespace Assets.Scripts.Models.Characters
{
    /// <summary>
    /// Модель персонажа на продажу
    /// </summary>
    public interface ICharacterModel
    {
        /// <summary>
        /// Индекс персонажа
        /// </summary>
        public int CharacterIndex { get; }

        /// <summary>
        /// Ключ персонажа
        /// </summary>
        public string CharacterKey { get; }

        /// <summary>
        /// Стоимость персонажа
        /// </summary>
        public int CharacterPrice { get; }
    }
}
