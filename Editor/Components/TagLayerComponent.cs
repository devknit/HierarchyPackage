
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

namespace Hierarchy
{
	public class TagLayerComponent : BaseComponent
	{
		public TagLayerComponent()
		{
			labelStyle = new GUIStyle();
			labelStyle.fontSize = 8;
			labelStyle.clipping = TextClipping.Clip;  
			labelStyle.alignment = TextAnchor.MiddleLeft;
			
			Settings.Instance.AddEventListener( Setting.kTagAndLayerShow, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kTagAndLayerEditable, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kTagAndLayerShowDuringPlayMode, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kTagAndLayerType, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kTagAndLayerSizeShowType, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kTagAndLayerSizeValueType, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kTagAndLayerSizeValuePixel, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kTagAndLayerSizeValuePercent, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kTagAndLayerLabelSize, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kTagAndLayerTagLabelColor, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kTagAndLayerLayerLabelColor, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kTagAndLayerAligment, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kTagAndLayerLabelAlpha, SettingsChanged);
			SettingsChanged();
		}
		void SettingsChanged()
		{
			enabled = Settings.Instance.Get<bool>( Setting.kTagAndLayerShow);
			editable = Settings.Instance.Get<bool>( Setting.kTagAndLayerEditable);
			showComponentDuringPlayMode = Settings.Instance.Get<bool>( Setting.kTagAndLayerShowDuringPlayMode);
			showAlways = Settings.Instance.Get<int>( Setting.kTagAndLayerType) == (int)HierarchyTagAndLayerType.kAlways;
			showType = (HierarchyTagAndLayerShowType)Settings.Instance.Get<int>( Setting.kTagAndLayerSizeShowType);
			sizeIsPixel = Settings.Instance.Get<int>( Setting.kTagAndLayerSizeValueType) == (int)HierarchyTagAndLayerSizeType.kPixel;
			pixelSize = Settings.Instance.Get<int>( Setting.kTagAndLayerSizeValuePixel);
			percentSize = Settings.Instance.Get<float>( Setting.kTagAndLayerSizeValuePercent);
			labelSize = (HierarchyTagAndLayerLabelSize)Settings.Instance.Get<int>( Setting.kTagAndLayerLabelSize);
			tagColor = Settings.Instance.GetColor( Setting.kTagAndLayerTagLabelColor);
			layerColor = Settings.Instance.GetColor( Setting.kTagAndLayerLayerLabelColor);
			labelAlpha = Settings.Instance.Get<float>( Setting.kTagAndLayerLabelAlpha);
			
			HierarchyTagAndLayerAligment aligment = (HierarchyTagAndLayerAligment)Settings.Instance.Get<int>( Setting.kTagAndLayerAligment);
			switch( aligment)
			{
				case HierarchyTagAndLayerAligment.kLeft:
				{
					labelStyle.alignment = TextAnchor.MiddleLeft;
					break;
				}
				case HierarchyTagAndLayerAligment.kCenter:
				{
					labelStyle.alignment = TextAnchor.MiddleCenter;
					break;
				}
				case HierarchyTagAndLayerAligment.kRight:
				{
					labelStyle.alignment = TextAnchor.MiddleRight;
					break;
				}
			}
		}
		public override LayoutStatus Layout( GameObject gameObject, Rect selectionRect, ref Rect curRect, float maxWidth)
		{
			float textWidth = sizeIsPixel ? pixelSize : percentSize * rect.x;
			rect.width = textWidth + 4;
			
			if( maxWidth < rect.width)
			{
				return LayoutStatus.kFailed;
			}
			else
			{
				curRect.x -= rect.width + 2;
				rect.x = curRect.x;
				rect.y = curRect.y;
				rect.y += (EditorGUIUtility.singleLineHeight - rect.height) * 0.5f;
				//rect.height = EditorGUIUtility.singleLineHeight;
				
				layer  = gameObject.layer; 
				tag = GetTagName( gameObject);			   
				
				needDrawTag = (showType != HierarchyTagAndLayerShowType.kLayer) && ((showAlways != false || tag != "Untagged"));
				needDrawLayer = (showType != HierarchyTagAndLayerShowType.kTag) && ((showAlways != false || layer != 0));
				
			#if UNITY_2019_1_OR_NEWER
				if( labelSize == HierarchyTagAndLayerLabelSize.kBig
				|| (labelSize == HierarchyTagAndLayerLabelSize.kBigIfSpecifiedOnlyTagOrLayer && needDrawTag != needDrawLayer))
				{
					labelStyle.fontSize = 8;
				}
				else
				{
					labelStyle.fontSize = 7;
				}
			#else
				if( labelSize == HierarchyTagAndLayerLabelSize.kBig
				|| (labelSize == HierarchyTagAndLayerLabelSize.kBigIfSpecifiedOnlyTagOrLayer && needDrawTag != needDrawLayer))
				{
					labelStyle.fontSize = 9;
				}
				else
				{
					labelStyle.fontSize = 8;
				}
			#endif
				if( needDrawTag)
				{
					tagRect.Set( rect.x, rect.y - (needDrawLayer ? 4 : 0), rect.width, rect.height);
				}
				if( needDrawLayer)
				{
					layerRect.Set( rect.x, rect.y + (needDrawTag ? 4 : 0), rect.width, rect.height);
				}
				return LayoutStatus.kSuccess;
			}
		}
		public override void Draw( GameObject gameObject, Rect selectionRect)
		{
			if( needDrawTag != false) 
			{
				tagColor.a = (tag == "Untagged")? labelAlpha : 1.0f;
				labelStyle.normal.textColor = tagColor;
				EditorGUI.LabelField( tagRect, tag, labelStyle);
			}
			
			if( needDrawLayer != false)
			{
				layerColor.a = (layer == 0)? labelAlpha : 1.0f;
				labelStyle.normal.textColor = layerColor;
				EditorGUI.LabelField( layerRect, GetLayerName( layer), labelStyle);
			}
		}
		public override void EventHandler( GameObject gameObject, Event currentEvent)
		{						
			if( currentEvent.type == EventType.MouseDown && Event.current.button == 0)
			{
				if( needDrawTag != false && needDrawLayer != false)
				{
					tagRect.height = 8;
					layerRect.height = 8;
					tagRect.y += 4;
					layerRect.y += 4;
				}
				if( needDrawTag != false && tagRect.Contains( Event.current.mousePosition) != false)
				{
					gameObjects = Selection.Contains( gameObject) ? Selection.gameObjects : new GameObject[]{ gameObject };
					ShowTagsContextMenu( tag);
					Event.current.Use();
				}
				else if( needDrawLayer != false && layerRect.Contains( currentEvent.mousePosition) != false)
				{
					gameObjects = Selection.Contains( gameObject) ? Selection.gameObjects : new GameObject[]{ gameObject };
					ShowLayersContextMenu( LayerMask.LayerToName( layer));
					Event.current.Use();
				}
			}
		}
		string GetTagName( GameObject gameObject)
		{
			string tag = "Undefined";
			try
			{
				tag = gameObject.tag;
			}
			catch {}
			return tag;
		}
		public string GetLayerName( int layer)
		{
			string layerName = LayerMask.LayerToName( layer);
			if( layerName.Equals( string.Empty) != false)
			{
				layerName = "Undefined";
			}
			return layerName;
		}
		void ShowTagsContextMenu(string tag)
		{
			List<string> tags = new List<string>( UnityEditorInternal.InternalEditorUtility.tags);
			
			GenericMenu menu = new GenericMenu();
			menu.AddItem( new GUIContent( "Untagged"), false, TagChangedHandler, "Untagged");
			
			for( int i0 = 0, n = tags.Count; i0 < n; ++i0)
			{
				string curTag = tags[ i0];
				menu.AddItem( new GUIContent( curTag), tag == curTag, TagChangedHandler, curTag);
			}
			
			menu.AddSeparator( string.Empty);
			menu.AddItem( new GUIContent( "Add Tag..."	), false, AddTagOrLayerHandler, "Tags");
			menu.ShowAsContext();
		}
		void ShowLayersContextMenu( string layer)
		{
			List<string> layers = new List<string>( UnityEditorInternal.InternalEditorUtility.layers);
			
			GenericMenu menu = new GenericMenu();
			menu.AddItem( new GUIContent( "Default"), false, LayerChangedHandler, "Default");
			
			for( int i0 = 0, n = layers.Count; i0 < n; ++i0)
			{
				string curLayer = layers[ i0];
				menu.AddItem( new GUIContent( curLayer), layer == curLayer, LayerChangedHandler, curLayer);
			}
			
			menu.AddSeparator( string.Empty);
			menu.AddItem( new GUIContent( "Add Layer..."), false, AddTagOrLayerHandler, "Layers");
			menu.ShowAsContext();
		}
		void TagChangedHandler( object newTag)
		{
			for( int i0 = gameObjects.Length - 1; i0 >= 0; --i0)
			{
				GameObject gameObject = gameObjects[ i0];
				Undo.RecordObject( gameObject, "Change Tag");
				gameObject.tag = newTag as string;
				EditorUtility.SetDirty( gameObject);
			}
		}
		void LayerChangedHandler( object newLayer)
		{
			int newLayerId = LayerMask.NameToLayer((string)newLayer);
			for( int i0 = gameObjects.Length - 1; i0 >= 0; --i0)
			{
				GameObject gameObject = gameObjects[ i0];
				Undo.RecordObject( gameObject, "Change Layer");
				gameObject.layer = newLayerId;
				EditorUtility.SetDirty( gameObject);
			}
		}
		void AddTagOrLayerHandler( object value)
		{
			PropertyInfo propertyInfo = typeof( EditorApplication).GetProperty( "tagManager", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetProperty);
			UnityEngine.Object obj = (UnityEngine.Object)(propertyInfo.GetValue( null, null));
			obj.GetType().GetField( "m_DefaultExpandedFoldout").SetValue( obj, value);
			Selection.activeObject = obj;
		}
		
		GUIStyle labelStyle;
		HierarchyTagAndLayerShowType showType;
		Color layerColor;
		Color tagColor; 	  
		bool showAlways;
		bool sizeIsPixel;
		int pixelSize;
		float percentSize;
		GameObject[] gameObjects;
		float labelAlpha;
		HierarchyTagAndLayerLabelSize labelSize;
		Rect tagRect;
		Rect layerRect;
		bool needDrawTag;
		bool needDrawLayer;
		int layer;
		string tag;
	}
}
