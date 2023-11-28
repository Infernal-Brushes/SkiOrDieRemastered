using Assets.Scripts.Wears;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Assets.Scripts
{

    public class User : MonoBehaviour
    {
        private string localizationCode;
        public string LocalizationCode
        {
            get
            {
                return localizationCode;
            }
            private set { }
        }

        public List<WearSerializable> UserWears
        {
            get
            {
                return userWears;
            }
        }
        private List<WearSerializable> userWears;

        public void SetLocalizationCode(string _localizationCode)
        {
            if (FindObjectOfType<LocalizationManager>().IsHavingLocalication(_localizationCode) &&
                LocalizationCode != _localizationCode)
            {
                Debug.Log("Настройки отличались");
                localizationCode = _localizationCode;
                
                SaveUser();
                FindObjectOfType<GameController>().ChangeLocalization(localizationCode);
            }   
        }

        private WearSerializable WearToWearSerializable(Wear wear)
        {
            WearSerializable wearSerializable = new WearSerializable();
            wearSerializable.titleKey = wear.titleKey;
            wearSerializable.targetSlot = (WearSerializable.Slot)wear.targetSlot;
            if (wear.targetSlot == Wear.Slot.torso)
                wear.prefab = FindObjectOfType<Prefabs>().torso.Find(w => w.titleKey == wear.titleKey).prefab;
            else if (wear.targetSlot == Wear.Slot.legs)
                wear.prefab = FindObjectOfType<Prefabs>().legs.Find(w => w.titleKey == wear.titleKey).prefab;
            else if (wear.targetSlot == Wear.Slot.boots)
                wear.prefab = FindObjectOfType<Prefabs>().boots.Find(w => w.titleKey == wear.titleKey).prefab;
            else if (wear.targetSlot == Wear.Slot.hat)
                wear.prefab = FindObjectOfType<Prefabs>().hats.Find(w => w.titleKey == wear.titleKey).prefab;
            else if (wear.targetSlot == Wear.Slot.wrists)
                wear.prefab = FindObjectOfType<Prefabs>().wrists.Find(w => w.titleKey == wear.titleKey).prefab;
            else if (wear.targetSlot == Wear.Slot.glasses)
                wear.prefab = FindObjectOfType<Prefabs>().glasses.Find(w => w.titleKey == wear.titleKey).prefab;
            else if (wear.targetSlot == Wear.Slot.mask)
                wear.prefab = FindObjectOfType<Prefabs>().masks.Find(w => w.titleKey == wear.titleKey).prefab;
            else if (wear.targetSlot == Wear.Slot.scarf)
                wear.prefab = FindObjectOfType<Prefabs>().scarf.Find(w => w.titleKey == wear.titleKey).prefab;

            if (wear.prefab == null)
            {
                Debug.LogError($"В Prefabs нет префаба с ключом {wear.titleKey} в слоте {wear.targetSlot}");
            }

            for (int i = 0; i < wear.slotRestricts.Count; i++)
            {
                wearSerializable.slotRestricts.Add((WearSerializable.Slot)wear.slotRestricts[i]);
            }

            var skinnedMeshRenderer = wear.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            if (wearSerializable.currentMaterialsIndexes == null)
                wearSerializable.currentMaterialsIndexes = new List<int>();
            for (int i = 0; i < skinnedMeshRenderer.materials.Length; i++)
            {
                wearSerializable.currentMaterialsIndexes.Add(wear.materialPresets[i].materirials.FindIndex(m => m.color == skinnedMeshRenderer.materials[i].color));
            }

            return wearSerializable;
        }

        internal void BuyWear(Wear wear)
        {
            //todo проверить что хватает денег
           


            userWears.Add(WearToWearSerializable(wear));
            SaveUser();
        }

        internal void UpdateWear(Wear wear)
        {
            int index = userWears.FindIndex(w => w.titleKey == wear.titleKey);
            if (index != -1)
            {
                userWears[index] = WearToWearSerializable(wear);
                SaveUser();
            }
        }

        internal void MockResetUser()
        {
            Debug.Log("User сброшен до дефолтных параметров");
            localizationCode = "ru-RU";
            userWears = new List<WearSerializable>();
            SaveUser();
        }

        public void SaveUser()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath
              + "/user_params.dat");
            UserSaved data = new UserSaved();

            data.savedLocalizationCode = LocalizationCode;
            data.userWears = userWears;

            bf.Serialize(file, data);
            file.Close();
            Debug.Log("User saved!");
        }

        public void LoadUser()
        {
            if (File.Exists(Application.persistentDataPath
              + "/user_params.dat"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file =
                  File.Open(Application.persistentDataPath
                  + "/user_params.dat", FileMode.Open);
                UserSaved data = (UserSaved)bf.Deserialize(file);
                file.Close();

                localizationCode = data.savedLocalizationCode;
                if (data.userWears != null)
                    userWears = data.userWears;
                else
                    userWears = new List<WearSerializable>();

                Debug.Log("User loaded!");
            }
            //первый запуск
            else
            {
                localizationCode = "ru-RU";
                userWears = new List<WearSerializable>();

                SaveUser();
                Debug.LogError("There is no save data! Created new one");
            }   
        }
    }

    [Serializable]
    class UserSaved
    {
        public string savedLocalizationCode;
        public List<WearSerializable> userWears;
    }
}
