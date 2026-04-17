using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace Project.Figures
{
    [RequireComponent(typeof(PlayableDirector))]
    public class CutsceneController : MonoBehaviour
    {
        public UnityEvent OnFinished;
        private PlayableDirector playableDirector;
        private bool playerOnPosition;

        private void Awake()
        {
            playableDirector = GetComponent<PlayableDirector>();
        }

        private void OnEnable()
        {
            Controls.OnCutscenePositionReached += () => playerOnPosition = true;
        }

        public void Play()
        {
            StopAllCoroutines();
            StartCoroutine(Cutscene());
        }

        private IEnumerator Cutscene()
        {
            while (!playerOnPosition) yield return null;

            playableDirector.Play();

            while (playableDirector.state == PlayState.Playing) yield return null;

            OnFinished.Invoke();
        }
    }
}
