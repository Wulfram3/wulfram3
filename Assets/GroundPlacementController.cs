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
                currentPlaceableObject = Instantiate(placeableObjectPrefab);
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

        Component[] renderers = currentPlaceableObject.GetComponentsInChildren(typeof(Renderer));
        foreach (Renderer curRenderer in renderers)
        {
            Color color;
            foreach (Material material in curRenderer.materials)
            {
                color = material.color;
                // change alfa for transparency
                color.a -= 0.5f;
                if (color.a < 0)
                {
                    color.a = 0;
                }
                material.color = color;
            }
        }

        foreach (var script in currentPlaceableObject.GetComponentsInChildren<MonoBehaviour>(true))
            script.enabled = false;
    }

    public void RotateFromMouseWheel()
    {
        Debug.Log(Input.mouseScrollDelta);
        mouseWheelRotation += Input.mouseScrollDelta.y;
        currentPlaceableObject.transform.Rotate(Vector3.up, mouseWheelRotation * 10f);
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


