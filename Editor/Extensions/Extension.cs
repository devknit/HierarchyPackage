
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Hierarchy
{
	public class Extension
	{
		public Extension()
		{
			components = new Dictionary<int, BaseComponent>();
			components.Add( (int)HierarchyComponentEnum.kLockComponent, new LockComponent());
			components.Add( (int)HierarchyComponentEnum.kActivityComponent, new ActivityComponent());
			components.Add( (int)HierarchyComponentEnum.kStaticComponent, new StaticComponent());
			components.Add( (int)HierarchyComponentEnum.kRendererComponent, new RendererComponent());
			components.Add( (int)HierarchyComponentEnum.kTagAndLayerComponent, new TagLayerComponent());
			components.Add( (int)HierarchyComponentEnum.kErrorComponent, new ErrorComponent());
			components.Add( (int)HierarchyComponentEnum.kComponentsComponent, new ComponentsComponent());
			
			preComponents = new List<BaseComponent>();
			preComponents.Add( new TreeMapComponent());
			preComponents.Add( new SeparatorComponent());
			
			orderedComponents = new List<BaseComponent>();
			
			trimIcon = Resources.Instance.GetTexture( Image.kTrimIcon);
			
			Settings.Instance.AddEventListener( Setting.kHierarchyExtension, SettingChanged);
			Settings.Instance.AddEventListener( Setting.kAdditionalIdentation, SettingChanged);
			Settings.Instance.AddEventListener( Setting.kComponentsOrder, SettingChanged);
			Settings.Instance.AddEventListener( Setting.kAdditionalHideIconsIfNotFit, SettingChanged);
			Settings.Instance.AddEventListener( Setting.kAdditionalBackgroundColor, SettingChanged);
			Settings.Instance.AddEventListener( Setting.kAdditionalInactiveColor, SettingChanged);
			SettingChanged();
		}
		void SettingChanged()
		{
			string componentOrder = Settings.Instance.Get<string>( Setting.kComponentsOrder);
			string[] componentIds = componentOrder.Split( ';');
			
			if( componentIds.Length != Settings.kDefaultOrderCount) 
			{
				Settings.Instance.Set( Setting.kComponentsOrder, Settings.kDefaultOrder, false);
				componentIds = Settings.kDefaultOrder.Split( ';');
			}
			orderedComponents.Clear(); 
			
			for( int i0 = 0; i0 < componentIds.Length; ++i0)
			{
				try
				{
					orderedComponents.Add( components[ int.Parse( componentIds[ i0])]);
				}
				catch
				{
				}
			}
			
			orderedComponents.Add( components[ (int)HierarchyComponentEnum.kComponentsComponent]);
			
			enabled = Settings.Instance.Get<bool>( Setting.kHierarchyExtension);
			indentation = Settings.Instance.Get<int>( Setting.kAdditionalIdentation);
			inactiveColor = Settings.Instance.GetColor( Setting.kAdditionalInactiveColor);
			backgroundColor = Settings.Instance.GetColor( Setting.kAdditionalBackgroundColor);
			hideIconsIfThereIsNoFreeSpace = Settings.Instance.Get<bool>( Setting.kAdditionalHideIconsIfNotFit);
		}
		public void HierarchyWindowItemOnGUIHandler( int instanceId, Rect selectionRect)
		{
			try
			{
				if( enabled != false)
				{
					if( EditorUtility.InstanceIDToObject( instanceId) is GameObject gameObject)
					{
						Rect curRect = selectionRect;
						curRect.width = EditorGUIUtility.singleLineHeight;
						curRect.x += selectionRect.width - indentation;
						
						float gameObjectNameWidth = hideIconsIfThereIsNoFreeSpace ? 
							GUI.skin.label.CalcSize( new GUIContent( gameObject.name)).x : 0;
						DrawComponents( orderedComponents, selectionRect, ref curRect, gameObject, true, 
							hideIconsIfThereIsNoFreeSpace ? selectionRect.x + gameObjectNameWidth + 20 : 0);
						
						errorHandled.Remove( instanceId);
					}
				}
			}
			catch( System.Exception e)
			{
				if( errorHandled.Add( instanceId) != false)
				{
					Debug.LogError( e);
				}
			}
		}
		void DrawComponents( List<BaseComponent> components, Rect selectionRect, ref Rect rect, GameObject gameObject, bool drawBackground, float minX=50)
		{
			BaseComponent component;
			
			if( Event.current.type == EventType.Repaint)
			{
				LayoutStatus layoutStatus = LayoutStatus.kSuccess;
				int toComponent = components.Count;
				
				for( int i0 = 0, n = toComponent; i0 < n; ++i0)
				{
					component = components[ i0];
					
					if( component.IsEnabled() != false)
					{
						layoutStatus = component.Layout( gameObject, selectionRect, ref rect, rect.x - minX);
						
						if( layoutStatus != LayoutStatus.kSuccess)
						{
							toComponent = layoutStatus == LayoutStatus.kFailed ? i0 : i0 + 1;
							rect.x -= 7;
							break;
						}
					}
					else
					{
						component.DisabledHandler( gameObject);
					}
				}
				if( drawBackground != false)
				{
					if (backgroundColor.a != 0)
					{
						rect.width = selectionRect.x + selectionRect.width - rect.x /*- indentation*/;
						EditorGUI.DrawRect( rect, backgroundColor);
					}
					DrawComponents( preComponents, selectionRect, ref rect, gameObject, false);
				}
				for( int i0 = 0, n = toComponent; i0 < n; ++i0)
				{
					component = components[ i0];
					
					if( component.IsEnabled() != false)
					{
						component.Draw( gameObject, selectionRect);
					}
				}
				if( layoutStatus != LayoutStatus.kSuccess)
				{
					rect.width = 7;
					ColorUtils.SetColor( inactiveColor);
					GUI.DrawTexture( rect, trimIcon);
					ColorUtils.ClearColor();
				}
			}
			else if( Event.current.isMouse != false)
			{
				for( int i0 = 0, n = components.Count; i0 < n; ++i0)
				{
					component = components[ i0];
					
					if( component.IsEnabled() != false)
					{
						if( component.Layout( gameObject, selectionRect, ref rect, rect.x - minX) != LayoutStatus.kFailed)
						{
							if( component.IsEdit() != false)
							{
								component.EventHandler( gameObject, Event.current);
							}
						}
					}
				}
			}
		}
		
		HashSet<int> errorHandled = new HashSet<int>();
		Dictionary<int, BaseComponent> components;
		List<BaseComponent> preComponents;
		List<BaseComponent> orderedComponents;
		
		bool enabled;
		int indentation;
		Texture2D trimIcon;
		Color inactiveColor;
		Color backgroundColor;
		bool hideIconsIfThereIsNoFreeSpace;
	}
}
