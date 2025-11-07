using UnityEngine;

public class FoldoutGroupAttribute : PropertyAttribute
{
    public string GroupName;
    public FoldoutGroupAttribute(string groupName)
    {
        GroupName = groupName;
    }
}
