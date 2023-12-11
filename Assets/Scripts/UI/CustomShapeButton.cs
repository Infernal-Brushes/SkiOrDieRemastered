using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Скрипт для кнопки чтобы нажималась по своей форме
    /// </summary>
    public class CustomShapeButton : MonoBehaviour
    {
        [SerializeField]
        private float _alphaHitTestMinimumThreshold = 1f;

        private void Start()
        {
            GetComponent<Image>().alphaHitTestMinimumThreshold = _alphaHitTestMinimumThreshold;
        }
    }
}
