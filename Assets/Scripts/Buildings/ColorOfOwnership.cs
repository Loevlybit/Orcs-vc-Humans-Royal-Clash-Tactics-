using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorOfOwnership : MonoBehaviour
{
    
    private Base _myBase;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private Color playerColor = Color.black;
    private Color AIColor = Color.white; 
    
    
    // Start is called before the first frame update
    void Start()
    {
        _myBase = GetComponent<Base>();
        _myBase.OnOwnerChange += OnOwnerChange;
        SetOwnerColor();
        
    }

    private void OnOwnerChange(object sender, System.EventArgs e)
    {
        SetOwnerColor();
    }

    private void SetOwnerColor()
    {
        //print("OnOwnerChange");
        if (_myBase.Owner == Owner.Player)
        {
            //print("Current Owner is player");
            _spriteRenderer.color = Color.black;
        }
        else if (_myBase.Owner == Owner.AI)
        {
            //print("Current Owner is AI");
            _spriteRenderer.color = AIColor;
        }
    }
}
