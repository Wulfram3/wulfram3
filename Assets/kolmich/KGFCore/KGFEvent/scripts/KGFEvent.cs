// <author>Alexander Murauer</author>
// <email>alexander.murauer@kolmich.at</email>
// <date>2012-07-26</date>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

/// <summary>
/// 
/// </summary>
[System.Serializable]
public class KGFEvent : KGFEventBase, KGFIValidator
{
	public delegate bool KGFEventFilterMethod(MethodInfo theMethod);

	#region KGFEventData
	[System.Serializable]
	public class KGFEventData
	{
		public bool itsRuntimeObjectSearch = false;
		public string itsRuntimeObjectSearchType = "";
		public string itsRuntimeObjectSearchFilter = "";
		public Type GetRuntimeType()
		{
			return Type.GetType(itsRuntimeObjectSearchType);
		}
		
		public GameObject itsObject = null;
		public string itsComponentName = "";
		public string itsMethodName = "";
		public string itsMethodNameShort = "";
		public EventParameter[]itsParameters = new EventParameter[0];
		public EventParameterType[]itsParameterTypes = new EventParameterType[0];
		public bool itsPassthroughMode = false;
		
		public KGFEventData()
		{}
		
		public KGFEventData(bool thePassThroughMode, params EventParameterType[] theParameterTypes)
		{
			itsParameterTypes = theParameterTypes;
			itsPassthroughMode = thePassThroughMode;
		}
		
		/// <summary>
		/// Checks if parameter pass through mode is active
		/// </summary>
		/// <returns></returns>
		public bool GetDirectPassThroughMode()
		{
			return itsPassthroughMode;
		}
		
		/// <summary>
		/// Set direct parameter pass through mode
		/// </summary>
		/// <param name="thePassThroughMode"></param>
		public void SetDirectPassThroughMode(bool thePassThroughMode)
		{
			itsPassthroughMode = thePassThroughMode;
		}
		
		/// <summary>
		/// Set parameter types
		/// </summary>
		/// <param name="theParameterTypes"></param>
		public void SetRuntimeParameterInfos(params EventParameterType[] theParameterTypes)
		{
			if (theParameterTypes == null)
				itsParameterTypes = new KGFEvent.EventParameterType[0];
			else
				itsParameterTypes = theParameterTypes;
		}
		
		/// <summary>
		/// Get parameter types
		/// </summary>
		/// <returns></returns>
		public EventParameterType[] GetParameterLinkTypes()
		{
			return itsParameterTypes;
		}
		
		/// <summary>
		/// Check if the creator of this event sends parameters
		/// </summary>
		/// <returns></returns>
		public bool GetSupportsRuntimeParameterInfos()
		{
			return itsParameterTypes.Length > 0;
		}
		
		
		/// <summary>
		/// Check if a parameter is linked to a runtime parameter
		/// </summary>
		/// <param name="theParameterIndex"></param>
		/// <returns></returns>
		public bool GetIsParameterLinked(int theParameterIndex)
		{
			if (!GetSupportsRuntimeParameterInfos())
				return false;
			if (theParameterIndex >= itsParameters.Length)
				return false;
			return itsParameters[theParameterIndex].itsLinked;
		}
		
		/// <summary>
		/// Set linked state of a parameter
		/// </summary>
		/// <param name="theParameterIndex"></param>
		/// <param name="theLinkState"></param>
		public void SetIsParameterLinked(int theParameterIndex, bool theLinkState)
		{
			if (theParameterIndex >= itsParameters.Length)
				return;
			itsParameters[theParameterIndex].itsLinked = theLinkState;
		}
		
		/// <summary>
		/// Check which runtime parameter is linked to a parameter
		/// </summary>
		/// <param name="theParameterIndex"></param>
		/// <returns></returns>
		public int GetParameterLink(int theParameterIndex)
		{
			if (theParameterIndex >= itsParameters.Length)
				return 0;
			return itsParameters[theParameterIndex].itsLink;
		}
		
		/// <summary>
		/// Link the parameter to a runtime parameter
		/// </summary>
		/// <param name="theParameterIndex"></param>
		/// <param name="theLink"></param>
		public void SetParameterLink(int theParameterIndex, int theLink)
		{
			if (theParameterIndex >= itsParameters.Length)
				return;
			itsParameters[theParameterIndex].itsLink = theLink;
		}
		
