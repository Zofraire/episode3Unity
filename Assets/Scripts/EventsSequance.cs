using UnityEngine;
using UnityEngine.Events;

namespace Project
{
    public class EventsSequence : MonoBehaviour
    {
        [SerializeField] private UnityEvent[] sequence;
        [SerializeField] private bool isLooped;
        private int currentIndex;

        public void Next()
        {
            if (currentIndex >= sequence.Length)
            {
                if (isLooped) currentIndex = 0;
                else return;
            }

            sequence[currentIndex].Invoke();
            currentIndex++;
        }
    }
}
