// <author>Christoph Hausjell</author>
// <email>christoph.hausjell@kolmich.at</email>
// <date>2011-03-01</date>
// <summary>this file contasins the class KGFGUIUtility and some enums to switch between diffent GUI styles of the controls</summary>

using System.Runtime.Serialization.Formatters;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// contains methods for easier usage of the Kolmich Game Framework skins.
/// </summary>
/// <remarks>
/// This class defines methods and enumerations to use the KOLMICH Game Framework skins as easy as possible.
/// </remarks>
public class KGFGUIUtility
{
	#region internal enums
	
	/// <summary>
	/// enumeration of all possible button styles
	/// </summary>
	/// <remarks>
	/// Buttons can be used to get userinput on gui. By using button styles with direction, it is possible to group them horizontal or vertical.
	/// </remarks>
	public enum eStyleButton
	{
		eButton,
		eButtonLeft,
		eButtonRight,
		eButtonTop,
		eButtonBottom,
		eButtonMiddle
	}
	
	/// <summary>
	/// enumeration of all possible toggle styles
	/// </summary>
	/// <remarks>
	/// Toggle buttons can be used to allow the user to switch between an on and off state.
	/// </remarks>
	public enum eStyleToggl
	{
		eToggl,
		eTogglStreched,
		eTogglCompact,
		eTogglSuperCompact,
		eTogglRadioStreched,
		eTogglRadioCompact,
		eTogglRadioSuperCompact,
		eTogglSwitch,
		eTogglBoolean,
		eTogglArrow,
		eTogglButton
	}
	
	/// <summary>
	/// enumeration of all possible textfield styles
	/// </summary>
	/// <remarks>
	/// Textfields can be used to get the user`s keyborad input. By using the textfield styles with direction it is possible to group them horizontal or vertical.
	/// </remarks>
	public enum eStyleTextField
	{
		eTextField,
		eTextFieldLeft,
		eTextFieldRight
	}
	
	/// <summary>
	/// enumeration of all possible box styles
	/// </summary>
	/// <remarks>
	/// Boxes can be used to group an align controls. The direction  in the boxname gives inforamtion about the position. this means an eBoxTop style has no bottom padding.
	/// </remarks>
	public enum eStyleBox
	{
		eBox,
		eBoxInvisible,
		eBoxInteractive,
		eBoxLeft,
		eBoxLeftInteractive,
		eBoxRight,
		eBoxRightInteractive,
		eBoxMiddleHorizontal,
		eBoxMiddleHorizontalInteractive,
		eBoxTop,
		eBoxTopInteractive,
		eBoxMiddleVertical,
		eBoxMiddleVerticalInteractive,
		eBoxBottom,
		eBoxBottomInteractive,
		eBoxDark,
		eBoxDarkInteractive,
		eBoxDarkLeft,
		eBoxDarkLeftInteractive,
		eBoxDarkRight,
		eBoxDarkRightInteractive,
		eBoxDarkMiddleHorizontal,
		eBoxDarkMiddleHorizontalInteractive,
		eBoxDarkTop,
		eBoxDarkTopInteractive,
		eBoxDarkBottom,
		eBoxDarkBottomInteractive,
		eBoxDarkMiddleVertical,
		eBoxDarkMiddleVerticalInteractive,
		eBoxDecorated
	}
	
	/// <summary>
	/// enumeration of all possible seperator styles
	/// </summary>
	/// <remarks>
	/// Seperators can be used to create space between horizontal or vertical oriented gui objects.
	/// </remarks>
	public enum eStyleSeparator
	{
		eSeparatorHorizontal,
		eSeparatorVertical,
		eSeparatorVerticalFitInBox,
	}
	
	/// <summary>
	/// enumeration of all possible label styles
	/// </summary>
	/// <remarks>
	/// Labels can be used to show text or icons. By using the style eLabelFitIntoBox the label will not resize the parent control.
	/// </remarks>
	public enum eStyleLabel
	{
		eLabel,
		eLabelMultiline,
		eLabelTitle,
		eLabelFitIntoBox
	}
	
	/// <summary>
	/// enumeration of all possible image styles
	/// </summary>
	/// <remarks>
	/// Images can be used tho show images. Padding and margin of images are always 0. The FreeSize image has height and with 0 so it can be resized.
	/// </remarks>
	public enum eStyleImage
	{
		eImage,
		eImageFitIntoBox
	}
	
	/// <summary>
	/// enumeration of all possible cursor states
	/// </summary>
	/// <remarks>
	/// A cursor state represnents the clicked button direction in a cursor gui element.
	/// </remarks>
	public enum eCursorState
	{
		eUp,
		eRight,
		eDown,
		eLeft,
		eCenter,
		eNone
	}
	
	#endregion
	
	#region member
	
	private static bool itsEnableKGFSkins = true;
	
	private static string[] itsDefaultGuiSkinPath = new string[2]{"KGFSkins/default/skins/skin_default_16","KGFSkins/default/skins/skin_default_16"};
	
	/// <summary>
	/// itsSkinIndex is 0 for the editor skin and 1 for the game skin
	/// </summary>
	private static int itsSkinIndex = 1;
	
	private static bool[] itsResetPath = {false,false};
	protected static GUISkin[] itsSkin = new GUISkin[2];
	private static Texture2D itsIcon = null;
	private static Texture2D itsKGFCopyright = null;
	private static Texture2D itsIconHelp = null;
	
	static Dictionary<string, AudioClip> itsAudioClips = new Dictionary<string, AudioClip>();
	static float itsVolume = 1;
	
	#region default styles
	
	private static GUIStyle[] itsStyleToggle = new GUIStyle[2];
	private static GUIStyle[]	itsStyleTextField = new GUIStyle[2];
	private static GUIStyle[]	itsStyleTextFieldLeft = new GUIStyle[2];
	private static GUIStyle[]	itsStyleTextFieldRight = new GUIStyle[2];
	
	private static GUIStyle[]	itsStyleTextArea  = new GUIStyle[2];
	private static GUIStyle[] itsStyleWindow = new GUIStyle[2];
	
	private static GUIStyle[] itsStyleHorizontalSlider = new GUIStyle[2];
	private static GUIStyle[] itsStyleHorizontalSliderThumb = new GUIStyle[2];
	
	private static GUIStyle[] itsStyleVerticalSlider = new GUIStyle[2];
	private static GUIStyle[] itsStyleVerticalSliderThumb = new GUIStyle[2];
	
	private static GUIStyle[] itsStyleHorizontalScrollbar = new GUIStyle[2];
	private static GUIStyle[] itsStyleHorizontalScrollbarThumb = new GUIStyle[2];
	private static GUIStyle[] itsStyleHorizontalScrollbarLeftButton = new GUIStyle[2];
	private static GUIStyle[] itsStyleHorizontalScrollbarRightButton = new GUIStyle[2];
	
	private static GUIStyle[] itsStyleVerticalScrollbar = new GUIStyle[2];
	private static GUIStyle[] itsStyleVerticalScrollbarThumb = new GUIStyle[2];
	private static GUIStyle[] itsStyleVerticalScrollbarUpButton = new GUIStyle[2];
	private static GUIStyle[] itsStyleVerticalScrollbarDownButton = new GUIStyle[2];
	
	private static GUIStyle[] itsStyleScrollView = new GUIStyle[2];
	private static GUIStyle[] itsStyleMinimap = new GUIStyle[2];
	private static GUIStyle[] itsStyleMinimapButton = new GUIStyle[2];
	
	#endregion
	
	#region custom styles

	private static GUIStyle[] itsStyleToggleStreched = new GUIStyle[2];
	private static GUIStyle[] itsStyleToggleCompact = new GUIStyle[2];
	private static GUIStyle[] itsStyleToggleSuperCompact = new GUIStyle[2];
	
	private static GUIStyle[] itsStyleToggleRadioStreched = new GUIStyle[2];
	private static GUIStyle[] itsStyleToggleRadioCompact = new GUIStyle[2];
	private static GUIStyle[] itsStyleToggleRadioSuperCompact = new GUIStyle[2];
	
	private static GUIStyle[] itsStyleToggleSwitch = new GUIStyle[2];
	private static GUIStyle[] itsStyleToggleBoolean = new GUIStyle[2];
	private static GUIStyle[] itsStyleToggleArrow = new GUIStyle[2];
	private static GUIStyle[] itsStyleToggleButton = new GUIStyle[2];
	
	private static GUIStyle[] itsStyleButton = new GUIStyle[2];
	private static GUIStyle[] itsStyleButtonLeft = new GUIStyle[2];
	private static GUIStyle[] itsStyleButtonRight = new GUIStyle[2];
	private static GUIStyle[] itsStyleButtonTop = new GUIStyle[2];
	private static GUIStyle[] itsStyleButtonBottom = new GUIStyle[2];
	private static GUIStyle[] itsStyleButtonMiddle = new GUIStyle[2];
	
	private static GUIStyle[] itsStyleBox = new GUIStyle[2];
	private static GUIStyle[] itsStyleBoxInvisible = new GUIStyle[2];
	private static GUIStyle[] itsStyleBoxInteractive = new GUIStyle[2];
	private static GUIStyle[] itsStyleBoxLeft = new GUIStyle[2];
	private static GUIStyle[] itsStyleBoxLeftInteractive = new GUIStyle[2];
	private static GUIStyle[] itsStyleBoxRight = new GUIStyle[2];
	private static GUIStyle[] itsStyleBoxRightInteractive = new GUIStyle[2];
	private static GUIStyle[] itsStyleBoxMiddleHorizontal = new GUIStyle[2];
	private static GUIStyle[] itsStyleBoxMiddleHorizontalInteractive = new GUIStyle[2];
	private static GUIStyle[] itsStyleBoxTop = new GUIStyle[2];
	private static GUIStyle[] itsStyleBoxTopInteractive = new GUIStyle[2];
	private static GUIStyle[] itsStyleBoxBottom = new GUIStyle[2];
	private static GUIStyle[] itsStyleBoxBottomInteractive = new GUIStyle[2];
	private static GUIStyle[] itsStyleBoxMiddleVertical = new GUIStyle[2];
	private static GUIStyle[] itsStyleBoxMiddleVerticalInteractive = new GUIStyle[2];
	
	private static GUIStyle[] itsStyleBoxDark = new GUIStyle[2];
	private static GUIStyle[] itsStyleBoxDarkInteractive = new GUIStyle[2];
	private static GUIStyle[] itsStyleBoxDarkLeft = new GUIStyle[2];
	private static GUIStyle[] itsStyleBoxDarkLeftInteractive = new GUIStyle[2];
	private static GUIStyle[] itsStyleBoxDarkRight = new GUIStyle[2];
	private static GUIStyle[] itsStyleBoxDarkRightInteractive = new GUIStyle[2];
	private static GUIStyle[] itsStyleBoxDarkMiddleHorizontal = new GUIStyle[2];
	private static GUIStyle[] itsStyleBoxDarkMiddleHorizontalInteractive = new GUIStyle[2];
	private static GUIStyle[] itsStyleBoxDarkTop = new GUIStyle[2];
	private static GUIStyle[] itsStyleBoxDarkTopInteractive = new GUIStyle[2];
	private static GUIStyle[] itsStyleBoxDarkBottom = new GUIStyle[2];
	private static GUIStyle[] itsStyleBoxDarkBottomInteractive = new GUIStyle[2];
	private static GUIStyle[] itsStyleBoxDarkMiddleVertical = new GUIStyle[2];
	private static GUIStyle[] itsStyleBoxDarkMiddleVerticalInteractive = new GUIStyle[2];
	private static GUIStyle[] itsStyleBoxDecorated = new GUIStyle[2];
	
