// <author>Alexander Murauer</author>
// <email>alexander.murauer@kolmich.at</email>
// <date>2012-09-04</date>
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

public class KGFObjectListItemDisplayAttribute : Attribute
{
	string itsHeader;
	bool itsSearchable;
	bool itsDisplay;

	public KGFObjectListItemDisplayAttribute (string theHeader)
	{
		itsHeader = theHeader;
		itsSearchable = false;
		itsDisplay = true;
	}
	
	public KGFObjectListItemDisplayAttribute (string theHeader, bool theSearchable)
	{
		itsHeader = theHeader;
		itsSearchable = theSearchable;
		itsDisplay = true;
	}
	
	public KGFObjectListItemDisplayAttribute (string theHeader, bool theSearchable, bool theDisplay)
	{
		itsHeader = theHeader;
		itsSearchable = theSearchable;
		itsDisplay = theDisplay;
	}
	
	public string Header
	{
		get
		{
			return itsHeader;
		}
	}
	
	public bool Searchable
	{
		get
		{
			return itsSearchable;
		}
	}
	
	public bool Display
	{
		get{
			return itsDisplay;
		}
	}
}

public class KGFGUIObjectList
{
	#region private members
	const string NONE_STRING = "<NONE>";
	List<KGFITaggable> itsListData;
	System.Type itsItemType;
	KGFDataTable itsData;
	KGFGUIDataTable itsGuiData;
	List<KGFObjectListColumnItem> itsListFieldCache;
	bool itsDisplayFullTextSearch = false;
	string itsFulltextSearch = "";
	KGFGUISelectionList itsListViewCategories;
	KGFDataRow itsCurrentSelectedRow = null;
	KGFITaggable itsCurrentSelectedItem = null;
	KGFeItemsPerPage itsItemsPerPage = KGFeItemsPerPage.e50;
	bool itsIncludeAll = true;
	#endregion

	public enum KGFeItemsPerPage
	{
		e10 = 10,
		e25 = 25,
		e50 = 50,
		e100 = 100,
		e250 = 250,
		e500 = 500
	}
	
	public class KGFGUIObjectListSelectEventArgs : EventArgs
	{
		KGFITaggable itsItem;
		
		public KGFGUIObjectListSelectEventArgs (KGFITaggable theItem)
		{
			itsItem = theItem;
		}
		
		public KGFITaggable GetItem ()
		{
			return itsItem;
		}
	}
	
	public event EventHandler EventSelect;
	public event EventHandler EventSettingsChanged;
	public event EventHandler EventNew;
	public event EventHandler EventDelete;
	
	class KGFObjectListColumnItem
	{
		public string itsHeader;
		public bool itsSearchable;
		public bool itsDisplay;
		public KGFGUIDropDown itsDropDown;
		public string itsFilterString = "";
		
		private MemberInfo itsMemberInfo;
		
		public KGFObjectListColumnItem(MemberInfo theMemberInfo)
		{
			itsMemberInfo = theMemberInfo;
		}
		
		public Type GetReturnType()
		{
			if (itsMemberInfo is FieldInfo)
			{
				return ((FieldInfo)itsMemberInfo).FieldType;
			}
			else if (itsMemberInfo is PropertyInfo)
			{
				return ((PropertyInfo)itsMemberInfo).PropertyType;
			}
			else
				return null;
		}
		
		public object GetReturnValue(object theInstance)
		{
			if (itsMemberInfo is FieldInfo)
			{
				return ((FieldInfo)itsMemberInfo).GetValue(theInstance);
			}
			else if (itsMemberInfo is PropertyInfo)
			{
				return ((PropertyInfo)itsMemberInfo).GetValue(theInstance,null);
			}
			else
				return null;
		}
		
		public bool GetIsFiltered (object theInstance)
		{
			if (GetReturnType() == typeof(bool) || GetReturnType().IsEnum)
			{
				if (itsDropDown != null)
				{
					if (itsDropDown.SelectedItem () == NONE_STRING)
					{
						return false;
					}
					
					if (itsDropDown.SelectedItem () != theInstance.ToString ())
					{
						return true;
					}
					
				}
				return false;
			}
			
			if (!string.IsNullOrEmpty(itsFilterString))
			{
				if (!theInstance.ToString().ToLower().Contains(itsFilterString.ToLower()))
					return true;
			}
			return false;
		}
	}
	
