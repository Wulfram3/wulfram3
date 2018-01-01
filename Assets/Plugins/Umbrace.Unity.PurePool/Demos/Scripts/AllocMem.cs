using System;
using System.Text;

using UnityEngine;

/// <summary>
/// Modified version of the Allocation Stats script from the Unity wiki (http://wiki.unity3d.com/index.php/AllocationStats).
/// </summary>
[ExecuteInEditMode]
public class AllocMem : MonoBehaviour {

	public bool Show = true;
	public bool ShowFPS = false;
	public bool ShowInEditor = false;

	private float lastCollect;
	private int lastCollectNum;
	private float delta;
	private float lastDeltaTime;
	private int allocRate;
	private int lastAllocMemory;
	private float lastAllocSet = -9999;
	private int allocMem;
	private int collectAlloc;
	private int peakAlloc;

	private readonly StringBuilder text = new StringBuilder();

	private void Start() {
		this.useGUILayout = false;
	}

	private void OnGUI() {
		if (!this.Show || (!Application.isPlaying && !this.ShowInEditor)) {
			return;
		}

		int collCount = GC.CollectionCount(0);

		if (this.lastCollectNum != collCount) {
			this.lastCollectNum = collCount;
			this.delta = Time.realtimeSinceStartup - this.lastCollect;
			this.lastCollect = Time.realtimeSinceStartup;
			this.lastDeltaTime = Time.deltaTime;
			this.collectAlloc = this.allocMem;
		}

		this.allocMem = (int)GC.GetTotalMemory(false);

		this.peakAlloc = this.allocMem > this.peakAlloc ? this.allocMem : this.peakAlloc;

		if (Time.realtimeSinceStartup - this.lastAllocSet >= 1) {
			int diff = this.allocMem - this.lastAllocMemory;
			this.lastAllocMemory = this.allocMem;
			this.lastAllocSet = Time.realtimeSinceStartup;

			if (diff >= 0) {
				this.allocRate = diff;
			}
		}

		this.text.Length = 0;

		this.text.Append("Currently allocated		").Append(Mathf.Round(this.allocMem / 1000000F)).Append(" MB\n");
		this.text.Append("Peak allocated		").Append(Mathf.Round(this.peakAlloc / 1000000F)).Append(" MB (last collect ").Append(Mathf.Round(this.collectAlloc / 1000000F)).Append(" MB)\n");
		this.text.Append("Allocation rate		").Append(Math.Round(this.allocRate / 1000000F, 1)).Append(" MB/S\n");
		this.text.Append("Collection frequency		").Append(Math.Round(this.delta, 2)).Append(" S\n");
		this.text.Append("Last collect delta		").Append(Math.Round(this.lastDeltaTime, 3)).Append(" S (").Append(Math.Round(1F / this.lastDeltaTime, 1)).Append(" FPS)");

		if (this.ShowFPS) {
			this.text.AppendLine().Append(Math.Round(1F / Time.deltaTime, 1)).Append(" FPS");
		}

		GUI.Box(new Rect(5, 5, 310, 80 + (this.ShowFPS ? 16 : 0)), "");
		GUI.Label(new Rect(10, 5, 1000, 200), this.text.ToString());
	}

}