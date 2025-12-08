using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MLAgentController;

public class EnviromentController : MonoBehaviour
{
    [Header("Initialize TextureElements------------------------")]
    [Tooltip("set Texture & Random Ground Position")]
    [SerializeField] private GameObject groundHolderObj;
    [Tooltip("set Texture")]
    [SerializeField] private GameObject mapEdges;
    [Tooltip("set Texture")]
    [SerializeField] private GameObject obstacleWalls;
    [SerializeField] private Material mapEdgeMaterial;
    [Tooltip("Shader (Lava)")]
    [SerializeField] private GameObject ShaderHolder;

    [Header("Other Settings------------------------------------")]
    [Tooltip("Einfach Leer lassen wenn im Level nicht benötigt")]
    [SerializeField] private GameObject targetHolderObj;
    [Tooltip("Einfach Leer lassen wenn im Level nicht benötigt")]
    [SerializeField] private GameObject disabledTargetHolderObj;
    [SerializeField] private Material groundMaterial;


    [Header("Level 3 Settings----------------------------------")]
    [Tooltip("Level3")]
    [SerializeField] private GameObject lavaWall;

    [Header("Level 1-3 Settings--------------------------------")]
    [Tooltip("set Texture level1-3 (2groundtypes) braucht man nicht")]
    [SerializeField] private GameObject groundHolderObj2;
    [Tooltip("Ground Material Obj2(level1-3)")]
    [SerializeField] private Material groundMaterial2;

    [Header("Level 4,7,8 Settings-------------------------------")]
    [Tooltip("fix Texture level 4,7,8 ")]
    [SerializeField] private GameObject firstGroundObj;


    [Header("Level 7 Settings------------------------------------")]
    [Tooltip("Level 07")]
    [SerializeField] private Material lavaObstacleMaterial;
    [Tooltip("Level 07")]
    [SerializeField] private Material lavaGroundMaterial;

    [Header("Level 8,9 Settings----------------------------------")]
    [Tooltip("Level 08, 09")]
    [SerializeField] private Material obstacleWallMaterial;
    [Tooltip("Level 08, 09")]
    [SerializeField] private Material obstacleWallMaterial2;

    [Header("Level 10 Settings-----------------------------------")]
    [SerializeField, Range(80, 160)] private int amount;
    [SerializeField] private GameObject groundPrefab;
    private List<Vector3> usedPositions = new List<Vector3>();


    [Header("Level 11 Settings-----------------------------------")]
    public bool useMBFRandomStartPos;
    [Header("↳ GameObjects (Szenen-Referenzen)")]
    public GameObject[] obsBFX;
    public GameObject[] obsBFX2;
    public GameObject[] obsBFZ;
    public GameObject[] obsBFZ2;
    public GameObject[] obsRAP;
    public Transform[] rotateAroundPos;
    public GameObject[] obsRS;

    [Header("↳ Movement Data (ScriptableObject)")]
    [SerializeField] private BackAndForth movementDataV1;
    [SerializeField] private RotateAroundPoint movementDataV2;
    [SerializeField] private RAPAndRS movementDataV3;

    public enum RotationDirection
    {
        CounterClockwise = -1,
        Clockwise = 1
    }

    [Header("Settings---------------------------------------------")]
    [SerializeField] private bool deactivateRndmTargetsOnLvlBeginn;
    [SerializeField] private bool useTexture;

    [Header("private Settings-------------------------------------")]
    private TrackCheckpoints trackCheckpoints;
    private List<Transform> groundList = new List<Transform>();
    private List<Transform> targetList = new List<Transform>();
    private MovingObstaclesManager movingObsManager;
    private ChaoticObsSpawner chaoticObsSpawner;
    [Header("Level12 Settings------------------------------------")]
    private int sideLvl12 = -1;

