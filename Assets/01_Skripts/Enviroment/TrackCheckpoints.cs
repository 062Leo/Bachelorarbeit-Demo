using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MLAgentController;

public class TrackCheckpoints : MonoBehaviour
{
    private List<SingleCheckpoint> checkPointSingleList;
    private int nextCheckpointSingleIndex;
    public MLAgentController parkourAgent;
    [SerializeField] private ButtonController buttonController;
    [SerializeField] private EnviromentController enviromentController;
    private void Awake()
    {
        CollectSingleCheckpoints();
        if (enviromentController.getUseTextures())
        {
            EnableTextures();
            Debug.Log("EnableTextures111");
        }
        else
        {
            DisableTextures();
        }
    }
    public void CollectSingleCheckpoints()
    {
        checkPointSingleList = new List<SingleCheckpoint>();
        foreach (Transform checkPointSingleTransform in gameObject.transform)
        {
            SingleCheckpoint checkPointSingle = checkPointSingleTransform.GetComponent<SingleCheckpoint>();
            checkPointSingle.SetTrackChechpoints(this);
            checkPointSingleList.Add(checkPointSingle);
            checkPointSingleTransform.gameObject.SetActive(false);
        }
        nextCheckpointSingleIndex = 0;
        checkPointSingleList[nextCheckpointSingleIndex].gameObject.SetActive(true);
        parkourAgent.SetTargetTransformObject(checkPointSingleList[nextCheckpointSingleIndex].transform);
    }
    public void PlayerThroughCheckpoint(SingleCheckpoint checkpointSingle)
    {
        int currentIndex = checkPointSingleList.IndexOf(checkpointSingle);
        if (currentIndex == nextCheckpointSingleIndex)
        {
            nextCheckpointSingleIndex = (nextCheckpointSingleIndex + 1) % checkPointSingleList.Count;
            parkourAgent.SetTargetTransformObject(checkPointSingleList[nextCheckpointSingleIndex].transform);
            if (checkpointSingle.gameObject.name != "BtnPress") //für Lvl 3
            {
                checkpointSingle.gameObject.SetActive(false);
            }

            float rewardPerCheckpoint = 50f / checkPointSingleList.Count;
            parkourAgent.AddAgentReward(rewardPerCheckpoint);
            parkourAgent.ReachedCheckpoint();
            checkPointSingleList[nextCheckpointSingleIndex].gameObject.SetActive(true);

            // Prüfen, ob es das letzte Objekt ist
            if (currentIndex == checkPointSingleList.Count - 1)
            {
                if (enviromentController.getUseTextures())
                {
                    checkPointSingleList[currentIndex].transform.GetChild(0).gameObject.SetActive(true);
                }
                parkourAgent.ReachGoal();
                if (parkourAgent.useRandomTargetPosition == UseTarget.Level123)
                {
                    parkourAgent.SetRandomEdgePosition(checkPointSingleList[currentIndex].transform);
                }
                parkourAgent.AddAgentReward(5f);
            }
        }
    }
    public void ResetCheckPoints()
    {
        if (buttonController != null)
        {
            buttonController.ResetButton();
        }
        nextCheckpointSingleIndex = 0;
        parkourAgent.SetTargetTransformObject(checkPointSingleList[nextCheckpointSingleIndex].transform);
        foreach (Transform checkPointSingleTransform in gameObject.transform)
        {
            checkPointSingleTransform.gameObject.SetActive(false);
        }
        checkPointSingleList[nextCheckpointSingleIndex].gameObject.SetActive(true);
        if (enviromentController.getUseTextures())
        {
            checkPointSingleList[checkPointSingleList.Count - 1].gameObject.SetActive(true);
            EnableTextures();
            Debug.Log("EnableTextures2222");
        }
    }



    private void EnableTextures()
    {
        Debug.Log("EnableTextures");
        if (parkourAgent.useRandomTargetPosition != UseTarget.Level123)
        {
            foreach (SingleCheckpoint singleCheckpoint in checkPointSingleList)
            {
                //renderer deaktivieren unsichtbar, Letztes Child aktivieren mit .GetChild(0)  
                singleCheckpoint.transform.GetChild(0).gameObject.SetActive(false);
                singleCheckpoint.GetComponent<Renderer>().enabled = false;
            }
        }
        else
        {
            checkPointSingleList[checkPointSingleList.Count - 1].GetComponent<Renderer>().enabled = false;
        }
        checkPointSingleList[checkPointSingleList.Count - 1].transform.GetChild(0).gameObject.SetActive(true);
    }

    private void DisableTextures()
    {
        foreach (SingleCheckpoint singleCheckpoint in checkPointSingleList)
        {
            singleCheckpoint.GetComponent<Renderer>().enabled = true;
            if (singleCheckpoint.transform.childCount > 0)
            {
                singleCheckpoint.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
}
