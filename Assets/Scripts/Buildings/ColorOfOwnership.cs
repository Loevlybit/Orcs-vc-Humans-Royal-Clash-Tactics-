using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorOfOwnership : MonoBehaviour
{
    
    private Base _myBase;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private Color playerColor = Color.black;
    private Color AIColor = Color.white; 
    private Color neutralColor = Color.yellow;
    
    
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
            _spriteRenderer.color = playerColor;
        }
        else if (_myBase.Owner == Owner.AI)
        {
            _spriteRenderer.color = AIColor;
        }

        else if (_myBase.Owner == Owner.Neutral)
        {
            _spriteRenderer.color = neutralColor;
        }
    }
}
