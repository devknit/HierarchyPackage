
using UnityEngine;
using UnityEditor;

namespace Hierarchy
{
	public class RendererComponent: BaseComponent
	{
		public RendererComponent()
		{
			rect.width = 12;

			rendererButtonTexture = Resources.Instance.GetTexture( Image.kRendererButton);
			
			Settings.Instance.AddEventListener( Setting.kRendererShow, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kRendererEditable, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kRendererShowDuringPlayMode, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kAdditionalActiveColor, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kAdditionalInactiveColor, SettingsChanged);
			SettingsChanged();
		}
		void SettingsChanged()
		{
			enabled = Settings.Instance.Get<bool>( Setting.kRendererShow);
			editable = Settings.Instance.Get<bool>( Setting.kRendererEditable);
			showComponentDuringPlayMode = Settings.Instance.Get<bool>( Setting.kRendererShowDuringPlayMode);
			activeColor = Settings.Instance.GetColor( Setting.kAdditionalActiveColor);
			inactiveColor = Settings.Instance.GetColor( Setting.kAdditionalInactiveColor);
		}
		public override LayoutStatus Layout( GameObject gameObject, Rect selectionRect, ref Rect curRect, float maxWidth)
		{
			if( maxWidth < 12)
			{
				return LayoutStatus.kFailed;
			}
			curRect.x -= 12;
			rect.x = curRect.x;
			rect.y = curRect.y;
			return LayoutStatus.kSuccess;
		}
		public override void Draw( GameObject gameObject, Rect selectionRect)
		{
			Renderer renderer = gameObject.GetComponent<Renderer>();
			if( renderer != null)
			{
				if( renderer.enabled != false)
				{
					ColorUtils.SetColor( activeColor);
					GUI.DrawTexture( rect, rendererButtonTexture);
					ColorUtils.ClearColor();
				}
				else
				{
					ColorUtils.SetColor( inactiveColor);
					GUI.DrawTexture( rect, rendererButtonTexture);
					ColorUtils.ClearColor();
				}
			}
		}
		public override void EventHandler( GameObject gameObject, Event currentEvent)
		{
			if( currentEvent.isMouse != false && currentEvent.button == 0 && rect.Contains( currentEvent.mousePosition) != false)
			{
				Renderer renderer = gameObject.GetComponent<Renderer>();
				if( renderer != null)
				{
					bool enabled = renderer.enabled;
					
					if( currentEvent.type == EventType.MouseDown)
					{
						targetRendererMode = ((!enabled) == true ? 1 : 0);
					}
					else if( currentEvent.type == EventType.MouseDrag && targetRendererMode != -1)
					{
						if( targetRendererMode == (enabled == true ? 1 : 0))
						{
							return;
						}
					} 
					else
					{
						targetRendererMode = -1;
						return;
					}

					Undo.RecordObject( renderer, "renderer visibility change");					  
					renderer.enabled = !enabled;
					EditorUtility.SetDirty( gameObject);
				}
				currentEvent.Use();
			}
		}
		
		Color activeColor;
		Color inactiveColor;
		Texture2D rendererButtonTexture;
		int targetRendererMode = -1; 
	}
}
