using Assets.Scripts.Menu;
using Assets.Scripts.Models.Characters;
using Assets.Scripts.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Shop
{
    /// <summary>
    /// Контроллер магазина
    /// </summary>
    public class ShopController : MonoBehaviour
    {
        [SerializeField]
        private ScenesManager _scenesManager;

        [Header("Настройки подиума")]

        [Tooltip("Главный подиум")]
        [SerializeField]
        private GameObject _mainPodium;

        [Tooltip("Префаб подиума для персонажа")]
        [SerializeField]
        private GameObject _charactersPodium;

        [Tooltip("Подиумы с персами")]
        [SerializeField]
        private GameObject[] _charactersOnPodiums;

        [Tooltip("Градус от круга, по которому будут размещены персонажи")]
        [Range(0f, 360f)]
        [SerializeField]
        private float _angleOfPlacing = 360f;

        [Tooltip("Отступ подиумов персонажа от края главного подиума")]
        [SerializeField]
        private float _offsetPositionFromEdge = 0.2f;

        [Tooltip("Скорость поворота подиума")]
        [SerializeField]
        private float _rotationSpeed = 1.3f;

        [Tooltip("Коэфициент скорости для крайних персонажей при конце списка")]
        [SerializeField]
        private float _rotationCoefficientForEdges = 1.8f;

        [Tooltip("Скорость поворота подиума для первого запуска")]
        [SerializeField]
        private float _firstSelectionRotationSpeed = 2.8f;

        [Tooltip("Партиклы которые активируются при покупке персонажа")]
        [SerializeField]
        private ParticleSystem _buyCharacterParticles;

        [Tooltip("Партиклы которые активируются при покупке цвета")]
        [field: SerializeField]
        public ParticleSystem BuyColorParticles { get; private set; }

        [Header("UI")]

        [Tooltip("Текст имени персонажа")]
        [SerializeField]
        private TextMeshProUGUI _nameText;

        [Tooltip("Текст описания персонажа")]
        [SerializeField]
        private TextMeshProUGUI _descriptionText;

        [Tooltip("Контроллеры позиций цветов одежды персожаней")]
        [SerializeField]
        private CharacterWearColorsController[] _charactersWearColors;

        [Tooltip("Кнопка покупки персонажа")]
        [SerializeField]
        private UnityEngine.UI.Button _buyCharacterButton;

        [Tooltip("Кнопка покупки цвета")]
        [SerializeField]
        private UnityEngine.UI.Button _buyColorButton;

        [Tooltip("Кнопка выбора персонажа")]
        [SerializeField]
        private UnityEngine.UI.Button _playButton;

        [Tooltip("Текст количества денег игрока")]
        [SerializeField]
        private TextMeshProUGUI _moneyText;

        [Tooltip("Фэйд текст траты денег")]
        [field: SerializeField]
        public FadeTextController MoneySpendFadeTextController { get; private set; }

        [Tooltip("Сколько времени длится анимация траты денег")]
        [SerializeField ]
        private float _moneySpendAnimationTime = 1f;

        [Tooltip("Значение зума при покупки игрока")]
        [SerializeField]
        private float _characterBuyZoomOffset = -12f;

        [Tooltip("Время зума при покупки игрока")]
        [SerializeField]
        private float _characterBuyZoomTime = 0.8f;

        /// <summary>
        /// Текст кнопки покупки персонажа
        /// </summary>
        private TextMeshProUGUI _buyCharacterButtonText;

        /// <summary>
        /// Текст кнопки покупки цвета
        /// </summary>
        private TextMeshProUGUI _buyColorButtonText;

        /// <summary>
        /// Угол между соседними персонажами
        /// </summary>
        private float _angle;

        /// <summary>
        /// Признак вращения платформы.
        /// <see cref="true"/> - платформа вращается.
        /// <see cref="false"/> - платформа стоит на месте
        /// </summary>
        private bool _isRotating;

        /// <summary>
        /// Градус незанятой области платформы (внешний угол между крайними персонажами)
        /// </summary>
        private float _notPlacedAngle => 360 - _angleOfPlacing + _angle;

        private List<CharacterSaleController> _charactersToSale = new();

        /// <summary>
        /// Текущий индекс персонажа
        /// </summary>
        private int _currentCharacterIndex = 0;

        private UserDataController _userDataController;

        /// <summary>
        /// Текущий выбрранный персонаж
        /// </summary>
        public ICharacterModel CurrentCharacter => _charactersToSale[_currentCharacterIndex].CharacterModel;

        /// <summary>
        /// Контроллер покупки цвета в предпросмотре для покупки
        /// </summary>
        private BuyCollorController _previewBuyColorController;

        /// <summary>
        /// Контроллер представления персонажа для продажи
        /// </summary>
        public CharacterSaleController CurrentCharacterSaleController => _charactersToSale[_currentCharacterIndex];

        private void Start()
        {
            _userDataController = FindObjectOfType<UserDataController>();

            _buyCharacterButtonText = _buyCharacterButton.GetComponentInChildren<TextMeshProUGUI>();
            _buyColorButtonText = _buyColorButton.GetComponentInChildren<TextMeshProUGUI>();

            InitPoduim();
            UpdateUI(isMoneyAnimated: false);
        }

        private void Update()
        {
            if (_isRotating)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                ClearCharacterMenuUI();
                ResetPreviewColors();

                if (_currentCharacterIndex > 0)
                {
                    _currentCharacterIndex--;
                    StartCoroutine(RotatePodium(-1));

                    return;
                }

                _currentCharacterIndex = _charactersOnPodiums.Length - 1;
                StartCoroutine(RotatePodium(-1, _notPlacedAngle, _rotationSpeed * _rotationCoefficientForEdges));
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                ClearCharacterMenuUI();
                ResetPreviewColors();

                if (_currentCharacterIndex < _charactersOnPodiums.Length - 1)
                {
                    _currentCharacterIndex++;
                    StartCoroutine(RotatePodium(1));

                    return;
                }

                _currentCharacterIndex = 0;
                StartCoroutine(RotatePodium(1, _notPlacedAngle, _rotationSpeed * _rotationCoefficientForEdges));
            }
        }

        /// <summary>
        /// Очистить превьюшные цвета
        /// </summary>
        private void ResetPreviewColors()
        {
            if (_previewBuyColorController != null)
            {
                CurrentCharacterSaleController.ResetColors();
            }
        }

        /// <summary>
        /// Купить текущего персонажа
        /// </summary>
        public void BuyCurrentCharacter()
        {
            bool wasOwned = _userDataController.UserDataModel.BuyCharacter(CurrentCharacter);
            if (wasOwned)
            {
                StartCoroutine(ZoomCamera(_characterBuyZoomOffset, _characterBuyZoomTime));
                _buyCharacterParticles.Play();

                MoneySpendFadeTextController.Show($"-{CurrentCharacter.Price}");

                _userDataController.UserDataModel.SelectCharacter(CurrentCharacter);

                UpdatePlayButtonUI();
                UpdateUserMoneyUI();
            }
        }

        /// <summary>
        /// Выбрать текущего персонажа
        /// </summary>
        public void SelectCurrentCharacter()
        {
            bool wasSelected = _userDataController.UserDataModel.SelectCharacter(CurrentCharacter);
            if (wasSelected)
            {
                _userDataController.UserDataModel.SelectCharacter(CurrentCharacter);
                UpdatePlayButtonUI();
            }
        }

        /// <summary>
        /// Отобразить кнопку покупки выбранного цвета
        /// </summary>
        /// <param name="buyCollorController">Выбранный цвет</param>
        public void ShowColorBuyButton(BuyCollorController buyCollorController)
        {
            _previewBuyColorController = buyCollorController;

            _buyColorButtonText.text = buyCollorController.WearColorModel.Price.ToString();
            
            _buyColorButton.gameObject.SetActive(true);
        }

        /// <summary>
        /// Скрыть кнопку покупки цвета
        /// </summary>
        public void HideColorBuyButton()
        {
            _previewBuyColorController = null;
            _buyColorButton.gameObject.SetActive(false);        }

        /// <summary>
        /// Приобрести выбранный цвет
        /// </summary>
        public void BuySelectedColor()
        {
            _previewBuyColorController.BuyColor();
            HideColorBuyButton();
        }

        /// <summary>
        /// Запустить заезд с текущим выбранным персонажем
        /// </summary>
        public void PlayGame()
        {
            SelectCurrentCharacter();
            _scenesManager.StartFreerideScene();
        }

        /// <summary>
        /// Настроить подиум с персонажами
        /// </summary>
        private void InitPoduim()
        {
            _angle = _angleOfPlacing / _charactersOnPodiums.Length;
            float radius = _mainPodium.transform.localScale.x / 2
                - _charactersPodium.transform.localScale.x / 2
                - _offsetPositionFromEdge;

            // Достать скрипты персонажей из GO персонажей на подиумах, отсортировать так чтобы первый был с индексом 0
            _charactersOnPodiums = _charactersOnPodiums
                .OrderByDescending(characterGO =>
                {
                    var character = characterGO.GetComponentInChildren<CharacterSaleController>();
                    _charactersToSale.Add(character);

                    return character.CharacterModel.Index;
                })
                .ToArray();

            for (int i = 0; i < _charactersOnPodiums.Length; i++)
            {
                float x = Mathf.Sin(Mathf.Deg2Rad * (_angle * i)) * radius;
                float z = Mathf.Cos(Mathf.Deg2Rad * (_angle * i)) * radius;
                float y = _mainPodium.transform.localScale.y + _charactersPodium.transform.localScale.y;

                _charactersOnPodiums[i].transform.SetPositionAndRotation(new Vector3(x, y, z), Quaternion.identity);
            }

            if (_userDataController.UserDataModel.SelectedCharacterKey != CurrentCharacter.Key)
            {
                ClearCharacterMenuUI();

                _currentCharacterIndex = _charactersToSale
                    .Select(characterGO => characterGO.CharacterModel)
                    .Where(character => character.Key == _userDataController.UserDataModel.SelectedCharacterKey)
                    .Select(character => character.Index)
                    .Single();

                int sectionsToRotate = _currentCharacterIndex;
                float angle = _angle * sectionsToRotate;
                StartCoroutine(RotatePodium(1,  angle, _firstSelectionRotationSpeed));
            }
        }

        /// <summary>
        /// Очистить UI меню персонажа
        /// </summary>
        private void ClearCharacterMenuUI()
        {
            _nameText.enabled = false;
            _descriptionText.enabled = false;
            _buyCharacterButton.gameObject.SetActive(false);

            CharacterWearColorsController characterWearColorsController = _charactersWearColors
                .FirstOrDefault(controller => controller.CharacterKey == CurrentCharacter.Key);
            if (characterWearColorsController != null)
            {
                characterWearColorsController.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Обновить UI сцены
        /// </summary>
        /// <param name="isMoneyAnimated"><see cref="true"/> - анимировать изменение денег</param>
        public void UpdateUI(bool isMoneyAnimated = true)
        {
            UpdateUserMoneyUI(isMoneyAnimated);
            UpdateCurrentCharacterMenuUI();
        }

        /// <summary>
        ///  Обновить информацию о деньгах пользователя
        /// </summary>
        /// <param name="isAnimated"><see cref="true"/> - анимировать изменение денег</param>
        public void UpdateUserMoneyUI(bool isAnimated = true)
        {
            if (isAnimated)
            {
                StartCoroutine(AnimateMoneySpending());
                return;
            }

            _moneyText.text = _userDataController.UserDataModel.Money.ToString();
        }

        /// <summary>
        /// Обновить UI меню персонажа информацией о текущем персонаже
        /// </summary>
        public void UpdateCurrentCharacterMenuUI()
        {
            _nameText.text = CurrentCharacter.Name;
            _nameText.enabled = true;

            _descriptionText.text = CurrentCharacter.Description;
            _descriptionText.enabled = true;

            CharacterWearColorsController characterWearColorsController = _charactersWearColors
                .FirstOrDefault(controller => controller.CharacterKey == CurrentCharacter.Key);
            if (characterWearColorsController != null)
            {
                characterWearColorsController.gameObject.SetActive(true);
            }

            UpdateBuyButtonUI();
            _buyColorButton.gameObject.SetActive(false);
            UpdatePlayButtonUI();
        }

        /// <summary>
        /// Отобразить кнопку покупки персонажа
        /// </summary>
        private void UpdateBuyButtonUI()
        {
            _buyCharacterButtonText.text = CurrentCharacter.Price.ToString();
            _buyCharacterButton.interactable = _userDataController.UserDataModel.Money >= CurrentCharacter.Price;
            _buyCharacterButton.gameObject.SetActive(true);
        }

        /// <summary>
        /// Отобразить кнопку выбора персонажа
        /// </summary>
        private void UpdatePlayButtonUI()
        {
            bool isCharacterOwned = _userDataController.UserDataModel.IsCharacterOwned(CurrentCharacter);
            _playButton.interactable = isCharacterOwned;
            _playButton.gameObject.SetActive(true);
            _buyCharacterButton.gameObject.SetActive(!isCharacterOwned);
        }

        /// <summary>
        /// Повернуть платформу на один сегмент
        /// </summary>
        /// <param name="direction">Направление поворота</param>
        private IEnumerator RotatePodium(int direction)
        {
            yield return RotatePodium(direction, _angle, _rotationSpeed);
        }

        /// <summary>
        /// Повернуть платформу на указанный угол
        /// </summary>
        /// <param name="direction">Направление</param>
        /// <param name="angle">Угол поворота</param>
        /// <param name="speed">Скорость поворота</param>
        /// <returns></returns>
        private IEnumerator RotatePodium(int direction, float angle, float speed)
        {
            _isRotating = true;

            float totalRotation = 0f;
            while (totalRotation < angle)
            {
                float rotationAmount = Mathf.Min(speed, angle - totalRotation);
                _mainPodium.transform.RotateAround(_mainPodium.transform.position, _mainPodium.transform.up, direction * rotationAmount);
                totalRotation += rotationAmount;

                yield return null;
            }

            _isRotating = false;
            UpdateCurrentCharacterMenuUI();
        }

        private IEnumerator AnimateMoneySpending()
        {
            float target = _userDataController.UserDataModel.Money;
            float current = Convert.ToSingle(_moneyText.text);

            float elapsedTime = 0f;
            while (elapsedTime < _moneySpendAnimationTime)
            {
                elapsedTime += Time.deltaTime;
                float time = Mathf.Clamp01(elapsedTime / _moneySpendAnimationTime);
                int newNumber = Mathf.RoundToInt(Mathf.Lerp(current, target, time));
                _moneyText.text = newNumber.ToString();
                yield return null;
            }

            _moneyText.text = target.ToString();
        }

        private IEnumerator ZoomCamera(float sizeOffset, float zoomTime)
        {
            float initialSize = Camera.main.fieldOfView;
            float targetSize = Camera.main.fieldOfView + sizeOffset;
            float elapsedTime = 0f;

            while (elapsedTime < zoomTime)
            {
                Camera.main.fieldOfView = Mathf.Lerp(initialSize, targetSize, elapsedTime / zoomTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Camera.main.fieldOfView = targetSize;
            elapsedTime = 0f;

            while (elapsedTime < zoomTime)
            {
                Camera.main.fieldOfView = Mathf.Lerp(targetSize, initialSize, elapsedTime / zoomTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            Camera.main.fieldOfView = initialSize;
        }
    }
}