	private static GUIStyle[] itsStyleSeparatorVertical = new GUIStyle[2];
	private static GUIStyle[] itsStyleSeparatorVerticalFitInBox = new GUIStyle[2];
	private static GUIStyle[] itsStyleSeparatorHorizontal = new GUIStyle[2];
	
	private static GUIStyle[] itsStyleLabel = new GUIStyle[2];
	private static GUIStyle[] itsStyleLabelMultiline = new GUIStyle[2];
	private static GUIStyle[] itsStyleLabelTitle = new GUIStyle[2];
	private static GUIStyle[] itsStyleLabelFitInToBox = new GUIStyle[2];
	
	private static GUIStyle[] itsStyleTable = new GUIStyle[2];
	private static GUIStyle[] itsStyleTableHeadingRow = new GUIStyle[2];
	private static GUIStyle[] itsStyleTableHeadingCell = new GUIStyle[2];
	private static GUIStyle[] itsStyleTableRow = new GUIStyle[2];
	private static GUIStyle[] itsStyleTableRowCell = new GUIStyle[2];
	
	private static GUIStyle[] itsStyleCursor = new GUIStyle[2];
	#endregion
	
	#endregion
	
	#region editor colors
	
	/// <summary>
	/// predefined default color which can be used in editor classes
	/// </summary>
	public static Color itsEditorColorContent
	{
		get { return new Color(0.1f, 0.1f, 0.1f); }
	}
	
	/// <summary>
	/// predefined default color which can be used in editor classes
	/// </summary>
	public static Color itsEditorColorTitle
	{
		get { return new Color(0.1f, 0.1f, 0.1f); }
	}
	
	/// <summary>
	/// predefined default color which can be used in editor classes
	/// </summary>
	public static Color itsEditorDocumentation
	{
		get { return new Color(0.74f, 0.79f, 0.64f); }
	}
	
	/// <summary>
	/// predefined default color which can be used in editor classes
	/// </summary>
	public static Color itsEditorColorDefault
	{
		get { return new Color(1.0f, 1.0f, 1.0f); }
		//get { return new Color(0.84f, 0.89f, 0.74f); }
	}
	
	/// <summary>
	/// predefined default color which should be used in editor classes to display information, or notes
	/// </summary>
	public static Color itsEditorColorInfo
	{
		get { return new Color(1.0f,1.0f,1.0f); }
	}
	
	/// <summary>
	/// predefined default color which should be used in editor classes to display warnings
	/// </summary>
	public static Color itsEditorColorWarning
	{
		get { return new Color(1f, 1f, 0.0f); }
	}
	
	/// <summary>
	/// predefined default color which should be used in editor classes to display errors
	/// </summary>
	public static Color itsEditorColorError
	{
		get { return new Color(0.9f, 0.5f, 0.5f); }
	}
	
	#endregion
	
	#region methods
	
	#region getter methods
	
	public static int GetSkinIndex()
	{
		return itsSkinIndex;
	}
	
	/// <summary>
	/// returns the height of the button control which is the current skin heigt. -> can be 16, 32, 64
	/// </summary>
	/// <returns>returns the current height of the selected skin.</returns>
	public static float GetSkinHeight()
	{
		if(itsSkinIndex == -1)
		{
			return 16.0f;
		}
		if(itsStyleButton != null)
		{
			if (itsSkinIndex < itsStyleButton.Length)
			{
				if (itsStyleButton[itsSkinIndex] != null)
					return itsStyleButton[itsSkinIndex].fixedHeight;
			}
		}
		return 16.0f;
	}
	
	/// <summary>
	/// returns the current selected skin.
	/// </summary>
	/// <returns>returns the current selected skin. Returns null if no skin is selected.</returns>
	public static GUISkin GetSkin()
	{
		if(itsSkin[itsSkinIndex] != null)
		{
			return itsSkin[itsSkinIndex];
		}
		
		return null;
	}
	
	public static Texture2D GetLogo()
	{
		if(itsIcon == null)
		{
			itsIcon = Resources.Load("KGFCore/textures/logo") as Texture2D;
		}
		
		return itsIcon;
	}
	
	public static Texture2D GetHelpIcon()
	{
		if(itsIconHelp == null)
		{
			itsIconHelp = Resources.Load("KGFCore/textures/help") as Texture2D;
		}
		
		return itsIconHelp;
	}
	
	public static Texture2D GetKGFCopyright()
	{
		if(itsKGFCopyright == null)
		{
			itsKGFCopyright = Resources.Load("KGFCore/textures/kgf_copyright_512x256") as Texture2D;
		}
		
		return itsKGFCopyright;
	}
	
	/// <summary>
	/// returns the guistyle for the requested toggle type
	/// </summary>
	/// <param name="theTogglStyle">the type of requested toggle style</param>
	/// <returns>returns the requested toggle style. Returns the default toggle style if no custom style was found.</returns>
	public static GUIStyle GetStyleToggl(eStyleToggl theTogglStyle)
	{
		if(itsSkinIndex == -1)
			return GUI.skin.toggle;
		Init();
		if(theTogglStyle == eStyleToggl.eTogglStreched && itsStyleToggleStreched[itsSkinIndex] != null)
		{
			return itsStyleToggleStreched[itsSkinIndex];
		}
		else if(theTogglStyle == eStyleToggl.eTogglCompact && itsStyleToggleCompact[itsSkinIndex] != null)
		{
			return itsStyleToggleCompact[itsSkinIndex];
		}
		else if(theTogglStyle == eStyleToggl.eTogglSuperCompact && itsStyleToggleSuperCompact[itsSkinIndex] != null)
		{
			return itsStyleToggleSuperCompact[itsSkinIndex];
		}
		else if(theTogglStyle == eStyleToggl.eTogglRadioStreched && itsStyleToggleRadioStreched[itsSkinIndex] != null)
		{
			return itsStyleToggleRadioStreched[itsSkinIndex];
		}
		else if(theTogglStyle == eStyleToggl.eTogglRadioCompact && itsStyleToggleRadioCompact[itsSkinIndex] != null)
		{
			return itsStyleToggleRadioCompact[itsSkinIndex];
		}
		else if(theTogglStyle == eStyleToggl.eTogglRadioSuperCompact && itsStyleToggleRadioSuperCompact[itsSkinIndex] != null)
		{
			return itsStyleToggleRadioSuperCompact[itsSkinIndex];
		}
		else if(theTogglStyle == eStyleToggl.eTogglSwitch && itsStyleToggleSwitch[itsSkinIndex] != null)
		{
			return itsStyleToggleSwitch[itsSkinIndex];
		}
		else if(theTogglStyle == eStyleToggl.eTogglBoolean && itsStyleToggleBoolean[itsSkinIndex] != null)
		{
			return itsStyleToggleBoolean[itsSkinIndex];
		}
		else if(theTogglStyle == eStyleToggl.eTogglArrow && itsStyleToggleArrow[itsSkinIndex] != null)
		{
			return itsStyleToggleArrow[itsSkinIndex];
		}
		else if(theTogglStyle == eStyleToggl.eTogglButton && itsStyleToggleButton[itsSkinIndex] != null)
		{
			return itsStyleToggleButton[itsSkinIndex];
		}
		
		if(itsStyleToggle[itsSkinIndex] != null)
		{
			return itsStyleToggle[itsSkinIndex];
		}
		else
		{
			return GUI.skin.toggle;
		}
	}
	
	/// <summary>
	/// returns the guistyle for a textfield
	/// </summary>
	/// <param name="theStyleTextField">the type of textfield style</param>
	/// <returns>returns the requested textfield style. Returns the default textfield style if no custom style was found.</returns>
	public static GUIStyle GetStyleTextField(eStyleTextField theStyleTextField)
	{
		if(itsSkinIndex == -1)
			return GUI.skin.textField;
		Init();
		if(theStyleTextField == eStyleTextField.eTextField && itsStyleTextField[itsSkinIndex] != null)
		{
			return itsStyleTextField[itsSkinIndex];
		}
		else if(theStyleTextField == eStyleTextField.eTextFieldLeft && itsStyleTextFieldLeft[itsSkinIndex] != null)
		{
			return itsStyleTextFieldLeft[itsSkinIndex];
		}
		else if(theStyleTextField == eStyleTextField.eTextFieldRight && itsStyleTextFieldRight[itsSkinIndex] != null)
		{
			return itsStyleTextFieldRight[itsSkinIndex];
		}
		
		return GUI.skin.textField;
	}
	
	/// <summary>
	/// returns the guistyle for a textarea
	/// </summary>
	/// <returns>returns the textarea style. Returns the default textarea style if no custom style was found.</returns>
	public static GUIStyle GetStyleTextArea()
	{
		if(itsSkinIndex == -1)
			return GUI.skin.textArea;
		Init();
		if(itsStyleTextArea != null)
		{
			return itsStyleTextArea[itsSkinIndex];
		}
		
		return GUI.skin.textArea;
	}
	
	#region horizontal slider
	
	/// <summary>
	/// returns the guistyle for a horizontal slider
	/// </summary>
	/// <returns>returns the horizontal slider style. Returns the default horizontal slider style if no custom style was found.</returns>
	public static GUIStyle GetStyleHorizontalSlider()
	{
		if(itsSkinIndex == -1)
			return GUI.skin.horizontalSlider;
		Init();
		if(itsStyleHorizontalSlider[itsSkinIndex] != null)
		{
			return itsStyleHorizontalSlider[itsSkinIndex];
		}
		
		return GUI.skin.horizontalSlider;
	}
	
	/// <summary>
	/// returns the guistyle for a horizontalt slider thumb
	/// </summary>
	/// <returns>returns the horizontal slider thumb style. Returns the default horizontal slider thumb style if no custom style was found.</returns>
	public static GUIStyle GetStyleHorizontalSliderThumb()
	{
		if(itsSkinIndex == -1)
			return GUI.skin.horizontalSliderThumb;
		Init();
		if(itsStyleHorizontalSliderThumb[itsSkinIndex] != null)
		{
			return itsStyleHorizontalSliderThumb[itsSkinIndex];
		}
		
		return GUI.skin.horizontalSliderThumb;
	}
	
	#endregion
	
	#region horizontal scrollbar
	
