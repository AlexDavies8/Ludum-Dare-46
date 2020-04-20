using System.Collections;
using UnityEngine;

public class LiftTrigger : MonoBehaviour
{
    public float chance = 0.5f;

    public float moveTime = 3f;
    public float doorTime = 1f;
    public AnimationCurve curve;

    public Transform up;
    public Transform down;

    public bool startUp = true;

    public LiftTrigger[] sync;
    public LiftTrigger[] syncInverse;

    Transform start, end;

    public Transform doorTransform;

    public Vector2 dogOffset;

    private void Awake()
    {
        UpdateState();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DogController dogController = collision.GetComponent<DogController>();

        if (dogController != null)
        {
            if (Random.Range(0f, 1f) < chance) StartCoroutine(LiftCoroutine(dogController));
        }
    }

    IEnumerator LiftCoroutine(DogController controller)
    {
        controller.stopped = true;
        controller.SetAnimation("Dog_Idle");

        Vector2 startPos = controller.transform.position;
        //Move to centre
        for (float t = 0; t < 1; t += Time.deltaTime * 2f)
        {
            controller.transform.position = Vector2.Lerp(startPos, (Vector2)transform.position + dogOffset, t);
            yield return null;
        }

        Transform prevParent = controller.transform.parent;
        controller.transform.SetParent(transform);

        //Close Door
        for (float t = 0; t < 1; t += Time.deltaTime / doorTime)
        {
            doorTransform.localScale = Vector3.Lerp(new Vector3(1, 0, 1), new Vector3(1, 1, 1), curve.Evaluate(t));
            yield return null;
        }

        //Move Lift
        for (float t = 0; t < 1f; t += Time.deltaTime / moveTime)
        {
            transform.position = Vector2.Lerp(start.position, end.position, curve.Evaluate(t));
            yield return null;
        }

        //Open Door
        for (float t = 0; t < 1; t += Time.deltaTime / doorTime)
        {
            doorTransform.localScale = Vector3.Lerp(new Vector3(1, 1, 1), new Vector3(1, 0, 1), curve.Evaluate(t));
            yield return null;
        }

        controller.transform.parent = prevParent;

        controller.SetAnimation("Dog_Walk");
        controller.stopped = false;

        startUp = !startUp;
        UpdateState();

        foreach (var lift in sync)
            lift.SyncState(startUp);
        foreach (var lift in syncInverse)
            lift.SyncState(!startUp);

        controller.direction = Random.Range(0f, 1f) > 0.5f ? 1 : -1;
    }

    void UpdateState()
    {
        if (startUp)
        {
            start = up;
            end = down;
        }
        else
        {
            end = up;
            start = down;
        }
    }

    public void SyncState(bool isUp)
    {
        startUp = isUp;
        UpdateState();

        transform.position = start.position;
    }
}
