﻿using Assets.Enums;
using Assets.Extensions;
using Assets.Helpers;
using Assets.Scripts.Maps;
using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Player
{
    public class PlayerController : PlayerAppearance
    {
        const string GroundLayerMaskName = "Ground";

        //private Joystick _joystick;

        /// <summary>
        /// Позиция рестарта игрока
        /// </summary>
        private Vector3 _restartPlayerTransformPosition;

        /// <summary>
        /// Поворот рестарта игрока
        /// </summary>
        private Quaternion _restartPlayerTransformRotation;

        [SerializeField]
        private InGameMenu _inGameMenu;

        [SerializeField]
        private MapController _mapController;

        [SerializeField]
        private FadeTextController _fadeTextController;

        [SerializeField]
        private string[] _riskTexts;

        [SerializeField]
        private GameObject _debugPanel;

        [SerializeField]
        private TMP_Text _forwardSpeedText;

        [SerializeField]
        private TMP_Text _sidewiseSpeedText;

        [SerializeField]
        private TMP_Text _strafeSpeedText;

        [SerializeField]
        private TMP_Text _velocityForwardText;

        [SerializeField]
        private TMP_Text _velocitySidewiseText;

        [SerializeField]
        private TMP_Text _metersText;

        [SerializeField]
        private TMP_Text _velocityMagnitudeText;

        [SerializeField]
        private Animator _animator;

        [Tooltip("Объект проверки контакта с землёй для выравнивания вперёд")]
        [SerializeField]
        private GameObject _groundForwardPoint;

        [Tooltip("Объект проверки контакта с землёй для выравнивания назад")]
        [SerializeField]
        private GameObject _groundBackwardPoint;

        [Tooltip("Объект проверки контакта с землёй для выравнивания влево")]
        [SerializeField]
        private GameObject _groundLeftPoint;

        [Tooltip("Объект проверки контакта с землёй для выравнивания вправо")]
        [SerializeField]
        private GameObject _groundRightPoint;

        [Tooltip("Длина луча для проверки контакта с землёй по выравниванию вперёд/назад")]
        [SerializeField]
        private float _groundCheckForwardLength = 0.3f;

        [Tooltip("Длина луча для проверки контакта с землёй по выравниванию по бокам")]
        [SerializeField]
        private float _groundCheckSidewiseLength = 0.3f;

        [Header("Очки")]

        [Tooltip("Денег в секунду на большой скорости")]
        [SerializeField]
        private int _moneyPerSecondOnHighSpeed = 4;

        /// <summary>
        /// Время проведённое на большой скорости с последнего изменения скорости
        /// </summary>
        private float _timeInHighSpeed;

        [Header("Скорости")]

        /// <summary>
        ///Коэфициент набора скорости по направлению лыж. Зависит от угла поворота. Прямо - быстрее, в боки - медленнее
        /// </summary>
        [Tooltip("Коэфициент набора скорости по направлению лыж. Зависит от угла поворота. Прямо - быстрее, в боки - медленнее")]
        [SerializeField]
        private float _velocityFreeRide = 0.24f;

        /// <summary>
        /// Боковая скорость при повороте. Зависит от угла поворота
        /// </summary>
        [Tooltip("Боковая скорость при повороте. Зависит от угла поворота")]
        [SerializeField]
        private float _velocitySidewise = 0.45f;

        /// <summary>
        /// Боковая скорость при стрейфе в крайнем положении
        /// </summary>
        [Tooltip("Боковая скорость при стрейфе в крайнем положении")]
        [SerializeField]
        private float _velocityStrafeCoefficient = 0.7f;

        /// <summary>
        /// Процент торможения при стрейфе от текущей скорости
        /// </summary>
        [Tooltip("Процент торможения при стрейфе от текущей скорости")]
        [SerializeField]
        private float _strafeStopperCoefficient = 0.3f;

        /// <summary>
        /// Скорость, ниже которой не может затормозить стрейф
        /// </summary>
        [Tooltip("Скорость, ниже которой не может затормозить стрейф")]
        [SerializeField]
        private float _strafeSpeedLimit = 1f;

        /// <summary>
        /// Процент скорости центробежной силы от текущей скорости
        /// </summary>
        [Tooltip("Процент скорости центробежной силы от текущей скорости")]
        [SerializeField]
        private float _centrifugalForceCoefficient = 0.011f;

        /// <summary>
        /// Скорость прямо для проигрыша
        /// </summary>
        [Tooltip("Скорость прямо для проигрыша")]
        [SerializeField]
        private float _deathSpeedX = 90f;

        /// <summary>
        /// Боковая скорость для проигрыша
        /// </summary>
        [Tooltip("Боковая скорость для проигрыша")]
        [SerializeField]
        private float _deathSpeedZ = 32;

        [Tooltip("Скорость на которой проигрывается анимации большой скорости")]
        [SerializeField]
        private float _deathAlertSpeedX = 60f;

        /// <summary>
        /// Скорость, на которой есть шанс потерять лыжи
        /// </summary>
        [Tooltip("Скорость, на которой есть шанс потерять лыжи")]
        [SerializeField]
        private float _velocityToLoseSki = 27;

        [Tooltip("Сила гравитации в полёте")]
        [SerializeField]
        private float _gravityBoost = 1f;

        [Tooltip("Скорость с которой дают деньги за скорость")]
        [SerializeField]
        private float _magnitudeSpeedToEarnMoney = 60f;

        [Tooltip("Скорость с которой считается риск")]
        [SerializeField]
        private float _magnitudeSpeedForRisk = 30f;

        [Tooltip("Делитель скорости для подсчёта награды за риск")]
        [SerializeField]
        private int _magnitudeSpeedDividerForRisk = 10;

        [Header("Повороты")]

        [Tooltip("Скорость поворота")]
        [SerializeField]
        private float speedOfNormalRotationY = 0.74f;

        [Tooltip("Скорость поворота в стрейфе")]
        [SerializeField]
        private float speedOfStrafeRotationY = 4.5f;

        [Tooltip("Скорость наклона вперёд назад")]
        [SerializeField]
        private float _speedOfTiltForward = 0.4f;

        [Tooltip("Скорость наклона по бокам")]
        [SerializeField]
        private float _speedOfTiltSidewise = 0.2f;

        [Header("Углы поворотов")]

        [Tooltip("Крайний угол поворота")]
        [SerializeField]
        private float angleOfTurn = 50f;

        [Tooltip("Крайний угол стрейфа")]
        [SerializeField]
        private float angleOfStrafe = 68f;

        /// <summary>
        /// true если персонаж касается земли
        /// </summary>
        private bool isGrounded = true;

        /// <summary>
        /// true если в стрэйфе
        /// </summary>
        private bool isInStrafe = false;

        /// <summary>
        /// true если держится кнопка поворота (до перехода в стрейф)
        /// </summary>
        private bool _isInTurning = false;

        /// <summary>
        /// Состояние проигрыша.
        /// <see cref="true"/> игрок проиграл
        /// <see cref="false"/> игрок ещё играет
        /// </summary>
        public bool IsLose { get; private set; }

        /// <summary>
        /// Значение точки по X, с которой игрок стартует
        /// </summary>
        private float _startPositionX;

        /// <summary>
        /// Текущее растояние, которое проехал игрок
        /// </summary>
        private int _currentMeters => Convert.ToInt32(transform.position.x - _startPositionX) / 6;

        /// <summary>
        /// Расстояние, на котором игрок проиграл
        /// </summary>
        private int _resultMeters;

        /// <summary>
        /// Количество заработанных денег за большую скорости
        /// </summary>
        private int _moneyForSpeed;

        /// <summary>
        /// Количество заработанных денег за риск
        /// </summary>
        private int _moneyForRisk;

        /// <summary>
        /// Угол поворота игрока текущий
        /// </summary>
        public float AngleOfCurrentTurning { get; private set; }

        /// <summary>
        /// Коэффициент угла поворота. 0 - нет поворота. 1 - вправо на 90. -1 - влево на 90
        /// </summary>
        private float _angleCoeficient => Mathf.Clamp(AngleOfCurrentTurning / 90f, -1, 1);

        /// <summary>
        /// Обратный коэфициент угла поворота. 1 если прямо. Чем дальше от центра, тем ближе к 0
        /// </summary>
        private float _angleCoeficientReversed => Mathf.Clamp(
            _angleCoeficient >= 0
                ? 1.1f - _angleCoeficient
                : 1.1f + _angleCoeficient,
            0,
            1);

        /// <summary>
        /// Отклонение в сторону по оси X (инпут клавы/джойстика)
        /// </summary>
        private float _axisX;

        /// <summary>
        /// Скорость стрейфа вбок
        /// </summary>
        private float _velocityStrafe => VelocityMagnitude * _velocityStrafeCoefficient;

        /// <summary>
        /// Скорость стрейфа торможения (назад)
        /// </summary>
        private float _velocityStrafeStopper => VelocityMagnitude * _strafeStopperCoefficient;

        /// <summary>
        /// Направление лыж
        /// </summary>
        Vector3 _skiesDirection => Vector3.ProjectOnPlane(transform.forward, Vector2.zero).normalized;

        [HideInInspector]
        public Rigidbody PlayerRigidBody { get; private set; }

        /// <summary>
        /// Скорость прямо по склонку
        /// </summary>
        public float VelocityForward => PlayerRigidBody.velocity.x;

        /// <summary>
        /// Скорость боковая
        /// </summary>
        public float VelocitySidewise => -PlayerRigidBody.velocity.z;

        /// <summary>
        /// Общая скорость
        /// </summary>
        public float VelocityMagnitude => PlayerRigidBody.velocity.magnitude;

        public Rigidbody[] ragdollRigidbody;
        public Transform[] bonesTransforms;

        [Tooltip("Объект для следования камеры")]
        [field: SerializeField]
        public GameObject ObjectForCameraFollowing { get; private set; }

        [HideInInspector]
        public Vector3[] defaultBonesPositions;
        [HideInInspector]
        public Quaternion[] defaultBonesRotations;
        private float[] bonesDefaultMass;

        [Header("Лыжи")]

        public PhysicMaterial skiMaterial;

        [SerializeField]
        private GameObject _leftSkiCollider;

        [SerializeField]
        private GameObject _rightSkiCollider;

        [SerializeField]
        private GameObject _leftSkiModel;

        [SerializeField]
        private GameObject _rightSkiModel;

        private Rigidbody _leftSkiRigidBody;
        private Rigidbody _rightSkiRigidBody;

        [SerializeField]
        private GameObject _leftFoot;

        [SerializeField] 
        private GameObject _rightFoot;

        private Vector3 _leftSkiColliderDefaultTransformPosition;
        private Vector3 _rightSkiColliderDefaultTransformPosition;
        private Quaternion _leftSkiColliderDefaultTransformRotation;
        private Quaternion _rightSkiColliderDefaultTransformRotation;


        private Vector3 _leftSkiModelDefaultTransformPosition;
        private Vector3 _rightSkiModelDefaultTransformPosition;
        private Quaternion _leftSkiModelDefaultTransformRotation;
        private Quaternion _rightSkiModelDefaultTransformRotation;

        public Transform forLeftSki;
        public Transform forRightSki;

        public enum LoseCause
        {
            fallX,
            fallZ,
            barrier
        }

        /// <summary>
        /// Флаг активности debug панели
        /// <see cref="true"/> панель активна
        /// <see cref="false"/> панель скрыта
        /// </summary>
        private bool _isDebugEnabled = false;

        public delegate void OnGroundOnDelegate();

        /// <summary>
        /// Событие приземления на поверхность
        /// </summary>
        public event OnGroundOnDelegate OnGroundOn;

        public delegate void OnGroundOffDelegate();

        /// <summary>
        /// Событие отрыва от поверхности
        /// </summary>
        public event OnGroundOffDelegate OnGroundOff;

        public delegate void OnStrafeDelegate();

        /// <summary>
        /// Событие входа в стрейф
        /// </summary>
        public event OnStrafeDelegate OnStrafeOn;

        /// <summary>
        /// Событие выхода из стрейфа
        /// </summary>
        public event OnStrafeDelegate OnStrafeOff;

        public delegate void OnTurningDelegate();

        /// <summary>
        /// Событие начала поворота
        /// </summary>
        public event OnTurningDelegate OnTurningOn;

        /// <summary>
        /// Событие окончания поворота
        /// </summary>
        public event OnTurningDelegate OnTurningOff;

        public delegate void OnLoseDelegate();
        
        /// <summary>
        /// Событие проигрыша игрока
        /// </summary>
        public event OnLoseDelegate OnLose;

        public delegate void OnBarrierCollisionDelegate();

        /// <summary>
        /// Событие столкновений с препятсвий
        /// </summary>
        public event OnBarrierCollisionDelegate OnBarrierCollision; 
        public delegate void OnRestartedDelegate();

        /// <summary>
        /// Событие перезапуска заезда
        /// </summary>
        public event OnRestartedDelegate OnRestarted;

        public delegate void OnRiskDelegate();

        /// <summary>
        /// Событие близкого проезда мимо преграды
        /// </summary>
        public event OnRiskDelegate OnRisk;

        private void Awake()
        {
            OnAwake();
            PlayerRigidBody = GetComponent<Rigidbody>();

            bonesDefaultMass = new float[bonesTransforms.Length];
            for (int i = 0; i < ragdollRigidbody.Length; i++)
            {
                bonesDefaultMass[i] = ragdollRigidbody[i].mass;
            }

            RagdollOff();

            _restartPlayerTransformPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            _restartPlayerTransformRotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
            defaultBonesPositions = new Vector3[bonesTransforms.Length];
            defaultBonesRotations = new Quaternion[bonesTransforms.Length];
            for (int i = 0; i < bonesTransforms.Length; i++)
            {
                defaultBonesPositions[i] = new Vector3(bonesTransforms[i].position.x, bonesTransforms[i].position.y, bonesTransforms[i].position.z);
                defaultBonesRotations[i] = new Quaternion(bonesTransforms[i].rotation.x, bonesTransforms[i].rotation.y, bonesTransforms[i].rotation.z, bonesTransforms[i].rotation.w);
            }

            _startPositionX = transform.position.x;

            _leftSkiColliderDefaultTransformPosition = _leftSkiCollider.transform.localPosition;
            _leftSkiColliderDefaultTransformRotation = _leftSkiCollider.transform.localRotation;
            _rightSkiColliderDefaultTransformPosition = _rightSkiCollider.transform.localPosition;
            _rightSkiColliderDefaultTransformRotation = _rightSkiCollider.transform.localRotation;

            _leftSkiModelDefaultTransformPosition = _leftSkiModel.transform.localPosition;
            _leftSkiModelDefaultTransformRotation = _leftSkiModel.transform.localRotation;
            _rightSkiModelDefaultTransformPosition = _rightSkiModel.transform.localPosition;
            _rightSkiModelDefaultTransformRotation = _rightSkiModel.transform.localRotation;

            _leftSkiRigidBody = forLeftSki.gameObject.GetComponent<Rigidbody>();
            _rightSkiRigidBody = forRightSki.gameObject.GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (!IsLose && Input.GetKeyDown(KeyCode.F9))
            {
                _isDebugEnabled = !_isDebugEnabled;
                _debugPanel.SetActive(!_debugPanel.activeSelf);
            }

            //_axisX = _joystick.Horizontal;
            //if (Input.GetAxis("Horizontal") != 0)
            //{
            _axisX = Input.GetAxis("Horizontal");
            //}

            AngleOfCurrentTurning = -(90f - Vector3.Angle(_skiesDirection, Vector3.forward));

            PrintText(_velocityForwardText, VelocityForward);
            PrintText(_velocitySidewiseText, VelocitySidewise);
            PrintText(_velocityMagnitudeText, VelocityMagnitude);
            PrintText(_metersText, $"{_currentMeters} м");
            if (!isInStrafe)
            {
                PrintText(_strafeSpeedText, "0");
            }
        }

        private void FixedUpdate()
        {
            if (IsLose)
            {
                AddGravityForce();
                return;
            }

            if (VelocityForward >= _deathSpeedX)
            {
                Lose(LoseCause.fallX);
            }

            if (Mathf.Abs(VelocitySidewise) >= _deathSpeedZ)
            {
                Lose(LoseCause.fallZ);
            }

            FlatToGroundByForward();
            FlatToGroundBySidewise();
            RideForward();
            MoveSidewise();
            RotateBody();
            EarnMoneyForSpeed();
        }

        /// <summary>
        /// Выровнять игрока относительно земли по оси прямо, применить гравитацию
        /// </summary>
        private void FlatToGroundByForward()
        {
            Ray rayForward = new(_groundForwardPoint.transform.position, -transform.up);
            bool needToFlatForward = !Physics.Raycast(rayForward, out RaycastHit hitForward, _groundCheckForwardLength, LayerMask.GetMask(GroundLayerMaskName));

            Ray rayBackward = new(_groundBackwardPoint.transform.position, -transform.up);
            bool needToFlatBackward = !Physics.Raycast(rayBackward, out RaycastHit hitBackward, _groundCheckForwardLength, LayerMask.GetMask(GroundLayerMaskName));

            // Если оба бока в небе, не надо поворачиваться
            if (needToFlatForward && needToFlatBackward)
            {
                if (isGrounded)
                {
                    isGrounded = false;
                    OnGroundOff?.Invoke();
                }

                AddGravityForce();
                return;
            }

            // Если хотя бы один на земле, значит приземлились
            if (!isGrounded)
            {
                isGrounded = true;
                OnGroundOn?.Invoke();
            }

            Vector3 normal = needToFlatForward ? hitForward.normal : hitBackward.normal;
            int turningCoefficient = needToFlatForward ? 1 : -1;
            PlayerRigidBody.AddTorque(_speedOfTiltForward * turningCoefficient * Vector3.Cross(transform.forward, normal), ForceMode.Force);

            if (needToFlatForward)
            {
                Debug.DrawRay(_groundForwardPoint.transform.position, -transform.up * _groundCheckForwardLength, Color.red);
            }
            if (needToFlatBackward)
            {
                Debug.DrawRay(_groundBackwardPoint.transform.position, -transform.up * _groundCheckForwardLength, Color.red);
            }
        }

        /// <summary>
        /// Выровнять игрока относительно земли по оси вбок
        /// </summary>
        private void FlatToGroundBySidewise()
        {
            Ray rayLeft = new(_groundLeftPoint.transform.position, -transform.up);
            bool needToFlatLeft = !Physics.Raycast(rayLeft, out RaycastHit hitLeft, _groundCheckSidewiseLength, LayerMask.GetMask(GroundLayerMaskName));

            Ray rayRight = new(_groundRightPoint.transform.position, -transform.up);
            bool needToFlatRight = !Physics.Raycast(rayRight, out RaycastHit hitRight, _groundCheckSidewiseLength, LayerMask.GetMask(GroundLayerMaskName));

            // Если оба бока в небе, не надо поворачиваться
            if (needToFlatLeft && needToFlatRight)
            {
                return;
            }

            Vector3 normal = needToFlatRight ? hitRight.normal : hitLeft.normal;
            int turningCoefficient = needToFlatRight ? 1 : -1;
            PlayerRigidBody.AddTorque(_speedOfTiltSidewise * turningCoefficient * Vector3.Cross(transform.right, normal), ForceMode.Force);

            if (needToFlatLeft)
            {
                Debug.DrawRay(_groundLeftPoint.transform.position, -transform.up * _groundCheckForwardLength, Color.blue);
            }
            if (needToFlatRight)
            {
                Debug.DrawRay(_groundRightPoint.transform.position, -transform.up * _groundCheckForwardLength, Color.blue);
            }
        }

        private void AddGravityForce()
        {
            // для улучшения гравитациии
            PlayerRigidBody.AddForce(Vector3.down * _gravityBoost, ForceMode.Force);
            Debug.DrawRay(transform.position, Vector3.down * _gravityBoost, ColorHelper.FromHex("#db67e6"));
        }

        /// <summary>
        /// Катить лыжи вперёд по их направлению поворота относительно склона
        /// </summary>
        private void RideForward()
        {
            if (!isGrounded || isInStrafe || IsLose )
            {
                return;
            }

            // Катить по направлению лыж
            float impulse = _angleCoeficientReversed * _velocityFreeRide;
            PlayerRigidBody.AddForce(impulse * _skiesDirection, ForceMode.Force);
            Debug.DrawRay(transform.position, impulse * _skiesDirection, Color.red);

            if (VelocityForward > _deathAlertSpeedX)
            {
                _animator.SetBool("isGoingFast", true);
            }
            else
            {
                _animator.SetBool("isGoingFast", false);
            }

            PrintText(_forwardSpeedText, impulse);
        }

        /// <summary>
        /// Смещение вбок из-за угла поворота лыж
        /// </summary>
        private void MoveSidewise()
        {
            if (!isGrounded || isInStrafe || IsLose)
            {
                return;
            }

            float impulse = _angleCoeficient * _velocitySidewise;
            PlayerRigidBody.AddForce(impulse * transform.right, ForceMode.Force);
            Debug.DrawRay(transform.position, impulse * transform.right, Color.blue);

            PrintText(_sidewiseSpeedText, impulse);
        }

        /// <summary>
        /// Обработчик поворота корпуса
        /// </summary>
        private void RotateBody()
        {
            // Если повороты по воздуху
            if (!isGrounded)
            {
                RotatePlayerInAir();
                return;
            }

            if (_axisX == 0)
            {
                ReturnToEdgeTurnRotation();
                return;
            }

            _animator.ResetTrigger("toDefault");

            RotatePlayerOnGround();
        }

        /// <summary>
        /// Повернуть персонажа до крайнего возможного поворота перед зоной стрейфа
        /// </summary>
        private void ReturnToEdgeTurnRotation()
        {
            StrafeTurnOff();
            TurningTurnOff();
            _animator.SetTrigger("toDefault");

            //восстанавливаем положение лыж
            if (AngleOfCurrentTurning > angleOfTurn)
            {
                RotateBodySidewise(-1, speedOfStrafeRotationY);
            }
            else if (AngleOfCurrentTurning < -angleOfTurn)
            {
                RotateBodySidewise(1, speedOfStrafeRotationY);
            }
        }

        /// <summary>
        /// Повернуть корпуск персонажа в бок
        /// </summary>
        /// <param name="rotationSpeedModifier">Модификатор скорости поворота. Меньше 0 - поворот влево, больше 0 - поворот вправо</param>
        /// <param name="speed">Базовая скорость поворота</param>
        private void RotateBodySidewise(float rotationSpeedModifier, float speed)
        {
            PlayerRigidBody.AddTorque(rotationSpeedModifier * speed * transform.up, ForceMode.VelocityChange);
        }

        /// <summary>
        /// Поворот корпуса персонажа в воздухе
        /// </summary>
        private void RotatePlayerInAir()
        {
            RotateBodySidewise(_axisX * 0.33f, speedOfNormalRotationY);
        }

        /// <summary>
        /// Поворачивать игрока по земле
        /// </summary>
        private void RotatePlayerOnGround()
        {
            // налево повернуть корпус
            if (_axisX < 0)
            {
                //сначала поворачиваемся до нужного угла
                if (AngleOfCurrentTurning > -angleOfTurn)
                {
                    StrafeTurnOff();
                    TurningTurnOn(Sides.Left);

                    // поворот
                    RotateBodySidewise(_axisX, speedOfNormalRotationY);

                    // центробежная скорость
                    float impulse = VelocityMagnitude * _centrifugalForceCoefficient;
                    PlayerRigidBody.AddForce(impulse * -transform.right, ForceMode.Force);
                    Debug.DrawRay(transform.position, impulse * -transform.right, Color.green);
                    return;
                }

                //затем когда дошли до угла максимального то держим скорость в пределе (входим в стрэйф)
                StrafeTurnOn(Sides.Left);
                TurningTurnOff();
                if (AngleOfCurrentTurning > -angleOfStrafe)
                {
                    RotateBodySidewise(_axisX, speedOfStrafeRotationY);
                }

                if (VelocityMagnitude > _strafeSpeedLimit)
                {
                    //сила назад
                    PlayerRigidBody.AddForce(Vector3.left * _velocityStrafeStopper, ForceMode.Force);
                    Debug.DrawRay(transform.position, Vector3.left * _velocityStrafeStopper, Color.black);
                    //сила в бок
                    float impulse = _axisX * _velocityStrafe;
                    PlayerRigidBody.AddForce(impulse * transform.right, ForceMode.Impulse);
                    Debug.DrawRay(transform.position, impulse * transform.right, Color.red);

                    PrintText(_strafeSpeedText, impulse);
                }
                else
                {
                    PlayerRigidBody.velocity = Vector3.zero;
                }
            }
            //направо повернуть корпус
            else if (_axisX > 0)
            {
                //сначала поворачиваемся до нужного угла
                if (AngleOfCurrentTurning < angleOfTurn)
                {
                    StrafeTurnOff();
                    TurningTurnOn(Sides.Right);

                    // поворот
                    RotateBodySidewise(_axisX, speedOfNormalRotationY);

                    // центробежная скорость
                    float impulse = VelocityMagnitude * _centrifugalForceCoefficient;
                    PlayerRigidBody.AddForce(impulse * transform.right, ForceMode.Force);
                    Debug.DrawRay(transform.position, impulse * transform.right, Color.green);
                    return;
                }

                //затем когда дошли до угла максимального то держим скорость в пределе (входим в стрэйф)

                StrafeTurnOn(Sides.Right);
                TurningTurnOff();
                if (AngleOfCurrentTurning < angleOfStrafe)
                {
                    RotateBodySidewise(_axisX, speedOfStrafeRotationY);
                }

                if (VelocityMagnitude > _strafeSpeedLimit)
                {
                    //сила назад
                    PlayerRigidBody.AddForce(Vector3.left * _velocityStrafeStopper, ForceMode.Force);
                    Debug.DrawRay(transform.position, Vector3.left * _velocityStrafeStopper, Color.black);
                    //сила в бок
                    float impulse = _axisX * _velocityStrafe;
                    PlayerRigidBody.AddForce(impulse * -transform.right, ForceMode.Impulse);
                    Debug.DrawRay(transform.position, impulse * transform.right, Color.red);

                    PrintText(_strafeSpeedText, impulse);
                }
                else
                {
                    PlayerRigidBody.velocity = Vector3.zero;
                }
            }
        }

        private void EarnMoneyForSpeed()
        {
            if (PlayerRigidBody.velocity.magnitude > _magnitudeSpeedToEarnMoney)
            {
                _timeInHighSpeed += Time.deltaTime;
                if (_timeInHighSpeed > 1f)
                {
                    _moneyForSpeed += _moneyPerSecondOnHighSpeed;
                    _timeInHighSpeed = 0f;
                }
            }
            else
            {
                _timeInHighSpeed = 0f;
            }
        }

        public void EarnMoneyForRisk()
        {
            if (IsLose || PlayerRigidBody.velocity.magnitude < _magnitudeSpeedForRisk)
            {
                return;
            }

            int riskScore = (int)PlayerRigidBody.velocity.magnitude / _magnitudeSpeedDividerForRisk;
            _moneyForRisk += riskScore;

            string greetText = _riskTexts.ElementAt(UnityEngine.Random.Range(0, _riskTexts.Length - 1));
            _fadeTextController.Show($"{greetText}{Environment.NewLine}+{riskScore}");


            OnRisk?.Invoke();
        }

        private void RagdollOn()
        {
            _animator.enabled = false;
            _animator.ResetToEntryState();
            for (int i = 0; i < ragdollRigidbody.Length; i++)
            {
                ragdollRigidbody[i].isKinematic = false;
                ragdollRigidbody[i].velocity = PlayerRigidBody.velocity;
                ragdollRigidbody[i].angularVelocity = PlayerRigidBody.angularVelocity;
                ragdollRigidbody[i].mass = bonesDefaultMass[i];
            }
        }

        public void RagdollOff()
        {
            _animator.ResetToEntryState();
            _animator.enabled = true;
            for (int i = 0; i < ragdollRigidbody.Length; i++)
            {
                ragdollRigidbody[i].isKinematic = true;
                ragdollRigidbody[i].velocity = Vector3.zero;
                ragdollRigidbody[i].angularVelocity = Vector3.zero;
                ragdollRigidbody[i].mass = 0f;
            }
        }

        public void Lose(LoseCause cause)
        {
            if (IsLose)
            {
                return;
            }

            _resultMeters = _currentMeters;
            _metersText.gameObject.SetActive(false);

            StartCoroutine(CalcScoreAndShowLoseMenu());

            _rightSkiCollider.layer = 7;
            _leftSkiCollider.layer = 7;

            //_joystick.OnPointerUp(new PointerEventData(null));
            //_joystick.gameObject.SetActive(false);

            _inGameMenu.pauseButton.SetActive(false);

            SetSkiToFeet();

            PlayerRigidBody.constraints = RigidbodyConstraints.None;
            if (cause == LoseCause.fallX)
            {
                StartCoroutine(Fall(-transform.right));
            }
            else if (cause == LoseCause.fallZ)
            {
                LoseSki();

                // TODO: разобраться с векторами падений
                if (VelocitySidewise > 0)
                    StartCoroutine(Fall(-transform.forward));
                else
                    StartCoroutine(Fall(transform.forward));
            }
            else if (cause == LoseCause.barrier)
            {
                LoseSki();

                RagdollOn();
                PlayerRigidBody.angularDrag = 0.06f;
                OnBarrierCollision?.Invoke();
            }

            IsLose = true;
            OnLose?.Invoke();
        }

        private void LoseSki()
        {
            bool isLeftSkiOff = false;
            bool isRightSkiOff = false;
            if (VelocityMagnitude >= _velocityToLoseSki)
            {
                int result = UnityEngine.Random.Range(0, 9);

                if (result == 0 || result == 1)
                {
                    isLeftSkiOff = true;
                }
                else if (result == 2 || result == 3)
                {
                    isRightSkiOff = true;
                }
                else if (result == 4)
                {
                    isLeftSkiOff = true;
                    isRightSkiOff = true;
                }
            }

            var vectorToLose = -_skiesDirection;
            if (isLeftSkiOff)
            {
                _leftSkiModel.transform.SetParent(forLeftSki, true);

                _leftSkiCollider.transform.SetParent(_leftSkiModel.transform, true);
                _leftSkiCollider.GetComponent<CapsuleCollider>().material = skiMaterial;

                _leftSkiRigidBody.isKinematic = false;
                _leftSkiRigidBody.AddForce(vectorToLose, ForceMode.Impulse);
            }
            if (isRightSkiOff)
            {
                _rightSkiModel.transform.SetParent(forRightSki, true);

                _rightSkiCollider.transform.SetParent(_rightSkiModel.transform, true);
                _rightSkiCollider.GetComponent<CapsuleCollider>().material = skiMaterial;

                _rightSkiRigidBody.isKinematic = false;
                _rightSkiRigidBody.AddForce(vectorToLose, ForceMode.Impulse);
            }
        }

        public void RestartToDefaultPosition()
        {
            OnRestarted?.Invoke();

            StrafeTurnOff();
            TurningTurnOff();

            if (_isDebugEnabled)
            {
                _debugPanel.SetActive(true);
            }

            _metersText.gameObject.SetActive(true);
            _resultMeters = 0;
            _moneyForSpeed = 0;
            _moneyForRisk = 0;
            _timeInHighSpeed = 0;

            //_joystick.gameObject.SetActive(true);
            PlayerRigidBody.velocity = Vector3.zero;
            PlayerRigidBody.angularVelocity = Vector3.zero;
            transform.SetPositionAndRotation(_restartPlayerTransformPosition, _restartPlayerTransformRotation);
            PlayerRigidBody.angularDrag = 18;
            PlayerRigidBody.constraints = RigidbodyConstraints.FreezeRotationZ;
            IsLose = false;

            RagdollOff();

            ReturnSkiToDefaultPosition();
        }

        /// <summary>
        /// Прицепить лыжи к ногам (только для регдола будет работать адекватно)
        /// </summary>
        private void SetSkiToFeet()
        {
            _leftSkiCollider.transform.SetParent(_leftFoot.transform);
            _leftSkiModel.transform.SetParent(_leftSkiCollider.transform);

            _rightSkiCollider.transform.SetParent(_rightFoot.transform);
            _rightSkiModel.transform.SetParent(_rightSkiCollider.transform);

            _leftSkiCollider.GetComponent<CapsuleCollider>().material = ragdollRigidbody[0].GetComponent<BoxCollider>().material;
            _rightSkiCollider.GetComponent<CapsuleCollider>().material = ragdollRigidbody[0].GetComponent<BoxCollider>().material;
        }

        /// <summary>
        /// Вернуть лыжи на исходную позицию
        /// </summary>
        private void ReturnSkiToDefaultPosition()
        {
            _leftSkiCollider.transform.SetParent(transform);
            _leftSkiCollider.transform.SetLocalPositionAndRotation(_leftSkiColliderDefaultTransformPosition, _leftSkiColliderDefaultTransformRotation);
            _leftSkiCollider.GetComponent<CapsuleCollider>().material = skiMaterial;
            _leftSkiCollider.layer = 10;

            _rightSkiCollider.transform.SetParent(transform);
            _rightSkiCollider.transform.SetLocalPositionAndRotation(_rightSkiColliderDefaultTransformPosition, _rightSkiColliderDefaultTransformRotation);
            _rightSkiCollider.GetComponent<CapsuleCollider>().material = skiMaterial;
            _rightSkiCollider.layer = 10;

            _leftSkiModel.transform.SetParent(_leftFoot.transform);
            _leftSkiModel.transform.SetLocalPositionAndRotation(_leftSkiModelDefaultTransformPosition, _leftSkiModelDefaultTransformRotation);

            _rightSkiModel.transform.SetParent(_rightFoot.transform);
            _rightSkiModel.transform.SetLocalPositionAndRotation(_rightSkiModelDefaultTransformPosition, _rightSkiModelDefaultTransformRotation);

            forLeftSki.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            forRightSki.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }

        private IEnumerator Fall(Vector3 direction)
        {
            PlayerRigidBody.angularDrag = 0.06f;
            RagdollOn();
            for (int i = 0; i < 6; i++)
            {
                //torsoRb.AddTorque(direction * 14000, ForceMode.Force);
                yield return new WaitForFixedUpdate();
            }
        }

        private IEnumerator CalcScoreAndShowLoseMenu()
        {
            int moneyForMeters = Mathf.Max(_resultMeters / _userDataController.UserDataModel.MetersScoreDelimeter, 1);
            int totalMoney = moneyForMeters + _moneyForSpeed + _moneyForRisk;
            _userDataController.UserDataModel.EarnMoneyAndTrySetBestMetersRecord(_resultMeters, totalMoney);


            yield return new WaitForSeconds(3);

            _debugPanel.SetActive(false);

            _mapController.ShowLoseMenu(
                meters: _resultMeters,
                bestMeters: _userDataController.UserDataModel.BestMetersRecord,
                scoreForMeters: moneyForMeters,
                scoreForSpeed: _moneyForSpeed,
                scoreForRisk: _moneyForRisk);

            _timeInHighSpeed = 0;
            _moneyForSpeed = 0;
            _moneyForRisk = 0;
        }

        /// <summary>
        /// Обновить текст в <see cref="TMP_Text"/>, если он не <see cref="null"/>
        /// </summary>
        /// <param name="textMesh"></param>
        /// <param name="obj"></param>
        private void PrintText(TMP_Text textMesh, object obj)
        {
            if (textMesh == null)
            {
                return;
            }

            textMesh.text = obj.ToString();
        }

        /// <summary>
        /// Выйти из стрейфа
        /// </summary>
        private void StrafeTurnOff()
        {
            if (!isInStrafe)
            {
                return;
            }

            isInStrafe = false;
            _animator.SetBool("isInStrafeLeft", false);
            _animator.SetBool("isInStrafeRight", false);

            OnStrafeOff?.Invoke();
        }

        /// <summary>
        /// Войти в стрейф
        /// </summary>
        /// <param name="side">Направление движения</param>
        private void StrafeTurnOn(Sides side)
        {
            if (isInStrafe)
            {
                return;
            }

            isInStrafe = true;
            _animator.SetBool($"isInStrafe{side}", true);

            OnStrafeOn?.Invoke();
        }

        private void TurningTurnOff()
        {
            if (!_isInTurning)
            {
                return;
            }

            _isInTurning = false;
            _animator.SetBool($"isInTurningLeft", false);
            _animator.SetBool($"isInTurningRight", false);

            OnTurningOff?.Invoke();
        }

        private void TurningTurnOn(Sides side)
        {
            if (!_isInTurning)
            {
                OnTurningOn?.Invoke();
            }

            _isInTurning = true;
            if (side == Sides.Left)
            {
                _animator.SetBool($"isInTurningLeft", true);
                _animator.SetBool($"isInTurningRight", false);
            }
            else if (side == Sides.Right)
            {
                _animator.SetBool($"isInTurningLeft", false);
                _animator.SetBool($"isInTurningRight", true);
            }
        }
    }
}