		/// <summary>
		/// Get all parameter values
		/// </summary>
		/// <returns></returns>
		public EventParameter[] GetParameters ()
		{
			return itsParameters;
		}
		
		/// <summary>
		/// Set parameter values
		/// </summary>
		/// <param name="theParameters"></param>
		public void SetParameters (EventParameter[] theParameters)
		{
			itsParameters = theParameters;
		}
		
		/// <summary>
		/// Gets the game object.
		/// </summary>
		/// <returns>
		/// The game object.
		/// </returns>
		public GameObject GetGameObject ()
		{
			return itsObject;
		}
		
		object GetFieldValueByReflection(MonoBehaviour theCaller,string theMemberName)
		{
			Type aType = theCaller.GetType();
			FieldInfo aFieldInfo = aType.GetField(theMemberName);
			if (aFieldInfo != null)
			{
				return aFieldInfo.GetValue(theCaller);
			}
			return null;
		}

		public void Trigger (MonoBehaviour theCaller, params object[]theParameters)
		{
			List<object> aList = new List<object>(theParameters);
			foreach (EventParameterType aParameterType in itsParameterTypes)
			{
				if (aParameterType.GetCopyFromSourceObject())
				{
					aList.Add(GetFieldValueByReflection(theCaller,aParameterType.itsName));
				}
			}
			
			if (itsRuntimeObjectSearch)
			{
				TriggerRuntimeSearch(theCaller,aList.ToArray());
			}else
			{
				TriggerDefault(theCaller,aList.ToArray());
			}
		}
		
		/// <summary>
		/// Method for getting for example:
		///  - the index of the second parameter with the type kgfstring
		/// </summary>
		/// <param name="theIndex"></param>
		/// <param name="theType"></param>
		/// <returns></returns>
		int GetParameterIndexWithType(int theIndex, string theType)
		{
			int anIndex = 0;
			for (int i=0;i<itsParameterTypes.Length;i++)
			{
				EventParameterType aParameterType = itsParameterTypes[i];
				if (aParameterType.itsTypeName == theType)
				{
					if (anIndex == theIndex)
						return i;
					anIndex++;
				}
			}
			return 0;
		}
		
		bool CheckRuntimeObjectName(MonoBehaviour theMonobehaviour)
		{
			if (itsRuntimeObjectSearchFilter.Trim() == string.Empty)
				return true;
			if (itsRuntimeObjectSearchFilter == theMonobehaviour.name)
				return true;
			return false;
		}
		