	/// <summary>
	/// returns the guistyle for a horizontal scrollbar
	/// </summary>
	/// <returns>returns the horizontal scrollbar style. Returns the default horizontal scrollbar style if no custom style was found.</returns>
	public static GUIStyle GetStyleHorizontalScrollbar()
	{
		if(itsSkinIndex == -1)
			return GUI.skin.horizontalScrollbar;
		Init();
		if(itsStyleHorizontalScrollbar[itsSkinIndex] != null)
		{
			return itsStyleHorizontalScrollbar[itsSkinIndex];
		}
		
		return GUI.skin.horizontalScrollbar;
	}
	
	/// <summary>
	/// returns the guistyle for a horizontal scrollbar thumb
	/// </summary>
	/// <returns>returns the horizontal scrollbar thumb style. Returns the default horizontal scrollbar thumb style if no custom style was found.</returns>
	public static GUIStyle GetStyleHorizontalScrollbarThumb()
	{
		if(itsSkinIndex == -1)
			return GUI.skin.horizontalScrollbarThumb;
		Init();
		if(itsStyleHorizontalScrollbarThumb[itsSkinIndex] != null)
		{
			return itsStyleHorizontalScrollbarThumb[itsSkinIndex];
		}
		
		return GUI.skin.horizontalScrollbarThumb;
	}
	
	/// <summary>
	/// returns the guistyle for the left button of a horizontal scrollbar
	/// </summary>
	/// <returns>returns the horizontal scrollbar left button style. Returns the default horizontal scrollbar left button style if no custom style was found.</returns>
	public static GUIStyle GetStyleHorizontalScrollbarLeftButton()
	{
		if(itsSkinIndex == -1)
			return GUI.skin.horizontalScrollbarLeftButton;
		Init();
		if(itsStyleHorizontalScrollbarLeftButton[itsSkinIndex] != null)
		{
			return itsStyleHorizontalScrollbarLeftButton[itsSkinIndex];
		}
		
		return GUI.skin.horizontalScrollbarLeftButton;
	}
	
	/// <summary>
	/// returns the guistyle for the right button of a horizontal scrollbar
	/// </summary>
	/// <returns>returns the horizontal scrollbar right button style. Returns the default horizontal scrollbar right button style if no custom style was found.</returns>
	public static GUIStyle GetStyleHorizontalScrollbarRightButton()
	{
		if(itsSkinIndex == -1)
			return GUI.skin.horizontalScrollbarRightButton;
		Init();
		if(itsStyleHorizontalScrollbarRightButton[itsSkinIndex] != null)
		{
			return itsStyleHorizontalScrollbarRightButton[itsSkinIndex];
		}
		
		return GUI.skin.horizontalScrollbarRightButton;
	}
	
	#endregion
	
	#region vertical slider
	
	/// <summary>
	/// returns the guistyle for a vertical slider
	/// </summary>
	/// <returns>returns the vertical slider style. Returns the default vertical slider style if no custom style was found.</returns>
	public static GUIStyle GetStyleVerticalSlider()
	{
		if(itsSkinIndex == -1)
			return GUI.skin.verticalSlider;
		Init();
		if(itsStyleVerticalSlider[itsSkinIndex] != null)
		{
			return itsStyleVerticalSlider[itsSkinIndex];
		}
		
		return GUI.skin.verticalSlider;
	}
	
	/// <summary>
	/// returns the guistyle for a horizontalt slider thumb
	/// </summary>
	/// <returns>returns the vertical slider thumb style. Returns the default vertical slider thumb style if no custom style was found.</returns>
	public static GUIStyle GetStyleVerticalSliderThumb()
	{
		if(itsSkinIndex == -1)
			return GUI.skin.verticalSliderThumb;
		Init();
		if(itsStyleVerticalSliderThumb[itsSkinIndex] != null)
		{
			return itsStyleVerticalSliderThumb[itsSkinIndex];
		}
		
		return GUI.skin.verticalSliderThumb;
	}
	
	#endregion
	
	#region vertical scrollbar
	
	/// <summary>
	/// returns the guistyle for a vertival scrollbar
	/// </summary>
	/// <returns>returns the vertival scrollbar style. Returns the default vertival scrollbar style if no custom style was found.</returns>
	public static GUIStyle GetStyleVerticalScrollbar()
	{
		if(itsSkinIndex == -1)
			return GUI.skin.verticalScrollbar;
		Init();
		if(itsStyleVerticalScrollbar[itsSkinIndex] != null)
		{
			return itsStyleVerticalScrollbar[itsSkinIndex];
		}
		
		return GUI.skin.verticalScrollbar;
	}
	
	/// <summary>
	/// returns the guistyle for a vertical scrollbar thumb
	/// </summary>
	/// <returns>returns the vertical scrollbar thumb style. Returns the default vertical scrollbar thumb style if no custom style was found.</returns>
	public static GUIStyle GetStyleVerticalScrollbarThumb()
	{
		if(itsSkinIndex == -1)
			return GUI.skin.verticalScrollbarThumb;
		Init();
		if(itsStyleVerticalScrollbarThumb[itsSkinIndex] != null)
		{
			return itsStyleVerticalScrollbarThumb[itsSkinIndex];
		}
		
		return GUI.skin.verticalScrollbarThumb;
	}
	
	/// <summary>
	/// returns the guistyle for the up button of a vertical scrollbar
	/// </summary>
	/// <returns>returns the vertical scrollbar up button style. Returns the default vertical scrollbar up button style if no custom style was found.</returns>
	public static GUIStyle GetStyleVerticalScrollbarUpButton()
	{
		if(itsSkinIndex == -1)
			return GUI.skin.verticalScrollbarUpButton;
		Init();
		if(itsStyleVerticalScrollbarUpButton[itsSkinIndex] != null)
		{
			return itsStyleVerticalScrollbarUpButton[itsSkinIndex];
		}
		
		return GUI.skin.verticalScrollbarUpButton;
	}
	
	/// <summary>
	/// returns the guistyle for the down button of a vertical scrollbar
	/// </summary>
	/// <returns>returns the vertical scrollbar down button style. Returns the default vertical scrollbar down button style if no custom style was found.</returns>
	public static GUIStyle GetStyleVerticalScrollbarDownButton()
	{
		if(itsSkinIndex == -1)
			return GUI.skin.verticalScrollbarDownButton;
		Init();
		if(itsStyleVerticalScrollbarDownButton[itsSkinIndex] != null)
		{
			return itsStyleVerticalScrollbarDownButton[itsSkinIndex];
		}
		
		return GUI.skin.verticalScrollbarDownButton;
	}
	
	#endregion
	
	#region minimap styles
	
	/// <summary>
	/// returns the guistyle for a scrollview
	/// </summary>
	/// <returns>returns the scrollview style. Returns the default scrollview style if no custom style was found.</returns>
	public static GUIStyle GetStyleScrollView()
	{
		if(itsSkinIndex == -1)
			return GUI.skin.scrollView;
		Init();
		if(itsStyleScrollView[itsSkinIndex] != null)
		{
			return itsStyleScrollView[itsSkinIndex];
		}
		
		return GUI.skin.scrollView;
	}
	
	/// <summary>
	/// returns the guistyle for a minimap border
	/// </summary>
	/// <returns>returns the minimap border style. Returns the default box style if no custom style was found.</returns>
	public static GUIStyle GetStyleMinimapBorder()
	{
		if(itsSkinIndex == -1)
			return GUI.skin.box;
		Init();
		if(itsStyleMinimap[itsSkinIndex] != null)
		{
			return itsStyleMinimap[itsSkinIndex];
		}
		
		return GUI.skin.box;
	}
	
	/// <summary>
	/// returns the guistyle for a minimap buttons
	/// </summary>
	/// <returns>returns the minimap button style. Returns the default button style if no custom style was found.</returns>
	public static GUIStyle GetStyleMinimapButton()
	{
		if(itsSkinIndex == -1)
			return GUI.skin.box;
		Init();
		if(itsStyleMinimapButton[itsSkinIndex] != null)
		{
			return itsStyleMinimapButton[itsSkinIndex];
		}
		
		return GUI.skin.button;
	}
	
	#endregion
	
	/// <summary>
	/// returns the requested guistyle for a button
	/// </summary>
	/// <param name="theStyleButton">the type of button style</param>
	/// <returns>returns the requested button style. Returns the default button style if no custom style was found.</returns>
	public static GUIStyle GetStyleButton(eStyleButton theStyleButton)
	{
		if(itsSkinIndex == -1)
			return GUI.skin.button;
		Init();
		if(theStyleButton == eStyleButton.eButton && itsStyleButton[itsSkinIndex] != null)
		{
			return itsStyleButton[itsSkinIndex];
		}
		else if(theStyleButton == eStyleButton.eButtonLeft && itsStyleButtonLeft[itsSkinIndex] != null)
		{
			return itsStyleButtonLeft[itsSkinIndex];
		}
		else if(theStyleButton == eStyleButton.eButtonRight && itsStyleButtonRight[itsSkinIndex] != null)
		{
			return itsStyleButtonRight[itsSkinIndex];
		}
		else if(theStyleButton == eStyleButton.eButtonTop && itsStyleButtonTop[itsSkinIndex] != null)
		{
			return itsStyleButtonTop[itsSkinIndex];
		}
		else if(theStyleButton == eStyleButton.eButtonBottom && itsStyleButtonBottom[itsSkinIndex] != null)
		{
			return itsStyleButtonBottom[itsSkinIndex];
		}
		else if(theStyleButton == eStyleButton.eButtonMiddle && itsStyleButtonMiddle[itsSkinIndex] != null)
		{
			return itsStyleButtonMiddle[itsSkinIndex];
		}
		
		return GUI.skin.button;
	}
	
