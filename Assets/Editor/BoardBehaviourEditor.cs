using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

[CustomEditor(typeof(BoardBehavior))]
[CanEditMultipleObjects]
public class BoardBehaviourEditor : Editor {
    
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        serializedObject.Update();

        SerializedProperty nbXTiles = serializedObject.FindProperty("m_numberOfXTiles");
        SerializedProperty nbYTiles = serializedObject.FindProperty("m_numberOfYTiles");
        SerializedProperty dimensions = serializedObject.FindProperty("m_dimensions");
        SerializedProperty tileSelected = serializedObject.FindProperty("m_selectedTile");
        SerializedProperty prefabBloc = serializedObject.FindProperty("m_prefabBloc");
        SerializedProperty prefabButton = serializedObject.FindProperty("m_prefabButton");
        SerializedProperty prefabChest = serializedObject.FindProperty("m_prefabChest");
        SerializedProperty tilesContent = serializedObject.FindProperty("m_tilesContent");
        
        if(tilesContent.objectReferenceValue != null)EditorUtility.SetDirty(tilesContent.objectReferenceValue);
        
        PropertyField(tilesContent);
        
        Space(10);
        IntSlider(nbXTiles, 2, 12);
        IntSlider(nbYTiles, 2, 12);
        PropertyField(dimensions);
        
        Space(10);
        PropertyField(tileSelected);
        if (GUILayout.Button("X +1")) tileSelected.vector2IntValue += new Vector2Int(1, 0);
        if (GUILayout.Button("X -1")) tileSelected.vector2IntValue += new Vector2Int(-1, 0);
        if (GUILayout.Button("Y +1")) tileSelected.vector2IntValue += new Vector2Int(0, 1);
        if (GUILayout.Button("Y -1")) tileSelected.vector2IntValue += new Vector2Int(0, -1);

        Space(20);
        PropertyField(prefabBloc);

        if (GUILayout.Button("Build a wall")) {
            if(prefabBloc.objectReferenceValue != null) {
                GameObject go = PrefabUtility.InstantiatePrefab(prefabBloc.objectReferenceValue) as GameObject;
                if (go != null) go.transform.position = ((BoardBehavior)target).PositionFromCoordinates(tileSelected.vector2IntValue.x, tileSelected.vector2IntValue.y);
            }
            ((BoardBehavior)target).SetTileWall(tileSelected.vector2IntValue);
        }

        Space(10);
        PropertyField(prefabButton);

        if (GUILayout.Button("Place a Button")) {
            GameObject go = PrefabUtility.InstantiatePrefab(prefabButton.objectReferenceValue) as GameObject;
            if (go != null) go.transform.position = ((BoardBehavior)target).PositionFromCoordinates(tileSelected.vector2IntValue.x, tileSelected.vector2IntValue.y);
            
            ((BoardBehavior)target).SetTileButton(tileSelected.vector2IntValue, go.GetComponent<ButtonOnGroundBehaviour>());
        }


        Space(10);
        PropertyField(prefabChest);
        
        if (GUILayout.Button("Place THE Chest")) {
            GameObject go = PrefabUtility.InstantiatePrefab(prefabChest.objectReferenceValue) as GameObject;
            if (go != null) go.transform.position = ((BoardBehavior)target).PositionFromCoordinates(tileSelected.vector2IntValue.x, tileSelected.vector2IntValue.y);
            
            ((BoardBehavior)target).SetTileChest(tileSelected.vector2IntValue, go.GetComponent<ChestBehavior>());
        }

        if (GUILayout.Button("Clear Tile")) {
            ((BoardBehavior)target).SetTileClear(tileSelected.vector2IntValue);
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}
