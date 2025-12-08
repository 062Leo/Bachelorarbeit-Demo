using Assets.Skripts;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MLAgentController : Agent
{
    public enum UseTarget
    {
        Off,
        Level123,
        Level4,
        Level4V2,
        Level4OC,
        Level10,
        Level11,
        Level12,
        Generalisierung01

    }
    [Header("Initialize")]
    [SerializeField] private TrackCheckpoints trackCheckpoints;
    [SerializeField] private EnviromentController enviromentController;
    private InGameUIManager uiManager;
    private MovementController playerMovement;
    private Rigidbody rb;
    private RayPerceptionSensorComponent3D[] raySensors;

    [Header("Level Settings")]
    [Tooltip("Immer schauen das es den selben Wert hat ")]
    [SerializeField] public UseTarget useRandomTargetPosition;
    private Transform targetTransform;

    [Header("DistanceCheck Settings")]
    [SerializeField] private float checkIntervalSteps = 100f; // Intervall in Steps
    [SerializeField] private float requiredMinDistance = 1f; // Distance die in den `checkInterval` Sekunden erreicht werden soll
    [SerializeField] private float reward = 2.5f;
    private float stepsSinceLastCheck;
    private float previousDistance;


    private float _lastDistanceToTarget;
    private Vector3 startPos;
    private float _maxLevelDistance;

    [Header("Informations Read Only")]
    public float stepCountView;
    private List<float> runDurations = new List<float>();
    private float currentRunTime = 0f;
    private bool wasRunning = false;
    private int runTrueSteps = 0;
    private string parentNameFull;
    private float episodeStartTime;
    [SerializeField] private string levelName;

   void Start()
    {
        parentNameFull = transform.parent.name;

        previousDistance = CalculateXZDistance(transform.localPosition, targetTransform.localPosition);
        startPos = transform.localPosition;
        _maxLevelDistance = Vector3.Distance(startPos, targetTransform.localPosition);
        _lastDistanceToTarget = Vector3.Distance(transform.localPosition, targetTransform.localPosition);
    }
    
    private void Awake()
    {
        raySensors = GetComponents<RayPerceptionSensorComponent3D>(); // Holt ALLE Ray Perception Sensoren im GameObject
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<MovementController>();
        //levelManager = FindAnyObjectByType<LevelManager>();
        playerMovement.SetControlMode(false);
        SetRayPercepionTags();
        
        // InGameUIManager automatisch in der Scene finden
        if (uiManager == null)
        {
            uiManager = FindAnyObjectByType<InGameUIManager>();
        }
    }
    void FixedUpdate()
    {
        CalculateMovementRewards();
        CheckDistance10();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 toTarget = targetTransform.localPosition - transform.localPosition;
        sensor.AddObservation(toTarget.normalized);  // Richtung (3)
        sensor.AddObservation(Mathf.Clamp(toTarget.magnitude / 50f, 0f, 1f));  // Skalierte Entfernung (1)
        sensor.AddObservation(playerMovement.isGrounded ? 1f : 0f);  // More reliable than single raycast (1 value)
        sensor.AddObservation(Mathf.Clamp(rb.linearVelocity.x / 8f, -1f, 1f));
        sensor.AddObservation(Mathf.Clamp(rb.linearVelocity.z / 8f, -1f, 1f));
        sensor.AddObservation(rb.linearVelocity.y);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        bool jump = actions.DiscreteActions[0] == 1; // 1 for jump, 0 for no jump
        bool run = actions.DiscreteActions[1] == 1; // 1 for jump, 0 for no jump
        playerMovement.SetMovement(moveX, moveZ, run, jump);
        stepCountView = StepCount;
        stepsSinceLastCheck++;

        if (StepCount >= MaxStep)
        {
            Die();
            Debug.Log("Died, Steps �berschritten");
        }
        if (run)
        {
            runTrueSteps++;
            currentRunTime += Time.deltaTime;
            wasRunning = true;
        }
        else if (wasRunning)
        {
            // Highlight: Run-Zeit speichern
            runDurations.Add(currentRunTime);
            currentRunTime = 0f;
            wasRunning = false;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
        discreteActions[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
        discreteActions[1] = Input.GetKey(KeyCode.LeftShift) ? 1 : 0;

    }
    public void AddAgentReward(float reward)
    {
        AddReward(reward);
    }
    public void EndAgentEpisode()
    {
        Die();
       
    }

    public void Die()
    {
        
        CollectStatsOnEpisodeEnd();
        float currentDistance = Vector3.Distance(transform.localPosition, targetTransform.localPosition);
        float distPenalty = currentDistance / _maxLevelDistance;
        //Debug.Log(" die= distPenalty = " + distPenalty);
        
        float timeToDie = Time.time - episodeStartTime;

        AddReward(-20f - distPenalty);
        
        // UI aktualisieren
        if (uiManager != null)
        {
            uiManager.OnAgentDied(GetCumulativeReward());
        }
        
        EndEpisode();
        playerMovement.playerColor.color = Color.red;

    }

    // Wird aufgerufen, wenn der Agent das Ziel erreicht
    public void ReachGoal()
    {
        CollectStatsOnEpisodeEnd();
        float timeBonus = Mathf.Clamp(1f - ((float)StepCount / MaxStep), 0f, 1f) * 20f; //fixxed am 20.06.25 // davor hat es einfach immer 10 ausgegeben... jetzt so von 5 bis 20 oder so
        
        float timeToWin = Time.time - episodeStartTime;

        //Debug.Log(" Win= timeBonus = " + timeBonus);
        AddReward(timeBonus);
        AddReward(10f); //bonus win reward neu ab 20.06.25
        
        // UI aktualisieren
        if (uiManager != null)
        {
            uiManager.OnAgentSurvived(GetCumulativeReward());
        }
        
        EndEpisode();
        playerMovement.playerColor.color = Color.green;

    }
    private void CollectStatsOnEpisodeEnd()
    {

        if (wasRunning && currentRunTime > 0f)
        {
            runDurations.Add(currentRunTime);
            currentRunTime = 0f;
            wasRunning = false;
        }

        float avgRun = runDurations.Count > 0 ? runDurations.Average() * 1000f : 0f; // in ms
        float runPercent = SafePercent(runTrueSteps, StepCount);

        runDurations.Clear();
        runTrueSteps = 0;
    }
    float SafePercent(int numerator, int denominator)
    {
        if (denominator <= 0) return 0f;
        return Mathf.Clamp01((float)numerator / denominator) * 100f;
    }


    public override void OnEpisodeBegin()
    {

        episodeStartTime = Time.time;
        if (useRandomTargetPosition == UseTarget.Level4 || useRandomTargetPosition == UseTarget.Level4OC || useRandomTargetPosition == UseTarget.Level4V2)
        {
            enviromentController.SetRandomPosMode(null, useRandomTargetPosition);
        }
        if (useRandomTargetPosition == UseTarget.Level10)
        {
            enviromentController.SetRandomPosMode(targetTransform, useRandomTargetPosition);
        }
        if (useRandomTargetPosition == UseTarget.Level11)
        {
            enviromentController.StartMovingObstacles();
            enviromentController.SetRandomPosMode(targetTransform, UseTarget.Level11);
        }
        if (useRandomTargetPosition == UseTarget.Level12)
        {
            enviromentController.SetRandomPosMode(targetTransform, UseTarget.Level12);
            enviromentController.SpawnChaoticObstacles();
        }
        if (useRandomTargetPosition == UseTarget.Generalisierung01)
        {
            enviromentController.StartMovingObstacles();
            enviromentController.SetRandomPosMode(targetTransform, useRandomTargetPosition);
        }

        enviromentController.DeactivateRandomTargets();
        trackCheckpoints.ResetCheckPoints();

        //Debug.Log("Handle3Dmove = " + playerMovement.h3dMoveReward);
        //Debug.Log("wallReward = " + playerMovement.wallReward);
        //Debug.Log("sumProgressReward = " + sumProgressReward);
        transform.localPosition = startPos;
        transform.Rotate(0f, Random.Range(0f, 360f), 0f);
        playerMovement.h3dMoveReward = 0f;
        playerMovement.wallReward = 0f;
        previousDistance = CalculateXZDistance(transform.localPosition, targetTransform.localPosition);
        _lastDistanceToTarget = Vector3.Distance(transform.localPosition, targetTransform.localPosition);
    }

    public void SetTargetTransformObject(Transform targetTransformObject)
    {
        targetTransform = targetTransformObject;
    }

    public void ReachedCheckpoint()
    {
        previousDistance = CalculateXZDistance(transform.localPosition, targetTransform.localPosition);
    }
    private void CheckDistance10()
    {
        // Pr�fe alle `checkInterval` Sekunden
        if (stepsSinceLastCheck >= checkIntervalSteps)
        {
            float currentDistance = CalculateXZDistance(transform.localPosition, targetTransform.localPosition);
            float distanceDifference = previousDistance - currentDistance;
            if (distanceDifference >= requiredMinDistance)  // Mind. ** Einheiten n�her
            {
                AddReward(reward);
                previousDistance = currentDistance;
            }
            else  // Nicht genug Fortschritt
            {
                if (useRandomTargetPosition == UseTarget.Level10)//doppelt  bestrafung level 10
                {
                    AddReward(-reward * 1.5f); 
                }
                AddReward(-reward * 1.5f);
            }
            stepsSinceLastCheck = 0f;
        }
    }
    private float CalculateXZDistance(Vector3 a, Vector3 b)
    {
        return Vector2.Distance(
            new Vector2(a.x, a.z),
            new Vector2(b.x, b.z)
        );
    }
    void CalculateMovementRewards()
    {
        // Fortschritts-Belohnung (Richtung Ziel)
        float currentDist = Vector3.Distance(transform.localPosition, targetTransform.localPosition);
        if (currentDist < _lastDistanceToTarget)
        {
            float progress = (_lastDistanceToTarget - currentDist) * 0.5f;
            AddReward(progress);
        }
        else
        {
            float progress = (currentDist - _lastDistanceToTarget) * 0.5f;
            AddReward(-progress);
        }
        _lastDistanceToTarget = currentDist;
    }

    private void SetRayPercepionTags()
    {
        List<string> newTags = new List<string> { "target", "killPlayer", "ground", "wall" };
        foreach (var sensor in raySensors)
        {
            sensor.DetectableTags = newTags;
        }
    }
    public void SetRandomEdgePosition(Transform obectToMove)
    {
        enviromentController.SetRandomPosMode(obectToMove, useRandomTargetPosition);
    }
}
