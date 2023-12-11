using Assets.Scripts.Models.Characters;
using Assets.Scripts.Models.Characters.WearColors;
using System.Collections.Generic;
using System.Linq;
using TNRD;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerAppearance : MonoBehaviour
    {
        [Tooltip("Модель персонажа")]
        [SerializeField]
        private SerializableInterface<ICharacterModel> _characterModel;

        [SerializeField]
        private MeshRenderer _leftSkiMeshRenderer;

        [SerializeField]
        private MeshRenderer _rightSkiMeshRenderer;

        /// <summary>
        /// Модель персонажа
        /// </summary>
        public ICharacterModel CharacterModel => _characterModel.Value;

        private SkinnedMeshRenderer _skinnedMeshRenderer;

        protected UserDataController _userDataController;

        /// <summary>
        /// Метод который нужно выполнить на Awake
        /// </summary>
        protected void OnAwake()
        {
            _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }

        private void Start()
        {
            _userDataController = FindObjectOfType<UserDataController>();
            ResetColors();
        }

        /// <summary>
        /// Окрасить соответствующие части модели в цвета
        /// </summary>
        /// <param name="wearColor">Модель цвета для покраски</param>
        public void ColorPart(IWearColorModel wearColor)
        {
            if (CharacterModel.BodyPartColors.Any(bodyPartColor => bodyPartColor.Key == wearColor.Key))
            {
                ColorBodyPart(wearColor);
                return;
            }

            if (CharacterModel.SkiColors.Any(skiColor => skiColor.Key == wearColor.Key))
            {
                ColorSki(wearColor);
                return;
            }
        }

        /// <summary>
        /// Сбросить цвета на сохранённые в модели данных пользователя
        /// </summary>
        public void ResetColors()
        {
            // Выбранные цвета
            IEnumerable<IWearColorModel> selectedBodyParts = CharacterModel.BodyPartColorsDefault.Concat(CharacterModel.BodyPartColors
                .Where(color => _userDataController.UserDataModel.WearColorKeysSelected.Contains(color.Key)));
            foreach (IWearColorModel partToRecolor in selectedBodyParts)
            {
                ColorBodyPart(partToRecolor);
            }

            // Выбранные цвета
            IEnumerable<IWearColorModel> selectedSkiParts = CharacterModel.SkiColorsDefault.Concat(CharacterModel.SkiColors
                .Where(color => _userDataController.UserDataModel.WearColorKeysSelected.Contains(color.Key)));
            foreach (IWearColorModel partToRecolor in selectedSkiParts)
            {
                ColorSki(partToRecolor);
            }
        }

        /// <summary>
        /// Окрасить тело персонажа в цвета
        /// </summary>
        /// <param name="wearColor">Модель цвета для покраски</param>
        private void ColorBodyPart(IWearColorModel wearColor)
        {
            for (int index = 0; index < wearColor.MaterialColors.Count; index++)
            {
                var materialColor = wearColor.MaterialColors[index];
                _skinnedMeshRenderer.materials[materialColor.MaterialIndex].color = materialColor.Color;
            }
        }

        /// <summary>
        /// Окрасить лыжи персонажа в цвета
        /// </summary>
        /// <param name="wearColor">Модель цвета для покраски</param>
        private void ColorSki(IWearColorModel wearColor)
        {
            for (int index = 0; index < wearColor.MaterialColors.Count; index++)
            {
                var materialColor = wearColor.MaterialColors[index];
                _leftSkiMeshRenderer.materials[materialColor.MaterialIndex].color = materialColor.Color;
                _rightSkiMeshRenderer.materials[materialColor.MaterialIndex].color = materialColor.Color;
            }
        }
    }
}
