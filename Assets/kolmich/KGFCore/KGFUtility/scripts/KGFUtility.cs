// Copyright (c) 2010 All Right Reserved, http://www.kolmich.at/
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// </copyright>
// <author>Michal Kolasinski</author>
// <email>michal.kolasinski@kolmich.at</email>
// <date>2010-05-28</date>
// <summary>short summary</summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Security.Cryptography;
using System;

public static class KGFUtility : System.Object
{
	#region Extension methods for: MonoBehaviour
	/// <summary>
	/// Alternative for GetComponents() if you want to use interfaces
	/// </summary>
	/// <param name="theMonobehaviour"></param>
	/// <returns></returns>
	public static T[] GetComponentsInterface<T>(this MonoBehaviour theMonobehaviour) where T : class
	{
		List<T> aList = new List<T>();
		
		foreach (MonoBehaviour aMonobehaviour in theMonobehaviour.GetComponents<MonoBehaviour>())
		{
			T aT = aMonobehaviour as T;
			if (aT != null)
			{
				aList.Add(aT);
			}
		}
		
		return aList.ToArray();
	}
	
	/// <summary>
	/// Alternative for GetComponent() if you want to use interfaces
	/// </summary>
	/// <param name="theMonobehaviour"></param>
	/// <returns></returns>
	public static T GetComponentInterface<T>(this MonoBehaviour theMonobehaviour) where T : class
	{
		T[] anArray = theMonobehaviour.GetComponentsInterface<T>();
		if (anArray.Length > 0)
			return anArray[0];
		return null;
	}
	#endregion
	
	#region Extension methods for: List<T>
	/// <summary>
	/// Sorted list
	/// </summary>
	/// <param name="theList"></param>
	/// <returns></returns>
	public static List<T> Sorted<T>(this List<T> theList)
	{
		List<T> aList = new List<T>(theList);
		aList.Sort();
		return aList;
	}
	#endregion
	
	#region Extension methods for: IEnumerable
	/// <summary>
	/// Check if item is in collection
	/// </summary>
	/// <param name="theList"></param>
	/// <param name="theNeedle"></param>
	/// <returns></returns>
	public static bool ContainsItem<T>(this IEnumerable<T> theList,T theNeedle) where T : class
	{
		foreach (T anElement in theList)
		{
			if (theNeedle.Equals(anElement))
			{
				return true;
			}
		}
		return false;
	}
	
	/// <summary>
	/// Join to string with separator
	/// </summary>
	/// <param name="theList"></param>
	/// <param name="theSeparator"></param>
	/// <returns></returns>
	public static string JoinToString<T>(this IEnumerable<T> theList,string theSeparator)
	{
		if (theList == null)
			return "";
		List<string> aListStrings = new List<string>();
		foreach (T anElement in theList)
		{
			aListStrings.Add(anElement.ToString());
		}
		return string.Join(theSeparator,aListStrings.ToArray());
	}
	
	/// <summary>
	/// Insert item at position
	/// </summary>
	/// <param name="theList"></param>
	/// <param name="theItem"></param>
	/// <param name="thePosition"></param>
	/// <returns></returns>
	public static IEnumerable<T> InsertItem<T>(this IEnumerable<T> theList,T theItem,int thePosition)
	{
		int i=0;
		bool anInserted = false;
		foreach (T anElement in theList)
		{
			if (i == thePosition)
			{
				yield return theItem;
				anInserted = true;
			}
			yield return anElement;
			i++;
		}
		
		if (!anInserted)
		{
			yield return theItem;
		}
	}
	
	/// <summary>
	/// Append a new item
	/// </summary>
	/// <param name="theList"></param>
	/// <param name="theItem"></param>
	/// <returns></returns>
	public static IEnumerable<T> AppendItem<T>(this IEnumerable<T> theList,T theItem)
	{
		foreach (T anElement in theList)
		{
			yield return anElement;
		}
		yield return theItem;
	}
	
