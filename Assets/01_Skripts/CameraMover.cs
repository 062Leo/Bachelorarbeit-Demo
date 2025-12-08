using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public float normalSpeed = 30.0f;
    public float fastSpeedMultiplier = 2.0f;
    public float sensitivity = 2.0f;

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;
    private float currentSpeed;
    private float timeSinceStart = 0.0f;

    [SerializeField] private GameObject pauseMenu;
    void Start()
    {
        rotationX = 73.4990311f;
        rotationY = 0f;
        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }

    void Update()
    {
        timeSinceStart += Time.deltaTime;
        
        if (pauseMenu.activeSelf||timeSinceStart < 1.0f)
        {
            return;
        }
       
        
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = normalSpeed * fastSpeedMultiplier;
        }
        else
        {
            currentSpeed = normalSpeed;
        }

        float xMovement = Input.GetAxisRaw("Horizontal") * currentSpeed * Time.deltaTime;
        float zMovement = Input.GetAxisRaw("Vertical") * currentSpeed * Time.deltaTime;

        float yMovement = 0f;
        if (Input.GetKey(KeyCode.Space)) // Nach oben
        {
            yMovement = currentSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftControl)) // Nach unten
        {
            yMovement = -currentSpeed * Time.deltaTime;
        }

        transform.position += transform.right * xMovement + transform.forward * zMovement + Vector3.up * yMovement;

        rotationY += Input.GetAxis("Mouse X") * sensitivity;
        rotationX -= Input.GetAxis("Mouse Y") * sensitivity;

        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0.0f);
    }
}