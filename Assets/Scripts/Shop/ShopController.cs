using Assets.Scripts.Models.Characters;
using Assets.Scripts.Models.Users;
using Assets.Scripts.Player;
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

        [Header("UI")]

        [Tooltip("Текст имени персонажа")]
        [SerializeField]
        private TextMeshProUGUI _nameText;

        [Tooltip("Текст описания персонажа")]
        [SerializeField]
        private TextMeshProUGUI _descriptionText;

        [Tooltip("Кнопка покупки персонажа")]
        [SerializeField]
        private UnityEngine.UI.Button _buyButton;

        private TextMeshProUGUI _buyButtonText;

        [Tooltip("Кнопка выбора персонажа")]
        [SerializeField]
        private UnityEngine.UI.Button _selectButton;

        [Tooltip("Текст количества денег игрока")]
        [SerializeField]
        private TextMeshProUGUI _moneyText;

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

        private ICharacterModel _currentCharacter => _charactersToSale[_currentCharacterIndex].CharacterModel.Value;

        private void Awake()
        {
            _userDataController = FindObjectOfType<UserDataController>();

            _buyButtonText = _buyButton.GetComponentInChildren<TextMeshProUGUI>();

            InitPoduim();
            UpdateUI();
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
        /// Купить текущего персонажа
        /// </summary>
        public void BuyCurrentCharacter()
        {
            bool wasOwned = _userDataController.UserDataModel.BuyCharacter(_currentCharacter);
            if (wasOwned)
            {
                UpdateSelectButtonUI();
                UpdateUserDataUI();
            }
        }

        /// <summary>
        /// Выбрать текущего персонажа
        /// </summary>
        public void SelectCurrentCharacter()
        {
            bool wasSelected = _userDataController.UserDataModel.SelectCharacter(_currentCharacter);
            if (wasSelected)
            {
                _userDataController.UserDataModel.SelectCharacter(_currentCharacter);
                UpdateSelectButtonUI();
            }
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

                    return character.CharacterModel.Value.Index;
                })
                .ToArray();

            for (int i = 0; i < _charactersOnPodiums.Length; i++)
            {
                float x = Mathf.Sin(Mathf.Deg2Rad * (_angle * i)) * radius;
                float z = Mathf.Cos(Mathf.Deg2Rad * (_angle * i)) * radius;
                float y = _mainPodium.transform.localScale.y + _charactersPodium.transform.localScale.y;

                _charactersOnPodiums[i].transform.SetPositionAndRotation(new Vector3(x, y, z), Quaternion.identity);
            }

            if (_userDataController.UserDataModel.SelectedCharacterKey != _currentCharacter.Key)
            {
                ClearCharacterMenuUI();

                _currentCharacterIndex = _charactersToSale
                    .Select(characterGO => characterGO.CharacterModel.Value)
                    .Where(character => character.Key == _userDataController.UserDataModel.SelectedCharacterKey)
                    .Select(character => character.Index)
                    .Single();

                int sectionsToRotate = _currentCharacterIndex;
                Debug.Log(sectionsToRotate);
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
            _buyButton.gameObject.SetActive(false);
            _selectButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// Обновить UI сцены
        /// </summary>
        private void UpdateUI()
        {
            UpdateUserDataUI();
            UpdateCurrentCharacterMenuUI();
        }

        private void UpdateUserDataUI()
        {
            // TODO: вместо текста подписи сделать иконку
            _moneyText.text = $"Денег: {_userDataController.UserDataModel.Money}";
        }

        /// <summary>
        /// Обновить UI меню персонажа информацией о текущем персонаже
        /// </summary>
        private void UpdateCurrentCharacterMenuUI()
        {
            _nameText.text = _currentCharacter.Name;
            _nameText.enabled = true;

            _descriptionText.text = _currentCharacter.Description;
            _descriptionText.enabled = true;

            if (!_userDataController.UserDataModel.IsCharacterOwned(_currentCharacter))
            {
                UpdateBuyButtonUI();
            }
            else
            {
                UpdateSelectButtonUI();
            }
        }

        /// <summary>
        /// Отобразить кнопку покупки персонажа
        /// </summary>
        private void UpdateBuyButtonUI()
        {
            _buyButtonText.text = _currentCharacter.Price.ToString();
            _buyButton.interactable = _userDataController.UserDataModel.Money >= _currentCharacter.Price;
            _buyButton.gameObject.SetActive(true);
            _selectButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// Отобразить кнопку выбора персонажа
        /// </summary>
        private void UpdateSelectButtonUI()
        {
            _selectButton.interactable = !_userDataController.UserDataModel.IsCharacterSelected(_currentCharacter);
            _selectButton.gameObject.SetActive(true);
            _buyButton.gameObject.SetActive(false);
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
    }
}