	/// <summary>
	/// Remove doubles from IEnumerable
	/// </summary>
	/// <param name="theList"></param>
	/// <returns></returns>
	public static IEnumerable<T> Distinct<T>(this IEnumerable<T> theList)
	{
		List<T> aDistinctList = new List<T>();
		
		foreach (T anElement in theList)
		{
			if (!aDistinctList.Contains(anElement))
			{
				aDistinctList.Add(anElement);
				yield return anElement;
			}
		}
		
		yield break;
	}
	
	/// <summary>
	/// Only return first list without elements of the second list
	/// </summary>
	/// <param name="theMainList"></param>
	/// <param name="theListToRemove"></param>
	/// <returns></returns>
	public static IEnumerable<T> Remove<T>(this IEnumerable<T> theMainList, T[] theListToRemove)
	{
		List<T> aListToRemove = new List<T>(theListToRemove);
		
		foreach (T anElement in theMainList)
		{
			if (!aListToRemove.Contains(anElement))
				yield return anElement;
		}
		yield break;
	}
	
	/// <summary>
	/// Sorted list
	/// </summary>
	/// <param name="theList"></param>
	/// <returns></returns>
	public static IEnumerable<T> Sorted<T>(this IEnumerable<T> theList)
	{
		List<T> aList = new List<T>(theList);
		aList.Sort();
		foreach (T aT in aList)
			yield return aT;
		yield break;
	}
	
	/// <summary>
	/// Sorted list
	/// </summary>
	/// <param name="theList"></param>
	/// <returns></returns>
	public static IEnumerable<T> Sorted<T>(this IEnumerable<T> theList, Comparison<T> theComparison)
	{
		List<T> aList = new List<T>(theList);
		aList.Sort(theComparison);
		foreach (T aT in aList)
			yield return aT;
		yield break;
	}
	
	/// <summary>
	/// Convert to generic list
	/// </summary>
	/// <returns></returns>
	public static List<T> ToDynList<T>(this IEnumerable<T> theList)
	{
		return new List<T>(theList);
	}
	#endregion

	#region Extension methods for: Transform
	/// <summary>
	/// Sets the scale recursively
	/// </summary>
	/// <param name="theTransform"></param>
	/// <param name="theScale"></param>
	public static void SetScaleRecursively(this Transform theTransform, Vector3 theScale)
	{
		foreach (Transform aChild in theTransform)
		{
			SetScaleRecursively(aChild,theScale);
		}
		theTransform.localScale = theScale;
	}
	#endregion

	#region Extension methods for: GameObject
	
	/// <summary>
	/// SetActive for all children
	/// </summary>
	/// <param name="theActive"></param>
	public static void SetChildrenActiveRecursively(this GameObject theGameObject, bool theActive)
	{
		foreach (Transform aChildTransform in theGameObject.transform)
		{
			//TODO: maybe do something like ignore all object_scripts
			//if (aChildTransform.GetComponent<sound_audiosource_script>() == null)
			#if UNITY_4_0
			aChildTransform.gameObject.SetActive(theActive);
			#else
			aChildTransform.gameObject.SetActiveRecursively(theActive);
			#endif
		}
	}

	/// <summary>
	/// Sets the layer of all base_scripts recursively.
	/// </summary>
	/// <param name='theLayer'>
	/// The layer.
	/// </param>
	public static void SetLayerRecursively(this GameObject theGameObject, int theLayer)
	{
		theGameObject.layer = theLayer;
		foreach(Transform aTransform in theGameObject.transform)
		{
			GameObject aGameObject = aTransform.gameObject;
			SetLayerRecursively(aGameObject,theLayer);
		}
	}
	#endregion

	#region Extension methods for: DateTime
	/// <summary>
	/// converts a datetime to unix time
	/// </summary>
	/// <param name="theDate"></param>
	/// <returns></returns>
	public static long DateToUnix(this DateTime theDate)
	{
		TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
		return (long)t.TotalSeconds;
	}		
	#endregion
	
