using UnityEngine;

namespace Project.Episode2
{
    /// <summary>
    /// Simple script to start the intro animation when the scene loads.
    /// Attach this to a GameManager object and assign the cutscene controller.
    /// </summary>
    public class GameStarter : MonoBehaviour
    {
        [Header("Choose One Controller")]
        [SerializeField] private IntroAnimationController introController;      // Code-based
        [SerializeField] private IntroCutsceneAnimator cutsceneAnimator;        // Animator-based

        [SerializeField] private bool startOnAwake = true;

        private void Start()
        {
            if (startOnAwake)
            {
                StartGame();
            }
        }

        /// <summary>
        /// Start the intro cutscene.
        /// </summary>
        public void StartGame()
        {
            // Prefer animator-based if assigned
            if (cutsceneAnimator != null)
            {
                cutsceneAnimator.StartIntro();
            }
            else if (introController != null)
            {
                introController.StartIntro();
            }
            else
            {
                Debug.LogError("No intro controller assigned to GameStarter!");
            }
        }
    }
}