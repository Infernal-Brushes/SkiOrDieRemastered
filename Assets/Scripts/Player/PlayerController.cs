using System;
using System.Collections;
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

        /// <summary>
        /// Объект проверки контакта с землёй
        /// </summary>
        [InspectorName("Объект проверки контакта с землёй")]
        public GameObject groundPoint;

        /// <summary>
        /// Длина луча для проверки контакта с землёй
        /// </summary>
        [InspectorName("Длина луча для проверки контакта с землёй")]
        public float groundCheckLength = 0.5f;
        /// <summary>
        /// Скорость наклона вперёд назад
        /// </summary>
        [InspectorName("Скорость наклона вперёд назад")]
        public float speedOfTiltX = 0.3f;
        /// <summary>
        /// Скорость поворота
        /// </summary>
        [InspectorName("Скорость поворота")]
        public float speedOfNormalRotationY = 0.3f;
        /// <summary>
        /// Скорость поворота в стрейфе
        /// </summary>
        [InspectorName("Скорость поворота в стрейфе")]
        public float speedOfStrafeRotationY = 1.3f;
        /// <summary>
        /// Коэфициент набора скорости лыж от их поворота относительно склона
        /// </summary>
        [InspectorName("Коэфициент набора скорости лыж от их поворота относительно склона")]
        public float freeRideVelocity = 0.018f;
        /// <summary>
        /// Боковая скорость при стрейфе в крайнем положении
        /// </summary>
        [InspectorName("Боковая скорость при стрейфе в крайнем положении")]
        public float velocityStrafe = 0.6f;
        /// <summary>
        /// Боковая скорость при повороте
        /// </summary>
        [InspectorName("Боковая скорость при повороте")]
        public float velocityOnTurning = 0.3f;

        /// <summary>
        /// Скорость торможения при стрейфе
        /// </summary>
        [InspectorName("Скорость торможения при стрейфе")]
        public float strafeStopperSpeed = 0.8f;
        /// <summary>
        /// Скорость, ниже которой не может затормозить стрейф
        /// </summary>
        [InspectorName("Скорость, ниже которой не может затормозить стрейф")]
        public float strafeSpeedLimit = 15f;
        public float deathSpeedX = 35;
        public float deathSpeedZ = 28;

        public float angleOfTurn = 50f;
        public float angleOfStrafe = 65f;

        public float velocityToLoseSki = 28;

        /// <summary>
        /// true если персонаж касается земли
        /// </summary>
        bool isGrounded = true;

        /// <summary>
        /// true если в стрэйфе
        /// </summary>
        bool isInStrafe = false;

        //true если проиграл
        //[HideInInspector]
        public bool isLose = false;

        private float startMeters;

        [HideInInspector]
        public Rigidbody rigidBody;

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
            rigidBody = GetComponent<Rigidbody>();
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

            startMeters = transform.position.x;

            //downTheHill = Vector3.right;

            //слоумо
            //Time.timeScale = 0.6f;
        }

        private void FixedUpdate()
        {
            if (isLose)
                return;

            Debug.DrawRay(groundPoint.transform.position, groundPoint.transform.up, Color.red, 12);

            if (Input.GetKey(KeyCode.V))
            {
                RagdollOn();
            }
            //при нажатии любой кнопки включается гравитация
            //else if (!rb.useGravity && Input.anyKey)
            //{
            //    rb.useGravity = true;
            //}

            if (rigidBody.velocity.x >= deathSpeedX)
            {
                Lose(LoseCause.fallX);
            }

            // Выровнять игрока относительно земли, применить гравитацию
            FlatToGround();

            // Катить лыжи прямо сами по себе
            RideForward();

            // Повороты лыж
            RotateBody();
        }

        private void OnCollisionEnter(Collision collision)
        {
            //Debug.Log("Player collision");
            if (collision.collider.gameObject.TryGetComponent(out Barrier barrier) && !isLose)
            {
                //Debug.Log("Barrier OnCollisionEnter");
                Lose(LoseCause.barrier);
            }
        }

        /// <summary>
        /// Выровнять игрока относительно земли
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
                    rigidBody.AddTorque(speedOfTiltX * turningCoefficient * Vector3.Cross(transform.forward, hit.normal), ForceMode.Impulse);
                }
                return;
            }

            isGrounded = false;

            // для улучшения гравитациии
            rigidBody.AddForce(Vector3.down * 1f, ForceMode.Impulse);
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

            float angle = Vector3.Angle(_skiesDirection, Vector3.forward);  
            float forceOfRide = Mathf.Abs(90f - angle) * freeRideVelocity;
            //Debug.Log($"angle: {angle}; forceOfRide: {forceOfRide}");
            rigidBody.AddForce(_skiesDirection * forceOfRide, ForceMode.Impulse);
        }

        private void RotateBody()
        {
            float angleY = transform.rotation.eulerAngles.y;
            //Debug.Log(angleY);

            float axisX = joystick.Horizontal;
            if (Input.GetAxis("Horizontal") != 0)
                axisX = Input.GetAxis("Horizontal");

            // повороты по земле
            if (!isGrounded)
            {
                // повороты в воздухе
                if (axisX < 0 && angleY > 90 - angleOfStrafe)
                {
                    rigidBody.AddTorque(transform.up * speedOfStrafeRotationY * 0.33f * axisX, ForceMode.VelocityChange);
                }
                else if (axisX > 0 && angleY < 90 + angleOfStrafe)
                {
                    rigidBody.AddTorque(transform.up * speedOfStrafeRotationY * 0.33f * axisX, ForceMode.VelocityChange);
                }

                return;
            }

            if (axisX == 0)
            {
                isInStrafe = false;
                //восстанавливаем положение лыж
                if (angleY > 90 + angleOfTurn)
                {
                    rigidBody.AddTorque(transform.up * -speedOfStrafeRotationY, ForceMode.VelocityChange);
                }
                else if (angleY < 90 - angleOfTurn)
                {
                    rigidBody.AddTorque(transform.up * speedOfStrafeRotationY, ForceMode.VelocityChange);
                }
                return;
            }
            // налево повернуть корпус
            else if (axisX < 0)
            {
                //сначала поворачиваемся до нужного угла
                if (angleY > 90 - angleOfTurn)
                {
                    isInStrafe = false;
                    rigidBody.AddTorque(axisX * speedOfNormalRotationY * transform.up, ForceMode.VelocityChange);
                    //ApplynTurning();

                    //float velocity = angleY > 90 ? 0.08f : velocityOnTurning;
                    //rigidBody.AddForce(vectorProjection.x * velocity * -transform.right, ForceMode.VelocityChange);
                    //RideForward();
                }
                //затем когда дошли до угла максимального то держим скорость в пределе (входим в стрэйф)
                else
                {
                    isInStrafe = true;
                    if (angleY > 90 - angleOfStrafe)
                    {
                        rigidBody.AddTorque(transform.up * speedOfStrafeRotationY * axisX, ForceMode.VelocityChange);
                    }

                    //if (rigidBody.velocity.z <= -deathSpeedZ)
                    //{
                    //    Lose(LoseCause.fallZ);
                    //}

                    if (rigidBody.velocity.x > strafeSpeedLimit)
                    {
                        //сила назад
                        rigidBody.AddForce(Vector3.left * strafeStopperSpeed, ForceMode.Impulse);
                        //сила в бок
                        rigidBody.AddForce(transform.right * velocityStrafe * -axisX, ForceMode.Impulse);
                    }
                    else
                    {
                        rigidBody.velocity = Vector3.zero;
                    }
                }
            }
            //направо повернуть корпус
            else if (axisX > 0)
            {
                //сначала поворачиваемся до нужного угла
                if (angleY < 90 + angleOfTurn)
                {
                    isInStrafe = false;
                    rigidBody.AddTorque(axisX * speedOfNormalRotationY * transform.up, ForceMode.VelocityChange);
                    //ApplyVelocityOnTurning();

                    //float velocity = angleY < 90 ? 0.08f : velocityOnTurning;
                    //rigidBody.AddForce(vectorProjection.x * velocity * transform.right, ForceMode.VelocityChange);
                    //RideForward();
                }
                //затем когда дошли до угла максимального то держим скорость в пределе (входим в стрэйф)
                else
                {
                    isInStrafe = true;
                    if (angleY < 90 + angleOfStrafe)
                    {
                        rigidBody.AddTorque(transform.up * speedOfStrafeRotationY * axisX, ForceMode.VelocityChange);
                    }

                    //if (rigidBody.velocity.z >= deathSpeedZ)
                    //{
                    //    Lose(LoseCause.fallZ);
                    //}

                    if (rigidBody.velocity.x > strafeSpeedLimit)
                    {
                        //сила назад
                        rigidBody.AddForce(Vector3.left * strafeStopperSpeed, ForceMode.Impulse);
                        //сила в бок
                        rigidBody.AddForce(transform.right * velocityStrafe * -axisX, ForceMode.Impulse);
                    }
                    else
                    {
                        rigidBody.velocity = Vector3.zero;
                    }
                }
            }
        }

        private void ApplyVelocityOnTurning()
        {
            float coefficient = Vector3.Project(_skiesDirection, Vector3.right).x;
            Debug.Log(coefficient * velocityOnTurning);
            rigidBody.AddForce(coefficient * velocityOnTurning * Vector3.right, ForceMode.Impulse);
        }

        private void RagdollOn()
        {
            for (int i = 0; i < ragdollRigidbody.Length; i++)
            {
                ragdollRigidbody[i].isKinematic = false;
                ragdollRigidbody[i].velocity = rigidBody.velocity;
                ragdollRigidbody[i].angularVelocity = rigidBody.angularVelocity;
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

            rigidBody.constraints = RigidbodyConstraints.None;
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


                if (rigidBody.velocity.z > 0)
                    StartCoroutine(Fall(transform.forward));
                else
                    StartCoroutine(Fall(-transform.forward));
            }
            else if (cause == LoseCause.barrier)
            {
                //StartCoroutine(SlowMotionOnLose());
                LoseSki();

                RagdollOn();
                rigidBody.angularDrag = 0.06f;
                //Debug.Log("Проигрыш! Врезался в препятствие");
            }

            StartCoroutine(ShowLoseMenu());
            isLose = true;
        }

        private void LoseSki()
        {
            bool isLeftSkiOff = false;
            bool isRightSkiOff = false;
            if (rigidBody.velocity.x >= velocityToLoseSki)
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
            Time.timeScale = 1f;
            joystick.gameObject.SetActive(true);
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
            transform.SetPositionAndRotation(_restartPlayerTransformPosition, _restartPlayerTransformRotation);
            rigidBody.angularDrag = 18;
            rigidBody.constraints = RigidbodyConstraints.FreezeRotationZ;
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
            rigidBody.angularDrag = 0.06f;
            RagdollOn();
            for (int i = 0; i < 6; i++)
            {
                //torsoRb.AddTorque(direction * 14000, ForceMode.Force);
                yield return new WaitForFixedUpdate();
            }
        }

        private IEnumerator ShowLoseMenu()
        {
            float meters = transform.position.x - startMeters;
            yield return new WaitForSeconds(3);
            FindObjectOfType<MapController>().ShowLoseMenu(Convert.ToInt32(meters));
        }

        //private IEnumerator SlowMotionOnLose()
        //{
        //    Time.timeScale = 0.3f;
        //    yield return new WaitForSeconds(0.7f);
        //    Time.timeScale = 1f;
        //}
    }
}