	#region Extension methods for: string
	public static string Shortened(this string theString, int theMaxLength)
	{
		if (theString.Length > theMaxLength)
			return theString.Substring(0,theMaxLength-2)+"..";
		return theString;
	}
	
	public static string Join(this string theSeparator, params string[] theItems)
	{
		return string.Join(theSeparator,theItems);
	}
	
	public static string Join(this string theSeparator, IEnumerable<string> theItems)
	{
		return string.Join(theSeparator,new List<string>(theItems).ToArray());
	}
	
	public static string RemoveRight(this string theString, char theSeparator)
	{
		string aStringCopy = ""+theString;
		
		while (aStringCopy.Length > 0 && aStringCopy[aStringCopy.Length-1] != theSeparator)
		{
			aStringCopy = aStringCopy.Remove(aStringCopy.Length-1);
		}
		return aStringCopy;
	}
	
	public static string GetLastPart(this string theString, char theSeparator)
	{
		string[] aSplit = theString.Split(theSeparator);
		return aSplit[aSplit.Length-1];
	}
	#endregion		
	
	#region filesystem stuff
	/// <summary>
	/// Converts a path from platform specific to unity style
	/// </summary>
	/// <param name="thePlatformPath"></param>
	/// <returns></returns>
	public static string ConvertPathToUnity(string thePlatformPath)
	{
		return thePlatformPath.Replace(Path.DirectorySeparatorChar,'/');
	}
	
	/// <summary>
	/// Converts a path from unity style to platform specific
	/// </summary>
	/// <param name="theUnityPath"></param>
	/// <returns></returns>
	public static string ConvertPathToPlatformSpecific(string theUnityPath)
	{
		return theUnityPath.Replace('/',Path.DirectorySeparatorChar);
	}
	#endregion
	
	#region external stuff
	#if UNITY_STANDALONE_WIN
	public struct Point{
		public int X;
		public int Y;
	};
	//--------------------------------------------------
	// GetCursorPos
	[DllImport( "user32.dll" )]
	[return: MarshalAs( UnmanagedType.Bool )]
	public static extern bool GetCursorPos( out Point _point );
	//--------------------------------------------------
	// SetCursorPos
	[DllImport( "user32.dll" )]
	[return: MarshalAs( UnmanagedType.Bool )]
	public static extern bool SetCursorPos( int _x, int _y );
	
