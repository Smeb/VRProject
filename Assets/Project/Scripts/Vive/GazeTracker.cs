using UnityEngine;

public class GazeTracker : MonoBehaviour
{
    public delegate void GazeEventHandler(GameObject target);
    public event GazeEventHandler GazeEnter;
    public event GazeEventHandler GazeExit;

    private GameObject currentTarget;

    // Contains a HMD tracked object that we can use to find the user's gaze
    Transform headTransform = null;

    public void OnGazeOn(GameObject target)
    {
        if (GazeEnter != null)
            GazeEnter(target);
    }

    public void OnGazeOff(GameObject target)
    {
        if (GazeExit != null)
            GazeExit(target);
    }

    // Update is called once per frame
	void Update ()
    {
        // If we haven't set up hmdTrackedObject find what the user is looking at
        if (headTransform == null)
        {
            SteamVR_Camera camera = FindObjectOfType<SteamVR_Camera>();
            headTransform = camera.head.transform;
        }

        if (headTransform)
        {
            Ray ray = new Ray(headTransform.position, headTransform.forward);
            RaycastHit raycastHit;

            if (Physics.Raycast(ray, out raycastHit, 10f, LayerMask.GetMask("Characters")))
            {
                GameObject target = raycastHit.transform.gameObject;
                if (currentTarget != raycastHit.transform.gameObject)
                {
                    OnGazeOff(currentTarget);
                    currentTarget = null;
                }

                if (currentTarget == null)
                {
                    currentTarget = raycastHit.transform.gameObject;
                    OnGazeOn(currentTarget);
                }
            }
            else
            {
                if (currentTarget != null)
                {
                    OnGazeOff(currentTarget);
                    currentTarget = null;
                }
            }
        }
    }
}
