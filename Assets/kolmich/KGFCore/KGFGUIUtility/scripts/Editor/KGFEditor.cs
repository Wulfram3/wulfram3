using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;
using UnityEngine;

public class KGFEditor : Editor
{
	static List<string> itsAlreadySentWarnings = new List<string>();
	
	public override sealed void OnInspectorGUI ()
	{
		KGFGUIUtility.SetSkinIndex(0);
		KGFGUIUtilityEditor.RenderKGFInspector(this,this.GetType(),CustomGui);
		KGFGUIUtility.SetSkinIndex(1);
	}
	
	protected virtual void CustomGui()
	{
	}
	
	public static KGFMessageList ValidateKGFEditor(UnityEngine.Object theTarget)
	{
		KGFMessageList aMessageList = new KGFMessageList();
		return aMessageList;
	}
	
	/// <summary>
	/// this class will call the validate method of the inspector
	/// </summary>
	/// <param name="theGameObject"></param>
	public static KGFMessageList ValidateEditor(UnityEngine.Object theObject)
	{
		KGFMessageList aMessageList = new KGFMessageList();
		string anObjectName = theObject.GetType().ToString();
		string aTypeName = anObjectName+"Editor";
		Type aType = Type.GetType(aTypeName);
		if(aType != null)
		{
			MethodInfo aMethodInfo = aType.GetMethod("Validate"+aTypeName,System.Reflection.BindingFlags.Static | BindingFlags.Public);
			if(aMethodInfo != null && aMethodInfo.GetParameters().Length == 1)
			{
				object[] aParameters = new object[1];
				aParameters[0] = theObject;
				aMessageList = (KGFMessageList)aMethodInfo.Invoke(null,aParameters);
			}
			else
			{
				if (!itsAlreadySentWarnings.Contains(aTypeName))
				{
					itsAlreadySentWarnings.Add(aTypeName);
					aMessageList.AddWarning("static method Validate"+aTypeName+"() not implemented in: "+aTypeName);
					Debug.LogWarning("static method Validate() not implemented in: "+aTypeName);
				}
			}
		}
		else
		{
			if (!itsAlreadySentWarnings.Contains(aTypeName))
			{
				itsAlreadySentWarnings.Add(aTypeName);
				aMessageList.AddWarning("type: "+aTypeName+" not implemented.");
				Debug.LogWarning("type: "+aTypeName+" not implemented.");
			}
		}
		return aMessageList;
	}
}
