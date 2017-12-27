using System;
using System.Collections;
using UnityEngine;

public class KGFMapSystemStyleDemo : KGFModule
{
	public MapSystemStyle[]itsStyles = new KGFMapSystemStyleDemo.MapSystemStyle[0];
	private KGFMapSystem itsMapSystem = null;
	public Texture2D itsKOLMICHTexture = null;
	
	private bool[] itsHorizontalAlignment;
	private bool[] itsVerticalAlignment;
	private bool[] itsHorizontalMargin;
	private bool[] itsVerticalMargin;
	private bool itsHideGui;
	private bool itsEnablePostEffects = true;
	private float itsSize = 0.415f;
	private float itsMapSize = 0.85f;
	private float itsButtonSize = 0.15f;
	private float itsMapButtonSize = 0.05f;
	private float itsButtonPadding = 0.03f;
	private float itsMapButtonPadding = 0.01f;
	private float itsMapButtonSpace = 0.01f;
	private float itsIconSize = 1.0f;
	private float itsMapIconSize = 1.24f;
	private float itsArrowSize = 0.15f;
	private float itsRadiusArrows = 0.8f;
	private bool itsFullscreen = false;
	private bool[] itsMapButtonOrientation;
	private bool[] itsMapButtonAlighmentH;
	private bool[] itsMapButtonAlighmentV;
	
	private Rect itsRect;
	
	[System.SerializableAttribute]
	public class MapSystemStyle
	{
		public string itsName;
		public Texture2D itsBackgroundMinimap;
		public Texture2D itsBackgroundMap;
		public Texture2D itsBackgroundTooltip;
		public Texture2D itsButton;
		public Texture2D itsButtonHover;
		public Texture2D itsButtonDown;
		public Texture2D itsButtonZoomIn;
		public Texture2D itsButtonZoomOut;
		public Texture2D itsButtonMap;
		public Texture2D itsButtonLock;
		public Color itsColorMap;
		public Color itsColorAll;
		public Texture2D itsMinimapMask;
		public Texture2D itsMapMask;
		public float itsMarginButtons;
		public Color itsViewportColor;
	}
	
//	GUIStyle itsGuiStyle = null;
	
	
	public KGFMapSystemStyleDemo() : base(new Version(1,0,0,0), new Version(1,1,0,0))
	{}
	
	protected override void KGFAwake()
	{
		base.KGFAwake();
		itsRect = new Rect(0.0f,0.0f,400.0f,Screen.height);
		KGFGUIUtility.SetSkinPath("KGFSkins/default/skins/skin_default_16");
		itsHorizontalAlignment = new bool[3];
		itsVerticalAlignment = new bool[3];
		SetBoolArray(2,itsHorizontalAlignment);
		SetBoolArray(0,itsVerticalAlignment);
		itsHorizontalMargin = new bool[3];
		itsVerticalMargin = new bool[3];
		SetBoolArray(0,itsHorizontalMargin);
		SetBoolArray(0,itsVerticalMargin);
		itsMapButtonOrientation = new bool[2];
		SetBoolArray(0,itsMapButtonOrientation);
		itsMapButtonAlighmentH = new bool[3];
		SetBoolArray(2,itsMapButtonAlighmentH);
		itsMapButtonAlighmentV = new bool[3];
		SetBoolArray(1,itsMapButtonAlighmentV);
		
		
		UpdateStyle(1);
	}
	
	void Start()
	{
		itsMapSystem = KGFAccessor.GetObject<KGFMapSystem>();
		itsMapSystem.EventFullscreenModeChanged += OnFullScreenModeChanged;
	}
	
	void OnFullScreenModeChanged(object theSender, EventArgs theEventArgs)
	{
		bool aFullScreenMode = itsMapSystem.GetFullscreen();
		itsFullscreen = aFullScreenMode;
	}
	
	private void ClearAlignment(bool[] theAlignment)
	{
		for(int i = 0; i< theAlignment.Length; i++)
			theAlignment[i] = false;
	}
	
