using Assets.Scripts.Models.Characters;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Models.Users
{
    /// <inheritdoc/>
    public sealed class UserData : IUserData
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

        private string _storePath;

        private string _persistentStorePath;

        private string _fileStoreName => "UserData.json";

        public UserData()
        {
            _storePath = $"{Application.dataPath}{Path.AltDirectorySeparatorChar}{_fileStoreName}";
            _persistentStorePath = $"{Application.persistentDataPath}{Path.AltDirectorySeparatorChar}{_fileStoreName}";
        }

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
        public void Commit()
        {
            string json = JsonUtility.ToJson(this);
            using StreamWriter writer = new(_storePath);
            writer.Write(json);
        }

        /// <inheritdoc/>
        public void Fetch()
        {
            if (!File.Exists(_storePath))
            {
                Commit();
                return;
            }

            using StreamReader reader = new(_storePath);
            string json = reader.ReadToEnd();

            JsonUtility.FromJsonOverwrite(json, this);
            Debug.Log($"Fetched data, money: {Money }");
        }

        public void Reset()
        {
            Money = 0;
            BestMetersRecord = 0;
            CharacterKeys = new() { "fb7df1cf4762c4f98935c2b2e6bb8fb3" };

            Commit();
        }
    }
}
