// <author>Alexander Murauer</author>
// <email>alexander.murauer@kolmich.at</email>
// <date>2012-07-26</date>
// <summary></summary>

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[CustomEditor(typeof(KGFEvent))]
public class KGFEventEditor : KGFEditor
{
	/// <summary>
	/// Converts an array of MonoBehaviours to a name array
	/// </summary>
	/// <returns>
	/// The component names.
	/// </returns>
	/// <param name='theComponents'>
	/// The components.
	/// </param>
	static string[] GetComponentNames(MonoBehaviour[] theComponents)
	{
		List<string> aList = new List<string>();
		foreach (Component aComponent in theComponents)
		{
			aList.Add(aComponent.GetType().Name);
		}
		return aList.ToArray();
	}

	/// <summary>
	/// Converts a method array to a string array
	/// </summary>
	/// <returns>
	/// The method names.
	/// </returns>
	/// <param name='theMethods'>
	/// The methods.
	/// </param>
	static string[] GetMethodNames(MethodInfo[] theMethods)
	{
		List<string> aList = new List<string>();
		foreach (MethodInfo aMethod in theMethods)
		{
			aList.Add(KGFEvent.GetMethodString(aMethod));
		}
		return aList.ToArray();
	}

	/// <summary>
	/// Lists the components.
	/// </summary>
	static void ListComponents(UnityEngine.Object theTarget, KGFEvent.KGFEventData theEventData)
	{
		GameObject aGameObject = theEventData.GetGameObject();
		
		if (aGameObject != null)
		{
			// search monobehaviours on game object
			MonoBehaviour [] aComponentList = aGameObject.GetComponents<MonoBehaviour>();
			string [] aComponentNamesList = GetComponentNames(aComponentList);

			// search for currently selected monobehaviour
			int anOldIndex = -1;
			for (int i=0; i<aComponentNamesList.Length; i++)
			{
				if (aComponentNamesList[i] == theEventData.itsComponentName)
				{
					anOldIndex = i;
				}
			}

			// draw user selection
			int aNewIndex = EditorGUILayout.Popup("itsScript", anOldIndex, aComponentNamesList);

			// set target to dirty if changed
			if (aNewIndex != anOldIndex)
			{
				theEventData.itsMethodName = "";
				theEventData.SetParameters(new KGFEvent.EventParameter[0]);
				EditorUtility.SetDirty(theTarget);
			}

			// set new value if selection is valid and list methods
			if (aNewIndex >= 0)
			{
				theEventData.itsComponentName = aComponentNamesList[aNewIndex];
				if (aComponentList[aNewIndex] != null)
				{
					ListMethods(theTarget,theEventData,aComponentList[aNewIndex].GetType());
				}
			}
		}
	}

	/// <summary>
	/// Lists the methods.
	/// </summary>
	/// <param name='theComponent'>
	/// The component.
	/// </param>
	static void ListMethods(UnityEngine.Object theTarget, KGFEvent.KGFEventData theEventData, Type theType)
	{
		if (theType != null)
		{
			// Get methods of the Monobehaviour
			MethodInfo[] aMethodList = KGFEvent.GetMethods(theType,theEventData);
			string [] aMethodNamesList = GetMethodNames(aMethodList);
			
			// search for currently selected method
			int anOldIndex = -1;
			for (int i=0; i<aMethodNamesList.Length; i++)
			{
				if (aMethodNamesList[i] == theEventData.itsMethodName)
				{
					anOldIndex = i;
				}
			}
			
			// draw user selection
			int aNewIndex = EditorGUILayout.Popup("Method", anOldIndex, aMethodNamesList);

			// set target to dirty if value changed
			if (aNewIndex != anOldIndex)
			{
				theEventData.SetParameters(new KGFEvent.EventParameter[0]);
				EditorUtility.SetDirty(theTarget);
			}
			
			// set new method name if selection is valid
			if (aNewIndex >= 0)
			{
				theEventData.itsMethodName = aMethodNamesList[aNewIndex];
				theEventData.itsMethodNameShort = aMethodList[aNewIndex].Name;
				ListParameters(theTarget,theEventData,aMethodList[aNewIndex]);
			}
		}
	}
	
