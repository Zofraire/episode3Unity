using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Collections;

namespace Project.Episode2
{
    /// <summary>
    /// Controls the intro animation sequence for Episode 2 Level 1:
    /// 1. Room scene with angry Niko (full screen)
    /// 2. Hexagon appears and shrinks inward (masking effect)
    /// 3. Background OUTSIDE hexagon changes to new scene (forest/decorative)
    /// 4. Hexagon stops at face size, Niko changes to thinking pose
    /// 5. Dialogue box slides in with text and talking animation
    /// 6. After dialogue, puzzle gameplay begins
    ///
    /// The hexagon serves dual purpose:
    /// - During intro: Contains Niko's face, shrinks to reveal new background
    /// - During puzzle: Same hexagon becomes the puzzle background where pieces go
    /// </summary>
    public class IntroAnimationController : MonoBehaviour
    {
        [Header("Backgrounds")]
        [SerializeField] private GameObject roomBackground;      // Full Niko room scene (Frame 35)
        [SerializeField] private GameObject stageBackground;     // Forest/decorative background
        [SerializeField] private Image roomBackgroundImage;      // For fading

        [Header("Hexagon (Puzzle Background)")]
        [SerializeField] private RectTransform hexagonFill;      // PuzzleBackground1 - the fill
        [SerializeField] private RectTransform hexagonOutline;   // PuzzleBackground2 - the outline
        [SerializeField] private Vector2 finalHexagonSize = new Vector2(400, 400);
        [SerializeField] private Vector2 startHexagonScale = new Vector2(3, 3);

        [Header("Niko Character")]
        [SerializeField] private GameObject nikoFace;            // Niko's face inside hexagon
        [SerializeField] private Image nikoFaceImage;            // For changing expression
        [SerializeField] private Sprite nikoAngrySprite;         // Initial angry pose
        [SerializeField] private Sprite nikoThinkingSprite;      // Thinking pose after zoom
        [SerializeField] private bool parentNikoToHexagon = true; // Parent NikoFace to hexagon for masking

        [Header("Talking Animation")]
        [SerializeField] private Image nikoMouthImage;           // Mouth image (separate from face)
        [SerializeField] private Sprite mouthOpen;               // Open mouth sprite
        [SerializeField] private Sprite mouthClosed;             // Closed mouth sprite
        [SerializeField] private float mouthAnimationSpeed = 0.1f;

        [Header("Dialogue Box")]
        [SerializeField] private GameObject dialogueBox;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private RectTransform dialogueBoxRect;
        [SerializeField] private float dialogueSlideDistance = 200f;
        [SerializeField] private float dialogueSlideDuration = 0.5f;
        [SerializeField] private string[] dialogueLines;
        [SerializeField] private float dialogueDisplayTime = 3f;
        [SerializeField] private float typewriterSpeed = 0.03f;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] dialogueVoiceLines;

        [Header("Puzzle Elements")]
        [SerializeField] private GameObject piecesContainer;
        [SerializeField] private GameObject targetSlots;

        [Header("Stage Backgrounds")]
        [SerializeField] private Image[] stageBackgroundImages;  // Different backgrounds for each stage

        [Header("Animation Settings")]
        [SerializeField] private float roomDisplayDuration = 2f;
        [SerializeField] private float hexagonShrinkDuration = 1.5f;
        [SerializeField] private float dialogueDelay = 0.5f;
        [SerializeField] private AnimationCurve shrinkCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Events")]
        [SerializeField] private UnityEvent OnIntroStart;
        [SerializeField] private UnityEvent OnHexagonZoomComplete;
        [SerializeField] private UnityEvent OnDialogueComplete;
        [SerializeField] private UnityEvent OnIntroComplete;
        [SerializeField] private UnityEvent OnStageBackgroundChanged;

        private bool isAnimating = false;
        private Coroutine mouthAnimationCoroutine;
        private Vector3 dialogueBoxStartPos;
        private int currentStageIndex = 0;
        private Transform nikoFaceOriginalParent;
        private Vector3 nikoFaceOriginalLocalPosition;

        public bool IsAnimating => isAnimating;

        private void Start()
        {
            InitializeState();
        }

