using Assets.Scripts.Models.Characters;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace Assets.Scripts.Models.Users
{
    /// <summary>
    /// Модель данных игрока
    /// </summary>
    public interface IUserData
    {
        /// <summary>
        /// Деньги игрока
        /// </summary>
        public int Money { get; }

        /// <summary>
        /// Лучший рекорд спуска в метрах
        /// </summary>
        public int BestMetersRecord { get; }

        /// <summary>
        /// Список ключей купленных персонажа
        /// </summary>
        public List<string> CharacterKeys { get; }

        /// <summary>
        /// Ключ текущего выбранного персонажа
        /// </summary>
        public string SelectedCharacterKey { get; }

        /// <summary>
        /// Признак того что персонаж выбран
        /// </summary>
        /// <param name="character">Персонаж для проверки</param>
        /// <returns>
        /// <see cref="true"/> - этот персонаж выбран
        /// <see cref="false"/> - этот персонаж не выбран
        /// </returns>
        public bool IsCharacterSelected(ICharacterModel character);

        /// <summary>
        /// Признак того что персонаж куплен
        /// </summary>
        /// <param name="character">Персонаж для проверки</param>
        /// <returns>
        /// <see cref="true"/> - этот персонаж куплен
        /// <see cref="false"/> - этот персонаж не куплен
        /// </returns>
        public bool IsCharacterOwned(ICharacterModel character);

        /// <summary>
        /// Заработать денег
        /// </summary>
        /// <param name="money">Поступившие деньги</param>
        public void EarnMoney(int money);

        /// <summary>
        /// Преобрести персонажа
        /// </summary>
        /// <param name="character">Персонаж для преобретения</param>
        /// <returns>
        /// <see cref="true"/> - персонаж преобретён.
        /// <see cref="false"/> - персонаж не преобретён
        /// </returns>
        public bool BuyCharacter(ICharacterModel character);

        /// <summary>
        /// Выбрать персонажа для игры
        /// </summary>
        /// <param name="character">Персонаж для выбора</param>
        public bool SelectCharacter(ICharacterModel character);

        /// <summary>
        /// Задать новый лучший рекорд спуска в метрах, если предыдущий был меньше
        /// </summary>
        /// <param name="meters">Количество пройденных метров</param>
        /// <returns>
        /// <see cref="true"/> - новый результат - лучший.
        /// <see cref="false"/> - новый результат не самый лучший
        /// </returns>
        public bool TrySetBestMetersRecord(int meters);

        /// <summary>
        /// Сохранить данные игрока
        /// </summary>
        public void Commit();

        /// <summary>
        /// Загрузить данные игрока
        /// </summary>
        public void Fetch();

        /// <summary>
        /// Сбросить данные игрока
        /// </summary>
        public void Reset();
    }
}
