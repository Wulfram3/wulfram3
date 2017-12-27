// <author>Michal Kolasinski</author>
// <email>michal.kolasinski@kolmich.at</email>
// <date>2010-05-28</date>
// <summary>short summary</summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Used to fire multiple events at once or separated by time intervals.
/// </summary>
[System.Serializable]
public class KGFEventSequence : KGFEventBase, KGFIValidator
{
	const string itsEventCategory = "KGFEventSystem";

	[System.Serializable]
	public class KGFEventSequenceEntry
	{
		public float itsWaitBefore;
		public KGFEventBase itsEvent;
		public float itsWaitAfter;
	}
	
	public List<KGFEventSequenceEntry> itsEntries = new List<KGFEventSequenceEntry>();
	
	/// <summary>
	/// static global member indicating if a sequence is running
	/// every time an event sequence starts this is set to true else to false.
	/// </summary>
	static List<KGFEventSequence> itsListOfRunningSequences = new List<KGFEventSequence>();

	/// <summary>
	/// local member indicating if the sequence is running
	/// </summary>
	bool itsEventSequenceRunning = false;
	
	/// <summary>
	/// All event sequences
	/// </summary>
	static List<KGFEventSequence> itsListOfSequencesAll = new List<KGFEventSequence>();
	
	/// <summary>
	/// Unity3d awake
	/// </summary>
	protected override void KGFAwake()
	{
		itsListOfSequencesAll.Add(this);
	}
	
	private static bool itsStepMode = false;
	private int itsStayBeforeStepID = 0;
	
	public void Step()
	{
		itsStayBeforeStepID++;
	}
	
	public void Finish()
	{
		itsStayBeforeStepID = itsEntries.Count+1;
	}
	
	/// <summary>
	/// Get status of step mode
	/// </summary>
	/// <returns></returns>
	public static bool GetSingleStepMode()
	{
		return itsStepMode;
	}
	
	public bool IsWaitingForDebugInput()
	{
//		return true;
		return (itsStayBeforeStepID == itsEventDoneCounter);
	}
	
	public int GetCurrentStepNumber()
	{
		return itsEventDoneCounter.GetValueOrDefault(0);
	}
	
	public int GetStepCount()
	{
		return itsEntries.Count;
	}
	
	/// <summary>
	/// Set single step mode
	/// </summary>
	/// <param name="theActivateStepMode"></param>
	public static void SetSingleStepMode(bool theActivateStepMode)
	{
		itsStepMode = theActivateStepMode;
	}
	
