// <author>Michal Kolasinski</author>
// <email>michal.kolasinski@kolmich.at</email>
// <date>2011-10-03</date>
// <summary>short summary</summary>

using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class KGFPhotoCapture : MonoBehaviour
{
	private KGFMapSystem.KGFPhotoData itsOldPhotoData = null;
	public KGFMapSystem itsMapSystem = null;
	private KGFMapSystem.KGFPhotoData itsPhotoData = null;
	private bool itsDestroy = false;
	
	
	private void Start()
	{
		itsPhotoData = itsMapSystem.GetNextPhotoData();
		if(itsPhotoData != null)
		{
			transform.position = CalculateCameraPosition(itsPhotoData);	//place camera correctly for photo
		}
	}
	
	private Vector3 CalculateCameraPosition(KGFMapSystem.KGFPhotoData thePhotoData)
	{
		Vector3 aPlanePosition = thePhotoData.itsPosition;
		float aHalfPlaneSize = thePhotoData.itsMeters/2.0f;
		aPlanePosition.x += aHalfPlaneSize; //center camera above plane
		
		if(itsMapSystem.GetOrientation() == KGFMapSystem.KGFMapSystemOrientation.XZDefault)
		{
			aPlanePosition.z += aHalfPlaneSize; //center camera above plane
			aPlanePosition.y += GetComponent<Camera>().farClipPlane;
		}
		else
		{
			aPlanePosition.y += aHalfPlaneSize; //center camera above plane
			aPlanePosition.z -= GetComponent<Camera>().farClipPlane;
		}
		return aPlanePosition;
	}
	
	/// <summary>
	/// Render method for ingame. Waits for postrender.
	/// </summary>
	private IEnumerator OnPostRender()
	{
		if(itsPhotoData != null)
		{
			itsPhotoData.itsTexture.ReadPixels(new Rect(0,0,itsPhotoData.itsTextureSize,itsPhotoData.itsTextureSize),0,0);
			itsPhotoData.itsTexture.wrapMode = TextureWrapMode.Clamp;
			itsPhotoData.itsTexture.Apply();
			
			itsOldPhotoData = itsPhotoData;
			// Get next frames data
			itsPhotoData = itsMapSystem.GetNextPhotoData();
			if(itsPhotoData != null)
			{
				transform.position = CalculateCameraPosition(itsPhotoData);	//place camera correctly for photo
			}
			yield return new WaitForEndOfFrame();	//this is very important! When using pro reflective water effects you cannot simply create new MeshRenderers during rendering. So wait for end of frame.
			KGFMapSystem.KGFSetChildrenActiveRecursively(itsOldPhotoData.itsPhotoPlane,true);
		}
		if (itsPhotoData == null)
		{
			yield return new WaitForEndOfFrame();
			itsDestroy = true;
		}
	}
	
	private void Update()
	{
		if(itsDestroy == true)
		{
			Destroy(gameObject);	//finished taking photos. selfdestruct
		}
	}
}
