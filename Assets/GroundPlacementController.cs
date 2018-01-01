using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GroundPlacementController : Photon.PunBehaviour
{
    [SerializeField]
    private GameObject placeableObjectPrefab;

    [SerializeField]
    private KeyCode newObjectHotkey = KeyCode.Comma;

    private GameObject currentPlaceableObject;

    private float mouseWheelRotation;

    public GameObject placeObject;

    public Material PreviewMaterial;


    private void Update()
    {
        HandleNewObjectHotkey();

        if (currentPlaceableObject != null)
        {
            MoveCurrentObjectToMouse();
            RotateFromMouseWheel();
            ReleaseIfClicked();
        }
    }

    private void HandleNewObjectHotkey()
    {
        if (Input.GetKeyDown(newObjectHotkey))
        {
            if (currentPlaceableObject != null)
            {
                Destroy(currentPlaceableObject);
            }
            else
            {
                currentPlaceableObject = Instantiate(placeableObjectPrefab);
            }
        }
    }

    private void MoveCurrentObjectToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
        currentPlaceableObject.transform.position = placeObject.transform.position;
            //currentPlaceableObject.transform.position = hitInfo.point;
  }

        //foreach (var renderer in currentPlaceableObject.GetComponentsInChildren<Renderer>(true))
          //  renderer.sharedMaterial = PreviewMaterial;
        foreach (var script in currentPlaceableObject.GetComponentsInChildren<MonoBehaviour>(true))
            script.enabled = false;
    }

    private void RotateFromMouseWheel()
    {
        Debug.Log(Input.mouseScrollDelta);
        mouseWheelRotation += Input.mouseScrollDelta.y;
        currentPlaceableObject.transform.Rotate(Vector3.up, mouseWheelRotation * 10f);
    }

    private void ReleaseIfClicked()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //currentPlaceableObject = null;
            Destroy(currentPlaceableObject);
            PhotonNetwork.Instantiate("FlakTurret", placeObject.transform.position, placeObject.transform.rotation, 0);
        }
    }
}


/* 
 Logic Flow:.
    -Create function to strip and ghost the prefab
    -Based on cargo layer/tag choose type of prefab to instantiate
     
     
 

 
     */

