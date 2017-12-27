// <author>Christoph Hausjell</author>
// <email>christoph.hausjell@kolmich.at</email>
// <date>2012-03-13</date>
// <summary>short summary</summary>

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class KGFGUIDataTable : KGFIControl
{
	private KGFDataTable itsDataTable;
	private Vector2 itsDataTableScrollViewPosition;
	private uint itsStartRow = 0;
	private uint itsDisplayRowCount = 100;
	private Dictionary<KGFDataColumn, uint> itsColumnWidth = new Dictionary<KGFDataColumn, uint>();
	private Dictionary<KGFDataColumn, bool> itsColumnVisible = new Dictionary<KGFDataColumn, bool>();
	private KGFDataRow itsClickedRow = null;
	private KGFDataRow itsCurrentSelected = null;
	private bool itsVisible = true;
	
	public event EventHandler PreRenderRow;
	public event EventHandler PostRenderRow;
	public event EventHandler PreRenderColumn;
	public event EventHandler PostRenderColumn;
	public event Func<KGFDataRow,KGFDataColumn,uint,bool> PreCellContentHandler;
	public event EventHandler OnClickRow;
	public event EventHandler EventSettingsChanged;
	
	public KGFGUIDataTable(KGFDataTable theDataTable, params GUILayoutOption[] theLayout)
	{
		itsDataTable = theDataTable;
		
		// add the column width auto to all columns
		foreach(KGFDataColumn aColumn in itsDataTable.Columns)
		{
			itsColumnWidth.Add(aColumn, 0);
			itsColumnVisible.Add(aColumn, true);
		}
	}
	
	static Texture2D itsTextureArrowUp = null;
	static Texture2D itsTextureArrowDown = null;

	static void LoadTextures()
	{
		string aTexturePath = "KGFCore/textures/";

		itsTextureArrowUp   = (Texture2D)Resources.Load(aTexturePath + "arrow_up",typeof(Texture2D));
		itsTextureArrowDown = (Texture2D)Resources.Load(aTexturePath + "arrow_down",typeof(Texture2D));
	}
	
	public uint GetStartRow()
	{
		return itsStartRow;
	}
	
	public void SetStartRow(uint theStartRow)
	{
		itsStartRow = (uint)Math.Min(theStartRow, itsDataTable.Rows.Count);
	}
	
	public uint GetDisplayRowCount()
	{
		return itsDisplayRowCount;
	}
	
	public void SetDisplayRowCount(uint theDisplayRowCount)
	{
		itsDisplayRowCount = (uint)Math.Min(theDisplayRowCount, itsDataTable.Rows.Count - itsStartRow);
	}
	
	public void SetColumnVisible(int theColumIndex, bool theValue)
	{
		if(theColumIndex >= 0 && theColumIndex < itsDataTable.Columns.Count)
		{
			itsColumnVisible[itsDataTable.Columns[theColumIndex]] = theValue;
		}
	}
	
	public bool GetColumnVisible(int theColumIndex)
	{
		if(theColumIndex >= 0 && theColumIndex < itsDataTable.Columns.Count)
		{
			return itsColumnVisible[itsDataTable.Columns[theColumIndex]];
		}
		
		return false;
	}
	
	public void SetColumnWidth(int theColumIndex, uint theValue)
	{
		if(theColumIndex >= 0 && theColumIndex < itsDataTable.Columns.Count)
		{
			itsColumnWidth[itsDataTable.Columns[theColumIndex]] = theValue;
		}
	}
	
	public uint GetColumnWidth(int theColumIndex)
	{
		if(theColumIndex >= 0 && theColumIndex < itsDataTable.Columns.Count)
		{
			return itsColumnWidth[itsDataTable.Columns[theColumIndex]];
		}
		
		return 0;
	}
	
	public KGFDataRow GetCurrentSelected()
	{
		return itsCurrentSelected;
	}
	
	public void SetCurrentSelected(KGFDataRow theDataRow)
	{
		if(itsDataTable.Rows.Contains(theDataRow))
		{
			itsCurrentSelected = theDataRow;
		}
	}
	
	#region Render functions
	
	private void RenderTableHeadings()
	{
		if (itsTextureArrowDown == null)
		{
			LoadTextures();
		}
		
		KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkTop, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));
		{
			foreach(KGFDataColumn aColumn in itsDataTable.Columns)
			{
				// check if visible is set to true
				if(itsColumnVisible[aColumn])
				{
					GUILayoutOption[] anOptions;
					
					// check if the width is fixed size
					if(itsColumnWidth[aColumn] != 0)
					{
						anOptions = new GUILayoutOption[]{GUILayout.Width(itsColumnWidth[aColumn])};
					}
					else
					{
						anOptions = new GUILayoutOption[]{GUILayout.ExpandWidth(true)};
					}
					
					GUILayout.BeginHorizontal(anOptions);
					{
						KGFGUIUtility.Label(aColumn.ColumnName,KGFGUIUtility.eStyleLabel.eLabelFitIntoBox);
						
						if (aColumn == itsSortColumn)
						{
							if (itsSortDirection)
							{
								KGFGUIUtility.Label("",itsTextureArrowDown,KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(14));
							}else
							{
								KGFGUIUtility.Label("",itsTextureArrowUp,KGFGUIUtility.eStyleLabel.eLabelFitIntoBox,GUILayout.Width(14));
							}
						}
					}
					GUILayout.EndHorizontal();
					
					if (Event.current.type == EventType.MouseUp &&
					    GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
					{
						SortColumn(aColumn);
					}
					
					KGFGUIUtility.Separator(KGFGUIUtility.eStyleSeparator.eSeparatorVerticalFitInBox);
				}
			}
		}
		KGFGUIUtility.EndHorizontalBox();
	}
	
	KGFDataColumn itsSortColumn = null;
	bool itsSortDirection = false;
	void SortColumn(KGFDataColumn theColumn)
	{
		if (itsSortColumn != theColumn)
		{
			SetSortingColumn(theColumn);
			itsSortDirection = false;
			itsDataTable.Rows.Sort(RowComparison);
		}else
		{
			itsSortDirection = !itsSortDirection;
			itsDataTable.Rows.Reverse();
		}
		itsRepaint = true;
	}
	
	int RowComparison(KGFDataRow theRow1, KGFDataRow theRow2)
	{
		if (itsSortColumn != null)
		{
			return theRow1[itsSortColumn].Value.ToString().CompareTo(theRow2[itsSortColumn].Value.ToString());
		}
		return 0;
	}
	
	private void RenderTableRows()
	{
		itsDataTableScrollViewPosition = KGFGUIUtility.BeginScrollView(itsDataTableScrollViewPosition, false, true, GUILayout.ExpandHeight(true));
		{
			//Log List Heading
//			RenderTableHeadings();
			
			if(itsDataTable.Rows.Count > 0)
			{
				GUILayout.BeginVertical();
				{
					Color aDefaultColor = GUI.color;
					
					for(int aRowIndex = (int)itsStartRow; aRowIndex < itsStartRow + itsDisplayRowCount && aRowIndex < itsDataTable.Rows.Count; aRowIndex++)
					{
						KGFDataRow aRow = itsDataTable.Rows[aRowIndex];
						
						//Pre Row Hook
						if(PreRenderRow != null)
						{
							PreRenderRow(aRow, EventArgs.Empty);
						}
						
						if(aRow == itsCurrentSelected)
						{
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxDarkTopInteractive, GUILayout.ExpandWidth(true));
						}
						else
						{
							KGFGUIUtility.BeginHorizontalBox(KGFGUIUtility.eStyleBox.eBoxMiddleVerticalInteractive, GUILayout.ExpandWidth(true));
						}
						
						//Row
						{
							foreach(KGFDataColumn aColumn in itsDataTable.Columns)
							{
								//check if the column is visible
								if(itsColumnVisible[aColumn])
								{
									//Pre Column Hook
									if(PreRenderColumn != null)
									{
										PreRenderColumn(aColumn, EventArgs.Empty);
									}
									
									bool aCustomDrawer = false;
									if(PreCellContentHandler != null)
									{
										aCustomDrawer = PreCellContentHandler(aRow, aColumn,itsColumnWidth[aColumn]);
									}
									
									if(!aCustomDrawer)
									{
										// create the string
										int itsStringMaxLenght = 85;
										string aString = aRow[aColumn].ToString().Substring(0, Math.Min(itsStringMaxLenght,aRow[aColumn].ToString().Length));
										
										if(aString.Length == itsStringMaxLenght)
										{
											aString += "...";
										}
										
										if(itsColumnWidth[aColumn] > 0)
										{
											KGFGUIUtility.Label(aString, KGFGUIUtility.eStyleLabel.eLabelFitIntoBox, GUILayout.Width(itsColumnWidth[aColumn]));
										}
										else
										{
											KGFGUIUtility.Label(aString, KGFGUIUtility.eStyleLabel.eLabelFitIntoBox, GUILayout.ExpandWidth(true));
										}
									}
									
									KGFGUIUtility.Separator(KGFGUIUtility.eStyleSeparator.eSeparatorVerticalFitInBox);

									//Post Column Hook
									if(PostRenderColumn != null)
									{
										PostRenderColumn(aColumn, EventArgs.Empty);
									}
								}
							}
						}
						KGFGUIUtility.EndHorizontalBox();
						
						//check if the rect contains the mouse and the pressed mouse button is the left mouse button
						if(GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
						{
							itsClickedRow = aRow;
							itsRepaint = true;
						}
						
						//only send this event on layouting
						if(OnClickRow != null && itsClickedRow != null && Event.current.type == EventType.Layout)
						{
							if(itsCurrentSelected != itsClickedRow)
							{
								itsCurrentSelected = itsClickedRow;
								//Debug.Log("itsCurrentSelected is set");
							}
							else
							{
								itsCurrentSelected = null;
							}
							
							OnClickRow(itsClickedRow, EventArgs.Empty);
							itsClickedRow = null;
							//Debug.Log("itsClickedRow is set to null");
						}
						
						//Post Row Hook
						if(PostRenderRow != null)
						{
							PostRenderRow(aRow, EventArgs.Empty);
						}
					}
					GUI.color = aDefaultColor;
					GUILayout.FlexibleSpace();
				}
				GUILayout.EndVertical();
			}
			else
			{
				GUILayout.Label("no items found");
				GUILayout.FlexibleSpace();
			}
		}
		GUILayout.EndScrollView();
		itsRectScrollView = GUILayoutUtility.GetLastRect();
	}
	
	Rect itsRectScrollView = new Rect();
	
	public Rect GetLastRectScrollview()
	{
		return itsRectScrollView;
	}
	
	bool itsRepaint = false;
	public bool GetRepaintWish()
	{
		bool aReturnValue = itsRepaint;
		itsRepaint = false;
		return aReturnValue;
	}
	
	/// <summary>
	/// Set sorting column by name
	/// </summary>
	/// <param name="theColumn"></param>
	public void SetSortingColumn(string theColumnName)
	{
		foreach (KGFDataColumn aColumn in itsDataTable.Columns)
		{
			if (aColumn.ColumnName == theColumnName)
			{
				itsSortColumn = aColumn;
				itsRepaint = true;
				break;
			}
		}
	}
	
	/// <summary>
	/// Set sorting column.
	/// </summary>
	/// <param name="theColumn"></param>
	public void SetSortingColumn(KGFDataColumn theColumn)
	{
		itsSortColumn = theColumn;
		itsRepaint = true;
		
		if (EventSettingsChanged != null)
			EventSettingsChanged(this,null);
	}
	
	/// <summary>
	/// Get current sorting column
	/// </summary>
	/// <returns></returns>
	public KGFDataColumn GetSortingColumn()
	{
		return itsSortColumn;
	}
	
	/// <summary>
	/// Save settings to string
	/// </summary>
	/// <returns></returns>
	public string SaveSettings()
	{
		return string.Format("SortBy:"+((itsSortColumn!=null)?itsSortColumn.ColumnName:""));
	}
	
	/// <summary>
	/// Load settings from string
	/// </summary>
	/// <param name="theSettingsString"></param>
	public void LoadSettings(string theSettingsString)
	{
		string []aSettingsArr = theSettingsString.Split(':');
		if (aSettingsArr.Length == 2)
		{
			if (aSettingsArr[0] == "SortBy")
			{
				if (aSettingsArr[1].Trim() == string.Empty)
					SetSortingColumn((KGFDataColumn)null);
				else
					SetSortingColumn(aSettingsArr[1]);
			}
		}
	}
	
	#endregion
	
	/*
	private void OnDataColumnChanged(object theSender, KGFCollectionChangeEventArgs theArgument)
	{
		KGFDataColumn aColumn = theArgument.Element as KGFDataColumn;
		
		if(theArgument.Action == CollectionChangeAction.Add)
		{
			itsColumnVisible.Add(aColumn, true);
			itsColumnWidth.Add(aColumn, 0);
		}
		else if(theArgument.Action == CollectionChangeAction.Remove)
		{
			if(aColumn != null)
			{
				itsColumnVisible.Remove(aColumn);
				itsColumnWidth.Remove(aColumn);
			}
		}
	}
	 */
	
	#region KGFIGUIControl
	
	public void Render()
	{
		if(itsVisible)
		{
			//Log List
			GUILayout.BeginVertical();
			{
				//Log List Heading
				RenderTableHeadings();
				
				//Log List
				RenderTableRows();
			}
			GUILayout.EndVertical();
		}
	}
	
	public string GetName()
	{
		return "KGFGUIDataTable";
	}
	
	public bool IsVisible()
	{
		return itsVisible;
	}
	
	#endregion
}