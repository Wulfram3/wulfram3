// <author>Christoph Hausjell</author>
// <email>christoph.hausjell@kolmich.at</email>
// <summary>Displays a list of modules that implement the KGFICustomGUI interface. By clicking a module icon the custom window of the module will be opened.</summary>

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents the main class of the Kolmich Game Framework Custom GUI.
/// </summary>
/// <remarks>
/// This class can list and manage all added instances which must implement the KGFIDebug interface.
/// </remarks>
public class KGFCustomGUI : KGFModule
{
	/// <summary>
	/// contains all data available for customization in the Unity3D inspector
	/// </summary>
	[System.Serializable]
	public class KGFDataCustomGUI
	{
		public Texture2D itsUnknownIcon = null;
		
		/// <summary>
		/// defines the key to activate the shortcut input.
		/// </summary>
		/// <remarks>
		/// This key defines the shortcut modifier key. The modifier key must be pressed first, before the shortcut key can take effect. This key`s down state is raised in each frame.
		/// </remarks>
		public KeyCode itsModifierKey = KeyCode.None;
		
		/// <summary>
		/// defines the shortcut key to hide the custom gui bar.
		/// </summary>
		/// <remarks>
		/// This key defines the shortcut key to hide or show the custom gui bar. The modifier key must be pressed before the shortcut key to activate the shortcut. Commands can be executed by pressing enter or clicking the execute button. This key`s down state is triggered once.
		/// </remarks>
		public KeyCode itsSchortcutKey = KeyCode.F3;
		
		public bool itsBarVisible = true;
	}
	
	//holds the only instance of the debugger
	private static KGFCustomGUI itsInstance = null;
	
	public KGFDataCustomGUI itsDataModuleCustomGUI = new KGFDataCustomGUI();
	
	static List<KGFICustomGUI> itsCustomGuiList = null;
	
	private static KGFICustomGUI itsCurrentSelectedGUI = null;
	private static Rect itsWindowRectangle = new Rect(50, 50, 800, 600);
	
	public KGFCustomGUI() : base(new Version(1,0,0,1), new Version(1,0,0,0))
	{
		
	}
	
	public static Rect GetItsWindowRectangle()
	{
		return itsWindowRectangle;
	}
	
	protected override void KGFAwake()
	{
		base.KGFAwake();
		if(itsInstance == null)
		{
			itsInstance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(this);
		}
		
		KGFAccessor.RegisterAddEvent<KGFICustomGUI>(OnCustomGuiChanged);
		KGFAccessor.RegisterRemoveEvent<KGFICustomGUI>(OnCustomGuiChanged);
		UpdateInternalList();
	}
	
	void OnCustomGuiChanged(object theSender,EventArgs theArgs)
	{
		UpdateInternalList();
	}
	
	void UpdateInternalList()
	{
		itsCustomGuiList = KGFAccessor.GetObjects<KGFICustomGUI>();
	}
	
	protected void OnGUI()
	{
		Render();
	}
	
	protected void Update()
	{
		if((Input.GetKey(itsDataModuleCustomGUI.itsModifierKey) && Input.GetKeyDown(itsDataModuleCustomGUI.itsSchortcutKey))
		   || (itsDataModuleCustomGUI.itsModifierKey == KeyCode.None && Input.GetKeyDown(itsDataModuleCustomGUI.itsSchortcutKey)))
		{
			itsDataModuleCustomGUI.itsBarVisible = !itsDataModuleCustomGUI.itsBarVisible;
		}
	}
	
