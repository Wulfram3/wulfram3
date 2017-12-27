// <author>Alexander Murauer</author>
// <email>alexander.murauer@kolmich.at</email>
// <date>2011-11-08</date>
// <summary>short summary</summary>

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(KGFCustomGUI))]
public class KGFCustomGUIEditor : KGFEditor 
{
	public static KGFMessageList ValidateKGFCustomGUIEditor(UnityEngine.Object theTarget)
	{
		KGFMessageList aMessageList = KGFEditor.ValidateKGFEditor(theTarget);		
		return aMessageList;
	}
}