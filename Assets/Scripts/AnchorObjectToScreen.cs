using UnityEngine;

namespace Project
{
    public class AnchorObjectToScreen : MonoBehaviour
    {
        [SerializeField] private Camera uiCam;
        [SerializeField] private Vector3 offset;

        void Update()
        {
            transform.position = uiCam.ViewportToWorldPoint(offset);
        }
    }
}
