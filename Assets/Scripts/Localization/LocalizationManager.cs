using Assets.Scripts.Exceptions;
using Assets.Scripts.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Xml.Linq;

namespace Assets.Scripts
{
    public class LocalizationManager : MonoBehaviour
    {
        [Header("Параметры")]
        [Tooltip("Код локализации (ru-RU, en-Us, ...)")]
        [SerializeField] private string localizationCode;

        //[Tooltip("Player Prefs код локализации")]
        //[SerializeField] private string saveLocalizationCodeKey;

        [Tooltip("Доступные локализации")]
        [SerializeField] private List<Localization> localizations;

        [Tooltip("Загруженные строки для выбранной локализации")]
        [SerializeField] private List<String> strings;

        private void Awake()
        {
            Localize();
        }

        public void Localize()
        {
            //if (PlayerPrefs.HasKey(this.saveLocalizationCodeKey))
            //{
            //    this.localizationCode = PlayerPrefs.GetString(this.saveLocalizationCodeKey);
            //}

            if (!localizations.Exists(l => l.LocalicationCode == this.localizationCode))
            {
                throw new LocalizationException($"Локализация с кодом ${this.localizationCode} не найдена!");
            }

            Localization localisation = localizations.Find(l => l.LocalicationCode == this.localizationCode);

            string resourceName = $"Values/{localisation.StringsFileName}";
            TextAsset xml = Resources.Load<TextAsset>(resourceName);
            if (xml == null)
            {
                throw new LocalizationException($"Файл локализации \"Resources/{resourceName}\" не найден");
            }

            XDocument document = XDocument.Parse(xml.text);
            List<XElement> elements = document.Root.Elements("string").ToList();
            if (elements.Count == 0)
            {
                throw new LocalizationFileFormatException("В файле локализации не найдены <string> тэги. Пример: <string name=\"str_name\">String Value</string>");
            }
            strings.Clear();
            foreach (XElement element in elements)
            {
                if (element.Attribute("name") == null)
                {
                    throw new LocalizationFileFormatException("Неверный формат. Пример: <string name=\"str_name\">String Value</string>");
                }

                this.strings.Add(new String
                {
                    Name = element.Attribute("name").Value,
                    Value = element.Value
                });
            }
        }

        private void OnApplicationPause(bool pause)
        {
            //if (Application.platform == RuntimePlatform.Android)
            //{
            //    PlayerPrefs.SetString(this.saveLocalizationCodeKey, this.localizationCode);
            //}
        }

        private void OnApplicationQuit()
        {
            //PlayerPrefs.SetString(this.saveLocalizationCodeKey, this.localizationCode);
        }

        /// <summary>
        /// сменить локализацию
        /// </summary>
        /// <param name="code"></param>
        public void SetLocalization(string code)
        {
            this.localizationCode = code;
            Localize();
        }

        public string GetLocalization()
        {
            return this.localizationCode;
        }

        public bool IsHavingLocalication(string localizationCode)
        {
            return localizations.Any(l => l.LocalicationCode == localizationCode);
        }

        /// <summary>
        /// полачить локализированный текст по ключу
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            String str = strings.Find(s => s.Name == key);
            return str.Value;
        }
    }
}
