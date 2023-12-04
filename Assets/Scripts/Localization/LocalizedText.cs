using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class LocalizedText : MonoBehaviour
    {
        [Header("Компоненты")]
        [Tooltip("Ссылка на LocalizationManager")]
        [SerializeField]
        private LocalizationManager localizationManager;

        [Header("Параметры")]
        [Tooltip("Имя строки в файле локализации")]
        [SerializeField]
        private string key;

        [Tooltip("Вставлять ли двоеточие после")]
        [SerializeField]
        private bool isColonAfter;

        /// <summary>
        /// Компонент текста для замены текста
        /// </summary>
        private TextMeshProUGUI _text;

        private void Awake()
        {
            //if (localizationManager == null)
            //{
            //    localizationManager = FindObjectOfType<LocalizationManager>();
            //    //Debug.Log("{GameLog} => [] (<color=yellow>Warning</color>) - set link to LocalizationManager before starting scene", this.gameObject);
            //}

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
            localizationManager = FindObjectOfType<LocalizationManager>();
            string str = localizationManager.GetValue(key);

            _text = GetComponent<TextMeshProUGUI>();
            _text.text = str + (isColonAfter ? ":" : "");
        }
    }
}
