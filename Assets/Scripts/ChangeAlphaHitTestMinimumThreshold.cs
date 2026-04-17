using UnityEngine;

namespace Project
{
    public class ChangeAlphaHitTestMinimumThreshold : MonoBehaviour
    {
        void OnEnable()
        {
            GetComponent<UnityEngine.UI.Image>().alphaHitTestMinimumThreshold = 0.1f;
        }
    }
}
