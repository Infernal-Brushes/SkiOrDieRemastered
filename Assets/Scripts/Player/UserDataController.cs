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
            //if (_instance == null)
            //{
            //    _instance = this;
            //    UserDataModel = new UserDataModel();
            //    UserDataModel.Fetch();

            //    DontDestroyOnLoad(gameObject);

            //    return;
            //}

            //if (_instance.UserDataModel != null)
            //{
            //    UserDataModel = (UserDataModel)_instance.UserDataModel.Clone();
            //    _instance = this;

            //    return;
            //}

            try
            {
                UserDataModel = (UserDataModel)_instance.UserDataModel.Clone();
                _instance = this;
            }
            catch
            {
                _instance = this;
                UserDataModel = new UserDataModel();
                UserDataModel.Fetch();

                DontDestroyOnLoad(gameObject);
            }
        }

        /// <summary>
        /// Сбросить прогресс
        /// </summary>
        public void ResetProgress()
        {
            var bestMeters = UserDataModel.BestMetersRecord;
            UserDataModel = new UserDataModel();
            UserDataModel.TrySetBestMetersRecord(bestMeters);
            UserDataModel.Commit();
        }

        /// <summary>
        /// Задать данные пользователя из JSON
        /// </summary>
        /// <param name="json">JSON с данными пользователя</param>
        public void SetUserDataFromJson(string json) => UserDataModel.SetDataFromJson(json);
    }
}
