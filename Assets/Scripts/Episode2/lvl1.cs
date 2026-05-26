using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project
{
    public class lvl1 : MonoBehaviour
    {
        [Header("Puzzle Area")]
        [SerializeField] private GameObject puzzleArea;  // Reference to PuzzleArea - will be activated when puzzle starts

        [Header("Stages")]
        [SerializeField] private Stage[] stages;
        [SerializeField] private int currentStageIndex;

        [Header("Backgrounds")]
        [SerializeField] private Image stageBackgroundImage;

        [Header("Cutscene Animator (Single Animator on Canvas)")]
        [SerializeField] private Animator cutsceneAnimator;
        [SerializeField] private string introStateName = "Intro";
        [SerializeField] private string endingStateName = "Ending";

        [Header("Events")]
        [SerializeField] private UnityEvent OnGameComplete;

        private Transform draggableObject;
        private Transform parentAfterDrag;
        private bool isStageActive = false;

        // =====================================
        // CUTSCENE FLOW METHODS
        // Call these from Animation Events
        // =====================================

        /// <summary>
        /// Call from Intro Animation Event when intro finishes.
        /// Starts Stage 1.
        /// </summary>
        public void OnIntroComplete()
        {
            StartStage(0);
        }

        /// <summary>
        /// Call from Mid Cutscene Animation Event when cutscene finishes.
        /// Starts the next stage.
        /// </summary>
        public void OnMidCutsceneComplete()
        {
            currentStageIndex++;
            StartStage(currentStageIndex);
        }

        /// <summary>
        /// Call from Ending Animation Event when ending finishes.
        /// </summary>
        public void OnEndingComplete()
        {
            //OnGameComplete?.Invoke();
            PlayerPrefs.SetInt("levelsUnlocked", 2);
            PlayerPrefs.Save();
            Debug.Log("Game Complete!");
        }

        // =====================================
        // STAGE MANAGEMENT
        // =====================================

        /// <summary>
        /// Starts a specific stage
        /// </summary>
        public void StartStage(int stageIndex)
        {
            if (stageIndex >= stages.Length) return;

            currentStageIndex = stageIndex;

            // Activate puzzle area if assigned
            if (puzzleArea != null)
            {
                puzzleArea.SetActive(true);
            }

            // Activate this stage's container if assigned
            if (stages[currentStageIndex].stageContainer != null)
            {
                // Deactivate all stage containers first
                foreach (var stage in stages)
                {
                    if (stage.stageContainer != null)
                        stage.stageContainer.SetActive(false);
                }
                // Activate current stage container
                stages[currentStageIndex].stageContainer.SetActive(true);
            }

            // Change background if specified
            if (stageBackgroundImage != null && stages[currentStageIndex].backgroundSprite != null)
            {
                stageBackgroundImage.sprite = stages[currentStageIndex].backgroundSprite;
            }

            // Reset placed count
            stages[currentStageIndex].placedCorrectlyAmount = 0;

            // Start the preview coroutine
            StartCoroutine(PreviewThenScatter());
        }

        private IEnumerator PreviewThenScatter()
        {
            // Phase 1: Show pieces at their target positions (preview)
            foreach (var piece in stages[currentStageIndex].pieces)
            {
                if (piece != null && piece.pieceObject != null && piece.targetSlot != null)
                {
                    piece.pieceObject.SetActive(true);
                    // Position at target slot
                    piece.pieceObject.transform.SetParent(piece.targetSlot.transform);
                    piece.pieceObject.transform.localPosition = Vector3.zero;

                    // Disable interaction during preview
                    var selectable = piece.pieceObject.GetComponent<Selectable>();
                    if (selectable != null)
                        selectable.interactable = false;
                    var image = piece.pieceObject.GetComponent<Image>();
                    if (image != null)
                        image.raycastTarget = false;
                }
            }

            // Wait for preview duration
            yield return new WaitForSeconds(previewDuration);

            // Phase 2: Scatter pieces randomly across the screen
            foreach (var piece in stages[currentStageIndex].pieces)
            {
                if (piece != null && piece.pieceObject != null)
                {
                    // Move to pieces container (or canvas root)
                    Transform scatterParent = stages[currentStageIndex].piecesContainer != null
                        ? stages[currentStageIndex].piecesContainer.transform
                        : piece.pieceObject.transform.root;

                    piece.pieceObject.transform.SetParent(scatterParent);

                    // Scatter to random position
                    if (scatterArea != null)
                    {
                        Vector2 randomPos = GetRandomPositionInArea(scatterArea);
                        piece.pieceObject.GetComponent<RectTransform>().position = randomPos;
                    }

                    // Enable interaction
                    var selectable = piece.pieceObject.GetComponent<Selectable>();
                    if (selectable != null)
                        selectable.interactable = true;
                    var image = piece.pieceObject.GetComponent<Image>();
                    if (image != null)
                        image.raycastTarget = true;
                }
            }

            // Now the stage is active for playing
            isStageActive = true;
        }

        private Vector2 GetRandomPositionInArea(RectTransform area)
        {
            Vector3[] corners = new Vector3[4];
            area.GetWorldCorners(corners);

            // corners[0] = bottom-left, corners[2] = top-right
            float minX = corners[0].x;
            float maxX = corners[2].x;
            float minY = corners[0].y;
            float maxY = corners[2].y;

            // Add some padding so pieces don't spawn at the very edge
            float padding = 50f;
            minX += padding;
            maxX -= padding;
            minY += padding;
            maxY -= padding;

            float randomX = UnityEngine.Random.Range(minX, maxX);
            float randomY = UnityEngine.Random.Range(minY, maxY);

            return new Vector2(randomX, randomY);
        }

        /// <summary>
        /// Called when all pieces are placed correctly
        /// </summary>
        private void OnStageCompleted()
        {
            isStageActive = false;

            // Invoke stage complete event
            stages[currentStageIndex].OnStageComplete?.Invoke();

            // Determine which cutscene to play
            bool isLastStage = (currentStageIndex >= stages.Length - 1);

            if (cutsceneAnimator == null)
            {
                // No animator, just advance
                if (isLastStage)
                    OnEndingComplete();
                else
                    OnMidCutsceneComplete();
                return;
            }

            if (isLastStage)
            {
                // Play ending cutscene
                cutsceneAnimator.Play(endingStateName);
            }
            else
            {
                // Play mid cutscene for this stage
                if (!string.IsNullOrEmpty(stages[currentStageIndex].midCutsceneStateName))
                {
                    cutsceneAnimator.Play(stages[currentStageIndex].midCutsceneStateName);
                }
                else
                {
                    // No cutscene state defined, go directly to next stage
                    OnMidCutsceneComplete();
                }
            }
        }

        // =====================================
        // DRAG AND DROP
        // =====================================

        public void BeginDrag(BaseEventData data)
        {
            if (!isStageActive) return;

            draggableObject = EventSystem.current.currentSelectedGameObject.transform;
            draggableObject.GetComponent<Image>().raycastTarget = false;
            parentAfterDrag = draggableObject.parent;
            draggableObject.SetParent(draggableObject.root);
            draggableObject.SetAsLastSibling();

            // Disable raycast on ALL pieces so target slots can receive drops
            DisableAllPiecesRaycast();

            // Only enable raycast on the matching target slot
            EnableOnlyMatchingTargetSlot(draggableObject.gameObject.name);
        }

        public void Drag(BaseEventData data)
        {
            if (!isStageActive || draggableObject == null) return;

            PointerEventData pointerData = (PointerEventData)data;
            draggableObject.position = pointerData.position;
        }

        [Header("Drop Settings")]
        [SerializeField] private float snapDistance = 100f;  // How close piece must be to target to snap

        [Header("Preview Settings")]
        [SerializeField] private float previewDuration = 2f;  // How long to show pieces at target positions
        [SerializeField] private RectTransform scatterArea;   // Area where pieces scatter (assign Canvas or a panel)

        public void EndDrag(BaseEventData data)
        {
            if (!isStageActive || draggableObject == null) return;
            // Re-enable raycast on all pieces and target slots
            EnableAllPiecesRaycast();
            EnableAllTargetSlotsRaycast();

            // Find the matching piece config
            var currentPiece = Array.Find(stages[currentStageIndex].pieces, p =>
                p.pieceObject != null && p.pieceObject.name == draggableObject.gameObject.name);

            // Check distance to target slot (ignore raycast completely)
            bool isCloseToTarget = false;
            if (currentPiece != null && currentPiece.targetSlot != null)
            {
                float distance = Vector3.Distance(draggableObject.position, currentPiece.targetSlot.transform.position);
                isCloseToTarget = distance < snapDistance;
            }

            if (!isCloseToTarget)
            {
                // Not close enough - return to original position
                draggableObject.SetParent(parentAfterDrag);
                draggableObject.GetComponent<Image>().raycastTarget = true;
                return;
            }

            // Piece placed correctly!
            stages[currentStageIndex].placedCorrectlyAmount++;
            currentPiece?.OnCorrect?.Invoke();

            // Snap to target slot
            draggableObject.SetParent(currentPiece.targetSlot.transform);
            draggableObject.localPosition = Vector3.zero;

            draggableObject.GetComponent<Selectable>().interactable = false;
            draggableObject.GetComponent<Image>().raycastTarget = false;

            if (stages[currentStageIndex].placedCorrectlyAmount == stages[currentStageIndex].pieces.Length)
            {
                OnStageCompleted();
            }
        }

        private void DisableAllPiecesRaycast()
        {
            // Disable raycast on ALL pieces so target slots can receive drops
            foreach (var piece in stages[currentStageIndex].pieces)
            {
                if (piece.pieceObject != null)
                {
                    var img = piece.pieceObject.GetComponent<Image>();
                    if (img != null)
                        img.raycastTarget = false;
                }
            }
        }

        private void EnableAllPiecesRaycast()
        {
            foreach (var piece in stages[currentStageIndex].pieces)
            {
                if (piece.pieceObject != null)
                {
                    var selectable = piece.pieceObject.GetComponent<Selectable>();
                    // Only re-enable if piece is still interactable (not placed yet)
                    if (selectable != null && selectable.interactable)
                    {
                        var img = piece.pieceObject.GetComponent<Image>();
                        if (img != null)
                            img.raycastTarget = true;
                    }
                }
            }
        }

        private void EnableOnlyMatchingTargetSlot(string pieceName)
        {
            // Disable all target slots, then enable only the matching one
            foreach (var piece in stages[currentStageIndex].pieces)
            {
                if (piece.targetSlot != null)
                {
                    var img = piece.targetSlot.GetComponent<Image>();
                    if (img != null)
                    {
                        // Enable only if this target matches the piece being dragged
                        img.raycastTarget = (piece.pieceObject != null && piece.pieceObject.name == pieceName);
                    }
                }
            }
        }

        private void EnableAllTargetSlotsRaycast()
        {
            foreach (var piece in stages[currentStageIndex].pieces)
            {
                if (piece.targetSlot != null)
                {
                    var img = piece.targetSlot.GetComponent<Image>();
                    if (img != null)
                        img.raycastTarget = true;
                }
            }
        }
    }

    [Serializable]
    public class Stage
    {
        public string name;
        public GameObject stageContainer;     // The Stage1, Stage2, etc. container GameObject
        public GameObject piecesContainer;    // Where pieces go after preview (has Grid Layout Group)
        public Sprite backgroundSprite;
        public string midCutsceneStateName;   // Animator state name for cutscene after this stage
        public PuzzlePiece[] pieces;
        public UnityEvent OnStageComplete;
        public int placedCorrectlyAmount;
    }

    [Serializable]
    public class PuzzlePiece
    {
        public string name;                   // Name for matching with target slot
        public GameObject pieceObject;        // Direct reference to the piece GameObject
        public GameObject targetSlot;         // Direct reference to the target slot GameObject
        public UnityEvent OnCorrect;
    }
}