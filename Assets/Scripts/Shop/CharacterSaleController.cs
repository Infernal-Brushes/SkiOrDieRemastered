﻿using Assets.Extensions;
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
        [field: SerializeField]
        public SerializableInterface<ICharacterModel> CharacterModel { get; private set; }

        /// <summary>
        /// Позиция камеры
        /// </summary>
        private Transform cameraTransform;

        private void Start()
        {
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
