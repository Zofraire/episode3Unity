using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class TimelineButtonController : MonoBehaviour
{
    public PlayableDirector director;

    // Individual timelines
    public PlayableAsset timeline_shirt;
    public PlayableAsset timeline_books;
    public PlayableAsset timeline_keys;
    public PlayableAsset timeline_pins;
    public PlayableAsset timeline_notes;
    public PlayableAsset timeline_stickers;
    public PlayableAsset timeline_pics;
    public PlayableAsset timeline_test;

    // Final timeline after all words are completed
    public PlayableAsset finalTimeline;
    [SerializeField] private GameObject endWindow;
    public Button[] buttons;          // Assign all 8 buttons here
    public GameObject[] strikeLines;  // Assign the 8 strike-through line images

    private bool[] completed = new bool[8]; // Tracks which words are completed
    private bool finalPlayed = false;       // Flag to know if final timeline played

    void Start()
    {
        // Disable buttons at start if intro timeline is playing
        SetButtons(false);

        // Subscribe to timeline stopped event
        director.stopped += OnTimelineStopped;
    }

    // Plays the timeline and marks the word as completed
    void PlayTimeline(PlayableAsset timeline, int index)
    {
        // Disable buttons while timeline plays
        SetButtons(false);

        // Strike-through the word if not already
        if (!completed[index])
        {
            strikeLines[index].SetActive(true);
            completed[index] = true;
        }

        // Play the timeline
        director.Stop();
        director.playableAsset = timeline;
        director.Play();
    }

    // Enables or disables all buttons
    void SetButtons(bool state)
    {
        foreach (Button b in buttons)
        {
            b.interactable = state;
        }
    }

    // Called when any timeline stops
    void OnTimelineStopped(PlayableDirector d)
    {
        // If final timeline already played, turn off everything
        if (finalPlayed)
        {
            OnEndingComplete();
            SetButtons(false);
            return;
        }

        // Enable buttons again
        SetButtons(true);

        // If all words are completed, play the final timeline
        if (AllCompleted())
        {
            PlayFinalTimeline();
        }
    }

    bool AllCompleted()
    {
        foreach (bool b in completed)
            if (!b) return false;
        return true;
    }

    void PlayFinalTimeline()
    {
        finalPlayed = true;
        SetButtons(false);

        director.Stop();
        director.playableAsset = finalTimeline;
        director.Play();
    }
    public void OnEndingComplete()
    {
        //OnGameComplete?.Invoke();
        PlayerPrefs.SetInt("levelsUnlocked", 2);
        PlayerPrefs.Save();
        Debug.Log("Game Complete!");
        if (endWindow != null)
        {
            endWindow.SetActive(true); 
        }
    }

    // Button click functions
    public void Shirt() { PlayTimeline(timeline_shirt, 0); }
    public void Books() { PlayTimeline(timeline_books, 1); }
    public void Keys() { PlayTimeline(timeline_keys, 2); }
    public void Pins() { PlayTimeline(timeline_pins, 3); }
    public void Notes() { PlayTimeline(timeline_notes, 4); }
    public void Stickers() { PlayTimeline(timeline_stickers, 5); }
    public void Pics() { PlayTimeline(timeline_pics, 6); }
    public void Test() { PlayTimeline(timeline_test, 7); }
}