// <author>Alexander Murauer</author>
// <email>alexander.murauer@kolmich.at</email>
// <date>2012-07-27</date>
// <summary></summary>

using UnityEngine;

/// <summary>
/// Base class for all events
/// </summary>
[System.Serializable]
public abstract class KGFEventBase : KGFObject, KGFIValidator
{
	/// <summary>
	/// Force existence of method Trigger()
	/// </summary>
	public abstract void Trigger ();
	
	public virtual KGFMessageList Validate()
	{
		KGFMessageList aMessageList = new KGFMessageList();
		return aMessageList;
	}
}