	/// <summary>
	/// Show input fields for all parameters of the given method
	/// </summary>
	/// <param name="theMethod">The reflected method info</param>
	static void ListParameters(UnityEngine.Object theTarget, KGFEvent.KGFEventData theEventData, MethodInfo theMethod)
	{
		if (theEventData.GetDirectPassThroughMode())
			return;
//		if (!theEventData.GetDisplayParametersInInspector())
//			return;

		EditorGUILayout.BeginHorizontal();
//		EditorGUILayout.Space();

		EditorGUILayout.BeginVertical();
		{
			// get parameters from method
			ParameterInfo [] aMethodParametersList = theMethod.GetParameters();
			// get saved parameter values from event_generic object
			KGFEvent.EventParameter []aParametersList = theEventData.GetParameters();
			// reinit parameter values if length counts do not match
			if (theEventData.itsParameters.Length != aMethodParametersList.Length)
			{
				aParametersList = new KGFEvent.EventParameter[aMethodParametersList.Length];
				for (int j=0;j<aParametersList.Length;j++)
				{
					aParametersList[j] = new KGFEvent.EventParameter();
				}
			}
			
			KGFEvent.EventParameterType[] aTypes = theEventData.GetParameterLinkTypes();
			string[] aPopUpArray = new string[0];
			
			// draw input field for each parameter
			for (int i=0;i<aMethodParametersList.Length;i++)
			{
				GUILayout.BeginHorizontal();
				{
					// check if there are linked parameters with the current parameter type
					if (theEventData.GetSupportsRuntimeParameterInfos())
					{
						List<string> aPopupList = new List<string>();
						foreach (KGFEvent.EventParameterType aParameterType in aTypes)
						{
							if (aParameterType.GetIsMatchingType(aMethodParametersList[i].ParameterType))
							{
								aPopupList.Add(string.Format("{0} ({1})",aParameterType.itsName,aParameterType.itsTypeName));
							}
						}
						aPopUpArray = aPopupList.ToArray();
					}
					
//					if (i < aMethodParametersList.Length-1)
					{
						GUILayout.Label("├",GUILayout.Width(20));
					}
//					else
//					{
//						GUILayout.Label("└",GUILayout.Width(20));
//					}
					
					if (theEventData.GetIsParameterLinked(i))
					{
						// if the parameter is linked, let the user choose the linked parameter
						GUILayout.Label(aMethodParametersList[i].Name+"=");
						int aIndexNew = EditorGUILayout.Popup(theEventData.GetParameterLink(i),aPopUpArray);
						if (aIndexNew != theEventData.GetParameterLink(i))
						{
							theEventData.SetParameterLink(i,aIndexNew);
							EditorUtility.SetDirty(theTarget);
						}
					}
					else
					{
						// if not linked, enable default input field
						DrawSingleParameter(theTarget,theEventData,aMethodParametersList[i],aParametersList[i],i);
					}
					
					// let the user enable/disable linked state for this parameter, if it is supported and there are parameters with matching types
					if (theEventData.GetSupportsRuntimeParameterInfos() && aPopUpArray.Length > 0)
					{
						bool aBoolValue = GUILayout.Toggle(theEventData.GetIsParameterLinked(i),"Link",GUILayout.Width(40));
						if (aBoolValue != theEventData.GetIsParameterLinked(i))
						{
							theEventData.SetIsParameterLinked(i,aBoolValue);
							EditorUtility.SetDirty(theTarget);
						}
					}else
					{
						theEventData.SetIsParameterLinked(i,false);
					}
				}
				GUILayout.EndHorizontal();
			}
			// save parameter values to the event generic object
			theEventData.SetParameters(aParametersList);
		}
		EditorGUILayout.EndVertical();

		EditorGUILayout.EndHorizontal();
	}
	
