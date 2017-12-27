// <author>Alexander Murauer</author>
// <email>alexander.murauer@kolmich.at</email>
// <date>2012-09-18</date>

/// <summary>
/// Boxed int type for reference behaviour
/// </summary>
[System.Serializable]
public class IDReference
{
	public string itsID = "";
	public bool itsEmpty = true;
	public bool itsCanBeDeleted = false;
	
	public string GetID()
	{
		return itsID;
	}
	
	public void SetID(string theID)
	{
		itsID = theID;
		itsEmpty = false;
	}
	
	public bool GetHasValue()
	{
		return !itsEmpty;
	}
	
	public void SetEmpty()
	{
		itsEmpty = true;
	}
	
	public override string ToString()
	{
		return GetID();
	}
	
	public bool GetCanBeDeleted()
	{
		return itsCanBeDeleted;
	}
	
	public void SetCanBeDeleted(bool theCanBeDeleted)
	{
		itsCanBeDeleted = theCanBeDeleted;
	}
}