	public static void Render()
	{
		KGFGUIUtility.SetSkinIndex(0);
		if(itsInstance != null && itsInstance.itsDataModuleCustomGUI.itsBarVisible)
		{
			GUIStyle aTogglStyle = KGFGUIUtility.GetStyleToggl(KGFGUIUtility.eStyleToggl.eTogglRadioStreched);
			GUIStyle aBoxStyle = KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDecorated);
			int aWidth = (int)(aTogglStyle.contentOffset.x + aTogglStyle.padding.horizontal + (KGFGUIUtility.GetSkinHeight() - aTogglStyle.padding.vertical));
			int aHeight = (int)(aBoxStyle.margin.top + aBoxStyle.margin.bottom + aBoxStyle.padding.top + aBoxStyle.padding.bottom
			                    + (aTogglStyle.fixedHeight + aTogglStyle.margin.top) * itsCustomGuiList.Count);
			
			GUILayout.BeginArea(new Rect(Screen.width - aWidth, (Screen.height - aHeight) / 2, aWidth, aHeight));
			{
				KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxDecorated, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
				{
					GUILayout.FlexibleSpace();
					
					foreach(KGFICustomGUI aCustomGUI in itsCustomGuiList)
					{
						bool aValue;
						
						if(itsCurrentSelectedGUI != null && itsCurrentSelectedGUI == aCustomGUI)
						{
							aValue = true;
						}
						else
						{
							aValue = false;
						}
						
						Texture2D aIcon = aCustomGUI.GetIcon();
						
						if(aIcon == null)
						{
							aIcon = itsInstance.itsDataModuleCustomGUI.itsUnknownIcon;
						}
						
						if(aValue != KGFGUIUtility.Toggle(aValue, aIcon, KGFGUIUtility.eStyleToggl.eTogglRadioStreched))
						{
							if(aValue)
							{
								itsCurrentSelectedGUI = null;
							}
							else
							{
								itsCurrentSelectedGUI = aCustomGUI;
							}
						}
					}
					
					GUILayout.FlexibleSpace();
				}
				KGFGUIUtility.EndVerticalBox();
			}
			GUILayout.EndArea();
			
			itsInstance.DrawCurrentCustomGUI(aWidth);
			
			#region old render code
			/*
			
			float aButtonSpaceHorizontal = Math.Max(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButton).fixedHeight, 16);
			float aButtonSpaceVertical = Math.Max(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButton).fixedHeight, 16);
			int aBoxSpaceHorizontal = Math.Max(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDecorated).margin.horizontal + KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDecorated).padding.horizontal, 24);
			int aBoxSpaceVertical = Math.Max(KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDecorated).margin.vertical + KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDecorated).padding.vertical, 24);
			
			GUILayout.BeginArea(new Rect(Screen.width - (aBoxSpaceHorizontal + aButtonSpaceHorizontal / 2),
			                             Screen.height / 2 - ((itsCustomGUIImplementations.Count * aButtonSpaceVertical) + aBoxSpaceHorizontal) / 2,
			                             aButtonSpaceHorizontal + aBoxSpaceHorizontal,
			                             (itsCustomGUIImplementations.Count * aButtonSpaceVertical) + aBoxSpaceVertical));
			
			{
				KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxDecorated, GUILayout.MinWidth(24));
				{
					foreach(KGFICustomGUI aCustomGUI in itsCustomGUIImplementations)
					{
						bool selected = false;
						float aWidth = Mathf.Max(KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButton).fixedHeight, 16);
						
						if(aCustomGUI.GetIcon() != null)
						{
							if(KGFGUIUtility.Button(aCustomGUI.GetIcon(), KGFGUIUtility.eStyleButton.eButton, GUILayout.Width(aWidth), GUILayout.Height(aWidth)))
							{
								selected = true;
							}
						}
						else
						{
							if(itsUnknownIcon != null)
							{
								if(KGFGUIUtility.Button(itsUnknownIcon, KGFGUIUtility.eStyleButton.eButton, GUILayout.Width(aWidth), GUILayout.Height(aWidth)))
								{
									selected = true;
								}
							}
							else
							{
								if(KGFGUIUtility.Button("?", KGFGUIUtility.eStyleButton.eButton, GUILayout.Width(aWidth), GUILayout.Height(aWidth)))
								{
									selected = true;
								}
							}
						}
						
						// check if one of the Buttons was clicked
						if(selected && aCustomGUI != itsCurrentSelectedGUI)
						{
							itsCurrentSelectedGUI = aCustomGUI;
						}
						else if(selected && aCustomGUI == itsCurrentSelectedGUI)
						{
							itsCurrentSelectedGUI = null;
						}
					}
				}
				KGFGUIUtility.EndVerticalBox();
			}
			GUILayout.EndArea();
			
			//Draw the Custom GUI
			if(itsCurrentSelectedGUI != null)
			{
				itsWindowRectangle = KGFGUIUtility.Window(0, itsWindowRectangle, itsInstance.DrawCurrentCustomGUI,string.Empty, GUILayout.MinHeight(200), GUILayout.MinWidth(300));

				// check if the window is still visible in screen
				if(itsWindowRectangle.x < -itsWindowRectangle.width + 20)
				{
					itsWindowRectangle.x = -itsWindowRectangle.width + 20;
				}
				else if(itsWindowRectangle.x > Screen.width - 20)
				{
					itsWindowRectangle.x = Screen.width - 20;
				}
				
				if(itsWindowRectangle.y < -itsWindowRectangle.height + 20)
				{
					itsWindowRectangle.y = -itsWindowRectangle.height + 20;
				}
				else if(itsWindowRectangle.y > Screen.height - 20)
				{
					itsWindowRectangle.y = Screen.height - 20;
				}
			}
		}
			 */
			#endregion
		}
		KGFGUIUtility.SetSkinIndex(1);
	}

	private static KGFCustomGUI GetInstance()
	{
		return itsInstance;
	}

	private void DrawCurrentCustomGUI(float aCustomGuiWidth)
	{
		if(itsCurrentSelectedGUI == null)
			return;
		
		float aHeight = KGFGUIUtility.GetSkinHeight() + KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButton).margin.vertical + KGFGUIUtility.GetStyleBox(KGFGUIUtility.eStyleBox.eBoxDecorated).padding.vertical;
		GUILayout.BeginArea(new Rect(aHeight,aHeight,Screen.width-aCustomGuiWidth-aHeight,Screen.height-aHeight*2.0f));
		{
			KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBox);
			{
				if(itsCurrentSelectedGUI.GetIcon() == null)
				{
					KGFGUIUtility.BeginWindowHeader(itsCurrentSelectedGUI.GetHeaderName(), itsDataModuleCustomGUI.itsUnknownIcon);
				}
				else
				{
					KGFGUIUtility.BeginWindowHeader(itsCurrentSelectedGUI.GetHeaderName(), itsCurrentSelectedGUI.GetIcon());
				}
				GUILayout.FlexibleSpace();
				bool aClose = KGFGUIUtility.EndWindowHeader(true);
				
				//Draw the content
				if(!aClose)
				{
					// hack to keep the window in min size
					GUILayout.Space(0);
					itsCurrentSelectedGUI.Render();
				}
				else
				{
					itsCurrentSelectedGUI = null;
				}
			}
			KGFGUIUtility.EndVerticalBox();
		}
		GUILayout.EndArea();
	}

	public static Texture2D GetDefaultIcon()
	{
		if(itsInstance != null)
		{
			return itsInstance.itsDataModuleCustomGUI.itsUnknownIcon;
		}
		
		return null;
	}

	public override Texture2D GetIcon()
	{
		return null;
	}

	public override string GetName()
	{
		return "KGFCustomGUI";
	}
	
	public override string GetDocumentationPath()
	{
		return "KGFCustomGUIManual.html";
	}
	
	public override string GetForumPath()
	{
		return string.Empty;
	}
	
	#region KGFIValidator

	public override KGFMessageList Validate()
	{
		KGFMessageList aMessageList = new KGFMessageList();
		
		if(itsDataModuleCustomGUI.itsUnknownIcon == null)
		{
			aMessageList.AddWarning("the unknown icon is missing");
		}
		
		if(itsDataModuleCustomGUI.itsModifierKey == itsDataModuleCustomGUI.itsSchortcutKey)
		{
			aMessageList.AddInfo("the modifier key is equal to the shortcut key");
		}
		
		return aMessageList;
	}

	#endregion
}