using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class LocalizedText : MonoBehaviour
    {
        [Header("Компоненты")]
        [Tooltip("Ссылка на LocalizationManager")]
        [SerializeField] private LocalizationManager localizationManager;

        [Header("Параметры")]
        [Tooltip("Имя строки в файле локализации")]
        [SerializeField] private string key;
        [Tooltip("TMP_Text для замены текста")]
        [SerializeField] private TMP_Text text;
        [Tooltip("Вставлять ли двоеточие после")]
        [SerializeField] private bool isColonAfter;

        private void Awake()
        {
            if (localizationManager == null)
            {
                localizationManager = FindObjectOfType<LocalizationManager>();
                Debug.Log("{GameLog} => [] (<color=yellow>Warning</color>) - set link to LocalizationManager before starting scene", this.gameObject);
            }
        }

        private void Start()
        {
            Refresh();
        }

        /// <summary>
        /// обновить текст когда сменился язык
        /// </summary>
        public void Refresh()
        {
            string str = this.localizationManager.GetValue(key);

            text.text = str + (isColonAfter ? ":" : "");
        }
    }
}
