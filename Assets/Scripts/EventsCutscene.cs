using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace Project
{
    public class EventsCutscene : MonoBehaviour
    {
        [SerializeField] private PlayableDirector playableDirector;
        [SerializeField] private UnityEvent OnCutsceneStart;
        [SerializeField] private UnityEvent OnCutsceneEnd;

        void OnEnable()
        {
            playableDirector.played += CutsceneStarted;
            playableDirector.stopped += CutsceneEnded;
        }

        void OnDisable()
        {
            playableDirector.played -= CutsceneStarted;
            playableDirector.stopped -= CutsceneEnded;
        }

        private void CutsceneStarted(PlayableDirector pd)
        {
            OnCutsceneStart.Invoke();
        }

        private void CutsceneEnded(PlayableDirector pd)
        {
            OnCutsceneEnd.Invoke();
        }
    }
}
