using UnityEngine;

namespace Assets.Scripts.Shop
{
    /// <summary>
    /// Контроллер позиций цветов одежды персонажа
    /// </summary>
    public class CharacterWearColorsController : MonoBehaviour
    {
        [field: SerializeField]
        public string CharacterKey { get; private set; }
    }
}
