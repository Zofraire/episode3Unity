using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace Project.RPC
{
    public class RPCGameplay : MonoBehaviour
    {
        [SerializeField] private int maxRounds;
        [Header("Animations")]
        [SerializeField] private Animator mainAnimator;
        [SerializeField] private float countdownDuration, showdownDuration, resultDuration;

        [Header("Hands")]
        [SerializeField] private Image playerHand;
        [SerializeField] private Image aiHand;
        [SerializeField] private Sprite[] playerHands;
        [SerializeField] private Sprite[] aiHands;
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI rounds;
        [SerializeField] private Button rock;
        [SerializeField] private Button paper;
        [SerializeField] private Button scissors;    
        [Header("Events")]
        [SerializeField] private UnityEvent OnBegin;
        [SerializeField] private UnityEvent OnEnd;
        [SerializeField] private UnityEvent OnWin;
        [SerializeField] private UnityEvent OnDraw;
        [SerializeField] private UnityEvent OnLoss;
        [SerializeField] private UnityEvent OnFinish;
        private int currentRound;
        private int wins, draws, losses;

        private void Start()
        {
            rounds.text = $"{currentRound}/{maxRounds}";
            EnableButtons(false);
        }

        public void PlayRound(string playerChoice)
        {
            playerChoice = playerChoice.Trim().ToLower();

            string[] choices = { "rock", "paper", "scissors" };
            string aiChoice = choices[Random.Range(0, 3)];

            StopAllCoroutines();
            StartCoroutine(Round(playerChoice, aiChoice));
        }

        private void Win()
        {
            wins++;
            currentRound++;
            mainAnimator.Play("Win");
            OnWin.Invoke();
        }

        private void Draw()
        {
            draws++;
            mainAnimator.Play("Draw");
            OnDraw.Invoke();
        }

        private void Loss()
        {
            losses++;
            currentRound++;
            mainAnimator.Play("Loss");
            OnLoss.Invoke();
        }

        private void Finish()
        {
            //mainAnimator.Play("Finish");
            OnFinish.Invoke();
        }

        public void EnableButtons(bool enable)
        {
            rock.interactable = enable;
            paper.interactable = enable;
            scissors.interactable = enable;

            if (!enable)
            {
                rock.GetComponent<Animator>().StartPlayback();
                paper.GetComponent<Animator>().StartPlayback();
                scissors.GetComponent<Animator>().StartPlayback();
            }
            else
            {
                rock.GetComponent<Animator>().StopPlayback();
                paper.GetComponent<Animator>().StopPlayback();
                scissors.GetComponent<Animator>().StopPlayback();
            }
        }

        private IEnumerator Round(string playerChoice, string aiChoice)
        {
            OnBegin.Invoke();
            EnableButtons(false);

            mainAnimator.Play("Countdown");
            yield return new WaitForSeconds(countdownDuration);
            mainAnimator.Play("Showdown");

            if (playerChoice.Equals("rock"))
            {
                playerHand.sprite = playerHands[0];

                if (aiChoice.Equals("paper"))
                {
                    aiHand.sprite = aiHands[1];
                    yield return new WaitForSeconds(showdownDuration);
                    Loss();
                }
                else if (aiChoice.Equals("scissors"))
                {
                    aiHand.sprite = aiHands[2];
                    yield return new WaitForSeconds(showdownDuration);
                    Win();
                }
                else
                {
                    aiHand.sprite = aiHands[0];
                    yield return new WaitForSeconds(showdownDuration);
                    Draw();
                }
            }
            else if (playerChoice.Equals("paper"))
            {
                playerHand.sprite = playerHands[1];

                if (aiChoice.Equals("rock"))
                {
                    aiHand.sprite = aiHands[0];
                    yield return new WaitForSeconds(showdownDuration);
                    Win();
                }
                else if (aiChoice.Equals("scissors"))
                {
                    aiHand.sprite = aiHands[2];
                    yield return new WaitForSeconds(showdownDuration);
                    Loss();
                }
                else
                {
                    aiHand.sprite = aiHands[1];
                    yield return new WaitForSeconds(showdownDuration);
                    Draw();
                }
            }
            else
            {
                playerHand.sprite = playerHands[2];

                if (aiChoice.Equals("rock"))
                {
                    aiHand.sprite = aiHands[0];
                    yield return new WaitForSeconds(showdownDuration);
                    Loss();
                }
                else if (aiChoice.Equals("paper"))
                {
                    aiHand.sprite = aiHands[1];
                    yield return new WaitForSeconds(showdownDuration);
                    Win();
                }
                else
                {
                    aiHand.sprite = aiHands[2];
                    yield return new WaitForSeconds(showdownDuration);
                    Draw();
                }
            }

            yield return new WaitForSeconds(resultDuration);

            rounds.text = $"{currentRound}/{maxRounds}";
            playerHand.sprite = playerHands[0];
            aiHand.sprite = aiHands[0];
            mainAnimator.Play("Prepare");

            OnEnd.Invoke();

            yield return new WaitForSeconds(1);

            if (currentRound >= maxRounds)
            {
                Finish();
            }
            else
            {
                EnableButtons(true);
            }
        }
    }
}