		void TriggerRuntimeSearch(MonoBehaviour theCaller, object[] theRuntimeParameters)
		{
			Type aType = GetRuntimeType();
			if (aType == null)
			{
				LogError ("could not find type", itsEventCategory, theCaller);
				return;
			}
			if (itsMethodName == null)
			{
				LogError ("event has no selected method", itsEventCategory, theCaller);
				return;
			}
			
			// find method
			MethodInfo aFoundMethod;
			MonoBehaviour aFoundComponent;
			if (!FindMethod (this, out aFoundMethod, out aFoundComponent))
			{
				LogError ("Could not find method on object.", itsEventCategory, theCaller);
				return;
			}
			
			// convert parameters
			object [] aParametersList = null;
			if (GetDirectPassThroughMode())
			{
				aParametersList = theRuntimeParameters;
				// check parameter right types
			}
			else
			{
				// convert parameters set in this event
				ParameterInfo []aParameterInfoArray = aFoundMethod.GetParameters ();
				aParametersList = ConvertParameters (aParameterInfoArray, itsParameters);
				// convert linked parameters and add to the list
				for (int i=0;i<itsParameters.Length;i++)
				{
					if (GetIsParameterLinked(i))
					{
						int anIndex = GetParameterIndexWithType(GetParameterLink(i),aParameterInfoArray[i].ParameterType.FullName);
						if (anIndex < theRuntimeParameters.Length)
						{
							aParametersList[i] = theRuntimeParameters[anIndex];
						}else
						{
							Debug.LogError("you did not give enough parameters");
						}
					}
				}
			}
			
//			// debug
//			foreach (object anObject in aParametersList)
//			{
//				print(" - "+anObject+ " / "+anObject.GetType());
//			}
			
			// call method
			List<MonoBehaviour> afoundObjectList = new List<MonoBehaviour>();
			try
			{
				if (aType.IsInterface || typeof(KGFObject).IsAssignableFrom(aType))
				{
					// use fast kgfaccessor method
					foreach (object anObject in KGFAccessor.GetObjects(aType))
					{
						MonoBehaviour aMonobehaviour = anObject as MonoBehaviour;
						if (aMonobehaviour != null)
						{
							if (CheckRuntimeObjectName(aMonobehaviour))
							{
								aFoundMethod.Invoke (anObject, aParametersList);
								afoundObjectList.Add(aMonobehaviour);
							}
						}
					}
				}else if (!aType.IsInterface)
				{
					foreach (object anObject in GameObject.FindObjectsOfType(aType))
					{
						MonoBehaviour aMonobehaviour = anObject as MonoBehaviour;
						if (aMonobehaviour != null)
						{
							if (CheckRuntimeObjectName(aMonobehaviour))
							{
								aFoundMethod.Invoke (anObject, aParametersList);
								afoundObjectList.Add(aMonobehaviour);
							}
						}
					}
				}
			} catch (Exception e)
			{
				LogError ("invoked method caused exception in event_generic:" + e, itsEventCategory, theCaller);
			}
			
			// log call
			List<string> aParamStringList = new List<string> ();
			if (aParametersList != null)
			{
				foreach (object aParam in aParametersList)
					aParamStringList.Add ("" + aParam);
			}
			
			foreach (MonoBehaviour aMonoBehaviour in afoundObjectList)
			{
				string aLogInfo = string.Format ("{0}({1}): {2} ({3})", aMonoBehaviour.name,itsRuntimeObjectSearchType, aFoundMethod.Name, string.Join (",", aParamStringList.ToArray ()));
				LogDebug (aLogInfo, itsEventCategory, theCaller);
			}
		}
		
		void TriggerDefault(MonoBehaviour theCaller, params object[]theRuntimeParameters)
		{
			// check all stuff != null
			if (itsObject == null)
			{
				LogError ("event has null object", itsEventCategory, theCaller);
				return;
			}
			if (itsComponentName == null)
			{
				LogError ("event has no selected component", itsEventCategory, theCaller);
				return;
			}
			if (itsMethodName == null)
			{
				LogError ("event has no selected method", itsEventCategory, theCaller);
				return;
			}

			// find method
			MethodInfo aFoundMethod;
			MonoBehaviour aFoundComponent;
			if (!FindMethod (this, out aFoundMethod, out aFoundComponent))
			{
				LogError ("Could not find method on object.", itsEventCategory, theCaller);
				return;
			}

			// convert parameters
			object [] aParametersList = null;
			if (GetDirectPassThroughMode())
			{
				aParametersList = theRuntimeParameters;
				// check parameter right types
			}
			else
			{
				// convert parameters set in this event
				ParameterInfo []aParameterInfoArray = aFoundMethod.GetParameters ();
				aParametersList = ConvertParameters (aParameterInfoArray, itsParameters);
				// convert linked parameters and add to the list
				for (int i=0;i<itsParameters.Length;i++)
				{
					if (GetIsParameterLinked(i))
					{
						int anIndex = GetParameterIndexWithType(GetParameterLink(i),aParameterInfoArray[i].ParameterType.FullName);
						if (anIndex < theRuntimeParameters.Length)
						{
							aParametersList[i] = theRuntimeParameters[anIndex];
						}else
						{
							Debug.LogError("you did not give enough parameters");
						}
					}
				}
			}
			
			// call method
			try
			{
				aFoundMethod.Invoke (aFoundComponent, aParametersList);
			} catch (Exception e)
			{
				LogError ("invoked method caused exception in event_generic:" + e, itsEventCategory, theCaller);
			}
			
			// log call
			List<string> aParamStringList = new List<string> ();
			if (aParametersList != null)
			{
				foreach (object aParam in aParametersList)
					aParamStringList.Add ("" + aParam);
			}
			
			string aLogInfo = string.Format ("{0}({1}): {2} ({3})", itsObject.name, aFoundComponent.GetType ().Name, aFoundMethod.Name, string.Join (",", aParamStringList.ToArray ()));
			LogDebug (aLogInfo, itsEventCategory, theCaller);
		}

