using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Episode2
{
    /// <summary>
    /// Scene controller for Episode 2 navigation.
    /// Handles transitions between the 3 mini-games and menu.
    /// </summary>
    public class Episode2SceneController : MonoBehaviour
    {
        // Scene names for Episode 2
        public const string MENU_SCENE = "Episode2 Menu";
        public const string PUZZLE_GAME_SCENE = "Episode2 Puzzle Game";
        public const string MEMORY_GAME_SCENE = "Episode2 Memory Game";
        public const string QUIZ_GAME_SCENE = "Episode2 Quiz Game";

        public void LoadEpisode2Menu()
        {
            SceneManager.LoadScene(MENU_SCENE);
        }

        public void LoadPuzzleGame()
        {
            SceneManager.LoadScene(PUZZLE_GAME_SCENE);
        }

        public void LoadMemoryGame()
        {
            SceneManager.LoadScene(MEMORY_GAME_SCENE);
        }

        public void LoadQuizGame()
        {
            SceneManager.LoadScene(QUIZ_GAME_SCENE);
        }

        public void ReloadCurrentScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void LoadMainMenu()
        {
            SceneManager.LoadScene("Home");
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
