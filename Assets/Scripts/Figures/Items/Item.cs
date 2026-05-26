using System;
using UnityEngine;
using UnityEngine.Events;

namespace Project.Figures
{
    [RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Collider2D))]
    public class Item : MonoBehaviour
    {
        public ItemType type;
        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private UnityEvent OnHit;
        public static event Action<ItemType> OnCorrect;
        public static event Action<ItemType> OnSpecial;
        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            OnHit.Invoke();

            if (collision.gameObject.tag.Equals("Finish"))
            {
                if (type == LevelManager.Instance.targetType)
                {
                    if (OnCorrect != null) OnCorrect.Invoke(type);
                    transform.parent = collision.transform;
                    rb.simulated = false;
                    enabled = false;
                    return;
                }
                else if (type == ItemType.Pillow || type == ItemType.Rocket)
                {
                    if (OnSpecial != null) OnSpecial.Invoke(type);
                }
            }
            else if (collision.gameObject.tag.Equals("Player"))
            {
                return;
            }

            GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation, transform.parent);
            Destroy(explosion, 1);
            Destroy(gameObject);
        }
    }
}