		KGFEventFilterMethod itsFilterMethod = null;
		public void SetMethodFilter(KGFEventFilterMethod theFilter)
		{
			itsFilterMethod = theFilter;
		}

		public void ClearMethodFilter()
		{
			itsFilterMethod = null;
		}

		KGFEventFilterMethod GetFilterMethod()
		{
			return itsFilterMethod;
		}

		/// <summary>
		/// Checks if the method should be displayed
		/// </summary>
		/// <returns>
		/// TRUE, if the method should be displayed, FALSE otherwise.
		/// </returns>
		/// <param name='theMethod'>
		/// the method to be checked
		/// </param>
		public bool CheckMethod (MethodInfo theMethod)
		{
			// try user given filter method
			if (itsFilterMethod != null)
			{
				if (!GetFilterMethod()(theMethod))
					return false;
			}
			
			// only filter methods if pass through mode is active and
			// there are runtime parameter infos
			if (GetSupportsRuntimeParameterInfos() && GetDirectPassThroughMode())
			{
				ParameterInfo []aParameterInfoArray = theMethod.GetParameters();
				
				// basic checks
				if (aParameterInfoArray.Length != itsParameterTypes.Length)
					return false;
				
				// detailed per item checks
				for (int i=0;i<aParameterInfoArray.Length;i++)
				{
					if (!itsParameterTypes[i].GetIsMatchingType(aParameterInfoArray[i].ParameterType))
						return false;
				}
			}
			return true;
		}
		
		/// <summary>
		/// Get errors
		/// </summary>
		/// <returns></returns>
		public KGFMessageList GetErrors()
		{
			KGFMessageList aMessageList = new KGFMessageList();
			
			if (string.IsNullOrEmpty(itsMethodName))
			{
				aMessageList.AddError("Empty method name");
			}
			if (itsRuntimeObjectSearch)
			{
				if (string.IsNullOrEmpty(itsRuntimeObjectSearchType))
				{
					aMessageList.AddError("Empty type field");
				}
			}
			
			MethodInfo aFoundMethod;
			MonoBehaviour aFoundComponent;
			if (!FindMethod (this, out aFoundMethod, out aFoundComponent))
			{
				aMessageList.AddError("Could not find method on object.");
			}else
			{
				ParameterInfo[] aParameterInfos = aFoundMethod.GetParameters();
				for (int i=0;i<itsParameters.Length;i++)
				{
					if (!GetIsParameterLinked(i))
					{
						if (typeof(UnityEngine.Object).IsAssignableFrom(aParameterInfos[i].ParameterType))
						{
							if (itsParameters[i].itsValueUnityObject == null)
							{
								aMessageList.AddError("Empty unity object in parameters");
							}
						}
					}
				}
			}
			
			return aMessageList;
		}
	}
	#endregion
	
	public KGFEventData itsEventData = new KGFEvent.KGFEventData ();
	
	/// <summary>
	/// Category used for logging
	/// </summary>
	const string itsEventCategory = "KGFEventSystem";
	#region parameter classes
	[System.Serializable]
	public class EventParameterType
	{
		public EventParameterType()
		{
		}
		
		public EventParameterType(string theName, Type theType)
		{
			itsName = theName;
			itsTypeName = theType.FullName;
		}
		
		public string itsName;
		public string itsTypeName;
		public bool itsCopyFromSourceObject = false;
		
		public void SetCopyFromSourceObject(bool theCopy)
		{
			itsCopyFromSourceObject = theCopy;
		}
		
		public bool GetCopyFromSourceObject()
		{
			return itsCopyFromSourceObject;
		}
		
		/// <summary>
		/// Checks if the given type matches this parameter type
		/// </summary>
		/// <param name="theOtherParameterType"></param>
		/// <returns></returns>
		public bool GetIsMatchingType(Type theOtherParameterType)
		{
			return itsTypeName == theOtherParameterType.FullName;
		}
	}
	
