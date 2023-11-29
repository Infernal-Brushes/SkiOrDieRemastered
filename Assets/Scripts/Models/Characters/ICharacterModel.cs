﻿namespace Assets.Scripts.Models.Characters
{
    /// <summary>
    /// Модель персонажа
    /// </summary>
    public interface ICharacterModel
    {
        /// <summary>
        /// Имя персонажа
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Описание персонажа
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Индекс персонажа
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Уникальный ключ персонажа
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Стоимость персонажа
        /// </summary>
        public int Price { get; }
    }
}
