using System.Collections.Generic;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
public class ARTapPlaceObject : MonoBehaviour
{

    public GameObject footBallPrefab;
    public GameObject goalPrefab;
    private GameObject spawnedBall;
    private GameObject spawnedGoal;
    public GameObject placementIndicator;
    private XROrigin xrOrigin;
    private ARRaycastManager raycastManager;
    private Pose placementPose;
    private bool isPlaced = false;
    private bool hasSpawnedObject = false;
    private Vector2 startTouch;
    private Vector2 endTouch;

    void Start()
    {
        xrOrigin = FindFirstObjectByType<XROrigin>();
        raycastManager = FindFirstObjectByType<ARRaycastManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (!hasSpawnedObject && isPlaced && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceObject();
        }

        if (spawnedBall != null && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouch = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                endTouch = touch.position;
                KickBall();
            }
        }

    }

    void PlaceObject()
    {
        // For the FootBall
        spawnedBall = Instantiate(footBallPrefab, placementIndicator.transform.position, footBallPrefab.transform.rotation);

        // For the GoalNet
        Vector3 goalPosition = placementPose.position + placementPose.forward * 4f;
        spawnedGoal = Instantiate(goalPrefab, goalPosition, Quaternion.LookRotation(new Vector3(90,0,0)));

        hasSpawnedObject = true;
 
    }
    void KickBall()
    {
        Vector2 swipe = endTouch - startTouch;
        Vector3 swipeDirection = new Vector3(swipe.x, swipe.y, 1).normalized;

        Rigidbody rb = spawnedBall.GetComponent<Rigidbody>();

        if (rb != null)
        {
            float force = 8f;
            rb.AddForce(Camera.main.transform.TransformDirection(swipeDirection) * force, ForceMode.Impulse);
        }
        
    }


    void UpdatePlacementPose(){
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        isPlaced = hits.Count > 0;

        if (isPlaced)
        {
            Debug.Log("Hit");
            placementPose = hits[0].pose;

            var cameraForward = Camera.main.transform.forward;

            // This handle for up and down direction when camera seeing up and down then not change indicator direction
            var cameraBearingUpDown = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;

            placementPose.rotation = Quaternion.LookRotation(cameraBearingUpDown);
        }
    }

    void UpdatePlacementIndicator(){
        
        if (isPlaced)
        {
            Debug.Log("Placed");
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }    
    }
}