	[System.Serializable]
	public class EventParameter
	{
		public int itsValueInt32;
		public string itsValueString;
		public float itsValueSingle;
		public double itsValueDouble;
		public Color itsValueColor;
		public Rect itsValueRect;
		public Vector2 itsValueVector2;
		public Vector3 itsValueVector3;
		public Vector4 itsValueVector4;
		public bool itsValueBoolean;
		public UnityEngine.Object itsValueUnityObject;
		
		public bool itsLinked = false;
		public int itsLink = 0;
		
		public EventParameter ()
		{
			itsValueUnityObject = null;
		}
	}
	#endregion
	
	/// <summary>
	/// Sets the destination.
	/// </summary>
	/// <param name='theGameObject'>
	/// The game object.
	/// </param>
	/// <param name='theComponentName'>
	/// The component name.
	/// </param>
	/// <param name='theMethodString'>
	/// The method string.
	/// </param>
	public void SetDestination (GameObject theGameObject, string theComponentName, string theMethodString)
	{
		itsEventData.itsObject = theGameObject;
		itsEventData.itsComponentName = theComponentName;
		itsEventData.itsMethodName = theMethodString;
	}
	
	/// <summary>
	/// Find method this event should call
	/// </summary>
	/// <returns></returns>
	static bool FindMethod (KGFEventData theEventData, out MethodInfo theMethod, out MonoBehaviour theComponent)
	{
		// init params
		theMethod = null;
		theComponent = null;
		
		if (theEventData.itsRuntimeObjectSearch)
		{
			foreach (MethodInfo aMethod in GetMethods(theEventData.GetRuntimeType(),theEventData))
			{
				string aMethodString = GetMethodString (aMethod);
				if (aMethodString == theEventData.itsMethodName)
				{
					// found method
					theMethod = aMethod;
					return true;
				}
			}
		}
		else
		{
			if (theEventData.itsObject != null)
			{
				// find components of gameobject
				MonoBehaviour [] aListComponents = theEventData.itsObject.GetComponents<MonoBehaviour> ();

				// find method in components
				foreach (MonoBehaviour aComponent in aListComponents)
				{
					if (aComponent.GetType ().Name == theEventData.itsComponentName)
					{
						theComponent = aComponent;
						foreach (MethodInfo aMethod in GetMethods(aComponent.GetType(),theEventData))
						{
							string aMethodString = GetMethodString (aMethod);
							if (aMethodString == theEventData.itsMethodName)
							{
								// found method
								theMethod = aMethod;
								return true;
							}
						}
					}
				}
			}
		}
		
		return false;
	}
	
	/// <summary>
	/// Trigger this event
	/// </summary>
	public override void Trigger ()
	{
		itsEventData.Trigger (this);
	}
	
	/// <summary>
	/// Searches an instance for a field and returns its value
	/// </summary>
	/// <param name="theType"></param>
	/// <param name="theInstance"></param>
	/// <param name="theName"></param>
	/// <returns></returns>
	static bool SearchInstanceForVariable (Type theType, object theInstance, string theName, ref object theValue)
	{
		FieldInfo aFieldInfo = theType.GetField (theName);
		if (aFieldInfo != null)
		{
			theValue = aFieldInfo.GetValue (theInstance);
			return true;
		}
		return false;
	}
	
	/// <summary>
	/// Convert parameters to object array
	/// </summary>
	/// <param name="theMethodParametersList"></param>
	/// <param name="theParametersList"></param>
	/// <returns></returns>
	static object[] ConvertParameters (ParameterInfo[]theMethodParametersList, EventParameter[]theParametersList)
	{
		object [] anObjectList = new object[theMethodParametersList.Length];
		
		for (int i=0; i<theMethodParametersList.Length; i++)
		{
			if (typeof(UnityEngine.Object).IsAssignableFrom (theMethodParametersList [i].ParameterType))
			{
				// unity objects, regardless of type
				anObjectList [i] = theParametersList [i].itsValueUnityObject;
			} else if (SearchInstanceForVariable (typeof(EventParameter), theParametersList [i], "itsValue" + theMethodParametersList [i].ParameterType.Name, ref anObjectList [i]))
			{
				// other objects
			} else
			{
				// could not find
				Debug.LogError ("could not find variable for type:" + theMethodParametersList [i].ParameterType.Name);
			}
		}
		
		return anObjectList;
	}

