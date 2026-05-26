using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace Project.Episode2
{
    /// <summary>
    /// Controls the intro cutscene using Unity Animator.
    /// Attach this to the Canvas or IntroController GameObject.
    /// Use Animation Events to call methods from animation clips.
    /// </summary>
    public class IntroCutsceneAnimator : MonoBehaviour
    {
        [Header("Animator")]
        [SerializeField] private Animator animator;

        [Header("Dialogue")]
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private string[] dialogueLines;
        [SerializeField] private float typewriterSpeed = 0.03f;

        [Header("Talking Animation")]
        [SerializeField] private Image nikoMouthImage;
        [SerializeField] private Sprite mouthOpen;
        [SerializeField] private Sprite mouthClosed;
        [SerializeField] private float mouthAnimationSpeed = 0.1f;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] dialogueVoiceLines;

        [Header("Events")]
        [SerializeField] private UnityEvent OnIntroStart;
        [SerializeField] private UnityEvent OnHexagonZoomComplete;
        [SerializeField] private UnityEvent OnDialogueStart;
        [SerializeField] private UnityEvent OnDialogueComplete;
        [SerializeField] private UnityEvent OnIntroComplete;

        private Coroutine mouthAnimationCoroutine;
        private Coroutine typewriterCoroutine;
        private int currentDialogueIndex = 0;
        private bool isPlaying = false;

        public bool IsPlaying => isPlaying;

        private void Start()
        {
            // Optionally auto-start the intro
            // StartIntro();
        }

        /// <summary>
        /// Call this to start the intro cutscene.
        /// Can be called from GameStarter or another script.
        /// </summary>
        public void StartIntro()
        {
            if (isPlaying) return;
            isPlaying = true;
            OnIntroStart?.Invoke();

            // The animator should auto-play from Entry state
            if (animator != null)
            {
                animator.enabled = true;
                animator.Play("Intro_RoomScene", 0, 0);
            }
        }

        // ============================================
        // ANIMATION EVENT METHODS
        // Call these from Animation Events in Unity
        // ============================================

        /// <summary>
        /// Called by Animation Event when room scene ends.
        /// </summary>
        public void OnRoomSceneEnd()
        {
            Debug.Log("Room scene ended, starting hexagon zoom");
        }

        /// <summary>
        /// Called by Animation Event when hexagon zoom completes.
        /// </summary>
        public void OnHexagonZoomEnd()
        {
            Debug.Log("Hexagon zoom complete");
            OnHexagonZoomComplete?.Invoke();
        }

        /// <summary>
        /// Called by Animation Event to start dialogue with typing effect.
        /// </summary>
        public void StartDialogueTyping()
        {
            OnDialogueStart?.Invoke();
            currentDialogueIndex = 0;
            StartCoroutine(PlayAllDialogueLines());
        }

        private IEnumerator PlayAllDialogueLines()
        {
            for (int i = 0; i < dialogueLines.Length; i++)
            {
                currentDialogueIndex = i;

                // Start mouth animation
                StartMouthAnimation();

                // Play voice line if available
                if (audioSource != null && dialogueVoiceLines != null &&
                    i < dialogueVoiceLines.Length && dialogueVoiceLines[i] != null)
                {
                    audioSource.clip = dialogueVoiceLines[i];
                    audioSource.Play();
                }

                // Typewriter effect
                yield return StartCoroutine(TypewriterText(dialogueLines[i]));

                // Stop mouth animation
                StopMouthAnimation();

                // Wait before next line (or for voice to finish)
                yield return new WaitForSeconds(2f);
            }

            OnDialogueComplete?.Invoke();

            // Trigger next animation state
            if (animator != null)
            {
                animator.SetTrigger("DialogueDone");
            }
        }

        private IEnumerator TypewriterText(string text)
        {
            if (dialogueText == null) yield break;

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

        /// <summary>
        /// Called by Animation Event when intro is fully complete.
        /// </summary>
        public void OnIntroEnd()
        {
            Debug.Log("Intro complete!");
            isPlaying = false;
            OnIntroComplete?.Invoke();
        }

        /// <summary>
        /// Skip the intro animation immediately.
        /// </summary>
        public void SkipIntro()
        {
            StopAllCoroutines();
            StopMouthAnimation();

            if (animator != null)
            {
                animator.Play("Intro_TransitionToPuzzle", 0, 1f); // Jump to end
            }

            isPlaying = false;
            OnIntroComplete?.Invoke();
        }
    }
}