using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionController : MonoBehaviour
{
    public CanvasGroup transitionGroup;
    public float transitionTime = 0.8f;

    private void Awake()
    {
        TransitionIn(null);
    }

    public void TransitionIn(System.Action callback)
    {
        StartCoroutine(TransitionInCoroutine(callback));
    }

    IEnumerator TransitionInCoroutine(System.Action callback)
    {
        transitionGroup.gameObject.SetActive(true);
        for (float t = 0; t < 1f; t += Time.deltaTime / transitionTime)
        {
            transitionGroup.alpha = 1 - t;
            yield return null;
        }
        transitionGroup.gameObject.SetActive(false);

        if (callback != null) callback.Invoke();
    }

    public void TransitionOut(System.Action callback)
    {
        StartCoroutine(TransitionOutCoroutine(callback));
    }

    IEnumerator TransitionOutCoroutine(System.Action callback)
    {
        transitionGroup.gameObject.SetActive(true);
        for (float t = 0; t < 1f; t += Time.deltaTime / transitionTime)
        {
            transitionGroup.alpha = t;
            yield return null;
        }

        if (callback != null) callback.Invoke();
    }
}
