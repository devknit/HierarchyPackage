
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Hierarchy
{
	[System.Serializable]
	class SettingsObject : ScriptableObject
	{
		public void Clear()
		{
			settingStringNames.Clear();
			settingStringValues.Clear();
			settingFloatNames.Clear();
			settingFloatValues.Clear();
			settingIntNames.Clear();
			settingIntValues.Clear();
			settingBoolNames.Clear();
			settingBoolValues.Clear();
		}
		public void Set( string settingName, object value)
		{
			if( value is bool boolValue)
			{
				settingBoolValues[ settingBoolNames.IndexOf(settingName)] = boolValue;
			}
			else if( value is string stringValue)
			{
				settingStringValues[ settingStringNames.IndexOf(settingName)] = stringValue;
			}
			else if( value is float floatValue)
			{
				settingFloatValues[ settingFloatNames.IndexOf(settingName)] = floatValue;
			}
			else if( value is int intValue)
			{
				settingIntValues[ settingIntNames.IndexOf(settingName)] = intValue;
			}
			EditorUtility.SetDirty( this);
		}
		public object Get( string settingName, object defaultValue)
		{
			if( defaultValue is bool boolValue)
			{
				int id = settingBoolNames.IndexOf( settingName);
				if( id == -1) 
				{
					settingBoolNames.Add( settingName);
					settingBoolValues.Add( boolValue);
					return defaultValue;
				}
				return settingBoolValues[ id];
			}
			else if( defaultValue is string stringValue)
			{
				int id = settingStringNames.IndexOf( settingName);
				if( id == -1) 
				{
					settingStringNames.Add( settingName);
					settingStringValues.Add( stringValue);
					return defaultValue;
				}
				return settingStringValues[ id];
			}
			else if( defaultValue is float floatValue)
			{
				int id = settingFloatNames.IndexOf( settingName);
				if( id == -1) 
				{
					settingFloatNames.Add( settingName);
					settingFloatValues.Add( floatValue);
					return defaultValue;
				}
				return settingFloatValues[ id];
			}
			else if( defaultValue is int intValue)
			{
				int id = settingIntNames.IndexOf( settingName);
				if( id == -1) 
				{
					settingIntNames.Add( settingName);
					settingIntValues.Add( intValue);
					return defaultValue;
				}
				return settingIntValues[ id];
			}
			return null;
		}
		public object Get<T>( string settingName)
		{
			if( typeof( T) == typeof( bool))
			{
				int id = settingBoolNames.IndexOf( settingName);
				if( id == -1)
				{
					return null;
				}
				return settingBoolValues[ id];
			}
			else if( typeof( T) == typeof( string))
			{
				int id = settingStringNames.IndexOf( settingName);
				if( id == -1)
				{
					return null;
				}
				return settingStringValues[ id];
			}
			else if( typeof( T) == typeof( float))
			{
				int id = settingFloatNames.IndexOf( settingName);
				if( id == -1)
				{
					return null;
				}
				return settingFloatValues[ id];
			}
			else if( typeof( T) == typeof( int))
			{
				int id = settingIntNames.IndexOf( settingName);
				if( id == -1)
				{
					return null;
				}
				return settingIntValues[ id];
			}
			return null;
		}
		[SerializeField]
		List<string> settingStringNames = new List<string>();
		[SerializeField]
		List<string> settingStringValues = new List<string>();

		[SerializeField]
		List<string> settingFloatNames = new List<string>();
		[SerializeField]
		List<float> settingFloatValues = new List<float>();

		[SerializeField]
		List<string> settingIntNames = new List<string>();
		[SerializeField]
		List<int> settingIntValues = new List<int>();

		[SerializeField]
		List<string> settingBoolNames = new List<string>();
		[SerializeField]
		List<bool> settingBoolValues = new List<bool>();
	}
}