	/// <summary>
	/// Gets the methods.
	/// </summary>
	/// <returns>
	/// The methods.
	/// </returns>
	/// <param name='theObject'>
	/// The object.
	/// </param>
	public static MethodInfo[] GetMethods (Type theType, KGFEventData theData)
	{
		List<MethodInfo> aList = new List<MethodInfo> ();
		Type anObjectType = theType;
		while (anObjectType != null)
		{
			// add methods
			MethodInfo[] aMethods = anObjectType.GetMethods (BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			
			foreach (MethodInfo aMethodInfo in aMethods)
			{
				if (aMethodInfo.GetCustomAttributes (typeof(KGFEventExpose),true).Length > 0)
				{
					if (theData.CheckMethod(aMethodInfo))
					{
						aList.Add (aMethodInfo);
					}
				}
			}
			
			// move to base type
			anObjectType = anObjectType.BaseType;
		}
		
		return aList.ToArray ();
	}

	/// <summary>
	/// Gets the method string.
	/// </summary>
	/// <returns>
	/// The method string.
	/// </returns>
	/// <param name='theMethod'>
	/// The method.
	/// </param>
	public static string GetMethodString (MethodInfo theMethod)
	{
		return theMethod.ToString ();
	}
	
	public static void LogError(string theMessage,string theCategory, MonoBehaviour theCaller)
	{
		#if KGFDebug
		KGFDebug.LogError(theMessage,"KGFEventSystem",null);
		#else
		Debug.LogError(theMessage);
		#endif
	}
	
	public static void LogDebug(string theMessage,string theCategory, MonoBehaviour theCaller)
	{
		#if KGFDebug
		KGFDebug.LogError(theMessage,"KGFEventSystem",null);
		#else
		Debug.Log(theMessage);
		#endif
	}
	
	public static void LogWarning(string theMessage,string theCategory, MonoBehaviour theCaller)
	{
		#if KGFDebug
		KGFDebug.LogError(theMessage,"KGFEventSystem",null);
		#else
		Debug.LogWarning(theMessage);
		#endif
	}
	
	#region KGFIValidator
	/// <summary>
	/// Check this instance for errors
	/// </summary>
	/// <returns></returns>
	public override KGFMessageList Validate ()
	{
		KGFMessageList aList = new KGFMessageList ();
		
		if (("" + itsEventData.itsMethodName).Trim () == string.Empty)
		{
			aList.AddError ("itsMethod is empty");
		}
		
		// find method
		if (!itsEventData.itsRuntimeObjectSearch)
		{
			if (itsEventData.itsObject == null)
			{
				aList.AddError ("itsObject == null");
			}
			
			if (("" + itsEventData.itsComponentName).Trim () == string.Empty)
			{
				aList.AddError ("itsScript is empty");
			}
			
			if (itsEventData.itsObject != null)
			{
				MethodInfo aFoundMethod;
				MonoBehaviour aFoundComponent;
				if (!FindMethod (itsEventData, out aFoundMethod, out aFoundComponent))
				{
					aList.AddError ("method could not be found");
				}
			}
		}
		
		// search type
		if (itsEventData.itsRuntimeObjectSearch)
		{
			Type aType = itsEventData.GetRuntimeType();
			if (aType == null)
			{
				aList.AddError("could not find type");
			}else
			{
				if (aType.IsInterface)
				{
					aList.AddWarning("you used an interface, please ensure that the objects you want to call the method on are derrived from KGFObject");
				}
				else
				{
					if (!typeof(MonoBehaviour).IsAssignableFrom(aType))
					{
						aList.AddError("type must be derrived from Monobehaviour");
					}
					if (!typeof(KGFObject).IsAssignableFrom(aType))
					{
						aList.AddWarning("please derrive from KGFObject because it will be faster to search");
					}
				}
			}
		}

//		// convert parameters
//		if (aFoundMethod != null)
//		{
//			object [] aParametersList = ConvertParameters (aFoundMethod.GetParameters (), itsParameters);
//			foreach (object anObject in aParametersList)
//			{
//				// Does not work, don't know why
//				if (anObject == null)
//				{
//					aList.AddError("null parameters are not allowed");
//					break;
//				}
//			}
//		}
		
		return aList;
	}
	#endregion
}