    private void Awake()
    {
        chaoticObsSpawner = GetComponent<ChaoticObsSpawner>();
        movingObsManager = GetComponent<MovingObstaclesManager>();
        if (targetHolderObj != null)
        {
            trackCheckpoints = targetHolderObj.GetComponent<TrackCheckpoints>();
            foreach (Transform targetObj in targetHolderObj.transform)
            {
                targetList.Add(targetObj);
            }
        }
        if (groundHolderObj != null)
        {
            foreach (Transform groundObj in groundHolderObj.transform)
            {
                groundList.Add(groundObj);
            }
        }

    }
    void Start()
    {
        UpdateTextures();
    }
    private void UpdateTextures()
    {
        //Aktiviert bzw deaktiviert die "schöneren" Texturen der Levels (nicht jedes Level ist gleich, daher unterschiedliche Holder objekte die manchmal leer sind
        if (useTexture)
        {
            if (lavaWall != null)
            {
                foreach (Transform child in lavaWall.transform)
                {
                    var mf = child.GetComponent<MeshFilter>();
                    var renderer = child.GetComponent<Renderer>();
                    if (mf == null || renderer == null) continue;

                    //Instanz der Materialien, damit jedes Objekt sein eigenes Material bekommt und auf größe angepasst werden kann ohne verzerrt zu wirken
                    var material = new Material(lavaObstacleMaterial);
                    renderer.material = material;

                    // größe bestimmen
                    var size = mf.sharedMesh.bounds.size;
                    var scale = child.localScale;

                    // höhe/breite berechnen
                    float width = size.x * scale.x;
                    float height = size.z * scale.z;

                    // Material Textur anpassen
                    material.mainTextureScale = new Vector2(width / size.x / 10, height / size.z / 10);

                }
            }
            if (ShaderHolder != null) 
            {
                ShaderHolder.SetActive(true);
            }
            if (groundHolderObj != null)
            {
                foreach (Transform child in groundHolderObj.transform)
                {
                    var mf = child.GetComponent<MeshFilter>();
                    var renderer = child.GetComponent<Renderer>();

                    if (mf == null || renderer == null) continue;

                    var material = new Material(groundMaterial);
                    renderer.material = material;

                    var size = mf.sharedMesh.bounds.size;
                    var scale = child.localScale;

                    float width = size.x * scale.x;
                    float height = size.z * scale.z;

                    material.mainTextureScale = new Vector2(width / size.x / 10, height / size.z / 10);

                    if (child.childCount > 0)
                    {
                        var childRenderer = child.GetChild(0).GetComponent<Renderer>();
                        var childMf = child.GetComponent<MeshFilter>();
                        if (childMf == null || childRenderer == null) continue;

                        var childmaterial = new Material(lavaObstacleMaterial);
                        childRenderer.material = childmaterial;

                        var childsize = childMf.sharedMesh.bounds.size;
                        var childscale = child.GetChild(0).localScale;

                        float childwidth = childsize.x * childscale.x;
                        float childheight = childsize.z * childscale.z;

                        childmaterial.mainTextureScale = new Vector2(childwidth / childsize.x / 10, childheight / childsize.z / 10);
                    }
                }

                if (groundMaterial2 != null && groundHolderObj2 != null)
                {
                    foreach (Transform child in groundHolderObj2.transform)
                    {
                        var mf = child.GetComponent<MeshFilter>();
                        var renderer = child.GetComponent<Renderer>();
                        if (mf == null || renderer == null) continue;

                        var material = new Material(groundMaterial2);
                        renderer.material = material;

                        var size = mf.sharedMesh.bounds.size;
                        var scale = child.localScale;

                        float width = size.x * scale.x;
                        float height = size.z * scale.z;

                        material.mainTextureScale = new Vector2(width / size.x / 10, height / size.z / 10);
                    }
                }

                if (firstGroundObj != null)
                {
                    Transform tf = firstGroundObj.transform;
                    var mf = tf.GetComponent<MeshFilter>();
                    var renderer = tf.GetComponent<Renderer>();
                    if (mf == null || renderer == null) return;

                    var material = new Material(groundMaterial);
                    renderer.material = material;

                    var size = mf.sharedMesh.bounds.size;
                    var scale = tf.localScale;

                    float width = size.x * scale.x;
                    float height = size.z * scale.z;

                    material.mainTextureScale = new Vector2(width / size.x / 10, height / size.z / 10);
                }

            }
            if (mapEdges != null)
            {
                foreach (Transform child in mapEdges.transform)
                {
                    var mf = child.GetComponent<MeshFilter>();
                    var renderer = child.GetComponent<Renderer>();
                    if (mf == null || renderer == null) continue;

                    var material = new Material(mapEdgeMaterial);
                    renderer.material = material;

                    var size = mf.sharedMesh.bounds.size;
                    var scale = child.localScale;

                    float width = size.x * scale.x;

                    material.mainTextureScale = new Vector2(width / size.x / 10, 0.5f);
                }
            }
            if (obstacleWalls != null)
            {
                bool switchMaterial = true;
                foreach (Transform child in obstacleWalls.transform)
                {
                    var mf = child.GetComponent<MeshFilter>();
                    var renderer = child.GetComponent<Renderer>();
                    if (mf == null || renderer == null) continue;

                    var material = new Material(obstacleWallMaterial);
                    if (switchMaterial)
                    {
                        material = new Material(obstacleWallMaterial2);
                        switchMaterial = false;
                    }
                    else
                    {
                        switchMaterial = true;
                    }
                    renderer.material = material;

                    var size = mf.sharedMesh.bounds.size;
                    var scale = child.localScale;

                    float width = size.x * scale.x;

                    material.mainTextureScale = new Vector2(width / size.x / 10, 0.5f);
                }
            }
        }
        else
        {
            if (ShaderHolder != null)
            {
                ShaderHolder.SetActive(false);
            }
        }
    }


