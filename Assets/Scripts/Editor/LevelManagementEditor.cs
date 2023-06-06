using Managament.Levels;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(LevelManagement))]
public class LevelManagementEditor : Editor
{
    private SerializedProperty editorMode;
    private SerializedProperty testMode;
    private SerializedProperty locationsDone;
    private LevelManagement _levelManagement;

    private void Awake()
    {
        _levelManagement = target as LevelManagement;
    }

    private void OnEnable()
    {
        editorMode = serializedObject.FindProperty("editorMode");
        testMode = serializedObject.FindProperty("testMode");
        locationsDone = serializedObject.FindProperty("editorLocationIndex");
    }

    public override void OnInspectorGUI()
    {

        editorMode.boolValue = GUILayout.Toggle(editorMode.boolValue, new GUIContent("Editor Mode"), GUILayout.Width(100), GUILayout.Height(20));
        _levelManagement.editorMode = editorMode.boolValue;
        serializedObject.ApplyModifiedProperties();

        testMode.boolValue = GUILayout.Toggle(testMode.boolValue, new GUIContent("Test Mode"), GUILayout.Width(100), GUILayout.Height(20));
        _levelManagement.testMode = testMode.boolValue;
        serializedObject.ApplyModifiedProperties();

        GUILayout.Space(25);

        if (_levelManagement.Locations != null)
        {
            DrawSelectedLevel();
        }
        DrawDefaultInspector();

        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Clear Player Prefs", GUILayout.Width(200), GUILayout.Height(20)))
            PlayerPrefs.DeleteAll();
    }

    private void DrawSelectedLevel()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        var levelIndex = EditorGUILayout.IntField("Level Index", _levelManagement.CurrantLevelIndex + 1);

        if (GUILayout.Button("<<", GUILayout.Width(30), GUILayout.Height(20)))
        {
            levelIndex--;
        }
        if (GUILayout.Button(">>", GUILayout.Width(30), GUILayout.Height(20)))
        {
            levelIndex++;
        }


        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (_levelManagement.Locations == null)
            return;

        var locationIndex = EditorGUILayout.IntField("Location Index", _levelManagement.CurrantLocationIndex + 1);
        if (GUILayout.Button("<<", GUILayout.Width(30), GUILayout.Height(20)))
        {
            locationIndex--;
        }
        if (GUILayout.Button(">>", GUILayout.Width(30), GUILayout.Height(20)))
        {
            locationIndex++;
        }

        if (EditorGUI.EndChangeCheck())
        {
            _levelManagement.SelectLevel(levelIndex - 1, locationIndex - 1);
        }

        EditorGUILayout.EndHorizontal();
    }
}
