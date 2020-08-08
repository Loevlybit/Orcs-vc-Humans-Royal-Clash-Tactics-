using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject backgroundTilePrefab;
    [SerializeField] private GameObject bridgeTilePrefab;
    [SerializeField] private GameObject baseBottomTilePrefab;
    [SerializeField] private GameObject baseTopTilePrefab;
    [SerializeField] private Transform backgroundParentTransform, bridgesParentTransform, basesBottomParentTransform, basesTopParentTransform;
    
    [SerializeField] private int numberOfBases = 40;
    private Vector3 startPos = new Vector3(0, 0, 0);
    private Vector3 offset = new Vector3(2, 0, 0);

    private void Start() {
        GenerateMap();
    }
    
    private void GenerateMap()
    {
        GenerateBackground(backgroundTilePrefab);
        GenerateBridges(bridgeTilePrefab);
        GenerateBottomBases(baseBottomTilePrefab);
        GenerateTopBases(baseTopTilePrefab);
    }

    private void GenerateBackground(GameObject backgroundTile)
    {
        GameObject go = Instantiate(backgroundTile);
        go.transform.SetParent(backgroundParentTransform);
    }

    private void GenerateBridges(GameObject bridgeTile)
    {
        GenerateVerticalBridges(bridgeTile);
        GenerateHorizontalBridges(bridgeTile);
    }

    private void GenerateVerticalBridges(GameObject bridgeTile)
    {
        startPos = bridgeTile.transform.position;

        int baseCount = 0;
        for (int i = 0; i < numberOfBases - 9; i++) // we don't need top raw
        {            

            if (i % 9 == 0 && i != 0) 
            {
                baseCount = 0;
                startPos.y += 2;
            }
            
            GameObject go = Instantiate(bridgeTile, startPos + offset * baseCount, Quaternion.identity);
            go.transform.SetParent(bridgesParentTransform);
            baseCount++;
        }
    }

    private void GenerateHorizontalBridges(GameObject bridgeTile)
    {
        startPos = bridgeTile.transform.position;
        var newStartPos = new Vector3(startPos.y, startPos.x, 0);
        startPos = newStartPos;

        int baseCount = 0;
        for (int i = 0; i < numberOfBases - 5; i++) // we dont need the last right raw
        {            

            if (i % 8 == 0 && i != 0) 
            {
                baseCount = 0;
                startPos.y += 2;
            }
            
            GameObject go = Instantiate(bridgeTile, startPos + offset * baseCount, Quaternion.Euler(0, 0, 90f));
            go.transform.SetParent(bridgesParentTransform);
            baseCount++;
        }
    }

    private void GenerateBottomBases(GameObject baseBottomTile)
    {
        startPos = baseBottomTile.transform.position;
        
        int baseCount = 0;
        for (int i = 0; i < numberOfBases; i++)
        {            

            if (i % 9 == 0 && i != 0) 
            {
                baseCount = 0;
                startPos.y += 2;
            }
            
            GameObject go = Instantiate(baseBottomTile, startPos + offset * baseCount, Quaternion.identity);
            go.transform.SetParent(basesBottomParentTransform);
            baseCount++;
        }
    }

    private void GenerateTopBases(GameObject baseTopTile)
    {
        //startPos = baseTopTile.transform.position;
        startPos = new Vector3 (0, 0, 0);

        int baseCount = 0;
        for (int i = 0; i < numberOfBases; i++)
        {            

            if (i % 9 == 0 && i != 0) 
            {
                baseCount = 0;
                startPos.y += 2;
            }
            
            GameObject go = Instantiate(baseTopTile, startPos + offset * baseCount, Quaternion.identity);
            go.transform.SetParent(basesTopParentTransform);
            baseCount++;
        }
    }
}