    public void StartMovingObstacles()
    {
        if (movingObsManager == null) return;
        // Rule detection for hardcoded presets
        bool condV1 = (obsBFX != null && obsBFX.Length == 2)
            && (obsBFX2 != null && obsBFX2.Length == 2)
            && (obsBFZ != null && obsBFZ.Length == 2)
            && (obsBFZ2 != null && obsBFZ2.Length == 2)
            && (obsRAP != null && obsRAP.Length == 0)
            && (rotateAroundPos != null && rotateAroundPos.Length == 1)
            && (obsRS != null && obsRS.Length == 0);

        bool condV2 = (obsBFX != null && obsBFX.Length == 0)
            && (obsBFX2 != null && obsBFX2.Length == 0)
            && (obsBFZ != null && obsBFZ.Length == 0)
            && (obsBFZ2 != null && obsBFZ2.Length == 0)
            && (obsRAP != null && obsRAP.Length == 8)
            && (rotateAroundPos != null && rotateAroundPos.Length == 1)
            && (obsRS != null && obsRS.Length == 0);

        bool condV3 = (obsBFX != null && obsBFX.Length == 0)
            && (obsBFX2 != null && obsBFX2.Length == 0)
            && (obsBFZ != null && obsBFZ.Length == 0)
            && (obsBFZ2 != null && obsBFZ2.Length == 0)
            && (obsRAP != null && obsRAP.Length == 4)
            && (rotateAroundPos != null && rotateAroundPos.Length == 4)
            && (obsRS != null && obsRS.Length == 4);

        if (useMBFRandomStartPos) //random Pos bei neuer Episode
        {
            if (movementDataV1 != null || condV1)
            {
                foreach (GameObject obj in obsBFX)
                {
                    if (obj != null)
                        movingObsManager.SetRandomPositionMBF(obj, MovingObstaclesManager.Axis.X,
                            movementDataV1 != null ? movementDataV1.moveLimitX : ScO_Data.BackAndForthPreset.moveLimitX);
                }
                foreach (GameObject obj in obsBFX2)
                {
                    if (obj != null)
                        movingObsManager.SetRandomPositionMBF(obj, MovingObstaclesManager.Axis.X,
                            movementDataV1 != null ? movementDataV1.moveLimitX2 : ScO_Data.BackAndForthPreset.moveLimitX2);
                }
                foreach (GameObject obj in obsBFZ)
                {
                    if (obj != null)
                        movingObsManager.SetRandomPositionMBF(obj, MovingObstaclesManager.Axis.Z,
                            movementDataV1 != null ? movementDataV1.moveLimitZ : ScO_Data.BackAndForthPreset.moveLimitZ);
                }
                foreach (GameObject obj in obsBFZ2)
                {
                    if (obj != null)
                        movingObsManager.SetRandomPositionMBF(obj, MovingObstaclesManager.Axis.Z,
                            movementDataV1 != null ? movementDataV1.moveLimitZ2 : ScO_Data.BackAndForthPreset.moveLimitZ2);
                }
            }
            if (movementDataV2 != null || condV2)
            {
                for (int i = 0; i < obsRAP.Length; i++)
                {
                    float radius = movementDataV2 != null
                        ? (i < movementDataV2.rotateAroundRadius.Length ? movementDataV2.rotateAroundRadius[i] : movementDataV2.rotateAroundRadius[movementDataV2.rotateAroundRadius.Length - 1])
                        : (i < ScO_Data.RotateAroundPointPreset.rotateAroundRadius.Length ? ScO_Data.RotateAroundPointPreset.rotateAroundRadius[i] : ScO_Data.RotateAroundPointPreset.rotateAroundRadius[ScO_Data.RotateAroundPointPreset.rotateAroundRadius.Length - 1]);
                    movingObsManager.SetRandomPosRAP(obsRAP[i].transform, rotateAroundPos[0], radius);
                }
            }
            if (movementDataV3 != null || condV3)
            {
                for (int i = 0; i < obsRAP.Length; i++)
                {
                    float radius = movementDataV3 != null
                        ? (i < movementDataV3.rotateAroundRadius.Length ? movementDataV3.rotateAroundRadius[i] : movementDataV3.rotateAroundRadius[movementDataV3.rotateAroundRadius.Length - 1])
                        : (i < ScO_Data.RAPAndRSPreset.rotateAroundRadius.Length ? ScO_Data.RAPAndRSPreset.rotateAroundRadius[i] : ScO_Data.RAPAndRSPreset.rotateAroundRadius[ScO_Data.RAPAndRSPreset.rotateAroundRadius.Length - 1]);
                    movingObsManager.SetRandomPosRAP(obsRAP[i].transform, rotateAroundPos[i], radius);
                }
            }
        }
        else
        {
            movingObsManager.ResetPositionsMBF(); //standartPos
        }
        // Starte Bewegungen mit Daten aus ScriptabelObjects
        if (movementDataV1 != null || condV1)
        {
            foreach (GameObject obj in obsBFX)
            {
                if (obj != null)
                    movingObsManager.MoveBackAndForth(obj,
                        movementDataV1 != null ? movementDataV1.moveSpeedX : ScO_Data.BackAndForthPreset.moveSpeedX,
                        MovingObstaclesManager.Axis.X,
                        movementDataV1 != null ? movementDataV1.moveLimitX : ScO_Data.BackAndForthPreset.moveLimitX);
            }
            foreach (GameObject obj in obsBFX2)
            {
                if (obj != null)
                    movingObsManager.MoveBackAndForth(obj,
                        movementDataV1 != null ? movementDataV1.moveSpeedX2 : ScO_Data.BackAndForthPreset.moveSpeedX2,
                        MovingObstaclesManager.Axis.X,
                        movementDataV1 != null ? movementDataV1.moveLimitX2 : ScO_Data.BackAndForthPreset.moveLimitX2);
            }
            foreach (GameObject obj in obsBFZ)
            {
                if (obj != null)
                    movingObsManager.MoveBackAndForth(obj,
                        movementDataV1 != null ? movementDataV1.moveSpeedZ : ScO_Data.BackAndForthPreset.moveSpeedZ,
                        MovingObstaclesManager.Axis.Z,
                        movementDataV1 != null ? movementDataV1.moveLimitZ : ScO_Data.BackAndForthPreset.moveLimitZ);
            }
            foreach (GameObject obj in obsBFZ2)
            {
                if (obj != null)
                    movingObsManager.MoveBackAndForth(obj,
                        movementDataV1 != null ? movementDataV1.moveSpeedZ2 : ScO_Data.BackAndForthPreset.moveSpeedZ2,
                        MovingObstaclesManager.Axis.Z,
                        movementDataV1 != null ? movementDataV1.moveLimitZ2 : ScO_Data.BackAndForthPreset.moveLimitZ2);
            }
        }
        if (movementDataV2 != null || condV2)
        {
            for (int i = 0; i < obsRAP.Length; i++)
            {
                float radius = movementDataV2 != null
                    ? (i < movementDataV2.rotateAroundRadius.Length ? movementDataV2.rotateAroundRadius[i] : movementDataV2.rotateAroundRadius[movementDataV2.rotateAroundRadius.Length - 1])
                    : (i < ScO_Data.RotateAroundPointPreset.rotateAroundRadius.Length ? ScO_Data.RotateAroundPointPreset.rotateAroundRadius[i] : ScO_Data.RotateAroundPointPreset.rotateAroundRadius[ScO_Data.RotateAroundPointPreset.rotateAroundRadius.Length - 1]);
                float speed = movementDataV2 != null
                    ? (i < movementDataV2.rotateAroundSpeed.Length ? movementDataV2.rotateAroundSpeed[i] : movementDataV2.rotateAroundSpeed[movementDataV2.rotateAroundSpeed.Length - 1])
                    : (i < ScO_Data.RotateAroundPointPreset.rotateAroundSpeed.Length ? ScO_Data.RotateAroundPointPreset.rotateAroundSpeed[i] : ScO_Data.RotateAroundPointPreset.rotateAroundSpeed[ScO_Data.RotateAroundPointPreset.rotateAroundSpeed.Length - 1]);
                var dir = movementDataV2 != null
                    ? (i < movementDataV2.rotateDirRAP.Length ? movementDataV2.rotateDirRAP[i] : movementDataV2.rotateDirRAP[movementDataV2.rotateDirRAP.Length - 1])
                    : (i < ScO_Data.RotateAroundPointPreset.rotateDirRAP.Length ? ScO_Data.RotateAroundPointPreset.rotateDirRAP[i] : ScO_Data.RotateAroundPointPreset.rotateDirRAP[ScO_Data.RotateAroundPointPreset.rotateDirRAP.Length - 1]);
                movingObsManager.RotateAroundPoint(obsRAP[i], rotateAroundPos[0], radius, speed, (int)dir);
            }
        }
        if (movementDataV3 != null || condV3)
        {
            for (int i = 0; i < obsRAP.Length; i++)
            {
                float radius = movementDataV3 != null
                    ? (i < movementDataV3.rotateAroundRadius.Length ? movementDataV3.rotateAroundRadius[i] : movementDataV3.rotateAroundRadius[movementDataV3.rotateAroundRadius.Length - 1])
                    : (i < ScO_Data.RAPAndRSPreset.rotateAroundRadius.Length ? ScO_Data.RAPAndRSPreset.rotateAroundRadius[i] : ScO_Data.RAPAndRSPreset.rotateAroundRadius[ScO_Data.RAPAndRSPreset.rotateAroundRadius.Length - 1]);
                float speed = movementDataV3 != null
                    ? (i < movementDataV3.rotateAroundSpeed.Length ? movementDataV3.rotateAroundSpeed[i] : movementDataV3.rotateAroundSpeed[movementDataV3.rotateAroundSpeed.Length - 1])
                    : (i < ScO_Data.RAPAndRSPreset.rotateAroundSpeed.Length ? ScO_Data.RAPAndRSPreset.rotateAroundSpeed[i] : ScO_Data.RAPAndRSPreset.rotateAroundSpeed[ScO_Data.RAPAndRSPreset.rotateAroundSpeed.Length - 1]);
                var dir = movementDataV3 != null
                    ? (i < movementDataV3.rotateDirRAP.Length ? movementDataV3.rotateDirRAP[i] : movementDataV3.rotateDirRAP[movementDataV3.rotateDirRAP.Length - 1])
                    : (i < ScO_Data.RAPAndRSPreset.rotateDirRAP.Length ? ScO_Data.RAPAndRSPreset.rotateDirRAP[i] : ScO_Data.RAPAndRSPreset.rotateDirRAP[ScO_Data.RAPAndRSPreset.rotateDirRAP.Length - 1]);
                movingObsManager.RotateAroundPoint(obsRAP[i], rotateAroundPos[i], radius, speed, (int)dir);
            }
            for (int i = 0; i < obsRS.Length; i++)
            {
                float selfSpeed = movementDataV3 != null
                    ? (i < movementDataV3.selfRotateSpeed.Length ? movementDataV3.selfRotateSpeed[i] : movementDataV3.selfRotateSpeed[movementDataV3.selfRotateSpeed.Length - 1])
                    : (i < ScO_Data.RAPAndRSPreset.selfRotateSpeed.Length ? ScO_Data.RAPAndRSPreset.selfRotateSpeed[i] : ScO_Data.RAPAndRSPreset.selfRotateSpeed[ScO_Data.RAPAndRSPreset.selfRotateSpeed.Length - 1]);
                var selfDir = movementDataV3 != null
                    ? (i < movementDataV3.rotateDirRS.Length ? movementDataV3.rotateDirRS[i] : movementDataV3.rotateDirRS[movementDataV3.rotateDirRS.Length - 1])
                    : (i < ScO_Data.RAPAndRSPreset.rotateDirRS.Length ? ScO_Data.RAPAndRSPreset.rotateDirRS[i] : ScO_Data.RAPAndRSPreset.rotateDirRS[ScO_Data.RAPAndRSPreset.rotateDirRS.Length - 1]);
                movingObsManager.RotateSelfY(obsRS[i], selfSpeed, (int)selfDir);
            }
        }

    }
    