	/// <summary>
	/// Draw a single parameter field
	/// </summary>
	/// <param name="theParameter">The reflected parameter type</param>
	/// <param name="theValue">The parameter value</param>
	/// <param name="theIndex">Index of the parameter in the itsParameters array</param>
	static void DrawSingleParameter(UnityEngine.Object theTarget, KGFEvent.KGFEventData theEventData, ParameterInfo theParameter, KGFEvent.EventParameter theValue, int theIndex)
	{
		if (typeof(UnityEngine.Object).IsAssignableFrom(theParameter.ParameterType))
		{
			// if the parameter is derrived from unityengine.object, display an object field
			UnityEngine.Object aObject = EditorGUILayout.ObjectField(theParameter.Name, theValue.itsValueUnityObject, theParameter.ParameterType,true);
			if (aObject != theValue.itsValueUnityObject)
			{
				EditorUtility.SetDirty(theTarget);
				theValue.itsValueUnityObject = aObject;
			}
		} else
		{
			// Search for parameter with right name in EventParameter script and display it
			if (typeof(int).IsAssignableFrom(theParameter.ParameterType))
			{
				theValue.itsValueInt32 = (int)KGFGUIUtilityEditor.DrawField(theTarget,typeof(int),theParameter.Name,theValue.itsValueInt32);
			}
			else if (typeof(double).IsAssignableFrom(theParameter.ParameterType))
			{
				theValue.itsValueSingle = (float)KGFGUIUtilityEditor.DrawField(theTarget,typeof(float),theParameter.Name,theValue.itsValueSingle);
			}
			else if (typeof(float).IsAssignableFrom(theParameter.ParameterType))
			{
				theValue.itsValueDouble = (double)KGFGUIUtilityEditor.DrawField(theTarget,typeof(double),theParameter.Name,theValue.itsValueDouble);
			}
			else if (typeof(string).IsAssignableFrom(theParameter.ParameterType))
			{
				theValue.itsValueString = (string)KGFGUIUtilityEditor.DrawField(theTarget,typeof(string),theParameter.Name,theValue.itsValueString);
			}
			else if (typeof(Color).IsAssignableFrom(theParameter.ParameterType))
			{
				theValue.itsValueColor = (Color)KGFGUIUtilityEditor.DrawField(theTarget,typeof(Color),theParameter.Name,theValue.itsValueColor);
			}
			else if (typeof(Rect).IsAssignableFrom(theParameter.ParameterType))
			{
				theValue.itsValueRect = (Rect)KGFGUIUtilityEditor.DrawField(theTarget,typeof(Rect),theParameter.Name,theValue.itsValueRect);
			}
			else if (typeof(Vector2).IsAssignableFrom(theParameter.ParameterType))
			{
				theValue.itsValueVector2 = (Vector2)KGFGUIUtilityEditor.DrawField(theTarget,typeof(Vector2),theParameter.Name,theValue.itsValueVector2);
			}
			else if (typeof(Vector3).IsAssignableFrom(theParameter.ParameterType))
			{
				theValue.itsValueVector3 = (Vector3)KGFGUIUtilityEditor.DrawField(theTarget,typeof(Vector3),theParameter.Name,theValue.itsValueVector3);
			}
			else if (typeof(Vector4).IsAssignableFrom(theParameter.ParameterType))
			{
				theValue.itsValueVector4 = (Vector4)KGFGUIUtilityEditor.DrawField(theTarget,typeof(Vector4),theParameter.Name,theValue.itsValueVector4);
			}
			else if (typeof(bool).IsAssignableFrom(theParameter.ParameterType))
			{
				theValue.itsValueBoolean = (bool)KGFGUIUtilityEditor.DrawField(theTarget,typeof(bool),theParameter.Name,theValue.itsValueBoolean);
			}
			else if (typeof(Enum).IsAssignableFrom(theParameter.ParameterType))
			{
				Enum anEnumValue = null;
				try{
					anEnumValue = (Enum)Enum.Parse(theParameter.ParameterType,theValue.itsValueString);
				}
				catch
				{
					// choose first value of enum
					foreach (Enum aValue in Enum.GetValues(theParameter.ParameterType))
					{
						anEnumValue = aValue;
						break;
					}
				}
				theValue.itsValueString = ((Enum)KGFGUIUtilityEditor.DrawField(theTarget,typeof(Enum),theParameter.Name,anEnumValue)).ToString();
			}
		}
	}
	
