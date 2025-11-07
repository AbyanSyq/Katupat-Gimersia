#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

[CustomEditor(typeof(MonoBehaviour), true, isFallback = true)]
[CanEditMultipleObjects]
public class FoldoutGroupDrawer : Editor
{
    // Simpan state per tipe + nama grup supaya tidak saling tumpang tindih
    private static readonly Dictionary<string, bool> FoldStates = new();
    private GUIStyle _boldFoldout;

    public override void OnInspectorGUI()
    {
        // Tampilkan Script reference (multi-object safe)
        using (new EditorGUI.DisabledGroupScope(true))
        {
            if (targets.Length == 1 && target is MonoBehaviour mb)
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(mb), typeof(MonoScript), false);
            else if (targets.Length == 1 && target is ScriptableObject so)
                EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject(so), typeof(MonoScript), false);
            else
                EditorGUILayout.LabelField("Script", "- Multiple Objects -");
        }
        EditorGUILayout.Space(5);

        serializedObject.Update();

        _boldFoldout ??= new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold };

        // Kumpulkan semua field serializable
        var type = target.GetType();
        var fields = type
            .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(f => f.IsPublic || f.GetCustomAttribute<SerializeField>() != null)
            .ToArray();

        // Cek apakah ada FoldoutGroup attribute
        bool hasFoldout = fields.Any(f => f.GetCustomAttribute<FoldoutGroupAttribute>() != null);
        if (!hasFoldout)
        {
            // Tidak ada grup → pakai default inspector, jadi editor lain tidak terpengaruh
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
            return;
        }

        // Kelompokkan TAPI TANPA nested: ambil nama grup apa adanya (abaikan "/")
        var grouped = new Dictionary<string, List<SerializedProperty>>();
        var ungrouped = new List<SerializedProperty>();

        foreach (var f in fields)
        {
            var sp = serializedObject.FindProperty(f.Name);
            if (sp == null) continue;

            var grp = f.GetCustomAttribute<FoldoutGroupAttribute>();
            if (grp == null || string.IsNullOrEmpty(grp.GroupName))
            {
                ungrouped.Add(sp);
                continue;
            }

            // Non-nested: pakai nama persis, jika user kirim "Inventory/Weapons" akan dianggap satu nama grup literal.
            var key = grp.GroupName;

            if (!grouped.ContainsKey(key)) grouped[key] = new List<SerializedProperty>();
            grouped[key].Add(sp);
        }

        // Gambar setiap grup sekali saja, isi menjorok
        foreach (var kv in grouped)
        {
            string groupName = kv.Key;
            string foldKey = $"{type.FullName}::{groupName}";
            if (!FoldStates.ContainsKey(foldKey)) FoldStates[foldKey] = true;

            EditorGUILayout.Space(2);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                FoldStates[foldKey] = EditorGUILayout.Foldout(FoldStates[foldKey], groupName, true, _boldFoldout);
            }

            if (FoldStates[foldKey])
            {
                EditorGUI.indentLevel++;
                foreach (var prop in kv.Value)
                    EditorGUILayout.PropertyField(prop, true);
                EditorGUI.indentLevel--;
            }
        }

        // Field tanpa grup ditampilkan setelahnya seperti biasa
        if (ungrouped.Count > 0)
        {
            EditorGUILayout.Space(4);
            foreach (var prop in ungrouped)
                EditorGUILayout.PropertyField(prop, true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
