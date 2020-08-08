using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private int _width;
    private int _height;
    
    // Start is called before the first frame update
    void Start()
    {
        _width = Screen.width;
        _height = Mathf.FloorToInt(_width / 16f * 9f);
        print("Width = " + _width + " Height = " + _height); 
    }

    
}
