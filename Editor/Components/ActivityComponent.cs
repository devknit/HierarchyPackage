
using UnityEngine;
using UnityEditor;

namespace Hierarchy
{
	public class ActivityComponent: BaseComponent
	{
		public ActivityComponent()
		{
			rect.width = 18;
			
			activityButtonTexture = Resources.Instance.GetTexture( Image.kActivityButton);
			activityOffButtonTexture = Resources.Instance.GetTexture( Image.kActivityOffButton);
			
			Settings.Instance.AddEventListener( Setting.kActivityShow, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kActivityEditable, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kActivityShowDuringPlayMode, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kAdditionalActiveColor, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kAdditionalInactiveColor, SettingsChanged);
			SettingsChanged();
		}
		void SettingsChanged()
		{
			enabled = Settings.Instance.Get<bool>( Setting.kActivityShow);
			editable = Settings.Instance.Get<bool>( Setting.kActivityEditable);
			showComponentDuringPlayMode = Settings.Instance.Get<bool>( Setting.kActivityShowDuringPlayMode);
			activeColor = Settings.Instance.GetColor( Setting.kAdditionalActiveColor);
			inactiveColor = Settings.Instance.GetColor( Setting.kAdditionalInactiveColor);
		}
		public override LayoutStatus Layout( GameObject gameObject, Rect selectionRect, ref Rect curRect, float maxWidth)
		{
			if( maxWidth < 18)
			{
				return LayoutStatus.kFailed;
			}
			curRect.x -= 18;
			rect.x = curRect.x;
			rect.y = curRect.y;
			return LayoutStatus.kSuccess;
		}
		public override void Draw( GameObject gameObject, Rect selectionRect)
		{
			bool activeSelf = gameObject.activeSelf;
			bool activeInHierarchy = gameObject.activeInHierarchy;
			
			if( activeSelf == false)
			{
				ColorUtils.SetColor( inactiveColor);
				GUI.DrawTexture( rect, activityOffButtonTexture);
			}
			else if( activeInHierarchy == false)
			{
				ColorUtils.SetColor( activeColor, 1.0f, 0.4f);
				GUI.DrawTexture( rect, activityButtonTexture);
			}
			else
			{
				ColorUtils.SetColor( activeColor);
				GUI.DrawTexture( rect, activityButtonTexture);
			}
			ColorUtils.ClearColor();
		}
		public override void EventHandler( GameObject gameObject, Event currentEvent)
		{
			SelectOperationTargets
			( 
				gameObject, currentEvent, ref targetActivityState, 
				() =>
				{
					return gameObject.activeSelf;
				},
				(gameObjects) =>
				{
					if( gameObjects.Length > 0)
					{
						Undo.RecordObjects( gameObjects, "active change");
						bool targetActivity = !gameObject.activeSelf;
						
						for( int i0 = gameObjects.Length - 1; i0 >= 0; --i0)
						{		 
							GameObject targetGameObject = gameObjects[ i0];
							targetGameObject.SetActive( targetActivity);
							EditorUtility.SetDirty( targetGameObject);
						}
					}
				}
			);
		}
		Color activeColor;
		Color inactiveColor;
//		Color specialColor;
		Texture2D activityButtonTexture;
		Texture2D activityOffButtonTexture;
		int targetActivityState = -1;
	}
}

