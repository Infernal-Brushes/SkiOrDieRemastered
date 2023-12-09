using Assets.Scripts.Models.Characters.WearColors;
using Assets.Scripts.Player;
using System.Linq;
using TNRD;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Shop
{
    public class BuyCollorController : MonoBehaviour
    {
        [Tooltip("Модель цвета")]
        [SerializeField]
        private SerializableInterface<IWearColorModel> _wearColorModel;

        [Tooltip("Иконка цвета")]
        [SerializeField]
        private Image _colorImage;

        [Tooltip("Иконка блокировки")]
        [SerializeField]
        private GameObject _lockIcon;

        [Tooltip("Контроллер магазина")]
        [SerializeField]
        private ShopController _shopController;

        private UserDataController _userDataController;

        /// <summary>
        /// Владеет ли игрок этим цветом
        /// </summary>
        private bool IsColorOwned => _userDataController.UserDataModel.IsColorOwned(_wearColorModel.Value.Key);

        private void Start()
        {
            _userDataController = FindObjectOfType<UserDataController>();
            _colorImage.color = _wearColorModel.Value.Color;
            UpdateLock();
        }

        /// <summary>
        /// Приобрести или выбрать цвет
        /// </summary>
        public void ButtonPress()
        {
            if (!_userDataController.UserDataModel.IsCharacterOwned(_shopController.CurrentCharacter))
            {
                return;
            }

            IWearColorModel wearColor = _shopController.CurrentCharacter.BodyPartColors
                .SingleOrDefault(wearColor => wearColor.Key == _wearColorModel.Value.Key);

            if (wearColor is null)
            {
                Debug.LogWarning("Цвет не добавлен персонажу в список возможных (в его модели ICharacterModel.BodyPartColors)");
                return;
            }

            if (_userDataController.UserDataModel.BuyColor(wearColor))
            {
                UpdateLock();
                _shopController.UpdateCurrentCharacterMenuUI();
            }

            if (_userDataController.UserDataModel.IsColorOwned(wearColor.Key))
            {
                _userDataController.UserDataModel.SelectColor(wearColor, _shopController.CurrentCharacter);
                _shopController.CurrentCharacterSaleController.ColorPart(wearColor);
            }
        }

        /// <summary>
        /// Обновить видимость иконки замочка
        /// </summary>
        public void UpdateLock()
        {
            if (_wearColorModel.Value.Price == 0)
            {
                _lockIcon.SetActive(false);
                return;
            }

            _lockIcon.SetActive(!IsColorOwned);
        }
    }
}