	#region public methods
	public KGFGUIObjectList (Type theType)
	{
		itsListData = new List<KGFITaggable> ();
		itsItemType = theType;
		itsData = new KGFDataTable ();
		itsListFieldCache = new List<KGFObjectListColumnItem> ();
		
		CacheTypeMembers ();
		itsGuiData = new KGFGUIDataTable (itsData);
		itsGuiData.OnClickRow += OnClickRow;
		itsGuiData.EventSettingsChanged += new EventHandler(OnGuiDataSettingsChanged);
		itsGuiData.SetColumnVisible (0, false);
		for (int i=0;i<itsListFieldCache.Count;i++)
		{
			itsGuiData.SetColumnVisible(i+1,itsListFieldCache[i].itsDisplay);
		}
		
		itsListViewCategories = new KGFGUISelectionList ();
		itsListViewCategories.EventItemChanged += OnCategoriesChanged;
	}

	void OnGuiDataSettingsChanged(object theSender, EventArgs theArgs)
	{
		OnSettingsChanged();
	}
	
	/// <summary>
	/// Change the fulltext filter
	/// </summary>
	/// <param name="theFulltextSearch"></param>
	public void SetFulltextFilter(string theFulltextSearch)
	{
		itsFulltextSearch = theFulltextSearch;
		UpdateList();
	}
	
	/// <summary>
	/// Change width of all columns
	/// </summary>
	/// <param name="theWidth"></param>
	public void SetColumnWidthAll(uint theWidth)
	{
		for (int i=1; i<itsListFieldCache.Count+1; i++)
		{
			itsGuiData.SetColumnWidth (i, theWidth);
		}
	}
	
	/// <summary>
	/// Change the width of a column by name
	/// </summary>
	/// <param name="theColumnHeader"></param>
	/// <param name="theWidth"></param>
	public void SetColumnWidth(string theColumnHeader, uint theWidth)
	{
		for (int i=0;i<itsListFieldCache.Count;i++)
		{
			if (itsListFieldCache[i].itsDisplay)
			{
				if (itsListFieldCache[i].itsHeader == theColumnHeader)
				{
					itsGuiData.SetColumnWidth (i+1, theWidth);
					break;
				}
			}
		}
	}
	
	/// <summary>
	/// Change visibility of a column by name
	/// </summary>
	/// <param name="theColumnHeader"></param>
	/// <param name="theVisible"></param>
	public void SetColumnVisible(string theColumnHeader, bool theVisible)
	{
		for (int i=0;i<itsListFieldCache.Count;i++)
		{
			if (itsListFieldCache[i].itsDisplay)
			{
				if (itsListFieldCache[i].itsHeader == theColumnHeader)
				{
					itsGuiData.SetColumnVisible(i+1,theVisible);
					break;
				}
			}
		}
	}
	
	public event EventHandler PreRenderRow
	{
		add{
			itsGuiData.PreRenderRow += value;
		}
		remove{
			itsGuiData.PreRenderRow -= value;
		}
	}
	
	public event Func<KGFDataRow,KGFDataColumn,uint,bool> PreCellHandler
	{
		add{
			itsGuiData.PreCellContentHandler += value;
		}
		remove{
			itsGuiData.PreCellContentHandler -= value;
		}
	}
	
	public event EventHandler PostRenderRow
	{
		add{
			itsGuiData.PostRenderRow += value;
		}
		remove{
			itsGuiData.PostRenderRow -= value;
		}
	}
	
	public void SetList (IEnumerable theList)
	{
		List<KGFITaggable> aList = new List<KGFITaggable> ();
		foreach (object anObject in theList)
		{
			if (anObject is KGFITaggable)
			{
				aList.Add ((KGFITaggable)anObject);
			}
		}
		
		SetList (aList);
	}
	
	public void SetList (IEnumerable<KGFITaggable> theList)
	{
		itsListData = new List<KGFITaggable> (theList);
		itsListViewCategories.SetValues (GetAllTags ().Distinct ());
//		itsListViewCategories.SetSelectedAll (true);
		UpdateList ();
	}
	
