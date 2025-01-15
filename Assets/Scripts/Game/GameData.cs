using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameData
{
    public static Scene currentScene;

    public static bool vectorCloseToVector(Vector3 vector1, Vector3 vector2, float accuracy = 0.1f)
    {
        return Mathf.Abs(vector1.x - vector2.x) < accuracy
                && Mathf.Abs(vector1.z - vector2.z) < accuracy;
    }

    public static Vector3 getDimensions(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds.size;
        }
        else
        {
            Debug.LogWarning($"Object {obj.name} does not have a Renderer component.");
            return Vector3.zero;
        }
    }

    public static void matchDimensions(GameObject obj, Vector3 targetSize)
    {
        Mesh m = obj.GetComponent<MeshFilter>().sharedMesh;
        Bounds meshBounds = m.bounds;
        Vector3 meshSize = meshBounds.size;
        float xScale = targetSize.x / meshSize.x;
        float yScale = targetSize.y / meshSize.y;
        float zScale = targetSize.z / meshSize.z;
        obj.transform.localScale = new Vector3(xScale, yScale, zScale);
    }

    public static List<Color> colorGradient(Color firstColor, Color lastColor, int length)
    {
        List<Color> colorGradient = new List<Color>();

        if (length < 2)
        {
            colorGradient.Add(firstColor);
            return colorGradient;
        }

        Vector3 firstColorHSV = Vector3.zero;
        Vector3 lastColorHSV = Vector3.zero;
        Color.RGBToHSV(firstColor, out firstColorHSV.x, out firstColorHSV.y, out firstColorHSV.z);
        Color.RGBToHSV(lastColor, out lastColorHSV.x, out lastColorHSV.y, out lastColorHSV.z);
        Vector3 colorStepHSV = (lastColorHSV - firstColorHSV) / (length - 1);
        
        for (int i = 0; i < length; i++)
        {
            Vector3 newColorHSV = firstColorHSV + i * colorStepHSV;
            Color newColor = Color.HSVToRGB(newColorHSV.x, newColorHSV.y, newColorHSV.z);

            colorGradient.Add(newColor);
        }

        return colorGradient;
    }
}

