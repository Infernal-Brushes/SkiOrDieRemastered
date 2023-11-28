using Assets.Scripts.Models.Characters;
using Assets.Scripts.Models.Users;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.Shop
{
    /// <summary>
    /// Контроллер магазина
    /// </summary>
    public class ShopController : MonoBehaviour
    {
        [Tooltip("Главный подиум")]
        [SerializeField]
        private GameObject _mainPodium;

        [Tooltip("Префаб подиума для персонажа")]
        [SerializeField]
        private GameObject _charactersPodium;

        [Tooltip("Подиумы с персами")]
        [SerializeField]
        private GameObject[] _characterOnPodiums;

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

        private void Start()
        {
            _userData = new UserData();
            _userData.Fetch();

            InitPoduim();
        }

        private void Update()
        {
            if (_isRotating)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                if (_currentCharacterIndex > 0)
                {
                    _currentCharacterIndex--;
                    StartCoroutine(RotatePodium(-1));

                    return;
                }

                _currentCharacterIndex = _characterOnPodiums.Length - 1;
                StartCoroutine(RotatePodium(-1, _notPlacedAngle, _rotationSpeed * _rotationCoefficientForEdges));
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                if (_currentCharacterIndex < _characterOnPodiums.Length - 1)
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

        private void InitPoduim()
        {
            _angle = _angleOfPlacing / _characterOnPodiums.Length;
            float radius = _mainPodium.transform.localScale.x / 2
                - _charactersPodium.transform.localScale.x / 2
                - _offsetPositionFromEdge;

            for (int i = 0; i < _characterOnPodiums.Length; i++)
            {
                float x = Mathf.Sin(Mathf.Deg2Rad * (_angle * i)) * radius;
                float z = Mathf.Cos(Mathf.Deg2Rad * (_angle * i)) * radius;
                float y = _mainPodium.transform.localScale.y + _charactersPodium.transform.localScale.y;

                _characterOnPodiums[i].transform.SetPositionAndRotation(new Vector3(x, y, z), Quaternion.identity);

                CharacterSaleController characterSaleController = _characterOnPodiums[i].GetComponentInChildren<CharacterSaleController>();
                _charactersToSale.Add(characterSaleController);
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
        }
    }
}
