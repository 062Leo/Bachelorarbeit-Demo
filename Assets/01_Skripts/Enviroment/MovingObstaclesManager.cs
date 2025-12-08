using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstaclesManager : MonoBehaviour
{
    public enum Axis { X, Z }

    private struct TransformData
    {
        public Vector3 position;
        public Quaternion rotation;

        public TransformData(Vector3 pos, Quaternion rot)
        {
            position = pos;
            rotation = rot;
        }
    }
    private Dictionary<GameObject, TransformData> startTransforms = new Dictionary<GameObject, TransformData>();
    private Dictionary<Transform, Coroutine> rotateAroundRoutines = new Dictionary<Transform, Coroutine>();
    private Dictionary<Transform, Coroutine> rotateSelfRoutines = new Dictionary<Transform, Coroutine>();



    public void RegisterObject(GameObject obj)
    {
        if (!startTransforms.ContainsKey(obj))
            startTransforms.Add(obj, new TransformData(obj.transform.position, obj.transform.rotation));
    }

    public void MoveBackAndForth(GameObject obj, float speed, Axis axis, float limit)
    {
        RegisterObject(obj);
        StartCoroutine(MoveBackAndForthRoutine(obj.transform, speed, axis, limit));
    }

    public void RotateAroundPoint(GameObject obj, Transform pos, float radius, float speed, int direction)
    {
        RegisterObject(obj);

        if (rotateAroundRoutines.ContainsKey(obj.transform))
        {
            StopCoroutine(rotateAroundRoutines[obj.transform]);
            rotateAroundRoutines.Remove(obj.transform);
        }

        Coroutine c = StartCoroutine(RotateAroundPointRoutine(obj.transform, pos, radius, speed, direction));
        rotateAroundRoutines.Add(obj.transform, c);
    }


    public void RotateSelfY(GameObject obj, float speed, int direction)
    {
        RegisterObject(obj);

        if (rotateSelfRoutines.ContainsKey(obj.transform))
        {
            StopCoroutine(rotateSelfRoutines[obj.transform]);
            rotateSelfRoutines.Remove(obj.transform);
        }

        Coroutine c = StartCoroutine(RotateSelfYRoutine(obj.transform, speed, direction));
        rotateSelfRoutines.Add(obj.transform, c);
    }


    // Methode zum Zurücksetzen aller registrierten Objekte auf Startwerte (movebackForth objekte)
    public void ResetPositionsMBF()
    {
        foreach (var kvp in startTransforms)
        {
            GameObject obj = kvp.Key;
            TransformData data = kvp.Value;

            obj.transform.localPosition = data.position;
            obj.transform.rotation = data.rotation;

            StopAllCoroutines();
        }
    }
    public void SetRandomPositionMBF(GameObject obj, Axis axis, float limit) //random Pos bei neuer Episode
    {
        StopAllCoroutines();
        Vector3 pos = obj.transform.localPosition;
        float randomValue = Random.Range(-limit, limit);

        if (axis == Axis.X)
            pos.x = randomValue;
        else if (axis == Axis.Z)
            pos.z = randomValue;

        obj.transform.localPosition = pos;
    }

    #region Couroutines
    private IEnumerator MoveBackAndForthRoutine(Transform obj, float speed, Axis axis, float limit)
    {
        float direction = (Random.value > 0.5f) ? 1f : -1f; //random start dir

        while (true)
        {
            Vector3 pos = obj.localPosition;
            float value = (axis == Axis.X) ? pos.x : pos.z;

            if (value >= limit)
                direction = -1f;
            else if (value <= -limit)
                direction = 1f;

            Vector3 move = (axis == Axis.X ? Vector3.right : Vector3.forward) * direction * speed * Time.deltaTime;
            obj.Translate(move, Space.World);

            yield return null;
        }
    }

    private IEnumerator RotateAroundPointRoutine(Transform obj, Transform pos, float radius, float speed,int direction)
    {
        Vector3 center = pos.localPosition;
        Vector3 offset = obj.localPosition - center;
        float angle = Mathf.Atan2(offset.z, offset.x) * Mathf.Rad2Deg;

        
        //int direction = Random.value < 0.5f ? 1 : -1; //random drehrichtung... wurde entfernt, wird nun im SO bestimmt

        while (true)
        {
            angle += direction * speed * Time.deltaTime;
            if (angle > 360f) angle -= 360f;
            if (angle < 0f) angle += 360f;

            float rad = angle * Mathf.Deg2Rad;
            float x = center.x + Mathf.Cos(rad) * radius;
            float z = center.z + Mathf.Sin(rad) * radius;

            obj.localPosition = new Vector3(x, obj.localPosition.y, z);
            yield return null;
        }
    }
    #endregion Courtines

    public void SetRandomPosRAP(Transform obj, Transform center, float radius) //random Pos bei neuer Episode Rotate Around Point
    {
        if (rotateAroundRoutines.ContainsKey(obj))
        {
            StopCoroutine(rotateAroundRoutines[obj]);
            rotateAroundRoutines.Remove(obj);
        }

        float angle = Random.Range(0f, 360f);
        float rad = angle * Mathf.Deg2Rad;

        float x = center.localPosition.x + Mathf.Cos(rad) * radius;
        float z = center.localPosition.z + Mathf.Sin(rad) * radius;

        obj.localPosition = new Vector3(x, obj.localPosition.y, z);
    }

    private IEnumerator RotateSelfYRoutine(Transform obj, float speed, int direction)
    {
        while (true)
        {
            obj.Rotate(0, direction * speed * Time.deltaTime, 0);
            yield return null;
        }
    }
}
