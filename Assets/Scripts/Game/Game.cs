using System.Collections.Generic;
using UnityEngine;
using System;

public class Game : MonoBehaviour
{
    [SerializeField] private GameObject canvas;

    [SerializeField] private string mapTag;

    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private string unitStr;

    [SerializeField, Range(0, 80)] private List<int> unitLengths;

    [SerializeField] private List<Vector2Int> positions;

    [SerializeField] private Color firstColorChief;
    [SerializeField] private Color lastColorChief;
    [SerializeField] private Color firstColorSoldiers;
    [SerializeField] private Color lastColorSoldiers;

    private List<Color> colorGradientChief;
    private List<Color> colorGradientSoldiers;

    private Dictionary<Vector2Int, Vector3> gridToWorld;

    private Vector3 mapDim;
    private float squareDim;

    private List<UnitScript> units;
    private int currUnitIdx = -1;

    private bool isPaused = false;


    /////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    /// MONOBEHAVIOUR
    /// 
    /////////////////////////////////////////////////////////////////////////////////////////////////////

    private void Start()
    {
        canvas.SetActive(false);
        mapGrid();
        generateUnits();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            handleClick();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    /// GENERATING
    /// 
    /////////////////////////////////////////////////////////////////////////////////////////////////////

    private void generateUnits()
    {
        units = new List<UnitScript>();

        colorGradientChief = GameData.colorGradient(firstColorChief, lastColorChief, positions.Count);
        colorGradientSoldiers = GameData.colorGradient(firstColorSoldiers, lastColorSoldiers, positions.Count);

        for (int i = 0; i < positions.Count; i++)
        {
            Vector3 position = GetWorldPosition(positions[i]) + Vector3.up;

            UnitScript unit = Instantiate(unitPrefab, position, Quaternion.identity).GetComponent<UnitScript>();
            setColorUnit(unit, colorGradientChief[i], colorGradientSoldiers[i]);
            setLength(unit.gameObject, i);
            unit.name = unitStr + i;

            units.Add(unit);
        }
    }

    private void setLength(GameObject unit, int i)
    {
        UnitScript unitScript = unit.GetComponent<UnitScript>();
        int length;
        float factor;
        try
        {
            length = unitLengths[i];
            factor = Mathf.Max(Mathf.Ceil(Mathf.Sqrt(length + 1)), 3);
        }
        catch (ArgumentOutOfRangeException)
        {
            length = 8;
            factor = 3;
        }
        unitScript.unitLength = length;
        unitScript.distanceBetweenTroops = 0.9f * squareDim / factor;
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    /// GRID
    /// 
    /////////////////////////////////////////////////////////////////////////////////////////////////////

    private void mapGrid()
    {
        gridToWorld = new Dictionary<Vector2Int, Vector3>();

        GameObject map = GameObject.FindGameObjectWithTag(mapTag);
        mapDim = GameData.getDimensions(map);
        squareDim = mapDim.x / gridSize.x;

        Vector3 mapOffset = new Vector3(-mapDim.x / 2, 0, -mapDim.z / 2);

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.y; z++)
            {
                Vector2Int gridCoords = new Vector2Int(x, z);
                Vector3 worldPosition = new Vector3(
                    (x + 0.5f) * squareDim,
                    100f,
                    (z + 0.5f) * squareDim
                ) + mapOffset;

                if (Physics.Raycast(
                        new Vector3(worldPosition.x, 100f, worldPosition.z),
                        Vector3.down,
                        out RaycastHit hit,
                        Mathf.Infinity,
                        groundLayer))
                {
                    worldPosition.y = hit.point.y;
                }
                else
                {
                    Debug.LogWarning($"No ground detected at grid {gridCoords}");
                    continue;
                }

                gridToWorld[gridCoords] = worldPosition;
            }
        }

        Debug.Log($"Grid initialized with {gridToWorld.Count} positions.");
    }

    public Vector3 GetWorldPosition(Vector2Int gridCoords)
    {
        return gridToWorld.TryGetValue(gridCoords, out Vector3 position)
            ? position
            : Vector3.zero;
    }

    public Vector2Int GetGridCoordinates(Vector3 worldPosition)
    {
        Vector3 mapOffset = new Vector3(-mapDim.x / 2, 0, -mapDim.z / 2);
        Vector3 localPosition = worldPosition - mapOffset;

        int gridX = Mathf.FloorToInt(localPosition.x / squareDim);
        int gridZ = Mathf.FloorToInt(localPosition.z / squareDim);

        if (gridX < 0 || gridX >= gridSize.x || gridZ < 0 || gridZ >= gridSize.y)
        {
            return new Vector2Int(-1, -1);
        }

        return new Vector2Int(gridX, gridZ);
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    /// PAYER INPUT
    /// 
    /////////////////////////////////////////////////////////////////////////////////////////////////////

    private void handleClick()
    {
        Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(myRay, out RaycastHit hitInfo, Mathf.Infinity, groundLayer)
                && GameData.vectorCloseToVector(hitInfo.normal, Vector3.up, 0.1f))
        {
            Vector2Int squareCoords = GetGridCoordinates(hitInfo.point);

            for (int i = 0; i < positions.Count; i++)
            {
                Vector2Int squareDest = GetGridCoordinates(units[i].GetComponent<UnitScript>().destination);
                if (squareCoords[0] == squareDest[0] && squareCoords[1] == squareDest[1])
                {
                    if (currUnitIdx == -1)
                    {
                        currUnitIdx = i;
                        break;
                    }

                    if (currUnitIdx != i)
                    {
                        moveChief(i, units[currUnitIdx].destination);
                        moveChief(currUnitIdx, hitInfo.point);
                    }

                    addColorUnit(Color.clear);
                    currUnitIdx = -1;
                    break;
                }
            }
        }
        else
        {
            if (currUnitIdx != -1) addColorUnit(Color.clear);
            currUnitIdx = -1;
        }

        if (currUnitIdx != -1)
        {
            addColorUnit(0.5f * Color.white);
            moveChief(currUnitIdx, hitInfo.point);
        }
    }

    private void moveChief(int index, Vector3 dest)
    {
        Vector2Int coords = GetGridCoordinates(dest);
        Vector3 destination = GetWorldPosition(coords);

        units[index].GetComponent<UnitScript>().destination = destination;
    }

    public void Pause()
    {
        canvas.SetActive(!isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
        isPaused = !isPaused;
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    /// COLORING
    /// 
    /////////////////////////////////////////////////////////////////////////////////////////////////////

    private void setColorUnit(UnitScript unit, Color chiefColor, Color soldierColor)
    {
        unit.chiefColor = chiefColor;
        unit.soldierColor = soldierColor;
    }

    private void changeColorUnit(UnitScript unit, Color chiefColor, Color soldiersColor)
    {
        int length = unit.GetComponent<UnitScript>().unitLength + 1;

        for (int i = 0; i < length; i++)
        {
            Color color;
            GameObject soldier = unit.transform.GetChild(i + 1).gameObject;
            if (i == 0)
            {
                color = chiefColor;
            }
            else
            {
                color = soldiersColor;
            }
            soldier.GetComponent<Renderer>().material.SetColor("_BaseColor", color);
        }
    }

    private void addColorUnit(Color colorToAdd)
    {
        changeColorUnit(units[currUnitIdx], colorGradientChief[currUnitIdx] + colorToAdd, 
            colorGradientSoldiers[currUnitIdx] + colorToAdd);
    }
}
