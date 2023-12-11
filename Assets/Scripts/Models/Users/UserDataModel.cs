using Assets.Scripts.Models.Characters;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using Assets.Scripts.Models.Characters.WearColors;
using System.Linq;

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
        public List<string> WearColorKeysOwned { get; private set; } = new();

        /// <inheritdoc/>
        [field: SerializeField]
        public List<string> WearColorKeysSelected { get; private set; } = new();

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
        public bool IsColorOwned(string colorKey) => WearColorKeysOwned.Contains(colorKey);

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
            SelectCharacter(character);

            return true;
        }

        /// <inheritdoc/>
        public bool BuyColor(IWearColorModel wearColor)
        {
            if (WearColorKeysOwned.Contains(wearColor.Key))
            {
                return false;
            }

            if (Money <  wearColor.Price)
            {
                return false;
            }

            Money -= wearColor.Price;
            WearColorKeysOwned.Add(wearColor.Key);

            return true;
        }

        public void SelectColor(IWearColorModel wearColor, ICharacterModel character)
        {
            IEnumerable<IWearColorModel> characterColorsForSameBodyPart = character.BodyPartColors
                .Where(characterColor => characterColor.MaterialColors
                    .All(characterMaterialColor => wearColor.MaterialColors
                        .Any(wearMaterialColor => wearMaterialColor.MaterialIndex == characterMaterialColor.MaterialIndex)));

            IEnumerable<IWearColorModel> characterColorsForSameSkiPart = character.SkiColors
               .Where(characterColor => characterColor.MaterialColors
                    .All(characterMaterialColor => wearColor.MaterialColors
                        .Any(wearMaterialColor => wearMaterialColor.MaterialIndex == characterMaterialColor.MaterialIndex)));

            var t1 = characterColorsForSameBodyPart.ToList();
            var t2 = characterColorsForSameSkiPart.ToList();

            IEnumerable<string> wearColorsToUnselect = characterColorsForSameBodyPart
                .Union(characterColorsForSameSkiPart)
                .Select(color => color.Key);
            WearColorKeysSelected.RemoveAll(color => wearColorsToUnselect.Contains(color));
            WearColorKeysSelected.Add(wearColor.Key);

            Commit();
        }

        /// <inheritdoc/>
        public bool SelectCharacter(ICharacterModel character)
        {
            if (!CharacterKeys.Contains(character.Key))
            {
                return false;
            }

            SelectedCharacterKey = character.Key;
            Commit();

            return true;
        }

        /// <inheritdoc/>
        public void EarnMoneyAndTrySetBestMetersRecord(int meters, int money)
        {
            if (BestMetersRecord < meters)
            {
                BestMetersRecord = meters;
#if UNITY_WEBGL
                SetLeaderboard();
#endif
            }

            Money += money;
            Commit();
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
            Debug.Log($"{nameof(UserDataModel)}.{nameof(SetDataFromJson)} call. Json: {json}");
            UserDataModel newData = JsonUtility.FromJson<UserDataModel>(json);

            Money = newData.Money;
            BestMetersRecord = newData.BestMetersRecord;
            CharacterKeys = newData.CharacterKeys;
            SelectedCharacterKey = newData.SelectedCharacterKey;
            LocalizationCode = newData.LocalizationCode;
            WearColorKeysOwned = newData.WearColorKeysOwned;
            WearColorKeysSelected = newData.WearColorKeysSelected;
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
                WearColorKeysOwned = WearColorKeysOwned,
                WearColorKeysSelected = WearColorKeysSelected,
            };
        }
    }
}
