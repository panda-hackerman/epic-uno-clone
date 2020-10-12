using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(PlayerManager))]
public class PlayerManagerEditor : Editor
{
    SerializedProperty cardPrefabs;

    ReorderableList list;

    private void OnEnable()
    {
        cardPrefabs = serializedObject.FindProperty("cardPrefabs");

        list = new ReorderableList(serializedObject, cardPrefabs, true, true, true, true);

        list.drawElementCallback = DrawListItems;
        list.drawHeaderCallback = DrawHeader;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (cardPrefabs == null) return;

        EditorGUILayout.Space();

        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }

    void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
    {
        var element = list.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2;

        EditorGUI.LabelField(
            new Rect(rect.x, rect.y, 10, EditorGUIUtility.singleLineHeight),
            index.ToString());

        EditorGUI.PropertyField(
            new Rect(rect.x + 15, rect.y, rect.width - 10, EditorGUIUtility.singleLineHeight),
            element, GUIContent.none);
    }

    void DrawHeader(Rect rect)
    {
        string name = "Card Prefabs";
        EditorGUI.LabelField(rect, name);
    }
}
