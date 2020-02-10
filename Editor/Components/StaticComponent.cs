
using UnityEngine;
using UnityEditor;

namespace Hierarchy
{
    public class StaticComponent: BaseComponent
    {
        public StaticComponent()
        {
            rect.width = 11;
            rect.height = 10;

            Settings.Instance.AddEventListener( Setting.kStaticShow, SettingsChanged);
            Settings.Instance.AddEventListener( Setting.kStaticEditable, SettingsChanged);
            Settings.Instance.AddEventListener( Setting.kStaticShowDuringPlayMode, SettingsChanged);
            Settings.Instance.AddEventListener( Setting.kAdditionalActiveColor, SettingsChanged);
            Settings.Instance.AddEventListener( Setting.kAdditionalInactiveColor, SettingsChanged);

            SettingsChanged();
        }
        void SettingsChanged()
        {
            enabled = Settings.Instance.Get<bool>( Setting.kStaticShow);
            editable = Settings.Instance.Get<bool>( Setting.kStaticEditable);
            showComponentDuringPlayMode = Settings.Instance.Get<bool>( Setting.kStaticShowDuringPlayMode);
            activeColor = Settings.Instance.GetColor( Setting.kAdditionalActiveColor);
            inactiveColor = Settings.Instance.GetColor( Setting.kAdditionalInactiveColor);
        }
        public override LayoutStatus Layout( GameObject gameObject, Rect selectionRect, ref Rect curRect, float maxWidth)
        {
            if( maxWidth < 13)
            {
                return LayoutStatus.kFailed;
            }
            else
            {
                curRect.x -= 13;
                rect.x = curRect.x;
                rect.y = curRect.y + 4;
                staticFlags = GameObjectUtility.GetStaticEditorFlags( gameObject);
                return LayoutStatus.kSuccess;
            }
        }
        public override void Draw( GameObject gameObject, Rect selectionRect)
        {
            if( staticButton == null)
            {
                staticButton = new Texture2D( 11, 10, TextureFormat.ARGB32, false, true);
                staticButtonColors = new Color32[ 11 * 10];
            }
			
    #if UNITY_4_6 || UNITY_4_7
            WritePixels( 39, 5, 4, ((staticFlags & StaticEditorFlags.LightmapStatic       ) > 0));
            WritePixels( 33, 5, 4, ((staticFlags & StaticEditorFlags.BatchingStatic       ) > 0));
    #else
       	#if UNITY_2019_2_OR_NEWER
            WritePixels( 37, 3, 4, ((staticFlags & StaticEditorFlags.ContributeGI         ) > 0));
        #else
        	WritePixels( 37, 3, 4, ((staticFlags & StaticEditorFlags.LightmapStatic       ) > 0));
        #endif
            WritePixels( 33, 3, 4, ((staticFlags & StaticEditorFlags.BatchingStatic       ) > 0));
            WritePixels( 41, 3, 4, ((staticFlags & StaticEditorFlags.ReflectionProbeStatic) > 0));
    #endif
            WritePixels(  0, 5, 2, ((staticFlags & StaticEditorFlags.OccludeeStatic       ) > 0));
            WritePixels(  6, 5, 2, ((staticFlags & StaticEditorFlags.OccluderStatic       ) > 0));
            WritePixels( 88, 5, 2, ((staticFlags & StaticEditorFlags.NavigationStatic     ) > 0));
            WritePixels( 94, 5, 2, ((staticFlags & StaticEditorFlags.OffMeshLinkGeneration) > 0));

            staticButton.SetPixels32( staticButtonColors);
            staticButton.Apply();
            GUI.DrawTexture( rect, staticButton);
        }
        public override void EventHandler( GameObject gameObject, Event currentEvent)
        {
            if( currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && rect.Contains( currentEvent.mousePosition) != false)
            {
                currentEvent.Use();

                int intStaticFlags = (int)staticFlags;
                gameObjects = Selection.Contains( gameObject) ? Selection.gameObjects : new GameObject[]{ gameObject };

                GenericMenu menu = new GenericMenu();
                menu.AddItem( new GUIContent( "Nothing"                   ), intStaticFlags == 0, StaticChangeHandler, 0);
                menu.AddItem( new GUIContent( "Everything"                ), intStaticFlags == -1, StaticChangeHandler, -1);
            #if UNITY_2019_2_OR_NEWER
                menu.AddItem( new GUIContent( "ContributeGI"              ), (intStaticFlags & (int)StaticEditorFlags.ContributeGI) > 0, StaticChangeHandler, (int)StaticEditorFlags.ContributeGI);
            #else
            	menu.AddItem( new GUIContent( "Lightmap Static"           ), (intStaticFlags & (int)StaticEditorFlags.LightmapStatic) > 0, StaticChangeHandler, (int)StaticEditorFlags.LightmapStatic);
            #endif
                menu.AddItem( new GUIContent( "Occluder Static"           ), (intStaticFlags & (int)StaticEditorFlags.OccluderStatic) > 0, StaticChangeHandler, (int)StaticEditorFlags.OccluderStatic);
                menu.AddItem( new GUIContent( "Occludee Static"           ), (intStaticFlags & (int)StaticEditorFlags.OccludeeStatic) > 0, StaticChangeHandler, (int)StaticEditorFlags.OccludeeStatic);
                menu.AddItem( new GUIContent( "Batching Static"           ), (intStaticFlags & (int)StaticEditorFlags.BatchingStatic) > 0, StaticChangeHandler, (int)StaticEditorFlags.BatchingStatic);
                menu.AddItem( new GUIContent( "Navigation Static"         ), (intStaticFlags & (int)StaticEditorFlags.NavigationStatic) > 0, StaticChangeHandler, (int)StaticEditorFlags.NavigationStatic);
                menu.AddItem( new GUIContent( "Off Mesh Link Generation"  ), (intStaticFlags & (int)StaticEditorFlags.OffMeshLinkGeneration) > 0, StaticChangeHandler, (int)StaticEditorFlags.OffMeshLinkGeneration);
                menu.AddItem( new GUIContent( "Reflection Probe Static"   ), (intStaticFlags & (int)StaticEditorFlags.ReflectionProbeStatic) > 0, StaticChangeHandler, (int)StaticEditorFlags.ReflectionProbeStatic);
                menu.ShowAsContext();
            }
        }
        void StaticChangeHandler( object result)
        {
			StaticEditorFlags resultStaticFlags = (StaticEditorFlags)result;
            int intResult = (int)result;
            
            if( intResult != 0 && intResult != -1)
            {
                resultStaticFlags = staticFlags ^ resultStaticFlags;
            }
            for( int i0 = gameObjects.Length - 1; i0 >= 0; --i0)
            {
                GameObject gameObject = gameObjects[ i0];
                Undo.RecordObject( gameObject, "Change Static Flags");            
                GameObjectUtility.SetStaticEditorFlags( gameObject, resultStaticFlags);
                EditorUtility.SetDirty( gameObject);
            }
        }
        void WritePixels(int startPosition, int width, int height, bool isActiveColor)
        {
            Color32 color = isActiveColor ? activeColor : inactiveColor;
            
            for( int iy = 0; iy < height; ++iy)
            {
                for( int ix = 0; ix < width; ++ix)
                {
                    int pos = startPosition + ix + iy * 11;
                    staticButtonColors[ pos].r = color.r;
                    staticButtonColors[ pos].g = color.g;
                    staticButtonColors[ pos].b = color.b;
                    staticButtonColors[ pos].a = color.a;
                }
            }
        }
        
        Color activeColor;
        Color inactiveColor;
        StaticEditorFlags staticFlags;
        GameObject[] gameObjects;
        Texture2D staticButton;
        Color32[] staticButtonColors;
    }
}
