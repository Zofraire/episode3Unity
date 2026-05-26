using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Figures
{
    public class ItemEvents : MonoBehaviour
    {
        [SerializeField] private ItemType itemToReactTo;
        [SerializeField] private float duration;
        [SerializeField] private UnityEvent OnActivated;
        [SerializeField] private UnityEvent OnDeactivated;

        private void OnEnable()
        {
            Item.OnCorrect += Activate;
            Item.OnSpecial += Activate;
        }

        private void OnDisable()
        {
            Item.OnCorrect -= Activate;
            Item.OnSpecial -= Activate;
        }

        private void Activate(ItemType itemType)
        {
            if (itemType != itemToReactTo) return;

            StopAllCoroutines();

            StartCoroutine(Activation());
        }

        private IEnumerator Activation()
        {
            OnActivated.Invoke();

            yield return new WaitForSeconds(duration);

            OnDeactivated.Invoke();
        }
    }
}
