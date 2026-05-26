using UnityEngine;
using UnityEngine.Events;

namespace Project
{
    public class ToggleEvents : MonoBehaviour
    {
        [SerializeField] private bool isOn;
        [SerializeField] private UnityEvent OnTurnedOn;
        [SerializeField] private UnityEvent OnTurnedOff;

        public void Toggle()
        {
            if (isOn) TurnOff();
            else TurnOn();
        }

        public void TurnOn()
        {
            OnTurnedOn.Invoke();
            isOn = true;
        }

        public void TurnOff()
        {
            OnTurnedOff.Invoke();
            isOn = false;
        }
    }
}
