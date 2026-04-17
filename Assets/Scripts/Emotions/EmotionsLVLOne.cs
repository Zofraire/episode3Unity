using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.Emotions
{
    public class EmotionsLVLOne : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] private UnityEvent OnCorrectHeart;
        [SerializeField] private UnityEvent OnCorrectStar;
        [SerializeField] private UnityEvent OnCorrectLike;
        [SerializeField] private UnityEvent OnCorrectSmile;
        [SerializeField] private UnityEvent OnCorrectAll;
        private int correctStickers;
        private Transform draggableObject;
        private Transform parentAfterDrag;

        public void BeginDrag(BaseEventData data)
        {
            draggableObject = EventSystem.current.currentSelectedGameObject.transform;
            draggableObject.transform.localScale *= 1.3f;
            draggableObject.GetComponent<Image>().raycastTarget = false;
            parentAfterDrag = draggableObject.parent;
            draggableObject.SetParent(draggableObject.root);
            draggableObject.SetAsLastSibling();
        }

        public void Drag(BaseEventData data)
        {
            PointerEventData pointerData = (PointerEventData)data;
            draggableObject.position = pointerData.position;
        }

        public void EndDrag(BaseEventData data)
        {
            PointerEventData pointerData = (PointerEventData)data;

            draggableObject.transform.localScale /= 1.3f;

            GameObject objectUnderPointer = pointerData.pointerEnter;

            if (objectUnderPointer == null || !draggableObject.gameObject.name.Equals(objectUnderPointer.name))
            {
                draggableObject.SetParent(parentAfterDrag);
                draggableObject.GetComponent<Image>().raycastTarget = true;
                return;
            }

            parentAfterDrag = objectUnderPointer.transform;
            draggableObject.SetParent(parentAfterDrag);

            draggableObject.GetComponent<Selectable>().interactable = false;
            draggableObject.GetComponent<Image>().raycastTarget = false;

            switch (draggableObject.gameObject.name)
            {
                case "Heart":
                    correctStickers++;
                    OnCorrectHeart.Invoke();
                    break;
                case "Star":
                    correctStickers++;
                    OnCorrectStar.Invoke();
                    break;
                case "Like":
                    correctStickers++;
                    OnCorrectLike.Invoke();
                    break;
                case "Smile":
                    correctStickers++;
                    OnCorrectSmile.Invoke();
                    break;
                default:
                    break;
            }

            if (correctStickers >= 4)
            {
                OnCorrectAll.Invoke();
                PlayerPrefs.SetInt("levelsUnlocked", 2);
                PlayerPrefs.Save();
            }
        }
    }
}
