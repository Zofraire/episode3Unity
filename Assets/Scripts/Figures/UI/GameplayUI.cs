using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Figures
{
    public class GameplayUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Image targetItem;
        [SerializeField] private TextMeshProUGUI points;
        [Header("Referemces")]
        [SerializeField] private LevelManager levelManager;
        [SerializeField] private Sprite triangle;
        [SerializeField] private Sprite square;
        [SerializeField] private Sprite circle;

        private void OnEnable()
        {
            LevelManager.OnPointsIncrease += UpdatePoints;
        }

        private void OnDisable()
        {
            LevelManager.OnPointsIncrease -= UpdatePoints;
        }

        private void Start()
        {
            switch (levelManager.targetType)
            {
                case ItemType.Triangle:
                    targetItem.sprite = triangle;
                    break;
                case ItemType.Square:
                    targetItem.sprite = square;
                    break;
                case ItemType.Circle:
                    targetItem.sprite = circle;
                    break;
                default:
                    targetItem.sprite = null;
                    break;
            }

            UpdatePoints();
        }

        private void UpdatePoints()
        {
            points.text = $"{levelManager.currentPoints}/{levelManager.targetPoints}";
        }
    }
}
