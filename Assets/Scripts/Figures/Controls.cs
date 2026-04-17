using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Project.Figures
{
    public class Controls : MonoBehaviour
    {
        public bool canControl;
        [SerializeField] private InputAction pressAction;
        [SerializeField] private InputAction pressPosition;
        [SerializeField] private float speed;
        [SerializeField] private float minPositionX;
        [SerializeField] private float maxPositionX;
        [Header("Events")]
        [SerializeField] private UnityEvent OnStop;
        [SerializeField] private UnityEvent OnRight;
        [SerializeField] private UnityEvent OnLeft;
        private Animator animator;
        private float currentSpeed;
        private Vector3 direction;
        private bool hasPressed;
        public static event Action OnCutscenePositionReached;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            pressAction.Enable();
            pressPosition.Enable();
            currentSpeed = speed;
        }

        private void OnEnable()
        {
            pressAction.performed += OnPressDown;
            pressAction.canceled += OnPressUp;
        }

        private void OnDisable()
        {
            pressAction.performed -= OnPressDown;
            pressAction.canceled -= OnPressUp;
        }

        void Update()
        {
            if (!canControl) return;
            if (!hasPressed) return;

            Vector2 touchPosition = pressPosition.ReadValue<Vector2>();

            if (touchPosition.x < Screen.width / 2)
            {
                direction = Vector3.left;
                animator.SetInteger("Direction", -1);
                OnRight.Invoke();
            }
            else
            {
                direction = Vector3.right;
                animator.SetInteger("Direction", 1);
                OnLeft.Invoke();
            }

            transform.position += direction * currentSpeed * Time.deltaTime;
            if (transform.position.x <= minPositionX)
            {
                transform.position = new Vector3(maxPositionX, transform.position.y, transform.position.z);
            }
            else if (transform.position.x >= maxPositionX)
            {
                transform.position = new Vector3(minPositionX, transform.position.y, transform.position.z);
            }
        }

        private void OnPressDown(InputAction.CallbackContext context)
        {
            if (!canControl) return;

            hasPressed = true;
        }

        private void OnPressUp(InputAction.CallbackContext context)
        {
            if (!canControl) return;

            direction = Vector3.zero;
            animator.SetInteger("Direction", 0);
            OnStop.Invoke();

            hasPressed = false;
        }

        public void EnableControls(bool enabled)
        {
            canControl = enabled;
        }

        public void MultiplySpeedBy(float amount)
        {
            currentSpeed = speed * amount;
        }

        public void MoveToX(float targetXPosition)
        {
            canControl = false;
            StartCoroutine(Move(new Vector3(targetXPosition, transform.position.y, transform.position.z)));
        }

        private IEnumerator Move(Vector3 targetPosition)
        {
            while (transform.position.x != targetPosition.x)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);

                if (transform.position.x < targetPosition.x)
                {
                    animator.SetInteger("Direction", -1);
                    OnLeft.Invoke();
                }
                else
                {
                    animator.SetInteger("Direction", 1);
                    OnRight.Invoke();
                }

                yield return null;
            }

            OnCutscenePositionReached.Invoke();
        }
    }
}
