using UnityEngine;
using UnityEngine.Events;

namespace Project
{
    public class EventsTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEvent OnTriggerEntered;
        [SerializeField] private UnityEvent OnTriggerExited;

        void OnTriggerEnter2D(Collider2D collision)
        {
            OnTriggerEntered.Invoke();
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            OnTriggerExited.Invoke();
        }
    }
}
