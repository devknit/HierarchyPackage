
using System;
using UnityEngine;
using UnityEditor;

namespace Hierarchy
{
	public class ColorUtils
	{
		public static void SetDefaultColor( Color defaultColor)
		{
			ColorUtils.defaultColor = defaultColor;
		}
		public static void SetColor( Color newColor)
		{
			GUI.color = newColor;
		}
		public static void SetColor( Color newColor, float multiColor, float multiAlpha)
		{
			newColor.r *= multiColor;
			newColor.g *= multiColor;
			newColor.b *= multiColor;
			newColor.a *= multiAlpha;
			GUI.color = newColor;
		}
		public static void ClearColor()
		{
			GUI.color = defaultColor;
		}
		public static Color FromString( string color)
		{
			return FromInt( Convert.ToUInt32( color, 16));
		}
		public static string ToString( Color color)
		{
			uint intColor = ToInt( color);
			return intColor.ToString( "X8");
		}
		public static Color FromInt( uint color)
		{
			return new Color(
				((color >> 16) & 0xFF) / 255.0f,
				((color >>  8) & 0xFF) / 255.0f,
				((color >>  0) & 0xFF) / 255.0f,
				((color >> 24) & 0xFF) / 255.0f);
		}
		public static uint ToInt( Color color)
		{
			return  (uint)((byte)(color.r * 255) << 16) + 
					(uint)((byte)(color.g * 255) << 8) + 
					(uint)((byte)(color.b * 255) << 0) + 
					(uint)((byte)(color.a * 255) << 24);
		}
		static Color defaultColor = Color.white;
	}
}
