// <author>Michal Kolasinski</author>
// <email>michal.kolasinski@kolmich.at</email>
// <date>2010-05-28</date>
// <summary>short summary</summary>

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;
using System;

[CustomEditor(typeof(KGFEventSequence))]
public class KGFEventSequenceEditor : KGFEditor
{
	private KGFEventSequence itsTarget;
	
	/// <summary>
	/// Cache target object on init
	/// </summary>
	void OnEnable ()
	{
		itsTarget = (KGFEventSequence)target;
		itsTarget.InitList();
	}
	
	/// <summary>
	/// Rename the target KGVEvent
	/// </summary>
	protected virtual void RenameTarget(UnityEngine.Object theTarget)
	{
	}
	
	/// <summary>
	/// Draw event sequence
	/// </summary>
	protected override void CustomGui()
	{
		base.CustomGui();
		EditorGUILayout.BeginVertical ();
		DrawEventArray ();
		EditorGUILayout.EndVertical ();
		RenameTarget(target);
	}
	
	/// <summary>
	/// Draw sequene items
	/// </summary>
	void DrawEventArray ()
	{
		EditorGUILayout.Separator();
		if(itsTarget.itsEntries.Count != 0)
		{
			for (int i = 0; i < itsTarget.itsEntries.Count; i++)
			{
				EditorGUILayout.Separator ();
				
				// change color on error
				KGFGUIUtilityEditor.SetColorDefault();
				if(itsTarget.itsEntries[i].itsEvent == null || itsTarget.itsEntries[i].itsWaitBefore < 0 || itsTarget.itsEntries[i].itsWaitAfter < 0)
					KGFGUIUtilityEditor.SetColorError();
				
				EditorGUILayout.BeginHorizontal (GUI.skin.box,GUILayout.Height(60));
				{
					// draw navigation buttons
					EditorGUILayout.BeginVertical(GUILayout.Width(40));
					{
						if(i != 0)
						{
							if(GUILayout.Button("up",GUILayout.ExpandHeight(true),GUILayout.Width(35)))
							{
								itsTarget.MoveUp(itsTarget.itsEntries[i]);
							}
						}
						if(i != itsTarget.itsEntries.Count-1)
						{
							if(GUILayout.Button("down",GUILayout.ExpandHeight(true),GUILayout.Width(35)))
							{
								itsTarget.MoveDown(itsTarget.itsEntries[i]);
							}
						}
					}
					EditorGUILayout.EndVertical();
					
					// draw input fields
					EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
					{
						float aValue = EditorGUILayout.FloatField ("itsWaitBefore: ", itsTarget.itsEntries[i].itsWaitBefore,GUILayout.Height(20));
						if (aValue != itsTarget.itsEntries[i].itsWaitBefore)
						{
							itsTarget.itsEntries[i].itsWaitBefore = aValue;
							EditorUtility.SetDirty(itsTarget);
						}
						KGFEventBase anEvent = (KGFEventBase)EditorGUILayout.ObjectField ("itsEvent " + i, itsTarget.itsEntries[i].itsEvent, typeof(KGFEventBase),true,GUILayout.Height(20));
						if (anEvent != itsTarget.itsEntries[i].itsEvent)
						{
							itsTarget.itsEntries[i].itsEvent = anEvent;
							EditorUtility.SetDirty(itsTarget);
						}
						
						aValue = EditorGUILayout.FloatField ("itsWaitAfter: ", itsTarget.itsEntries[i].itsWaitAfter,GUILayout.Height(20));
						if (aValue != itsTarget.itsEntries[i].itsWaitAfter)
						{
							itsTarget.itsEntries[i].itsWaitAfter = aValue;
							EditorUtility.SetDirty(itsTarget);
						}
					}
					EditorGUILayout.EndVertical();
					
					// draw command buttons
					EditorGUILayout.BeginVertical(GUILayout.Width(40));
					{
						if(GUILayout.Button("new",GUILayout.ExpandHeight(true),GUILayout.Width(35)))
						{
							itsTarget.Insert(itsTarget.itsEntries[i],new KGFEventSequence.KGFEventSequenceEntry());
						}
						if(i != 0)
						{
							if(GUILayout.Button("del",GUILayout.ExpandHeight(true),GUILayout.Width(35)))
							{
								itsTarget.Delete(itsTarget.itsEntries[i]);
							}
						}
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndHorizontal();
			}
		}
		
		EditorGUILayout.Separator();
	}
}
