// <author>Michal Kolasinski</author>
// <email>michal.kolasinski@kolmich.at</email>
// <date>2010-05-28</date>
// <summary>short summary</summary>

using System;
using System.Collections;
using System.IO;
using System.Reflection;

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(KGFMapSystem))]
public class KGFMapSystemEditor : KGFEditor
{
	KGFMapSystem itsTarget;
	
	public void OnEnable ()
	{
		itsTarget = (KGFMapSystem)target;
	}
	
	protected override void CustomGui ()
	{
		base.CustomGui ();
		
		if(itsTarget.itsDataModuleMinimap.itsAppearanceMiniMap.itsMarginVertical > 1)
		{
			itsTarget.itsDataModuleMinimap.itsAppearanceMiniMap.itsMarginVertical = 1;
			EditorUtility.SetDirty(itsTarget);
		}
		else if(itsTarget.itsDataModuleMinimap.itsAppearanceMiniMap.itsMarginVertical < 0)
		{
			itsTarget.itsDataModuleMinimap.itsAppearanceMiniMap.itsMarginVertical = 0;
			EditorUtility.SetDirty(itsTarget);
		}
		
		if(itsTarget.itsDataModuleMinimap.itsAppearanceMiniMap.itsMarginHorizontal > 1)
		{
			itsTarget.itsDataModuleMinimap.itsAppearanceMiniMap.itsMarginHorizontal = 1;
			EditorUtility.SetDirty(itsTarget);
		}
		else if(itsTarget.itsDataModuleMinimap.itsAppearanceMiniMap.itsMarginHorizontal < 0)
		{
			itsTarget.itsDataModuleMinimap.itsAppearanceMiniMap.itsMarginHorizontal = 0;
			EditorUtility.SetDirty(itsTarget);
		}
		
		if (Application.isPlaying && itsTarget.itsDataModuleMinimap.itsPhoto.itsTakePhoto)
		{
			if (GUILayout.Button("save photo"))
			{
				SavePhoto();
			}
		}
	}
	
	void SavePhoto()
	{
		ConvertImages();
	}
	
	public KGFMapSystem GetMapSystem()
	{
		return itsTarget;
	}
	
	public void ConvertImages()
	{
		// directories
		string aNamePrefab = "photo.prefab";
		string aName = string.Format("{0:yyyyMMdd_HHmmss}",DateTime.Now);
		string aDirectory = "Assets/kolmich/KGFMapSystem/photocache/"+aName+"/";
		string aDirectoryTextures = aDirectory+"textures/";
		string aDirectoryMaterials = aDirectory+"materials/";
		string aDirectoryPrefabs = aDirectory+"prefabs/";
		Directory.CreateDirectory(aDirectoryTextures);
		Directory.CreateDirectory(aDirectoryMaterials);
		Directory.CreateDirectory(aDirectoryPrefabs);
		
		KGFMapSystem.KGFPhotoData []aData = itsTarget.GetPhotoData();
		for (int i=0;i<aData.Length;i++)
		{
			// texture
			string aFilePathTexture = aDirectoryTextures+i+".png";
			byte[] bt = aData[i].itsTexture.EncodeToPNG();
			File.WriteAllBytes(aFilePathTexture,bt);
			
			AssetDatabase.ImportAsset(aFilePathTexture);
			
			TextureImporter anImporter = TextureImporter.GetAtPath(aFilePathTexture) as TextureImporter;
			anImporter.wrapMode = TextureWrapMode.Clamp;
			
			AssetDatabase.ImportAsset(aFilePathTexture);
			
			// material
			aData[i].itsTexture = AssetDatabase.LoadAssetAtPath(aFilePathTexture,typeof(Texture2D)) as Texture2D;
			aData[i].itsPhotoPlaneMaterial.mainTexture = aData[i].itsTexture;
			
//			AssetDatabase.CreateAsset((Texture2D)(aData[i].itsTexture),"Assets/t"+i+".png");
			AssetDatabase.CreateAsset(aData[i].itsPhotoPlaneMaterial,aDirectoryMaterials+i+".mat");
		}
		
		// prefab
		UnityEngine.Object aPrefab = PrefabUtility.CreatePrefab(aDirectoryPrefabs+aNamePrefab,itsTarget.GetPhotoParent(),ReplacePrefabOptions.ConnectToPrefab);
		
		// Mesh
		Mesh aMesh = null;
		for (int i=0;i<aData.Length;i++)
		{
			if (aMesh == null)
			{
				// save first mesh we find to prefab
				aMesh = aData[i].itsPhotoPlane.GetComponent<MeshFilter>().sharedMesh;
				aMesh.name = "SimplePlaneMesh";
				AssetDatabase.AddObjectToAsset(aMesh,aDirectoryPrefabs+aNamePrefab);
				AssetDatabase.ImportAsset(aDirectoryPrefabs+aNamePrefab);
				
				// get link of mesh in prefab
				aMesh = AssetDatabase.LoadAssetAtPath(aDirectoryPrefabs+aNamePrefab,typeof(Mesh)) as Mesh;
			}
			else
			{
				// set all other meshfilters to use the mesh in the prefab
				aData[i].itsPhotoPlane.GetComponent<MeshFilter>().sharedMesh = aMesh;
			}
		}
		
		// save changes to prefab
		PrefabUtility.ReplacePrefab(itsTarget.GetPhotoParent(),aPrefab,ReplacePrefabOptions.ConnectToPrefab);
	}
	
	public static KGFMessageList ValidateKGFMapSystemEditor(UnityEngine.Object theObject)
	{
		return new KGFMessageList();
	}
}