	/// <summary>
	/// returns the requested guistyle for a box
	/// </summary>
	/// <param name="theStyleBox">the type of box style</param>
	/// <returns>returns the requested box style. Returns the default box style if no custom style was found.</returns>
	public static GUIStyle GetStyleBox(eStyleBox theStyleBox)
	{
		if(itsSkinIndex == -1)
			return GUI.skin.box;
		Init();
		if(theStyleBox == eStyleBox.eBox && itsStyleBox[itsSkinIndex] != null)
		{
			return itsStyleBox[itsSkinIndex];
		}
		else if(theStyleBox == eStyleBox.eBoxInvisible&& itsStyleBoxInvisible[itsSkinIndex] != null)
		{
			return itsStyleBoxInvisible[itsSkinIndex];
		}
		else if(theStyleBox == eStyleBox.eBoxInteractive && itsStyleBox[itsSkinIndex] != null)
		{
			return itsStyleBoxInteractive[itsSkinIndex];
		}
		else if(theStyleBox == eStyleBox.eBoxLeft && itsStyleBoxLeft[itsSkinIndex] != null)
		{
			return itsStyleBoxLeft[itsSkinIndex];
		}
		else if(theStyleBox == eStyleBox.eBoxLeftInteractive && itsStyleBoxLeft[itsSkinIndex] != null)
		{
			return itsStyleBoxLeftInteractive[itsSkinIndex];
		}
		else if(theStyleBox == eStyleBox.eBoxRight && itsStyleBoxRight[itsSkinIndex] != null)
		{
			return itsStyleBoxRight[itsSkinIndex];
		}
		else if(theStyleBox == eStyleBox.eBoxRightInteractive && itsStyleBoxRight[itsSkinIndex] != null)
		{
			return itsStyleBoxRightInteractive[itsSkinIndex];
		}
		else if(theStyleBox == eStyleBox.eBoxMiddleHorizontal && itsStyleBoxMiddleHorizontal[itsSkinIndex] != null)
		{
			return itsStyleBoxMiddleHorizontal[itsSkinIndex];
		}
		else if(theStyleBox == eStyleBox.eBoxMiddleHorizontalInteractive && itsStyleBoxMiddleHorizontal[itsSkinIndex] != null)
		{
			return itsStyleBoxMiddleHorizontalInteractive[itsSkinIndex];
		}
		else if(theStyleBox == eStyleBox.eBoxTop && itsStyleBoxTop[itsSkinIndex] != null)
		{
			return itsStyleBoxTop[itsSkinIndex];
		}
		else if(theStyleBox == eStyleBox.eBoxTopInteractive && itsStyleBoxTop[itsSkinIndex] != null)
		{
			return itsStyleBoxTopInteractive[itsSkinIndex];
		}
		else if(theStyleBox == eStyleBox.eBoxBottom && itsStyleBoxBottom[itsSkinIndex] != null)
		{
			return itsStyleBoxBottom[itsSkinIndex];
		}
		else if(theStyleBox == eStyleBox.eBoxBottomInteractive && itsStyleBoxBottom[itsSkinIndex] != null)
		{
			return itsStyleBoxBottomInteractive[itsSkinIndex];
		}
		else if(theStyleBox == eStyleBox.eBoxMiddleVertical && itsStyleBoxMiddleVertical[itsSkinIndex] != null)
		{
			return itsStyleBoxMiddleVertical[itsSkinIndex];
		}
		else if(theStyleBox == eStyleBox.eBoxMiddleVerticalInteractive && itsStyleBoxMiddleVertical[itsSkinIndex] != null)
		{
			return itsStyleBoxMiddleVerticalInteractive[itsSkinIndex];
		}
		else if(theStyleBox == eStyleBox.eBoxDark && itsStyleBoxDark[itsSkinIndex] != null)
		{
			return itsStyleBoxDark[itsSkinIndex];
		}
		else if(theStyleBox == eStyleBox.eBoxDarkInteractive && itsStyleBoxDark[itsSkinIndex] != null)
		{
			return itsStyleBoxDarkInteractive[itsSkinIndex];
		}
		else if(theStyleBox == eStyleBox.eBoxDarkLeft && itsStyleBoxDarkLeft[itsSkinIndex] != null)
		{
			return itsStyleBoxDarkLeft[itsSkinIndex];
		}
		else if(theStyleBox == eStyleBox.eBoxDarkLeftInteractive && itsStyleBoxDarkLeft[itsSkinIndex] != null)
		{
			return itsStyleBoxDarkLeftInteractive[itsSkinIndex];
		}
		else if(theStyleBox == eStyleBox.eBoxDarkRight && itsStyleBoxDarkRight[itsSkinIndex] != null)
		{
			return itsStyleBoxDarkRight[itsSkinIndex];
		}
		else if(theStyleBox == eStyleBox.eBoxDarkRightInteractive && itsStyleBoxDarkRight[itsSkinIndex] != null)
		{
			return itsStyleBoxDarkRightInteractive[itsSkinIndex];
		}
		else if(theStyleBox == eStyleBox.eBoxDarkMiddleHorizontal && itsStyleBoxDarkMiddleHorizontal[itsSkinIndex] != null)
		{
			return itsStyleBoxDarkMiddleHorizontal[itsSkinIndex];
		}
		else if(theStyleBox == eStyleBox.eBoxDarkMiddleHorizontalInteractive && itsStyleBoxDarkMiddleHorizontal[itsSkinIndex] != null)
		{
			return itsStyleBoxDarkMiddleHorizontalInteractive[itsSkinIndex];
		}
		else if(theStyleBox == eStyleBox.eBoxDarkTop && itsStyleBoxDarkTop[itsSkinIndex] != null)
		{
			return itsStyleBoxDarkTop[itsSkinIndex];
		}
		else if(theStyleBox == eStyleBox.eBoxDarkTopInteractive && itsStyleBoxDarkTop[itsSkinIndex] != null)
		{
			return itsStyleBoxDarkTopInteractive[itsSkinIndex];
		}
		else if(theStyleBox == eStyleBox.eBoxDarkBottom && itsStyleBoxDarkBottom[itsSkinIndex] != null)
		{
			return itsStyleBoxDarkBottom[itsSkinIndex];
		}
		else if(theStyleBox == eStyleBox.eBoxDarkBottomInteractive && itsStyleBoxDarkBottom[itsSkinIndex] != null)
		{
			return itsStyleBoxDarkBottomInteractive[itsSkinIndex];
		}
		else if(theStyleBox == eStyleBox.eBoxDarkMiddleVertical && itsStyleBoxDarkMiddleVertical[itsSkinIndex] != null)
		{
			return itsStyleBoxDarkMiddleVertical[itsSkinIndex];
		}
		else if(theStyleBox == eStyleBox.eBoxDarkMiddleVerticalInteractive && itsStyleBoxDarkMiddleVertical[itsSkinIndex] != null)
		{
			return itsStyleBoxDarkMiddleVerticalInteractive[itsSkinIndex];
		}
		else if(theStyleBox == eStyleBox.eBoxDecorated && itsStyleBoxDecorated[itsSkinIndex] != null)
		{
			return itsStyleBoxDecorated[itsSkinIndex];
		}
		
		return GUI.skin.box;
	}
	
	/// <summary>
	/// returns the requested guistyle for a seperator
	/// </summary>
	/// <param name="theStyleSeparator">the type of the seperator</param>
	/// <returns>returns the requested seperator style. Returns the default label style if no custom style was found.</returns>
	public static GUIStyle GetStyleSeparator(eStyleSeparator theStyleSeparator)
	{
		if(itsSkinIndex == -1)
			return GUI.skin.box;
		Init();
		if(theStyleSeparator == eStyleSeparator.eSeparatorHorizontal && itsStyleSeparatorHorizontal[itsSkinIndex] != null)
		{
			return itsStyleSeparatorHorizontal[itsSkinIndex];
		}
		else if(theStyleSeparator == eStyleSeparator.eSeparatorVertical && itsStyleSeparatorVertical[itsSkinIndex] != null)
		{
			return itsStyleSeparatorVertical[itsSkinIndex];
		}
		else if(theStyleSeparator == eStyleSeparator.eSeparatorVerticalFitInBox && itsStyleSeparatorVerticalFitInBox[itsSkinIndex] != null)
		{
			return itsStyleSeparatorVerticalFitInBox[itsSkinIndex];
		}
		
		return GUI.skin.label;
	}
	
	/// <summary>
	/// returns the requested guistyle for a label
	/// </summary>
	/// <param name="theStyleSeparator">the type of the label</param>
	/// <returns>returns the requested label style. Returns the default label style if no custom style was found.</returns>
	public static GUIStyle GetStyleLabel (eStyleLabel theStyleLabel)
	{
		if(itsSkinIndex == -1)
			return GUI.skin.label;
		Init();
		if (theStyleLabel == eStyleLabel.eLabel && itsStyleLabel[itsSkinIndex] != null)
		{
			return itsStyleLabel[itsSkinIndex];
		}
		if (theStyleLabel == eStyleLabel.eLabelFitIntoBox && itsStyleLabelFitInToBox[itsSkinIndex] != null)
		{
			return itsStyleLabelFitInToBox[itsSkinIndex];
		}
		if (theStyleLabel == eStyleLabel.eLabelMultiline && itsStyleLabelMultiline[itsSkinIndex] != null)
		{
			return itsStyleLabelMultiline[itsSkinIndex];
		}
		if (theStyleLabel == eStyleLabel.eLabelTitle && itsStyleLabelTitle[itsSkinIndex] != null)
		{
			return itsStyleLabelTitle[itsSkinIndex];
		}
		
		return GUI.skin.box;
	}
	
	/// <summary>
	/// returns the requested guistyle for a window
	/// </summary>
	/// <param name="theStyleSeparator">the type of the window</param>
	/// <returns>returns the requested window style. Returns the default window style if no custom style was found.</returns>
	public static GUIStyle GetStyleWindow()
	{
		if(itsSkinIndex == -1)
			return GUI.skin.window;
		Init();
		if(itsStyleWindow[itsSkinIndex] != null)
		{
			return itsStyleWindow[itsSkinIndex];
		}
		
		return GUI.skin.window;
	}
	
	/// <summary>
	/// returns the requested guistyle for the cursor style
	/// </summary>
	/// <returns>returns the requested windowTitle style. Returns the label if no custom style was found.</returns>
	public static GUIStyle GetStyleCursor()
	{
		if(itsSkinIndex == -1)
			return GUI.skin.box;
		Init();
		if(itsStyleCursor[itsSkinIndex] != null)
		{
			return itsStyleCursor[itsSkinIndex];
		}
		
		return itsStyleCursor[itsSkinIndex];
	}
	
	#region table styles
	
	/// <summary>
	/// returns the requested guistyle for the custom table style
	/// </summary>
	/// <returns>returns the requested custom table style. Returns default box style if no custom style was found.</returns>
	public static GUIStyle GetTableStyle()
	{
		if(itsSkinIndex == -1)
			return GUI.skin.box;
		Init();
		if(itsStyleTable[itsSkinIndex] != null)
		{
			return itsStyleTable[itsSkinIndex];
		}
		else
		{
			return GUI.skin.box;
		}
	}
	
	/// <summary>
	/// returns the requested guistyle for the custom table_heading_row style
	/// </summary>
	/// <returns>returns the requested custom table row style. Returns default box style if no custom style was found.</returns>
	public static GUIStyle GetTableHeadingRowStyle()
	{
		if(itsSkinIndex == -1)
			return GUI.skin.box;
		Init();
		if(itsStyleTableHeadingRow[itsSkinIndex] != null)
		{
			return itsStyleTableHeadingRow[itsSkinIndex];
		}
		else
		{
			return GUI.skin.box;
		}
	}
	
	/// <summary>
	/// returns the requested guistyle for the custom table_heading_cell style
	/// </summary>
	/// <returns>returns the requested custom table cell style. Returns default box style if no custom style was found.</returns>
	public static GUIStyle GetTableHeadingCellStyle()
	{
		if(itsSkinIndex == -1)
			return GUI.skin.box;
		Init();
		if(itsStyleTableHeadingCell[itsSkinIndex] != null)
		{
			return itsStyleTableHeadingCell[itsSkinIndex];
		}
		else
		{
			return GUI.skin.box;
		}
	}
	
