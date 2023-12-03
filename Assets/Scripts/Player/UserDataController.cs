using Assets.Scripts.Models.Users;
using UnityEngine;

namespace Assets.Scripts.Player
{
    /// <summary>
    /// Контроллер для доступа к данным пользователя
    /// </summary>
    public sealed class UserDataController : MonoBehaviour
    {
        private static UserDataController _instance = null;

        /// <summary>
        /// Модель данных игрока
        /// </summary>
        public IUserDataModel UserDataModel { get; private set; }

        private void Awake()
        {
            if (_instance == this)
            {
                return;
            }

            if (_instance != null)
            {
                Destroy(_instance.gameObject);
            }

            _instance = this;
            UserDataModel = new UserDataModel();
            UserDataModel.Fetch();

            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Сбросить прогресс
        /// </summary>
        public void ResetProgress()
        {
            UserDataModel = new UserDataModel();
            UserDataModel.Commit();
        }

        /// <summary>
        /// Задать данные пользователя из JSON
        /// </summary>
        /// <param name="json">JSON с данными пользователя</param>
        public void SetUserDataFromJson(string json) => UserDataModel.SetDataFromJson(json);
    }
}
