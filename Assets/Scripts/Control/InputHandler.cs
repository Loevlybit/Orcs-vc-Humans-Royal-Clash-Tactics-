using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private void Update() 
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick(Input.mousePosition);
        }
    }

    private void HandleMouseClick(Vector3 mousePos)
    {
        var worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        worldMousePos.z += 10f;
        //print(worldMousePos);
    }
}