	/// <summary>
	/// returns the requested guistyle for the custom table_row style
	/// </summary>
	/// <returns>returns the requested custom table row style. Returns default box style if no custom style was found.</returns>
	public static GUIStyle GetTableRowStyle()
	{
		if(itsSkinIndex == -1)
			return GUI.skin.box;
		Init();
		if(itsStyleTableRow[itsSkinIndex]!= null)
		{
			return itsStyleTableRow[itsSkinIndex];
		}
		else
		{
			return GUI.skin.box;
		}
	}
	
	/// <summary>
	/// returns the requested guistyle for the custom table_cell style
	/// </summary>
	/// <returns>returns the requested custom table cell style. Returns default box style if no custom style was found.</returns>
	public static GUIStyle GetTableCellStyle()
	{
		if(itsSkinIndex == -1)
			return GUI.skin.box;
		Init();
		if(itsStyleTableRowCell[itsSkinIndex] != null)
		{
			return itsStyleTableRowCell[itsSkinIndex];
		}
		else
		{
			return GUI.skin.box;
		}
	}
	
	#endregion
	
	#endregion
	
	#region sounds
	/// <summary>
	/// Set volume for sounds
	/// </summary>
	/// <param name="theVolume"></param>
	public static void SetVolume(float theVolume)
	{
		itsVolume = theVolume;
	}
	
	/// <summary>
	/// Set click sound for buttons
	/// </summary>
	/// <param name="theButtonStyle"></param>
	/// <param name="theAudioClip"></param>
	public static void SetSoundForButton(eStyleButton theButtonStyle, AudioClip theAudioClip)
	{
		SetSound(theButtonStyle.ToString(),theAudioClip);
	}
	
	/// <summary>
	/// Set click sound for toggles
	/// </summary>
	/// <param name="theTogglStyle"></param>
	/// <param name="theAudioClip"></param>
	public static void SetSoundForToggle(eStyleToggl theTogglStyle, AudioClip theAudioClip)
	{
		SetSound(theTogglStyle.ToString(),theAudioClip);
	}
	
	/// <summary>
	/// Set sound for gui style
	/// </summary>
	/// <param name="theStyle"></param>
	/// <param name="theAudioClip"></param>
	static void SetSound(string theStyle, AudioClip theAudioClip)
	{
		if (theAudioClip != null && itsAudioClips.ContainsKey(theStyle))
			itsAudioClips.Remove(theStyle);
		else
		{
			itsAudioClips[theStyle] = theAudioClip;
		}
	}
	
	/// <summary>
	/// Play sound for the style id, if there is one set
	/// </summary>
	/// <param name="theStyle"></param>
	static void PlaySound(string theStyle)
	{
		if (Application.isPlaying)
		{
			if (itsAudioClips.ContainsKey(theStyle))
			{
				AudioSource.PlayClipAtPoint(itsAudioClips[theStyle],Vector3.zero,itsVolume);
			}
		}
	}
	#endregion
	
	
	/// <summary>
	/// Enables or disables the kgfskins in editor mode.
	/// </summary>
	/// <param name="theSetEnableKGFSkins"></param>
	public static void SetEnableKGFSkinsInEdior(bool theSetEnableKGFSkins)
	{
		KGFGUIUtility.itsEnableKGFSkins = theSetEnableKGFSkins;
	}
	
	/// <summary>
	/// changes the index of the skin
	/// </summary>
	/// <param name="theIndex"></param>
	public static void SetSkinIndex(int theIndex)
	{
		itsSkinIndex = theIndex;
		if(itsSkinIndex == 0 && !itsEnableKGFSkins)
		{
			itsSkinIndex = -1;
		}
	}
	
	/// <summary>
	/// resets the path of the skin
	/// </summary>
	/// <param name="thePath">the new filepath of the guiskin.</param>
	public static void SetSkinPath(string thePath)
	{
		itsDefaultGuiSkinPath[1] = thePath;
		itsResetPath[1] = true;
	}
	
	/// <summary>
	/// resets the path of the skin
	/// </summary>
	/// <param name="thePath">the new filepath of the guiskin.</param>
	public static void SetSkinPathEditor(string thePath)
	{
		itsDefaultGuiSkinPath[0] = thePath;
		itsResetPath[0] = true;
	}
	
	/// <summary>
	/// resets the path of the skin
	/// </summary>
	/// <param name="thePath">the new filepath of the guiskin.</param>
	public static string GetSkinPath()
	{
		return itsDefaultGuiSkinPath[itsSkinIndex];
	}
	
	/// <summary>
	/// reloads the selected skin elements
	/// </summary>
	private static void Init()
	{
		Init(false);
	}
	
