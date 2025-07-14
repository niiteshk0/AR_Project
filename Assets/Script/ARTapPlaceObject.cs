using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
public class ARTapPlaceObject : MonoBehaviour
{

    [SerializeField] GameObject footBallPrefab;
    [SerializeField] GameObject goalPrefab;
    [SerializeField] GameObject plane;
    [SerializeField] GameObject placementIndicator;
    [SerializeField] float force;
    private GameObject spawnedBall;
    private GameObject spawnedGoal;
    private XROrigin xrOrigin;
    private ARRaycastManager raycastManager;
    private Pose placementPose;
    private bool isPlaced = false;
    private bool hasSpawnedGoal = false;
    private Vector2 startTouch;
    private Vector2 endTouch;
    
    void Start()
    {
        xrOrigin = FindFirstObjectByType<XROrigin>();
        raycastManager = FindFirstObjectByType<ARRaycastManager>();

        // For the Physics plane
        GameObject planePhysics = Instantiate(plane, placementIndicator.transform.position, plane.transform.rotation);
        planePhysics.transform.position = new Vector3(placementIndicator.transform.position.x, -0.1f, placementIndicator.transform.position.z);
        planePhysics.transform.localScale = new Vector3(10, 0, 10);
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (isPlaced && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
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
        spawnedBall = GetComponent<BallSpawner>().GetBall();
        spawnedBall.SetActive(true);

        Vector3 footBallSpawnPos = placementIndicator.transform.position;
        spawnedBall.transform.position = footBallSpawnPos;        
        
        // spawnedBall = Instantiate(footBallPrefab, placementIndicator.transform.position, footBallPrefab.transform.rotation);

        if (!hasSpawnedGoal)
        {
            // For the GoalNet
            Vector3 goalPosition = placementPose.position + placementPose.forward * 3f;

            Vector3 cameraPos = xrOrigin.Camera.transform.position;

            Vector3 directionToCamera = ( cameraPos- goalPosition).normalized;
            // directionToCamera.y = 0;

            Quaternion goalRotation = Quaternion.LookRotation(directionToCamera);
            spawnedGoal = Instantiate(goalPrefab, goalPosition, goalPrefab.transform.rotation);
        }

        hasSpawnedGoal = true;
    }
    void KickBall()
    {
        Vector2 swipe = endTouch - startTouch;
        Vector3 swipeDirection = new Vector3(swipe.x, swipe.y, 1).normalized;

        Rigidbody rb = spawnedBall.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(Camera.main.transform.TransformDirection(swipeDirection) * force, ForceMode.Impulse);
        }
        StartCoroutine(DeactivateAfterTime(spawnedBall, 4f));
    }

    IEnumerator DeactivateAfterTime(GameObject ball, float delay)
    {
        yield return new WaitForSeconds(delay);

         if (ball.activeInHierarchy)
        {
            ball.SetActive(false);
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
