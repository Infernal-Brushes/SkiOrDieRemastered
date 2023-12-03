using Assets.Scripts.Models.Users;
using Assets.Scripts.Player;
using TMPro;
using UnityEngine;

namespace Assets.Scripts {
    public class MapController : MonoBehaviour
    {
        [SerializeField]
        private FollowingCamera _followingCamera;

        [Tooltip("Играбельные персонажи")]
        [SerializeField]
        private PlayerController[] _characterGameObjects;

        public MeshGenerator meshGeneratorPrefab;

        private Transform startPositionCamera;
        private Transform startPositionMeshGeneratror;

        public GameObject loseMenu;
        public TextMeshProUGUI metersText;
        public TextMeshProUGUI metersBestText;
        public TextMeshProUGUI scoreForMetersText;
        public TextMeshProUGUI scoreForSpeedText;
        public TextMeshProUGUI scoreForRiskText;
        public TextMeshProUGUI totalTitleText;
        public TextMeshProUGUI totalText;

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
                if (_userDataController.UserDataModel.SelectedCharacterKey == character.CharacterModel.Value.Key)
                {
                    character.gameObject.SetActive(true);
                    _followingCamera.SetObjectToFollow(character.ObjectForCameraFollowing);
                    continue;
                }

                character.gameObject.SetActive(false);
            }

            loseMenu.SetActive(false);
            startPositionCamera = FindObjectOfType<Camera>().transform;
            startPositionMeshGeneratror = meshGeneratorPrefab.transform;
            CreateFirstShape();
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

        private void CreateFirstShape()
        {
            var meshGenerator = Instantiate(meshGeneratorPrefab, startPositionMeshGeneratror.position, startPositionMeshGeneratror.rotation);
            meshGenerator.CreateShape(spawnBarriers: SpawnBarriers);
        }

        public void Restart()
        {
            FindObjectOfType<PlayerController>().StopAllCoroutines();

            FindObjectOfType<InGameMenu>().pauseButton.SetActive(true);

            var meshes = FindObjectsOfType<MeshGenerator>();
            for (int i = 0; i < meshes.Length; i++)
            {
                Destroy(meshes[i].gameObject);
            }

            CreateFirstShape();


            FindObjectOfType<PlayerController>().RestartToDefaultPosition();

            Camera camera = FindObjectOfType<Camera>();
            camera.transform.position = startPositionCamera.position;
            camera.transform.rotation = startPositionCamera.rotation;



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
