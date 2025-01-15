using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapClass : MonoBehaviour
{
    [SerializeField] GameObject squarePrefab;

    /////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    /// MONOBEHAVIOUR
    /// 
    /////////////////////////////////////////////////////////////////////////////////////////////////////
    
    void Start()
    {
        generateSquares();
    }

    void Update()
    {
        
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    /// GENERATION
    /// 
    /////////////////////////////////////////////////////////////////////////////////////////////////////

    private void generateSquares()
    {
        Vector3 mapDimensions = Vector3.Scale(transform.localScale, GetComponent<MeshFilter>().mesh.bounds.size);
        Vector3 squareDimensions = 0.1f * mapDimensions;
        squareDimensions -= squareDimensions.y * Vector3.up;
        Vector3 squaresStep = 0.1f * mapDimensions;

        Vector3 startingPos = -0.5f * (1 - 0.1f) * mapDimensions;
        startingPos += (1.01f - startingPos.y) * Vector3.up;

        Vector3 position;

        int xMin = 1;
        int xMax = (int)mapDimensions.x / 10 - 1;
        int zMin = xMin;
        int zMax = xMax;

        for (int x = xMin; x < xMax; x++)
        {
            for (int z = zMin; z < zMax; z++)
            {
                position = startingPos + squaresStep.x * Vector3.right * x + squaresStep.z * Vector3.forward * z;
                GameObject square = Instantiate(squarePrefab, position, Quaternion.identity, GameObject.Find("Squares").transform);

                GameData.matchDimensions(square, squareDimensions);
                square.transform.localScale += Vector3.up;

                square.name = "Square " + x + z;
            }
        }
    }
}
