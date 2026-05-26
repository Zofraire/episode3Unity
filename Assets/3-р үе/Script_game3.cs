using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class QuizGameManager : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public GameObject questionObject;
        public Button optionAButton;
        public Button optionBButton;
        public bool isOptionACorrect;

        [HideInInspector] public CanvasGroup canvasGroup;
    }

    [Header("Questions")]
    public Question[] questions;

    [Header("Timeline Director")]
    public PlayableDirector director;

    [Header("Timeline Assets")]
    public PlayableAsset introTimeline;
    public PlayableAsset wrongTimeline;
    public PlayableAsset[] correctTimelines;
    public PlayableAsset outroTimeline;
    public PlayableAsset[] questionTimelines;

    [Header("End Game UI")]
    public GameObject endWindow; // <--- ADDED END WINDOW HERE

    private int currentQuestionIndex = 0;

    void Awake()
    {
        foreach (var q in questions)
        {
            if (!q.questionObject.TryGetComponent<CanvasGroup>(out var cg))
                cg = q.questionObject.AddComponent<CanvasGroup>();

            q.canvasGroup = cg;
            q.canvasGroup.alpha = 0;
            q.canvasGroup.interactable = false;
            q.canvasGroup.blocksRaycasts = false;
        }
    }

    void Start()
    {
        PlayTimeline(introTimeline, () => PlayQuestion(currentQuestionIndex));
    }

    void PlayQuestion(int index)
    {
        currentQuestionIndex = index;

        // Hide all questions first
        foreach (var q in questions)
        {
            q.canvasGroup.alpha = 0;
            q.canvasGroup.interactable = false;
            q.canvasGroup.blocksRaycasts = false;
        }

        // Play the question timeline first
        PlayTimeline(questionTimelines[index], () => ShowQuestion(questions[index]));
    }

    void ShowQuestion(Question question)
    {
        // Make question visible
        question.canvasGroup.alpha = 1;

        EnableButtons(question);
    }

    void EnableButtons(Question question)
    {
        question.optionAButton.interactable = true;
        question.optionBButton.interactable = true;

        question.canvasGroup.interactable = true;
        question.canvasGroup.blocksRaycasts = true;

        question.optionAButton.onClick.RemoveAllListeners();
        question.optionBButton.onClick.RemoveAllListeners();

        question.optionAButton.onClick.AddListener(() => OnAnswerSelected(question, true));
        question.optionBButton.onClick.AddListener(() => OnAnswerSelected(question, false));
    }

    void OnAnswerSelected(Question question, bool selectedA)
    {
        bool isCorrect = (selectedA == question.isOptionACorrect);

        // Hide question during timeline
        question.canvasGroup.alpha = 0;
        question.canvasGroup.interactable = false;
        question.canvasGroup.blocksRaycasts = false;

        question.optionAButton.interactable = false;
        question.optionBButton.interactable = false;

        if (isCorrect)
        {
            PlayTimeline(correctTimelines[currentQuestionIndex], () =>
            {
                int nextIndex = currentQuestionIndex + 1;
                if (nextIndex < questions.Length)
                    PlayQuestion(nextIndex);
                else
                    // <--- CHANGED THIS LINE to call OnEndingComplete when outro finishes --->
                    PlayTimeline(outroTimeline, OnEndingComplete); 
            });
        }
        else
        {
            // Wait until wrong timeline finishes before showing question again
            PlayTimeline(wrongTimeline, () => PlayQuestion(currentQuestionIndex));
        }
    }

    // ---------------- COMPLETION LOGIC ----------------
    public void OnEndingComplete()
    {
        // Change the "5" to whatever level number this game actually unlocks!
        PlayerPrefs.SetInt("levelsUnlocked", 5);
        PlayerPrefs.Save();
        Debug.Log("Quiz Complete!");

        if (endWindow != null)
        {
            endWindow.SetActive(true);
        }
    }

    void PlayTimeline(PlayableAsset timeline, System.Action onComplete)
    {
        director.playableAsset = timeline;
        director.Play();

        director.stopped -= OnTimelineStopped;
        director.stopped += OnTimelineStopped;

        void OnTimelineStopped(PlayableDirector d)
        {
            director.stopped -= OnTimelineStopped;
            onComplete?.Invoke();
        }
    }
}