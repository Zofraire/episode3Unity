using System.Collections;
using UnityEngine;

public class ColorChange : MonoBehaviour
{
    [SerializeField] private Color targetColor;
    [SerializeField] private float speed;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void ChangeToTargetColor()
    {
        StopAllCoroutines();
        StartCoroutine(ChangingToTarget(targetColor));
    }

    public void ChangeToRandomColor()
    {
        StopAllCoroutines();
        StartCoroutine(ChangingToRandom());
    }

    public void Restore()
    {
        StopAllCoroutines();
        StartCoroutine(ChangingToTarget(Color.white));
    }

    // Update is called once per frame
    private IEnumerator ChangingToTarget(Color color)
    {
        while (sr.color != color)
        {
            Color currentColor = Vector4.MoveTowards(sr.color, color, Time.deltaTime * speed);
            sr.color = currentColor;
            yield return null;
        }
    }

    private IEnumerator ChangingToRandom()
    {
        while (true)
        {
            targetColor = Random.ColorHSV(0, 1, 0, 1, 1, 1);
            while (sr.color != targetColor)
            {
                Color currentColor = Vector4.MoveTowards(sr.color, targetColor, Time.deltaTime * speed);
                sr.color = currentColor;
                yield return null;
            }
            yield return null;
        }
    }
}
