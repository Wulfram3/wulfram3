using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Wulfram3;

public class GroundPlacementController : Photon.PunBehaviour
{
    [SerializeField]
    public GameObject placeableObjectPrefab;
    
    [SerializeField]
    public KeyCode newObjectHotkey = KeyCode.Comma;

    public GameObject currentPlaceableObject;

    public float mouseWheelRotation;

    public GameObject placeObject;

    public Material PreviewMaterial;

    public GameManager gameManager;

    public string baseUnit;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        
    }

    public void Update()
    {
        HandleNewObjectHotkey();
        baseUnit = this.GetComponentInParent<CargoManager>().pickedUpCargo;
        if (currentPlaceableObject != null)
        {
            MoveCurrentObjectToMouse();
            RotateFromMouseWheel();
            ReleaseIfClicked();
        }
    }

    public void HandleNewObjectHotkey()
    {
        if (Input.GetKeyDown(newObjectHotkey) && !(baseUnit == ""))
        {
            if (currentPlaceableObject != null)
            {
                Destroy(currentPlaceableObject);
            }
            else
            {
                currentPlaceableObject = PhotonNetwork.Instantiate(baseUnit, placeObject.transform.position, placeObject.transform.rotation, 0);
            }
        }
    }

    public void MoveCurrentObjectToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
        currentPlaceableObject.transform.position = placeObject.transform.position;
            //currentPlaceableObject.transform.position = hitInfo.point;
  }


        //foreach (var renderer in currentPlaceableObject.GetComponentsInChildren<MeshRenderer>(true))
            //renderer.sharedMaterial = PreviewMaterial;
        ChangeMaterial(PreviewMaterial);


        foreach (var script in currentPlaceableObject.GetComponentsInChildren<MonoBehaviour>(true))
            script.enabled = true;
    }

    public void RotateFromMouseWheel()
    {
        Debug.Log(Input.mouseScrollDelta);
        mouseWheelRotation += Input.mouseScrollDelta.y;
        currentPlaceableObject.transform.Rotate(Vector3.up, mouseWheelRotation * 10f);
    }

    public void ChangeMaterial(Material newMat)
    {
        Renderer[] children;
        children = currentPlaceableObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in children)
        {
            var mats = new Material[rend.materials.Length];
            for (var j = 0; j < rend.materials.Length; j++)
            {
                mats[j] = newMat;
            }
            rend.materials = mats;
        }
    }

    public void ReleaseIfClicked()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //get baseunit to deploy based on its name in the resources folder (Must match)
            
            Debug.Log(baseUnit);
          
            Destroy(currentPlaceableObject);
            PhotonNetwork.Instantiate(baseUnit, placeObject.transform.position, placeObject.transform.rotation, 0);
            this.GetComponentInParent<CargoManager>().SetPickedUpCargo("");
        }
    }
}


