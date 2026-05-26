using System;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Figures
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        [Header("Parameters")]
        public ItemType targetType;
        public int targetPoints;
        public int currentPoints;
        [Header("Events")]
        public UnityEvent OnAllPointsCollected;
        public static event Action OnPointsIncrease;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
        }

        private void OnEnable()
        {
            Item.OnCorrect += AddPoint;
        }

        private void OnDisable()
        {
            Item.OnCorrect -= AddPoint;
        }

        private void AddPoint(ItemType itemType)
        {
            currentPoints++;
            OnPointsIncrease.Invoke();

            if (currentPoints >= targetPoints)
            {
                OnAllPointsCollected.Invoke();
            }
        }
    }
}
