using System.Collections;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    private LineRenderer line;
    [SerializeField] private float pointPerDistance;
    [SerializeField] private float maxRandomnessPerPoint;
    [SerializeField] private float pointDelay;

    private WaitForSeconds wait;
    private WaitForSeconds initialDelay = new WaitForSeconds(1.5f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        line = GetComponent<LineRenderer>();
        // wait = new WaitForSeconds(pointDelay);

        // // SetTarget(transform.position, target.position);
    }

    public void SetTarget(Vector2 start, Transform end, PlayerID player)
    {
        StartCoroutine(LightningCor(start, end, player));
        // 1. Calculate distance and direction

    }


    private IEnumerator LightningCor(Vector2 start, Transform target, PlayerID p = null)
    {

        Vector2 end = target.position;
        float distance = Vector2.Distance(start, end);
        int pointCount = Mathf.CeilToInt(distance / pointPerDistance);
        Debug.Log("Point count is" + pointCount);
        line.positionCount = 0;

        // 2. Direction from start to end
        Vector2 direction = (end - start).normalized;

        // 3. Get perpendicular direction
        Vector2 perpendicular = new Vector2(-direction.y, direction.x);

        // 4. Setup LineRenderer

        float currentOffset = pointCount * maxRandomnessPerPoint;
        int flip = 1;

        for (int i = 0; i < pointCount; i++)
        {
            line.positionCount = i + 1;
            float t = (float)i / (pointCount - 1);
            Vector2 basePos = Vector2.Lerp(start, end, t);


            Vector2 offset = perpendicular * currentOffset * flip * Random.Range(.5f, 1.1f);
            Debug.Log("offset is " + offset + " current offset: " + currentOffset);
            if (i == 0)
                line.SetPosition(i, basePos);
            else if (i == pointCount - 1)
            {
                line.SetPosition(i, target.position);
                if (p != null) p.events.LoseLife?.Invoke();
            }


            else
                line.SetPosition(i, basePos + offset);


            currentOffset -= maxRandomnessPerPoint;
            flip *= -1;
            yield return null;
            yield return null;

        }
        yield return new WaitForSeconds(.25f);
        line.positionCount = 0;
    }

    // Update is called once per frame

}