    public void SpawnChaoticObstacles()
    {
        if (chaoticObsSpawner != null)
        {
            chaoticObsSpawner.SpawnObstacles(this, sideLvl12); //sideLvl12 = die seite an der das Target ist.
        }
    }

    public bool getUseTextures()
    {
        return useTexture;
    }
    public void SetRandomPosMode(Transform objectToMove, UseTarget useRandomTargetPosition)
    {
        //Toggelt die RandomTargetPos der einzelnen Level ( falls benötigt). 
        if (useRandomTargetPosition == UseTarget.Off)
        {
            return;
        }
        switch (useRandomTargetPosition)
        {
            case UseTarget.Level123:
                RandomTargetPos(objectToMove);
                break;
            case UseTarget.Level4:
                SetRandomGroundPos(-4, 5);
                break;
            case UseTarget.Level4V2:
                SetRandomGroundPos(-5, 6);
                break;
            case UseTarget.Level4OC:
                SetRandomGroundPosOC(-4, 5);
                break;
            case UseTarget.Level10:
                CreateRandomGroundsLevel10();
                bool found = false;
                do
                {
                    foreach (var pos in usedPositions)
                    {
                        if ((pos.x >= 34 && pos.x <= 37 || pos.x <= -34 && pos.x >= -37) ||
                            (pos.z >= 34 && pos.z <= 37 || pos.z <= -34 && pos.z >= -37))
                        {
                            Vector3 newPos = pos;
                            newPos.y = 2.5f;
                            objectToMove.localPosition = newPos;
                            found = true;
                            break;
                        }
                    }
                } while (!found);
                break;
            case UseTarget.Level11:
                RandomTargetPos(objectToMove);
                break;
            case UseTarget.Level12:
                sideLvl12=RandomTargetPosLvl12(objectToMove,null);
                break;
            case UseTarget.Generalisierung01:
                RandomTargetPos(objectToMove);
                break;
            default:
                return;
        }
    }
    public void DeactivateRandomTargets()
    {
        //Deaktiviert Random Targets(bzw checkpoints) und verschiebt diese in ein leeres gameobject holder.
        if (!deactivateRndmTargetsOnLvlBeginn)
        {
            return;
        }
        if (disabledTargetHolderObj == null || targetHolderObj == null)
        {
            Debug.LogError("disabledTargetHolderObj oder targetHolderObj wurde nicht zugewiesen");
            return;
        }

        List<Transform> SortChilds = new List<Transform>();
        foreach (Transform child in disabledTargetHolderObj.transform) //zu beginn alle aktivieren
        {
            Vector3 pos = child.transform.localPosition;
            child.SetParent(targetHolderObj.transform, worldPositionStays: true);
            child.transform.localPosition = pos;
        }
        foreach (Transform child in targetHolderObj.transform)
        {
            SortChilds.Add(child);
        }
        SortChilds = SortChilds.OrderBy(t => t.position.z).ToList(); //dann Sortieren

        List<Transform> activeChildren = new List<Transform>();
        foreach (Transform child in SortChilds) //dann aktivieren
        {
            child.SetSiblingIndex(SortChilds.IndexOf(child)); 
            activeChildren.Add(child);
        }


        int x = Random.Range(1, activeChildren.Count - 1);

        for (int i = 0; i < x; i++) //dann eine random anzahl wieder deaktivieren
        {
            int index = Random.Range(0, activeChildren.Count);
            Transform selected = activeChildren[index];
            Vector3 pos = selected.transform.localPosition;
            selected.SetParent(disabledTargetHolderObj.transform, worldPositionStays: true);// verschieben
            selected.transform.localPosition = pos;
            selected.gameObject.SetActive(false); // deaktivieren
            activeChildren.RemoveAt(index); // aus liste entfernen
        }
        if (trackCheckpoints != null)
        {
            trackCheckpoints.CollectSingleCheckpoints();
        }
        else
        {
            Debug.LogError("trackCheckpoints wurde nicht zugewiesen");
        }

    }
    public void SetRandomGroundPosOC(int min, int max)
    {
        //falls option OC (One Checkpoint) aktiv ist aber dennoch random ground pos (bzw original random target pos) benutzt werden soll
        for (int i = 0; i < groundList.Count; i++)
        {
            int x = Random.Range(min, max);
            groundList[i].localPosition = new Vector3(x, groundList[i].localPosition.y, groundList[i].localPosition.z);
            targetList[0].localPosition = new Vector3(x, targetList[0].localPosition.y, targetList[0].localPosition.z);

        }
    }
    public void SetRandomGroundPos(int min, int max)
    { 
        //bewegt Ground auf random pos (nut bei bestimmten leveln, glaub 4 und 7)
        for (int i = 0; i < groundList.Count; i++)
        {
            int x = Random.Range(min, max);
            groundList[i].localPosition = new Vector3(x, groundList[i].localPosition.y, groundList[i].localPosition.z);
            targetList[i].localPosition = new Vector3(x, targetList[i].localPosition.y, targetList[i].localPosition.z);
        }
    }
    public void RandomTargetPos(Transform obectToMove)
    {
        float x = 0;
        float z = 0;
        switch (Random.Range(0, 4))
        {
            case 0: //rechts
                x += Random.Range(-33f, 38f);
                z += Random.Range(27f, 34.5f);
                break;
            case 1: //links
                x += Random.Range(-33f, 38f);
                z += Random.Range(-39f, -27f);
                break;
            case 2: //oben
                x += Random.Range(-27f, -35f);
                z += Random.Range(-37f, 33f);
                break;
            case 3: //unten
                x += Random.Range(27f, 39f);
                z += Random.Range(-37f, 33f);
                break;
            default:
                return;
        }
        obectToMove.localPosition = new Vector3(x, 1f, z);
    }
    public int RandomTargetPosLvl12(Transform obectToMove,int? sideParam)
    {
        float x = 0;
        float z = 0;
        int side = sideParam ?? Random.Range(0, 4);
        switch (side)
        {
            case 0: //rechts
                x += Random.Range(-33f, 38f);
                z += Random.Range(27f, 34.5f);
                side = 0;
                break;
            case 1: //links
                x += Random.Range(-33f, 38f);
                z += Random.Range(-39f, -27f);
                side = 1;
                break;
            case 2: //oben
                x += Random.Range(-27f, -35f);
                z += Random.Range(-37f, 33f);
                side = 2;
                break;
            case 3: //unten
                x += Random.Range(27f, 39f);
                z += Random.Range(-37f, 33f);
                side = 3;
                break;
            default:
                return -1;
        }
        obectToMove.localPosition = new Vector3(x, 1f, z);
        return side;
    }

