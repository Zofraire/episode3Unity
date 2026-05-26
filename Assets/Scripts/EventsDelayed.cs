using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Project
{
    public class EventsDelayed : MonoBehaviour
    {
        [SerializeField] private bool startOnEnable;
        [SerializeField] private float delayInSeconds;
        [SerializeField] private UnityEvent OnTimeElapsed;

        private void OnEnable()
        {
            if (!startOnEnable) return;

            StartCountdown();
        }

        public void StartCountdown()
        {
            StopAllCoroutines();
            StartCoroutine(Countdown());
        }

        private IEnumerator Countdown()
        {
            yield return new WaitForSeconds(delayInSeconds);
            OnTimeElapsed.Invoke();
        }
    }
}
