using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Project.Emotions
{
    public class EmotionsLVLThree : MonoBehaviour
    {
        [Header("Params")]
        [SerializeField] private bool shuffleOnStart;
        [SerializeField] private int pairsAmount;
        [SerializeField] private CanvasGroup cardsGroup;
        [SerializeField] private AudioSource audioSource;
        private int currentScore;
        private GameObject firstCard;
        private GameObject secondCard;
        private bool isFirstCardOpen;
        private bool once = true;
        [Header("Tip")]
        [SerializeField] private GameObject tip;
        [SerializeField] private Image tipImage;
        [SerializeField] private TextMeshProUGUI tipText;
        [Space()]
        [SerializeField] private string selfAcceptance;
        [SerializeField] private AudioClip selfAcceptanceClip;
        [SerializeField] private Sprite selfAcceptanceSprite;
        [Space()]
        [SerializeField] private string selfConscious;
        [SerializeField] private AudioClip selfConsciousClip;
        [SerializeField] private Sprite selfConsciousSprite;
        [Space()]
        [SerializeField] private string beJust;
        [SerializeField] private AudioClip beJustClip;
        [SerializeField] private Sprite beJustSprite;
        [Space()]
        [SerializeField] private string tolerance;
        [SerializeField] private AudioClip toleranceClip;
        [SerializeField] private Sprite toleranceSprite;
        [Space()]
        [SerializeField] private string interests;
        [SerializeField] private AudioClip interestsClip;
        [SerializeField] private Sprite interestsSprite;
        [Header("Events")]
        [SerializeField] private UnityEvent OnCorrect;
        [SerializeField] private UnityEvent OnWrong;
        [SerializeField] private UnityEvent OnFinish;

        private void Start()
        {
            if (shuffleOnStart) ShuffleCards();
        }

        private void Update()
        {
            if (once && currentScore >= pairsAmount && !tip.activeSelf)
            {
                once = false;
                OnFinish.Invoke();
            }
        }

        public void SelectCard(GameObject selectedCard)
        {
            if (firstCard == null)
            {
                firstCard = selectedCard;
                firstCard.GetComponent<Animator>().SetTrigger("Flip");
                return;
            }

            if (selectedCard == firstCard) return;

            secondCard = selectedCard;
            secondCard.GetComponent<Animator>().SetTrigger("Flip");

            cardsGroup.interactable = false;
        }

        private void CompareCards()
        {
            if (!firstCard.transform.GetChild(0).GetChild(0).name.Equals(secondCard.transform.GetChild(0).GetChild(0).name)) Wrong();
            else Correct();

            isFirstCardOpen = false;
            firstCard = secondCard = null;
            cardsGroup.interactable = true;
        }

        public void SetAsOpen()
        {
            if (!isFirstCardOpen)
            {
                isFirstCardOpen = true;
                return;
            }

            CompareCards();
        }

        private void Wrong()
        {
            firstCard.GetComponent<Animator>().SetTrigger("Flip");
            secondCard.GetComponent<Animator>().SetTrigger("Flip");
            OnWrong.Invoke();
        }

        private void Correct()
        {
            firstCard.GetComponent<Animator>().SetBool("Is Correct", true);
            secondCard.GetComponent<Animator>().SetBool("Is Correct", true);

            ShowTip();

            OnCorrect.Invoke();
            currentScore++;
        }

        private void ShuffleCards()
        {
            for (int i = 0; i < cardsGroup.transform.childCount; i++)
            {
                cardsGroup.transform.GetChild(Random.Range(0, cardsGroup.transform.childCount)).SetAsLastSibling();
            }
        }

        private void ShowTip()
        {
            tip.SetActive(true);
            switch (secondCard.transform.GetChild(0).GetChild(0).name)
            {
                case "Self Acceptance":
                    tipText.text = selfAcceptance;
                    audioSource.clip = selfAcceptanceClip;
                    tipImage.sprite = selfAcceptanceSprite;
                    audioSource.Play();
                    break;
                case "Self Conscious":
                    tipText.text = selfConscious;
                    audioSource.clip = selfConsciousClip;
                    tipImage.sprite = selfConsciousSprite;
                    audioSource.Play();
                    break;
                case "Just":
                    tipText.text = beJust;
                    audioSource.clip = beJustClip;
                    tipImage.sprite = beJustSprite;
                    audioSource.Play();
                    break;
                case "Tolerance":
                    tipText.text = tolerance;
                    audioSource.clip = toleranceClip;
                    tipImage.sprite = toleranceSprite;
                    audioSource.Play();
                    break;
                case "Interests":
                    tipText.text = interests;
                    audioSource.clip = interestsClip;
                    tipImage.sprite = interestsSprite;
                    audioSource.Play();
                    break;
                default:
                    break;
            }
        }
    }
}
