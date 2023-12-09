using Assets.Scripts.Models.Characters.WearColors;
using Assets.Scripts.Player;
using Assets.Scripts.UI;
using System.Linq;
using TMPro;
using TNRD;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Shop
{
    public class BuyCollorController : TooltipController
    {
        [Tooltip("Текст стоимости в тултипе")]
        [SerializeField]
        private TextMeshProUGUI _priceText;

        [Tooltip("Модель цвета")]
        [SerializeField]
        private SerializableInterface<IWearColorModel> _wearColorModel;

        [Tooltip("Иконки цвета")]
        [SerializeField]
        private Image[] _colorImages;

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
            for(int index = 0; index < _colorImages.Length; index++)
            {
                _colorImages[index].color = _wearColorModel.Value.MaterialColors[index].Color;
            }

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
                wearColor = _shopController.CurrentCharacter.SkiColors
                    .SingleOrDefault(wearColor => wearColor.Key == _wearColorModel.Value.Key);

                if (wearColor is null)
                {
                    Debug.LogWarning("Цвет не добавлен персонажу в список возможных (в его модели ICharacterModel.BodyPartColors)");
                    return;
                }
            }

            if (_userDataController.UserDataModel.BuyColor(wearColor))
            {
                if (wearColor.Price > 0)
                {
                    _shopController.MoneySpendFadeTextController.Show($"-{wearColor.Price}");
                }

                UpdateLock();
                HideTooltip();
                _shopController.UpdateCurrentCharacterMenuUI();
            }

            if (_userDataController.UserDataModel.IsColorOwned(wearColor.Key))
            {
                _userDataController.UserDataModel.SelectColor(wearColor, _shopController.CurrentCharacter);
                _shopController.CurrentCharacterSaleController.ColorPart(wearColor);
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (IsColorOwned || _wearColorModel.Value.Price == 0)
            {
                return;
            }

            _priceText.text = _wearColorModel.Value.Price.ToString();

            base.OnPointerEnter(eventData);
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
