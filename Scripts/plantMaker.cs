﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class plantMaker : MonoBehaviour {

    public GameObject plantPrefab01;
    public GameObject plantPrefab02;
    public GameObject plantPrefab03;
    public GameObject plantPrefab04;
    public GameObject plantPrefab05;
    

    private GameObject plantPrefab;
    public GameObject[] plantArray = new GameObject[5];
    public int plantIndex;
    bool plantChose;

    public float createHeight;
	public float maxRayDistance = 30.0f;
	public LayerMask collisionLayer = 1 << 10;  //ARKitPlane layer
	private MaterialPropertyBlock props;
    public GETRequest02 Places;

    public Transform m_HitTransform;

    bool HitTestWithResultType(ARPoint point, ARHitTestResultType resultTypes)
    {
        List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(point, resultTypes);
        if (hitResults.Count > 0)
        {
            foreach (var hitResult in hitResults)
            {
                Debug.Log("Got hit!");
                m_HitTransform.position = UnityARMatrixOps.GetPosition(hitResult.worldTransform);
                m_HitTransform.rotation = UnityARMatrixOps.GetRotation(hitResult.worldTransform);
                Debug.Log(string.Format("x:{0:0.######} y:{1:0.######} z:{2:0.######}", m_HitTransform.position.x, m_HitTransform.position.y, m_HitTransform.position.z));
                return true;
            }
        }
        return false;
    }


    // Use this for initialization
    void Start () {
        plantChose = false;
		props = new MaterialPropertyBlock ();
        plantIndex = 0;
        GameObject[] plantArray = { plantPrefab01, plantPrefab02, plantPrefab03, plantPrefab04, plantPrefab05 };
        
	}

	void CreatePlant(Vector3 atPosition)
	{

        string park = Places.place;

        if (park == "Washington Square Park")
        {
            //plantPrefab = plantArray[0];
            if(!plantChose){
                plantIndex = 0;
            }
           
        }
        else if (park == "Tompkins Square Park")
        {
            //plantPrefab = plantArray[1];
            if (!plantChose)
            {
                plantIndex = 1;
            }

        }
        else if (park == "Central Park")
        {
            if (!plantChose)
            {
                plantIndex = 0;
            }

        }

        plantChose = true;
        plantPrefab = plantArray[plantIndex];



        GameObject plantGO = Instantiate (plantPrefab, atPosition, Quaternion.identity);
			
		
		float r = Random.Range(0.0f, 1.0f);
		float g = Random.Range(0.0f, 1.0f);
		float b = Random.Range(0.0f, 1.0f);

		props.SetColor("_InstanceColor", new Color(r, g, b));

		MeshRenderer renderer = plantGO.GetComponent<MeshRenderer>();
		renderer.SetPropertyBlock(props);

	}


    public void ButtonPressed()
    {
        plantIndex++;
        if (plantIndex > 4)
        {
            plantIndex = 0;
        }
        Debug.Log(plantIndex);
    }

    // Update is called once per frame
    void Update()
    {


#if UNITY_EDITOR   //we will only use this script on the editor side, though there is nothing that would prevent it from working on device
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //we'll try to hit one of the plane collider gameobjects that were generated by the plugin
            //effectively similar to calling HitTest with ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent
            if (Physics.Raycast(ray, out hit, maxRayDistance, collisionLayer))
            {
                CreatePlant(new Vector3(hit.point.x, hit.point.y + createHeight, hit.point.z));

                //we're going to get the position from the contact point
                Debug.Log(string.Format("x:{0:0.######} y:{1:0.######} z:{2:0.######}", hit.point.x, hit.point.y, hit.point.z));
            }
        }
#else
		if (Input.touchCount > 0 )
		{
			var touch = Input.GetTouch(0);
			if (touch.phase == TouchPhase.Began)
			{
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(touch.position);

				var screenPosition = Camera.main.ScreenToViewportPoint(touch.position);
				ARPoint point = new ARPoint {
					x = screenPosition.x,
					y = screenPosition.y
				};

                if(Physics.Raycast(ray, out hit, 200))
                {
                    if(hit.transform.tag == "plant" && touch.phase == TouchPhase.Moved)
                    {
                        // prioritize reults types
                        ARHitTestResultType[] resultTypes = {
                            //ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingGeometry,
                            ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent, 
                            // if you want to use infinite planes use this:
                            //ARHitTestResultType.ARHitTestResultTypeExistingPlane,
                            //ARHitTestResultType.ARHitTestResultTypeEstimatedHorizontalPlane, 
                            //ARHitTestResultType.ARHitTestResultTypeEstimatedVerticalPlane, 
                            //ARHitTestResultType.ARHitTestResultTypeFeaturePoint
                        }; 
                        
                        foreach (ARHitTestResultType resultType in resultTypes)
                        {
                            if (HitTestWithResultType (point, resultType))
                            {
                                return;
                            }
                        }
                    } else 
                    {
                        List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface ().HitTest (point, 
                    ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent);
                        if (hitResults.Count > 0) {
                            foreach (var hitResult in hitResults) {
                                Vector3 position = UnityARMatrixOps.GetPosition (hitResult.worldTransform);
                                CreatePlant (new Vector3 (position.x, position.y + createHeight, position.z));
                                break;
                            }
                        }
                    }

                }

			}
		}
#endif

    }

}
