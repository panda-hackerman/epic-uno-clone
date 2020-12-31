using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

[CustomEditor(typeof(UneDrawPile))]
public class UneDrawPileEditor : Editor
{
    private SerializedProperty cards;
    private ReorderableList reorderableList;

    private void OnEnable()
    {
        cards = serializedObject.FindProperty("cards");

        reorderableList = new ReorderableList(serializedObject, cards, true, true, true, true);

        reorderableList.drawHeaderCallback = DrawHeader;
        reorderableList.drawElementCallback = DrawListItems;
    }

    private void DrawHeader(Rect rect)
    {
        EditorGUI.LabelField(rect, "DrawPile");
    }

    private void DrawListItems(Rect rect, int index, bool isactive, bool isfocused)
    {
        SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);

        EditorGUI.PropertyField(
            new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("prefab"),
            GUIContent.none
        );

        EditorGUI.LabelField(new Rect(rect.x + 220, rect.y, 100, EditorGUIUtility.singleLineHeight), "#");

        EditorGUI.PropertyField(
            new Rect(rect.x + 230, rect.y, 20, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("numberInDeck"),
            GUIContent.none
        );
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        reorderableList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}
