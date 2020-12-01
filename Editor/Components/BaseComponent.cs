
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Hierarchy
{
	public enum LayoutStatus
	{
		kSuccess,
		kPartly,
		kFailed,
	}
	public class BaseComponent
	{
		public BaseComponent()
		{
		}
		public virtual LayoutStatus Layout( GameObject gameObject, Rect selectionRect, ref Rect curRect, float maxWidth)
		{
			return LayoutStatus.kSuccess;
		}
		public virtual void Draw( GameObject gameObject, Rect selectionRect)
		{
		}
		public virtual void EventHandler( GameObject gameObject, Event currentEvent)
		{
		}
		public virtual void DisabledHandler( GameObject gameObject)
		{
		}
		public virtual void SetEnabled( bool value)
		{
			enabled = value;
		}		
		public virtual bool IsEnabled()
		{
			if( enabled == false) 
			{
				return false;
			}
			else 
			{
				if( Application.isPlaying != false)
				{
					return showComponentDuringPlayMode;
				}
			}
			return true;
		}
		public virtual bool IsEdit()
		{
			return editable;
		}
		protected void GetGameObjectListRecursive( GameObject gameObject, ref List<GameObject> result, int maxDepth=int.MaxValue)
		{
			result.Add( gameObject);
			if( maxDepth > 0)
			{
				Transform transform = gameObject.transform;
				
				for( int i0 = transform.childCount - 1; i0 >= 0; --i0)
				{
					GetGameObjectListRecursive( transform.GetChild( i0).gameObject, ref result, maxDepth - 1);
				}
			}
		}
		protected void SelectOperationTargets( 
			GameObject gameObject, Event currentEvent, ref int targetState, 
			System.Func<bool> stateCallback, System.Action<GameObject[]> targetCallback)
		{
			if( currentEvent.isMouse != false && currentEvent.button == 0 && rect.Contains( currentEvent.mousePosition) != false)
			{
				if( currentEvent.type == EventType.MouseDown)
				{
					targetState = ((!stateCallback()) == true ? 1 : 0);
				}
				else if( currentEvent.type == EventType.MouseDrag && targetState != -1)
				{
					if( targetState == (stateCallback() == true ? 1 : 0))
					{
						return;
					}
				} 
				else
				{
					targetState = -1;
					return;
				}
															
				bool showWarning = Settings.Instance.Get<bool>( Setting.kAdditionalShowModifierWarning);
				
				var targetGameObjects = new List<GameObject>();
				
				if( currentEvent.control || currentEvent.command) 
				{
				}
				else if( currentEvent.shift != false)
				{
					if( showWarning == false
					||	EditorUtility.DisplayDialog( "Change state", "Are you sure you want to switch the state of this GameObject and all its children? (You can disable this warning in the settings)", "Yes", "Cancel"))
					{
						GetGameObjectListRecursive( gameObject, ref targetGameObjects);
					}
				}
				else if( currentEvent.alt != false) 
				{
					if( gameObject.transform.parent != null)
					{
						if( showWarning == false
						||	EditorUtility.DisplayDialog( "Change state", "Are you sure you want to switch the state this GameObject and its siblings? (You can disable this warning in the settings)", "Yes", "Cancel"))
						{
							GetGameObjectListRecursive( gameObject.transform.parent.gameObject, ref targetGameObjects, 1);
							targetGameObjects.Remove( gameObject.transform.parent.gameObject);
						}
					}
					else
					{
						Debug.Log("This action for root objects is supported for Unity3d 5.3.3 and above");
						return;
					}
				}
				else 
				{
					if( Selection.Contains( gameObject) != false)
					{
						targetGameObjects.AddRange( Selection.gameObjects);
					}
					else
					{
						GetGameObjectListRecursive( gameObject, ref targetGameObjects, 0);
					}
				}
				
				targetCallback?.Invoke( targetGameObjects.ToArray());
				currentEvent.Use();  
			} 
		}
		
		public Rect rect = new Rect( 0, 0, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
		
		protected bool enabled = false;
		protected bool editable = false;
		protected bool showComponentDuringPlayMode = false;
	}
}
