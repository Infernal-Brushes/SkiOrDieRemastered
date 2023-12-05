using UnityEngine;

namespace Assets.Extensions
{
    /// <summary>
    /// Расширения для <see cref="Animator"/>
    /// </summary>
    public static class AnimatorExtensions
    {
        /// <summary>
        /// Сбросить состояние аниматора в entry
        /// </summary>
        /// <param name="animator">Инстанс аниматора</param>
        public static void ResetToEntryState(this Animator animator)
        {
            animator.Rebind();
            animator.Update(0f);
        } 
    }
}