//please uncomment the following line if you own the PlayMaker package
//#define PLAYMAKER

using UnityEngine;
using System.Collections;
using System;

#if PLAYMAKER

using HutongGames.PlayMaker;

#region KGFMapSystem
[ActionCategory("KGFMapSystem")]
public class KGFMapSystemSetModeStatic : FsmStateAction
{
	public KGFMapSystem MapSystem;
	public FsmBool FullScreenMode;
	public override void Reset ()
	{
		MapSystem = null;
	}
	public override void OnEnter ()
	{
		if(MapSystem == null)
		{
			MapSystem = KGFAccessor.GetObject<KGFMapSystem>();
		}
		if (MapSystem != null)
		{
			MapSystem.SetModeStatic(FullScreenMode.Value);
		}else
		{
			LogError("Could not find mapsystem object in scene");
		}
		Finish();
	}
}

[ActionCategory("KGFMapSystem")]
public class KGFMapSystemSetModeFullscreen : FsmStateAction
{
	public KGFMapSystem MapSystem;
	public FsmBool Visibility;
	public override void Reset ()
	{
		MapSystem = null;
	}
	public override void OnEnter ()
	{
		if(MapSystem == null)
		{
			MapSystem = KGFAccessor.GetObject<KGFMapSystem>();
		}
		if (MapSystem != null)
		{
			MapSystem.SetFullscreen(Visibility.Value);
		}else
		{
			LogError("Could not find mapsystem object in scene");
		}
		Finish();
	}
}

[ActionCategory("KGFMapSystem")]
public class KGFMapSystemSetIconsVisibleByCategory : FsmStateAction
{
	public KGFMapSystem MapSystem;
	[RequiredField]
	public FsmString IconCategory;
	public FsmBool ZoomValue;
	public override void Reset ()
	{
		MapSystem = null;
	}
	public override void OnEnter ()
	{
		if(MapSystem == null)
		{
			MapSystem = KGFAccessor.GetObject<KGFMapSystem>();
		}
		if (MapSystem != null)
		{
			MapSystem.SetIconsVisibleByCategory(IconCategory.Value,ZoomValue.Value);
		}else
		{
			LogError("Could not find mapsystem object in scene");
		}
		Finish();
	}
}

[ActionCategory("KGFMapSystem")]
public class KGFMapSystemSetTooltipsByCategory : FsmStateAction
{
	public KGFMapSystem MapSystem;
	[RequiredField]
	public FsmString IconCategory;
	public FsmBool ToolTipValue;
	public override void Reset ()
	{
		MapSystem = null;
	}
	public override void OnEnter ()
	{
		if(MapSystem == null)
		{
			MapSystem = KGFAccessor.GetObject<KGFMapSystem>();
		}
		if (MapSystem != null)
		{
			MapSystem.SetTooltipsByCategory(IconCategory.Value,ToolTipValue.Value);
		}else
		{
			LogError("Could not find mapsystem object in scene");
		}
		Finish();
	}
}

[ActionCategory("KGFMapSystem")]
public class KGFMapSystemSetZoom : FsmStateAction
{
	public KGFMapSystem MapSystem;
	public FsmFloat ViewPort;
	public override void Reset ()
	{
		MapSystem = null;
		ViewPort = 10;
	}
	public override void OnEnter ()
	{
		if(MapSystem == null)
		{
			MapSystem = KGFAccessor.GetObject<KGFMapSystem>();
		}
		if (MapSystem != null)
		{
			MapSystem.SetZoom(ViewPort.Value);
		}else
		{
			LogError("Could not find mapsystem object in scene");
		}
		Finish();
	}
}

