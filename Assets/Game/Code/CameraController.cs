using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    public Transform cameraPivot;

    public Transform target;
    public float smoothingValue = 5.0f;
    public bool direct = false;

    [HideInInspector]
    public Camera mainCam;

    void Awake()
    {
        if (Debug.isDebugBuild)
        {
            Debug.developerConsoleVisible = false;
        }

        instance = this;
        mainCam = GetComponent<Camera>();
        cameraPivot = transform.parent;
    }

    // Use this for initialization
    void Start()
    {
    }

    void Update()
    {
        if (target)
        {
            Vector3 targetPosition = target.position;
            if (direct)
                cameraPivot.position = targetPosition;
            else //smoothing
                cameraPivot.position -= (cameraPivot.position - targetPosition) * smoothingValue * Time.deltaTime;
        }

    }

    public void SetTarget(GameObject obj)
    {
        try
        {
            target = obj.transform.Find("_CamTarget");
        }
        catch
        {
            target = obj.transform;
        }
    }
}
