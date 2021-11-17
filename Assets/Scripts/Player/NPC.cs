using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    private Queue<Vector2> mWayPoints = new Queue<Vector2>();
    public float Speed = 5.0f;

    void Start()
    {
        StartCoroutine(Coroutine_MoveTo());
    }

    public void AddWayPoint(float x, float y)
    {
        AddWayPoint(new Vector2(x, y));
    }

    public void AddWayPoint(Vector2 p)
    {
        mWayPoints.Enqueue(p);
    }

    IEnumerator Coroutine_MoveTo()
    {
        while (true)
        {
            while (mWayPoints.Count > 0)
            {
                yield return StartCoroutine(Coroutine_MoveToPoint(mWayPoints.Dequeue(), Speed));
            }
            yield return null;
        }
    }

    IEnumerator Coroutine_MoveToPoint(Vector2 endP, float speed)
    {
        Vector3 p = new Vector3(endP.x, endP.y, transform.position.z);

        float duration = (transform.position - p).magnitude / speed;

        yield return StartCoroutine(Coroutine_MoveInSeconds(p, duration));
    }

    IEnumerator Coroutine_MoveInSeconds(Vector3 endP, float duration)
    {
        float elaspedTime = 0.0f;
        Vector3 startP = transform.position;

        while (elaspedTime < duration)
        {
            transform.position = Vector3.Lerp(startP, endP, elaspedTime / duration);
            elaspedTime += Time.deltaTime;

            yield return null;
        }
        transform.position = endP;
    }
}
