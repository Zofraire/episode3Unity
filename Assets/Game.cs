using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

public class lv3 : MonoBehaviour
{
    [SerializeField] private GameObject endWindow;
    [SerializeField] private GameObject Choices;
    [Header("Per-Button Feedback (Size = 4)")]
    [SerializeField] private GameObject[] correctObjects;
    [SerializeField] private GameObject[] incorrectObjects;

    [SerializeField] private UnityEvent onIntro;

    [SerializeField] private UnityEvent onQuiz_1Complete;
    [SerializeField] private UnityEvent onQuiz_2Complete;
    [SerializeField] private UnityEvent onQuiz_3Complete;

    [SerializeField] private UnityEvent onRound_1;
    [SerializeField] private UnityEvent onRound_2;
    [SerializeField] private UnityEvent onRound_3;

    public AudioSource correctSource;
    public AudioClip correctClip;

    public AudioSource incorrectSource;
    public AudioClip incorrectClip;


    [System.Serializable]
    public class Question
    {
        public string questionText;
        public string[] options = new string[4];
        public int correctAnswerIndex;
    }

    public TextMeshProUGUI questionText;
    public Button[] optionButtons;

    public List<Question> questions = new List<Question>();

    private int currentQuestionIndex = 0;

    public void RealBegin()
    {
        StartCoroutine(Begin());
    }
    private IEnumerator Begin()
    {
        onIntro.Invoke();
        yield return new WaitForSeconds(1f);
        LoadQuestion();
    }

    private IEnumerator Wait()
    {
        { yield return new WaitForSeconds(1f); }
        if (currentQuestionIndex >= questions.Count)
        {
            Invoke(nameof(LoadQuestion), 16f); ;
        }
        else { Invoke(nameof(LoadQuestion), 1f); };
        
        switch (currentQuestionIndex)
        {
            case 1:
                onQuiz_1Complete.Invoke();
                break;
            case 2:
                onQuiz_2Complete.Invoke();
                break;
            case 3:
                onQuiz_3Complete.Invoke();
                break;
            default:
                break;
        }
    }

    /// /////////////////////////////////////////////////////////////////////////////////////////////

    void LoadQuestion()
    {
        ResetFeedback();

        if (currentQuestionIndex >= questions.Count)
        {
            PlayerPrefs.SetInt("levelsUnlocked", 2);
            PlayerPrefs.Save();
            foreach (Button b in optionButtons)
                b.gameObject.SetActive(false);
            endWindow.SetActive(true);
            return;
        }

        Question q = questions[currentQuestionIndex];
        questionText.text = q.questionText;

        switch (currentQuestionIndex)
        {
            case 0:
                onRound_1.Invoke();
                break;
            case 1:
                onRound_2.Invoke();
                break;
            case 2:
                onRound_3.Invoke();
                break;
            default:
                break;
        }

        List<int> indexes = new List<int> { 0, 1, 2, 3 };
        Shuffle(indexes);

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int buttonIndex = i;
            int optionIndex = indexes[i];

            optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text =
                q.options[optionIndex];

            optionButtons[i].onClick.RemoveAllListeners();

            if (optionIndex == q.correctAnswerIndex)
            {
                optionButtons[i].onClick.AddListener(() => CorrectAnswer(buttonIndex));
            }
            else
            {
                optionButtons[i].onClick.AddListener(() => WrongAnswer(buttonIndex));
            }

            optionButtons[i].transform.SetSiblingIndex(Random.Range(0, 4));
        }
    }

    /// /////////////////////////////////////////////////////////////////////////////////////////////
 
    void CorrectAnswer(int buttonIndex)
    {
        correctObjects[buttonIndex].SetActive(true);
        incorrectObjects[buttonIndex].SetActive(false);

        correctSource.PlayOneShot(correctClip);

        DisableButtons();
        currentQuestionIndex++;
        StartCoroutine(Wait());
        

    }
    void WrongAnswer(int buttonIndex)
    {

        incorrectObjects[buttonIndex].SetActive(true);
        correctObjects[buttonIndex].SetActive(false);

        DisableButtons();
        Invoke(nameof(LoadQuestion), 1f);
        incorrectSource.PlayOneShot(incorrectClip);
    }


    /// /////////////////////////////////////////////////////////////////////////////////////////////

    void ResetFeedback()
    {
        Choices.SetActive(true);
        for (int i = 0; i < 4; i++)
        {
            correctObjects[i].SetActive(false);
            incorrectObjects[i].SetActive(false);
        }

        EnableButtons();
    }

    void DisableButtons()
    {
        foreach (Button b in optionButtons)
            b.interactable = false;
    }

    void EnableButtons()
    {
        foreach (Button b in optionButtons)
            b.interactable = true;
    }

    void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            int temp = list[i];
            list[i] = list[rnd];
            list[rnd] = temp;
        }
    }

}
