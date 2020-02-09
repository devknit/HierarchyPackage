
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

namespace Hierarchy
{
	public class ComponentsComponent: BaseComponent
	{
		public ComponentsComponent()
		{
			grayColor = Resources.Instance.GetColor( ColorTone.kGray);
			backgroundDarkColor = Resources.Instance.GetColor( ColorTone.kBackgroundDark);
			componentIcon = Resources.Instance.GetTexture( Image.kComponentUnknownIcon);

			hintLabelStyle = new GUIStyle();
			hintLabelStyle.normal.textColor = grayColor;
			hintLabelStyle.fontSize = 11;
			hintLabelStyle.clipping = TextClipping.Clip;  

			Settings.Instance.AddEventListener( Setting.kComponentsShow, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kComponentsEditable, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kComponentsShowDuringPlayMode, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kComponentsIconSize, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kComponentsIgnore, SettingsChanged);
			SettingsChanged();
		}
		void SettingsChanged()
		{
			enabled = Settings.Instance.Get<bool>( Setting.kComponentsShow);
			editable = Settings.Instance.Get<bool>( Setting.kComponentsEditable);
			showComponentDuringPlayMode = Settings.Instance.Get<bool>( Setting.kComponentsShowDuringPlayMode);
			HierarchySizeAll size = (HierarchySizeAll)Settings.Instance.Get<int>( Setting.kComponentsIconSize);
			string ignoreString = Settings.Instance.Get<string>( Setting.kComponentsIgnore);
			
			rect.width = rect.height = (size == HierarchySizeAll.kNormal ? 15 : (size == HierarchySizeAll.kBig ? 16 : 13)); 	  
			if( string.IsNullOrEmpty( ignoreString) == false) 
			{
				ignoreScripts = new List<string>( ignoreString.Split( new char[]{ ',', ';', '.', ' ' }));
				ignoreScripts.RemoveAll( x => x == string.Empty);
			}
			else
			{
				ignoreScripts = null;
			}
		}
		public override LayoutStatus Layout( GameObject gameObject, Rect selectionRect, ref Rect curRect, float maxWidth)
		{
			Component[] currentComponents = gameObject.GetComponents<Component>();

			components.Clear();
			
			if( ignoreScripts != null)
			{
				for( int i0 = 0; i0 < currentComponents.Length; ++i0)
				{
					Component currentComponent = currentComponents[ i0];
					bool ignore = false;
					
					if( currentComponent != null)
					{
						string componentName = currentComponent.GetType().FullName;
						
						for( int i1 = ignoreScripts.Count - 1; i1 >= 0; --i1)
						{
							if( componentName.Contains( ignoreScripts[ i1]) != false)
							{
								ignore = true;
								break;
							} 
						}
					}
					if( ignore == false)
					{
						components.Add( currentComponent);
					}
				}
			}
			else
			{
				components.AddRange( currentComponents);
			}

			int maxComponentsCount = Mathf.FloorToInt( (maxWidth - 2) / rect.width);
			componentsToDraw = System.Math.Min( maxComponentsCount, components.Count - 1);

			float totalWidth = 2 + rect.width * componentsToDraw;
	
			curRect.x -= totalWidth;

			rect.x = curRect.x;
			rect.y = curRect.y - (rect.height - 16) / 2;

			eventRect.width = totalWidth;
			eventRect.x = rect.x;
			eventRect.y = rect.y;

			if( maxComponentsCount >= components.Count - 1)
			{
				return LayoutStatus.kSuccess;
			}
			else if( maxComponentsCount == 0)
			{
				return LayoutStatus.kFailed;
			}
			return LayoutStatus.kPartly;
		}
		public override void Draw( GameObject gameObject, Rect selectionRect)
		{
			for( int i0 = components.Count - componentsToDraw, n = components.Count; i0 < n; ++i0)
			{
				Component component = components[ i0];
				
				if( component is Transform)
				{
					continue;
				}
				GUIContent content = EditorGUIUtility.ObjectContent( component, null);
				bool enabled = true;
				
				try
				{
					PropertyInfo propertyInfo = component.GetType().GetProperty( "enabled");
					enabled = (bool)propertyInfo.GetGetMethod().Invoke( component, null);
				}
				catch
				{
				}

				Color color = GUI.color;
				color.a = enabled ? 1f : 0.3f;
				GUI.color = color;
				GUI.DrawTexture( rect, content.image == null ? componentIcon : content.image, ScaleMode.ScaleToFit);
				color.a = 1;
				GUI.color = color;

				if( rect.Contains( Event.current.mousePosition) != false)
				{		 
					string componentName = "Missing script";
					if( component != null)
					{
						componentName = component.GetType().Name;
					}

					int labelWidth = Mathf.CeilToInt( hintLabelStyle.CalcSize( new GUIContent( componentName)).x); 				   
					selectionRect.x = rect.x - labelWidth / 2 - 4;
					selectionRect.width = labelWidth + 8;
					selectionRect.height -= 1;

					if( selectionRect.y > 16)
					{
						selectionRect.y -= 16;
					}
					else
					{
						selectionRect.x += labelWidth / 2 + 18;
					}

					EditorGUI.DrawRect( selectionRect, backgroundDarkColor);
					selectionRect.x += 4;
					selectionRect.y += (EditorGUIUtility.singleLineHeight - rect.height) * 0.5f;
					selectionRect.height = EditorGUIUtility.singleLineHeight;
					EditorGUI.LabelField( selectionRect, componentName, hintLabelStyle);
				}
				rect.x += rect.width;
			}
		}
		public override void EventHandler( GameObject gameObject, Event currentEvent)
		{
			if( currentEvent.isMouse != false && currentEvent.button == 0 && eventRect.Contains( currentEvent.mousePosition) != false)
			{
				if( currentEvent.type == EventType.MouseDown)
				{
					int id = Mathf.FloorToInt( (currentEvent.mousePosition.x - eventRect.x) / rect.width) + components.Count - 1 - componentsToDraw + 1;

					try
					{
						Component component = components[ id];
						PropertyInfo propertyInfo = component.GetType().GetProperty( "enabled");
						bool enabled = (bool)propertyInfo.GetGetMethod().Invoke( component, null);
						Undo.RecordObject( component, "Change component enabled");
						propertyInfo.GetSetMethod().Invoke( component, new object[]{ !enabled });
					}
					catch {}

					EditorUtility.SetDirty( gameObject);
				}
				currentEvent.Use();
			}
		}
		
		Color grayColor;
		Color backgroundDarkColor;
		Texture2D componentIcon;
		GUIStyle hintLabelStyle;
		
		Rect eventRect = new Rect( 0, 0, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
		List<Component> components = new List<Component>();   
		List<string> ignoreScripts;
		int componentsToDraw;
	}
}
