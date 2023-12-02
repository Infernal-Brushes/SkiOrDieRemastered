using Assets.Scripts.Models.Users;
using UnityEngine;

namespace Assets.Scripts.Player
{
    /// <summary>
    /// Контроллер для доступа к данным пользователя
    /// </summary>
    public sealed class UserDataController : MonoBehaviour
    {
        /// <summary>
        /// Модель данных игрока
        /// </summary>
        public IUserDataModel UserDataModel { get; private set; }

        private void Awake()
        {
            UserDataModel = new UserDataModel();
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Задать данные пользователя из JSON
        /// </summary>
        /// <param name="json">JSON с данными пользователя</param>
        public void SetUserDataFromJson(string json) => UserDataModel.SetDataFromJson(json);
    }
}
