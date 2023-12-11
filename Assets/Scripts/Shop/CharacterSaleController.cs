using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Shop
{
    /// <summary>
    /// Контроллер персонажа в магазине
    /// </summary>
    public class CharacterSaleController : PlayerAppearance
    {
        [SerializeField]
        private Animator _animator;

        /// <summary>
        /// Позиция камеры
        /// </summary>
        private Transform cameraTransform;

        private void Awake()
        {
            OnAwake();
            cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            FaceCamera();
        }

        /// <summary>
        /// Повернуть лицом к камере
        /// </summary>
        private void FaceCamera()
        {
            Vector3 directionToCamera = cameraTransform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(new Vector3(directionToCamera.x, 0f, directionToCamera.z));
        }
    }
}
