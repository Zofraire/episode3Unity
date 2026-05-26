using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class DragDropUI : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("General")]
    public string id; // Unique ID for draggable items
    public bool isDraggable;

    [Header("Drag Settings")]
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 startPos;

    [Header("Target Settings (Character)")]
    public string correctItemID;
    public Image defaultImage;
    public Image alternateImage;

    [Header("Timeline Settings")]
    public PlayableDirector director;     // Shared director
    public TimelineAsset shortTimeline;   // Shared short timeline before each character
    public TimelineAsset timelineAsset;   // Character-specific timeline

    [Header("Final Timeline & UI")]
    public TimelineAsset finalTimelineAsset;
    public int totalRequired = 3; // number of items to place
    public GameObject endWindow;  // <--- ADDED END WINDOW HERE

    [Header("Feedback")]
    public AudioClip incorrectClip;
    private static AudioSource audioSource;

    private bool isCompleted = false;
    private static int correctCount = 0;
    private static bool timelinePlaying = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        startPos = rectTransform.anchoredPosition;

        // Setup shared AudioSource
        if (audioSource == null)
        {
            GameObject go = new GameObject("AudioSourceDragDrop");
            audioSource = go.AddComponent<AudioSource>();
        }
    }

    // ---------------- DRAG ----------------
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable || timelinePlaying) return;
        startPos = rectTransform.position;
        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable || timelinePlaying) return;
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable || timelinePlaying) return;
        rectTransform.position = startPos;
        canvasGroup.blocksRaycasts = true;
    }

    // ---------------- DROP ----------------
    public void OnDrop(PointerEventData eventData)
    {
        if (isDraggable || isCompleted || timelinePlaying) return;

        DragDropUI dragged = eventData.pointerDrag?.GetComponent<DragDropUI>();
        if (dragged != null && dragged.isDraggable)
        {
            if (dragged.id == correctItemID)
            {
                isCompleted = true;
                dragged.gameObject.SetActive(false);

                // Swap images
                if (defaultImage) defaultImage.enabled = false;
                if (alternateImage) alternateImage.enabled = true;

                correctCount++;

                // Play short timeline + character timeline + (final timeline if last)
                if (director != null)
                    StartCoroutine(PlayTimelinesAndFinal());
            }
            else
            {
                // Incorrect drop
                if (incorrectClip != null && audioSource != null)
                    audioSource.PlayOneShot(incorrectClip);
            }
        }
    }

    // ---------------- TIMELINE COROUTINE ----------------
    private IEnumerator PlayTimelinesAndFinal()
    {
        timelinePlaying = true;
        SetAllItemsInteractable(false);

        // 1️⃣ Play short timeline
        if (shortTimeline != null)
        {
            director.playableAsset = shortTimeline;
            director.Play();
            while (director.state == PlayState.Playing)
                yield return null;
        }

        // 2️⃣ Play character-specific timeline
        if (timelineAsset != null)
        {
            director.playableAsset = timelineAsset;
            director.Play();
            while (director.state == PlayState.Playing)
                yield return null;
        }

        // 3️⃣ Play final timeline if all correct
        if (correctCount >= totalRequired)
        {
            if (finalTimelineAsset != null)
            {
                director.playableAsset = finalTimelineAsset;
                director.Play();
                while (director.state == PlayState.Playing)
                    yield return null;
            }

            // <--- TRIGGER ENDING HERE --->
            OnEndingComplete();
            yield break; // Stop the coroutine here so we don't re-enable interactions
        }

        timelinePlaying = false;
        SetAllItemsInteractable(true);
    }

    // ---------------- COMPLETION LOGIC ----------------
    public void OnEndingComplete()
    {
        // Change the "4" to whatever level number this game actually unlocks!
        PlayerPrefs.SetInt("levelsUnlocked", 4); 
        PlayerPrefs.Save();
        Debug.Log("Game Complete!");

        if (endWindow != null)
        {
            endWindow.SetActive(true); 
        }
    }

    // ---------------- INTERACTABILITY ----------------
    void SetAllItemsInteractable(bool interactable)
    {
        DragDropUI[] allItems = FindObjectsOfType<DragDropUI>();
        foreach (var item in allItems)
        {
            if (item.isDraggable && !item.isCompleted)
            {
                if (item.canvasGroup != null)
                    item.canvasGroup.blocksRaycasts = interactable;
            }
        }
    }
}