
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace Hierarchy
{
	public class SettingsWindow : EditorWindow 
	{
		[MenuItem ("Tools/Hierarchy/Settings")]	
		static void ShowWindow() 
		{
			EditorWindow window = EditorWindow.GetWindow( typeof( SettingsWindow));			
			window.minSize = new Vector2( 350, 50);
			
		#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0
			window.title = "Hierarchy Settings";
		#else
			window.titleContent = new GUIContent( "Hierarchy Settings");
		#endif
		}
		void Initialize() 
		{ 
			initialized		  = true;
			isProSkin		  = EditorGUIUtility.isProSkin;
			separatorColor	  = isProSkin ? new Color( 0.18f, 0.18f, 0.18f) : new Color( 0.59f, 0.59f, 0.59f);
			yellowColor 	  = isProSkin ? new Color( 1.00f, 0.90f, 0.40f) : new Color( 0.31f, 0.31f, 0.31f);
			checkBoxChecked   = Resources.Instance.GetTexture( Image.kCheckBoxChecked);
			checkBoxUnchecked = Resources.Instance.GetTexture( Image.kCheckBoxUnchecked);
			restoreButtonTexture = Resources.Instance.GetTexture( Image.kRestoreButton);
			componentsOrderList = new ComponentsOrderList( this);
		} 
		void OnGUI()
		{
			if( initialized == false || isProSkin != EditorGUIUtility.isProSkin)
			{
				Initialize();
			}
			
			indentLevel = 8;
			scrollPosition = EditorGUILayout.BeginScrollView( scrollPosition);
			{
				Rect targetRect = EditorGUILayout.GetControlRect( GUILayout.Height( 0));
				if( Event.current.type == EventType.Repaint)
				{
					totalWidth = targetRect.width + 8;
				}
				lastRect = new Rect( 0, 1, 0, 0);
				
				DrawSection( LabelComponentSetting);
				float sectionStartY = lastRect.y + lastRect.height;
				
				DrawTreeMapComponentSettings();
				DrawSeparator();
				DrawSeparatorComponentSettings();
				DrawSeparator();
				DrawActivityComponentSettings();
				DrawSeparator();
				DrawLockComponentSettings();
				DrawSeparator();
				DrawStaticComponentSettings();
				DrawSeparator();
				DrawErrorComponentSettings();
				DrawSeparator();
				DrawRendererComponentSettings();
				DrawSeparator();
				DrawTagLayerComponentSettings();
				DrawSeparator();
			//	drawChildrenCountComponentSettings();
			//	DrawSeparator();
			//	drawVerticesAndTrianglesCountComponentSettings();
			//	DrawSeparator();
				DrawComponentsComponentSettings();
				DrawLeftLine( sectionStartY, lastRect.y + lastRect.height, separatorColor);
				
				/* ORDER */
				DrawSection( LabelOrderOfComponents); 		
				sectionStartY = lastRect.y + lastRect.height;
				DrawSpace( 8);  
				DrawOrderSettings();
				DrawSpace( 6);	   
				DrawLeftLine( sectionStartY, lastRect.y + lastRect.height, separatorColor);
				
				/* ADDITIONAL */
				DrawSection( LabelAdditionalSettings); 			
				sectionStartY = lastRect.y + lastRect.height;
				DrawSpace( 3);  
				DrawAdditionalSettings();
				DrawLeftLine(sectionStartY, lastRect.y + lastRect.height + 4, separatorColor);
				
				indentLevel -= 1;
			}
			EditorGUILayout.EndScrollView();
		}
		void DrawTreeMapComponentSettings() 
		{
			if( DrawComponentCheckBox( LabelTreeMapComponent, Setting.kTreeMapShow) != false)
			{
				Rect rect = GetControlRect( 0, 0);
				if( DrawRestore() != false)
				{
					Settings.Instance.Restore( Setting.kTreeMapColor);
					Settings.Instance.Restore( Setting.kTreeMapEnhanced);
					Settings.Instance.Restore( Setting.kTreeMapTransparentBackground);
				}
				DrawBackground(  rect.x, rect.y, rect.width, 18 * 3 + 5);
				DrawSpace( 4);
				DrawColorPicker( LabelTreeMapColor, Setting.kTreeMapColor);
				DrawCheckBoxRight( LabelTreeMapTransparentBackground, Setting.kTreeMapTransparentBackground);
				DrawCheckBoxRight( LabelTreeMapEnhanced, Setting.kTreeMapEnhanced);
				DrawSpace( 1);
			}
		}
		void DrawSeparatorComponentSettings() 
		{
			if( DrawComponentCheckBox( LabelSeparatorComponent, Setting.kSeparatorShow) != false)
			{
				Rect rect = GetControlRect( 0, 0);
				if( DrawRestore() != false)
				{
					Settings.Instance.Restore( Setting.kSeparatorColor);
					Settings.Instance.Restore( Setting.kSeparatorShowRowShading);
					Settings.Instance.Restore( Setting.kSeparatorOddRowShadingColor);
					Settings.Instance.Restore( Setting.kSeparatorEvenRowShadingColor);
				}
				bool rowShading = Settings.Instance.Get<bool>( Setting.kSeparatorShowRowShading);
				
				DrawBackground( rect.x, rect.y, rect.width, 18 * (rowShading ? 4 : 2) + 5);
				DrawSpace( 4);
				DrawColorPicker( LabelSeparatorColor, Setting.kSeparatorColor);
				DrawCheckBoxRight( LabelSeparatorShowRowShading, Setting.kSeparatorShowRowShading);
				if( rowShading != false)
				{
					DrawColorPicker( LabelSeparatorEvenRowShadingColor, Setting.kSeparatorEvenRowShadingColor);
					DrawColorPicker( LabelSeparatorOddRowShadingColor, Setting.kSeparatorOddRowShadingColor);
				}
				DrawSpace( 1);
			}
		}
		void DrawActivityComponentSettings() 
		{
			if( DrawComponentCheckBox( LabelActivityComponent, Setting.kActivityShow))
			{
				Rect rect = GetControlRect( 0, 0);
				if( DrawRestore() != false)
				{
					Settings.Instance.Restore( Setting.kActivityEditable);
					Settings.Instance.Restore( Setting.kActivityShowDuringPlayMode);
				}
				DrawBackground( rect.x, rect.y, rect.width, 18 * 2 + 5);
				DrawSpace( 4);
				DrawCheckBoxRight( LabelActivityEditable, Setting.kActivityEditable);
				DrawCheckBoxRight( LabelActivityShowDuringPlayMode, Setting.kActivityShowDuringPlayMode);
				DrawSpace( 1);
			}
		}
		void DrawLockComponentSettings()
		{
			if( DrawComponentCheckBox( LabelLockComponent, Setting.kLockShow))
			{	
				Rect rect = GetControlRect( 0, 0);
				if( DrawRestore() != false)
				{
					Settings.Instance.Restore( Setting.kLockEditable);
					Settings.Instance.Restore( Setting.kLockShowDuringPlayMode);
				}
				DrawBackground( rect.x, rect.y, rect.width, 18 * 2 + 5);
				DrawSpace( 4);
				DrawCheckBoxRight( LabelLockEditable, Setting.kLockEditable);
				DrawCheckBoxRight( LabelLockShowDuringPlayMode, Setting.kLockShowDuringPlayMode);
				DrawSpace( 1);
			}
		}
		void DrawStaticComponentSettings() 
		{
			if( DrawComponentCheckBox( LabelStaticComponent, Setting.kStaticShow))
			{
				Rect rect = GetControlRect( 0, 0);
				if( DrawRestore() != false)
				{
					Settings.Instance.Restore( Setting.kStaticEditable);
					Settings.Instance.Restore( Setting.kStaticShowDuringPlayMode);
				}
				DrawBackground( rect.x, rect.y, rect.width, 18 * 2 + 5);
				DrawSpace( 4);
				DrawCheckBoxRight( LabelStaticEditable, Setting.kStaticEditable);
				DrawCheckBoxRight( LabelStaticShowDuringPlayMode, Setting.kStaticShowDuringPlayMode);
				DrawSpace( 1);
			}		 
		}
		void DrawErrorComponentSettings() 
		{
			if( DrawComponentCheckBox( LabelErrorComponent, Setting.kErrorShow))
			{
				Rect rect = GetControlRect( 0, 0);
				if( DrawRestore() != false)
				{
					Settings.Instance.Restore( Setting.kErrorShowDuringPlayMode);
					Settings.Instance.Restore( Setting.kErrorShowForDisabledComponents);
					Settings.Instance.Restore( Setting.kErrorShowForDisabledGameObjects);
					Settings.Instance.Restore( Setting.kErrorShowIconOnParent);
					Settings.Instance.Restore( Setting.kErrorShowWhenTagOrLayerIsUndefined);
					Settings.Instance.Restore( Setting.kErrorShowComponentIsMissing);
					Settings.Instance.Restore( Setting.kErrorShowReferenceIsMissing);
					Settings.Instance.Restore( Setting.kErrorIgnoreString);
				}
				DrawBackground( rect.x, rect.y, rect.width, 18 * 9 + 5);
				DrawSpace( 4);
				DrawCheckBoxRight( LabelErrorShowDuringPlayMode, Setting.kErrorShowDuringPlayMode);
				DrawCheckBoxRight( LabelErrorShowForDisabledGameObjects, Setting.kErrorShowForDisabledGameObjects);
				DrawCheckBoxRight( LabelErrorShowForDisabledComponents, Setting.kErrorShowForDisabledComponents);
				DrawCheckBoxRight( LabelErrorShowIconOnParent, Setting.kErrorShowIconOnParent);
				DrawLabel( LabelErrorFollowing);
				indentLevel += 16;
				DrawCheckBoxRight( LabelErrorShowWhenTagOrLayerIsUndefined, Setting.kErrorShowWhenTagOrLayerIsUndefined);
				DrawCheckBoxRight( LabelErrorShowComponentIsMissing, Setting.kErrorShowComponentIsMissing);
				DrawCheckBoxRight( LabelErrorShowReferenceIsMissing, Setting.kErrorShowReferenceIsMissing);
				indentLevel -= 16;
				DrawTextField( LabelErrorIgnoreString, Setting.kErrorIgnoreString);
				DrawSpace( 1);
			}
		}
		void DrawRendererComponentSettings() 
		{
			if( DrawComponentCheckBox( LabelRendererComponent, Setting.kRendererShow))
			{
				Rect rect = GetControlRect( 0, 0);
				if( DrawRestore() != false)
				{
					Settings.Instance.Restore( Setting.kRendererEditable);
					Settings.Instance.Restore( Setting.kRendererShowDuringPlayMode);
				}
				DrawBackground( rect.x, rect.y, rect.width, 18 * 2 + 5);
				DrawSpace( 4);
				DrawCheckBoxRight( LabelRendererEditable, Setting.kRendererEditable);
				DrawCheckBoxRight( LabelRendererShowDuringPlayMode, Setting.kRendererShowDuringPlayMode);
				DrawSpace( 1);
			}
		}
		void DrawTagLayerComponentSettings()
		{
			if( DrawComponentCheckBox( LabelTagAndLayerComponent, Setting.kTagAndLayerShow))
			{
				Rect rect = GetControlRect( 0, 0);
				if( DrawRestore() != false)
				{
					Settings.Instance.Restore( Setting.kTagAndLayerEditable);
					Settings.Instance.Restore( Setting.kTagAndLayerShowDuringPlayMode);
					Settings.Instance.Restore( Setting.kTagAndLayerSizeShowType);
					Settings.Instance.Restore( Setting.kTagAndLayerType);
					Settings.Instance.Restore( Setting.kTagAndLayerSizeValueType);
					Settings.Instance.Restore( Setting.kTagAndLayerSizeValuePixel);
					Settings.Instance.Restore( Setting.kTagAndLayerSizeValuePercent);
					Settings.Instance.Restore( Setting.kTagAndLayerAligment);
					Settings.Instance.Restore( Setting.kTagAndLayerLabelSize);
					Settings.Instance.Restore( Setting.kTagAndLayerLabelAlpha);
					Settings.Instance.Restore( Setting.kTagAndLayerTagLabelColor);
					Settings.Instance.Restore( Setting.kTagAndLayerLayerLabelColor);
				}
				DrawBackground( rect.x, rect.y, rect.width, 18 * 11 + 5);
				DrawSpace( 4);
				DrawCheckBoxRight( LabelTagAndLayerEditable, Setting.kTagAndLayerEditable);  
				DrawCheckBoxRight( LabelTagAndLayerShowDuringPlayMode, Setting.kTagAndLayerShowDuringPlayMode);  
				DrawEnum( LabelTagAndLayerSizeShowType, Setting.kTagAndLayerSizeShowType, typeof( HierarchyTagAndLayerShowType));
				DrawEnum( LabelTagAndLayerType, Setting.kTagAndLayerType, typeof( HierarchyTagAndLayerType));
				
				var newTagAndLayerSizeValueType = (HierarchyTagAndLayerSizeType)DrawEnum( 
						LabelTagAndLayerSizeValueType, Setting.kTagAndLayerSizeValueType, typeof( HierarchyTagAndLayerSizeType));
				
				if( newTagAndLayerSizeValueType == HierarchyTagAndLayerSizeType.kPixel)
				{
					DrawIntSlider( LabelTagAndLayerSizeValuePixel, Setting.kTagAndLayerSizeValuePixel, 5, 250);
				}
				else
				{
					DrawFloatSlider( LabelTagAndLayerSizeValuePercent, Setting.kTagAndLayerSizeValuePercent, 0, 0.5f);
				}
				
				DrawEnum( LabelTagAndLayerAligment, Setting.kTagAndLayerAligment, typeof( HierarchyTagAndLayerAligment));
				DrawEnum( LabelTagAndLayerLabelSize, Setting.kTagAndLayerLabelSize, typeof( HierarchyTagAndLayerLabelSize));
				DrawFloatSlider( LabelTagAndLayerLabelAlpha, Setting.kTagAndLayerLabelAlpha, 0, 1.0f);
				DrawColorPicker( LabelTagAndLayerTagLabelColor, Setting.kTagAndLayerTagLabelColor);
				DrawColorPicker( LabelTagAndLayerLayerLabelColor, Setting.kTagAndLayerLayerLabelColor);
				DrawSpace( 1);
			}
		}
#if false
		private void drawChildrenCountComponentSettings() 
		{
			if( DrawComponentCheckBox( "Children Count", Setting.kChildrenCountShow))
			{
				Rect rect = GetControlRect( 0, 0);
				if( DrawRestore() != false)
				{
					Settings.Instance.Restore( Setting.kChildrenCountShowDuringPlayMode);
					Settings.Instance.Restore( Setting.kChildrenCountLabelSize);
					Settings.Instance.Restore( Setting.kChildrenCountLabelColor);
				}
				DrawBackground( rect.x, rect.y, rect.width, 18 * 3 + 5);
				DrawSpace( 4);
				DrawCheckBoxRight( "Show component during play mode", Setting.kChildrenCountShowDuringPlayMode);
				DrawEnum( "Label size", Setting.kChildrenCountLabelSize, typeof(QHierarchySize));
				DrawColorPicker( "Label color", Setting.kChildrenCountLabelColor);
				DrawSpace( 1);
			}
		}
		private void drawVerticesAndTrianglesCountComponentSettings()
		{
			if( DrawComponentCheckBox( "Vertices And Triangles Count", Setting.kVerticesAndTrianglesShow))
			{
				Rect rect = GetControlRect( 0, 0);
				if( DrawRestore() != false)
				{
					Settings.Instance.Restore( Setting.kVerticesAndTrianglesShowDuringPlayMode);
					Settings.Instance.Restore( Setting.kVerticesAndTrianglesShowVertices);
					Settings.Instance.Restore( Setting.kVerticesAndTrianglesShowTriangles);
					Settings.Instance.Restore( Setting.kVerticesAndTrianglesCalculateTotalCount);
					Settings.Instance.Restore( Setting.kVerticesAndTrianglesLabelSize);
					Settings.Instance.Restore( Setting.kVerticesAndTrianglesVerticesLabelColor);
					Settings.Instance.Restore( Setting.kVerticesAndTrianglesTrianglesLabelColor);
				}
				DrawBackground( rect.x, rect.y, rect.width, 18 * 7 + 5);
				DrawSpace( 4);
				DrawCheckBoxRight( "Show component during play mode", Setting.kVerticesAndTrianglesShowDuringPlayMode);					 
				if (DrawCheckBoxRight( "Show vertices count", Setting.kVerticesAndTrianglesShowVertices))
				{
					if (Settings.Instance.Get<bool>(Setting.kVerticesAndTrianglesShowVertices) == false &&
						Settings.Instance.Get<bool>(Setting.kVerticesAndTrianglesShowTriangles) == false)					 
						Settings.Instance.set(Setting.kVerticesAndTrianglesShowTriangles, true);
				}
				if (DrawCheckBoxRight( "Show triangles count (very slow)", Setting.kVerticesAndTrianglesShowTriangles))
				{
					if (Settings.Instance.Get<bool>(Setting.kVerticesAndTrianglesShowVertices) == false &&
						Settings.Instance.Get<bool>(Setting.kVerticesAndTrianglesShowTriangles) == false)				  
						Settings.Instance.set(Setting.kVerticesAndTrianglesShowVertices, true);
				}
				DrawCheckBoxRight( "Calculate the count including children (very slow)", Setting.kVerticesAndTrianglesCalculateTotalCount);
				DrawEnum( "Label size", Setting.kVerticesAndTrianglesLabelSize, typeof(QHierarchySize));
				DrawColorPicker( "Vertices label color", Setting.kVerticesAndTrianglesVerticesLabelColor);
				DrawColorPicker( "Triangles label color", Setting.kVerticesAndTrianglesTrianglesLabelColor);
				DrawSpace( 1);
			}
		}
#endif
		void DrawComponentsComponentSettings() 
		{
			if( DrawComponentCheckBox( LabelComponentsComponent, Setting.kComponentsShow))
			{
				Rect rect = GetControlRect( 0, 0);
				if( DrawRestore() != false)
				{
					Settings.Instance.Restore( Setting.kComponentsEditable);
					Settings.Instance.Restore( Setting.kComponentsShowDuringPlayMode);
					Settings.Instance.Restore( Setting.kComponentsIconSize);
				}
				DrawBackground( rect.x, rect.y, rect.width, 18 * 4 + 6);
				DrawSpace( 4);
				DrawCheckBoxRight( LabelComponentsEditable, Setting.kComponentsEditable);
				DrawCheckBoxRight( LabelComponentsShowDuringPlayMode, Setting.kComponentsShowDuringPlayMode);
				DrawEnum( LabelComponentsIconSize, Setting.kComponentsIconSize, typeof( HierarchySizeAll));
				DrawTextField( LabelComponentsIgnore, Setting.kComponentsIgnore);
				DrawSpace( 2);
			}
		}
		void DrawOrderSettings()
		{
			if( DrawRestore() != false)
			{
				Settings.Instance.Restore( Setting.kComponentsOrder);
			}
			
			indentLevel += 4;
			
			string componentOrder = Settings.Instance.Get<string>(Setting.kComponentsOrder);
			string[] componentIds = componentOrder.Split( ';');
			
			Rect rect = GetControlRect( position.width, 17 * componentIds.Length + 10, 0, 0);
			if( componentsOrderList == null)
			{
				componentsOrderList = new ComponentsOrderList( this);
			}
			componentsOrderList.Draw( rect, componentIds);
			
			indentLevel -= 4;
		}  
		void DrawAdditionalSettings()
		{
			if( DrawRestore() != false)
			{
				Settings.Instance.Restore( Setting.kHierarchyExtension);
				Settings.Instance.Restore( Setting.kAdditionalHideIconsIfNotFit);
				Settings.Instance.Restore( Setting.kAdditionalIdentation);
				Settings.Instance.Restore( Setting.kAdditionalShowModifierWarning);
				Settings.Instance.Restore( Setting.kAdditionalBackgroundColor);
				Settings.Instance.Restore( Setting.kAdditionalActiveColor);
				Settings.Instance.Restore( Setting.kAdditionalInactiveColor);
			}
			DrawSpace( 4);
			
			DrawCheckBoxRight( LabelAdditionalHierarchyExtension, Setting.kHierarchyExtension);
			DrawCheckBoxRight( LabelAdditionalHideIconsIfNotFit, Setting.kAdditionalHideIconsIfNotFit);
			DrawIntSlider( LabelAdditionalIdentation, Setting.kAdditionalIdentation, 0, 500);	   
			DrawCheckBoxRight( LabelAdditionalShowModifierWarning, Setting.kAdditionalShowModifierWarning);
			DrawColorPicker( LabelAdditionalBackgroundColor, Setting.kAdditionalBackgroundColor);
			DrawColorPicker( LabelAdditionalActiveColor, Setting.kAdditionalActiveColor);
			DrawColorPicker( LabelAdditionalInactiveColor, Setting.kAdditionalInactiveColor);
			DrawSpace( 1);
		}
		
		void DrawSection( string title)
		{
			Rect rect = GetControlRect( 0, 24, -3, 0);
			rect.width *= 2;
			rect.x = 0;
			GUI.Box( rect, string.Empty);
			
			DrawLeftLine( rect.y, rect.y + 24, yellowColor);
			
			rect.x = lastRect.x + 8;
			rect.y += 4;
			EditorGUI.LabelField( rect, title);
		}
		void DrawSeparator( int spaceBefore=0, int spaceAfter=0, int height=1)
		{
			if( spaceBefore > 0)
			{
				DrawSpace( spaceBefore);
			}
			Rect rect = GetControlRect( 0, height, 0, 0);
			rect.width += 8;
			EditorGUI.DrawRect( rect, separatorColor);
			if( spaceAfter > 0)
			{
				DrawSpace( spaceAfter);
			}
		}
		bool DrawComponentCheckBox( string label, Setting setting)
		{
			indentLevel += 8;
			
			Rect rect = GetControlRect( 0, 28, 0, 0);
			
			float rectWidth = rect.width;
			bool isChecked = Settings.Instance.Get<bool>( setting);
			
			rect.x -= 1;
			rect.y += 7;
			rect.width	= 14;
			rect.height = 14;
			
			if( GUI.Button( rect, isChecked ? checkBoxChecked : checkBoxUnchecked, GUIStyle.none) != false)
			{
				Settings.Instance.Set( setting, !isChecked);
			}
			
			rect.x += 14 + 10;
			rect.width = rectWidth - 14 - 8;
			rect.y -= (EditorGUIUtility.singleLineHeight - rect.height) * 0.5f;
			rect.height = EditorGUIUtility.singleLineHeight;
			
				EditorGUI.LabelField( rect, label);
			
			indentLevel -= 8;
			
			return isChecked;
		}
		bool DrawCheckBoxRight( string label, Setting setting)
		{
			Rect rect = GetControlRect( 0, 18, 34, 6);
			bool result = false;
			bool isChecked = Settings.Instance.Get<bool>( setting);
			
			float tempX = rect.x;
			rect.x += rect.width - 14;
			rect.y += 1;
			rect.width	= 14;
			rect.height = 14;
			
			if( GUI.Button( rect, isChecked ? checkBoxChecked : checkBoxUnchecked, GUIStyle.none) != false)
			{
				Settings.Instance.Set( setting, !isChecked);
				result = true;
			}
			
			rect.width = rect.x - tempX - 4;
			rect.x = tempX;
			rect.y -= (EditorGUIUtility.singleLineHeight - rect.height) * 0.5f;
			rect.height = EditorGUIUtility.singleLineHeight;
			
			EditorGUI.LabelField( rect, label);
			
			return result;
		}
		void DrawSpace( int value)
		{
			GetControlRect( 0, value, 0, 0);
		}
		void DrawBackground( float x, float y, float width, float height)
		{
			EditorGUI.DrawRect( new Rect( x, y, width, height), separatorColor);
		}
		void DrawLeftLine( float fromY, float toY, Color color, float width = 0)
		{
			EditorGUI.DrawRect( new Rect( 0, fromY, (width == 0)? indentLevel : width, toY - fromY), color);
		}
		private Rect GetControlRect( float width, float height, float addIndent = 0, float remWidth = 0)
		{ 
			EditorGUILayout.GetControlRect(false, height, GUIStyle.none, GUILayout.ExpandWidth(true));
			Rect rect = new Rect(indentLevel + addIndent, lastRect.y + lastRect.height, (width == 0 ? totalWidth - indentLevel - addIndent - remWidth: width), height);
			lastRect = rect;
			return rect;
		}
		bool DrawRestore()
		{
			if( GUI.Button( new Rect( lastRect.x + lastRect.width - 16 - 5, lastRect.y - 20, 16, 16), restoreButtonTexture, GUIStyle.none) != false)
			{
				if( EditorUtility.DisplayDialog( LabelRestoreCaption, LabelRestoreMessage, "Ok", "Cancel"))
				{
					return true;
				}
			}
			return false;
		}
		
		void DrawLabel( string label)
		{
			Rect rect = GetControlRect( 0, 16, 34, 6);
			rect.y -= (EditorGUIUtility.singleLineHeight - rect.height) * 0.5f;
			EditorGUI.LabelField( rect, label);
			DrawSpace( 2);
		}
		void DrawTextField( string label, Setting setting)
		{
			string currentValue = Settings.Instance.Get<string>(　setting);
			string newValue = EditorGUI.TextField(　GetControlRect( 0, 16, 34, 6), label, currentValue);
			if( currentValue.Equals( newValue) == false)
			{
				Settings.Instance.Set( setting, newValue);
			}
			DrawSpace( 2);
		}
		bool DrawFoldout( string label, Setting setting)
		{
		#if UNITY_2019_1_OR_NEWER
			Rect foldoutRect = GetControlRect( 0, 16, 19, 6);
		#else
			Rect foldoutRect = GetControlRect( 0, 16, 22, 6);
		#endif
			bool foldoutValue = Settings.Instance.Get<bool>( setting);
			bool newFoldoutValue = EditorGUI.Foldout( foldoutRect, foldoutValue, label);
			if( foldoutValue != newFoldoutValue)
			{
				Settings.Instance.Set( setting, newFoldoutValue);
			}
			DrawSpace( 2);
			return newFoldoutValue;
		}
		void DrawColorPicker( string label, Setting setting)
		{
			Color currentColor = Settings.Instance.GetColor( setting);
			Color newColor = EditorGUI.ColorField( GetControlRect( 0, 16, 34, 6), label, currentColor);
			if( currentColor.Equals( newColor) == false)
			{
				Settings.Instance.SetColor( setting, newColor);
			}
			DrawSpace( 2);
		}
		Enum DrawEnum( string label, Setting setting, Type enumType)
		{
			Enum currentEnum = (Enum)Enum.ToObject( enumType, Settings.Instance.Get<int>( setting));
			Enum newEnumValue;
			
			if( (newEnumValue = EditorGUI.EnumPopup( GetControlRect( 0, 16, 34, 6), label, currentEnum)).Equals( currentEnum) == false)
			{
				Settings.Instance.Set( setting, (int)(object)newEnumValue);
			}
			DrawSpace( 2);
			return newEnumValue;
		}
		void DrawIntSlider( string label, Setting setting, int minValue, int maxValue)
		{
			Rect rect = GetControlRect( 0, 16, 34, 4);
			int currentValue = Settings.Instance.Get<int>( setting);
			int newValue = EditorGUI.IntSlider( rect, label, currentValue, minValue, maxValue);
			if( currentValue != newValue)
			{
				Settings.Instance.Set( setting, newValue);
			}
			DrawSpace( 2);
		}
		void DrawFloatSlider( string label, Setting setting, float minValue, float maxValue)
		{
			Rect rect = GetControlRect( 0, 16, 34, 4);
			float currentValue = Settings.Instance.Get<float>( setting);
			float newValue = EditorGUI.Slider( rect, label, currentValue, minValue, maxValue);
			if( currentValue != newValue)
			{
				Settings.Instance.Set( setting, newValue);
			}
			DrawSpace( 2);
		}
		
		/* COMPONENTS */
		string LabelComponentSetting{
			get{ switch( language){
				case 1: return "拡張機能設定";
				default: return "COMPONENTS SETTINGS";
		}}}
		/* TreeMapComponent */
		string LabelTreeMapComponent{
			get{ switch( language){
				case 1: return "ツリービュー";
				default: return "Hierarchy Tree";
		}}}
		string LabelTreeMapColor{
			get{ switch( language){
				case 1: return "ノードラインカラー";
				default: return "Node line color";
		}}}
		string LabelTreeMapTransparentBackground{
			get{ switch( language){
				case 1: return "透明な背景";
				default: return "Transparent background";
		}}}
		string LabelTreeMapEnhanced{
			get{ switch( language){
				case 1: return "線を終端で止める";
				default: return "Enhanced (\"Transform Sort\" only)";
		}}}
		/* SeparatorComponent */
		string LabelSeparatorComponent{
			get{ switch( language){
				case 1: return "セパレーター";
				default: return "Separator";
		}}}
		string LabelSeparatorColor{
			get{ switch( language){
				case 1: return "セパレーターカラー";
				default: return "Separator Color";
		}}}
		string LabelSeparatorShowRowShading{
			get{ switch( language){
				case 1: return "行の網掛け";
				default: return "Row shading";
		}}}
		string LabelSeparatorEvenRowShadingColor{
			get{ switch( language){
				case 1: return "偶数行の網掛け色";
				default: return "Even row shading color";
		}}}
		string LabelSeparatorOddRowShadingColor{
			get{ switch( language){
				case 1: return "奇数行の網掛け色";
				default: return "Odd row shading color";
		}}}
		/* ActivityComponent */
		string LabelActivityComponent{
			get{ switch( language){
				case 1: return "アクティブ";
				default: return "Activity";
		}}}
		string LabelActivityEditable{
			get{ switch( language){
				case 1: return "編集可能";
				default: return "Editable";
		}}}
		string LabelActivityShowDuringPlayMode{
			get{ switch( language){
				case 1: return "再生モード中にも動作する";
				default: return "Show component during play mode";
		}}}
		/* LockComponent */
		string LabelLockComponent{
			get{ switch( language){
				case 1: return "Inspector上の編集抑止";
				default: return "Lock";
		}}}
		string LabelLockEditable{
			get{ switch( language){
				case 1: return "編集可能";
				default: return "Editable";
		}}}
		string LabelLockShowDuringPlayMode{
			get{ switch( language){
				case 1: return "再生モード中にも動作する";
				default: return "Show component during play mode";
		}}}
		/* StaticComponent */
		string LabelStaticComponent{
			get{ switch( language){
				case 1: return "静的ゲームオブジェクト";
				default: return "Static";
		}}}
		string LabelStaticEditable{
			get{ switch( language){
				case 1: return "編集可能";
				default: return "Editable";
		}}}
		string LabelStaticShowDuringPlayMode{
			get{ switch( language){
				case 1: return "再生モード中にも動作する";
				default: return "Show component during play mode";
		}}}
		/* ErrorComponent */
		string LabelErrorComponent{
			get{ switch( language){
				case 1: return "エラー判定";
				default: return "Error";
		}}}
		string LabelErrorShowDuringPlayMode{
			get{ switch( language){
				case 1: return "再生モード中にも動作する";
				default: return "Show component during play mode";
		}}}
		string LabelErrorShowForDisabledGameObjects{
			get{ switch( language){
				case 1: return "無効化されてるゲームオブジェクトも対象にする";
				default: return "Show error icon for disabled GameObjects";
		}}}
		string LabelErrorShowForDisabledComponents{
			get{ switch( language){
				case 1: return "無効化されてるコンポーネントも対象にする";
				default: return "Show error icon for disabled components";
		}}}
		string LabelErrorShowIconOnParent{
			get{ switch( language){
				case 1: return "[高負荷]親階層にもエラーアイコンを表示する";
				default: return "Show error icon up of hierarchy (very slow)";
		}}}
		string LabelErrorFollowing{
			get{ switch( language){
				case 1: return "エラーとして扱う状態:";
				default: return "Show error icon for the following:";
		}}}
		string LabelErrorShowWhenTagOrLayerIsUndefined{
			get{ switch( language){
				case 1: return "- タグまたはレイヤーが未定義";
				default: return "- tag or layer is undefined";
		}}}
		string LabelErrorShowComponentIsMissing{
			get{ switch( language){
				case 1: return "- コンポーネントが Missing";
				default: return "- component is missing";
		}}}
		string LabelErrorShowReferenceIsMissing{
			get{ switch( language){
				case 1: return "- [高負荷]参照が Missing";
				default: return "- reference is missing (very slow)";
		}}}
		string LabelErrorIgnoreString{
			get{ switch( language){
				case 1: return "対象外コンポーネント名";
				default: return "Ignore packages/classes";
		}}}
		/* RendererComponent */
		string LabelRendererComponent{
			get{ switch( language){
				case 1: return "レンダラー";
				default: return "Renderer";
		}}}
		string LabelRendererEditable{
			get{ switch( language){
				case 1: return "編集可能";
				default: return "Editable";
		}}}
		string LabelRendererShowDuringPlayMode{
			get{ switch( language){
				case 1: return "再生モード中にも動作する";
				default: return "Show component during play mode";
		}}}
		/* TagAndLayerComponent */
		string LabelTagAndLayerComponent{
			get{ switch( language){
				case 1: return "タグ＆レイヤー";
				default: return "TagAndLayer";
		}}}
		string LabelTagAndLayerEditable{
			get{ switch( language){
				case 1: return "編集可能";
				default: return "Editable";
		}}}
		string LabelTagAndLayerShowDuringPlayMode{
			get{ switch( language){
				case 1: return "再生モード中にも動作する";
				default: return "Show component during play mode";
		}}}
		string LabelTagAndLayerSizeShowType{
			get{ switch( language){
				case 1: return "表示形式";
				default: return "Show";
		}}}
		string LabelTagAndLayerType{
			get{ switch( language){
				case 1: return "初期値時の表示";
				default: return "Show tag and layer";
		}}}
		string LabelTagAndLayerSizeValueType{
			get{ switch( language){
				case 1: return "表示幅の単位";
				default: return "Unit of width";
		}}}
		string LabelTagAndLayerSizeValuePixel{
			get{ switch( language){
				case 1: return "幅(px)";
				default: return "Width in pixels";
		}}}
		string LabelTagAndLayerSizeValuePercent{
			get{ switch( language){
				case 1: return "幅(％)";
				default: return "Percentage width";
		}}}
		string LabelTagAndLayerAligment{
			get{ switch( language){
				case 1: return "水平方向の表示基点";
				default: return "Alignment";
		}}}
		string LabelTagAndLayerLabelSize{
			get{ switch( language){
				case 1: return "表示サイズ";
				default: return "Label size";
		}}}
		string LabelTagAndLayerLabelAlpha{
			get{ switch( language){
				case 1: return "初期値時の不透明度";
				default: return "Label alpha if default";
		}}}
		string LabelTagAndLayerTagLabelColor{
			get{ switch( language){
				case 1: return "タグの文字色";
				default: return "Tag label color";
		}}}
		string LabelTagAndLayerLayerLabelColor{
			get{ switch( language){
				case 1: return "レイヤーの文字色";
				default: return "Layer label color";
		}}}
		/* ComponentsComponent */
		string LabelComponentsComponent{
			get{ switch( language){
				case 1: return "コンポーネント";
				default: return "Components";
		}}}
		string LabelComponentsEditable{
			get{ switch( language){
				case 1: return "編集可能";
				default: return "Editable";
		}}}
		string LabelComponentsShowDuringPlayMode{
			get{ switch( language){
				case 1: return "再生モード中にも動作する";
				default: return "Show component during play mode";
		}}}
		string LabelComponentsIconSize{
			get{ switch( language){
				case 1: return "アイコンの表示サイズ";
				default: return "Icon size";
		}}}
		string LabelComponentsIgnore{
			get{ switch( language){
				case 1: return "非表示コンポーネント名";
				default: return "Ignore packages/classes";
		}}}
		/* ORDER */
		string LabelOrderOfComponents{
			get{ switch( language){
				case 1: return "拡張機能の表示順序";
				default: return "ORDER OF COMPONENTS";
		}}}
		/* ADDITIONAL */
		string LabelAdditionalSettings{
			get{ switch( language){
				case 1: return "その他";
				default: return "ADDITIONAL SETTINGS";
		}}}
		string LabelAdditionalHierarchyExtension{
			get{ switch( language){
				case 1: return "拡張機能を有効にする";
				default: return "Enable extensions";
		}}}
		string LabelAdditionalHideIconsIfNotFit{
			get{ switch( language){
				case 1: return "収まらないアイコンを非表示にする";
				default: return "Hide icons if not fit";
		}}}
		string LabelAdditionalIdentation{
			get{ switch( language){
				case 1: return "オフセット幅(px)";
				default: return "Right indent";
		}}}
		string LabelAdditionalShowModifierWarning{
			get{ switch( language){
				case 1: return "修飾子を使用して状態を切り替える際に警告を表示する";
				default: return "Show warning when using modifiers + click";
		}}}
		string LabelAdditionalBackgroundColor{
			get{ switch( language){
				case 1: return "背景色";
				default: return "Background color";
		}}}
		string LabelAdditionalActiveColor{
			get{ switch( language){
				case 1: return "有効時の色";
				default: return "Active color";
		}}}
		string LabelAdditionalInactiveColor{
			get{ switch( language){
				case 1: return "無効時の色";
				default: return "Inactive color";
		}}}
		
		/* Restore */
		string LabelRestoreCaption{
			get{ switch( language){
				case 1: return "復元";
				default: return "Restore";
		}}}
		string LabelRestoreMessage{
			get{ switch( language){
				case 1: return "デフォルトの設定を復元しますか？";
				default: return "Restore default settings?";
		}}}
		
		bool initialized = false;
		int language = 1;
		Rect lastRect;
		bool isProSkin;
		int indentLevel;
		Texture2D checkBoxChecked;
		Texture2D checkBoxUnchecked;
		Texture2D restoreButtonTexture;
		Vector2 scrollPosition = new Vector2();
		Color separatorColor;
		Color yellowColor;
		float totalWidth;
		ComponentsOrderList componentsOrderList;
	}
}