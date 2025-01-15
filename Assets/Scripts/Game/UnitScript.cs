using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitScript : MonoBehaviour
{
    [SerializeField] GameObject troopPrefab;
    [SerializeField] GameObject chiefPrefab;
    [SerializeField] GameObject boxPrefab;

    public Color chiefColor;
    public Color soldierColor;

    public int unitLength = 8;
    public float distanceBetweenTroops = 3;

    public Vector3 destination;

    private GameObject chief;

    private List<GameObject> boxes;


    /////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    /// MONOBEHAVIOUR
    /// 
    /////////////////////////////////////////////////////////////////////////////////////////////////////

    private void Start()
    {
        boxes = new List<GameObject>();
        GameObject box = Instantiate(boxPrefab, transform.GetChild(0));
        boxes.Add(box);
        chief = Instantiate(chiefPrefab, box.transform.position, Quaternion.identity, transform);
        chief.GetComponent<Renderer>().material.SetColor("_BaseColor", chiefColor);
        destination = chief.transform.position;
        Debug.Log(destination);

        generateTroops();

        //StartCoroutine(CalcVelocity());
    }

    private void Update()
    {
        transform.GetChild(0).position = destination;
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    /// GENERATION
    /// 
    /////////////////////////////////////////////////////////////////////////////////////////////////////

    private void generateTroops()
    {
        Vector3 localPosition = distanceBetweenTroops * new Vector3(0, 0, 1);
        Vector3 direction = new Vector3(1, 0, 0);

        for (int i = 0; i < unitLength; i++)
        {
            if (i != 0)
            {
                if (canTurnRight(localPosition, direction))
                {
                    direction = turnRight(direction);
                }

                localPosition += distanceBetweenTroops * direction;
            }

            Vector3 boxPosition = transform.GetChild(0).position + localPosition;

            GameObject box = Instantiate(boxPrefab, boxPosition, Quaternion.identity, transform.GetChild(0));
            GameObject troop = Instantiate(troopPrefab, boxPosition, Quaternion.identity, transform);

            troop.name = "Troop " + (i + 1);
            box.name = "Box " + (i + 1);

            troop.GetComponent<Renderer>().material.SetColor("_BaseColor", soldierColor);
            troop.GetComponent<TroopClass>().box = box;

            boxes.Add(box);
        }
    }

    private Vector3 turnRight(Vector3 direction)
    {
        return Vector3.Cross(Vector3.up, direction);
    }

    private bool canTurnRight(Vector3 troopPosition, Vector3 direction)
    {
        foreach (GameObject box in boxes)
        {
            Vector3 newPos = troopPosition + distanceBetweenTroops * turnRight(direction);

            if (GameData.vectorCloseToVector(newPos, box.transform.localPosition))
            {
                return false;
            }
        }

        return true;
    }
}
