
using UnityEngine;
using UnityEditor;

namespace Hierarchy
{
	[InitializeOnLoad]
	public class EntryPoint
	{
		static EntryPoint()
		{
            EditorApplication.hierarchyWindowItemOnGUI -= HierarchyWindowItemOnGUIHandler;
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUIHandler;
            Undo.undoRedoPerformed -= UndoRedoPerformed;
            Undo.undoRedoPerformed += UndoRedoPerformed;
		}
        static void HierarchyWindowItemOnGUIHandler( int instanceId, Rect selectionRect)
        {
			if( extension == null)
            {
				extension = new Extension();
			}
			extension.HierarchyWindowItemOnGUIHandler( instanceId, selectionRect);
		}
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
