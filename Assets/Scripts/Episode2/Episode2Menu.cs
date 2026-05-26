using UnityEngine;
using UnityEngine.UI;

namespace Project.Episode2
{
    /// <summary>
    /// Level selection menu for Episode 2: "Сэтгэл хөдлөлийн ертөнц" (World of Emotions)
    /// Manages the unlocking and selection of the 3 mini-games:
    /// - Game 1: Puzzle Assembly (Understanding the bully's emotions)
    /// - Game 2: Color Wheel Memory (Memory and attention game for bystanders)
    /// - Game 3: Story Quiz (Identifying types of peer bullying)
    /// </summary>
    public class Episode2Menu : MonoBehaviour
    {
        [Header("Level Buttons")]
        [SerializeField] private Button puzzleGameButton;
        [SerializeField] private Button memoryGameButton;
        [SerializeField] private Button quizGameButton;

        [Header("Lock Icons")]
        [SerializeField] private GameObject puzzleLockIcon;
        [SerializeField] private GameObject memoryLockIcon;
        [SerializeField] private GameObject quizLockIcon;

        [Header("Completion Indicators")]
        [SerializeField] private GameObject puzzleCompleteIcon;
        [SerializeField] private GameObject memoryCompleteIcon;
        [SerializeField] private GameObject quizCompleteIcon;

        private const string LEVELS_UNLOCKED_KEY = "episode2LevelsUnlocked";
        private const string EPISODE_COMPLETE_KEY = "episode2Complete";

        private void Start()
        {
            UpdateMenuState();
        }

        private void UpdateMenuState()
        {
            // Get saved progress
            int levelsUnlocked = PlayerPrefs.GetInt(LEVELS_UNLOCKED_KEY, 1);
            bool episodeComplete = PlayerPrefs.GetInt(EPISODE_COMPLETE_KEY, 0) == 1;

            // Game 1: Puzzle Assembly - Always unlocked
            if (puzzleGameButton != null)
                puzzleGameButton.interactable = true;

            if (puzzleLockIcon != null)
                puzzleLockIcon.SetActive(false);

            if (puzzleCompleteIcon != null)
                puzzleCompleteIcon.SetActive(levelsUnlocked >= 2);

            // Game 2: Memory Game - Unlocked after completing Game 1
            if (memoryGameButton != null)
                memoryGameButton.interactable = levelsUnlocked >= 2;

            if (memoryLockIcon != null)
                memoryLockIcon.SetActive(levelsUnlocked < 2);

            if (memoryCompleteIcon != null)
                memoryCompleteIcon.SetActive(levelsUnlocked >= 3);

            // Game 3: Quiz Game - Unlocked after completing Game 2
            if (quizGameButton != null)
                quizGameButton.interactable = levelsUnlocked >= 3;

            if (quizLockIcon != null)
                quizLockIcon.SetActive(levelsUnlocked < 3);

            if (quizCompleteIcon != null)
                quizCompleteIcon.SetActive(episodeComplete);
        }

        public void ResetProgress()
        {
            PlayerPrefs.SetInt(LEVELS_UNLOCKED_KEY, 1);
            PlayerPrefs.SetInt(EPISODE_COMPLETE_KEY, 0);
            PlayerPrefs.Save();
            UpdateMenuState();
        }

        public void UnlockAllLevels()
        {
            PlayerPrefs.SetInt(LEVELS_UNLOCKED_KEY, 3);
            PlayerPrefs.Save();
            UpdateMenuState();
        }
    }
}
