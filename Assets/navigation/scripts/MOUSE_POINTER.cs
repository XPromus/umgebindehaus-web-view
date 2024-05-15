using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Timers;
using UnityEngine.Events;
using Lean.Touch;
using Lean.Common;

public class MOUSE_POINTER : MonoBehaviour
{


    [SerializeField]
    private GameObject DefaultCenter;

    [SerializeField]
    private GameObject Position;

    [SerializeField]
    private float doubleClickTime = .5f;
    private float lastClickTime;

    [SerializeField]
    private LayerMask IgnoreMe;

    public LeanMaintainDistance CameraDistance;
    public UnityEvent DoubleClick;
    public UnityEvent SetPivotEvent;

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            float timeSinceLastClick = Time.time - lastClickTime;

            if (timeSinceLastClick <= doubleClickTime)
            {
                Debug.Log("Double click");
                DoubleClick.Invoke();
            }

            lastClickTime = Time.time;
        }

        if (Input.GetMouseButtonDown(1))
        {
            float timeSinceLastClick = Time.time - lastClickTime;

            if (timeSinceLastClick <= doubleClickTime)
            {
                Debug.Log("Double click");
                DoubleClick.Invoke();
            }

            lastClickTime = Time.time;
        }
    }



    public void SetPivot()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 2500f, IgnoreMe))
        {
            Position.transform.position = hit.point;

            if (CameraDistance)
            {
                if (CameraDistance.Distance > 1)
                {
                    CameraDistance.Distance = hit.distance;
                }
            }

            Position.transform.position = hit.point;

            if (hit.normal.y == 1.0)
            {
                Position.transform.position = Position.transform.position + new Vector3(0, 1.5f, 0);
                // Debug.Log("normal=1");
            }

            Position.GetComponent<LeanSelectableByFinger>().SelfSelected = true;
            SetPivotEvent.Invoke();

        }
        else
        {
            Position.transform.position = DefaultCenter.transform.position;
            Position.GetComponent<LeanSelectableByFinger>().SelfSelected = true;
            // Debug.Log("reset pivot");
        }
    }
}

