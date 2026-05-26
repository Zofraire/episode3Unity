using System.Collections;
using UnityEngine;

namespace Project.Figures
{
    public class ItemSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject[] itemPrefabs;
        [SerializeField] private float width;
        [SerializeField] private float cooldownTime;

        private void Start()
        {
            StartCoroutine(Spawn());
        }

        private IEnumerator Spawn()
        {
            while (true)
            {
                int randomIndex = Random.Range(0, itemPrefabs.Length);
                GameObject item = Instantiate(itemPrefabs[randomIndex], transform);

                float randomX = Random.Range(transform.position.x - (width / 2), transform.position.x + (width / 2));
                item.transform.position = new Vector3(randomX, transform.position.y);

                yield return new WaitForSeconds(cooldownTime);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position - new Vector3(width / 2, 0), transform.position + new Vector3(width / 2, 0));
        }
    }
}
