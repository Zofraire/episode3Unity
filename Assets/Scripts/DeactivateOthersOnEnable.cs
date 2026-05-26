using System.Collections;
using UnityEngine;

public class DeactivateOthersOnEnable : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("How many seconds to wait before hiding the objects.")]
    public float delayInSeconds = 1.0f; // Default is 1 second

    [Header("Objects to Hide")]
    [Tooltip("Assign the GameObjects you want to deactivate here.")]
    public GameObject[] objectsToDeactivate;

    private void OnEnable()
    {
        // Every time this object turns on, start the countdown
        StartCoroutine(DeactivateWithDelay());
    }

    private IEnumerator DeactivateWithDelay()
    {
        // 1. Wait for the specified amount of time
        yield return new WaitForSeconds(delayInSeconds);

        // 2. Loop through the array and turn off every object
        foreach (GameObject obj in objectsToDeactivate)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }
}