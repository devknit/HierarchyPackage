
using UnityEngine;
using UnityEditor;

namespace Hierarchy
{
	public class LockComponent: BaseComponent
	{
		public LockComponent()
		{
			rect.width = 13;
			lockButtonTexture = Resources.Instance.GetTexture( Image.kLockButton);
			
			Settings.Instance.AddEventListener( Setting.kLockShow, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kLockEditable, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kAdditionalShowModifierWarning, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kLockShowDuringPlayMode, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kAdditionalActiveColor, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kAdditionalInactiveColor, SettingsChanged);
			SettingsChanged();
		}
		void SettingsChanged()
		{
			enabled = Settings.Instance.Get<bool>( Setting.kLockShow);
			editable = Settings.Instance.Get<bool>( Setting.kLockEditable);
			showModifierWarning = Settings.Instance.Get<bool>( Setting.kAdditionalShowModifierWarning);
			showComponentDuringPlayMode = Settings.Instance.Get<bool>( Setting.kLockShowDuringPlayMode);
			activeColor = Settings.Instance.GetColor( Setting.kAdditionalActiveColor);
			inactiveColor = Settings.Instance.GetColor( Setting.kAdditionalInactiveColor);
		}
		public override LayoutStatus Layout( GameObject gameObject, Rect selectionRect, ref Rect curRect, float maxWidth)
		{
			if (maxWidth < 13)
			{
				return LayoutStatus.kFailed;
			}
			curRect.x -= 13;
			rect.x = curRect.x;
			rect.y = curRect.y;
			return LayoutStatus.kSuccess;
		}
		public override void Draw( GameObject gameObject, Rect selectionRect)
		{  
			bool locked = IsLock( gameObject);
			ColorUtils.SetColor( locked ? activeColor : inactiveColor);
			GUI.DrawTexture( rect, lockButtonTexture);
			ColorUtils.ClearColor();
		}
		public override void EventHandler( GameObject gameObject, Event currentEvent)
		{
			SelectOperationTargets
			( 
				gameObject, currentEvent, ref targetLockState, 
				() =>
				{
					return IsLock( gameObject);
				},
				(gameObjects) =>
				{
					if( gameObjects.Length > 0)
					{
						Undo.RecordObjects( gameObjects, "Change lock state");
						bool targetLocked = IsLock( gameObject);
						
						for( int i0 = gameObjects.Length - 1; i0 >= 0; --i0)
						{		 
							GameObject targetGameObject = gameObjects[ i0];
							
							if( targetLocked == false)
							{
								targetGameObject.hideFlags |= HideFlags.NotEditable;
							}
							else
							{
								targetGameObject.hideFlags &= ~HideFlags.NotEditable;
							}
							EditorUtility.SetDirty( targetGameObject);
						}
					}
				}
			);
		}
		bool IsLock( GameObject gameObject)
		{
			return (gameObject.hideFlags & HideFlags.NotEditable) == HideFlags.NotEditable;
		}
		
		Color activeColor;
		Color inactiveColor;
		Texture2D lockButtonTexture;
		bool showModifierWarning;
		int targetLockState = -1;
	}
}
