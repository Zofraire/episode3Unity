using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace Project.Episode2
{
    /// <summary>
    /// Visual novel-style dialogue box with talking animation.
    /// Displays text with typewriter effect while alternating between
    /// two mouth images to create a talking illusion.
    /// </summary>
    public class DialogueBox : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private Image continueIndicator;

        [Header("Character Mouth Animation")]
        [SerializeField] private Image characterMouthImage;
        [SerializeField] private Sprite mouthOpen;
        [SerializeField] private Sprite mouthClosed;
        [SerializeField] private float mouthAnimationSpeed = 0.1f;

        [Header("Typewriter Settings")]
        [SerializeField] private float typewriterSpeed = 0.03f;
        [SerializeField] private float punctuationPause = 0.2f;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip typewriterSound;

        [Header("Animation Settings")]
        [SerializeField] private float showAnimationDuration = 0.3f;
        [SerializeField] private float hideAnimationDuration = 0.2f;
        [SerializeField] private AnimationCurve showCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Events")]
        [SerializeField] private UnityEvent OnDialogueStart;
        [SerializeField] private UnityEvent OnDialogueTextComplete;
        [SerializeField] private UnityEvent OnDialogueEnd;
        [SerializeField] private UnityEvent OnContinuePressed;

        private bool isTyping = false;
        private bool isWaitingForInput = false;
        private bool skipRequested = false;
        private Coroutine currentDialogue;
        private Coroutine mouthAnimation;
        private CanvasGroup canvasGroup;

        public bool IsTyping => isTyping;
        public bool IsWaitingForInput => isWaitingForInput;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null && dialoguePanel != null)
            {
                canvasGroup = dialoguePanel.GetComponent<CanvasGroup>();
            }
        }

        private void Update()
        {
            // Check for skip/continue input
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                if (isTyping)
                {
                    skipRequested = true;
                }
                else if (isWaitingForInput)
                {
                    Continue();
                }
            }
        }

        /// <summary>
        /// Shows the dialogue box with animation.
        /// </summary>
        public void Show()
        {
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(true);
                StartCoroutine(AnimateShow());
            }
        }

        /// <summary>
        /// Hides the dialogue box with animation.
        /// </summary>
        public void Hide()
        {
            StartCoroutine(AnimateHide());
        }

        private IEnumerator AnimateShow()
        {
            if (canvasGroup == null) yield break;

            canvasGroup.alpha = 0;
            float elapsed = 0f;

            while (elapsed < showAnimationDuration)
            {
                elapsed += Time.deltaTime;
                float t = showCurve.Evaluate(elapsed / showAnimationDuration);
                canvasGroup.alpha = t;
                yield return null;
            }

            canvasGroup.alpha = 1;
        }

        private IEnumerator AnimateHide()
        {
            if (canvasGroup != null)
            {
                float elapsed = 0f;
                while (elapsed < hideAnimationDuration)
                {
                    elapsed += Time.deltaTime;
                    float t = 1 - (elapsed / hideAnimationDuration);
                    canvasGroup.alpha = t;
                    yield return null;
                }
            }

            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);

            OnDialogueEnd?.Invoke();
        }

        /// <summary>
        /// Plays a dialogue with typewriter effect and talking animation.
        /// </summary>
        /// <param name="text">The text to display</param>
        /// <param name="voiceLine">Optional voice audio to play</param>
        /// <returns>Coroutine that completes when player continues</returns>
        public IEnumerator PlayDialogue(string text, AudioClip voiceLine = null)
        {
            if (dialogueText == null) yield break;

            OnDialogueStart?.Invoke();
            isTyping = true;
            isWaitingForInput = false;
            skipRequested = false;

            // Clear previous text
            dialogueText.text = "";

            // Hide continue indicator
            if (continueIndicator != null)
                continueIndicator.gameObject.SetActive(false);

            // Play voice line
            if (audioSource != null && voiceLine != null)
            {
                audioSource.clip = voiceLine;
                audioSource.Play();
            }

            // Start mouth animation
            if (mouthAnimation != null)
                StopCoroutine(mouthAnimation);
            mouthAnimation = StartCoroutine(AnimateMouth());

            // Typewriter effect
            yield return StartCoroutine(TypeText(text));

            // Stop mouth animation
            if (mouthAnimation != null)
            {
                StopCoroutine(mouthAnimation);
                mouthAnimation = null;
            }

            // Set mouth to closed
            if (characterMouthImage != null && mouthClosed != null)
                characterMouthImage.sprite = mouthClosed;

            isTyping = false;
            OnDialogueTextComplete?.Invoke();

            // Show continue indicator
            if (continueIndicator != null)
            {
                continueIndicator.gameObject.SetActive(true);
                StartCoroutine(AnimateContinueIndicator());
            }

            // Wait for player input
            isWaitingForInput = true;
            while (isWaitingForInput)
            {
                yield return null;
            }

            OnContinuePressed?.Invoke();
        }

        /// <summary>
        /// Plays dialogue with character name displayed.
        /// </summary>
        public IEnumerator PlayDialogue(string characterName, string text, AudioClip voiceLine = null)
        {
            if (characterNameText != null)
                characterNameText.text = characterName;

            yield return PlayDialogue(text, voiceLine);
        }

        private IEnumerator TypeText(string text)
        {
            dialogueText.text = "";

            for (int i = 0; i < text.Length; i++)
            {
                // Check for skip
                if (skipRequested)
                {
                    dialogueText.text = text;
                    skipRequested = false;
                    yield break;
                }

                char c = text[i];
                dialogueText.text += c;

                // Play typewriter sound
                if (typewriterSound != null && audioSource != null && !char.IsWhiteSpace(c))
                {
                    audioSource.PlayOneShot(typewriterSound, 0.5f);
                }

                // Pause for punctuation
                if (c == '.' || c == '!' || c == '?' || c == ',')
                {
                    yield return new WaitForSeconds(punctuationPause);
                }
                else
                {
                    yield return new WaitForSeconds(typewriterSpeed);
                }
            }
        }

        private IEnumerator AnimateMouth()
        {
            if (characterMouthImage == null || mouthOpen == null || mouthClosed == null)
                yield break;

            bool isOpen = false;

            while (isTyping)
            {
                characterMouthImage.sprite = isOpen ? mouthOpen : mouthClosed;
                isOpen = !isOpen;
                yield return new WaitForSeconds(mouthAnimationSpeed);
            }

            // Ensure mouth is closed when done
            characterMouthImage.sprite = mouthClosed;
        }

        private IEnumerator AnimateContinueIndicator()
        {
            if (continueIndicator == null) yield break;

            float time = 0f;
            Vector3 startPos = continueIndicator.rectTransform.anchoredPosition;

            while (isWaitingForInput)
            {
                time += Time.deltaTime;
                float offset = Mathf.Sin(time * 3f) * 5f;
                continueIndicator.rectTransform.anchoredPosition = startPos + new Vector3(0, offset, 0);
                yield return null;
            }
        }

        /// <summary>
        /// Called when player wants to continue (tap/click/key press).
        /// </summary>
        public void Continue()
        {
            if (isWaitingForInput)
            {
                isWaitingForInput = false;
            }
        }

        /// <summary>
        /// Sets the mouth sprites for talking animation.
        /// </summary>
        public void SetMouthSprites(Sprite open, Sprite closed)
        {
            mouthOpen = open;
            mouthClosed = closed;
        }

        /// <summary>
        /// Sets the character name displayed in the dialogue box.
        /// </summary>
        public void SetCharacterName(string name)
        {
            if (characterNameText != null)
                characterNameText.text = name;
        }

        /// <summary>
        /// Immediately completes any current dialogue.
        /// </summary>
        public void CompleteImmediately()
        {
            skipRequested = true;
        }
    }
}
