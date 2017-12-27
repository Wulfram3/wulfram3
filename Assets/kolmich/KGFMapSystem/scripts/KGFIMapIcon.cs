// <author>Alexander Murauer</author>
// <email>alexander.murauer@kolmich.at</email>
// <date>2011-10-03</date>

using UnityEngine;
using System.Collections;

/// <summary>
/// Defines a generic map icon for usage with the KGFMapSystem. The default implementation is KGFMapIcon
/// </summary>
/// <seealso cref="KGFMapIcon"/>
public interface KGFIMapIcon
{
	/// <summary>
	/// Get the category of map icon (The visibility of all map icons of a category can be enabled/disabled at once by the KGFMapSystem)
	/// </summary>
	/// <returns>a category name</returns>
	/// <remarks>Get the category name.</remarks>
	string GetCategory();
	
	/// <summary>
	/// This is the color in which the map icon and its arrow will be displayed on the minimap and map
	/// </summary>
	/// <returns>the current map icon color</returns>
	/// <remarks>Get current map icon color.</remarks>
	Color GetColor();
	
	/// <summary>
	/// this texture will be used as arrow that will point in the direction of the map icon if it is outside the minimap.
	/// </summary>
	/// <returns>the texture that is used for the arrow icon</returns>
	/// <remarks>Get the texture used for the arrow.</remarks>
	Texture2D GetTextureArrow();
	
	/// <summary>
	/// Indicates if the icon on the minimap will follow the rotation of the gameobject
	/// </summary>
	/// <returns>
	/// TRUE, if the map icon should follow the rotation of the gameobject,
	/// FALSE otherwise.
	/// </returns>
	/// <remarks>Get if the icon rotation is bound to the gameobject.</remarks>
	bool GetRotate();
	
	/// <summary>
	/// Returns if map icon should be visible at the moment (this method should return a cached value, used very often)
	/// </summary>
	/// <returns>TRUE, if icon is visible, FALSE otherwise.</returns>
	/// <remarks>Get current icon visiblity.</remarks>
	bool GetIsVisible();
	
	/// <summary>
	/// Returns if the arrow icon is allowd to be displayed.
	/// </summary>
	/// <returns>TRUE, if the arrow is used to display out of minimap icons, FALSE otherwise.</returns>
	/// <remarks>Get an arrow is used for this map icon.</remarks>
	bool GetIsArrowVisible();
	
	/// <summary>
	/// Get transform of map icon
	/// </summary>
	/// <returns>the Transform of this map icon.</returns>
	/// <remarks>Get map icon represenation transform.</remarks>
	Transform GetTransform();
	
	/// <summary>
	/// This method should return the name of the gameObject the interface is attached to (used for debug reasons)
	/// </summary>
	/// <returns>the name of the gameobject.</returns>
	/// <remarks></remarks>
	string GetGameObjectName();
	
	/// <summary>
	/// This method should return a gameObject that will represent the map icon on the minimap. This gameobject must not be the map icon itself.
	/// This method can for example return a plane with a mapicon texture on it or simply a red cube. This gameobject will be transformed by the
	/// mapsystem to match the position of the map icon.
	/// </summary>
	/// <returns>the gameobject the mapsystem uses to represent the map icon.</returns>
	GameObject GetRepresentation();
	
	/// <summary>
	/// Returns if the tooltip should be displayed
	/// </summary>
	/// <returns>the if the tooltip should be displayed.</returns>
	bool GetShowToolTip();
	
	/// <summary>
	/// Allows to change the tooltip visibility
	/// </summary>
	/// <param name="theShowTooltip"></param>
	void SetShowToolTip(bool theShowTooltip);
	
	/// <summary>
	/// Returns the text that should be displayed as tooltip.
	/// </summary>
	/// <returns>the current tooltip of this map icon.</returns>
	string GetToolTipText();
	
	/// <summary>
	/// Return the per icon scaling multiplier.
	/// </summary>
	/// <returns>a scaling value the map representation is multiplied by.</returns>
	/// <remarks>Get per icon scaling multiplier.</remarks>
	float GetIconScale();
	
	/// <summary>
	/// Get blinking state.
	/// </summary>
	/// <returns>TRUE, if the mapicon currently is blinking, FALSE otherwise.</returns>
	/// <remarks>Get blinking state.</remarks>
	bool GetIsBlinking();
	
	/// <summary>
	/// Get depth
	/// </summary>
	/// <returns> depth of the mapicon. Icons with a higher depth are rendered above icons with a lower depth</returns>
	int GetDepth();
	
	/// <summary>
	/// Set blinking mode.
	/// </summary>
	/// <remarks>Set blinking state.</remarks>
	void SetIsBlinking(bool theActivate);
}