[ActionCategory("KGFMapSystem")]
public class KGFMapSystemSetViewPort : FsmStateAction
{
	public KGFMapSystem MapSystem;
	public FsmBool ViewPort;
	public override void Reset ()
	{
		MapSystem = null;
	}
	public override void OnEnter ()
	{
		if(MapSystem == null)
		{
			MapSystem = KGFAccessor.GetObject<KGFMapSystem>();
		}
		if (MapSystem != null)
		{
			MapSystem.SetViewportEnabled(ViewPort.Value);
		}else
		{
			LogError("Could not find mapsystem object in scene");
		}
		Finish();
	}
}

[ActionCategory("KGFMapSystem")]
public class KGFMapSystemSetMinimapState : FsmStateAction
{
	public KGFMapSystem MapSystem;
	public FsmBool MinimapEnable;
	public override void Reset ()
	{
		MapSystem = null;
	}
	public override void OnEnter ()
	{
		if(MapSystem == null)
		{
			MapSystem = KGFAccessor.GetObject<KGFMapSystem>();
		}
		if (MapSystem != null)
		{
			MapSystem.SetMinimapEnabled(MinimapEnable.Value);
		}else
		{
			LogError("Could not find mapsystem object in scene");
		}
		Finish();
	}
}

[ActionCategory("KGFMapSystem")]
public class KGFMapSystemSetMinimapSize : FsmStateAction
{
	public KGFMapSystem MapSystem;
	[Tooltip("The size of the minimap normalized to the Screen.width. Possible values are 0-1.")]
	[HasFloatSlider(0,1)]
	public FsmFloat Size;
	public override void Reset ()
	{
		MapSystem = null;
	}
	public override void OnEnter ()
	{
		if(MapSystem == null)
		{
			MapSystem = KGFAccessor.GetObject<KGFMapSystem>();
		}
		if (MapSystem != null)
		{
			MapSystem.SetMinimapSize(Size.Value);
		}else
		{
			LogError("Could not find mapsystem object in scene");
		}
		Finish();
	}
}

[ActionCategory("KGFMapSystem")]
public class KGFMapSystemZoomIn : FsmStateAction
{
	public KGFMapSystem MapSystem;
	public override void Reset ()
	{
		MapSystem = null;
	}
	public override void OnEnter ()
	{
		if(MapSystem == null)
		{
			MapSystem = KGFAccessor.GetObject<KGFMapSystem>();
		}
		if (MapSystem != null)
		{
			MapSystem.ZoomIn();
		}else
		{
			LogError("Could not find mapsystem object in scene");
		}
		Finish();
	}
}

[ActionCategory("KGFMapSystem")]
public class KGFMapSystemZoomOut : FsmStateAction
{
	public KGFMapSystem MapSystem;
	public override void Reset ()
	{
		MapSystem = null;
	}
	public override void OnEnter ()
	{
		if(MapSystem == null)
		{
			MapSystem = KGFAccessor.GetObject<KGFMapSystem>();
		}
		if (MapSystem != null)
		{
			MapSystem.ZoomOut();
		}else
		{
			LogError("Could not find mapsystem object in scene");
		}
		Finish();
	}
}

[ActionCategory("KGFMapSystem")]
public class KGFMapSystemZoomMax : FsmStateAction
{
	public KGFMapSystem MapSystem;
	public override void Reset ()
	{
		MapSystem = null;
	}
	public override void OnEnter ()
	{
		if(MapSystem == null)
		{
			MapSystem = KGFAccessor.GetObject<KGFMapSystem>();
		}
		if (MapSystem != null)
		{
			MapSystem.ZoomMax();
		}else
		{
			LogError("Could not find mapsystem object in scene");
		}
		Finish();
	}
}

[ActionCategory("KGFMapSystem")]
public class KGFMapSystemZoomMin : FsmStateAction
{
	public KGFMapSystem MapSystem;
	public FsmInt Size;
	public override void Reset ()
	{
		MapSystem = null;
	}
	public override void OnEnter ()
	{
		if(MapSystem == null)
		{
			MapSystem = KGFAccessor.GetObject<KGFMapSystem>();
		}
		if (MapSystem != null)
		{
			MapSystem.ZoomMin();
		}else
		{
			LogError("Could not find mapsystem object in scene");
		}
		Finish();
	}
}

