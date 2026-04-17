using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace Project.Episode2
{
    /// <summary>
    /// Attach to each cutscene GameObject (with its own Animator).
    /// Call methods from Animation Events.
    /// Wire OnCutsceneComplete to the appropriate lvl1 method.
    /// </summary>
    public class CutsceneController : MonoBehaviour
    {
        [Header("Dialogue")]
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private float typingSpeed = 0.03f;

        [Header("Mouth Animation")]
        [SerializeField] private Image mouthImage;
        [SerializeField] private Sprite mouthOpen;
        [SerializeField] private Sprite mouthClosed;
        [SerializeField] private float mouthSpeed = 0.1f;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;

        [Header("Event - Wire to lvl1 method")]
        public UnityEvent OnCutsceneComplete;

        private Coroutine typingCoroutine;
        private Coroutine mouthCoroutine;
        private bool isTalking;

        // =====================================
        // ANIMATION EVENT METHODS
        // =====================================

        /// <summary>
        /// Display text with typewriter effect.
        /// </summary>
        public void ShowText(string text)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeText(text));
        }

        /// <summary>
        /// Display text instantly.
        /// </summary>
        public void SetText(string text)
        {
            if (dialogueText != null)
                dialogueText.text = text;
        }

        /// <summary>
        /// Clear dialogue text.
        /// </summary>
        public void ClearText()
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
            if (dialogueText != null)
                dialogueText.text = "";
        }

        /// <summary>
        /// Start mouth animation.
        /// </summary>
        public void StartTalking()
        {
            if (mouthImage == null) return;

            isTalking = true;
            if (mouthCoroutine != null)
                StopCoroutine(mouthCoroutine);
            mouthCoroutine = StartCoroutine(AnimateMouth());
        }

        /// <summary>
        /// Stop mouth animation.
        /// </summary>
        public void StopTalking()
        {
            isTalking = false;
            if (mouthCoroutine != null)
            {
                StopCoroutine(mouthCoroutine);
                mouthCoroutine = null;
            }
            if (mouthImage != null && mouthClosed != null)
                mouthImage.sprite = mouthClosed;
        }

        /// <summary>
        /// Play audio clip.
        /// </summary>
        public void PlayAudio(AudioClip clip)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }

        /// <summary>
        /// Call at the END of the cutscene animation.
        /// Wire OnCutsceneComplete to:
        /// - Intro: lvl1.OnIntroComplete
        /// - Mid cutscenes: lvl1.OnMidCutsceneComplete
        /// - Ending: lvl1.OnEndingComplete
        /// </summary>
        public void CutsceneFinished()
        {
            StopTalking();
            ClearText();
            OnCutsceneComplete?.Invoke();

            // Optionally deactivate this cutscene object
            gameObject.SetActive(false);
        }

        // =====================================
        // COROUTINES
        // =====================================

        private IEnumerator TypeText(string text)
        {
            if (dialogueText == null) yield break;

            dialogueText.text = "";
            foreach (char c in text)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        private IEnumerator AnimateMouth()
        {
            bool open = false;
            while (isTalking)
            {
                if (mouthImage != null)
                    mouthImage.sprite = open ? mouthOpen : mouthClosed;
                open = !open;
                yield return new WaitForSeconds(mouthSpeed);
            }
        }
    }
}