        private void InitializeState()
        {
            // Room background visible at start
            if (roomBackground != null)
                roomBackground.SetActive(true);

            // Hide other elements initially
            if (stageBackground != null)
                stageBackground.SetActive(false);
            if (hexagonFill != null)
                hexagonFill.gameObject.SetActive(false);
            if (hexagonOutline != null)
                hexagonOutline.gameObject.SetActive(false);
            if (nikoFace != null)
            {
                // Store original parent and position for later restoration
                nikoFaceOriginalParent = nikoFace.transform.parent;
                nikoFaceOriginalLocalPosition = nikoFace.transform.localPosition;
                nikoFace.SetActive(false);
            }
            if (dialogueBox != null)
                dialogueBox.SetActive(false);
            if (piecesContainer != null)
                piecesContainer.SetActive(false);
            if (targetSlots != null)
                targetSlots.SetActive(false);

            // Store dialogue box position for slide animation
            if (dialogueBoxRect != null)
                dialogueBoxStartPos = dialogueBoxRect.anchoredPosition;
        }

        /// <summary>
        /// Starts the intro animation sequence.
        /// Call this from GameStarter or when the scene loads.
        /// </summary>
        public void StartIntro()
        {
            if (isAnimating) return;
            StartCoroutine(IntroSequence());
        }

        private IEnumerator IntroSequence()
        {
            isAnimating = true;
            OnIntroStart?.Invoke();

            // Step 1: Display room scene with angry Niko
            yield return new WaitForSeconds(roomDisplayDuration);

            // Step 2: Start hexagon zoom animation
            yield return StartCoroutine(HexagonZoomIn());
            OnHexagonZoomComplete?.Invoke();

            // Step 3: Change Niko to thinking pose
            if (nikoFaceImage != null && nikoThinkingSprite != null)
                nikoFaceImage.sprite = nikoThinkingSprite;

            yield return new WaitForSeconds(dialogueDelay);

            // Step 4: Slide in dialogue box and play dialogue
            yield return StartCoroutine(ShowDialogue());
            OnDialogueComplete?.Invoke();

            // Step 5: Prepare for puzzle gameplay
            yield return StartCoroutine(TransitionToPuzzle());

            isAnimating = false;
            OnIntroComplete?.Invoke();
        }

        private IEnumerator HexagonZoomIn()
        {
            // Activate stage background first (will be revealed as hexagon shrinks)
            // This needs to be BEHIND the hexagon
            if (stageBackground != null)
            {
                stageBackground.SetActive(true);
                var stageBgImage = stageBackground.GetComponent<Image>();
                if (stageBgImage != null)
                    stageBgImage.color = new Color(1, 1, 1, 0);
            }

            // Activate hexagon elements at large scale
            if (hexagonFill != null)
            {
                hexagonFill.gameObject.SetActive(true);
                hexagonFill.localScale = new Vector3(startHexagonScale.x, startHexagonScale.y, 1);
            }
            if (hexagonOutline != null)
            {
                hexagonOutline.gameObject.SetActive(true);
                hexagonOutline.localScale = new Vector3(startHexagonScale.x, startHexagonScale.y, 1);
            }

            // Show Niko face inside hexagon - PARENT to hexagonFill for masking
            if (nikoFace != null)
            {
                // Parent NikoFace to hexagonFill so the Mask component clips it
                if (parentNikoToHexagon && hexagonFill != null)
                {
                    nikoFace.transform.SetParent(hexagonFill);
                    nikoFace.transform.localPosition = Vector3.zero;
                    nikoFace.transform.localScale = Vector3.one;
                }

                nikoFace.SetActive(true);
                if (nikoFaceImage != null && nikoAngrySprite != null)
                    nikoFaceImage.sprite = nikoAngrySprite;
            }

            // Animate hexagon shrinking and backgrounds transitioning
            float elapsed = 0f;
            Color startColor = Color.white;
            Color endColor = new Color(1, 1, 1, 0);

            while (elapsed < hexagonShrinkDuration)
            {
                elapsed += Time.deltaTime;
                float t = shrinkCurve.Evaluate(elapsed / hexagonShrinkDuration);

                // Shrink hexagon from start scale to 1
                float currentScale = Mathf.Lerp(startHexagonScale.x, 1f, t);
                if (hexagonFill != null)
                    hexagonFill.localScale = Vector3.one * currentScale;
                if (hexagonOutline != null)
                    hexagonOutline.localScale = Vector3.one * currentScale;

                // Fade out room background
                if (roomBackgroundImage != null)
                    roomBackgroundImage.color = Color.Lerp(startColor, endColor, t);

                // Fade in stage background
                if (stageBackground != null)
                {
                    var stageBgImage = stageBackground.GetComponent<Image>();
                    if (stageBgImage != null)
                        stageBgImage.color = Color.Lerp(endColor, startColor, t);
                }

                yield return null;
            }

            // Ensure final states
            if (hexagonFill != null)
                hexagonFill.localScale = Vector3.one;
            if (hexagonOutline != null)
                hexagonOutline.localScale = Vector3.one;
            if (roomBackground != null)
                roomBackground.SetActive(false);
            if (stageBackground != null)
            {
                var stageBgImage = stageBackground.GetComponent<Image>();
                if (stageBgImage != null)
                    stageBgImage.color = Color.white;
            }
        }

