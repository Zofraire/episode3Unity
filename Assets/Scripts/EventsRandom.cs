using UnityEngine;
using UnityEngine.Events;

namespace Project
{
    public class EventsRandom : MonoBehaviour
    {
        [SerializeField] private UnityEvent[] events;
        public void Activate()
        {
            int rndIndex = Random.Range(0, events.Length);
            events[rndIndex].Invoke();
        }
    }
}