	/// <summary>
	/// Add a member to the list.
	/// Supported Memberinfos are PropertyInfo and FieldInfo
	/// </summary>
	/// <param name="theMemberInfo"></param>
	/// <param name="theHeader"></param>
	public void AddMember(MemberInfo theMemberInfo,string theHeader)
	{
		AddMember(theMemberInfo,theHeader,false);
	}
	
	/// <summary>
	/// Add a member to the list.
	/// Supported Memberinfos are PropertyInfo and FieldInfo
	/// </summary>
	/// <param name="theMemberInfo"></param>
	/// <param name="theHeader"></param>
	/// <param name="theSearchable"></param>
	public void AddMember(MemberInfo theMemberInfo,string theHeader, bool theSearchable)
	{
		AddMember(theMemberInfo,theHeader,theSearchable,true);
	}
	
	/// <summary>
	/// Add a member to the list.
	/// Supported Memberinfos are PropertyInfo and FieldInfo
	/// </summary>
	/// <param name="theMemberInfo"></param>
	/// <param name="theHeader"></param>
	/// <param name="theSearchable"></param>
	/// <param name="theDisplay"></param>
	public void AddMember(MemberInfo theMemberInfo,string theHeader, bool theSearchable, bool theDisplay)
	{
		KGFObjectListColumnItem anItem = new KGFObjectListColumnItem (theMemberInfo);
		anItem.itsHeader = theHeader;
		anItem.itsSearchable = theSearchable;
		anItem.itsDisplay = theDisplay;
		itsListFieldCache.Add (anItem);
		
		itsData.Columns.Add (new KGFDataColumn (theHeader, anItem.GetReturnType()));
		
		if (anItem.itsSearchable)
		{
			itsDisplayFullTextSearch = true;
		}
	}
	
	/// <summary>
	/// Returns currently selected item
	/// </summary>
	/// <returns></returns>
	public object GetCurrentSelected ()
	{
		return itsCurrentSelectedItem;
	}
	
	/// <summary>
	/// Clears the selected item
	/// </summary>
	public void ClearSelected ()
	{
		itsCurrentSelectedItem = null;
	}
	
	/// <summary>
	/// Set currently selected item
	/// </summary>
	/// <param name="theObject"></param>
	public void SetSelected(KGFITaggable theObject)
	{
		itsCurrentSelectedItem = theObject;
		
		int i = 0;
		foreach (KGFDataRow aRow in itsData.Rows)
		{
			if (aRow[0].Value == theObject)
			{
				itsGuiData.SetCurrentSelected(aRow);
				itsCurrentPage = i/((int)itsItemsPerPage);
				break;
			}
			i++;
		}
	}
	
	public int itsCurrentPage = 0;
	
	public Rect GetLastRectScrollView()
	{
		return itsGuiData.GetLastRectScrollview();
	}
	
	/// <summary>
	/// Save settings to a string
	/// </summary>
	/// <returns></returns>
	public string SaveSettings()
	{
		List<string> aListFilters = new List<string>();
		
		foreach (KGFObjectListColumnItem anItem in itsListFieldCache)
		{
			if (anItem.itsDropDown != null)
			{
				aListFilters.Add(anItem.itsHeader + "=" + anItem.itsDropDown.SelectedItem());
			}else
			{
				aListFilters.Add(anItem.itsHeader + "=" + anItem.itsFilterString);
			}
		}
		string aSortingString = ((itsGuiData.GetSortingColumn() != null)?itsGuiData.GetSortingColumn().ColumnName:"");
		
		List<string> aListTags = new List<string>();
		foreach (object anObject in itsListViewCategories.GetSelected())
		{
			aListTags.Add(""+anObject);
		}
		string aTagsString = aListTags.JoinToString(",");
		string aReturnString = string.Format("Filter:{0};SortBy:{1};Tags:{2}",aListFilters.JoinToString(","),aSortingString,aTagsString);
//		Debug.Log("Save:"+aReturnString);
		return aReturnString;
	}
	
