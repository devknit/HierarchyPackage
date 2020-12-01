
using UnityEditor;
using UnityEngine;

namespace Hierarchy
{
	public class ComponentsOrderList
	{
		public ComponentsOrderList( EditorWindow window)
		{			 
			this.window = window;
			dragButton = Resources.Instance.GetTexture( Image.kDragButton);
			backgroundColor = Resources.Instance.GetColor( ColorTone.kBackgroundDark);
		}
		public void Draw( Rect rect, string[] componentIds)
		{
			Event currentEvent = Event.current;
			
			int currentMouseIndex = Mathf.Clamp(Mathf.RoundToInt((currentEvent.mousePosition.y - dragOffset - rect.y) / 18), 0, componentIds.Length - 1);
			
			if( dragAndDrop != false && currentEvent.type == EventType.MouseUp)		
			{
				dragAndDrop = false;
				window.Repaint();
				
				if( currentMouseIndex != originalDragIndex)
				{
					string newIconOrder = "";
					for( int i0 = 0; i0 < componentIds.Length; ++i0)
					{
						if( i0 == currentMouseIndex) 
						{
							if( i0 > originalDragIndex)
							{
								newIconOrder += componentIds[ i0] + ";";
								newIconOrder += componentIds[ originalDragIndex] + ";";
							}
							else
							{
								newIconOrder += componentIds[ originalDragIndex] + ";";
								newIconOrder += componentIds[ i0] + ";";
							}
						}
						else if( i0 != originalDragIndex) 
						{
							newIconOrder += componentIds[ i0] + ";";
						}
					}
					newIconOrder = newIconOrder.TrimEnd( ';');
					Settings.Instance.Set( Setting.kComponentsOrder, newIconOrder);
					componentIds = newIconOrder.Split( ';');
				}
			}
			else if( dragAndDrop != false && currentEvent.type == EventType.MouseDrag)
			{
				window.Repaint();
			}
			for( int i0 = 0; i0 < componentIds.Length; ++i0)
			{
				HierarchyComponentEnum type = (HierarchyComponentEnum)int.Parse( componentIds[ i0]);
				
				Rect curRect = new Rect( rect.x, rect.y + 18 * i0, rect.width, 16);
				
				if( dragAndDrop == false && currentEvent.type == EventType.MouseDown && curRect.Contains( currentEvent.mousePosition) != false)
				{
					dragAndDrop = true;
					originalDragIndex = i0;
					dragOffset = currentEvent.mousePosition.y - curRect.y;
					Event.current.Use();
				}
				if( dragAndDrop != false)
				{
					if( originalDragIndex != i0)
					{
						if( i0 < originalDragIndex && currentMouseIndex <= i0)
						{
							curRect.y += 18;
						}
						else if( i0 > originalDragIndex && currentMouseIndex >= i0)
						{
							curRect.y -= 18;
						}
						DrawComponentLabel( curRect, type);				  
					}
				}
				else
				{
					DrawComponentLabel( curRect, type);					  
				}
			}
			if( dragAndDrop != false)
			{
				float curY = currentEvent.mousePosition.y - dragOffset;
				curY = Mathf.Clamp( curY, rect.y, rect.y + rect.height - 16);
				DrawComponentLabel( new Rect( rect.x, curY, rect.width, rect.height), (HierarchyComponentEnum)int.Parse( componentIds[ originalDragIndex]), true);
			}
		}
		void DrawComponentLabel( Rect rect, HierarchyComponentEnum type, bool withBackground=false)
		{
			if( withBackground != false)
			{
				EditorGUI.DrawRect( new Rect( rect.x, rect.y - 2, rect.width, 20), backgroundColor);
			}
			GUI.DrawTexture( new Rect( rect.x, rect.y - 2, 20, 20), dragButton);
			Rect labelRect = new Rect( rect.x + 31, rect.y, rect.width - 20, 16);
			labelRect.y -= (EditorGUIUtility.singleLineHeight - labelRect.height) * 0.5f;
			EditorGUI.LabelField( labelRect, GetTextWithSpaces( type.ToString().Substring( 1)));
		}
		string GetTextWithSpaces( string text)
		{
			var newText = new System.Text.StringBuilder( text.Length * 2);
			newText.Append( text[ 0]);
			
			for( int i0 = 1; i0 < text.Length; ++i0)
			{
				if( char.IsUpper( text[ i0]) && text[ i0 - 1] != ' ')
				{
					newText.Append(' ');
				}
				newText.Append( text[ i0]);				
			}
			newText.Replace( " Component", "");
			return newText.ToString();
		}
		
		EditorWindow window;
		Texture2D dragButton;
		bool dragAndDrop = false;
		float dragOffset;
		int originalDragIndex;
		Color backgroundColor;
	}
}
