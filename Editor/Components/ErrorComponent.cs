
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Hierarchy
{
	public class ErrorComponent : BaseComponent
	{
		public ErrorComponent()
		{
			rect.width = 16; 

			errorIconTexture = Resources.Instance.GetTexture( Image.kErrorIcon);
			
			Settings.Instance.AddEventListener( Setting.kErrorShow						   , SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kErrorShowDuringPlayMode		   , SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kErrorShowForDisabledComponents    , SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kErrorShowForDisabledGameObjects   , SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kErrorShowIconOnParent			   , SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kErrorShowWhenTagOrLayerIsUndefined, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kErrorShowComponentIsMissing	   , SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kErrorShowReferenceIsMissing	   , SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kErrorIgnoreString				   , SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kAdditionalActiveColor			   , SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kAdditionalInactiveColor		   , SettingsChanged);
			SettingsChanged();
		}
		void SettingsChanged()
		{
			editable						= true;
			enabled 						= Settings.Instance.Get<bool>( Setting.kErrorShow);
			showComponentDuringPlayMode 	= Settings.Instance.Get<bool>( Setting.kErrorShowDuringPlayMode);
			showErrorForDisabledComponents	= Settings.Instance.Get<bool>( Setting.kErrorShowForDisabledComponents);
			showErrorForDisabledGameObjects = Settings.Instance.Get<bool>( Setting.kErrorShowForDisabledGameObjects);
			showErrorOfChildren 			= Settings.Instance.Get<bool>( Setting.kErrorShowIconOnParent);
			showErrorIconWhenTagIsUndefined = Settings.Instance.Get<bool>( Setting.kErrorShowWhenTagOrLayerIsUndefined);
			showErrorIconComponentIsMissing	= Settings.Instance.Get<bool>( Setting.kErrorShowComponentIsMissing);
			showErrorTypeReferenceIsMissing = Settings.Instance.Get<bool>( Setting.kErrorShowReferenceIsMissing);
			activeColor 					= Settings.Instance.GetColor( Setting.kAdditionalActiveColor);
			inactiveColor					= Settings.Instance.GetColor( Setting.kAdditionalInactiveColor);

			string ignoreErrorOfMonoBehavioursString = Settings.Instance.Get<string>( Setting.kErrorIgnoreString);
			if( string.IsNullOrEmpty( ignoreErrorOfMonoBehavioursString) == false) 
			{
				ignoreErrorOfMonoBehaviours = new List<string>( ignoreErrorOfMonoBehavioursString.Split( new char[]{ ',', ';', '.', ' ' }));
				ignoreErrorOfMonoBehaviours.RemoveAll( x => x == string.Empty);
			}
			else
			{
				ignoreErrorOfMonoBehaviours = null;
			}
		}
		public override LayoutStatus Layout( GameObject gameObject, Rect selectionRect, ref Rect curRect, float maxWidth)
		{
			if( maxWidth < 16) 
			{
				return LayoutStatus.kFailed;
			}
			curRect.x -= 16;
			rect.x = curRect.x;
			rect.y = curRect.y;
			return LayoutStatus.kSuccess;
		}
		public override void Draw( GameObject gameObject, Rect selectionRect)
		{
			if( showErrorForDisabledGameObjects != false || gameObject.activeSelf != false)
			{
				bool errorFound = FindError( gameObject, gameObject.GetComponents<Component>());
			//	bool errorFound = FindError( gameObject, gameObject.GetComponents<MonoBehaviour>());

				if( errorFound != false)
				{			
					ColorUtils.SetColor( activeColor);
					GUI.DrawTexture( rect, errorIconTexture);
					ColorUtils.ClearColor();
				}
				else if( showErrorOfChildren != false)
				{
					errorFound = FindError( gameObject, gameObject.GetComponentsInChildren<Component>( true));
				//	errorFound = FindError( gameObject, gameObject.GetComponentsInChildren<MonoBehaviour>( true));
					if( errorFound != false)
					{
						ColorUtils.SetColor( inactiveColor);
						GUI.DrawTexture( rect, errorIconTexture);
						ColorUtils.ClearColor();
					}
				}			 
			}
		}
		public override void EventHandler( GameObject gameObject, Event currentEvent)
		{
			if( showErrorForDisabledGameObjects != false || gameObject.activeSelf != false)
			{
				if( currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && rect.Contains( currentEvent.mousePosition) != false)
				{
					currentEvent.Use();

					errorCount = 0;
					errorStringBuilder = new System.Text.StringBuilder();
					
					FindError( gameObject, gameObject.GetComponents<Component>(), true);
				//	FindError( gameObject, gameObject.GetComponents<MonoBehaviour>(), true);

					if( errorCount > 0)
					{
						EditorUtility.DisplayDialog( 
							errorCount + (errorCount == 1 ? " error was found" : " errors were found"), 
							errorStringBuilder.ToString(), "OK");
					}
				}
			}
		}
		bool FindError( GameObject gameObject, Component[] components, bool printError=false)
		{
			/* タグ、またはレイヤーが未定義だった場合 */
			if( showErrorIconWhenTagIsUndefined != false)
			{
				try
				{ 
					gameObject.tag.CompareTo( null); 
				}
				catch 
				{
					if( printError != false)
					{
						AppendErrorLine( "Tag is undefined");
					}
					else
					{
						return true;
					}
				}
				if( LayerMask.LayerToName( gameObject.layer).Equals( string.Empty) != false)
				{
					if( printError != false)
					{
						AppendErrorLine( "Layer is undefined");
					}
					else
					{
						return true;
					}
				}
			}
			for( int i0 = 0; i0 < components.Length; ++i0)
			{
				Component component = components[ i0];
				
				if( component == null)
				{
					/* コンポーネントが null の場合 Missing として扱う */
					if( showErrorIconComponentIsMissing != false)
					{
						if( printError != false)
						{
							AppendErrorLine( "Component #" + i0 + " is missing");
						}
						else
						{
							return true;
						}
					}
				}
				else if( showErrorTypeReferenceIsMissing != false)
				{
					/* 対象外のコンポーネントをエラー探査対象から除去する */
					if( ignoreErrorOfMonoBehaviours != null)
					{
						int i1;
						
						for( i1 = ignoreErrorOfMonoBehaviours.Count - 1; i1 >= 0; --i1)
						{
							if( component.GetType().FullName.Contains( ignoreErrorOfMonoBehaviours[ i1]) != false)
							{
								break;
							} 
						}
						if( i1 >= 0)
						{
							continue;
						}
					}
					
					var serializedObject = new SerializedObject( component);
					SerializedProperty property = serializedObject.GetIterator();
					
					while( property.Next( true))
					{
						if( property.propertyType == SerializedPropertyType.ObjectReference)
						{
							if( property.objectReferenceValue == null)
							{
								bool missing = false;
								
								if( property.objectReferenceInstanceIDValue != 0)
								{
									missing = true;
								}
								else
								{
									if( property.hasChildren != false)
									{
										SerializedProperty fileIdProperty = property.FindPropertyRelative( "m_FileID");
										if( fileIdProperty != null && fileIdProperty.intValue != 0)
										{
											missing = true;
										}
									}
								}
								if( missing != false)
								{
									if( printError != false)
									{
										AppendErrorLine( string.Format( 
											$"{component.GetType().Name}.{property.displayName}: Reference is missing"));
									}
									else
									{
										return true;
									}
								}
							}
						}
					}
				}
			}
			return false;
		}
		void AppendErrorLine(string error)
		{
			errorCount++;
			errorStringBuilder.Append(errorCount.ToString());
			errorStringBuilder.Append(") ");
			errorStringBuilder.AppendLine(error);
		}
		Color activeColor;
		Color inactiveColor;
		Texture2D errorIconTexture;
		bool showErrorOfChildren;
		bool showErrorTypeReferenceIsNull;
		bool showErrorTypeReferenceIsMissing;
		bool showErrorTypeStringIsEmpty;
		bool showErrorIconComponentIsMissing;
		bool showErrorIconWhenTagIsUndefined;
		bool showErrorForDisabledComponents;
		bool showErrorIconMissingEventMethod;
		bool showErrorForDisabledGameObjects;
		List<string> ignoreErrorOfMonoBehaviours;
		System.Text.StringBuilder errorStringBuilder;
		int errorCount;
	}
}
