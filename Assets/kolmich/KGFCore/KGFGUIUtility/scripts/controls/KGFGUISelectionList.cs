// <author>Alexander Murauer</author>
// <email>alexander.murauer@kolmich.at</email>
// <date>2012-09-04</date>

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class KGFGUISelectionList
{
	class ListItem
	{
		public ListItem(object theItem)
		{
			itsItem = theItem;
			itsSelected = false;
			itsFiltered = false;
			
			UpdateCache(null);
		}
		
		private string itsCachedString;
		private object itsItem;
		public bool itsSelected;
		public bool itsFiltered;
		
		public void UpdateCache(Func<object,string> theDisplayMethod)
		{
			if (theDisplayMethod != null)
			{
				itsCachedString = theDisplayMethod(itsItem);
			}
			else
			{
				itsCachedString = itsItem.ToString();
			}
		}
		
		public string GetString()
		{
			return itsCachedString;
		}
		
		public object GetItem()
		{
			return itsItem;
		}
	}
	
	List<ListItem> itsData = new List<ListItem>();
	string itsSearch = "";
	IEnumerable itsListSource;
	Vector2 itsScrollPosition = Vector2.zero;
	Func<object,string> itsDisplayMethod = null;
	
	const string itsControlSearchName = "tagSearch";
	const string itsTextSearch = "Search";
	
	public event EventHandler EventItemChanged;
	
	/// <summary>
	/// Use the enumeration as the object source for the list, updates the list
	/// </summary>
	/// <param name="theList"></param>
	public void SetValues(IEnumerable theList)
	{
		itsListSource = theList;
		UpdateList();
		UpdateItemFilter();
	}
	
	/// <summary>
	/// Returns the selection state of an item
	/// </summary>
	/// <param name="theTitle"></param>
	/// <returns></returns>
	public bool GetIsSelected(object theItem)
	{
		foreach (ListItem anItem in itsData)
		{
			if (theItem == anItem.GetItem())
			{
				return anItem.itsSelected;
			}
		}
		return false;
	}
	
	/// <summary>
	/// Set a method to convert the objects to strings instead of using ToString method on every object
	/// </summary>
	/// <param name="theDisplayMethod"></param>
	public void SetDisplayMethod(Func<object,string> theDisplayMethod)
	{
		itsDisplayMethod = theDisplayMethod;
		UpdateItemFilter();
	}
	
	/// <summary>
	/// Use ToString() method on every object for conversion to displayable string
	/// </summary>
	public void ClearDisplayMethod()
	{
		itsDisplayMethod = null;
		UpdateItemFilter();
	}
	
	int ListItemComparer(ListItem theListItem1, ListItem theListItem2)
	{
		return theListItem1.GetString().CompareTo(theListItem2.GetString());
	}
	
	public void Render()
	{
		GUILayout.BeginVertical();
		{
			KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxDarkTop);
			{
				DrawButtons();
			}
			KGFGUIUtility.EndVerticalBox();
			KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVertical);
			{
				DrawList();
			}
			KGFGUIUtility.EndVerticalBox();
			KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleVertical);
			{
				KGFGUIUtility.Label ("",GUILayout.ExpandWidth(true));
			}
			KGFGUIUtility.EndHorizontalBox();
			KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxDarkBottom);
			{
				DrawSearch();
			}
			KGFGUIUtility.EndVerticalBox();
		}
		GUILayout.EndVertical();
		
		if(GUI.GetNameOfFocusedControl().Equals(itsControlSearchName))
		{
			if(itsSearch.Equals(itsTextSearch))
			{
				itsSearch = string.Empty;
			}
		}
		
		if(!GUI.GetNameOfFocusedControl().Equals(itsControlSearchName))
		{
			if(itsSearch.Equals(string.Empty))
			{
				itsSearch = itsTextSearch;
			}
		}
	}
	
	/// <summary>
	/// Get all selected items
	/// </summary>
	/// <returns></returns>
	public IEnumerable<object> GetSelected()
	{
		foreach (ListItem anItem in itsData)
		{
			if (anItem.itsSelected)
				yield return anItem.GetItem();
		}
		yield break;
	}
	
	/// <summary>
	/// Select only the items in the list, deselect all others
	/// </summary>
	/// <param name="theList"></param>
	public void SetSelected(IEnumerable<object> theList)
	{
		SetSelectedAll(false);
		foreach (object anItem in theList)
		{
			SetSelected(anItem,true);
		}
	}
	
	/// <summary>
	/// Set the selection state of a single item
	/// </summary>
	/// <param name="theTitle"></param>
	public void SetSelected(object theItem, bool theSelectionState)
	{
		foreach (ListItem anItem in itsData)
		{
			if (theItem == anItem.GetItem())
			{
				anItem.itsSelected = theSelectionState;
				return;
			}
		}
	}
	
	/// <summary>
	/// Set the selection state of items with the same string representation
	/// </summary>
	/// <param name="theTitle"></param>
	public void SetSelected(string theItem, bool theSelectionState)
	{
		foreach (ListItem anItem in itsData)
		{
			if (theItem == anItem.GetItem().ToString())
			{
				anItem.itsSelected = theSelectionState;
				return;
			}
		}
	}
	
	/// <summary>
	/// Updates the list with the filter from the source list
	/// Tries to keep the selection
	/// </summary>
	void UpdateList()
	{
		List<object> aSavedSelectionList = new List<object>(GetSelected());
		
		itsData.Clear();
		foreach (object anItem in itsListSource)
		{
			itsData.Add(new ListItem(""+anItem));
		}
		itsData.Sort(ListItemComparer);
		
		SetSelected(aSavedSelectionList);
	}
	
	/// <summary>
	/// Update the filtered state of each item in the list
	/// </summary>
	public void UpdateItemFilter()
	{
		if (itsSearch.Trim() == string.Empty || itsSearch.Trim() == itsTextSearch)
		{
			foreach (ListItem anItem in itsData)
			{
				anItem.itsFiltered = false;
			}
		}
		else
		{
			foreach (ListItem anItem in itsData)
			{
				anItem.UpdateCache(itsDisplayMethod);
				anItem.itsFiltered = !anItem.GetString().Trim().ToLower().Contains(itsSearch.Trim().ToLower());
			}
		}
	}
	
	/// <summary>
	/// Set the selected state of all in the list
	/// </summary>
	/// <param name="theValue"></param>
	public void SetSelectedAll(bool theValue)
	{
		foreach (ListItem anItem in itsData)
		{
			anItem.itsSelected = theValue;
		}
		
		if (EventItemChanged != null)
		{
			EventItemChanged(this,null);
		}
	}
	
	/// <summary>
	/// Draw the select all/none buttons
	/// </summary>
	void DrawButtons()
	{
		GUILayout.BeginHorizontal();
		{
			if (KGFGUIUtility.Button("All",KGFGUIUtility.eStyleButton.eButton))
			{
				SetSelectedAll(true);
			}
			if (KGFGUIUtility.Button("None",KGFGUIUtility.eStyleButton.eButton))
			{
				SetSelectedAll(false);
			}
		}
		GUILayout.EndHorizontal();
	}
	
	/// <summary>
	/// Draw the list of all items
	/// </summary>
	void DrawList()
	{
		itsScrollPosition = GUILayout.BeginScrollView(itsScrollPosition);
		{
			KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxInvisible);
			{
				foreach (ListItem aListItem in itsData)
				{
					if (aListItem.itsFiltered)
						continue;
					
					bool aValue = KGFGUIUtility.Toggle(aListItem.itsSelected,aListItem.GetString(),KGFGUIUtility.eStyleToggl.eTogglSuperCompact);
					if (aValue != aListItem.itsSelected)
					{
						aListItem.itsSelected = aValue;
						if (EventItemChanged != null)
						{
							EventItemChanged(this,null);
						}
					}
				}
			}
			KGFGUIUtility.EndVerticalBox();
		}
		GUILayout.EndScrollView();
	}
	
	/// <summary>
	/// Draw the full text search
	/// </summary>
	void DrawSearch()
	{
		GUI.SetNextControlName(itsControlSearchName);
		string aValue = KGFGUIUtility.TextField(itsSearch,KGFGUIUtility.eStyleTextField.eTextField);
		if (aValue != itsSearch)
		{
			itsSearch = aValue;
			UpdateItemFilter();
		}
	}
}