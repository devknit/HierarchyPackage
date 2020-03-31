
using UnityEngine;
using UnityEditor;

namespace Hierarchy
{
	public class SeparatorComponent: BaseComponent
	{
		public SeparatorComponent()
		{
			showComponentDuringPlayMode = true;
			Settings.Instance.AddEventListener( Setting.kSeparatorShowRowShading, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kSeparatorShow, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kSeparatorColor, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kSeparatorEvenRowShadingColor, SettingsChanged);
			Settings.Instance.AddEventListener( Setting.kSeparatorOddRowShadingColor, SettingsChanged);
			SettingsChanged();
		}
		void SettingsChanged()
		{
			enabled = Settings.Instance.Get<bool>( Setting.kSeparatorShow);
			showRowShading = Settings.Instance.Get<bool>( Setting.kSeparatorShowRowShading);
			evenShadingColor = Settings.Instance.GetColor( Setting.kSeparatorEvenRowShadingColor);
			oddShadingColor = Settings.Instance.GetColor( Setting.kSeparatorOddRowShadingColor);
			separatorColor = Settings.Instance.GetColor( Setting.kSeparatorColor);
		}
		public override void Draw( GameObject gameObject, Rect selectionRect)
		{
			rect.y = selectionRect.y;
			rect.width = selectionRect.width + selectionRect.x;
			rect.height = 1;
			rect.x = 0;

			EditorGUI.DrawRect( rect, separatorColor);

			if( showRowShading != false)
			{
				selectionRect.width += selectionRect.x;
				selectionRect.x = 0;
				selectionRect.height -=1;
				selectionRect.y += 1;
				EditorGUI.DrawRect( selectionRect, (Mathf.FloorToInt( ((selectionRect.y - 4) / 16) % 2) == 0)? evenShadingColor : oddShadingColor);
			}
		}
		Color separatorColor;
		Color evenShadingColor;
		Color oddShadingColor;
		bool showRowShading;
	}
}
