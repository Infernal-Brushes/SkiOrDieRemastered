using Assets.Scripts.Models.Characters;
using Assets.Scripts.Models.Characters.WearColors;
using System.Linq;
using TNRD;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerAppearance : MonoBehaviour
    {
        [Tooltip("Модель персонажа")]
        [field: SerializeField]
        public SerializableInterface<ICharacterModel> CharacterModel { get; private set; }

        [SerializeField]
        private MeshRenderer _leftSkiMeshRenderer;

        [SerializeField]
        private MeshRenderer _rightSkiMeshRenderer;

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

            var bodyPartsToRecolor = CharacterModel.Value.BodyPartColors.Where(color => _userDataController.UserDataModel.WearColorKeysSelected.Contains(color.Key));
            foreach (var partToRecolor in bodyPartsToRecolor)
            {
                ColorBodyPart(partToRecolor);
            }

            var skiPartsToRecolor = CharacterModel.Value.SkiColors.Where(color => _userDataController.UserDataModel.WearColorKeysSelected.Contains(color.Key));
            foreach (var partToRecolor in skiPartsToRecolor)
            {
                ColorSki(partToRecolor);
            }
        }

        public void ColorPart(IWearColorModel wearColor)
        {
            if (CharacterModel.Value.BodyPartColors.Any(bodyPartColor => bodyPartColor.Key == wearColor.Key))
            {
                ColorBodyPart(wearColor);
                return;
            }

            if (CharacterModel.Value.SkiColors.Any(skiColor => skiColor.Key == wearColor.Key))
            {
                ColorSki(wearColor);
                return;
            }
        }

        private void ColorBodyPart(IWearColorModel wearColor)
        {
            for (int index = 0; index < wearColor.MaterialColors.Count; index++)
            {
                var materialColor = wearColor.MaterialColors[index];
                _skinnedMeshRenderer.materials[materialColor.MaterialIndex].color = materialColor.Color;
            }
        }

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