[ActionCategory("KGFMapSystem")]
public class KGFMapSystemRevealedMinPercent : FsmStateAction
{
	[Tooltip("KGFMapSystem object. If not given it will try to find it in runtime.")]
	public KGFMapSystem MapSystem;
	[HasFloatSlider(0,1)]
	public FsmFloat PercentReached;
	[Tooltip("Event to trigger when the minimal percentage of revealing the map has been reached.")]
	public FsmEvent EventOnReached;
	public override void Reset ()
	{
		MapSystem = null;
	}
	public override void OnEnter ()
	{
		if(MapSystem == null)
		{
			MapSystem = KGFAccessor.GetObject<KGFMapSystem>();
		}
		if (MapSystem != null)
		{
		}else
		{
			LogError("Could not find mapsystem object in scene");
		}
	}
	public override void OnUpdate()
	{
		if (MapSystem.GetRevealedPercent() > PercentReached.Value)
		{
			Fsm.Event(EventOnReached);
			Finish();
		}
	}
}
#endregion

#region KGFMapIcon
[ActionCategory("KGFMapSystem")]
public class KGFMapIconSetVisibility : FsmStateAction
{
	[RequiredField]
	public KGFMapIcon MapIcon;
	public FsmBool Visibility;
	public override void Reset ()
	{
		MapIcon = null;
	}
	public override void OnEnter ()
	{
		if (MapIcon != null)
		{
			MapIcon.SetVisibility(Visibility.Value);
		}else
		{
			LogError("Please a add map icon to this action.");
		}
	}
	public override string ErrorCheck()
	{
		if (MapIcon == null)
		{
			return "MapIcon needs to be filled in";
		}
		return null;
	}
}

[ActionCategory("KGFMapSystem")]
public class KGFMapIconSetBlinking : FsmStateAction
{
	[RequiredField]
	public KGFMapIcon MapIcon;
	public FsmBool Blink;
	public override void Reset ()
	{
		MapIcon = null;
	}
	public override void OnEnter ()
	{
		if (MapIcon != null)
		{
			MapIcon.SetIsBlinking(Blink.Value);
		}else
		{
			LogError("Please a add map icon to this action.");
		}
	}
	public override string ErrorCheck()
	{
		if (MapIcon == null)
		{
			return "MapIcon needs to be filled in";
		}
		return null;
	}
}

[ActionCategory("KGFMapSystem")]
public class KGFMapIconSetIconColor : FsmStateAction
{
	[RequiredField]
	public KGFMapIcon MapIcon;
	public FsmColor IconColor;
	public override void Reset ()
	{
		MapIcon = null;
		IconColor = Color.white;
	}
	public override void OnEnter ()
	{
		if (MapIcon != null)
		{
			MapIcon.SetColor(IconColor.Value);
		}else
		{
			LogError("Please a add map icon to this action.");
		}
	}
	public override string ErrorCheck()
	{
		if (MapIcon == null)
		{
			return "MapIcon needs to be filled in";
		}
		return null;
	}
}

[ActionCategory("KGFMapSystem")]
public class KGFMapIconSetArrowUsage : FsmStateAction
{
	[RequiredField]
	public KGFMapIcon MapIcon;
	public FsmBool ArrowUsage;
	public override void Reset ()
	{
		MapIcon = null;
		ArrowUsage = true;
	}
	public override void OnEnter ()
	{
		if (MapIcon != null)
		{
			MapIcon.SetArrowUsage(ArrowUsage.Value);
		}else
		{
			LogError("Please a add map icon to this action.");
		}
	}
	public override string ErrorCheck()
	{
		if (MapIcon == null)
		{
			return "MapIcon needs to be filled in";
		}
		return null;
	}
}