	public static void EventGui(UnityEngine.Object theTarget, KGFEvent.KGFEventData theData,params string[] theRuntimeObjectList)
	{
		EventGui(theTarget,theData,true,theRuntimeObjectList);
	}
	
	public static void EventGui(UnityEngine.Object theTarget, KGFEvent.KGFEventData theData, bool theDirectObject,params string[] theRuntimeObjectList)
	{
		if (theDirectObject)
		{
			bool aValue = EditorGUILayout.Toggle("Runtime Object Search",theData.itsRuntimeObjectSearch);
			if (aValue != theData.itsRuntimeObjectSearch)
			{
				theData.itsRuntimeObjectSearch = aValue;
				// set target to dirty
				EditorUtility.SetDirty(theTarget);
			}
		}else
		{
			theData.itsRuntimeObjectSearch = true;
		}
		
		if (theData.itsRuntimeObjectSearch)
		{
			if (theRuntimeObjectList.Length == 0)
			{
				string aValueString = EditorGUILayout.TextField("Type",theData.itsRuntimeObjectSearchType);
				if (aValueString != theData.itsRuntimeObjectSearchType)
				{
					theData.itsRuntimeObjectSearchType = aValueString;
					// set target to dirty
					EditorUtility.SetDirty(theTarget);
				}
			}else
			{
				int aSelectedIndex = 0;
				for (int i=0;i<theRuntimeObjectList.Length;i++)
				{
					if (theRuntimeObjectList[i] == theData.itsRuntimeObjectSearchType)
					{
						aSelectedIndex = i;
						break;
					}
				}
				
				aSelectedIndex = EditorGUILayout.Popup(aSelectedIndex,theRuntimeObjectList);
				if (theData.itsRuntimeObjectSearchType != theRuntimeObjectList[aSelectedIndex])
				{
					theData.itsRuntimeObjectSearchType = theRuntimeObjectList[aSelectedIndex];
					// set target to dirty
					EditorUtility.SetDirty(theTarget);
				}
			}
			
			string aValueFilter = EditorGUILayout.TextField("Gameobject Filter",theData.itsRuntimeObjectSearchFilter);
			if (aValueFilter != theData.itsRuntimeObjectSearchFilter)
			{
				theData.itsRuntimeObjectSearchFilter = aValueFilter;
				EditorUtility.SetDirty(theTarget);
			}
			
			ListMethods(theTarget,theData,theData.GetRuntimeType());
		}else
		{
			// draw object selection
			GameObject aNewGameObject = (GameObject)EditorGUILayout.ObjectField("itsObject", theData.itsObject, typeof(GameObject),true);
			if (aNewGameObject != theData.itsObject)
			{
				// clear selection of sub items if game object changed
				theData.itsObject = aNewGameObject;
				theData.itsComponentName = "";
				theData.itsMethodName = "";
				theData.SetParameters(new KGFEvent.EventParameter[0]);
				// set target to dirty
				EditorUtility.SetDirty(theTarget);
			}
			ListComponents(theTarget,theData);
		}
	}
	
	/// <summary>
	/// Rename the target KGVEvent
	/// </summary>
	protected virtual void RenameTarget(UnityEngine.Object theTarget,string theComponentName, string theMethodName)
	{
		string aName;
		if ((""+theMethodName).Trim() != string.Empty)
			aName = string.Format("EV[{0}].{1}()",theComponentName,theMethodName);
		else
			aName = string.Format("Event EMPTY");
		
		if (theTarget.name != aName)
			theTarget.name = aName;
	}
	
	protected override void CustomGui()
	{
		base.CustomGui();
		
		KGFEvent.KGFEventData aData = ((KGFEvent)target).itsEventData;
		aData.SetDirectPassThroughMode(false);
		EventGui(target,aData);
		if (aData.itsRuntimeObjectSearch)
			RenameTarget(target,aData.itsRuntimeObjectSearchType,aData.itsMethodNameShort);
		else
			RenameTarget(target,aData.itsComponentName,aData.itsMethodNameShort);
	}
}
