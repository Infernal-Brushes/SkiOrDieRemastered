using Assets.Scripts.Models.Characters;
using Assets.Scripts.Models.Characters.WearColors;
using System;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace Assets.Scripts.Models.Users
{
    /// <summary>
    /// Модель данных игрока
    /// </summary>
    public interface IUserDataModel : ICloneable
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
        /// Делитель метров для подсчёта очков за расстояние
        /// </summary>
        public int MetersScoreDelimeter { get; }

        /// <summary>
        /// Список ключей купленных персонажа
        /// </summary>
        public List<string> CharacterKeys { get; }

        /// <summary>
        /// Ключ текущего выбранного персонажа
        /// </summary>
        public string SelectedCharacterKey { get; }

        /// <summary>
        /// Список ключей купленных цветов
        /// </summary>
        public List<string> WearColorKeysOwned { get; }

        /// <summary>
        /// Список ключей выбранных цветов
        /// </summary>
        public List<string> WearColorKeysSelected { get; }

        /// <summary>
        /// Код локализации
        /// </summary>
        public string LocalizationCode { get; }

        /// <summary>
        /// Задать данные игрока из json
        /// </summary>
        /// <param name="json">Данные игрока в виде JSON</param>
        public void SetDataFromJson(string json);

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
        /// Признак того что цвет куплен
        /// </summary>
        /// <param name="colorKey">Ключ цвета</param>
        /// <returns>
        /// <see cref="true"/> - этот цвет куплен
        /// <see cref="false"/> - этот цвет не куплен
        /// </returns>
        public bool IsColorOwned(string colorKey);

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
        /// Приобрести цвет для части 3д модели персонажа
        /// </summary>
        /// <param name="wearColor">Цвет для приобретения</param>
        /// <returns>
        /// <see cref="true"/> - цвет преобретён.
        /// <see cref="false"/> - цвет не преобретён
        /// </returns>
        public bool BuyColor(IWearColorModel wearColor);

        /// <summary>
        /// Выбрать цвет персонажа
        /// </summary>
        /// <param name="wearColor">Модель цвета</param>
        /// <param name="character">Модель персонажа</param>
        void SelectColor(IWearColorModel wearColor, ICharacterModel character);

        /// <summary>
        /// Задать новый лучший рекорд спуска в метрах, если предыдущий был меньше
        /// </summary>
        /// <param name="meters">Количество пройденных метров</param>
        /// <returns>
        /// <see cref="true"/> - новый результат - лучший.
        /// <see cref="false"/> - новый результат не самый лучший
        /// </returns>
        public void EarnMoneyAndTrySetBestMetersRecord(int meters, int money);

        /// <summary>
        /// Сменить локализацию игры
        /// </summary>
        /// <param name="localizationCode">Код локализации</param>
        public void SetLocalizationCode(string localizationCode);

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
