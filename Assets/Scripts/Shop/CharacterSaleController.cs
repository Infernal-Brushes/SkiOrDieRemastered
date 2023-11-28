using Assets.Extensions;
using Assets.Scripts.Models.Characters;
using TNRD;
using UnityEngine;

namespace Assets.Scripts.Shop
{
    /// <summary>
    /// Контроллер персонажа в магазине
    /// </summary>
    public class CharacterSaleController : MonoBehaviour
    {
        [SerializeField]
        private Animator _animator;

        [Tooltip("Модель персонажа")]
        public SerializableInterface<ICharacterModel> CharacterModel { get; private set; }

        private Transform cameraTransform;

        private void Start()
        {
            cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            FaceCamera();
        }

        private void FaceCamera()
        {
            Vector3 directionToCamera = cameraTransform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(new Vector3(directionToCamera.x, 0f, directionToCamera.z));
        }

        public void StartIdleAnimation()
        {
            _animator.enabled = true;
        }

        public void StopAnimation()
        {
            _animator.ResetToEntryState();
            _animator.enabled = false;
        }
    }
}
