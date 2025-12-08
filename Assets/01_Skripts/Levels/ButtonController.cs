using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    private Material material;
    public bool isPressed;
    [SerializeField] private GameObject lavaWall;
    private Vector3 lavaWallPos;
    private void Awake()
    {
        isPressed = false;
        material = GetComponent<MeshRenderer>().material;
        lavaWallPos = lavaWall.transform.localPosition;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!isPressed)
            {
                SetColor(Color.green);
                lavaWall.SetActive(false);
                isPressed = true;
            }
        }
    }
    public void ResetButton()
    {
        RandomButtonPosSet(transform);
        isPressed = false;
        SetColor(Color.red);
        lavaWall.SetActive(true);
    }
    public void SetColor(Color color)
    {
        material.color = color;
    }
    
   
    private void RandomButtonPosSet(Transform obectToMove)
    {
        float x = 0;
        float z = 0;
        switch (Random.Range(0, 4))
        {
            case 0: // Oben
                x += Random.Range(-14f, 14f);
                z = 14f;
                break;
            case 1: // Unten
                x += Random.Range(-14f, 14f);
                z += -14f;
                break;
            case 2: // Links
                x += -14f;
                z += Random.Range(-14f, 14f);
                break;
            case 3: // Rechts
                x += 14f;
                z += Random.Range(-14f, 14f);
                break;
            default:
                return;
        }
        obectToMove.localPosition = new Vector3(x, obectToMove.localPosition.y, z);
    }
}
