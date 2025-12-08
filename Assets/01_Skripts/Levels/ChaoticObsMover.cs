using System.Collections;
using UnityEngine;

public class ChaoticObsMover : MonoBehaviour
{
    private float planeMin;
    private float planeMax;
    private float speed;
    private float waitTime;
    private Vector3 direction;
    private Vector3 origin;


    public void Initialize(float min, float max, float spd, float wait)
    {
        planeMin = min;
        planeMax = max;
        speed = spd;
        waitTime = wait;

        StartCoroutine(MoveRoutine());
    }
    public void SetOrigin(Vector3 originPos)
    {
        origin = originPos;
    }

    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            // 1 Sekunde warten am Rand
            yield return new WaitForSeconds(waitTime);

            // Richtung setzen
            direction = GetRandomDirection();

            // Falls am Rand, zuerst rein ins Feld bewegen
            while (IsAtEdge(transform.localPosition))
            {
                MoveInDirectionTowardsCenter();
                yield return null;
            }

            // Jetzt ins Feld rein bewegen, bis Rand erreicht
            while (!IsAtEdge(transform.localPosition))
            {
                MoveInDirection();
                yield return null;
            }
        }
    }

    private void MoveInDirection()
    {
        Vector3 newPos = transform.localPosition + direction * speed * Time.deltaTime;
        newPos.x = Mathf.Clamp(newPos.x, origin.x + planeMin, origin.x + planeMax);
        newPos.z = Mathf.Clamp(newPos.z, origin.z + planeMin, origin.z + planeMax);
        transform.localPosition = newPos;
    }

    private void MoveInDirectionTowardsCenter()
    {
        Vector3 toCenter = (origin - transform.localPosition).normalized;
        Vector3 newPos = transform.localPosition + toCenter * speed * Time.deltaTime;
        newPos.x = Mathf.Clamp(newPos.x, origin.x + planeMin, origin.x + planeMax);
        newPos.z = Mathf.Clamp(newPos.z, origin.z + planeMin, origin.z + planeMax);
        transform.localPosition = newPos;
    }

    private Vector3 GetRandomDirection()
    {
        Vector2 randDir2D = Random.insideUnitCircle.normalized;
        return new Vector3(randDir2D.x, 0f, randDir2D.y);
    }

    private bool IsAtEdge(Vector3 pos)
    {
        float epsilon = 0.1f;
        return Mathf.Abs(pos.x - (origin.x + planeMin)) < epsilon ||
               Mathf.Abs(pos.x - (origin.x + planeMax)) < epsilon ||
               Mathf.Abs(pos.z - (origin.z + planeMin)) < epsilon ||
               Mathf.Abs(pos.z - (origin.z + planeMax)) < epsilon;
    }
}
