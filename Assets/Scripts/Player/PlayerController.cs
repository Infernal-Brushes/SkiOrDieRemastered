using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        public Joystick joystick;

        /// <summary>
        /// Позиция рестарта игрока
        /// </summary>
        private Vector3 _restartPlayerTransformPosition;

        /// <summary>
        /// Поворот рестарта игрока
        /// </summary>
        private Quaternion _restartPlayerTransformRotation;

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
        private TMP_Text _scoreText;

        /// <summary>
        /// Объект проверки контакта с землёй
        /// </summary>
        [Tooltip("Объект проверки контакта с землёй")]
        public GameObject groundPoint;

        /// <summary>
        /// Длина луча для проверки контакта с землёй
        /// </summary>
        [Tooltip("Длина луча для проверки контакта с землёй")]
        public float groundCheckLength = 0.5f;
        /// <summary>
        /// Скорость наклона вперёд назад
        /// </summary>
        [Tooltip("Скорость наклона вперёд назад")]
        public float speedOfTiltX = 0.3f;
        /// <summary>
        /// Скорость, ниже которой не может затормозить стрейф
        /// </summary>
        [Tooltip("Скорость, ниже которой не может затормозить стрейф")]
        public float strafeSpeedLimit = 15f;
        public float deathSpeedX = 35;
        public float deathSpeedZ = 28;

        public float velocityToLoseSki = 28;

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

        [Header("Повороты")]

        /// <summary>
        /// Скорость поворота
        /// </summary>
        [Tooltip("Скорость поворота")]
        public float speedOfNormalRotationY = 0.3f;

        /// <summary>
        /// Скорость поворота в стрейфе
        /// </summary>
        [Tooltip("Скорость поворота в стрейфе")]
        public float speedOfStrafeRotationY = 1.3f;

        [Header("Углы поворотов")]

        /// <summary>
        /// Крайний угол поворота
        /// </summary>
        [Tooltip("Крайний угол поворота")]
        public float angleOfTurn = 50f;

        /// <summary>
        /// Крайний угол стрейфа
        /// </summary>
        [Tooltip("Крайний угол стрейфа")]
        public float angleOfStrafe = 65f;

        /// <summary>
        /// true если персонаж касается земли
        /// </summary>
        bool isGrounded = true;

        /// <summary>
        /// true если в стрэйфе
        /// </summary>
        bool isInStrafe = false;

        /// <summary>
        /// Состояние проигрыша.
        /// <see cref="true"/> игрок проиграл
        /// <see cref="false"/> игрок ещё играет
        /// </summary>
        private bool isLose = false;

        /// <summary>
        /// Значение точки по X, с которой игрок стартует
        /// </summary>
        private float _startPositionX;

        /// <summary>
        /// Текущее растояние, которое проехал игрок
        /// </summary>
        private int _currentMeters => Convert.ToInt32(transform.position.x - _startPositionX);

        /// <summary>
        /// Угол поворота игрока
        /// </summary>
        private float _angle;

        /// <summary>
        /// Коэффициент угла поворота. 0 - нет поворота. 1 - вправо на 90. -1 - влево на 90
        /// </summary>
        private float _angleCoeficient => Mathf.Clamp(_angle / -90f, -1, 1);

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
        private float _velocityStrafe => playerRigidBody.velocity.x * _velocityStrafeCoefficient;

        /// <summary>
        /// Скорость стрейфа торможения (назад)
        /// </summary>
        private float _velocityStrafeStopper => playerRigidBody.velocity.x * _strafeStopperCoefficient;


        [HideInInspector]
        public Rigidbody playerRigidBody;

        public Rigidbody[] ragdollRigidbody;
        public Transform[] bonesTransforms;
        [HideInInspector]
        public Vector3[] defaultBonesPositions;
        [HideInInspector]
        public Quaternion[] defaultBonesRotations;
        private float[] bonesDefaultMass;

        public PhysicMaterial skiMaterial;
        public GameObject leftSki;
        public GameObject rightSki;
        public GameObject leftFoot;
        public GameObject rightFoot;
        public Transform forLeftSki;
        public Transform forRightSki;

        public enum LoseCause
        {
            fallX,
            fallZ,
            barrier
        }

        /// <summary>
        /// Направление лыж
        /// </summary>
        Vector3 _skiesDirection => Vector3.ProjectOnPlane(transform.forward, Vector2.zero).normalized;

        private void Awake()
        {
            playerRigidBody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
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
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                RagdollOn();
            }

            if (!isLose && Input.GetKeyDown(KeyCode.Q))
            {
                _debugPanel.SetActive(!_debugPanel.activeSelf);
            }

            _axisX = joystick.Horizontal;
            if (Input.GetAxis("Horizontal") != 0)
            {
                _axisX = Input.GetAxis("Horizontal");
            }

            _angle = 90f - Vector3.Angle(_skiesDirection, Vector3.forward);

            PrintText(_velocityForwardText, playerRigidBody.velocity.x);
            PrintText(_velocitySidewiseText, (-playerRigidBody.velocity.z));
            PrintText(_scoreText, $"{_currentMeters} m");
            if (!isInStrafe)
            {
                PrintText(_strafeSpeedText, "0");
            }
        }

        private void FixedUpdate()
        {
            if (isLose)
                return;

            Debug.DrawRay(groundPoint.transform.position, groundPoint.transform.up, Color.red, 12);

            if (playerRigidBody.velocity.x >= deathSpeedX)
            {
                Lose(LoseCause.fallX);
            }

            FlatToGround();
            RideForward();
            MoveSidewise();
            RotateBody();
        }

        /// <summary>
        /// Выровнять игрока относительно земли, применить гравитацию
        /// </summary>
        private void FlatToGround()
        {
            Ray ray = new(groundPoint.transform.position, -transform.up);

            // если игрок касается земли
            if (Physics.Raycast(ray, out RaycastHit hit, groundCheckLength, LayerMask.GetMask("Ground")))
            {
                if (!isGrounded)
                {
                    isGrounded = true;

                    // направление наклона. 1 - накланять вперёд, -1 - наклонять назад
                    int turningCoefficient = 1;
                    // если игрок заваливается назад то надо его вперёд наклонять а не назад
                    if (Vector3.Angle(transform.up, hit.normal) <= 90)
                    {
                        turningCoefficient = -1;
                    }
                    // задаём крутящий момент по ортогонали между нормальню и вектором Y у игрока (вокруг оси X вращение)
                    playerRigidBody.AddTorque(speedOfTiltX * turningCoefficient * Vector3.Cross(transform.forward, hit.normal), ForceMode.Impulse);
                }
                return;
            }

            isGrounded = false;

            // для улучшения гравитациии
            playerRigidBody.AddForce(Vector3.down * 1f, ForceMode.Impulse);
        }

        /// <summary>
        /// Катить лыжи вперёд по их направлению поворота относительно склона
        /// </summary>
        private void RideForward()
        {
            if (!isGrounded || isInStrafe || isLose )
            {
                return;
            }

            // Катить по направлению лыж
            float impulse = _angleCoeficientReversed * _velocityFreeRide;
            playerRigidBody.AddForce(impulse * _skiesDirection, ForceMode.Impulse);

            PrintText(_forwardSpeedText, impulse);
        }

        /// <summary>
        /// Смещение вбок из-за угла поворота лыж
        /// </summary>
        private void MoveSidewise()
        {
            if (isInStrafe)
            {
                return;
            }

            float impulse = _angleCoeficient * _velocitySidewise;
            playerRigidBody.AddForce(impulse * transform.right, ForceMode.Impulse);

            PrintText(_sidewiseSpeedText, impulse);
        }

        /// <summary>
        /// Поворот лыж
        /// </summary>
        private void RotateBody()
        {
            float angleY = transform.rotation.eulerAngles.y;

            // Если повороты по воздуху
            if (!isGrounded)
            {
                RotatePlayerInAir(angleY);
                return;
            }

            if (_axisX == 0)
            {
                ReturningToMainRotation(angleY);
                return;
            }

            RotatePlayerOnGround(angleY);
        }

        private void RotatePlayerInAir(float angleY)
        {
            // повороты в воздухе
            if (_axisX < 0 && angleY > 90 - angleOfStrafe)
            {
                playerRigidBody.AddTorque(transform.up * speedOfStrafeRotationY * 0.33f * _axisX, ForceMode.VelocityChange);
            }
            else if (_axisX > 0 && angleY < 90 + angleOfStrafe)
            {
                playerRigidBody.AddTorque(transform.up * speedOfStrafeRotationY * 0.33f * _axisX, ForceMode.VelocityChange);
            }
        }

        private void ReturningToMainRotation(float angleY)
        {
            isInStrafe = false;
            //восстанавливаем положение лыж
            if (angleY > 90 + angleOfTurn)
            {
                playerRigidBody.AddTorque(transform.up * -speedOfStrafeRotationY, ForceMode.VelocityChange);
            }
            else if (angleY < 90 - angleOfTurn)
            {
                playerRigidBody.AddTorque(transform.up * speedOfStrafeRotationY, ForceMode.VelocityChange);
            }
        }

        private void RotatePlayerOnGround(float angleY)
        {
            // налево повернуть корпус
            if (_axisX < 0)
            {
                //сначала поворачиваемся до нужного угла
                if (angleY > 90 - angleOfTurn)
                {
                    isInStrafe = false;
                    playerRigidBody.AddTorque(_axisX * speedOfNormalRotationY * transform.up, ForceMode.VelocityChange);
                    return;
                }

                //затем когда дошли до угла максимального то держим скорость в пределе (входим в стрэйф)
                isInStrafe = true;
                if (angleY > 90 - angleOfStrafe)
                {
                    playerRigidBody.AddTorque(transform.up * speedOfStrafeRotationY * _axisX, ForceMode.VelocityChange);
                }

                if (playerRigidBody.velocity.x > strafeSpeedLimit)
                {
                    //сила назад
                    Debug.Log(_velocityStrafeStopper);
                    playerRigidBody.AddForce(Vector3.left * _velocityStrafeStopper, ForceMode.Impulse);
                    //сила в бок
                    float impulse = -_axisX * _velocityStrafe;
                    playerRigidBody.AddForce(impulse * transform.right, ForceMode.Impulse);

                    PrintText(_strafeSpeedText, impulse);
                }
                else
                {
                    playerRigidBody.velocity = Vector3.zero;
                }
            }
            //направо повернуть корпус
            else if (_axisX > 0)
            {
                //сначала поворачиваемся до нужного угла
                if (angleY < 90 + angleOfTurn)
                {
                    isInStrafe = false;
                    playerRigidBody.AddTorque(_axisX * speedOfNormalRotationY * transform.up, ForceMode.VelocityChange);
                    return;
                }

                //затем когда дошли до угла максимального то держим скорость в пределе (входим в стрэйф)
                isInStrafe = true;
                if (angleY < 90 + angleOfStrafe)
                {
                    playerRigidBody.AddTorque(transform.up * speedOfStrafeRotationY * _axisX, ForceMode.VelocityChange);
                }

                if (playerRigidBody.velocity.x > strafeSpeedLimit)
                {
                    //сила назад
                    Debug.Log(_velocityStrafeStopper);
                    playerRigidBody.AddForce(Vector3.left * _velocityStrafeStopper, ForceMode.Impulse);
                    //сила в бок
                    float impulse = -_axisX * _velocityStrafe;
                    playerRigidBody.AddForce(impulse * transform.right, ForceMode.Impulse);

                    PrintText(_strafeSpeedText, impulse);
                }
                else
                {
                    playerRigidBody.velocity = Vector3.zero;
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.gameObject.TryGetComponent(out Barrier barrier) && !isLose)
            {
                Lose(LoseCause.barrier);
            }
        }

        private void RagdollOn()
        {
            for (int i = 0; i < ragdollRigidbody.Length; i++)
            {
                ragdollRigidbody[i].isKinematic = false;
                ragdollRigidbody[i].velocity = playerRigidBody.velocity;
                ragdollRigidbody[i].angularVelocity = playerRigidBody.angularVelocity;
                ragdollRigidbody[i].mass = bonesDefaultMass[i];
            }
        }

        public void RagdollOff()
        {
            for (int i = 0; i < ragdollRigidbody.Length; i++)
            {
                ragdollRigidbody[i].isKinematic = true;
                ragdollRigidbody[i].velocity = Vector3.zero;
                ragdollRigidbody[i].angularVelocity = Vector3.zero;
                ragdollRigidbody[i].mass = default;
            }
        }

        public void Lose(LoseCause cause)
        {
            if (isLose)
                return;

            rightSki.layer = 7;
            leftSki.layer = 7;

            joystick.OnPointerUp(new PointerEventData(null));
            joystick.gameObject.SetActive(false);

            var ingameMenu = FindObjectOfType<InGameMenu>();
            ingameMenu.pauseButton.SetActive(false);

            playerRigidBody.constraints = RigidbodyConstraints.None;
            if (cause == LoseCause.fallX)
            {
                Debug.Log("Проигрыш! Большая скорость прямо");

                //прицепляем лыжи к ногам
                leftSki.GetComponent<CapsuleCollider>().material = ragdollRigidbody[0].GetComponent<BoxCollider>().material;
                leftSki.transform.SetParent(leftFoot.transform);
                rightSki.GetComponent<CapsuleCollider>().material = ragdollRigidbody[0].GetComponent<BoxCollider>().material;
                rightSki.transform.SetParent(rightFoot.transform);

                StartCoroutine(Fall(-transform.right));
            }
            else if (cause == LoseCause.fallZ)
            {
                Debug.Log("Проигрыш! Большая скорость стрейфа");

                LoseSki();


                if (playerRigidBody.velocity.z > 0)
                    StartCoroutine(Fall(transform.forward));
                else
                    StartCoroutine(Fall(-transform.forward));
            }
            else if (cause == LoseCause.barrier)
            {
                //StartCoroutine(SlowMotionOnLose());
                LoseSki();

                RagdollOn();
                playerRigidBody.angularDrag = 0.06f;
                //Debug.Log("Проигрыш! Врезался в препятствие");
            }

            StartCoroutine(ShowLoseMenu());
            isLose = true;
        }

        private void LoseSki()
        {
            bool isLeftSkiOff = false;
            bool isRightSkiOff = false;
            if (playerRigidBody.velocity.x >= velocityToLoseSki)
            {
                //todo поменяй шанс, чтобы не всегда отваливались
                int result = UnityEngine.Random.Range(0, 9);

                //todo удали
                //result = 4;

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
            //прицепляем лыжи к ногам
            if (!isLeftSkiOff)
            {
                leftSki.GetComponent<CapsuleCollider>().material = ragdollRigidbody[0].GetComponent<BoxCollider>().material;
                leftSki.transform.SetParent(leftFoot.transform);
            }
            else
            {
                leftSki.transform.SetParent(forLeftSki, true);
                forLeftSki.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            }
            if (!isRightSkiOff)
            {
                rightSki.GetComponent<CapsuleCollider>().material = ragdollRigidbody[0].GetComponent<BoxCollider>().material;
                rightSki.transform.SetParent(rightFoot.transform);
            }
            else
            {
                rightSki.transform.SetParent(forRightSki, true);
                forRightSki.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            }
        }

        public void RestartToDefaultPosition()
        {
            _debugPanel.SetActive(true);

            joystick.gameObject.SetActive(true);
            playerRigidBody.velocity = Vector3.zero;
            playerRigidBody.angularVelocity = Vector3.zero;
            transform.SetPositionAndRotation(_restartPlayerTransformPosition, _restartPlayerTransformRotation);
            playerRigidBody.angularDrag = 18;
            playerRigidBody.constraints = RigidbodyConstraints.FreezeRotationZ;
            //rb.useGravity = false;
            isLose = false;
            //лыжи на место
            rightSki.transform.SetParent(transform);
            rightSki.GetComponent<CapsuleCollider>().material = skiMaterial;
            leftSki.transform.SetParent(transform);
            leftSki.GetComponent<CapsuleCollider>().material = skiMaterial;

            rightSki.layer = 10;
            leftSki.layer = 10;

            forLeftSki.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            forRightSki.gameObject.GetComponent<Rigidbody>().isKinematic = true;

            RagdollOff();
            for (int i = 0; i < bonesTransforms.Length; i++)
            {
                bonesTransforms[i].position = defaultBonesPositions[i];
                bonesTransforms[i].rotation = defaultBonesRotations[i];
            }
        }

        private IEnumerator Fall(Vector3 direction)
        {
            playerRigidBody.angularDrag = 0.06f;
            RagdollOn();
            for (int i = 0; i < 6; i++)
            {
                //torsoRb.AddTorque(direction * 14000, ForceMode.Force);
                yield return new WaitForFixedUpdate();
            }
        }

        private IEnumerator ShowLoseMenu()
        {
            yield return new WaitForSeconds(3);
            _debugPanel.SetActive(false);
            FindObjectOfType<MapController>().ShowLoseMenu(Convert.ToInt32(_currentMeters));
        }

        /// <summary>
        /// Обновить текст в <see cref="TMP_Text"/>, если он не <see cref="null"/>
        /// </summary>
        /// <param name="textMesh"></param>
        /// <param name="text"></param>
        private void PrintText(TMP_Text textMesh, object obj)
        {
            if (textMesh == null)
            {
                return;
            }

            textMesh.text = obj.ToString();
        }

        //private IEnumerator SlowMotionOnLose()
        //{
        //    Time.timeScale = 0.3f;
        //    yield return new WaitForSeconds(0.7f);
        //    Time.timeScale = 1f;
        //}
    }
}
