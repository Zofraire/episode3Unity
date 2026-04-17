using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Project
{
    public class ColorController : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private float fadeDurationSec;

        public void AddRGB(float percent)
        {
            Color imageColor = image.color;
            imageColor += new Color(percent, percent, percent, 0);
            image.color = imageColor;
        }

        public void AddRGBA(float percent)
        {
            Color imageColor = image.color;
            imageColor += new Color(percent, percent, percent, percent);
            image.color = imageColor;
        }

        public void AddAlpha(float percent)
        {
            Color imageColor = image.color;
            imageColor.a += percent;
            image.color = imageColor;
        }
    }
}
