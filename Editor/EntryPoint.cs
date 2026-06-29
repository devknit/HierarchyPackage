
using UnityEngine;
using UnityEditor;

namespace Hierarchy
{
	[InitializeOnLoad]
	public class EntryPoint
	{
		static EntryPoint()
		{
        #if UNITY_6000_4_OR_NEWER
            EditorApplication.hierarchyWindowItemByEntityIdOnGUI -= HierarchyWindowItemOnGUIHandler;
            EditorApplication.hierarchyWindowItemByEntityIdOnGUI += HierarchyWindowItemOnGUIHandler;
        #else
            EditorApplication.hierarchyWindowItemOnGUI -= HierarchyWindowItemOnGUIHandler;
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUIHandler;
        #endif
            Undo.undoRedoPerformed -= UndoRedoPerformed;
            Undo.undoRedoPerformed += UndoRedoPerformed;
		}
    #if UNITY_6000_4_OR_NEWER
        static void HierarchyWindowItemOnGUIHandler( EntityId entityId, Rect selectionRect)
        {
			if( extension == null)
            {
				extension = new Extension();
			}
			extension.HierarchyWindowItemOnGUIHandler( entityId, selectionRect);
		}
    #else
        static void HierarchyWindowItemOnGUIHandler( int instanceId, Rect selectionRect)
        {
			if( extension == null)
            {
				extension = new Extension();
			}
			extension.HierarchyWindowItemOnGUIHandler( instanceId, selectionRect);
		}
    #endif
		static void UndoRedoPerformed()
        {
            EditorApplication.RepaintHierarchyWindow();          
        }
		[MenuItem( "GameObject/Remove Missing Scripts")]
		static void RemoveMissingScripts()
        {
			if( Selection.activeObject is GameObject gameObject)
			{
				if( RemoveMissingScripts( gameObject.transform) > 0)
				{
					EditorUtility.SetDirty( gameObject);
				}
			}
		}
		static int RemoveMissingScripts( Transform transform)
		{
			int count = GameObjectUtility.RemoveMonoBehavioursWithMissingScript( transform.gameObject);
			
			foreach( Transform child in transform)
			{
				count += RemoveMissingScripts( child);
			}
			return count;
		}
		
        static Extension extension;
	}
}