	/// <summary>
	/// Get all event_sequences
	/// </summary>
	/// <returns></returns>
	public static KGFEventSequence[] GetAllSequences()
	{
		return itsListOfSequencesAll.ToArray();
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public static IEnumerable<KGFEventSequence> GetQueuedSequences()
	{
		for (int i=itsListOfSequencesAll.Count-1;i>=0;i--)
		{
			KGFEventSequence aSequence = itsListOfSequencesAll[i];
			if (aSequence == null)
			{
				itsListOfSequencesAll.RemoveAt(i);
				continue;
			}
			if (aSequence.gameObject == null)
			{
				itsListOfSequencesAll.RemoveAt(i);
				continue;
			}
			if (aSequence.IsQueued())
				yield return aSequence;
		}
		yield break;
	}
	
	public static int GetNumberOfRunningSequences()
	{
		return itsListOfRunningSequences.Count;
	}
	
	/// <summary>
	/// Get list of all running event_sequences
	/// </summary>
	/// <returns></returns>
	public static KGFEventSequence[] GetRunningEventSequences()
	{
		return itsListOfRunningSequences.ToArray();
	}
	
	public void InitList()
	{
		if(itsEntries.Count == 0)
		{
			itsEntries.Add(new KGFEventSequenceEntry());
		}
	}
	
	public void Insert(KGFEventSequenceEntry theElementAfterToInsert,KGFEventSequenceEntry theElementToInsert)
	{
		int anIndex = itsEntries.IndexOf(theElementAfterToInsert);
		itsEntries.Insert(anIndex+1,theElementToInsert);
	}
	
	public void Delete(KGFEventSequenceEntry theElementToDelete)
	{
		if(itsEntries.Count > 1)
			itsEntries.Remove(theElementToDelete);
	}
	
	public void MoveUp(KGFEventSequenceEntry theElementToMoveUp)
	{
		int anIndex = itsEntries.IndexOf(theElementToMoveUp);
		if(anIndex <= 0)
		{
			KGFEvent.LogWarning("cannot move up element at 0 index",itsEventCategory,this);
			return;
		}
		Delete(theElementToMoveUp);
		itsEntries.Insert(anIndex-1,theElementToMoveUp);
	}
	
	public void MoveDown(KGFEventSequenceEntry theElementToMoveDown)
	{
		int anIndex = itsEntries.IndexOf(theElementToMoveDown);
		if(anIndex >= itsEntries.Count-1)
		{
			KGFEvent.LogWarning("cannot move down element at end index",itsEventCategory,this);
			return;
		}
		Delete(theElementToMoveDown);
		itsEntries.Insert(anIndex+1,theElementToMoveDown);
	}
	
	/// <summary>
	/// TRUE, if the sequence is currently running
	/// </summary>
	/// <returns></returns>
	public bool IsRunning()
	{
		return itsEventSequenceRunning;
	}
	
	/// <summary>
	/// TRUE, if the sequence is currently queued
	/// </summary>
	/// <returns></returns>
	public bool IsQueued()
	{
		return itsEventDoneCounter != null && !itsEventSequenceRunning;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public string GetNextExecutedJobItem()
	{
		if (itsEventDoneCounter != null)
		{
			if (itsEventDoneCounter.GetValueOrDefault() < itsEntries.Count)
				return itsEntries[itsEventDoneCounter.GetValueOrDefault()].itsEvent.name;
			else
				return "finished";
		}else
			return "not running";
	}
	
	/// <summary>
	/// Starts event sequence
	/// </summary>
	[KGFEventExpose]
	public override void Trigger ()
	{
		// reset counter
		itsEventDoneCounter = 0;
		
		#if UNITY_4_0
		if (gameObject.activeSelf)
		#else
		if (gameObject.active)
		#endif
		{
			itsEventSequenceRunning = true;
			KGFEvent.LogDebug("Start: "+gameObject.name,itsEventCategory,this);
			StartCoroutine ("StartSequence");
		}else
			KGFEvent.LogDebug("Queued: "+gameObject.name,itsEventCategory,this);
	}
	
	/// <summary>
	/// Stop the execution of the event sequence
	/// </summary>
	[KGFEventExpose]
	public void StopSequence()
	{
		StopCoroutine("StartSequence");
		itsEventSequenceRunning = false;
		itsEventDoneCounter = null;
		if (itsListOfRunningSequences.Contains(this))
			itsListOfRunningSequences.Remove(this);
	}

	/// <summary>
	/// The sequence waits for itsWaits[i] seconds and fires itsEvents[i]
	/// </summary>
	/// <returns>
	/// A <see cref="IEnumerator"/>
	/// </returns>
	IEnumerator StartSequence ()
	{
		itsStayBeforeStepID = 0;
		if (!itsListOfRunningSequences.Contains(this))
			itsListOfRunningSequences.Add(this);
		
		// do not start sequnce if counter is not set
		if (itsEventDoneCounter == null)
			yield break;
		// for all entries, start at the current counter position
		for (int i = itsEventDoneCounter.GetValueOrDefault(0);i< itsEntries.Count; i++)
		{
			KGFEventSequenceEntry anEntry = itsEntries[i];
			// wait some time
			if (anEntry.itsWaitBefore > 0)
				yield return new WaitForSeconds (anEntry.itsWaitBefore);
			
			#if UNITY_EDITOR
			// only for debug
			while (itsStepMode && i>=itsStayBeforeStepID)
				yield return new WaitForSeconds(0.2f);
			#endif
			
//			if(itsEventSequenceRunning == false)	//event sequence and coroutine was stopped. This here should never happen. -> just to make really sure!
//				Debugger.LogError(this,"continuing stopped coroutine!!!");
			
			// trigger event
			try{
				if (anEntry.itsEvent != null)
				{
					anEntry.itsEvent.Trigger();
				}
				else
					KGFEvent.LogError("events have null entries",itsEventCategory,this);
			}
			catch(System.Exception e)
			{
				KGFEvent.LogError("Exception in event_sequence:"+e,itsEventCategory,this);
			}
			// increase counter
			itsEventDoneCounter = i+1;
			
			// wait some time
			if (anEntry.itsWaitAfter > 0)
				yield return new WaitForSeconds (anEntry.itsWaitAfter);
		}
		// set counter to inactive
		itsEventDoneCounter = null;
		itsEventSequenceRunning = false;
		// remove from running list
		if (itsListOfRunningSequences.Contains(this))
			itsListOfRunningSequences.Remove(this);
	}
	
	/// <summary>
	/// Saves the number of the events already done in the sequence
	/// </summary>
	private int? itsEventDoneCounter = null;
	
	/// <summary>
	/// 
	/// </summary>
	void OnDestruct()
	{
		StopSequence();
	}
	
	/// <summary>
	/// Checks for errors in the inspector
	/// </summary>
	public override KGFMessageList Validate()
	{
		KGFMessageList aReturnValue = new KGFMessageList();
		
		bool aHasNullEvent = false;
		bool aWaitBeforeError = false;
		bool aWaitAfterError = false;

		if (itsEntries != null)
		{
			for (int i=0; i<itsEntries.Count; i++)
			{
				KGFEventSequenceEntry anEntry = itsEntries[i];
				if (anEntry.itsEvent == null)
					aHasNullEvent = true;
				if (anEntry.itsWaitBefore < 0)
					aWaitBeforeError = true;
				if (anEntry.itsWaitAfter < 0)
					aWaitAfterError = true;
			}
		}

		if (aHasNullEvent)
			aReturnValue.AddError("sequence entry has null event");
		if (aWaitBeforeError)
			aReturnValue.AddError("sequence entry itsWaitBefore <= 0");
		if (aWaitAfterError)
			aReturnValue.AddError("sequence entry itsWaitAfter <= 0");
		
		return aReturnValue;
	}
}