using UnityEngine;

public class CameraClass : MonoBehaviour
{
    public GameObject target;//the target object
    public GameObject targetTarget;//the target's target

    [SerializeField, Range(1, 200)]  float minFieldOfView    = 15;
    [SerializeField, Range(1, 200)]  float middleFieldOfView = 45;
    [SerializeField, Range(1, 200)]  float maxFieldOfView    = 125;

    [SerializeField, Range(1, 50)]   float minZDistance      = 1;
    [SerializeField, Range(0, 100)]  float zDistance         = 20;
    [SerializeField, Range(1, 100)]  float maxZDistance      = 50;

    [SerializeField, Range(0, 45)]   float minAngleX         = 15;
    [SerializeField, Range(45, 90)]  float maxAngleX         = 60;

    [SerializeField, Range(0, 500)]  float turningSpeed      = 100.0f;
    [SerializeField, Range(0, 50)]    float transitionSpeed   = 5;

    [SerializeField] float timeBeforeStopTurning = 2;
    [SerializeField] float timeBeforeStartTurning = 1;

    private Vector3 point; //the coord to the point where the camera looks at
    private Vector3 targetPoint; //the coord to the point where the target looks at
    private Vector3 localZ;
    private Vector3 localX;

    private  enum RotationMode { turn, slowDown };
    private RotationMode rotationMode = RotationMode.turn;

    private float deltaTime = 0; //time since the last press on left/right btns
    private float deltaY = 0;
    private float turningSpeedTemp = 0;

    private bool startTransition = false;

    private float timePressingArrow = 0;
    private float transitionSpeedTemp;


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    /// MONOBEHAVIOUR
    /// 
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void Start()
    {
        serializeFieldGestion();

        transform.LookAt(point);//makes the camera look to it
    }

    void Update()
    {
        point = target.transform.position;//get target's coords
        targetPoint = targetTarget.transform.position;//get target's target's coords

        setLocalAxis();

        if (Camera.main.fieldOfView <= middleFieldOfView)
        {
            closeRangeMode();
        }
        else
        {
            longRangeMode();   
        }

        yRotation();
        zoom();
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    /// RANGE MODE
    /// 
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void closeRangeMode()
    {
        zDeplacement();
    }

    private void longRangeMode()
    {
        xRotation();
        
        Vector3 zPosition = targetPoint + zDistance * localZ;
        Vector3 travelVector = point - zPosition;
        float travel = Vector3.Dot(travelVector, localZ);

        if (Mathf.Abs(travel) > 1E-4)
        {
            transitToZDistance(travel);
        }
        else
        {
            target.transform.position = targetPoint + zDistance * localZ;
            startTransition = false;
        }
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    /// LOCAL AXIS
    /// 
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void setLocalAxis()
    {
        Vector3 temp = (point - targetPoint).normalized;
        if (temp != localZ && Vector3.Dot(temp, localZ) >= 0)
        {
            localZ = temp;
        }

        localX = Vector3.Cross(localZ, Vector3.up);
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    /// ROTATION
    /// 
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private static float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;

        return angle;
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    /// XROTATION
    /// 
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    private void xRotation()
    {
        float deltaX = turningSpeed * Time.fixedDeltaTime * Input.GetAxis("Vertical");
        float angleX = WrapAngle(transform.eulerAngles.x); //angle of rotation

        if (!(deltaX < 0 && angleX < minAngleX) && !(deltaX > 0 && angleX > maxAngleX)) //bounds of rotation
        {
            transform.RotateAround(point, localX, deltaX);
        }
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    /// YROTATION
    /// 
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void turn()
    {
        deltaY = -turningSpeed * Time.fixedDeltaTime * Input.GetAxis("Horizontal");
    }

    private void rotationRelease()
    {
        if (Input.GetKeyUp(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow)
            || Input.GetKeyUp(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
        {
            if (timePressingArrow >= timeBeforeStartTurning)
            {
                turningSpeedTemp = deltaY;
                rotationMode = RotationMode.slowDown;
            }
            timePressingArrow = 0;
        }
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            timePressingArrow += Time.deltaTime;
        }
    }

    private void slowDown()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)
            || deltaTime > timeBeforeStopTurning)
        {
            deltaTime = 0;
            rotationMode = RotationMode.turn;
        }
        else
        {
            deltaTime += Time.deltaTime;
            deltaY = turningSpeedTemp * (timeBeforeStopTurning - deltaTime) / timeBeforeStopTurning;
        }
    }

    private void yRotation()
    {
        if (rotationMode == RotationMode.turn)
        {
            turn();
            rotationRelease();
        }
        else if (rotationMode == RotationMode.slowDown)
        {
            slowDown();
        }

        target.transform.RotateAround(targetPoint, Vector3.up, deltaY);
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    /// ZOOM
    /// 
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void zoom()
    {
        float smooth = Camera.main.fieldOfView / maxFieldOfView;

        // -------------------Code for Zooming Out------------
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (Camera.main.fieldOfView <= maxFieldOfView)
                Camera.main.fieldOfView += 2 * smooth;
            if (Camera.main.orthographicSize <= 20)
                Camera.main.orthographicSize += 0.5f * smooth;

        }
        // ---------------Code for Zooming In------------------------
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (Camera.main.fieldOfView > minFieldOfView)
                Camera.main.fieldOfView -= 2 * smooth;
            if (Camera.main.orthographicSize >= 1)
                Camera.main.orthographicSize -= 0.5f * smooth;
        }
    }

    private void zDeplacement()
    {
        float upDown = -5f * Camera.main.fieldOfView / 8 * Time.fixedDeltaTime * Input.GetAxis("Vertical");
        float newPos = Vector3.Distance(point, targetPoint) + upDown;

        if (!(Mathf.Abs(newPos) < minZDistance && upDown < 0)
            && !(Mathf.Abs(newPos) > maxZDistance && upDown > 0))
        {
            target.transform.position = targetPoint + newPos * localZ;
        }
    }

    private void transitToZDistance(float travel)
    {
        if (startTransition)
        {
            transitionSpeedTemp = transitionSpeed * travel;
            Vector3 translation = -Time.deltaTime * transitionSpeedTemp * localZ;
            target.transform.Translate(translation, Space.World);
        }
        else
        {
            startTransition = true;
        }
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    /// SET BOUNDS
    /// 
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private void serializeFieldGestion()
    {
        if (minFieldOfView > maxFieldOfView)
        {
            float temp = minFieldOfView;
            minFieldOfView = maxFieldOfView;
            maxFieldOfView = temp;
        }
        if (middleFieldOfView < minFieldOfView || middleFieldOfView > maxFieldOfView)
        {
            middleFieldOfView = 0.5f * (minFieldOfView + maxFieldOfView);
        }

        if (minZDistance > maxZDistance)
        {
            float temp = minZDistance;
            minZDistance = maxZDistance;
            maxZDistance = temp;
        }
        if (zDistance < minZDistance || zDistance > maxZDistance)
        {
            zDistance = 0.5f * (minZDistance + maxZDistance);
        }
    }
}
