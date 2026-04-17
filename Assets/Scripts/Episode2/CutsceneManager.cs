using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Project.Episode2
{
    /// <summary>
    /// Manages the visual novel-style opening cutscene for Episode 2 Level 1.
    /// Handles the hexagon zoom animation, background transitions, and dialogue display.
    ///
    /// Flow:
    /// 1. Niko appears in center (in his room, looking angry)
    /// 2. After a few seconds, hexagon appears from outside camera
    /// 3. Hexagon shrinks, revealing only Niko's face inside
    /// 4. Background outside hexagon transitions to new background
    /// 5. Dialogue box appears with text and sound
    /// 6. After dialogue, Stage 1 begins
    /// </summary>
    public class CutsceneManager : MonoBehaviour
    {
        [Header("Character Display")]
        [SerializeField] private Image characterFullImage; // Full Niko in room
        [SerializeField] private Image characterFaceImage; // Just Niko's face for hexagon
        [SerializeField] private float characterDisplayDuration = 3f;

        [Header("Hexagon Animation")]
        [SerializeField] private RectTransform hexagonContainer; // Parent for both hexagon parts
        [SerializeField] private Image hexagonFill; // PuzzleBackground1 - the fill/background
        [SerializeField] private Image hexagonOutline; // PuzzleBackground2 - the outline
        [SerializeField] private float hexagonStartScale = 5f; // Start large (off screen)
        [SerializeField] private float hexagonEndScale = 1f; // End at normal size
        [SerializeField] private float hexagonZoomDuration = 1.5f;
        [SerializeField] private AnimationCurve hexagonZoomCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Background Transition")]
        [SerializeField] private Image roomBackground; // Niko's room background
        [SerializeField] private Image cutsceneBackground; // New background outside hexagon
        [SerializeField] private Image[] stageBackgrounds; // Backgrounds for each stage
        [SerializeField] private float backgroundFadeDuration = 0.5f;

        [Header("Dialogue System")]
        [SerializeField] private DialogueBox dialogueBox;
        [SerializeField] private DialogueData openingDialogue;
        [SerializeField] private DialogueData[] stageIntroDialogues; // Dialogue for each stage intro
        [SerializeField] private DialogueData[] stageCompleteDialogues; // Dialogue after completing each stage

        [Header("Events")]
        [SerializeField] private UnityEvent OnCutsceneStart;
        [SerializeField] private UnityEvent OnHexagonZoomStart;
        [SerializeField] private UnityEvent OnHexagonZoomComplete;
        [SerializeField] private UnityEvent OnDialogueStart;
        [SerializeField] private UnityEvent OnDialogueComplete;
        [SerializeField] private UnityEvent OnCutsceneComplete;
        [SerializeField] private UnityEvent OnStageTransitionComplete;

        private bool isCutscenePlaying = false;
        private int currentStageIndex = 0;

        public bool IsCutscenePlaying => isCutscenePlaying;

        [Serializable]
        public class DialogueData
        {
            public string characterName;
            [TextArea(3, 5)]
            public string dialogueText;
            public AudioClip voiceLine;
            public Sprite characterExpression;
        }

        private void Start()
        {
            // Initialize - hide cutscene elements
            InitializeCutscene();
        }

        private void InitializeCutscene()
        {
            // Show room background, hide cutscene background
            if (roomBackground != null)
                roomBackground.gameObject.SetActive(true);
            if (cutsceneBackground != null)
                cutsceneBackground.gameObject.SetActive(false);

            // Hide hexagon initially
            if (hexagonContainer != null)
            {
                hexagonContainer.gameObject.SetActive(false);
                hexagonContainer.localScale = Vector3.one * hexagonStartScale;
            }

            // Show full character image initially
            if (characterFullImage != null)
                characterFullImage.gameObject.SetActive(true);
            if (characterFaceImage != null)
                characterFaceImage.gameObject.SetActive(false);

            // Hide dialogue box
            if (dialogueBox != null)
                dialogueBox.Hide();
        }

        /// <summary>
        /// Starts the opening cutscene sequence.
        /// </summary>
        public void PlayOpeningCutscene()
        {
            if (isCutscenePlaying) return;
            StartCoroutine(OpeningCutsceneSequence());
        }

        private IEnumerator OpeningCutsceneSequence()
        {
            isCutscenePlaying = true;
            OnCutsceneStart?.Invoke();

            // Step 1: Show Niko in his room for a few seconds
            if (characterFullImage != null)
            {
                characterFullImage.gameObject.SetActive(true);
                yield return new WaitForSeconds(characterDisplayDuration);
            }

            // Step 2: Start hexagon zoom animation
            OnHexagonZoomStart?.Invoke();
            yield return StartCoroutine(HexagonZoomAnimation());
            OnHexagonZoomComplete?.Invoke();

            // Step 3: Show dialogue
            if (dialogueBox != null && openingDialogue != null)
            {
                OnDialogueStart?.Invoke();
                yield return StartCoroutine(ShowDialogue(openingDialogue));
                OnDialogueComplete?.Invoke();
            }

            isCutscenePlaying = false;
            OnCutsceneComplete?.Invoke();
        }

        private IEnumerator HexagonZoomAnimation()
        {
            // Activate hexagon and cutscene background
            if (hexagonContainer != null)
            {
                hexagonContainer.gameObject.SetActive(true);
                hexagonContainer.localScale = Vector3.one * hexagonStartScale;
            }

            // Swap to face image inside hexagon
            if (characterFaceImage != null)
            {
                characterFaceImage.gameObject.SetActive(true);
                // Parent face image to hexagon so it scales with it
                characterFaceImage.transform.SetParent(hexagonFill.transform);
                characterFaceImage.rectTransform.anchoredPosition = Vector2.zero;
            }

            // Start showing cutscene background (behind hexagon)
            if (cutsceneBackground != null)
            {
                cutsceneBackground.gameObject.SetActive(true);
                cutsceneBackground.color = new Color(1, 1, 1, 0);
            }

            // Animate hexagon shrinking and background fading
            float elapsed = 0f;
            Color bgStartColor = new Color(1, 1, 1, 0);
            Color bgEndColor = Color.white;

            while (elapsed < hexagonZoomDuration)
            {
                elapsed += Time.deltaTime;
                float t = hexagonZoomCurve.Evaluate(elapsed / hexagonZoomDuration);

                // Shrink hexagon
                if (hexagonContainer != null)
                {
                    float currentScale = Mathf.Lerp(hexagonStartScale, hexagonEndScale, t);
                    hexagonContainer.localScale = Vector3.one * currentScale;
                }

                // Fade in cutscene background
                if (cutsceneBackground != null)
                {
                    cutsceneBackground.color = Color.Lerp(bgStartColor, bgEndColor, t);
                }

                // Fade out room background
                if (roomBackground != null)
                {
                    roomBackground.color = Color.Lerp(Color.white, bgStartColor, t);
                }

                yield return null;
            }

            // Ensure final states
            if (hexagonContainer != null)
                hexagonContainer.localScale = Vector3.one * hexagonEndScale;
            if (cutsceneBackground != null)
                cutsceneBackground.color = Color.white;
            if (roomBackground != null)
                roomBackground.gameObject.SetActive(false);

            // Hide full character (we're now showing just the face in hexagon)
            if (characterFullImage != null)
                characterFullImage.gameObject.SetActive(false);
        }

        private IEnumerator ShowDialogue(DialogueData dialogue)
        {
            if (dialogueBox == null) yield break;

            // Update character expression if provided
            if (dialogue.characterExpression != null && characterFaceImage != null)
            {
                characterFaceImage.sprite = dialogue.characterExpression;
            }

            // Show dialogue box and play
            dialogueBox.Show();
            yield return dialogueBox.PlayDialogue(dialogue.dialogueText, dialogue.voiceLine);
        }

        /// <summary>
        /// Plays a stage intro dialogue and waits for completion.
        /// Called before starting a new puzzle stage.
        /// </summary>
        public IEnumerator PlayStageIntro(int stageIndex)
        {
            if (stageIntroDialogues == null || stageIndex >= stageIntroDialogues.Length) yield break;

            currentStageIndex = stageIndex;
            isCutscenePlaying = true;

            var dialogue = stageIntroDialogues[stageIndex];
            if (dialogue != null)
            {
                yield return StartCoroutine(ShowDialogue(dialogue));
            }

            isCutscenePlaying = false;
        }

        /// <summary>
        /// Plays a stage completion dialogue.
        /// Called after completing a puzzle stage.
        /// </summary>
        public IEnumerator PlayStageComplete(int stageIndex)
        {
            if (stageCompleteDialogues == null || stageIndex >= stageCompleteDialogues.Length) yield break;

            isCutscenePlaying = true;

            var dialogue = stageCompleteDialogues[stageIndex];
            if (dialogue != null)
            {
                yield return StartCoroutine(ShowDialogue(dialogue));
            }

            isCutscenePlaying = false;
        }

        /// <summary>
        /// Transitions to a new background for stage changes.
        /// No hexagon animation - just a direct background swap.
        /// </summary>
        public IEnumerator TransitionToStageBackground(int stageIndex)
        {
            if (stageBackgrounds == null || stageIndex >= stageBackgrounds.Length) yield break;

            isCutscenePlaying = true;

            // Get current and new backgrounds
            Image currentBg = stageIndex > 0 && stageIndex - 1 < stageBackgrounds.Length
                ? stageBackgrounds[stageIndex - 1]
                : cutsceneBackground;
            Image newBg = stageBackgrounds[stageIndex];

            if (newBg == null)
            {
                isCutscenePlaying = false;
                yield break;
            }

            // Fade transition
            newBg.gameObject.SetActive(true);
            newBg.color = new Color(1, 1, 1, 0);

            float elapsed = 0f;
            while (elapsed < backgroundFadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / backgroundFadeDuration;

                if (newBg != null)
                    newBg.color = Color.Lerp(new Color(1, 1, 1, 0), Color.white, t);
                if (currentBg != null && currentBg != cutsceneBackground)
                    currentBg.color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), t);

                yield return null;
            }

            // Finalize
            if (newBg != null)
                newBg.color = Color.white;
            if (currentBg != null && currentBg != cutsceneBackground)
                currentBg.gameObject.SetActive(false);

            isCutscenePlaying = false;
            OnStageTransitionComplete?.Invoke();
        }

        /// <summary>
        /// Updates the character face expression.
        /// </summary>
        public void SetCharacterExpression(Sprite expression)
        {
            if (characterFaceImage != null && expression != null)
            {
                characterFaceImage.sprite = expression;
            }
        }

        /// <summary>
        /// Hides the dialogue box.
        /// </summary>
        public void HideDialogue()
        {
            if (dialogueBox != null)
                dialogueBox.Hide();
        }

        /// <summary>
        /// Skips the current cutscene animation.
        /// </summary>
        public void SkipCutscene()
        {
            StopAllCoroutines();

            // Set final states immediately
            if (hexagonContainer != null)
            {
                hexagonContainer.gameObject.SetActive(true);
                hexagonContainer.localScale = Vector3.one * hexagonEndScale;
            }
            if (cutsceneBackground != null)
            {
                cutsceneBackground.gameObject.SetActive(true);
                cutsceneBackground.color = Color.white;
            }
            if (roomBackground != null)
                roomBackground.gameObject.SetActive(false);
            if (characterFullImage != null)
                characterFullImage.gameObject.SetActive(false);
            if (characterFaceImage != null)
                characterFaceImage.gameObject.SetActive(true);

            isCutscenePlaying = false;
            OnCutsceneComplete?.Invoke();
        }
    }
}
