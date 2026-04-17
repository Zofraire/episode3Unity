using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Project.Emotions
{
    public class EmotionsLVLTwo : MonoBehaviour
    {
        [Header("Params")]
        [SerializeField] private PaperContent[] papers;
        [SerializeField] private float moveForce;
        [SerializeField] private float holdingLinearDamping = 5;
        [SerializeField] private float fallingLinearDamping = 0;
        private bool canFollowTarget;
        private int currentPaperIndex;
        private bool isHoldingPaper;
        private Vector2 targetPosition;
        private bool isVoiceTalking;
        [Header("References")]
        [SerializeField] private TextMeshProUGUI paperText;
        [SerializeField] private GameObject normalPaper;
        [SerializeField] private Rigidbody2D crumpledPaper;
        [SerializeField] private Transform scoreParent;
        [SerializeField] private AudioSource voiceSource;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameObject tip1;
        [SerializeField] private GameObject tip2;
        [Header("Actions")]
        [SerializeField] private InputActionReference mousePosition;
        [Header("Events")]
        [SerializeField] private UnityEvent OnNewPaper;
        [SerializeField] private UnityEvent OnFinish;
        [SerializeField] private UnityEvent OnCorrectPhysical;
        [SerializeField] private UnityEvent OnCorrectVerbal;
        [SerializeField] private UnityEvent OnCorrectSocial;
        [SerializeField] private UnityEvent OnCorrectOnline;
        [SerializeField] private UnityEvent OnWrongPhysical;
        [SerializeField] private UnityEvent OnWrongVerbal;
        [SerializeField] private UnityEvent OnWrongSocial;
        [SerializeField] private UnityEvent OnWrongOnline;

        private void Update()
        {
            if (isHoldingPaper) targetPosition = mousePosition.action.ReadValue<Vector2>();
            else if (canFollowTarget && targetPosition == (Vector2)normalPaper.transform.position && Vector3.Distance(crumpledPaper.transform.position, targetPosition) < 50) SwitchPaper();

            if (isVoiceTalking)
            {
                if (voiceSource.isPlaying) canvasGroup.interactable = false;
                else
                {
                    isVoiceTalking = false;
                    canvasGroup.interactable = true;
                }
            }
        }

        private void FixedUpdate()
        {
            if (!crumpledPaper.gameObject.activeSelf || !canFollowTarget) return;
            crumpledPaper.AddForce((targetPosition - (Vector2)crumpledPaper.transform.position) * moveForce, ForceMode2D.Force);
        }

        public void PickOrDrop()
        {
            isHoldingPaper = !isHoldingPaper;
            canFollowTarget = isHoldingPaper;
            crumpledPaper.linearDamping = canFollowTarget ? holdingLinearDamping : fallingLinearDamping;
        }

        public void ChoosePhysical()
        {
            CheckAnswer(AbuseType.Physical);
        }

        public void ChooseVerbal()
        {
            CheckAnswer(AbuseType.Verbal);
        }

        public void ChooseSocial()
        {
            CheckAnswer(AbuseType.Social);
        }

        public void ChooseOnline()
        {
            CheckAnswer(AbuseType.Online);
        }

        private void CheckAnswer(AbuseType chosenAbuseType)
        {
            isHoldingPaper = false;
            crumpledPaper.GetComponent<Image>().raycastTarget = false;
            crumpledPaper.linearDamping = holdingLinearDamping;

            if (chosenAbuseType == papers[currentPaperIndex].abuseType)
            {
                switch (chosenAbuseType)
                {
                    case AbuseType.Physical:
                        OnCorrectPhysical.Invoke();
                        break;
                    case AbuseType.Verbal:
                        OnCorrectVerbal.Invoke();
                        break;
                    case AbuseType.Social:
                        OnCorrectSocial.Invoke();
                        break;
                    case AbuseType.Online:
                        OnCorrectOnline.Invoke();
                        break;
                    default:
                        break;
                }

                scoreParent.GetChild(currentPaperIndex).gameObject.SetActive(true);
                currentPaperIndex++;

                if (currentPaperIndex >= papers.Length)
                {
                    OnFinish.Invoke();
                    PlayerPrefs.SetInt("levelsUnlocked", 3);
                    PlayerPrefs.Save();
                }
                else if (currentPaperIndex == Mathf.CeilToInt(papers.Length * 0.3f))
                {
                    tip1.SetActive(true);
                }
                else if (currentPaperIndex == Mathf.CeilToInt(papers.Length * 0.6f))
                {
                    tip2.SetActive(true);
                }
                else
                {
                    NewPaper();
                }
            }
            else
            {
                switch (chosenAbuseType)
                {
                    case AbuseType.Physical:
                        OnWrongPhysical.Invoke();
                        break;
                    case AbuseType.Verbal:
                        OnWrongVerbal.Invoke();
                        break;
                    case AbuseType.Social:
                        OnWrongSocial.Invoke();
                        break;
                    case AbuseType.Online:
                        OnWrongOnline.Invoke();
                        break;
                    default:
                        break;
                }

                canFollowTarget = true;
                targetPosition = normalPaper.transform.position;
            }
        }

        public void UpdatePaper()
        {
            paperText.text = papers[currentPaperIndex].text;
            voiceSource.clip = papers[currentPaperIndex].voiceLine;
            voiceSource.Play();
            isVoiceTalking = true;
        }

        public void SwitchPaper()
        {
            normalPaper.SetActive(!normalPaper.activeSelf);

            crumpledPaper.gameObject.SetActive(!normalPaper.activeSelf);
            crumpledPaper.transform.position = normalPaper.transform.position;
            crumpledPaper.linearDamping = holdingLinearDamping;
            crumpledPaper.GetComponent<Image>().raycastTarget = true;

            canFollowTarget = crumpledPaper.gameObject.activeSelf;
            isHoldingPaper = crumpledPaper.gameObject.activeSelf;
        }

        public void NewPaper()
        {
            StopAllCoroutines();
            StartCoroutine(LoadingNewPaper(1));
        }

        public enum AbuseType
        {
            Physical, Verbal, Social, Online
        }

        [Serializable]
        public class PaperContent
        {
            public string text;
            public AudioClip voiceLine;
            public AbuseType abuseType;
        }

        private IEnumerator LoadingNewPaper(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            UpdatePaper();
            SwitchPaper();
            OnNewPaper.Invoke();
        }
    }
}
