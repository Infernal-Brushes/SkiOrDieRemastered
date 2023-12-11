using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI
{
    public class TooltipController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Tooltip("Элемент тултипа")]
        [SerializeField]
        protected GameObject _tooltip;

        [Tooltip("Задержка отображения в секундах")]
        [SerializeField]
        private float _delay = 0.2f;

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            ShowTooltip();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HideTooltip();
        }

        /// <summary>
        /// Отобразить тултип
        /// </summary>
        public void ShowTooltip()
        {
            StartCoroutine(Show());
        }

        /// <summary>
        /// Скрыть тултип
        /// </summary>
        public void HideTooltip()
        {
            if (_tooltip == null)
            {
                return;
            }

            StopAllCoroutines();
            _tooltip.SetActive(false);
        }

        private IEnumerator Show()
        {
            yield return new WaitForSecondsRealtime(_delay);

            _tooltip.SetActive(true);
        }
    }
}
