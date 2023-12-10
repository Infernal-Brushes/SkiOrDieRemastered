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

        /// <summary>
        /// Модель цвета
        /// </summary>
        public IWearColorModel WearColorModel => _wearColorModel.Value;

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

            _shopController.CurrentCharacterSaleController.ColorPart(WearColorModel);

            if (WearColorModel.Price > 0 && !IsColorOwned)
            {
                _shopController.ShowColorBuyButton(this);
            }
            else
            {
                _shopController.HideColorBuyButton();
                SelectColor();
            }
        }

        public void BuyColor()
        {
            if (_userDataController.UserDataModel.BuyColor(WearColorModel))
            {
                if (WearColorModel.Price > 0)
                {
                    _shopController.MoneySpendFadeTextController.Show($"-{WearColorModel.Price}");
                    _shopController.BuyColorParticles.Play();
                    _shopController.UpdateUserMoneyUI();
                }

                UpdateLock();
                HideTooltip();
                _shopController.UpdateCurrentCharacterMenuUI();
            }

            SelectColor();
        }

        private void SelectColor()
        {
            if (_userDataController.UserDataModel.IsColorOwned(WearColorModel.Key))
            {
                _userDataController.UserDataModel.SelectColor(WearColorModel, _shopController.CurrentCharacter);
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