	bool itsLoadingActive = false;
	/// <summary>
	/// Load settings from a string
	/// </summary>
	/// <param name="theSettingsString"></param>
	public void LoadSettings(string theSettingsString)
	{
//		Debug.Log("load:"+theSettingsString);
		itsLoadingActive = true;
		
		string []aSettingsArr = theSettingsString.Split(';');
		foreach (string aSettingString in aSettingsArr)
		{
			string []aSettingArr = aSettingString.Split(':');
			if (aSettingArr.Length == 2)
			{
				if (aSettingArr[0] == "Filter")
				{
					foreach (string aFilter in aSettingArr[1].Split(','))
					{
						string []aFilterArr = aFilter.Split('=');
						if (aFilterArr.Length == 2)
						{
							SetFilterInternal(aFilterArr[0],aFilterArr[1]);
						}
					}
				}
				if (aSettingArr[0] == "SortBy")
				{
					if (aSettingArr[1].Trim() == string.Empty)
						itsGuiData.SetSortingColumn((KGFDataColumn)null);
					else
						itsGuiData.SetSortingColumn(aSettingArr[1]);
				}
				if (aSettingArr[0] == "Tags")
				{
					itsListViewCategories.SetSelectedAll(false);
					foreach (string aTag in aSettingArr[1].Split(','))
					{
						itsListViewCategories.SetSelected(aTag,true);
					}
				}
			}
		}
		itsRepaintWish = true;
		UpdateList();
		
		itsLoadingActive = false;
	}
	
	/// <summary>
	/// Set filter for a column
	/// </summary>
	/// <param name="theColumnName"></param>
	/// <param name="theFilter"></param>
	public void SetFilter(string theColumnName, string theFilter)
	{
		if (SetFilterInternal(theColumnName,theFilter))
		{
			OnSettingsChanged();
		}
	}
	
	/// <summary>
	/// Clear all filters
	/// </summary>
	public void ClearFilters()
	{
		foreach (KGFObjectListColumnItem anItem in itsListFieldCache)
		{
			anItem.itsFilterString = "";
			if (anItem.itsDropDown != null)
				anItem.itsDropDown.SetSelectedItem("<NONE>");
		}
		
		itsRepaintWish = true;
		OnSettingsChanged();
	}
	
	bool SetFilterInternal(string theColumnName, string theFilter)
	{
		foreach (KGFObjectListColumnItem anItem in itsListFieldCache)
		{
			if (theColumnName == anItem.itsHeader)
			{
				anItem.itsFilterString = theFilter;
				itsRepaintWish = true;
				
				return true;
			}
		}
		return false;
	}
	
	const string itsControlSearchName = "KGFGuiObjectList.FullTextSearch";
	const string itsTextSearch = "Search";
	
