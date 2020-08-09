using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AutoDirection
{
    none,
    up,
    left,
    down,
    right    
}

public class AutoMovement : MonoBehaviour
{
    [SerializeField] private Sprite[] _buttonSprites;
    [SerializeField] private Image _buttonImage;

    private int _currentSpriteIndex = 0;

    private void Start() {
        _buttonImage.sprite = _buttonSprites[_currentSpriteIndex];
    }

    public void OnClick()
    {
        var selectedBase = BaseSelection.BaseSelected.GetComponent<Base>();
        
        if (selectedBase.currentAutoDirection == AutoDirection.right)
        {
            selectedBase.currentAutoDirection = AutoDirection.none;
            _buttonImage.sprite = _buttonSprites[0];
            _currentSpriteIndex = 0;
        }
        
        else
        {
            selectedBase.currentAutoDirection += 1;
            _currentSpriteIndex += 1;
            _buttonImage.sprite = _buttonSprites[_currentSpriteIndex];
            selectedBase.autoMovementTargetBase = GetAutoMovementTargetBase(selectedBase);
        }        
        print("Current Auto Movement = " + selectedBase.currentAutoDirection + " autoMovementTargetBase = " + selectedBase.autoMovementTargetBase.name);        
    }


    private Base GetAutoMovementTargetBase(Base selectedBase)
    {
        var currentAutoDirection = selectedBase.currentAutoDirection;
        var connectedBases = selectedBase.ConnectedBases;
        print("Connected base count = " + connectedBases.Count);
        
        if (currentAutoDirection == AutoDirection.up)
        {
            foreach (var connectedBase in connectedBases)
            {
                print("Go to foreach loop to find upper connectedBase");
                if (connectedBase.gameObject.transform.position.y > selectedBase.gameObject.transform.position.y)
                    return connectedBase;
            }
            print("Didn't find upper base");
            return null;
        }

        if (currentAutoDirection == AutoDirection.left)
        {
            foreach (var connectedBase in connectedBases)
            {
                if (connectedBase.transform.position.x > selectedBase.transform.position.x)
                    return connectedBase;
            }
            return null;
        }

        if (currentAutoDirection == AutoDirection.down)
        {
            foreach (var connectedBase in connectedBases)
            {
                if (connectedBase.transform.position.y < selectedBase.transform.position.y)
                    return connectedBase;
            }
            return null;
        }

        if (currentAutoDirection == AutoDirection.right)
        {
            foreach (var connectedBase in connectedBases)
            {
                if (connectedBase.transform.position.x < selectedBase.transform.position.x)
                    return connectedBase;
            }
            return null;
        }
        
        return null;
    }
}
