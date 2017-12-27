// <author>Michal Kolasinski</author>
// <email>michal.kolasinski@kolmich.at</email>
// <date>2012-11-17</date>
// <summary>short summary</summary>

using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// This is a demo class demonstrating on how to control the KGFMapSystem with a c# script. 
/// Attach this script to the KGFMapSystem to see how it works. 
/// </summary>
public class KGFMapSystemScriptControlDemo : MonoBehaviour
{
	/// <summary>
	/// private member caching the KGFMapSystem
	/// </summary>
	KGFMapSystem itsMapSystem = null;
	
	void Start()
	{
		itsMapSystem = GetComponent<KGFMapSystem>();								//get the mapsystem only once and cache it
		itsMapSystem.EventClickedOnMinimap += OnUserClickedOnMap;					//register OnClick method
		itsMapSystem.EventUserFlagCreated += OnUserFlagWasCreated;					//register OnFlagSet method
	}
	
	/// <summary>
	/// This methods will be invoked every time when the user clicks with the mouse into the KGFMapSysetm
	/// </summary>
	/// <param name="theSender"></param>
	/// <param name="theEventArgs"></param>
	void OnUserClickedOnMap(object theSender, EventArgs theEventArgs)
	{
		KGFMapSystem.KGFClickEventArgs anEventArgs = theEventArgs as KGFMapSystem.KGFClickEventArgs;
		if (anEventArgs != null)
		{
			Debug.Log("Clicked at position(world space): "+anEventArgs.itsPosition);
		}
	}
	
	/// <summary>
	/// This methods will be invoked every time when the user creates a new flag marker by clicking on the map
	/// This will only work if the feature section itsUserFlags is enabled
	/// </summary>
	/// <param name="theSender"></param>
	/// <param name="theEventArgs"></param>
	void OnUserFlagWasCreated(object theSender, EventArgs theEventArgs)
	{
		KGFMapSystem.KGFFlagEventArgs anEventArgs = theEventArgs as KGFMapSystem.KGFFlagEventArgs;
		if (anEventArgs != null)
		{
			Debug.Log("Created marker at position(world space): "+anEventArgs.itsPosition);
		}
	}
	
	/// <summary>
	/// Invoke map system methods
	/// </summary>
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.M))
		{
			itsMapSystem.SetFullscreen(!itsMapSystem.GetFullscreen());
		}
		else if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			itsMapSystem.ZoomIn();
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			itsMapSystem.ZoomOut();
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			itsMapSystem.ZoomMax();
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			itsMapSystem.ZoomMin();
		}
		else if (Input.GetKeyDown(KeyCode.H))	//make sure to assign a your camera in the itsViewPort Section. Else this will not work
		{
			itsMapSystem.SetViewportEnabled(!itsMapSystem.GetViewportEnabled());
		}
		else if (Input.GetKeyDown(KeyCode.Z))
		{
			itsMapSystem.SetModeStatic(!itsMapSystem.GetModeStatic());
		}
	}
}
