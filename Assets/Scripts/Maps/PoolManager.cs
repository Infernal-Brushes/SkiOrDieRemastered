using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Maps
{
    public class PoolManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _prefab;

        [Min(2)]
        [SerializeField]
        private int _poolInitialSize = 2;

        [Tooltip("Максимальное количество активных инстансов")]
        [SerializeField]
        private int _maxInstancesCount = 0;

        /// <summary>
        /// Пул объектов
        /// </summary>
        private List<GameObject> _pool = new();

        /// <summary>
        /// Первый неактивный элемент пула
        /// </summary>
        GameObject FirstInactive => _pool.FirstOrDefault(obj => !obj.activeInHierarchy);

        /// <summary>
        /// Первый активный элемент пула
        /// </summary>
        GameObject FirstActive => _pool.FirstOrDefault(obj => obj.activeInHierarchy);

        private void Awake()
        {
            int index = 0;
            PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(_prefab);
            if (prefabType == PrefabAssetType.NotAPrefab)
            {
                index = 1;
                _prefab.SetActive(false);
                _prefab.transform.SetParent(transform);

                _pool.Add(_prefab);
            }

            for (; index < _poolInitialSize; index++)
            {
                GameObject obj = Instantiate(_prefab, transform);
                obj.SetActive(false);
                _pool.Add(obj);
            }
        }

        public GameObject GetFromPool()
        {
            if (FirstInactive != null)
            {
                FirstInactive.SetActive(true);
                return PopFirstActive();
            }

            if (_maxInstancesCount > 0)
            {
                if (_pool.Where(obj => obj.activeInHierarchy).Count() <= _maxInstancesCount)
                {
                    return PopFirstActive();
                }
            }

            return PopNewActive();
        }

        public void ReturnObject(GameObject obj)
        {
            if (_maxInstancesCount > 0)
            {
                var activeInstances = _pool.Count(obj => obj.activeInHierarchy);
                if (activeInstances <= _maxInstancesCount)
                {
                    return;
                }
            }

            obj.SetActive(false);
            obj.transform.SetParent(transform);
        }

        public void ReturnAllObjects()
        {
            _pool.ForEach(obj =>
            {
                obj.SetActive(false);
                obj.transform.SetParent(transform);
            });
        }

        public void ReturnObjectForce(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(transform);
        }

        /// <summary>
        /// Протолкнуть первый активный в конец пула, чтобы он вышел последним
        /// </summary>
        /// <returns>Первый активный элемент пула</returns>
        private GameObject PopFirstActive()
        {
            GameObject firstActive = FirstActive;
            _pool.Remove(firstActive);
            _pool.Add(firstActive);

            return firstActive;
        }

        /// <summary>
        /// Инстантиировать новый объект в пуле
        /// </summary>
        /// <returns>Новый активный объект в пуле</returns>
        private GameObject PopNewActive()
        {
            GameObject newObj = Instantiate(_prefab);
            _pool.Add(newObj);
            newObj.SetActive(true);

            return newObj;
        }
    }
}
