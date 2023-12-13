using Assets.Scripts.Models.Users;
using Assets.Scripts.Player;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Maps {
    public class MapController : MonoBehaviour
    {
        [SerializeField]
        private FollowingCamera _followingCamera;

        [Tooltip("Играбельные персонажи")]
        [SerializeField]
        private PlayerController[] _characterGameObjects;

        private Transform startPositionCamera;

        public GameObject loseMenu;
        public TextMeshProUGUI metersText;
        public TextMeshProUGUI metersBestText;
        public TextMeshProUGUI scoreForMetersText;
        public TextMeshProUGUI scoreForSpeedText;
        public TextMeshProUGUI scoreForRiskText;
        public TextMeshProUGUI totalTitleText;
        public TextMeshProUGUI totalText;

        [field: SerializeField]
        public PoolManager MeshPoolManager { get; private set; }

        [SerializeField]
        private PoolManager _treePoolManager;

        [SerializeField]
        private PoolManager _stumpPoolManager;

        private MeshGenerator _lastMeshGenerator;

        /// <summary>
        /// Флаг необходимости спавнить преграды
        /// <see cref="true"/> Спавнить преграды
        /// <see cref="false"/> Не спавнить преграды
        /// </summary>
        public bool SpawnBarriers { get; protected set; } = true;

        private UserDataController _userDataController;

        private void Start()
        {
            _userDataController = FindObjectOfType<UserDataController>();

            foreach (PlayerController character in _characterGameObjects)
            {
                if (_userDataController.UserDataModel.SelectedCharacterKey == character.CharacterModel.Key)
                {
                    character.gameObject.SetActive(true);
                    continue;
                }

                character.gameObject.SetActive(false);
            }

            loseMenu.SetActive(false);
            startPositionCamera = FindObjectOfType<Camera>().transform;
            GenerateFirstShape();
        }

        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.E))
            //{
            //    SpawnBarriers = !SpawnBarriers;
            //}

            if (Input.GetKeyDown(KeyCode.R))
            {
                Restart();
            }
        }

        private void GenerateFirstShape()
        {
            GameObject meshGeneratorGO = MeshPoolManager.GetFromPool();
            meshGeneratorGO.transform.position = transform.position;

            _lastMeshGenerator = meshGeneratorGO.GetComponent<MeshGenerator>();
            _lastMeshGenerator.CreateShape(spawnBarriers: SpawnBarriers);
        }

        /// <summary>
        /// Создать следующую поверхность
        /// </summary>
        public void GenerateNextShape(float zSize, Vector3[] vertices, int[] triangles)
        {
            Vector3 originalPosition = _lastMeshGenerator.transform.position;
            Vector3 direction = _lastMeshGenerator.transform.rotation * Vector3.forward;
            // Немного меньше чтобы спрятать шов
            Vector3 displacement = direction * (zSize - 0.082f);
            Vector3 newPosition = originalPosition + displacement;

            _lastMeshGenerator = MeshPoolManager.GetFromPool().GetComponent<MeshGenerator>();
            _lastMeshGenerator.transform.SetPositionAndRotation(newPosition, _lastMeshGenerator.transform.rotation);
            _lastMeshGenerator.CreateShape(SpawnBarriers, vertices, triangles);
        }

        public void Restart()
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            player.StopAllCoroutines();

            FindObjectOfType<InGameMenu>().pauseButton.SetActive(true);

            FindObjectsOfType<MeshGenerator>().ToList().ForEach(meshGenerator => meshGenerator.FreeBarriers());
            MeshPoolManager.ReturnAllObjects();
            _treePoolManager.ReturnAllObjects();
            _stumpPoolManager.ReturnAllObjects();
            GenerateFirstShape();

            player.RestartToDefaultPosition();

            Camera camera = FindObjectOfType<Camera>();
            camera.transform.SetPositionAndRotation(startPositionCamera.position, startPositionCamera.rotation);

            loseMenu.SetActive(false);
        }

        public void ShowLoseMenu(
            int meters,
            int bestMeters,
            int scoreForMeters,
            int scoreForSpeed,
            int scoreForRisk)
        {
            metersText.text = string.Format("Пройдено метров: {0}", meters);
            metersBestText.text = string.Format("Лучший результат: {0}", bestMeters);
            scoreForMetersText.text = string.Format("За расстояние: +{0}", scoreForMeters);
            scoreForSpeedText.text = string.Format("За скорость: +{0}", scoreForSpeed);
            scoreForRiskText.text = string.Format("За риск: +{0}", scoreForRisk);
            totalTitleText.text = string.Format("Итого: +{0}", scoreForMeters + scoreForSpeed + scoreForRisk);
            totalText.text = string.Format("Всего: {0}", _userDataController.UserDataModel.Money);

            loseMenu.SetActive(true);
        }
    }
}
