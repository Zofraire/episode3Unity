using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace Project
{
    public class VideoController : MonoBehaviour
    {
        [SerializeField] private bool onlyOneLocale = true;
        [SerializeField] private UnityEngine.Localization.LocaleIdentifier playOnLocale;
        [SerializeField] private VideoPlayer player;
        [SerializeField] private UnityEvent OnPlay;
        [SerializeField] private UnityEvent OnEnd;

        private void OnEnable()
        {
            player.loopPointReached += (player) => OnEnd.Invoke();
        }

        public void Play(string clipName)
        {
            if (onlyOneLocale)
            {
                if (!UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale.Identifier.Equals(playOnLocale)) return;
            }

            string clipPath = System.IO.Path.Combine(Application.streamingAssetsPath + "/" + clipName + ".mp4");
            player.url = clipPath;
            player.Play();
            OnPlay.Invoke();
        }

        public void PlayOnline(string clipName)
        {
            player.url = clipName;
            OnPlay.Invoke();
        }
    }
}
