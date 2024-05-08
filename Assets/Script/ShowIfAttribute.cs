using UnityEngine;

public class ShowIfAttribute : PropertyAttribute
{
    public string Condition;
    public object Value;
    public ShowIfAttribute(string condition, object value)
    {
#if UNITY_EDITOR
        this.Condition = condition;
        this.Value = value;
#endif
    }
}