        private IEnumerator ShowDialogue()
        {
            if (dialogueBox == null || dialogueText == null) yield break;

            // Position dialogue box off screen for slide-in
            if (dialogueBoxRect != null)
            {
                dialogueBoxRect.anchoredPosition = dialogueBoxStartPos + new Vector3(0, -dialogueSlideDistance, 0);
            }

            dialogueBox.SetActive(true);

            // Slide dialogue box in
            yield return StartCoroutine(SlideDialogueBox(true));

            // Play each dialogue line with talking animation
            for (int i = 0; i < dialogueLines.Length; i++)
            {
                // Start mouth animation
                StartMouthAnimation();

                // Play voice line if available
                if (audioSource != null && dialogueVoiceLines != null && i < dialogueVoiceLines.Length && dialogueVoiceLines[i] != null)
                {
                    audioSource.clip = dialogueVoiceLines[i];
                    audioSource.Play();
                }

                // Typewriter effect for text
                yield return StartCoroutine(TypewriterText(dialogueLines[i]));

                // Stop mouth animation
                StopMouthAnimation();

                // Wait for display duration
                yield return new WaitForSeconds(dialogueDisplayTime);
            }

            // Slide dialogue box out
            yield return StartCoroutine(SlideDialogueBox(false));

            dialogueBox.SetActive(false);
        }

        private IEnumerator SlideDialogueBox(bool slideIn)
        {
            if (dialogueBoxRect == null) yield break;

            Vector3 startPos = slideIn
                ? dialogueBoxStartPos + new Vector3(0, -dialogueSlideDistance, 0)
                : dialogueBoxStartPos;
            Vector3 endPos = slideIn
                ? dialogueBoxStartPos
                : dialogueBoxStartPos + new Vector3(0, -dialogueSlideDistance, 0);

            float elapsed = 0f;
            while (elapsed < dialogueSlideDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / dialogueSlideDuration;
                dialogueBoxRect.anchoredPosition = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }

            dialogueBoxRect.anchoredPosition = endPos;
        }

