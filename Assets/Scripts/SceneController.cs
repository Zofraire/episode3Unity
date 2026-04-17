using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project
{
    public class SceneController : MonoBehaviour
    {
        public void Reload()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}