	/// <summary>
	/// This method has to be called before using this class
	/// </summary>
	private static void Init(bool theForceInit)
	{
		if(itsSkinIndex == -1)
			return;
		
		if(itsSkin[itsSkinIndex] != null && !theForceInit && !itsResetPath[itsSkinIndex])
		{
			return;
		}
		
		itsResetPath[itsSkinIndex] = false;
		
		Debug.Log("Loading skin: "+itsDefaultGuiSkinPath[itsSkinIndex]);
		itsSkin[itsSkinIndex] = Resources.Load(itsDefaultGuiSkinPath[itsSkinIndex]) as GUISkin;
		
		if(itsSkin[itsSkinIndex] == null)
		{
			Debug.Log("Kolmich Game Framework default skin wasn`t found");
			itsSkin[itsSkinIndex] = GUI.skin;
			return;
		}
		
		GUI.skin = itsSkin[itsSkinIndex];
		
		//cache the styles
		itsStyleToggle[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("toggle");
		itsStyleTextField[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("textfield");
		itsStyleTextFieldLeft[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("textfield_left");
		itsStyleTextFieldRight[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("textfield_right");
		itsStyleTextArea[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("textarea");
		itsStyleWindow[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("window");
		
		itsStyleHorizontalSlider[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("horizontalslider");
		itsStyleHorizontalSliderThumb[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("horizontalsliderthumb");
		
		itsStyleVerticalSlider[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("verticalslider");
		itsStyleVerticalSliderThumb[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("verticalsliderthumb");
		
		itsStyleHorizontalScrollbar[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("horizontalscrollbar");
		itsStyleHorizontalScrollbarThumb[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("horizontalscrollbarthumb");
		itsStyleHorizontalScrollbarLeftButton[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("horizontalscrollbarleftbutton");
		itsStyleHorizontalScrollbarRightButton[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("horizontalscrollbarrightbutton");
		
		itsStyleVerticalScrollbar[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("verticalscrollbar");
		itsStyleVerticalScrollbarThumb[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("verticalscrollbarthumb");
		itsStyleVerticalScrollbarUpButton[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("verticalscrollbarupbutton");
		itsStyleVerticalScrollbarDownButton[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("verticalscrollbardownbutton");
		
		itsStyleScrollView[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("scrollview");
		itsStyleMinimap[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("minimap");
		itsStyleMinimapButton[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("minimap_button");
		
		itsStyleToggleStreched[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("toggle_stretched");
		itsStyleToggleCompact[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("toggle_compact");
		itsStyleToggleSuperCompact[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("toggle_supercompact");
		
		itsStyleToggleRadioStreched[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("toggle_radio_stretched");
		itsStyleToggleRadioCompact[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("toggle_radio_compact");
		itsStyleToggleRadioSuperCompact[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("toggle_radio_supercompact");
		
		itsStyleToggleSwitch[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("toggle_switch");
		itsStyleToggleBoolean[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("toggle_boolean");
		itsStyleToggleArrow[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("toggle_arrow");
		itsStyleToggleButton[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("toggle_button");
		
		itsStyleButton[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("Button");
		itsStyleButtonLeft[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("button_left");
		itsStyleButtonRight[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("button_right");
		itsStyleButtonTop[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("button_top");
		itsStyleButtonBottom[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("button_bottom");
		itsStyleButtonMiddle[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("button_middle");
		
		itsStyleBox[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("Box");
		itsStyleBoxInvisible[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("box_invisible");
		itsStyleBoxInteractive[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("box_interactive");
		itsStyleBoxLeft[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("box_left");
		itsStyleBoxLeftInteractive[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("box_left_interactive");
		itsStyleBoxRight[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("box_right");
		itsStyleBoxRightInteractive[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("box_right_interactive");
		itsStyleBoxMiddleHorizontal[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("box_middle_horizontal");
		itsStyleBoxMiddleHorizontalInteractive[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("box_middle_horizontal_interactive");
		itsStyleBoxTop[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("box_top");
		itsStyleBoxTopInteractive[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("box_top_interactive");
		itsStyleBoxBottom[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("box_bottom");
		itsStyleBoxBottomInteractive[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("box_bottom_interactive");
		itsStyleBoxMiddleVertical[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("box_middle_vertical");
		itsStyleBoxMiddleVerticalInteractive[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("box_middle_vertical_interactive");
		
		itsStyleBoxDark[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("box_dark");
		itsStyleBoxDarkInteractive[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("box_dark_interactive");
		itsStyleBoxDarkLeft[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("box_dark_left");
		itsStyleBoxDarkLeftInteractive[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("box_dark_left_interactive");
		itsStyleBoxDarkRight[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("box_dark_right");
		itsStyleBoxDarkRightInteractive[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("box_dark_right_interactive");
		itsStyleBoxDarkMiddleHorizontal[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("box_dark_middle_horizontal");
		itsStyleBoxDarkMiddleHorizontalInteractive[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("box_dark_middle_horizontal_interactive");
		itsStyleBoxDarkTop[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("box_dark_top");
		itsStyleBoxDarkTopInteractive[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("box_dark_top_interactive");
		itsStyleBoxDarkBottom[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("box_dark_bottom");
		itsStyleBoxDarkBottomInteractive[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("box_dark_bottom_interactive");
		itsStyleBoxDarkMiddleVertical[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("box_dark_middle_vertical");
		itsStyleBoxDarkMiddleVerticalInteractive[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("box_dark_middle_vertical_interactive");
		itsStyleBoxDecorated[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("box_decorated");
		
		itsStyleSeparatorVertical[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("separator_vertical");
		itsStyleSeparatorVerticalFitInBox[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("separator_vertical_fitinbox");
		itsStyleSeparatorHorizontal[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("separator_horizontal");
		
		itsStyleLabel[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("label");
		itsStyleLabelFitInToBox[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("label_fitintobox");
		itsStyleLabelMultiline[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("label_multiline");
		itsStyleLabelTitle[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("label_title");
		
		itsStyleCursor[itsSkinIndex] = itsSkin[itsSkinIndex].GetStyle("mouse_cursor");
	}
	
	#region gui draw methods
	
	/// <summary>
	/// Use this method to draw a window title with icon an text.
	/// </summary>
	/// <remarks>
	/// Use this method to draw a title into a window or box. If the icon parameter is null, the window will display the text only. To properly close the window header call EndWindowHeader().
	/// </remarks>
	/// <param name="theTitle">the window title</param>
	/// <param name="theIcon">the icon displayed before the window text</param>
	public static void BeginWindowHeader(string theTitle, Texture2D theIcon)
	{
		Init();

		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDark);
		
		KGFGUIUtility.Label("",theIcon,eStyleLabel.eLabel,GUILayout.Width(GetSkinHeight()));
		KGFGUIUtility.Label(theTitle,eStyleLabel.eLabel);
	}
	
	/// <summary>
	/// Use this method to end the a window title.
	/// </summary>
	/// <remarks>
	/// This method ends a window title. Before calling this method call BeginWindowHeader() to start the window header properly.
	/// </remarks>
	/// <param name="theCloseButton">true if the close button of the window should be displayed.</param>
	/// <returns>returns true if the close button was clicked</returns>
	public static bool EndWindowHeader(bool theCloseButton)
	{
		bool aClick = false;
		
		if(theCloseButton)
		{
			Init();
			if(itsSkinIndex == -1)
			{
				aClick = GUILayout.Button("x",GUILayout.Width(KGFGUIUtility.GetSkinHeight()));
			}
			else
			{
				aClick = KGFGUIUtility.Button("x", KGFGUIUtility.eStyleButton.eButton,GUILayout.Width(KGFGUIUtility.GetSkinHeight()));
			}
		}
		
		KGFGUIUtility.EndHorizontalBox();
		
		return aClick;
	}
	
	/// <summary>
	/// Use this method to draw the current open drop down box.
	/// </summary>
	/// <remarks>
	/// If using the KGFGUIDropDown this method must be called after all other render operations in the OnGUI function. The open drop down list will be rendered on top of the screen.
	/// </remarks>
	public static void RenderDropDownList()
	{
		if(KGFGUIDropDown.itsOpenInstance != null && KGFGUIDropDown.itsCorrectedOffset)
		{
			GUI.depth = 0;
			
			Rect aListRect;
			bool aDirection;
			
			if(KGFGUIDropDown.itsOpenInstance.itsDirection == KGFGUIDropDown.eDropDirection.eDown
			   || (KGFGUIDropDown.itsOpenInstance.itsDirection == KGFGUIDropDown.eDropDirection.eAuto
			       && (KGFGUIDropDown.itsOpenInstance.itsLastRect.y + KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButton).fixedHeight + KGFGUIDropDown.itsOpenInstance.itsHeight) < Screen.height))
			{
				aListRect = new Rect(KGFGUIDropDown.itsOpenInstance.itsLastRect.x,
				                     KGFGUIDropDown.itsOpenInstance.itsLastRect.y + KGFGUIUtility.GetStyleButton(KGFGUIUtility.eStyleButton.eButton).fixedHeight,
				                     KGFGUIDropDown.itsOpenInstance.itsWidth,
				                     KGFGUIDropDown.itsOpenInstance.itsHeight);
				aDirection = true;
			}
			else
			{
				aListRect = new Rect(KGFGUIDropDown.itsOpenInstance.itsLastRect.x,
				                     KGFGUIDropDown.itsOpenInstance.itsLastRect.y - KGFGUIDropDown.itsOpenInstance.itsHeight,
				                     KGFGUIDropDown.itsOpenInstance.itsWidth,
				                     KGFGUIDropDown.itsOpenInstance.itsHeight);
				aDirection = false;
			}
			
			/*
			if(Application.isPlaying)
			{
			 */
			GUILayout.BeginArea(aListRect);
			{
				if(itsSkinIndex == -1)
				{
					KGFGUIDropDown.itsOpenInstance.itsScrollPosition = KGFGUIUtility.BeginScrollView(KGFGUIDropDown.itsOpenInstance.itsScrollPosition, false, false, GUILayout.ExpandWidth(true));
				}
				else
				{
					KGFGUIDropDown.itsOpenInstance.itsScrollPosition = GUILayout.BeginScrollView(KGFGUIDropDown.itsOpenInstance.itsScrollPosition, false, false, GUILayout.ExpandWidth(true));
				}
				{
					foreach(string aEntry in KGFGUIDropDown.itsOpenInstance.GetEntrys())
					{
						if(aEntry != string.Empty)
						{
							if(KGFGUIUtility.Button(aEntry, KGFGUIUtility.eStyleButton.eButtonMiddle, GUILayout.ExpandWidth(true)))
							{
								KGFGUIDropDown.itsOpenInstance.SetSelectedItem(aEntry);
								KGFGUIDropDown.itsOpenInstance = null;
								break;
							}
						}
					}
				}
				GUILayout.EndScrollView();
			}
			GUILayout.EndArea();
			/*
			}
			else
			{
				
			}*/

			if(aDirection)
			{
				aListRect.y -= KGFGUIUtility.GetSkinHeight();
				aListRect.height += KGFGUIUtility.GetSkinHeight();
			}
			else
			{
				//aListRect.y -= KGFGUIUtility.GetButtonStyle().fixedHeight;
				aListRect.height += KGFGUIUtility.GetSkinHeight();
			}
			
			Vector3 aMousePosition = Input.mousePosition;
			aMousePosition.y = Screen.height - aMousePosition.y;
			
			//check if the rect contains the mouse and the pressed mouse button is the left mouse button
			if(!aListRect.Contains(aMousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
			{
				KGFGUIDropDown.itsOpenInstance = null;
			}
			
			if(KGFGUIDropDown.itsOpenInstance != null)
			{
				if(aListRect.Contains(aMousePosition))
				{
					KGFGUIDropDown.itsOpenInstance.itsHover = true;
				}
				else
				{
					KGFGUIDropDown.itsOpenInstance.itsHover = false;
				}
			}
		}
	}
	
	/// <summary>
	/// Use this method to draw a space in the size of the skin height
	/// </summary>
	/// <remarks>
	/// Use GUILayout to draw a custom sized space. If you use KGFGUIUtility.Space the space will always match the current size of the skin,
	/// so changing the skin will also adapt the space to the correct size
	/// </remarks>
	public static void Space()
	{
		GUILayout.Space(GetSkinHeight());
	}
	
	/// <summary>
	/// Use this method to draw a space in half of the size
	/// </summary>
	/// <remarks>
	/// Use GUILayout to draw a custom sized space. If you use KGFGUIUtility.SpaceSmall the space will always match the current size of the skin/2.0f,
	/// so changing the skin will also adapt the space to the correct size
	/// </remarks>
	public static void SpaceSmall()
	{
		GUILayout.Space(GetSkinHeight()/2.0f);
	}
	
	
	#region label
	
	/// <summary>
	/// Use this method to draw a label
	/// </summary>
	/// <remarks>
	/// Use this method to draw a label. The style of the label will be set to the default skin style.
	/// </remarks>
	/// <param name="theText">the text inside the lable</param>
	/// <param name="theLayout">GUILayout options</param>
	public static void Label(string theText, params GUILayoutOption[] theLayout)
	{
		Label(theText, eStyleLabel.eLabel, theLayout);
	}

	/// <summary>
	/// Use this method to draw a label
	/// </summary>
	/// <remarks>
	/// Use this method to draw a label with a specified style.
	/// </remarks>
	/// <param name="theText">the text inside the label</param>
	/// <param name="theStyleLabel">the style of the label</param>
	/// <param name="theLayout">GUILayout options</param>
	public static void Label(string theText, eStyleLabel theStyleLabel, params GUILayoutOption[] theLayout)
	{
		Label(theText,null,theStyleLabel,theLayout);
	}
	
	/// <summary>
	/// Use this method to draw a label with text and an icon
	/// </summary>
	/// <remarks>
	/// Use this method to draw a label with a specified style and an icon.
	/// </remarks>
	/// <param name="theText">the text inside the label</param>
	/// <param name="theImage">the icon inside the label</param>
	/// <param name="theStyleLabel">the style of the label</param>
	/// <param name="theLayout">GUILayout options</param>
	public static void Label(string theText, Texture2D theImage, eStyleLabel theStyleLabel, params GUILayoutOption[] theLayout)
	{
		Init();
		GUIContent aGuiContent = null;
		
		if(theImage != null)
		{
			aGuiContent = new GUIContent(theText, theImage);
		}
		else
		{
			aGuiContent = new GUIContent(theText);
		}
		
		if(itsSkinIndex == -1)
		{
			GUILayout.Label(aGuiContent, theLayout);
		}
		else
		{
			GUILayout.Label(aGuiContent, GetStyleLabel(theStyleLabel), theLayout);
		}
	}
	
	#endregion
	
	/// <summary>
	/// Use this method to draw a seperator
	/// </summary>
	/// <remarks>
	/// Use this method to draw a seperator for a spacing between two elements
	/// </remarks>
	/// <param name="theStyleSeparator">the style of the seperator</param>
	/// <param name="theLayout">GUILayout options</param>
	public static void Separator(eStyleSeparator theStyleSeparator, params GUILayoutOption[] theLayout)
	{
		Init();
		if(itsSkinIndex == -1)
		{
			GUILayout.Label("|",theLayout);
		}
		else
		{
			GUILayout.Label("", GetStyleSeparator(theStyleSeparator),theLayout);
		}
	}

	/// <summary>
	/// Use this method to draw a toggle button
	/// </summary>
	/// <remarks>
	/// Use this method to draw a toggle button. to check if the state of the button has changed check the return value of this method.
	/// </remarks>
	/// <param name="theValue">the current state of the toggle button</param>
	/// <param name="theText">the text of the toggle button</param>
	/// <param name="theToggleStyle">the style of the toggle button</param>
	/// <param name="theLayout">GUILayout options</param>
	/// <returns>returns true if the close toggle button was clicked</returns>
	public static bool Toggle(bool theValue, string theText, eStyleToggl theToggleStyle, params GUILayoutOption[] theLayout)
	{
		Init();
		bool aNewValue = false;
		
		if(itsSkinIndex == -1)
		{
			aNewValue = GUILayout.Toggle(theValue, theText, theLayout);
		}
		else
		{
			aNewValue = GUILayout.Toggle(theValue, theText, GetStyleToggl(theToggleStyle), theLayout);
		}
		if (aNewValue != theValue)
		{
			PlaySound(theToggleStyle.ToString());
		}
		return aNewValue;
	}
	
	/// <summary>
	/// Use this method to draw a toggle button
	/// </summary>
	/// <remarks>
	/// Use this method to draw a toggle button. to check if the state of the button has changed check the return value of this method.
	/// </remarks>
	/// <param name="theValue">the current state of the toggle button</param>
	/// <param name="theImage">the image of the toggle button</param>
	/// <param name="theToggleStyle">the style of the toggle button</param>
	/// <param name="theLayout">GUILayout options</param>
	/// <returns>returns true if the close toggle button was clicked</returns>
	public static bool Toggle(bool theValue, Texture2D theImage, eStyleToggl theToggleStyle, params GUILayoutOption[] theLayout)
	{
		Init();
		bool aNewValue = false;
		if(itsSkinIndex == -1)
		{
			aNewValue = GUILayout.Toggle(theValue, theImage, theLayout);
		}
		else
		{
			aNewValue = GUILayout.Toggle(theValue, theImage, GetStyleToggl(theToggleStyle), theLayout);
		}
		if (aNewValue != theValue)
		{
			PlaySound(theToggleStyle.ToString());
		}
		return aNewValue;
	}

	public static bool Toggle (bool theValue, string theText, Texture2D theImage, eStyleToggl theToggleStyle, params GUILayoutOption[] theLayout)
	{
		Init();
		GUIContent aGuiContent = null;

		if(theImage != null)
		{
			aGuiContent = new GUIContent(theText, theImage);
		}
		else
		{
			aGuiContent = new GUIContent(theText);
		}
		bool aNewValue = false;
		if(itsSkinIndex == -1)
		{
			aNewValue = GUILayout.Toggle(theValue, aGuiContent, theLayout);
		}
		else
		{
			aNewValue = GUILayout.Toggle(theValue, aGuiContent, GetStyleToggl(theToggleStyle), theLayout);
		}
		if (aNewValue != theValue)
		{
			PlaySound(theToggleStyle.ToString());
		}
		return aNewValue;
	}

	#region window
	
	/// <summary>
	/// Use this method to draw a window
	/// </summary>
	/// <remarks>
	/// Use this method to draw a window. To get the new position of the window check the return value of this method.
	/// </remarks>
	/// <param name="theId">the unique window id</param>
	/// <param name="theRect">the windows rectangle</param>
	/// <param name="theFunction">the function that draw this window</param>
	/// <param name="theText">the window header text</param>
	/// <param name="theLayout">GUILayout options</param>
	/// <returns>returns the new window rectangle if the window was draged to a differnt screen position</returns>
	public static Rect Window(int theId, Rect theRect, GUI.WindowFunction theFunction, string theText, params GUILayoutOption[] theLayout)
	{
		return Window(theId,theRect,theFunction,null,theText,theLayout);
	}
	
	/// <summary>
	/// Use this method to draw a window
	/// </summary>
	/// <remarks>
	/// Use this method to draw a window. To get the new position of the window check the return value of this method.
	/// </remarks>
	/// <param name="theId">the unique window id</param>
	/// <param name="theRect">the windows rectangle</param>
	/// <param name="theFunction">the function that draw this window</param>
	/// <param name="theImage">the window header icon</param>
	/// <param name="theLayout">GUILayout options</param>
	/// <returns>returns the new window rectangle if the window was draged to a differnt screen position</returns>
	public static Rect Window(int theId, Rect theRect, GUI.WindowFunction theFunction, Texture theImage, params GUILayoutOption[] theLayout)
	{
		return Window(theId,theRect,theFunction,theImage,string.Empty,theLayout);
	}
	
	/// <summary>
	/// Use this method to draw a window
	/// </summary>
	/// <remarks>
	/// Use this method to draw a window. To get the new position of the window check the return value of this method.
	/// </remarks>
	/// <param name="theId">the unique window id</param>
	/// <param name="theRect">the windows rectangle</param>
	/// <param name="theFunction">the function that draw this window</param>
	/// <param name="theImage">the window header icon</param>
	/// <param name="theText">the window header text</param>
	/// <param name="theLayout">GUILayout options</param>
	/// <returns>returns the new window rectangle if the window was draged to a differnt screen position</returns>
	public static Rect Window(int theId, Rect theRect, GUI.WindowFunction theFunction, Texture theImage, string theText, params GUILayoutOption[] theLayout)
	{
		Init();
		
		GUIContent aGuiContent = null;
		if(theImage != null)
		{
			aGuiContent = new GUIContent(theText, theImage);
		}
		else
		{
			aGuiContent = new GUIContent(theText);
		}
		
		if(itsSkinIndex != -1)
		{
			if(itsStyleWindow[itsSkinIndex] != null)
			{
				return GUILayout.Window(theId, theRect,theFunction,aGuiContent,itsStyleWindow[itsSkinIndex],theLayout);
			}
			else
			{
				return GUILayout.Window(theId, theRect,theFunction,aGuiContent,theLayout);
			}
		}
		else
		{
			return GUILayout.Window(theId, theRect,theFunction,aGuiContent,theLayout);
		}
	}

	#endregion
	
	#region box
	
	/// <summary>
	/// Use this method to draw a box
	/// </summary>
	/// <remarks>
	/// Use this method to draw a box.
	/// </remarks>
	/// <param name="theText">the text inside the box</param>
	/// <param name="theStyleBox">the style of the box</param>
	/// <param name="theLayout">GUILayout options</param>
	public static void Box(string theText, eStyleBox theStyleBox, params GUILayoutOption[] theLayout)
	{
		Box(null, theText, theStyleBox, theLayout);
	}

	/// <summary>
	/// Use this method to draw a box
	/// </summary>
	/// <remarks>
	/// Use this method to draw a box.
	/// </remarks>
	/// <param name="theText">the text inside the box</param>
	/// <param name="theStyleBox">the style of the box</param>
	/// <param name="theLayout">GUILayout options</param>
	public static void Box(Texture theImage, eStyleBox theStyleBox, params GUILayoutOption[] theLayout)
	{
		Box(theImage, "", theStyleBox, theLayout);
	}
	
	/// <summary>
	/// Use this method to draw a box
	/// </summary>
	/// <remarks>
	/// Use this method to draw a box.
	/// </remarks>
	/// <param name="theImage">the icon of the box</param>
	/// <param name="theText">the text inside the box</param>
	/// <param name="theStyleBox">the style of the box</param>
	/// <param name="theLayout">GUILayout options</param>
	public static void Box(Texture theImage, string theText, eStyleBox theStyleBox, params GUILayoutOption[] theLayout)
	{
		Init();
		GUIContent aGuiContent = null;
		if(theImage != null)
		{
			aGuiContent = new GUIContent(theText, theImage);
		}
		else
		{
			aGuiContent = new GUIContent(theText);
		}
		
		if(itsSkinIndex == -1)
		{
			GUILayout.Box(aGuiContent, theLayout);
		}
		else
		{
			GUILayout.Box(aGuiContent, GetStyleBox(theStyleBox), theLayout);
		}
	}
	
	#endregion
	
	/// <summary>
	/// Use this method to draw a vertical box
	/// </summary>
	/// <remarks>
	/// Use this method to draw a vertical box. For proper usage call EndVerticalBox() after using this function.
	/// </remarks>
	/// <param name="theStyleBox">the style of the box</param>
	/// <param name="theLayout">GUILayout options</param>
	public static void BeginVerticalBox(eStyleBox theStyleBox, params GUILayoutOption[] theLayout)
	{
		Init();
		if(itsSkinIndex == -1)
		{
			GUILayout.BeginVertical(GUI.skin.box,theLayout);
		}
		else
		{
			GUILayout.BeginVertical(GetStyleBox(theStyleBox), theLayout);
		}
	}
	
	/// <summary>
	/// Use this method to end a vertical box
	/// </summary>
	/// <remarks>
	/// Use this method to end a vertical box. For proper usage call BeginVerticalBox() before using this function.
	/// </remarks>
	public static void EndVerticalBox()
	{
		GUILayout.EndVertical();
	}

	/// <summary>
	/// Use this method to draw a box with vertical padding
	/// </summary>
	/// <remarks>
	/// Use this method to draw a box with padding. For proper usage call EndVerticalPadding() after using this function.
	/// </remarks>
	public static void BeginVerticalPadding()
	{
		GUILayout.BeginVertical();
		BeginHorizontalBox(eStyleBox.eBoxInvisible);
	}
	
	/// <summary>
	/// Use this method to end a box with vertical padding
	/// </summary>
	/// <remarks>
	/// Use this method to end a box with vertical padding. For proper usage call BeginVerticalBox() before using this function.
	/// </remarks>
	public static void EndVerticalPadding()
	{
		EndHorizontalBox();
		GUILayout.EndVertical();
	}
	
	/// <summary>
	/// Use this method to draw a box with horizontal padding
	/// </summary>
	/// <remarks>
	/// Use this method to draw a box with horizontal padding. For proper usage call EndHorizontalPadding() after using this function.
	/// </remarks>
	public static void BeginHorizontalPadding()
	{
		GUILayout.BeginHorizontal();
		BeginVerticalBox(eStyleBox.eBoxInvisible);
	}
	
	/// <summary>
	/// Use this method to end a box with horizontal padding
	/// </summary>
	/// <remarks>
	/// Use this method to end a box with horizontal padding. For proper usage call BeginHorizontalPadding() before using this function.
	/// </remarks>
	public static void EndHorizontalPadding()
	{
		EndVerticalBox();
		GUILayout.EndHorizontal();
	}
	
	/// <summary>
	/// Use this method to draw a horizontal box
	/// </summary>
	/// <remarks>
	/// Use this method to draw a horizontal box. For proper usage call EndHorizontalBox() after using this function.
	/// </remarks>
	/// <param name="theStyleBox">the style of the box</param>
	/// <param name="theLayout">GUILayout options</param>
	public static void BeginHorizontalBox(eStyleBox theStyleBox, params GUILayoutOption[] theLayout)
	{
		Init();
		if(itsSkinIndex == -1)
		{
			GUILayout.BeginHorizontal(GUI.skin.box,theLayout);
		}
		else
		{
			GUILayout.BeginHorizontal(GetStyleBox(theStyleBox), theLayout);
		}
	}
	
	/// <summary>
	/// Use this method to end a vertical box
	/// </summary>
	/// <remarks>
	/// Use this method to end a vertical box. For proper usage call BeginHorizontalBox() before using this function.
	/// </remarks>
	public static void EndHorizontalBox()
	{
		GUILayout.EndHorizontal();
	}

	/// <summary>
	/// Use this method to draw a scroll view
	/// </summary>
	/// <remarks>
	/// Use this method to draw a scroll view. To get the new position of the scrollview check the return value of this method. For proper use call EndScrollView() after using this method.
	/// </remarks>
	/// <param name="thePosition">the current position of the scrollview</param>
	/// <param name="theHorizontalAlwaysVisible">if the horizontal scrollbar is always visible</param>
	/// <param name="theVerticalAlwaysVisible">if the vertical scrollbar is always visible</param>
	/// <param name="theLayout">GUILayout options</param>
	/// <returns>returns the new window rectangle if the window was draged to a differnt screen position</returns>
	public static Vector2 BeginScrollView(Vector2 thePosition, bool theHorizontalAlwaysVisible, bool theVerticalAlwaysVisible, params GUILayoutOption[] theLayout)
	{
		Init();
		if(itsSkinIndex != -1)
		{
			GUI.skin = itsSkin[itsSkinIndex];
		}
		
		if(itsStyleHorizontalScrollbar != null && itsStyleVerticalScrollbar != null && itsSkinIndex != -1)
		{
			return GUILayout.BeginScrollView(thePosition, theHorizontalAlwaysVisible, theVerticalAlwaysVisible, itsStyleHorizontalScrollbar[itsSkinIndex], itsStyleVerticalScrollbar[itsSkinIndex], theLayout);
		}
		else
		{
			return GUILayout.BeginScrollView(thePosition, theHorizontalAlwaysVisible, theVerticalAlwaysVisible, theLayout);
		}
	}
	
	/// <summary>
	/// Use this method to end a scroll view
	/// </summary>
	/// <remarks>
	/// Use this method to end a scroll view. For proper usage call BeginScrollView() before using this function.
	/// </remarks>
	public static void EndScrollView()
	{
		GUILayout.EndScrollView();
	}

	/// <summary>
	/// Use this method to draw a text field
	/// </summary>
	/// <remarks>
	/// Use this method to draw a text field. To get the new text inside the control use the return value of this method.
	/// </remarks>
	/// <param name="theText">the current text of the textbox</param>
	/// <param name="theStyleTextField">the style of the text field</param>
	/// <param name="theLayout">GUILayout options</param>
	/// <returns>returns the new text inside the control</returns>
	public static string TextField(string theText, eStyleTextField theStyleTextField,  params GUILayoutOption[] theLayout)
	{
		Init();
		if(itsSkinIndex == -1)
		{
			return GUILayout.TextField(theText, theLayout);
		}
		else
		{
			return GUILayout.TextField(theText, GetStyleTextField(theStyleTextField), theLayout);
		}
	}
	
	/// <summary>
	/// Use this method to draw a text area
	/// </summary>
	/// <remarks>
	/// Use this method to draw a text area. To get the new text inside the control use the return value of this method.
	/// </remarks>
	/// <param name="theText">the current text of the textbox</param>
	/// <param name="theLayout">GUILayout options</param>
	/// <returns>returns the new text inside the control</returns>
	public static string TextArea(string theText, params GUILayoutOption[] theLayout)
	{
		Init();
		
		if(itsStyleTextArea[itsSkinIndex] != null && itsSkinIndex != -1)
		{
			return GUILayout.TextArea(theText, itsStyleTextArea[itsSkinIndex], theLayout);
		}
		else
		{
			return GUILayout.TextArea(theText, theLayout);
		}
	}

	#region button
	
	/// <summary>
	/// Use this method to draw a button
	/// </summary>
	/// <remarks>
	/// Use this method to draw a button. To get the state of the utton use the return value of this method.
	/// </remarks>
	/// <param name="theText">the text inside the button</param>
	/// <param name="theButtonStyle">the render style of the button</param>
	/// <param name="theLayout">GUILayout options</param>
	/// <returns>returns the state of the button in this frame. returns true if the button is clicked.</returns>
	public static bool Button(string theText, eStyleButton theButtonStyle, params GUILayoutOption[] theLayout)
	{
		return Button(null, theText, theButtonStyle, theLayout);
	}

	/// <summary>
	/// Use this method to draw a button
	/// </summary>
	/// <remarks>
	/// Use this method to draw a button. To get the state of the utton use the return value of this method.
	/// </remarks>
	/// <param name="theImage">the icon inside the button</param>
	/// <param name="theButtonStyle">the render style of the button</param>
	/// <param name="theLayout">GUILayout options</param>
	/// <returns>returns the state of the button in this frame. returns true if the button is clicked.</returns>
	public static bool Button(Texture theImage, eStyleButton theButtonStyle, params GUILayoutOption[] theLayout)
	{
		return Button(theImage, "", theButtonStyle, theLayout);
	}
	
	/// <summary>
	/// Use this method to draw a button
	/// </summary>
	/// <remarks>
	/// Use this method to draw a button. To get the state of the utton use the return value of this method.
	/// </remarks>
	/// <param name="theImage">the icon inside the button (icon is before text)</param>
	/// <param name="theText">the text inside the button (text is after icon)</param>
	/// <param name="theButtonStyle">the render style of the button</param>
	/// <param name="theLayout">GUILayout options</param>
	/// <returns>returns the state of the button in this frame. returns true if the button is clicked.</returns>
	public static bool Button(Texture theImage, string theText, eStyleButton theButtonStyle, params GUILayoutOption[] theLayout)
	{
		GUIContent aGuiContent = null;
		
		if(theImage != null)
		{
			aGuiContent = new GUIContent(theText, theImage);
		}
		else
		{
			aGuiContent = new GUIContent(theText);
		}
		
		Init();
		
		if(itsSkinIndex == -1)
		{
			if (GUILayout.Button(aGuiContent, theLayout))
			{
				PlaySound(theButtonStyle.ToString());
				return true;
			}
		}
		else
		{
			if (GUILayout.Button(aGuiContent, GetStyleButton(theButtonStyle), theLayout))
			{
				PlaySound(theButtonStyle.ToString());
				return true;
			}
		}
		return false;
	}
	
	#endregion
	
	/// <summary>
	/// Use this method to draw a cursor with up, right, down, left and center button
	/// </summary>
	/// <remarks>
	/// Use this method to draw a text area. To get the new text inside the control use the return value of this method.
	/// </remarks>
	/// <returns>returns the state of the cursor in this frame</returns>
	public static eCursorState Cursor()
	{
		return Cursor(null, null, null, null, null);
	}
	
	/// <summary>
	/// Use this method to draw a cursor with up, right, down, left and center button
	/// </summary>
	/// <remarks>
	/// Use this method to draw a text area. To get the new text inside the control use the return value of this method.
	/// </remarks>
	/// <param name="theUp">the text inside the up button</param>
	/// <param name="theRight">the text inside the right button</param>
	/// <param name="theDown">the text inside the down button</param>
	/// <param name="theLeft">the text inside the left button</param>
	/// <param name="theCenter">the text inside the center button</param>
	/// <returns>returns the state of the cursor in this frame</returns>
	public static eCursorState Cursor(Texture theUp, Texture theRight, Texture theDown, Texture theLeft, Texture theCenter)
	{
		float aHeight = GetSkinHeight();
		float aTotalControlSize = aHeight*3.0f;
		
		eCursorState aState = eCursorState.eNone;
		
		GUILayout.BeginVertical(GUILayout.ExpandWidth(false),GUILayout.ExpandHeight(false));
		{
			BeginHorizontalBox(eStyleBox.eBoxInvisible);
			{
				GUILayout.BeginVertical(GUILayout.Width(aTotalControlSize),GUILayout.Height(aTotalControlSize));
				{
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false),GUILayout.ExpandHeight(false));
					{
						GUILayout.Space(aHeight);
						
						if(theUp != null)
						{
							if(Button(theUp,eStyleButton.eButtonTop,GUILayout.Width(aHeight)))
							{
								aState = eCursorState.eUp;
							}
						}
						else
						{
							if(Button("",eStyleButton.eButtonTop,GUILayout.Width(aHeight)))
							{
								aState = eCursorState.eUp;
							}
						}
						GUILayout.Space(aHeight);
					}
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false),GUILayout.ExpandHeight(false));
					{
						if(theLeft != null)
						{
							if(Button(theLeft,eStyleButton.eButtonLeft,GUILayout.Width(aHeight)))
							{
								aState = eCursorState.eLeft;
							}
						}
						else
						{
							if(Button("",eStyleButton.eButtonLeft,GUILayout.Width(aHeight)))
							{
								aState = eCursorState.eLeft;
							}
						}
						
						if(theCenter != null)
						{
							if(Button(theCenter,eStyleButton.eButtonLeft,GUILayout.Width(aHeight)))
							{
								aState = eCursorState.eCenter;
							}
						}
						else
						{
							if(Button("",eStyleButton.eButtonMiddle,GUILayout.Width(aHeight)))
							{
								aState = eCursorState.eCenter;
							}
						}
						
						if(theRight != null)
						{
							if(Button(theRight, eStyleButton.eButtonLeft,GUILayout.Width(aHeight)))
							{
								aState = eCursorState.eRight;
							}
						}
						else
						{
							if(Button("",eStyleButton.eButtonRight,GUILayout.Width(aHeight)))
							{
								aState = eCursorState.eRight;
							}
						}
					}
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false),GUILayout.ExpandHeight(false));
					{
						GUILayout.Space(aHeight);
						
						if(theDown != null)
						{
							if(Button(theDown, eStyleButton.eButtonLeft,GUILayout.Width(aHeight)))
							{
								aState = eCursorState.eDown;
							}
						}
						else
						{
							if(Button("",eStyleButton.eButtonBottom,GUILayout.Width(aHeight)))
							{
								aState = eCursorState.eDown;
							}
						}
						GUILayout.Space(aHeight);
					}
					GUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();
			}
			EndHorizontalBox();
		}
		GUILayout.EndVertical();
		
		return aState;
	}

	#region slider
	
	/// <summary>
	/// use this method to draw a horizontal slider
	/// </summary>
	/// <remarks>
	/// This method draws a horizontal oriented slider and returns the current slider position.
	/// </remarks>
	/// <param name="theValue">the current slider position</param>
	/// <param name="theLeftValue">the minimum value of the slider</param>
	/// <param name="theRightValue">the maximum value of the slider</param>
	/// <param name="theLayout">GUILayout options</param>
	/// <returns>returns the new position of the slider.</returns>
	public static float HorizontalSlider(float theValue, float theLeftValue, float theRightValue, params GUILayoutOption[] theLayout)
	{
		Init();
		
		if(itsStyleHorizontalSlider != null && itsStyleHorizontalSliderThumb != null && itsSkinIndex != -1)
		{
			return GUILayout.HorizontalSlider(theValue, theLeftValue, theRightValue, itsStyleHorizontalSlider[itsSkinIndex], itsStyleHorizontalSliderThumb[itsSkinIndex], theLayout);
		}
		else
		{
			return GUILayout.HorizontalSlider(theValue, theLeftValue, theRightValue, theLayout);
		}
	}
	
	/// <summary>
	/// use this method to draw a vertival slider
	/// </summary>
	/// <remarks>
	/// This method draws a vertical oriented slider and returns the current slider position.
	/// </remarks>
	/// <param name="theValue">the current slider position</param>
	/// <param name="theLeftValue">the minimum value of the slider</param>
	/// <param name="theRightValue">the maximum value of the slider</param>
	/// <param name="theLayout">GUILayout options</param>
	/// <returns>returns the new position of the slider.</returns>
	public static float VerticalSlider(float theValue, float theLeftValue, float theRightValue, params GUILayoutOption[] theLayout)
	{
		Init();
		
		if(itsStyleVerticalSlider != null && itsStyleVerticalSliderThumb != null && itsSkinIndex != -1)
		{
			return GUILayout.VerticalSlider(theValue, theLeftValue, theRightValue, itsStyleVerticalSlider[itsSkinIndex], itsStyleVerticalSliderThumb[itsSkinIndex], theLayout);
		}
		else
		{
			return GUILayout.VerticalSlider(theValue, theLeftValue, theRightValue, theLayout);
		}
	}
	
	#endregion
	
	#endregion
	
	#endregion
}