	private void SetBoolArray(int theIndex, bool[] theAlignment)
	{
		ClearAlignment(theAlignment);
		theAlignment[theIndex] = true;
	}
	
	/// <summary>
	/// Draw buttons
	/// </summary>
	void OnGUI()
	{
		GUILayout.BeginArea(itsRect);
		{
			GUILayout.BeginVertical();
			{
				GUILayout.BeginHorizontal();
				{
					GUILayout.Label(itsKOLMICHTexture);
				}
				GUILayout.EndHorizontal();
				
				GUILayout.FlexibleSpace();
				
				GUILayout.BeginHorizontal();
				{
					KGFGUIUtility.Space();
					KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxDecorated);
					{
						if(!itsFullscreen)
						{
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkTop);
							GUILayout.FlexibleSpace();
							KGFGUIUtility.Label("TEST MINIMAP FEATURES HERE!");
							GUILayout.FlexibleSpace();
							KGFGUIUtility.EndHorizontalBox();
							
							KGFGUIUtility.Space();
							GUILayout.Label(" 1) Click into the map and drag it around!");
							GUILayout.Label(" 2) Click into the map to place a bookmark!");
							GUILayout.Label(" 3) Use the scrollwheel for zooming!");
							GUILayout.Label(" 4) Hover over an icon to reveal its tooltip!");
							GUILayout.Label(" 5) Use all controls below to test some configuration features!");
							KGFGUIUtility.Space();
							
							//toggle minimap
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
							KGFGUIUtility.Label("switch to big map: ");
							GUI.color = new Color(0.5f,1.0f,0.5f,1.0f);
							if(KGFGUIUtility.Button("switch now",KGFGUIUtility.eStyleButton.eButton))
							{
								itsMapSystem.SetFullscreen(true);
								itsFullscreen = true;
							}
							GUI.color = new Color(1.0f,1.0f,1.0f,1.0f);
							KGFGUIUtility.EndHorizontalBox();
							
							KGFGUIUtility.Space();
							
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
							KGFGUIUtility.Label("skin samples: ");
							if (KGFGUIUtility.Button(itsStyles[0].itsName,KGFGUIUtility.eStyleButton.eButton))
								UpdateStyle(0);
							if (KGFGUIUtility.Button(itsStyles[2].itsName,KGFGUIUtility.eStyleButton.eButton))
								UpdateStyle(2);
							if (KGFGUIUtility.Button(itsStyles[3].itsName,KGFGUIUtility.eStyleButton.eButton))
								UpdateStyle(3);
							if (KGFGUIUtility.Button(itsStyles[1].itsName,KGFGUIUtility.eStyleButton.eButton))
								UpdateStyle(1);
							KGFGUIUtility.EndHorizontalBox();
							
							
							//size
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
							KGFGUIUtility.Label("minimap size: ",KGFGUIUtility.eStyleLabel.eLabel);
							float aSize = KGFGUIUtility.HorizontalSlider(itsSize,0.2f,0.6f);
							if(aSize != itsSize)
							{
								itsSize = aSize;
								itsMapSystem.SetMinimapSize(itsSize);
							}
							KGFGUIUtility.EndHorizontalBox();
							
							//button size
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
							KGFGUIUtility.Label("button size: ",KGFGUIUtility.eStyleLabel.eLabel);
							aSize = KGFGUIUtility.HorizontalSlider(itsButtonSize,0.1f,0.3f);
							if(aSize != itsButtonSize)
							{
								itsButtonSize = aSize;
								itsMapSystem.SetMinimapButtonSize(itsButtonSize);
							}
							KGFGUIUtility.EndHorizontalBox();
							
							//button padding
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
							KGFGUIUtility.Label("button padding: ",KGFGUIUtility.eStyleLabel.eLabel);
							aSize = KGFGUIUtility.HorizontalSlider(itsButtonPadding,0.01f,0.1f);
							if(aSize != itsButtonPadding)
							{
								itsButtonPadding = aSize;
								itsMapSystem.SetMinimapButtonPadding(itsButtonPadding);
							}
							KGFGUIUtility.EndHorizontalBox();
							
							//icon size
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
							KGFGUIUtility.Label("icon size: ",KGFGUIUtility.eStyleLabel.eLabel);
							aSize = KGFGUIUtility.HorizontalSlider(itsIconSize,0.5f,2.0f);
							if(aSize != itsIconSize)
							{
								itsIconSize = aSize;
								itsMapSystem.SetMinimapIconScale(itsIconSize);
							}
							KGFGUIUtility.EndHorizontalBox();
							
							//arrow size
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
							KGFGUIUtility.Label("arrow size: ",KGFGUIUtility.eStyleLabel.eLabel);
							aSize = KGFGUIUtility.HorizontalSlider(itsArrowSize,0.1f,0.3f);
							if(aSize != itsArrowSize)
							{
								itsArrowSize = aSize;
								itsMapSystem.SetMinimapArrowScale(itsArrowSize);
							}
							KGFGUIUtility.EndHorizontalBox();
							
							//arrow radius
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
							KGFGUIUtility.Label("arrow radius: ",KGFGUIUtility.eStyleLabel.eLabel);
							aSize = KGFGUIUtility.HorizontalSlider(itsRadiusArrows,0.4f,1.0f);
							if(aSize != itsRadiusArrows)
							{
								itsRadiusArrows = aSize;
								itsMapSystem.SetMinimapArrowRadius(itsRadiusArrows);
							}
							KGFGUIUtility.EndHorizontalBox();
							
							//horizontal alignment
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
							KGFGUIUtility.Label("horizontal alignment: ",KGFGUIUtility.eStyleLabel.eLabel);
							GUILayout.FlexibleSpace();
							bool aHorizontalAlignment = KGFGUIUtility.Toggle(itsHorizontalAlignment[0]," left",KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact);
							if(aHorizontalAlignment != itsHorizontalAlignment[0] && aHorizontalAlignment == true)
							{
								SetBoolArray(0,itsHorizontalAlignment);
								itsMapSystem.SetMinimapHorizontalAlignment(KGFAlignmentHorizontal.Left);
							}
							aHorizontalAlignment = KGFGUIUtility.Toggle(itsHorizontalAlignment[1]," middle",KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact);
							if(aHorizontalAlignment != itsHorizontalAlignment[1] && aHorizontalAlignment == true)
							{
								SetBoolArray(1,itsHorizontalAlignment);
								itsMapSystem.SetMinimapHorizontalAlignment(KGFAlignmentHorizontal.Middle);
							}
							aHorizontalAlignment = KGFGUIUtility.Toggle(itsHorizontalAlignment[2]," right",KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact);
							if(aHorizontalAlignment != itsHorizontalAlignment[2] && aHorizontalAlignment == true)
							{
								SetBoolArray(2,itsHorizontalAlignment);
								itsMapSystem.SetMinimapHorizontalAlignment(KGFAlignmentHorizontal.Right);
							}
							KGFGUIUtility.EndHorizontalBox();
							
							//vertical alignment
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
							KGFGUIUtility.Label("vertical alignment: ",KGFGUIUtility.eStyleLabel.eLabel);
							GUILayout.FlexibleSpace();
							bool aVerticalAlignment = KGFGUIUtility.Toggle(itsVerticalAlignment[0]," top",KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact);
							if(aVerticalAlignment != itsVerticalAlignment[0] && aVerticalAlignment == true)
							{
								SetBoolArray(0,itsVerticalAlignment);
								itsMapSystem.SetMinimapVerticalAlignment(KGFAlignmentVertical.Top);
							}
							aVerticalAlignment = KGFGUIUtility.Toggle(itsVerticalAlignment[1]," middle",KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact);
							if(aVerticalAlignment != itsVerticalAlignment[1] && aVerticalAlignment == true)
							{
								SetBoolArray(1,itsVerticalAlignment);
								itsMapSystem.SetMinimapVerticalAlignment(KGFAlignmentVertical.Middle);
							}
							aVerticalAlignment = KGFGUIUtility.Toggle(itsVerticalAlignment[2]," bottom",KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact);
							if(aVerticalAlignment != itsVerticalAlignment[2] && aVerticalAlignment == true)
							{
								SetBoolArray(2,itsVerticalAlignment);
								itsMapSystem.SetMinimapVerticalAlignment(KGFAlignmentVertical.Bottom);
							}
							KGFGUIUtility.EndHorizontalBox();
							
							//horizontal Margin
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
							KGFGUIUtility.Label("horizontal margin: ",KGFGUIUtility.eStyleLabel.eLabel);
							GUILayout.FlexibleSpace();
							bool aHorizontalMargin = KGFGUIUtility.Toggle(itsHorizontalMargin[0]," 0.0",KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact);
							if(aHorizontalMargin != itsHorizontalMargin[0] && aHorizontalMargin == true)
							{
								SetBoolArray(0,itsHorizontalMargin);
								itsMapSystem.SetMinimapHorizontalMargin(0.0f);
							}
							aHorizontalMargin = KGFGUIUtility.Toggle(itsHorizontalMargin[1]," 0.025",KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact);
							if(aHorizontalMargin != itsHorizontalMargin[1] && aHorizontalMargin == true)
							{
								SetBoolArray(1,itsHorizontalMargin);
								itsMapSystem.SetMinimapHorizontalMargin(0.025f);
							}
							aHorizontalMargin = KGFGUIUtility.Toggle(itsHorizontalMargin[2]," 0.05",KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact);
							if(aHorizontalMargin != itsHorizontalMargin[2] && aHorizontalMargin == true)
							{
								SetBoolArray(2,itsHorizontalMargin);
								itsMapSystem.SetMinimapHorizontalMargin(0.05f);
							}
							KGFGUIUtility.EndHorizontalBox();
							
							//vertical Margin
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
							KGFGUIUtility.Label("vertical margin: ",KGFGUIUtility.eStyleLabel.eLabel);
							GUILayout.FlexibleSpace();
							bool aVerticalMargin = KGFGUIUtility.Toggle(itsVerticalMargin[0]," 0.0",KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact);
							if(aVerticalMargin != itsVerticalMargin[0] && aVerticalMargin == true)
							{
								SetBoolArray(0,itsVerticalMargin);
								itsMapSystem.SetMinimapVerticalMargin(0.0f);
							}
							aVerticalMargin = KGFGUIUtility.Toggle(itsVerticalMargin[1]," 0.025",KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact);
							if(aVerticalMargin != itsVerticalMargin[1] && aVerticalMargin == true)
							{
								SetBoolArray(1,itsVerticalMargin);
								itsMapSystem.SetMinimapVerticalMargin(0.025f);
							}
							aVerticalMargin = KGFGUIUtility.Toggle(itsVerticalMargin[2]," 0.05",KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact);
							if(aVerticalMargin != itsVerticalMargin[2] && aVerticalMargin == true)
							{
								SetBoolArray(2,itsVerticalMargin);
								itsMapSystem.SetMinimapVerticalMargin(0.05f);
							}
							KGFGUIUtility.EndHorizontalBox();
							
							
							//gui
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
							KGFGUIUtility.Label("hide gui: ",KGFGUIUtility.eStyleLabel.eLabel);
							GUILayout.FlexibleSpace();
							bool aHideGui = KGFGUIUtility.Toggle(itsHideGui,"",KGFGUIUtility.eStyleToggl.eTogglSwitch);
							if(aHideGui != itsHideGui)
							{
								itsHideGui = aHideGui;
								itsMapSystem.SetGlobalHideGui(itsHideGui);
							}
							KGFGUIUtility.EndHorizontalBox();
							
							//effects
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
							KGFGUIUtility.Label("enable posteffects: ",KGFGUIUtility.eStyleLabel.eLabel);
							GUILayout.FlexibleSpace();
							bool anEnableEffects = KGFGUIUtility.Toggle(itsEnablePostEffects,"",KGFGUIUtility.eStyleToggl.eTogglSwitch);
							if(anEnableEffects != itsEnablePostEffects)
							{
								itsEnablePostEffects = anEnableEffects;
								Transform aMinimapCamera = itsMapSystem.transform.Find("camera_minimap");
								
								Behaviour[] aComponents = aMinimapCamera.GetComponents<Behaviour>();
								foreach(Behaviour aComponent in aComponents)
								{
									if(aComponent.GetType().ToString() == "NoiseEffect")
									{
										aComponent.enabled = itsEnablePostEffects;
									}
									if(aComponent.GetType().ToString() == "BloomAndLensFlares")
									{
										aComponent.enabled = itsEnablePostEffects;
									}
								}
							}
							KGFGUIUtility.EndHorizontalBox();
						}
						else
						{
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkTop);
							GUILayout.FlexibleSpace();
							KGFGUIUtility.Label("TEST MAP FEATURES HERE!");
							GUILayout.FlexibleSpace();
							KGFGUIUtility.EndHorizontalBox();
							
							KGFGUIUtility.Space();
							GUILayout.Label(" 1) Click into the map and drag it around!");
							GUILayout.Label(" 2) Click into the map to place a bookmark!");
							GUILayout.Label(" 3) Use the scrollwheel for zooming!");
							GUILayout.Label(" 4) Hover over an icon to reveal its tooltip!");
							GUILayout.Label(" 5) Use all controls below to test some configuration features!");
							KGFGUIUtility.Space();
							
							//toggle minimap
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
							KGFGUIUtility.Label("switch to minimap: ");
							GUI.color = new Color(0.5f,1.0f,0.5f,1.0f);
							if(KGFGUIUtility.Button("switch now",KGFGUIUtility.eStyleButton.eButton))
							{
								itsMapSystem.SetFullscreen(false);
								itsFullscreen = false;
							}
							GUI.color = new Color(1.0f,1.0f,1.0f,1.0f);
							KGFGUIUtility.EndHorizontalBox();
							
							KGFGUIUtility.Space();
							
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
							KGFGUIUtility.Label("skin samples: ");
							if (KGFGUIUtility.Button(itsStyles[0].itsName,KGFGUIUtility.eStyleButton.eButton))
								UpdateStyle(0);
							if (KGFGUIUtility.Button(itsStyles[2].itsName,KGFGUIUtility.eStyleButton.eButton))
								UpdateStyle(2);
							if (KGFGUIUtility.Button(itsStyles[3].itsName,KGFGUIUtility.eStyleButton.eButton))
								UpdateStyle(3);
							if (KGFGUIUtility.Button(itsStyles[1].itsName,KGFGUIUtility.eStyleButton.eButton))
								UpdateStyle(1);
							KGFGUIUtility.EndHorizontalBox();
							
							
							//size
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
							KGFGUIUtility.Label("map size: ",KGFGUIUtility.eStyleLabel.eLabel);
							float aSize = KGFGUIUtility.HorizontalSlider(itsMapSize,0.6f,1.0f);
							if(aSize != itsMapSize)
							{
								itsMapSize = aSize;
								itsMapSystem.SetMapSize(itsMapSize);
							}
							KGFGUIUtility.EndHorizontalBox();
							
							//button size
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
							KGFGUIUtility.Label("button size: ",KGFGUIUtility.eStyleLabel.eLabel);
							aSize = KGFGUIUtility.HorizontalSlider(itsMapButtonSize,0.03f,0.08f);
							if(aSize != itsMapButtonSize)
							{
								itsMapButtonSize = aSize;
								itsMapSystem.SetMapButtonSize(itsMapButtonSize);
							}
							KGFGUIUtility.EndHorizontalBox();
							
							//button padding
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
							KGFGUIUtility.Label("button padding: ",KGFGUIUtility.eStyleLabel.eLabel);
							aSize = KGFGUIUtility.HorizontalSlider(itsMapButtonPadding,0.01f,0.1f);
							if(aSize != itsMapButtonPadding)
							{
								itsMapButtonPadding = aSize;
								itsMapSystem.SetMapButtonPadding(itsMapButtonPadding);
							}
							KGFGUIUtility.EndHorizontalBox();
							
							//button space
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
							KGFGUIUtility.Label("button space: ",KGFGUIUtility.eStyleLabel.eLabel);
							aSize = KGFGUIUtility.HorizontalSlider(itsMapButtonSpace,0.01f,0.1f);
							if(aSize != itsMapButtonSpace)
							{
								itsMapButtonSpace = aSize;
								itsMapSystem.SetMapButtonSpace(itsMapButtonSpace);
							}
							KGFGUIUtility.EndHorizontalBox();
							
							//icon size
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
							KGFGUIUtility.Label("icon size: ",KGFGUIUtility.eStyleLabel.eLabel);
							aSize = KGFGUIUtility.HorizontalSlider(itsMapIconSize,0.8f,2.0f);
							if(aSize != itsMapIconSize)
							{
								itsMapIconSize = aSize;
								itsMapSystem.SetMapIconScale(itsMapIconSize);
							}
							KGFGUIUtility.EndHorizontalBox();
							
							//button orientation
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
							KGFGUIUtility.Label("button orientation: ",KGFGUIUtility.eStyleLabel.eLabel);
							GUILayout.FlexibleSpace();
							bool anOrientation = KGFGUIUtility.Toggle(itsMapButtonOrientation[0]," horizontal",KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact);
							if(anOrientation != itsMapButtonOrientation[0] && anOrientation == true)
							{
								SetBoolArray(0,itsMapButtonOrientation);
								itsMapSystem.SetMapButtonOrientation(KGFOrientation.Horizontal);
							}
							anOrientation = KGFGUIUtility.Toggle(itsMapButtonOrientation[1]," vertical",KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact);
							if(anOrientation != itsMapButtonOrientation[1] && anOrientation == true)
							{
								SetBoolArray(1,itsMapButtonOrientation);
								itsMapSystem.SetMapButtonOrientation(KGFOrientation.Vertical);
							}
							KGFGUIUtility.EndHorizontalBox();
							
							//button alignment horizontal
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
							KGFGUIUtility.Label("button alignment H: ",KGFGUIUtility.eStyleLabel.eLabel);
							GUILayout.FlexibleSpace();
							bool anAlighment = KGFGUIUtility.Toggle(itsMapButtonAlighmentH[0]," left",KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact);
							if(anAlighment != itsMapButtonAlighmentH[0] && anAlighment == true)
							{
								SetBoolArray(0,itsMapButtonAlighmentH);
								itsMapSystem.SetMapButtonAlighmentHorizontal(KGFAlignmentHorizontal.Left);
							}
							anAlighment = KGFGUIUtility.Toggle(itsMapButtonAlighmentH[1]," middle",KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact);
							if(anAlighment != itsMapButtonAlighmentH[1] && anAlighment == true)
							{
								SetBoolArray(1,itsMapButtonAlighmentH);
								itsMapSystem.SetMapButtonAlighmentHorizontal(KGFAlignmentHorizontal.Middle);
							}
							anAlighment = KGFGUIUtility.Toggle(itsMapButtonAlighmentH[2]," right",KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact);
							if(anAlighment != itsMapButtonAlighmentH[2] && anAlighment == true)
							{
								SetBoolArray(2,itsMapButtonAlighmentH);
								itsMapSystem.SetMapButtonAlighmentHorizontal(KGFAlignmentHorizontal.Right);
							}
							KGFGUIUtility.EndHorizontalBox();
							
							//button alignment vertical
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
							KGFGUIUtility.Label("button alignment V: ",KGFGUIUtility.eStyleLabel.eLabel);
							GUILayout.FlexibleSpace();
							anAlighment = KGFGUIUtility.Toggle(itsMapButtonAlighmentV[0]," top",KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact);
							if(anAlighment != itsMapButtonAlighmentV[0] && anAlighment == true)
							{
								SetBoolArray(0,itsMapButtonAlighmentV);
								itsMapSystem.SetMapButtonAlighmentVertical(KGFAlignmentVertical.Top);
							}
							anAlighment = KGFGUIUtility.Toggle(itsMapButtonAlighmentV[1]," middle",KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact);
							if(anAlighment != itsMapButtonAlighmentV[1] && anAlighment == true)
							{
								SetBoolArray(1,itsMapButtonAlighmentV);
								itsMapSystem.SetMapButtonAlighmentVertical(KGFAlignmentVertical.Middle);
							}
							anAlighment = KGFGUIUtility.Toggle(itsMapButtonAlighmentV[2]," bottom",KGFGUIUtility.eStyleToggl.eTogglRadioSuperCompact);
							if(anAlighment != itsMapButtonAlighmentV[2] && anAlighment == true)
							{
								SetBoolArray(2,itsMapButtonAlighmentV);
								itsMapSystem.SetMapButtonAlighmentVertical(KGFAlignmentVertical.Bottom);
							}
							KGFGUIUtility.EndHorizontalBox();
							
							//gui
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
							KGFGUIUtility.Label("hide gui: ",KGFGUIUtility.eStyleLabel.eLabel);
							GUILayout.FlexibleSpace();
							bool aHideGui = KGFGUIUtility.Toggle(itsHideGui,"",KGFGUIUtility.eStyleToggl.eTogglSwitch);
							if(aHideGui != itsHideGui)
							{
								itsHideGui = aHideGui;
								itsMapSystem.SetGlobalHideGui(itsHideGui);
							}
							KGFGUIUtility.EndHorizontalBox();
							
							//effects
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
							KGFGUIUtility.Label("enable posteffects: ",KGFGUIUtility.eStyleLabel.eLabel);
							GUILayout.FlexibleSpace();
							bool anEnableEffects = KGFGUIUtility.Toggle(itsEnablePostEffects,"",KGFGUIUtility.eStyleToggl.eTogglSwitch);
							if(anEnableEffects != itsEnablePostEffects)
							{
								itsEnablePostEffects = anEnableEffects;
								Transform aMinimapCamera = itsMapSystem.transform.Find("camera_minimap");
								
								Behaviour[] aComponents = aMinimapCamera.GetComponents<Behaviour>();
								foreach(Behaviour aComponent in aComponents)
								{
									if(aComponent.GetType().ToString() == "NoiseEffect")
									{
										aComponent.enabled = itsEnablePostEffects;
									}
									if(aComponent.GetType().ToString() == "BloomAndLensFlares")
									{
										aComponent.enabled = itsEnablePostEffects;
									}
								}
							}
							KGFGUIUtility.EndHorizontalBox();														
						}
					}
					KGFGUIUtility.EndVerticalBox();
				}
				GUILayout.EndHorizontal();
			}
			KGFGUIUtility.Space();
			GUILayout.EndVertical();
		}
		GUILayout.EndArea();
	}
	
	/// <summary>
	/// Update the style of the map system
	/// </summary>
	/// <param name="theIndex"></param>
	void UpdateStyle(int theIndex)
	{
		MapSystemStyle aStyle = itsStyles[theIndex];
		
//		if(theIndex == 0)
//			KGFGUIUtility.SetSkinPath("KGFSkins/default/skins/skin_default_32");
//		if(theIndex == 1)
//			KGFGUIUtility.SetSkinPath("KGFSkins/default/skins/skin_default_32");
//		if(theIndex == 2)
//			KGFGUIUtility.SetSkinPath("KGFSkins/scifi/skins/skin_scifi_32");
//		if(theIndex == 3)
//			KGFGUIUtility.SetSkinPath("KGFSkins/fantasy/skins/skin_fantasy_32");
		
		KGFMapSystem aMapSystem = KGFAccessor.GetObject<KGFMapSystem>();
		if (aMapSystem != null)
		{
			aMapSystem.itsDataModuleMinimap.itsAppearanceMap.itsBackground = aStyle.itsBackgroundMap;
			aMapSystem.itsDataModuleMinimap.itsAppearanceMiniMap.itsBackground = aStyle.itsBackgroundMinimap;
			
			aMapSystem.itsDataModuleMinimap.itsAppearanceMap.itsButton = aStyle.itsButton;
			aMapSystem.itsDataModuleMinimap.itsAppearanceMap.itsButtonHover = aStyle.itsButtonHover;
			aMapSystem.itsDataModuleMinimap.itsAppearanceMap.itsButtonDown = aStyle.itsButtonDown;
			
			aMapSystem.itsDataModuleMinimap.itsAppearanceMiniMap.itsButton = aStyle.itsButton;
			aMapSystem.itsDataModuleMinimap.itsAppearanceMiniMap.itsButtonHover = aStyle.itsButtonHover;
			aMapSystem.itsDataModuleMinimap.itsAppearanceMiniMap.itsButtonDown = aStyle.itsButtonDown;
			
			
			aMapSystem.itsDataModuleMinimap.itsAppearanceMiniMap.itsIconZoomIn = aStyle.itsButtonZoomIn;
			aMapSystem.itsDataModuleMinimap.itsAppearanceMiniMap.itsIconZoomOut = aStyle.itsButtonZoomOut;
			aMapSystem.itsDataModuleMinimap.itsAppearanceMiniMap.itsIconFullscreen = aStyle.itsButtonMap;
			aMapSystem.itsDataModuleMinimap.itsAppearanceMiniMap.itsIconZoomLock = aStyle.itsButtonLock;
			aMapSystem.itsDataModuleMinimap.itsGlobalSettings.itsColorMap = aStyle.itsColorMap;
			
			aMapSystem.itsDataModuleMinimap.itsAppearanceMap.itsIconZoomIn = aStyle.itsButtonZoomIn;
			aMapSystem.itsDataModuleMinimap.itsAppearanceMap.itsIconZoomOut = aStyle.itsButtonZoomOut;
			aMapSystem.itsDataModuleMinimap.itsAppearanceMap.itsIconFullscreen = aStyle.itsButtonMap;
			aMapSystem.itsDataModuleMinimap.itsAppearanceMap.itsIconZoomLock = aStyle.itsButtonLock;
			
			aMapSystem.SetMask(aStyle.itsMinimapMask, aStyle.itsMapMask);
			aMapSystem.itsDataModuleMinimap.itsAppearanceMiniMap.itsButtonPadding = aStyle.itsMarginButtons;
			aMapSystem.itsDataModuleMinimap.itsGlobalSettings.itsColorAll = aStyle.itsColorAll;
			
			aMapSystem.itsDataModuleMinimap.itsViewport.itsColor = aStyle.itsViewportColor;
			
			aMapSystem.itsDataModuleMinimap.itsToolTip.itsTextureBackground = aStyle.itsBackgroundTooltip;
			
			aMapSystem.UpdateStyles();
		}
		
//		itsGuiStyle = new GUIStyle();
//		itsGuiStyle.normal.background = aStyle.itsButton;
//		itsGuiStyle.hover.background = aStyle.itsButtonHover;
//		itsGuiStyle.active.background = aStyle.itsButtonDown;
	}
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Alpha1))
			UpdateStyle(0);
		else if(Input.GetKeyDown(KeyCode.Alpha2))
			UpdateStyle(1);
		else if(Input.GetKeyDown(KeyCode.Alpha3))
			UpdateStyle(2);
		else if(Input.GetKeyDown(KeyCode.Alpha4))
			UpdateStyle(3);
	}
	
	#region KGFModule methods
	public override KGFMessageList Validate()
	{
		return new KGFMessageList();
	}
	
	public override string GetName()
	{
		return name;
	}
	
	public override Texture2D GetIcon()
	{
		return null;
	}
	
	public override string GetForumPath()
	{
		return "";
	}
	
	public override string GetDocumentationPath()
	{
		return "";
	}
	#endregion
}
