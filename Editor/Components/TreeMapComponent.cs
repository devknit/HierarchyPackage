
using UnityEngine;
using UnityEditor;

namespace Hierarchy
{
	public class TreeMapComponent: BaseComponent
	{
		public TreeMapComponent()
		{
			treeMapLevelTexture = Resources.Instance.GetTexture( Image.kTreeMapLevel);
			treeMapLevel4Texture = Resources.Instance.GetTexture( Image.kTreeMapLevel4);
			treeMapCurrentTexture = Resources.Instance.GetTexture( Image.kTreeMapCurrent);
		#if UNITY_2018_3_OR_NEWER
			treeMapObjectTexture = Resources.Instance.GetTexture( Image.kTreeMapLine);
		#else
			treeMapObjectTexture = Resources.Instance.GetTexture( Image.kTreeMapObject);
		#endif
			treeMapLastTexture = Resources.Instance.GetTexture( Image.kTreeMapLast);
			
			rect.width	= 14;
			rect.height = 16;
			
			showComponentDuringPlayMode = true;
			
			Settings.Instance.AddEventListener( Setting.kAdditionalBackgroundColor, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kTreeMapShow, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kTreeMapColor, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kTreeMapEnhanced, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kTreeMapTransparentBackground, SettingsChanged);
			SettingsChanged();
		}
		void SettingsChanged()
		{
			enabled = Settings.Instance.Get<bool>( Setting.kTreeMapShow);
			enhanced = Settings.Instance.Get<bool>( Setting.kTreeMapEnhanced);
			treeMapColor = Settings.Instance.GetColor( Setting.kTreeMapColor);
			backgroundColor = Settings.Instance.GetColor( Setting.kAdditionalBackgroundColor);
			transparentBackground = Settings.Instance.Get<bool>( Setting.kTreeMapTransparentBackground);
		}
		public override LayoutStatus Layout( GameObject gameObject, Rect selectionRect, ref Rect curRect, float maxWidth)
		{
			rect.y = selectionRect.y;
			
			if( transparentBackground == false)
			{
				rect.x = 0;
				rect.width = selectionRect.x - 14;
				EditorGUI.DrawRect( rect, backgroundColor);
				rect.width = 14;
			}
			return LayoutStatus.kSuccess;
		}
		public override void Draw( GameObject gameObject, Rect selectionRect)
		{
			int childCount = gameObject.transform.childCount;
			int level = Mathf.RoundToInt( selectionRect.x / 14.0f);
			
			if( enhanced != false)
			{
				Transform gameObjectTransform = gameObject.transform;
				Transform parentTransform = null;
				
				for( int i0 = 0, i1 = level - 1; i1 >= 0; ++i0, --i1)
				{
					rect.x = 14 * i1;
					if( i0 == 0)
					{
						if( childCount == 0)
						{
						#if UNITY_2018_3_OR_NEWER
							ColorUtils.SetColor( treeMapColor);
						#endif
							GUI.DrawTexture( rect, treeMapObjectTexture);
						}
						gameObjectTransform = gameObject.transform;
					}
					else if( i0 == 1)
					{
						ColorUtils.SetColor( treeMapColor);
						if( parentTransform == null)
						{
							if( gameObjectTransform.GetSiblingIndex() == gameObject.scene.rootCount - 1)
							{
								GUI.DrawTexture( rect, treeMapLastTexture);
							}
							else
							{
								GUI.DrawTexture( rect, treeMapCurrentTexture);
							}
						}
						else if( gameObjectTransform.GetSiblingIndex() == parentTransform.childCount - 1)
						{
							GUI.DrawTexture( rect, treeMapLastTexture);
						}
						else
						{
							GUI.DrawTexture( rect, treeMapCurrentTexture);
						}
						gameObjectTransform = parentTransform;
					}
					else
					{
						if( parentTransform == null)
						{
							if( gameObjectTransform.GetSiblingIndex() != gameObject.scene.rootCount - 1)
							{
								GUI.DrawTexture( rect, treeMapLevelTexture);
							}
						}
						else if( gameObjectTransform.GetSiblingIndex() != parentTransform.childCount - 1)
						{
							GUI.DrawTexture(rect, treeMapLevelTexture);
						}
						gameObjectTransform = parentTransform;						 
					}
					if( gameObjectTransform != null)
					{
						parentTransform = gameObjectTransform.parent;
					}
					else
					{
						break;
					}
				}
				ColorUtils.ClearColor();
			}
			else
			{
				for( int i0 = 0, i1 = level - 1; i1 >= 0; ++i0, --i1)
				{
					rect.x = 14 * i1;
					if( i0 == 0)
					{
						if( childCount > 0)
						{
							continue;
						}
						else
						{
						#if UNITY_2018_3_OR_NEWER
							ColorUtils.SetColor( treeMapColor);
						#endif
							GUI.DrawTexture( rect, treeMapObjectTexture);
						}
					}
					else if( i0 == 1)
					{
						ColorUtils.SetColor( treeMapColor);
						GUI.DrawTexture( rect, treeMapCurrentTexture);
					}
					else
					{
						rect.width = 14 * 4;
						rect.x -= 14 * 3;
						i1 -= 3;
						GUI.DrawTexture( rect, treeMapLevel4Texture);
						rect.width = 14;
					}
				}
				ColorUtils.ClearColor();
			}
		}
		bool enhanced;
		bool transparentBackground;
		Color treeMapColor;
		Color backgroundColor;
		Texture2D treeMapLevelTexture;		 
		Texture2D treeMapLevel4Texture; 	  
		Texture2D treeMapCurrentTexture;   
		Texture2D treeMapLastTexture;
		Texture2D treeMapObjectTexture;    
	}
}
