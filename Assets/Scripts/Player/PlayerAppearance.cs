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
            var partsToRecolor = CharacterModel.Value.BodyPartColors.Where(color => _userDataController.UserDataModel.WearColorKeysSelected.Contains(color.Key));
            foreach (var partToRecolor in partsToRecolor)
            {
                ColorPart(partToRecolor);
            }
        }

        public void ColorPart(IWearColorModel wearColor)
        {
            _skinnedMeshRenderer.materials[wearColor.MaterialIndex].color = wearColor.Color;
        }
    }
}
