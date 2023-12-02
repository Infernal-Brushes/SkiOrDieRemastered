using Assets.Scripts.Models.Users;
using UnityEngine;

namespace Assets.Scripts.Player
{
    /// <summary>
    /// Контроллер данных пользователя
    /// </summary>
    public class UserDataController : MonoBehaviour
    {
        private IUserDataModel _userDataModel;

        private void Awake()
        {
            _userDataModel = new UserDataModel();
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Задать данные пользователя из JSON
        /// </summary>
        /// <param name="json">JSON с данными пользователя</param>
        public void SetUserDataFromJson(string json) => _userDataModel.SetDataFromJson(json);
    }
}
