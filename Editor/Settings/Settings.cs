
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Hierarchy
{
	public enum Setting
	{
		kHierarchyExtension							 = 0,
		
		kTreeMapShow								 = 1,
		kTreeMapColor								 = 101,
		kTreeMapEnhanced							 = 102,
		kTreeMapTransparentBackground				 = 103,

		kSeparatorShow								 = 2,
		kSeparatorShowRowShading					 = 201,
		kSeparatorColor 							 = 202,
		kSeparatorEvenRowShadingColor				 = 203, 	  
		kSeparatorOddRowShadingColor				 = 204, 	  

		kActivityShow								 = 3,
		kActivityEditable							 = 301,
		kActivityShowDuringPlayMode 				 = 302,

		kLockShow									 = 4,
		kLockEditable								 = 401,
		kLockShowDuringPlayMode 					 = 402,

		kStaticShow 								 = 5,
		kStaticEditable 							 = 501,
		kStaticShowDuringPlayMode					 = 502,

		kErrorShow									 = 6,
		kErrorShowDuringPlayMode					 = 601,
		kErrorShowForDisabledComponents 			 = 602,
		kErrorShowForDisabledGameObjects			 = 603,
		kErrorShowIconOnParent						 = 604,
		kErrorShowWhenTagOrLayerIsUndefined 		 = 605,
		kErrorShowComponentIsMissing				 = 606,
		kErrorShowReferenceIsMissing				 = 607,
		kErrorIgnoreString							 = 608,

		kRendererShow								 = 7,
		kRendererEditable							 = 701,
		kRendererShowDuringPlayMode 				 = 702,

		kTagAndLayerShow							 = 8,
		kTagAndLayerEditable						 = 801,
		kTagAndLayerShowDuringPlayMode				 = 802,
		kTagAndLayerSizeShowType					 = 803,
		kTagAndLayerType							 = 804,
		kTagAndLayerSizeType						 = 805,
		kTagAndLayerSizeValuePixel					 = 806,
		kTagAndLayerAligment						 = 807,
		kTagAndLayerSizeValueType					 = 808,
		kTagAndLayerSizeValuePercent				 = 809,
		kTagAndLayerLabelSize						 = 810,
		kTagAndLayerTagLabelColor					 = 811,
		kTagAndLayerLayerLabelColor 				 = 812,
		kTagAndLayerLabelAlpha						 = 813,

		kChildrenCountShow							 = 9,
		kChildrenCountShowDuringPlayMode			 = 901,
		kChildrenCountLabelSize 					 = 902,
		kChildrenCountLabelColor					 = 903,

		kVerticesAndTrianglesShow					 = 10,
		kVerticesAndTrianglesShowDuringPlayMode 	 = 1001,
		kVerticesAndTrianglesCalculateTotalCount	 = 1002,
		kVerticesAndTrianglesShowTriangles			 = 1003, 
		kVerticesAndTrianglesShowVertices			 = 1004, 
		kVerticesAndTrianglesLabelSize				 = 1005,
		kVerticesAndTrianglesVerticesLabelColor 	 = 1006,
		kVerticesAndTrianglesTrianglesLabelColor	 = 1007,

		kComponentsShow 							 = 11,
		kComponentsEditable							 = 1101,
		kComponentsShowDuringPlayMode				 = 1102,
		kComponentsIconSize 						 = 1103,
		kComponentsIgnore							 = 1104,

		kComponentsOrder							 = 12,

		kAdditionalIdentation						 = 99,
		kAdditionalShowModifierWarning				 = 9901,
		kAdditionalHideIconsIfNotFit				 = 9902,  
		kAdditionalBackgroundColor					 = 9903,
		kAdditionalActiveColor						 = 9904,
		kAdditionalInactiveColor					 = 9905,
	}
	public enum HierarchyTagAndLayerType
	{
		kAlways 		  = 0,
		kOnlyIfNotDefault = 1
	}
	public enum HierarchyTagAndLayerShowType
	{
		kTagAndLayer = 0,
		kTag		 = 1,
		kLayer		 = 2
	}
	public enum HierarchyTagAndLayerAligment
	{
		kLeft	= 0,
		kCenter = 1,
		kRight	= 2
	}
	public enum HierarchyTagAndLayerSizeType
	{
		kPixel	 = 0,
		kPercent = 1
	}
	public enum HierarchyTagAndLayerLabelSize
	{
		kNormal 						 = 0,
		kBig							 = 1,
		kBigIfSpecifiedOnlyTagOrLayer	 = 2
	}
	public enum HierarchySize
	{
		kNormal  = 0,
		kBig	 = 1
	}
	public enum HierarchySizeAll
	{
		kSmall	 = 0,
		kNormal  = 1,
		kBig	 = 2
	}
	public enum TargetComponent
	{
		kComponent		= 0,
		kMonoBehaviour	= 1,
	}
	public enum HierarchyComponentEnum
	{
		kLockComponent				 = 0,
		kActivityComponent			 = 1,
		kStaticComponent			 = 2,
		kErrorComponent 			 = 3,
		kRendererComponent			 = 4,
		kTagAndLayerComponent		 = 5,
	  //kChildrenCountComponent 	 = 6,
	  //kVerticesAndTrianglesCount	 = 7,
		kSeparatorComponent 		 = 1000,
		kTreeMapComponent			 = 1001,
		kComponentsComponent		 = 1002,
	}

	public class Settings 
	{
		public static Settings Instance
		{
			get
			{
				if( instance == null)
				{
					instance = new Settings();
				}
				return instance;
			}
		}
		Settings()
		{ 
			string[] paths = AssetDatabase.FindAssets( kSettingsFileName); 
			for( int i0 = 0; i0 < paths.Length; ++i0)
			{
				settingsObject = (SettingsObject)AssetDatabase.LoadAssetAtPath( 
					AssetDatabase.GUIDToAssetPath( paths[ i0]), typeof( SettingsObject));
				if( settingsObject != null)
				{
					break;
				}
			}
			if( settingsObject == null) 
			{
				settingsObject = ScriptableObject.CreateInstance<SettingsObject>();
				string path = AssetDatabase.GetAssetPath( MonoScript.FromScriptableObject( settingsObject));
				path = path.Substring( 0, path.LastIndexOf( "/"));
				AssetDatabase.CreateAsset( settingsObject, path + "/" + kSettingsFileName + ".asset");
				AssetDatabase.SaveAssets();
			}
			
			InitSetting( Setting.kHierarchyExtension					 	 , true);

			InitSetting( Setting.kTreeMapShow								 , true);
			InitSetting( Setting.kTreeMapColor								 , "39FFFFFF", "905D5D5D");
			InitSetting( Setting.kTreeMapEnhanced							 , true);
			InitSetting( Setting.kTreeMapTransparentBackground				 , true);

			InitSetting( Setting.kSeparatorShow 							 , true);
			InitSetting( Setting.kSeparatorShowRowShading					 , true);
			InitSetting( Setting.kSeparatorColor							 , "FF303030", "48666666");
			InitSetting( Setting.kSeparatorEvenRowShadingColor				 , "13000000", "08000000");
			InitSetting( Setting.kSeparatorOddRowShadingColor				 , "00000000", "00FFFFFF");

			InitSetting( Setting.kActivityShow								 , true);
			InitSetting( Setting.kActivityEditable							 , true);
			InitSetting( Setting.kActivityShowDuringPlayMode				 , true);

			InitSetting( Setting.kLockShow									 , true);
			InitSetting( Setting.kLockEditable								 , true);
			InitSetting( Setting.kLockShowDuringPlayMode					 , false);

			InitSetting( Setting.kStaticShow								 , true);
			InitSetting( Setting.kStaticEditable							 , true);
			InitSetting( Setting.kStaticShowDuringPlayMode					 , false);
			
			InitSetting( Setting.kErrorShow 								 , true);
			InitSetting( Setting.kErrorShowDuringPlayMode					 , false);
			InitSetting( Setting.kErrorShowForDisabledComponents			 , true);
			InitSetting( Setting.kErrorShowForDisabledGameObjects			 , true);
			InitSetting( Setting.kErrorShowIconOnParent						 , true);
			InitSetting( Setting.kErrorShowWhenTagOrLayerIsUndefined		 , true);
			InitSetting( Setting.kErrorShowComponentIsMissing				 , true);
			InitSetting( Setting.kErrorShowReferenceIsMissing				 , false);
			InitSetting( Setting.kErrorIgnoreString 						 , "");

			InitSetting( Setting.kRendererShow								 , true);
			InitSetting( Setting.kRendererEditable							 , true);
			InitSetting( Setting.kRendererShowDuringPlayMode				 , true);

			InitSetting( Setting.kTagAndLayerShow							 , false);
			InitSetting( Setting.kTagAndLayerEditable						 , false);
			InitSetting( Setting.kTagAndLayerShowDuringPlayMode 			 , true);
			InitSetting( Setting.kTagAndLayerSizeShowType					 , (int)HierarchyTagAndLayerShowType.kTagAndLayer);
			InitSetting( Setting.kTagAndLayerType							 , (int)HierarchyTagAndLayerType.kAlways);
			InitSetting( Setting.kTagAndLayerAligment						 , (int)HierarchyTagAndLayerAligment.kLeft);
			InitSetting( Setting.kTagAndLayerSizeValueType					 , (int)HierarchyTagAndLayerSizeType.kPixel);
			InitSetting( Setting.kTagAndLayerSizeValuePercent				 , 0.25f);
			InitSetting( Setting.kTagAndLayerSizeValuePixel 				 , 75);
			InitSetting( Setting.kTagAndLayerLabelSize						 , (int)HierarchyTagAndLayerLabelSize.kBig);
			InitSetting( Setting.kTagAndLayerTagLabelColor					 , "FFCCCCCC", "FF333333");
			InitSetting( Setting.kTagAndLayerLayerLabelColor				 , "FFCCCCCC", "FF333333");
			InitSetting( Setting.kTagAndLayerLabelAlpha 					 , 0.35f);

			InitSetting( Setting.kChildrenCountShow 						 , false);	   
			InitSetting( Setting.kChildrenCountShowDuringPlayMode			 , true);
			InitSetting( Setting.kChildrenCountLabelSize					 , (int)HierarchySize.kNormal);
			InitSetting( Setting.kChildrenCountLabelColor					 , "FFCCCCCC", "FF333333");

			InitSetting( Setting.kVerticesAndTrianglesShow					 , false);
			InitSetting( Setting.kVerticesAndTrianglesShowDuringPlayMode	 , false);
			InitSetting( Setting.kVerticesAndTrianglesCalculateTotalCount	 , false);
			InitSetting( Setting.kVerticesAndTrianglesShowTriangles 		 , false);
			InitSetting( Setting.kVerticesAndTrianglesShowVertices			 , true);
			InitSetting( Setting.kVerticesAndTrianglesLabelSize 			 , (int)HierarchySize.kNormal);
			InitSetting( Setting.kVerticesAndTrianglesVerticesLabelColor	 , "FFCCCCCC", "FF333333");
			InitSetting( Setting.kVerticesAndTrianglesTrianglesLabelColor	 , "FFCCCCCC", "FF333333");

			InitSetting( Setting.kComponentsShow							 , true);
			InitSetting( Setting.kComponentsEditable						 , false);
			InitSetting( Setting.kComponentsShowDuringPlayMode				 , true);
			InitSetting( Setting.kComponentsIconSize						 , (int)HierarchySizeAll.kBig);
			InitSetting( Setting.kComponentsIgnore							 , "");

			InitSetting( Setting.kComponentsOrder							 , kDefaultOrder);

			InitSetting( Setting.kAdditionalHideIconsIfNotFit				 , true);
			InitSetting( Setting.kAdditionalIdentation						 , 0);
			InitSetting( Setting.kAdditionalShowModifierWarning 			 , false);

		#if UNITY_2019_1_OR_NEWER
			InitSetting( Setting.kAdditionalBackgroundColor 				 , "00383838", "00CFCFCF");
		#else
			InitSetting( Setting.kAdditionalBackgroundColor 				 , "00383838", "00C2C2C2");
		#endif
			InitSetting( Setting.kAdditionalActiveColor 					 , "FFFFFF80", "CF363636");
			InitSetting( Setting.kAdditionalInactiveColor					 , "FF4F4F4F", "1E000000");
		} 
		public void OnDestroy()
		{
			skinDependedSettings = null;
			defaultSettings = null;
			settingsObject = null;
			settingChangedHandlerList = null;
			instance = null;
		}
		public T Get<T>( Setting setting)
		{
			return (T)settingsObject.Get<T>( GetSettingName( setting));
		}
		public Color GetColor( Setting setting)
		{
			string stringColor = settingsObject.Get<string>( GetSettingName( setting)) as string;
			return ColorUtils.FromString( stringColor);
		}
		public void SetColor( Setting setting, Color color)
		{
			string stringColor = ColorUtils.ToString( color);
			Set( setting, stringColor);
		}
		public void Set<T>( Setting setting, T value, bool invokeChanger=true)
		{
			int settingId = (int)setting;
			settingsObject.Set( GetSettingName( setting), value);

			if( invokeChanger != false
			&&	settingChangedHandlerList.ContainsKey( settingId)
			&&	settingChangedHandlerList[ settingId] != null)
			{
				settingChangedHandlerList[ settingId].Invoke();
			}
			EditorApplication.RepaintHierarchyWindow();
		}
		public void AddEventListener( Setting setting, System.Action handler)
		{
			int settingId = (int)setting;
			
			if( settingChangedHandlerList.ContainsKey( settingId) == false)
			{
				settingChangedHandlerList.Add(settingId, null);
			}
			if( settingChangedHandlerList[ settingId] == null)
			{
				settingChangedHandlerList[ settingId] = handler;
			}
			else
			{
				settingChangedHandlerList[ settingId] += handler;
			}
		}
		public void RemoveEventListener( Setting setting, System.Action handler)
		{
			int settingId = (int)setting;
			
			if( settingChangedHandlerList.ContainsKey( settingId) != false
			&&	settingChangedHandlerList[ settingId] != null)
			{
				settingChangedHandlerList[ settingId] -= handler;
			}
		}
		public void Restore( Setting setting)
		{
			Set( setting, defaultSettings[ (int)setting]);
		}
		void InitSetting(Setting setting, object defaultValueDark, object defaultValueLight)
		{
			skinDependedSettings.Add( (int)setting);
			InitSetting( setting, EditorGUIUtility.isProSkin ? defaultValueDark : defaultValueLight);
		}
		void InitSetting( Setting setting, object defaultValue)
		{
			string settingName = GetSettingName( setting);
			defaultSettings.Add( (int)setting, defaultValue);
			object value = settingsObject.Get( settingName, defaultValue);
			
			if( value == null || value.GetType() != defaultValue.GetType())
			{
				settingsObject.Set( settingName, defaultValue);
			}		 
		}
		string GetSettingName( Setting setting)
		{
			int settingId = (int)setting;
			string settingName = kPrefsPrefix;
			
			if( skinDependedSettings.Contains( settingId) != false)
			{
				settingName += EditorGUIUtility.isProSkin ? kPrefsDark : kPrefsLight;			 
			}
			settingName += setting.ToString( "G");
			return settingName.ToString();
		}
		
		const string kPrefsPrefix = "Hierarchy_";
		const string kPrefsDark = "Dark_";
		const string kPrefsLight = "Light_";
		const string kSettingsFileName = "SettingsObjectAsset";
	   	public const string kDefaultOrder = "0;1;2;3;4;5";
		public const int kDefaultOrderCount = 6;
		
		static Settings instance;
		
		SettingsObject settingsObject;
		Dictionary<int, object> defaultSettings = new Dictionary<int, object>();
		HashSet<int> skinDependedSettings = new HashSet<int>();
		Dictionary<int, System.Action> settingChangedHandlerList = new Dictionary<int, System.Action>();
	}
}