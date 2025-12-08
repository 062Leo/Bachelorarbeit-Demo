using System.Collections.Generic;
using UnityEngine;

public class ChaoticObsSpawner : MonoBehaviour
{
    public GameObject obsCubePrefab;
    public int spawnCount = 10;
    public float speed = 5f;
    public float waitTime = 1f;
    public Transform origin;
    public Transform obsHolder;

    private List<GameObject> spawnedCubes = new List<GameObject>();
    private float planeMin = -38f;
    private float planeMax = 38f;

    public void SpawnObstacles(EnviromentController enviromentController, int sideParam)
    {
        ResetObstacles(); // Falls vorher welche gespawnt wurden

        bool side = true;
        for (int i = 0; i < spawnCount; i++)
        {
            GameObject cube = Instantiate(obsCubePrefab, origin.localPosition, Quaternion.identity, obsHolder);
            if (side)
            {
                enviromentController.RandomTargetPosLvl12(cube.transform, sideParam);
                side=false;
            }
            else
            {
                enviromentController.RandomTargetPosLvl12(cube.transform, null);
                side = true;
            }

            ChaoticObsMover mover = cube.GetComponent<ChaoticObsMover>();
            mover.SetOrigin(origin.localPosition);
            mover.Initialize(planeMin, planeMax, speed, waitTime);

            spawnedCubes.Add(cube);
        }
    }

    public void ResetObstacles()
    {
        foreach (GameObject cube in spawnedCubes)
        {
            if (cube != null)
                Destroy(cube);
        }
        spawnedCubes.Clear();
    }
    
}
