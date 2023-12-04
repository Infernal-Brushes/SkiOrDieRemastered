using UnityEngine;

namespace Assets.Scripts.Menu
{
    /// <summary>
    /// Контроллер главного меню
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField]
        private GameObject _howToPlayPanel;

        public void ShowHowToPlayPanel()
        {
            _howToPlayPanel.SetActive(true);
        }

        public void HideHowToPlayPanel()
        {
            _howToPlayPanel.SetActive(false);
        }
    }
}
