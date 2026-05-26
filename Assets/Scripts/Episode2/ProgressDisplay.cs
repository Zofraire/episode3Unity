using UnityEngine;
using UnityEngine.UI;

namespace Project.Episode2
{
    /// <summary>
    /// Displays progress through visual indicators.
    /// Used for showing Niko's face changes in Game 1 and general progress in other games.
    /// </summary>
    public class ProgressDisplay : MonoBehaviour
    {
        [Header("Progress Settings")]
        [SerializeField] private int totalSteps = 4;
        [SerializeField] private Image[] stepIndicators;
        [SerializeField] private Sprite incompleteSprite;
        [SerializeField] private Sprite completeSprite;

        [Header("Optional Slider")]
        [SerializeField] private Slider progressSlider;

        [Header("Optional Face Display")]
        [SerializeField] private Image faceDisplay;
        [SerializeField] private Sprite[] faceSprites; // Progressive face changes

        private int currentStep = 0;

        private void Start()
        {
            ResetProgress();
        }

        public void ResetProgress()
        {
            currentStep = 0;
            UpdateDisplay();
        }

        public void IncrementProgress()
        {
            if (currentStep < totalSteps)
            {
                currentStep++;
                UpdateDisplay();
            }
        }

        public void SetProgress(int step)
        {
            currentStep = Mathf.Clamp(step, 0, totalSteps);
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            // Update step indicators
            if (stepIndicators != null)
            {
                for (int i = 0; i < stepIndicators.Length; i++)
                {
                    if (stepIndicators[i] != null)
                    {
                        stepIndicators[i].sprite = i < currentStep ? completeSprite : incompleteSprite;
                    }
                }
            }

            // Update slider
            if (progressSlider != null)
            {
                progressSlider.maxValue = totalSteps;
                progressSlider.value = currentStep;
            }

            // Update face display
            if (faceDisplay != null && faceSprites != null && faceSprites.Length > 0)
            {
                int faceIndex = Mathf.Clamp(currentStep, 0, faceSprites.Length - 1);
                faceDisplay.sprite = faceSprites[faceIndex];
            }
        }

        public int GetCurrentStep() => currentStep;
        public bool IsComplete() => currentStep >= totalSteps;
    }
}
