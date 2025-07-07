using System.Collections.Generic;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
public class ARTapPlaceObject : MonoBehaviour
{

    public GameObject objectToPlace;
    public GameObject placementIndicator;
    private XROrigin xrOrigin;
    private ARRaycastManager raycastManager;
    private Pose placementPose;
    private bool isPlaced = false;

    void Start()
    {
        xrOrigin = FindFirstObjectByType<XROrigin>();
        raycastManager = FindObjectOfType<ARRaycastManager>();
        
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

    }

    void PlaceObject()
    {
        Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
 
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
