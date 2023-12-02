using Assets.Scripts.Models.Users;
using UnityEngine;

namespace Assets.Scripts.Player
{
    /// <summary>
    /// Контроллер данных пользователя
    /// </summary>
    public class UserDataController : MonoBehaviour
    {
        private readonly IUserDataModel _userDataModel;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Задать данные пользователя из JSON
        /// </summary>
        /// <param name="json">JSON с данными пользователя</param>
        public void SetUserDataFromJson(string json) => _userDataModel.SetDataFromJson(json);
    }
}