    #region Level 10
    private void CreateRandomGroundsLevel10() //erzeugt grounds an zufälligen positionen... // level zurzeit nicht in Benutzung //altes level 10... (level99 oder so in unity editor)
    {
        usedPositions.Clear();
        foreach (Transform child in groundHolderObj.transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < amount; i++)
        {
            Transform obj = Instantiate(groundPrefab, groundHolderObj.transform).transform;
            RandomTargetPosLevel10a(obj);
        }

    }
    public void RandomTargetPosLevel10(Transform objectToMove)
    {
        float rangeS = 10f, rangeB = 37f;
        Vector3 pos;
        int attempts = 0;

        do
        {
            int dir = Random.Range(0, 4);
            bool posX = dir == 0 || dir == 3;
            bool posZ = dir == 0 || dir == 1;

            float x = Random.Range(rangeS, rangeB) * (posX ? 1 : -1);
            float z = Random.Range(rangeS, rangeB) * (posZ ? 1 : -1);

            pos = new Vector3(x, 0f, z);
            attempts++;

        } while (usedPositions.Contains(pos) && attempts < 100);

        usedPositions.Add(pos);
        objectToMove.localPosition = pos;
    }

    bool IsFarEnough(Vector3 pos)
    {
        foreach (var used in usedPositions)
        {
            if (Mathf.Abs(pos.x - used.x) < 5f && Mathf.Abs(pos.z - used.z) < 5f)
                return false;
        }
        return true;
    }
    public void RandomTargetPosLevel10a(Transform objectToMove)
    {
        float rangeS = 8f, rangeB = 37f;
        Vector3 pos;
        int attempts = 0;

        do
        {
            float x = 0f, z = 0f;
            switch (Random.Range(0, 4))
            {
                case 0:
                    x += Random.Range(rangeS, rangeB);
                    z += Random.Range(-rangeB, rangeB);
                    break;   // rechts
                case 1:
                    x += Random.Range(-rangeS, -rangeB);
                    z += Random.Range(-rangeB, rangeB);
                    break; // links
                case 2:
                    x += Random.Range(-rangeB, rangeB);
                    z += Random.Range(rangeS, rangeB);
                    break; // oben
                case 3:
                    x += Random.Range(-rangeB, rangeB);
                    z += Random.Range(-rangeS, -rangeB);
                    break;   // unten
            }
            pos = new Vector3(x, 0f, z);
            attempts++;
            if (attempts == 29)
            {
                Debug.Log("zu viel versuche");
            }
        } while (!IsFarEnough(pos) && attempts < 30);

        usedPositions.Add(pos);
        objectToMove.localPosition = pos;
    }
    #endregion Level 10

}