[ActionCategory("KGFMapSystem")]
public class KGFMapIconSetToolTipText : FsmStateAction
{
	[RequiredField]
	public KGFMapIcon MapIcon;
	public FsmString ToolTipText;
	public override void Reset ()
	{
		MapIcon = null;
		ToolTipText = "";
	}
	public override void OnEnter ()
	{
		if (MapIcon != null)
		{
			MapIcon.SetToolTipText(ToolTipText.Value);
		}else
		{
			LogError("Please a add map icon to this action.");
		}
	}
	public override string ErrorCheck()
	{
		if (MapIcon == null)
		{
			return "MapIcon needs to be filled in";
		}
		return null;
	}
}

[ActionCategory("KGFMapSystem")]
public class KGFMapIconOnClicked : FsmStateAction
{
	public KGFMapSystem MapSystem;
	[RequiredField]
	public KGFMapIcon MapIcon;
	public FsmEvent EventOnIconClick;
	[Tooltip("Repeat every frame.")]
	public bool everyFrame;
	public override void Reset ()
	{
		MapSystem = null;
	}
	public override void OnEnter ()
	{
		if(MapSystem == null)
		{
			MapSystem = KGFAccessor.GetObject<KGFMapSystem>();
		}
		if (MapSystem != null)
		{
			MapSystem.EventMouseMapIconClicked += OnMapIconClick;
		}else
		{
			LogError("Please a add map icon to this action.");
		}
	}
	
	public override void OnExit()
	{
		if (MapSystem != null)
		{
			MapSystem.EventMouseMapIconClicked -= OnMapIconClick;
		}
	}
	
	void OnMapIconClick(object theSender, EventArgs theArgs)
	{
		if (!Finished)
		{
			KGFMapSystem.KGFMarkerEventArgs anArgs = (KGFMapSystem.KGFMarkerEventArgs)theArgs;
			if (anArgs.itsMarker == MapIcon)
			{
				Fsm.Event(EventOnIconClick);
				if (!everyFrame)
				{
					Finish();
				}
			}
		}
	}
	
	public override string ErrorCheck()
	{
		if (MapIcon == null)
		{
			return "MapIcon needs to be filled in";
		}
		return null;
	}
}

[ActionCategory("KGFMapSystem")]
public class KGFMapIconWaitForIconVisibility : FsmStateAction
{
	public KGFMapSystem MapSystem;
	[RequiredField]
	public KGFMapIcon MapIcon;
	public FsmBool Visibility;
	public FsmEvent EventOnVisibilityChanged;
	public override void Reset ()
	{
		MapSystem = null;
	}
	public override void OnEnter ()
	{
		if(MapSystem == null)
		{
			MapSystem = KGFAccessor.GetObject<KGFMapSystem>();
		}
		if (MapSystem != null)
		{
			if (MapSystem.GetIsVisibleOnMap(MapIcon) == Visibility.Value)
			{
				Finish();
			}else
			{
				MapSystem.EventVisibilityOnMinimapChanged += OnVisibilityChanged;
			}
		}else
		{
			LogError("Please a add map icon to this action.");
		}
	}
	
	public override void OnExit()
	{
		if (MapSystem != null)
		{
			MapSystem.EventMouseMapIconClicked -= OnVisibilityChanged;
		}
	}
	
	void OnVisibilityChanged(object theSender, EventArgs theArgs)
	{
		if (!Finished)
		{
			KGFMapSystem.KGFMarkerEventArgs anArgs = (KGFMapSystem.KGFMarkerEventArgs)theArgs;
			if (anArgs.itsMarker == MapIcon)
			{
				Fsm.Event(EventOnVisibilityChanged);
				Finish();
			}
		}
	}
	
	public override string ErrorCheck()
	{
		if (MapIcon == null)
		{
			return "MapIcon needs to be filled in";
		}
		return null;
	}
}
#endregion

#endif