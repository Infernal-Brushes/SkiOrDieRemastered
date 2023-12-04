using UnityEngine;

namespace Assets.Scripts.Models.Characters.WearColors
{
    /// <summary>
    /// Модель цвета для элемента одежды персонажа
    /// </summary>
    public interface IWearColorModel
    {
        /// <summary>
        /// Ключ цвета элемента одежды
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Индекс материала, для которого цвет
        /// </summary>
        public int MaterialIndex { get; }

        /// <summary>
        /// Цвет элемента одежды
        /// </summary>
        public Color Color { get; }

        /// <summary>
        /// Цена за цвет элемента одежды
        /// </summary>
        public int Price { get; }
    }
}
