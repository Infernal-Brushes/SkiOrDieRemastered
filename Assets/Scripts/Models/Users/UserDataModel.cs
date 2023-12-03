using Assets.Scripts.Models.Characters;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

#if UNITY_WEBGL
using System.Runtime.InteropServices;
#endif

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
        public int MetersScoreDelimeter => 13 - CharacterKeys.Count;

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

            return true;
        }

        /// <inheritdoc/>
        public bool SelectCharacter(ICharacterModel character)
        {
            if (!CharacterKeys.Contains(character.Key))
            {
                return false;
            }

            SelectedCharacterKey = character.Key;

            return true;
        }

        /// <inheritdoc/>
        public bool TrySetBestMetersRecord(int meters)
        {
            if (BestMetersRecord < meters)
            {
                BestMetersRecord = meters;
#if UNITY_WEBGL
                SetLeaderboard();
#endif

                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public void SetLocalizationCode(string localizationCode)
        {
            LocalizationCode = localizationCode;
        }

        /// <inheritdoc/>
        public void Commit()
        {
            string json = JsonUtility.ToJson(this);
#if UNITY_WEBGL
            CommitToYandex(json);
#elif UNITY_STANDALONE_WIN
            using StreamWriter writer = new(_userDataPath);
            writer.Write(json);
#endif
        }

        /// <inheritdoc/>
        public void Fetch()
        {
#if UNITY_WEBGL
            FetchFromYandex();
#elif UNITY_STANDALONE_WIN
            if (!File.Exists(_userDataPath))
            {
                Commit();
                return;
            }

            using StreamReader reader = new(_userDataPath);
            string json = reader.ReadToEnd();
            SetDataFromJson(json);
            reader.Close();
#endif
        }

        public void SetDataFromJson(string json)
        {
            UserDataModel newData = JsonUtility.FromJson<UserDataModel>(json);

            Money = newData.Money;
            BestMetersRecord = newData.BestMetersRecord;
            CharacterKeys = newData.CharacterKeys;
            SelectedCharacterKey = newData.SelectedCharacterKey;
            LocalizationCode = newData.LocalizationCode;
        }

#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void CommitToYandex(string data);

        [DllImport("__Internal")]
        private static extern void FetchFromYandex();

        [DllImport("__Internal")]
        private static extern void SetLeaderboard();
#endif

        public void Reset()
        {
            Money = 0;
            BestMetersRecord = 0;
            CharacterKeys = new() { "fb7df1cf4762c4f98935c2b2e6bb8fb3" };

            Commit();
        }

        public object Clone()
        {
            return new UserDataModel()
            {
                Money = Money,
                BestMetersRecord = BestMetersRecord,
                CharacterKeys = CharacterKeys,
                SelectedCharacterKey = SelectedCharacterKey,
                LocalizationCode = LocalizationCode,
            };
        }
    }
}
