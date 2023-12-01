using Assets.Scripts.Models.Users;
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

        /// <summary>
        /// Флаг необходимости спавнить преграды
        /// <see cref="true"/> Спавнить преграды
        /// <see cref="false"/> Не спавнить преграды
        /// </summary>
        public bool SpawnBarriers { get; protected set; } = true;

        private IUserDataModel _userData;

        private void Awake()
        {
            _userData = new UserDataModel();
            _userData.Fetch();

            foreach (PlayerController character in _characterGameObjects)
            {
                if (_userData.SelectedCharacterKey == character.CharacterModel.Value.Key)
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
            if (Input.GetKeyDown(KeyCode.E))
            {
                SpawnBarriers = !SpawnBarriers;
            }

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

        public void ShowLoseMenu(int meters)
        {
            _userData.Fetch();

            metersText.text = string.Format("Пройдено метров: {0}", meters);
            metersBestText.text = string.Format("Лучший результат: {0}", _userData.BestMetersRecord);
            loseMenu.SetActive(true);
        }
    }
}
