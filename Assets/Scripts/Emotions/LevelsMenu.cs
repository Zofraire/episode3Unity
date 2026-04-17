using UnityEngine;
using UnityEngine.UI;

namespace Project.Emotions
{
    public class LevelsMenu : MonoBehaviour
    {
        [SerializeField] private Button[] levels;

        void Start()
        {
            if (!PlayerPrefs.HasKey("levelsUnlocked"))
            {
                PlayerPrefs.SetInt("levelsUnlocked", 1);
                PlayerPrefs.Save();
            }

            int levelsUnlocked = PlayerPrefs.GetInt("levelsUnlocked");

            for (int i = 0; i < levels.Length; i++)
            {
                if (i < levelsUnlocked) levels[i].interactable = true;
                else levels[i].interactable = false;
            }
        }
    }
}