	[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true )]
	[return: MarshalAs( UnmanagedType.Bool )]
	public static extern bool ClipCursor( ref RECT rcClip );
	[DllImport("user32.dll" )]
	[return: MarshalAs( UnmanagedType.Bool )]
	public static extern bool GetClipCursor( out RECT rcClip );
	[DllImport("user32.dll" )]
	static extern int GetForegroundWindow( );
	[DllImport("user32.dll")]
	[return: MarshalAs( UnmanagedType.Bool )]
	static extern bool GetWindowRect( int hWnd, ref RECT lpRect );
	
	[StructLayout( LayoutKind.Sequential )]
	public struct RECT
	{
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;
		public RECT( int left, int top, int right, int bottom )
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}
	}
	
	static bool itsMouseRectActive = false;
	static RECT itsOriginalClippingRect;
	#endif
	
	/// <summary>
	/// Confine the mouse to an area on the screen
	/// </summary>
	/// <param name="theRect"></param>
	public static void SetMouseRect(Rect theRect)
	{
		#if UNITY_STANDALONE_WIN
		RECT aClippingRect = new RECT((int)theRect.x,(int)theRect.y,(int)theRect.xMax,(int)theRect.yMax);
		if (itsMouseRectActive)
			ClearMouseRect();
		
		itsOriginalClippingRect = new RECT( );
		GetClipCursor( out itsOriginalClippingRect );
		ClipCursor( ref aClippingRect);
		itsMouseRectActive = true;
		#endif
	}
	
	/// <summary>
	/// Clear the mouse confinement
	/// </summary>
	public static void ClearMouseRect()
	{
		#if UNITY_STANDALONE_WIN
		if (itsMouseRectActive)
		{
			ClipCursor( ref itsOriginalClippingRect );
			itsMouseRectActive = false;
		}
		#endif
	}
	
	/// <summary>
	/// Get the rect of the current foreground window
	/// </summary>
	/// <returns></returns>
	public static Rect GetWindowRect()
	{
		#if UNITY_STANDALONE_WIN
		int aWindowHandle = GetForegroundWindow( );
		RECT aCurrentClippingRect = new RECT();
		
		GetWindowRect( aWindowHandle, ref aCurrentClippingRect );
		return new Rect(aCurrentClippingRect.Left,aCurrentClippingRect.Top,aCurrentClippingRect.Right,aCurrentClippingRect.Bottom);
		#else
		return new Rect(0,0,0,0);
		#endif
	}
	#endregion
	
	/// <summary>
	/// Mathf.PingPong with additional ping staytime, pong staytime and transition time
	/// </summary>
	/// <param name="theTime"></param>
	/// <param name="theMaxValue"></param>
	/// <param name="thePingStayTime"></param>
	/// <param name="thePongStayTime"></param>
	/// <param name="theTransitionTime"></param>
	/// <returns></returns>
	public static float PingPong(float theTime,float theMaxValue, float thePingStayTime, float thePongStayTime, float theTransitionTime)
	{
		float aTimeSum = thePingStayTime+thePongStayTime+2*theTransitionTime;
		float aNumber = theTime % aTimeSum;
		
		if (aNumber < thePingStayTime)
			return 0;
		if (aNumber < thePingStayTime + theTransitionTime)
			return (aNumber-thePingStayTime)*theMaxValue/theTransitionTime;
		if (aNumber < thePingStayTime + theTransitionTime + thePongStayTime)
			return theMaxValue;
		
		return theMaxValue - ((aNumber - (thePingStayTime + theTransitionTime + thePongStayTime))*theMaxValue/theTransitionTime);
	}
	
	// should be faster than 2D, but not ready yet
	static Color32[] BlockBlur1D(Color32 []thePixels, int theWidth,int theHeight, int theBlurRadius)
	{
		Color32[] thePixelsResult = new Color32[thePixels.Length];
		
		for (int anY = 0;anY < theHeight;anY++)
		{
			for (int anX = 0;anX < theWidth;anX++)
			{
				int aR,aG,aB;
				aR=aG=aB=0;
//				int aStartX=anX-theBlurRadius>=0?anX-theBlurRadius:0;
//				int aStartY=anY-theBlurRadius>=0?anY-theBlurRadius:0;
				int aPixelCount = 0;
				
				for (int aBlockX=anX-theBlurRadius;aBlockX<=anX+theBlurRadius;aBlockX++)
				{
					Color32 aPixelBlock = thePixels[Mathf.Clamp(aBlockX,0,theWidth-1)+anY*theWidth];
					aR+=aPixelBlock.r;
					aG+=aPixelBlock.g;
					aB+=aPixelBlock.b;
					aPixelCount++;
				}
				
				Color32 aPixel = thePixels[anX+anY*theWidth];
				aPixel.r = (byte)(aR/aPixelCount);
				aPixel.g = (byte)(aG/aPixelCount);
				aPixel.b = (byte)(aB/aPixelCount);
//				thePixelsResult[anY+anX*theHeight] = aPixel;
				thePixelsResult[anX+anY*theWidth] = aPixel;
			}
		}
		
		return thePixelsResult;
	}
	
	static Color32[] BlockBlur2D(Color32 []thePixels, int theWidth,int theHeight, int theBlurRadiusX, int theBlurRadiusY)
	{
		Color32[] thePixelsResult = new Color32[thePixels.Length];
		
		for (int anY = 0;anY < theHeight;anY++)
		{
			for (int anX = 0;anX < theWidth;anX++)
			{
				int aR,aG,aB;
				aR=aG=aB=0;
				int aStartX=anX-theBlurRadiusX>=0?anX-theBlurRadiusX:0;
				int aStartY=anY-theBlurRadiusY>=0?anY-theBlurRadiusY:0;
				int aPixelCount = 0;
				for (int aBlockY=aStartY;(aBlockY<theHeight && aBlockY<=anY+theBlurRadiusY);aBlockY++)
				{
					for (int aBlockX=aStartX;(aBlockX<theWidth && aBlockX<=anX+theBlurRadiusX);aBlockX++)
					{
						Color32 aPixelBlock = thePixels[aBlockX+aBlockY*theWidth];
						aR+=aPixelBlock.r;
						aG+=aPixelBlock.g;
						aB+=aPixelBlock.b;
						aPixelCount++;
					}
				}
				Color32 aPixel = thePixels[anX+anY*theWidth];
				aPixel.r = (byte)(aR/aPixelCount);
				aPixel.g = (byte)(aG/aPixelCount);
				aPixel.b = (byte)(aB/aPixelCount);
				thePixelsResult[anX+anY*theWidth] = aPixel;
			}
		}
		
		return thePixelsResult;
	}
	
	static Rect itsCachedRect = new Rect();
	/// <summary>
	/// Shorthand for new Rect handling without the garbage collector overhead.
	/// Rect are only used for short time, so it is possible to use always the same one.
	/// </summary>
	/// <param name="theX"></param>
	/// <param name="theY"></param>
	/// <param name="theWidth"></param>
	/// <param name="theHeight"></param>
	/// <returns></returns>
	public static Rect GetCachedRect(float theX, float theY, float theWidth, float theHeight)
	{
		itsCachedRect.x = theX;
		itsCachedRect.y = theY;
		
		itsCachedRect.width = theWidth;
		itsCachedRect.height = theHeight;
		
		return itsCachedRect;
	}
	/// <summary>
	/// Shorthand for new Rect handling without the garbage collector overhead.
	/// Rect are only used for short time, so it is possible to use always the same one.
	/// </summary>
	/// <param name="theRect"></param>
	public static Rect GetCachedRect(Rect theRect)
	{
		itsCachedRect.x = theRect.x;
		itsCachedRect.y = theRect.y;

		itsCachedRect.width = theRect.width;
		itsCachedRect.height = theRect.height;

		return itsCachedRect;
	}
	
	static Vector3 itsCachedVector3 = new Vector3();
	/// <summary>
	/// Shorthand for new Vector3 handling without the garbage collector overhead.
	/// Rect are only used for short time, so it is possible to use always the same one.
	/// </summary>
	/// <param name="theX"></param>
	/// <param name="theY"></param>
	/// <param name="theZ"></param>
	/// <returns></returns>
	public static Vector3 GetCachedVector3(float theX,float theY, float theZ)
	{
		itsCachedVector3.x = theX;
		itsCachedVector3.y = theY;
		itsCachedVector3.z = theZ;
		
		return itsCachedVector3;
	}


	static Vector2 itsCachedVector2 = new Vector2();
	/// <summary>
	/// Shorthand for new Vector2 handling without the garbage collector overhead.
	/// Rect are only used for short time, so it is possible to use always the same one.
	/// </summary>
	/// <param name="theX"></param>
	/// <param name="theY"></param>
	/// <returns></returns>
	public static Vector2 GetCachedVector2(float theX, float theY)
	{
		itsCachedVector2.x = theX;
		itsCachedVector2.y = theY;
		
		return itsCachedVector2;
	}

	/// <summary>
	/// converts a unix time to a c# datetime
	/// </summary>
	/// <param name="theSeconds"></param>
	/// <returns></returns>
	public static DateTime DateFromUnix(long theSeconds)
	{
		DateTime aUnixStartTime = new DateTime(1970, 1, 1);
		return aUnixStartTime.AddSeconds(theSeconds);
	}
	
	/// <summary>
	/// Converts byte array to hex string
	/// </summary>
	/// <param name="buffer"></param>
	/// <returns></returns>
	public static string ToHexString(byte []buffer)
	{
		string aHexString = string.Empty;
		foreach (byte aByte in buffer)
		{
			aHexString += string.Format("{0:x02}",aByte);
		}
		return aHexString;
	}
	
	/// <summary>
	/// Get MD5 hash sum of file
	/// </summary>
	/// <param name="theFilePath"></param>
	/// <returns></returns>
	public static string GetHashMD5OfFile(string theFilePath)
	{
		if (File.Exists(theFilePath))
		{
			MD5CryptoServiceProvider aMD5Provider = new MD5CryptoServiceProvider();
			
			FileStream aFile = File.Open(theFilePath,FileMode.Open);
			byte[] aHash = aMD5Provider.ComputeHash(aFile);
			aFile.Close();
			return ToHexString(aHash);
		}
		return null;
	}
	
	/// <summary>
	/// Search for the texture that matches the aspect ratio the most
	/// </summary>
	/// <param name="theAspectRatio"></param>
	/// <param name="theTextures"></param>
	/// <returns></returns>
	public static Texture2D GetBestAspectMatchingTexture(float theAspectRatio, params Texture2D[] theTextures)
	{
		Texture2D aBestChoice = null;
		if (theTextures.Length > 0)
		{
			aBestChoice = theTextures[0];
			
			for (int i=1;i<theTextures.Length;i++)
			{
				Texture2D anImage = theTextures[i];
				if (anImage == null)
					continue;
				float aRatioDifferenceCurrent = Mathf.Abs(theAspectRatio-((float)aBestChoice.width)/((float)aBestChoice.height));
				float aRatioDifferenceNext = Mathf.Abs(theAspectRatio-((float)anImage.width)/((float)anImage.height));
				if (aRatioDifferenceNext < aRatioDifferenceCurrent)
					aBestChoice = anImage;
			}
		}
		
		return aBestChoice;
	}
	
	/// <summary>
	/// Makes sure all parameters are passed correctly to SetLookRotation, prevents Warning log and performance kill
	/// </summary>
	public static Quaternion SetLookRotationSafe(Quaternion theQuaternion, Vector3 theUpVector, Vector3 theLookRotation, Vector3 theAlternativeLookDirection)
	{
		if(theAlternativeLookDirection.magnitude == 0.0f)
		{
			throw new Exception("Alternative look vector can never be 0!");
		}
		else
		{
			if (theLookRotation.magnitude != 0.0f)
			{
				theQuaternion.SetLookRotation (theLookRotation, theUpVector);
				return theQuaternion;
			}
			else
			{
				theQuaternion.SetLookRotation (theAlternativeLookDirection, theUpVector);
				return theQuaternion;
			}
		}
	}
	
	
//	public float getConvertXMouseRotation()
//	{
//		float aCorrector = 0.2f;
//		float aMouseDelta = Input.GetAxis("Mouse X")*aCorrector;
//		return aMouseDelta;
//	}
//
//	public float getConvertYMouseRotation()
//	{
//		float aCorrector = 0.2f;
//		float aMouseDelta = Input.GetAxis("Mouse Y")*aCorrector;
//		return aMouseDelta;
//	}
//
//	public bool getMouseCursorInScreenRect(Rect theRect)
//	{
//		if(Input.mousePosition.x > theRect.xMin &&
//		   Input.mousePosition.x < theRect.xMax &&
//		   Input.mousePosition.y > theRect.yMin &&
//		   Input.mousePosition.y < theRect.yMax)
//			return true;
//		else
//			return false;
//	}
//
//	public Rect multiplyRects(Rect theRect1, Rect theRect2)
//	{
//		Rect aResultRect = new Rect(theRect1.x,theRect1.y,theRect1.width,theRect1.height);
//		aResultRect.x *= theRect2.x;
//		aResultRect.y *= theRect2.y;
//		aResultRect.width *= theRect2.width;
//		aResultRect.height *= theRect2.height;
//
//		return aResultRect;
//	}
}
