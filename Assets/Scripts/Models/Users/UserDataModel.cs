using Assets.Scripts.Models.Characters;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Assets.Scripts.Models.Users
{
    /// <inheritdoc/>
    public sealed class UserDataModel : IUserDataModel
    {
        private const string DefaultCharacterKey = "fb7df1cf4762c4f98935c2b2e6bb8fb3";

        /// <inheritdoc/>
        [field: SerializeField]
        public int Money { get; private set; }

        /// <inheritdoc/>
        [field: SerializeField]
        public int BestMetersRecord { get; private set; }

        /// <inheritdoc/>
        [field: SerializeField]
        public List<string> CharacterKeys { get; private set; } = new () { DefaultCharacterKey };

        /// <inheritdoc/>
        [field: SerializeField]
        public string SelectedCharacterKey { get; private set; } = DefaultCharacterKey;

        /// <inheritdoc/>
        [field: SerializeField]
        public string LocalizationCode { get; private set; } = "ru-RU";

        private string _userDataPath => $"{Application.persistentDataPath}{Path.AltDirectorySeparatorChar}{_fileStoreName}";

        private string _fileStoreName => "UserData.json";

        /// <inheritdoc/>
        public bool IsCharacterSelected(ICharacterModel character) => SelectedCharacterKey == character.Key;

        /// <inheritdoc/>
        public bool IsCharacterOwned(ICharacterModel character) => CharacterKeys.Contains(character.Key);

        /// <inheritdoc/>
        public void EarnMoney(int money)
        {
            if (money < 0)
            {
                return;
            }

            Money += money;
            Commit();
        }

        /// <inheritdoc/>
        public bool BuyCharacter(ICharacterModel character)
        {
            if (CharacterKeys.Contains(character.Key))
            {
                return false;
            }

            if (Money < character.Price)
            {
                return false;
            }

            Money -= character.Price;
            CharacterKeys.Add(character.Key);
            Commit();

            Debug.Log($"Куплен {character.Name}");

            return true;
        }

        /// <inheritdoc/>
        public bool SelectCharacter(ICharacterModel character)
        {
            if (!CharacterKeys.Contains(character.Key))
            {
                Debug.Log("Персонаж не куплен");
                return false;
            }

            SelectedCharacterKey = character.Key;
            Commit();

            Debug.Log($"Выбран персонаж {character.Name}");

            return true;
        }

        /// <inheritdoc/>
        public bool TrySetBestMetersRecord(int meters)
        {
            if (BestMetersRecord < meters)
            {
                BestMetersRecord = meters;
                Commit();

                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public void SetLocalizationCode(string localizationCode)
        {
            LocalizationCode = localizationCode;
            Commit();
        }

        /// <inheritdoc/>
        public void Commit()
        {
            string json = JsonUtility.ToJson(this);
#if UNITY_WEBGL
            CommitToYandex(json);
#else
            using StreamWriter writer = new(_userDataPath);
            writer.Write(json);
#endif
        }

        /// <inheritdoc/>
        public void Fetch()
        {
#if UNITY_WEBGL
            FetchFromYandex();
#else 
            Debug.Log(_userDataPath);
            if (!File.Exists(_userDataPath))
            {
                Commit();
                return;
            }

            using StreamReader reader = new(_userDataPath);
            string json = reader.ReadToEnd();

            JsonUtility.FromJsonOverwrite(json, this);
            Debug.Log($"Fetched data, money: {Money }");
#endif
        }

        [DllImport("__Internal")]
        private static extern void CommitToYandex(string data);

        [DllImport("__Internal")]
        private static extern void FetchFromYandex();

        public void Reset()
        {
            Money = 0;
            BestMetersRecord = 0;
            CharacterKeys = new() { "fb7df1cf4762c4f98935c2b2e6bb8fb3" };

            Commit();
        }
    }
}