        private IEnumerator TypewriterText(string text)
        {
            dialogueText.text = "";
            foreach (char c in text)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(typewriterSpeed);
            }
        }

        private void StartMouthAnimation()
        {
            if (nikoMouthImage == null || mouthOpen == null || mouthClosed == null) return;
            mouthAnimationCoroutine = StartCoroutine(AnimateMouth());
        }

        private void StopMouthAnimation()
        {
            if (mouthAnimationCoroutine != null)
            {
                StopCoroutine(mouthAnimationCoroutine);
                mouthAnimationCoroutine = null;
            }

            // Set mouth to closed
            if (nikoMouthImage != null && mouthClosed != null)
                nikoMouthImage.sprite = mouthClosed;
        }

        private IEnumerator AnimateMouth()
        {
            bool isOpen = false;
            while (true)
            {
                if (nikoMouthImage != null)
                    nikoMouthImage.sprite = isOpen ? mouthOpen : mouthClosed;
                isOpen = !isOpen;
                yield return new WaitForSeconds(mouthAnimationSpeed);
            }
        }

        private IEnumerator TransitionToPuzzle()
        {
            // Hide Niko face (puzzle pieces will go on hexagon fill now)
            if (nikoFace != null)
            {
                nikoFace.SetActive(false);

                // Restore to original parent if it was moved
                if (parentNikoToHexagon && nikoFaceOriginalParent != null)
                {
                    nikoFace.transform.SetParent(nikoFaceOriginalParent);
                    nikoFace.transform.localPosition = nikoFaceOriginalLocalPosition;
                }
            }

            // Show puzzle elements
            if (piecesContainer != null)
                piecesContainer.SetActive(true);
            if (targetSlots != null)
                targetSlots.SetActive(true);

            yield return null;
        }

        /// <summary>
        /// Changes the stage background directly (no hexagon animation).
        /// Call this after completing each puzzle stage.
        /// </summary>
        public void ChangeStageBackground(int stageIndex)
        {
            StartCoroutine(ChangeStageBackgroundCoroutine(stageIndex));
        }

        /// <summary>
        /// Changes the stage background with a fade transition.
        /// </summary>
        public IEnumerator ChangeStageBackgroundCoroutine(int stageIndex)
        {
            if (stageBackgroundImages == null || stageIndex >= stageBackgroundImages.Length) yield break;

            Image newBg = stageBackgroundImages[stageIndex];
            if (newBg == null) yield break;

            // Get current background
            Image currentBg = currentStageIndex > 0 && currentStageIndex - 1 < stageBackgroundImages.Length
                ? stageBackgroundImages[currentStageIndex - 1]
                : (stageBackground != null ? stageBackground.GetComponent<Image>() : null);

            // Fade transition
            newBg.gameObject.SetActive(true);
            newBg.color = new Color(1, 1, 1, 0);

            float fadeDuration = 0.5f;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;

                newBg.color = Color.Lerp(new Color(1, 1, 1, 0), Color.white, t);
                if (currentBg != null)
                    currentBg.color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), t);

                yield return null;
            }

            // Finalize
            newBg.color = Color.white;
            if (currentBg != null && currentBg.gameObject != stageBackground)
                currentBg.gameObject.SetActive(false);

            currentStageIndex = stageIndex;
            OnStageBackgroundChanged?.Invoke();
        }

        /// <summary>
        /// Shows dialogue with talking animation.
        /// Call this between stages to show completion messages.
        /// </summary>
        public IEnumerator ShowStageDialogue(string text, AudioClip voiceLine = null)
        {
            if (dialogueBox == null || dialogueText == null) yield break;

            // Show Niko face for dialogue - parent to hexagon for masking
            if (nikoFace != null)
            {
                if (parentNikoToHexagon && hexagonFill != null)
                {
                    nikoFace.transform.SetParent(hexagonFill);
                    nikoFace.transform.localPosition = Vector3.zero;
                    nikoFace.transform.localScale = Vector3.one;
                }
                nikoFace.SetActive(true);
            }

            // Position and show dialogue box
            if (dialogueBoxRect != null)
                dialogueBoxRect.anchoredPosition = dialogueBoxStartPos + new Vector3(0, -dialogueSlideDistance, 0);

            dialogueBox.SetActive(true);
            yield return StartCoroutine(SlideDialogueBox(true));

            // Start mouth animation
            StartMouthAnimation();

            // Play voice line
            if (audioSource != null && voiceLine != null)
            {
                audioSource.clip = voiceLine;
                audioSource.Play();
            }

            // Typewriter text
            yield return StartCoroutine(TypewriterText(text));

            // Stop mouth animation
            StopMouthAnimation();

            // Wait
            yield return new WaitForSeconds(dialogueDisplayTime);

            // Hide dialogue
            yield return StartCoroutine(SlideDialogueBox(false));
            dialogueBox.SetActive(false);

            // Hide Niko face and restore to original parent
            if (nikoFace != null)
            {
                nikoFace.SetActive(false);
                if (parentNikoToHexagon && nikoFaceOriginalParent != null)
                {
                    nikoFace.transform.SetParent(nikoFaceOriginalParent);
                    nikoFace.transform.localPosition = nikoFaceOriginalLocalPosition;
                }
            }
        }

        /// <summary>
        /// Skips to the end of the intro animation immediately.
        /// </summary>
        public void SkipIntro()
        {
            StopAllCoroutines();
            StopMouthAnimation();

            // Set final states
            if (roomBackground != null)
                roomBackground.SetActive(false);
            if (stageBackground != null)
            {
                stageBackground.SetActive(true);
                var img = stageBackground.GetComponent<Image>();
                if (img != null) img.color = Color.white;
            }
            if (hexagonFill != null)
            {
                hexagonFill.gameObject.SetActive(true);
                hexagonFill.localScale = Vector3.one;
            }
            if (hexagonOutline != null)
            {
                hexagonOutline.gameObject.SetActive(true);
                hexagonOutline.localScale = Vector3.one;
            }
            if (nikoFace != null)
            {
                nikoFace.SetActive(false);
                // Restore to original parent if it was moved
                if (parentNikoToHexagon && nikoFaceOriginalParent != null)
                {
                    nikoFace.transform.SetParent(nikoFaceOriginalParent);
                    nikoFace.transform.localPosition = nikoFaceOriginalLocalPosition;
                }
            }
            if (dialogueBox != null)
                dialogueBox.SetActive(false);
            if (piecesContainer != null)
                piecesContainer.SetActive(true);
            if (targetSlots != null)
                targetSlots.SetActive(true);

            isAnimating = false;
            OnIntroComplete?.Invoke();
        }
    }
}