// <author>Florian Gnadlinger</author>
// <email>fg@ovos.at</email>

using UnityEngine;
using System.Collections;

/// <summary>
///	This class represents the entity for localized strings
/// </summary>
/// <remarks>
/// This class contains a text in a specific language 
/// </remarks>

//TODO FG: Implement Validator (check empty String) + Editor Class
public class KGFString : MonoBehaviour {

	/// <summary>
	/// itsString containes a text in a specific language
	/// </summary>
	public string itsString;


	/// <summary>
	/// GetString returns the text
	/// </summary>
	/// <returns></returns>
	public string GetString()
	{
		return itsString;
	}

	public override string ToString()
	{
		return itsString;
	}
}
