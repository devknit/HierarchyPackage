
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
        static Extension extension;
	}
}
