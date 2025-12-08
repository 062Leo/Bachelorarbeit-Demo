using UnityEngine;

[CreateAssetMenu(fileName = "BackAndForth", menuName = "Custom/MovingObstacleData/BackAndForth")]
public class BackAndForth : ScriptableObject //scale zurzeit nicht in benutzung.. InitializeMovingObstacles methode in EnvController auskommentiert
{
    [Header("Back and Forth X Settings")]
    [Range(5f, 40f)] public float moveSpeedX = 10f;
    [Range(7f, 37f)] public float moveLimitX = 15f;
    [Range(5f, 25f)] public float scaleX = 10f;

    [Header("Back and Forth X2 Settings")]
    [Range(5f, 40f)] public float moveSpeedX2 = 10f;
    [Range(7f, 37f)] public float moveLimitX2 = 15f;
    [Range(5f, 25f)] public float scaleX2 = 4f;

    [Header("Back and Forth Z Settings")]
    [Range(5f, 40f)] public float moveSpeedZ = 10f;
    [Range(7f, 37f)] public float moveLimitZ = 15f;
    [Range(5f, 25f)] public float scaleZ = 10f;

    [Header("Back and Forth Z2 Settings")]
    [Range(5f, 40f)] public float moveSpeedZ2 = 10f;
    [Range(7f, 37f)] public float moveLimitZ2 = 15f;
    [Range(5f, 25f)] public float scaleZ2 = 4f;
}

[CreateAssetMenu(fileName = "RotateAroundPoint", menuName = "Custom/MovingObstacleData/RotateAroundPoint")]
public class RotateAroundPoint : ScriptableObject
{
    [Header("Rotate Around Point Settings")]
    public float[] rotateAroundRadius; //15,10
    public float[] rotateAroundSpeed;  //100,80
    public EnviromentController.RotationDirection[] rotateDirRAP;
}

[CreateAssetMenu(fileName = "RAPAndRS", menuName = "Custom/MovingObstacleData/RAPAndRS")]
public class RAPAndRS : ScriptableObject
{
    [Header("Rotate Around Point Settings")]
    public float[] rotateAroundRadius; //15,10
    public float[] rotateAroundSpeed;  //100,80
    public EnviromentController.RotationDirection[] rotateDirRAP;

    [Header("Rotate Self Settings")]
    public float[] selfRotateSpeed; //100,80
    public EnviromentController.RotationDirection[] rotateDirRS;

}
/* //SO_Lvl11V2 daten 
 * 11V2:
 * 10,17,20,23,26,29,31,33 RAR
 * 80,85,90,95,90,85,90,85 RAS
 * 
*/

/* //SO_Lvl11V3 daten 
 * 11V3:
 * 12,12,12,12 rar
 * 50,70,50,70 ras
 * 90,120,90,120 srs
 * 
*/