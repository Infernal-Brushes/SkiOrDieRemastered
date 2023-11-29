using Assets.Scripts.Models.Characters;
using Assets.Scripts.Models.Users;
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
        private float _rotationSpeed = 10f;

        [Tooltip("Коэфициент скорости для крайних персонажей при конце списка")]
        [SerializeField]
        private float _rotationCoefficientForEdges = 3f;

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

        private IUserData _userData;

        private void Awake()
        {
            _userData = new UserData();
            _userData.Fetch();

            _buyButtonText = _buyButton.GetComponentInChildren<TextMeshProUGUI>();

            InitPoduim();
            UpdateCurrentCharacterMenu();
        }

        private void Update()
        {
            if (_isRotating)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                ClearCharacterMenu();
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
                ClearCharacterMenu();
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

        public void BuySelectedCharacter()
        {
            ICharacterModel characterModel = _charactersToSale[_currentCharacterIndex].CharacterModel.Value;
            _userData.BuyCharacter(characterModel);
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
        }

        /// <summary>
        /// Очистить UI меню персонажа
        /// </summary>
        private void ClearCharacterMenu()
        {
            _nameText.enabled = false;
            _descriptionText.enabled = false;
            _buyButton.gameObject.SetActive(false);
            _selectButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// Обновить UI меню персонажа информацией о текущем персонаже
        /// </summary>
        private void UpdateCurrentCharacterMenu()
        {
            var currentCharacter = _charactersToSale[_currentCharacterIndex].CharacterModel.Value;

            _nameText.text = currentCharacter.Name;
            _nameText.enabled = true;

            _descriptionText.text = currentCharacter.Description;
            _descriptionText.enabled = true;

            if (!_userData.IsCharacterOwned(currentCharacter))
            {
                _buyButtonText.text = currentCharacter.Price.ToString();
                _buyButton.interactable = _userData.Money >= currentCharacter.Price;
                _buyButton.gameObject.SetActive(true);
                _selectButton.gameObject.SetActive(false);
            }
            else
            {
                bool isCurrentCharacterSelected = _userData.IsCharacterSelected(currentCharacter);
                _selectButton.interactable = !isCurrentCharacterSelected;
                _selectButton.gameObject.SetActive(true);
            }
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
            UpdateCurrentCharacterMenu();
        }
    }
}