	/// <summary>
	/// Render this control
	/// </summary>
	public void Render ()
	{
		if (itsUpdateWish)
		{
			UpdateList();
		}
		
		int itsNumberOfPages = (int)Math.Ceiling ((float)itsData.Rows.Count / (float)itsItemsPerPage);
		if (itsCurrentPage >= itsNumberOfPages)
			itsCurrentPage = 0;
		
		itsRepaintWish = false;
		itsGuiData.SetDisplayRowCount ((uint)itsItemsPerPage);

		KGFGUIUtility.BeginHorizontalBox (KGFGUIUtility.eStyleBox.eBoxDecorated);
		{
			// categories view
			GUILayout.BeginVertical (GUILayout.Width (180));
			{
				itsListViewCategories.Render ();
			}
			GUILayout.EndVertical ();
			
			KGFGUIUtility.SpaceSmall ();
			
			GUILayout.BeginVertical ();
			{
				// item table
				itsGuiData.SetStartRow ((uint)(itsCurrentPage * (uint)itsItemsPerPage));
				itsGuiData.Render ();
				
				KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVerticalInteractive);
				{
					// enum filter boxes
					int aColumnNo = 0;
//						KGFGUIUtility.SpaceSmall();
					foreach (KGFObjectListColumnItem anItem in itsListFieldCache)
					{
						aColumnNo++;
						
						if (!anItem.itsDisplay)
							continue;
						if (!itsGuiData.GetColumnVisible(aColumnNo))
							continue;
						
						if (anItem.itsSearchable && (anItem.GetReturnType().IsEnum ||
						                             anItem.GetReturnType() == typeof(bool) ||
						                             anItem.GetReturnType() == typeof(string)))
						{
							GUILayout.BeginHorizontal(GUILayout.Width(itsGuiData.GetColumnWidth(aColumnNo)));
							{
								KGFGUIUtility.BeginVerticalBox(KGFGUIUtility.eStyleBox.eBoxInvisible);
								DrawFilterBox (anItem,itsGuiData.GetColumnWidth(aColumnNo)-4);
								KGFGUIUtility.EndVerticalBox();
							}
							GUILayout.EndHorizontal();
							KGFGUIUtility.Separator(KGFGUIUtility.eStyleSeparator.eSeparatorVerticalFitInBox);
						}
						else
						{
							GUILayout.BeginHorizontal(GUILayout.Width(itsGuiData.GetColumnWidth(aColumnNo)));
							{
								GUILayout.Label(" ");
							}
							GUILayout.EndHorizontal();
							KGFGUIUtility.Separator(KGFGUIUtility.eStyleSeparator.eSeparatorVerticalFitInBox);
							continue;
						}
						
					}
					GUILayout.FlexibleSpace();
				}
				KGFGUIUtility.EndHorizontalBox();
				
				KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkMiddleVertical);
				{
					GUILayout.Label("");
					GUILayout.FlexibleSpace();
				}
				KGFGUIUtility.EndHorizontalBox();
				
				KGFGUIUtility.BeginVerticalBox (KGFGUIUtility.eStyleBox.eBoxDarkBottom);
				{
					GUILayout.BeginHorizontal();
					{
						if (!Application.isPlaying)
						{
							if (EventNew != null)
							{
								if (KGFGUIUtility.Button ("New", KGFGUIUtility.eStyleButton.eButton,GUILayout.Width(75)))
								{
									EventNew(this,null);
								}
							}
							if (EventDelete != null)
							{
								if (KGFGUIUtility.Button ("Delete", KGFGUIUtility.eStyleButton.eButton,GUILayout.Width(75)))
								{
									EventDelete(this,null);
								}
							}
							
							GUILayout.FlexibleSpace();
						}
						
						// full text search box
						if (itsDisplayFullTextSearch)
						{
							GUI.SetNextControlName(itsControlSearchName);
							string aNewString = KGFGUIUtility.TextField (itsFulltextSearch, KGFGUIUtility.eStyleTextField.eTextField,GUILayout.Width(200));
							if (aNewString != itsFulltextSearch)
							{
								itsFulltextSearch = aNewString;
								UpdateList ();
							}
						}
						
						KGFGUIUtility.Space();
						
						bool anIncludeAll = KGFGUIUtility.Toggle(itsIncludeAll,"all Tags",KGFGUIUtility.eStyleToggl.eTogglSuperCompact,GUILayout.Width(70));
						if(anIncludeAll != itsIncludeAll)
						{
							itsIncludeAll =	anIncludeAll;
							UpdateList();
						}
						
						if (KGFGUIUtility.Button("clear filters",KGFGUIUtility.eStyleButton.eButton,GUILayout.Width(100)))
						{
							itsFulltextSearch = "";
							ClearFilters();
							UpdateList();
						}
						
						GUILayout.FlexibleSpace();
						
						KGFGUIUtility.BeginHorizontalBox (KGFGUIUtility.eStyleBox.eBoxInvisible);
						{
							if (GetDisplayEntriesPerPage())
							{
								//number of items in List
								if (KGFGUIUtility.Button ("<", KGFGUIUtility.eStyleButton.eButtonLeft, GUILayout.Width (25)))
								{
									switch (itsItemsPerPage)
									{
										case KGFeItemsPerPage.e25:
											itsItemsPerPage = KGFeItemsPerPage.e10;
											break;
										case KGFeItemsPerPage.e50:
											itsItemsPerPage = KGFeItemsPerPage.e25;
											break;
										case KGFeItemsPerPage.e100:
											itsItemsPerPage = KGFeItemsPerPage.e50;
											break;
										case KGFeItemsPerPage.e250:
											itsItemsPerPage = KGFeItemsPerPage.e100;
											break;
										case KGFeItemsPerPage.e500:
											itsItemsPerPage = KGFeItemsPerPage.e250;
											break;
											
										default:
											break;
									}
								}
								
								KGFGUIUtility.BeginVerticalBox (KGFGUIUtility.eStyleBox.eBoxMiddleHorizontal);
								{
									string aLogsPerPageString = itsItemsPerPage.ToString ().Substring (1) + " entries per page";
									KGFGUIUtility.Label (aLogsPerPageString, KGFGUIUtility.eStyleLabel.eLabelFitIntoBox);
								}
								KGFGUIUtility.EndVerticalBox ();
								
								if (KGFGUIUtility.Button (">", KGFGUIUtility.eStyleButton.eButtonRight, GUILayout.Width (25)))
								{
									switch (itsItemsPerPage)
									{
										case KGFeItemsPerPage.e10:
											itsItemsPerPage = KGFeItemsPerPage.e25;
											break;
										case KGFeItemsPerPage.e25:
											itsItemsPerPage = KGFeItemsPerPage.e50;
											break;
										case KGFeItemsPerPage.e50:
											itsItemsPerPage = KGFeItemsPerPage.e100;
											break;
										case KGFeItemsPerPage.e100:
											itsItemsPerPage = KGFeItemsPerPage.e250;
											break;
										case KGFeItemsPerPage.e250:
											itsItemsPerPage = KGFeItemsPerPage.e500;
											break;
											
										default:
											break;
									}
								}
							}
							
							GUILayout.Space (10.0f);
							
							// page control
							if (KGFGUIUtility.Button ("<", KGFGUIUtility.eStyleButton.eButtonLeft, GUILayout.Width (25)) && itsCurrentPage > 0)
							{
								itsCurrentPage--;
							}
							
							KGFGUIUtility.BeginVerticalBox (KGFGUIUtility.eStyleBox.eBoxMiddleHorizontal);
							{
								string aString = string.Format ("page {0}/{1}", itsCurrentPage + 1, Math.Max (itsNumberOfPages, 1));
								KGFGUIUtility.Label (aString, KGFGUIUtility.eStyleLabel.eLabelFitIntoBox);
							}
							KGFGUIUtility.EndVerticalBox ();
							
							if (KGFGUIUtility.Button (">", KGFGUIUtility.eStyleButton.eButtonRight, GUILayout.Width (25)) && itsData.Rows.Count > ((itsCurrentPage + 1) * (int)itsItemsPerPage))
							{
								itsCurrentPage++;
							}
						}
						KGFGUIUtility.EndHorizontalBox();
					}
					GUILayout.EndHorizontal();
				}
				KGFGUIUtility.EndVerticalBox ();
			}
			GUILayout.EndVertical ();
		}
		KGFGUIUtility.EndHorizontalBox ();
		
		if(GUI.GetNameOfFocusedControl().Equals(itsControlSearchName))
		{
			if(itsFulltextSearch.Equals(itsTextSearch))
			{
				itsFulltextSearch = string.Empty;
			}
		}
		
		if(!GUI.GetNameOfFocusedControl().Equals(itsControlSearchName))
		{
			if(itsFulltextSearch.Equals(string.Empty))
			{
				itsFulltextSearch = itsTextSearch;
			}
		}
	}
	
	bool itsDisplayEntriesPerPage = true;
	
	/// <summary>
	/// Display the entries per page control
	/// </summary>
	/// <param name="theDisplay"></param>
	public void SetDisplayEntriesPerPage(bool theDisplay)
	{
		itsDisplayEntriesPerPage = theDisplay;
	}
	
	/// <summary>
	/// Returns TRUE, if the entries per page control will be displayed, FALSE otherwise.
	/// </summary>
	/// <returns></returns>
	public bool GetDisplayEntriesPerPage()
	{
		return itsDisplayEntriesPerPage;
	}
	
	bool itsRepaintWish = false;
	public bool GetRepaint()
	{
		return itsGuiData.GetRepaintWish() || itsRepaintWish;
	}
	#endregion
	
	#region private methods
	void OnClickRow (object theSender, EventArgs theArgs)
	{
		KGFDataRow aRow = theSender as KGFDataRow;
		
		if (aRow != null)
		{
			itsCurrentSelectedItem = (KGFITaggable)aRow [0].Value;
			if (itsCurrentSelectedRow != aRow)
			{
				itsCurrentSelectedRow = aRow;
			}
			if (EventSelect != null)
			{
				EventSelect (this, new KGFGUIObjectListSelectEventArgs (itsCurrentSelectedItem));
			}
		}
	}
	
	void OnCategoriesChanged (object theSender, EventArgs theArgs)
	{
		UpdateList ();
		OnSettingsChanged();
	}
	
	void OnSettingsChanged()
	{
		if (!itsLoadingActive)
		{
			if (EventSettingsChanged != null)
				EventSettingsChanged(this,null);
		}
	}
	
	const string UnTagged = "<untagged>";
	
	IEnumerable<string> GetAllTags ()
	{
		foreach (KGFITaggable anItem in itsListData)
		{
			if (anItem.GetTags().Length == 0)
				yield return UnTagged;
			foreach (string aTag in anItem.GetTags())
			{
				yield return aTag;
			}
		}
		yield break;
	}
	
	/// <summary>
	/// Reflect type and cache fields to display
	/// </summary>
	void CacheTypeMembers ()
	{
		itsDisplayFullTextSearch = false;
		itsData.Rows.Clear ();
		itsData.Columns.Clear ();
		itsListFieldCache.Clear ();
		
		itsData.Columns.Add (new KGFDataColumn ("DATA", itsItemType));
		
		foreach (FieldInfo aField in itsItemType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
		{
			TryAddMember(aField);
		}
		foreach (PropertyInfo aProperty in itsItemType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
		{
			TryAddMember(aProperty);
		}
	}
	
	/// <summary>
	/// Search for custom attribute on member and add the member if it has been found
	/// </summary>
	/// <param name="theMemberInfo"></param>
	void TryAddMember(MemberInfo theMemberInfo)
	{
		KGFObjectListItemDisplayAttribute[] anAttributeList = theMemberInfo.GetCustomAttributes (typeof(KGFObjectListItemDisplayAttribute), true) as KGFObjectListItemDisplayAttribute[];
		if (anAttributeList.Length == 1)
		{
			AddMember(theMemberInfo,anAttributeList [0].Header,anAttributeList [0].Searchable,anAttributeList [0].Display);
		}
	}
	
	/// <summary>
	/// Returns TRUE if filtered
	/// </summary>
	/// <param name="theItem"></param>
	/// <returns></returns>
	bool FullTextFilter(KGFITaggable theItem)
	{
		if (itsFulltextSearch.Trim() == itsTextSearch)
			return false;
		
		foreach (string aSearchPartFull in itsFulltextSearch.Trim().ToLower().Split(' '))
		{
			bool aFoundPart = false;
			string aSearchPartValue = aSearchPartFull;
			string aSearchPartName = null;
			string [] aSearchPartArr = aSearchPartFull.Split('=');
			if (aSearchPartArr.Length == 2)
			{
				aSearchPartValue = aSearchPartArr[1];
				aSearchPartName = aSearchPartArr[0];
			}
			
			foreach (KGFObjectListColumnItem aFieldItem in itsListFieldCache)
			{
				// if it is a search for a special field, only use it for that field
				if (aSearchPartName != null)
				{
					if (aFieldItem.itsHeader.ToLower() != aSearchPartName.ToLower())
						continue;
				}
				
				object aValue = aFieldItem.GetReturnValue(theItem);//itsFieldInfo.GetValue (theItem);
				
				if (aFieldItem.itsSearchable)
				{
					if (aValue is IEnumerable && !(aValue is string))
					{
						foreach (object aValueItem in (IEnumerable)aValue)
						{
							if (aValueItem == null)
								continue;
							if (aValueItem.ToString().Trim().ToLower().Contains(aSearchPartValue))
							{
								aFoundPart = true;
							}
						}
					}
					else
					{
						string aValueString = aValue.ToString();
						if (aValueString.Trim ().ToLower ().Contains (aSearchPartValue))
						{
							aFoundPart = true;
						}
					}
				}
			}
			
			if (!aFoundPart)
				return true;
		}
		return false;
	}

	/// <summary>
	/// Returns TRUE if filtered
	/// </summary>
	/// <param name="theItem"></param>
	/// <returns></returns>
	bool PerItemFilter(KGFITaggable theItem)
	{
		foreach (KGFObjectListColumnItem aFieldItem in itsListFieldCache)
		{
			object aValue = aFieldItem.GetReturnValue (theItem);
			// check item filtering
			if (aFieldItem.GetIsFiltered (aValue))
			{
				return true;
			}
		}
		return false;
	}

	bool itsUpdateWish = false;
	
	/// <summary>
	/// Update contents of this list
	/// </summary>
	void UpdateList ()
	{
		if (Event.current != null)
		{
			if (Event.current.type != EventType.Layout)
			{
				itsUpdateWish = true;
				return;
			}
		}
		itsUpdateWish = false;
		
		itsData.Rows.Clear ();
		
		// for every item in original data
		foreach (KGFITaggable anItem in itsListData)
		{
			// category filter
			if (!GetIsTagSelected (anItem.GetTags()))
			{
				continue;
			}
			
			// check full text search
			if (!string.IsNullOrEmpty (itsFulltextSearch))
			{
				if (FullTextFilter(anItem))
				{
					continue;
				}
			}
			
			// per item filter
			if (PerItemFilter(anItem))
			{
				continue;
			}
			
			// add new row
			KGFDataRow aRow = itsData.NewRow ();
			aRow [0].Value = anItem;
			int aRowIndex = 1;
			foreach (KGFObjectListColumnItem aFieldItem in itsListFieldCache)
			{
				object aValue = aFieldItem.GetReturnValue(anItem);
				// only add content if it should be displayed
//				if (aFieldItem.itsDisplay)
				{
					aRow [aRowIndex].Value = aValue;
					aRowIndex ++;
				}
			}
			itsData.Rows.Add (aRow);
		}
	}

	bool GetIsTagSelected (string []theTags)
	{
		List<object> aListOfCategories = new List<object>(itsListViewCategories.GetSelected());
		int aNumberOfSelectedTags = aListOfCategories.Count;
		
//		if(aNumberOfSelectedTags == 0 && theTags.Length == 0)
//			return true;
		
		int aNumberOfMatchingTags = 0;
		
		foreach (string anItem in itsListViewCategories.GetSelected())
		{
			if (theTags.Length == 0 && anItem == UnTagged)
			{
				if (itsIncludeAll)
					aNumberOfMatchingTags++;
				else
					return true;
			}
			foreach (string aTag in theTags)
			{
				if (aTag == anItem)
				{
					if(itsIncludeAll)
					{
						aNumberOfMatchingTags++;
					}
					else	//return true on the first matching tag
					{
						return true;
					}
				}
			}
		}
		
		if(aNumberOfMatchingTags == aNumberOfSelectedTags && itsIncludeAll)
		{
			return true;
		}
		
		return false;
	}

	void OnDropDownValueChanged (object theSender, EventArgs theArgs)
	{
		UpdateList ();
		OnSettingsChanged();
	}

	string[] itsBoolValues = new string[]{"True","False"};
	void DrawFilterBox (KGFObjectListColumnItem theItem, uint theWidth)
	{
		if (theItem.GetReturnType().IsEnum ||theItem.GetReturnType() == typeof(bool))
		{
			if (theItem.itsDropDown == null)
			{
				if (theItem.GetReturnType() == typeof(bool))
				{
					theItem.itsDropDown = new KGFGUIDropDown ((new List<string>(itsBoolValues)).InsertItem (NONE_STRING, 0), theWidth, 5, KGFGUIDropDown.eDropDirection.eUp);
				}else if (theItem.GetReturnType().IsEnum)
				{
					theItem.itsDropDown = new KGFGUIDropDown (Enum.GetNames (theItem.GetReturnType()).InsertItem (NONE_STRING, 0), theWidth, 5, KGFGUIDropDown.eDropDirection.eUp);
				}
				
				theItem.itsDropDown.itsTitle = "";
				theItem.itsDropDown.SetSelectedItem(theItem.itsFilterString);
				theItem.itsDropDown.SelectedValueChanged += OnDropDownValueChanged;
			}
			theItem.itsDropDown.Render ();
		}
		else if (theItem.GetReturnType() == typeof(string))
		{
			if (theItem.itsFilterString == null)
				theItem.itsFilterString = "";
			
			string aFilter = KGFGUIUtility.TextField(theItem.itsFilterString,KGFGUIUtility.eStyleTextField.eTextField,GUILayout.Width(theWidth));
			if (aFilter != theItem.itsFilterString)
			{
				theItem.itsFilterString = aFilter;
				UpdateList ();
				OnSettingsChanged();
			}
		}
	}
	#endregion
}
