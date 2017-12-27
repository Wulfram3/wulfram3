// <author>Alexander Murauer</author>
// <email>alexander.murauer@kolmich.at</email>
// <date>2012-09-12</date>

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

public class KGFDialogWindow : EditorWindow
{
	public enum eIconStyle{Info,Question,Warning,Error};
	public enum eButtonStyle{OK,YesNo,OkCancel};
	public class KGFDialogWindowButtonClickEventArgs : EventArgs
	{
		int itsButton = 0;
		public KGFDialogWindowButtonClickEventArgs(int theButton)
		{
			itsButton = theButton;
		}
		public int GetButton()
		{
			return itsButton;
		}
	}
	
	eButtonStyle itsButtonStyle = eButtonStyle.OK;
	eIconStyle itsIconStyle = eIconStyle.Info;
	string itsTitle = "";
	string itsInfo = "";
	
	List<string> itsButtons = new List<string>();
	
	EventHandler itsEventClosed;
	
	static Texture2D itsTextureError = null;
	static Texture2D itsTextureQuestion = null;
	static Texture2D itsTextureInfo = null;
	static Texture2D itsTextureWarning = null;
	
	static void LoadTextures()
	{
		const string aTexturePath = "Assets/3rdparty/kolmich/KGFCore/textures/";
		
		itsTextureError =    (Texture2D)AssetDatabase.LoadAssetAtPath(aTexturePath + "icon_error.png",typeof(Texture2D));
		itsTextureQuestion = (Texture2D)AssetDatabase.LoadAssetAtPath(aTexturePath + "icon_question.png",typeof(Texture2D));
		itsTextureInfo =     (Texture2D)AssetDatabase.LoadAssetAtPath(aTexturePath + "icon_info.png",typeof(Texture2D));
		itsTextureWarning =  (Texture2D)AssetDatabase.LoadAssetAtPath(aTexturePath + "icon_warning.png",typeof(Texture2D));
	}
	
	void DrawIcon()
	{
		switch(itsIconStyle)
		{
			case eIconStyle.Info:
				KGFGUIUtility.Label("",itsTextureInfo,KGFGUIUtility.eStyleLabel.eLabel);
				break;
			case eIconStyle.Question:
				KGFGUIUtility.Label("",itsTextureQuestion,KGFGUIUtility.eStyleLabel.eLabel);
				break;
			case eIconStyle.Warning:
				KGFGUIUtility.Label("",itsTextureWarning,KGFGUIUtility.eStyleLabel.eLabel);
				break;
			case eIconStyle.Error:
				KGFGUIUtility.Label("",itsTextureError,KGFGUIUtility.eStyleLabel.eLabel);
				break;
		}
	}
	
	void DrawButtons()
	{
		for (int i=0;i<itsButtons.Count;i++)
		{
			if (KGFGUIUtility.Button(itsButtons[i],KGFGUIUtility.eStyleButton.eButton))
			{
				if (itsEventClosed != null)
					itsEventClosed(this,new KGFDialogWindowButtonClickEventArgs(i));
				Close();
			}
		}
	}
	
	void InitButtons()
	{
		switch(itsButtonStyle)
		{
			case eButtonStyle.OK:
				itsButtons.Add("OK");
				break;
			case eButtonStyle.OkCancel:
				itsButtons.Add("OK");
				itsButtons.Add("Cancel");
				break;
			case eButtonStyle.YesNo:
				itsButtons.Add("Yes");
				itsButtons.Add("No");
				break;
		}
	}
	
	void OnGUI()
	{
//		GUILayout.BeginVertical();
//		{
//			KGFGUIUtility.SpaceSmall();
//			GUILayout.BeginHorizontal();
//			{
//				KGFGUIUtility.SpaceSmall();
				KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxDecorated);
				{
					KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkTop);
					{
						DrawIcon();
						KGFGUIUtility.Label(itsTitle);
						GUILayout.FlexibleSpace();
					}
					KGFGUIUtility.EndHorizontalBox();
					KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
					{
						GUIStyle aWrapStyle = new GUIStyle(KGFGUIUtility.GetStyleLabel(KGFGUIUtility.eStyleLabel.eLabel));
						aWrapStyle.wordWrap = true;
						aWrapStyle.fixedHeight = 0;
						GUILayout.Label(itsInfo,aWrapStyle,GUILayout.ExpandWidth(true));
						GUILayout.FlexibleSpace();
					}
					KGFGUIUtility.EndVerticalBox();
					KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkBottom);
					{
						DrawButtons();
					}
					KGFGUIUtility.EndHorizontalBox();
				}
				KGFGUIUtility.EndVerticalBox();
//				KGFGUIUtility.SpaceSmall();
//			}
//			GUILayout.EndHorizontal();
//			KGFGUIUtility.SpaceSmall();
//		}
//		GUILayout.EndVertical();
	}
	
	#region public methods
	public static KGFDialogWindow Create(EventHandler theCloseEvent, string theInfo)
	{
		return Create(theCloseEvent,theInfo,"Message");
	}
	
	public static KGFDialogWindow Create(EventHandler theCloseEvent, string theInfo, string theTitle)
	{
		return Create(theCloseEvent,theInfo,theTitle,eIconStyle.Info);
	}
	
	public static KGFDialogWindow Create(EventHandler theCloseEvent, string theInfo, string theTitle, eIconStyle theIconStyle)
	{
		return Create(theCloseEvent,theInfo,theTitle,theIconStyle,eButtonStyle.OK);
	}
	
	public static KGFDialogWindow Create(EventHandler theCloseEvent, string theInfo, string theTitle, eIconStyle theIconStyle,eButtonStyle theButtonStyle)
	{
		LoadTextures();
		
		KGFDialogWindow aWindow = ScriptableObject.CreateInstance<KGFDialogWindow>();
		aWindow.itsInfo = theInfo;
		aWindow.itsTitle = theTitle;
		aWindow.itsButtonStyle = theButtonStyle;
		aWindow.itsIconStyle = theIconStyle;
		aWindow.itsEventClosed = theCloseEvent;
		aWindow.InitButtons();
		aWindow.ShowUtility();
		return aWindow;
	}
	#endregion
}
