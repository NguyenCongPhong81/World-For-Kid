#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfDrawer : PropertyDrawer
{
    private float height = 0;
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return height;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var showif = attribute as ShowIfAttribute;
        var path = property.propertyPath;
        var newPath = path.Substring(0, path.LastIndexOf(property.name)) + showif.Condition;
        var a = property.serializedObject.FindProperty(newPath);
        try
        {
            var value = a.intValue;
            if (value == Convert.ToInt32(showif.Value))
            {
                DrawProperty();
            }
            else
            {
                height = 0;
            }
        }
        catch
        {
            DrawProperty();
            Debug.Log("Thuộc tính điều hướng không phải kiểu int");
        }
        void DrawProperty()
        {
            EditorGUI.indentLevel++;
            var labelRect = new Rect(position.x, position.y, position.width, 16);
            EditorGUI.PropertyField(labelRect, property, label, true);
            height = EditorGUI.GetPropertyHeight(property, null, true);
            EditorGUI.indentLevel--;
        }
    }


}
#endif