using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BaseSelection
{
    
    private static GameObject _baseSelected;
    

    public static GameObject BaseSelected {get { return _baseSelected;} set { _baseSelected = value; }}
}
