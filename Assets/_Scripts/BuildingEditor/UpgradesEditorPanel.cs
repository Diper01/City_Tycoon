using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class UpgradesEditorPanel {
  private BuildingWindow window;
  private List<bool> foldouts = new List<bool>();

  public UpgradesEditorPanel (BuildingWindow win)
    => window = win;

  public void OnGUI() {
    var data = window.previewData;

    if (data == null)
      return;

    if (data.upgrades == null)
      data.upgrades = new List<UpgradeStep>();

    var list = data.upgrades;

    while (foldouts.Count < list.Count)
      foldouts.Add(true);

    while (foldouts.Count > list.Count)
      foldouts.RemoveAt(foldouts.Count - 1);

    EditorGUILayout.LabelField("4) Upgrade Steps", EditorStyles.boldLabel);

    if (list.Count == 0) {
      if (GUILayout.Button("Add Level"))
        list.Add(new UpgradeStep {
          material = null,
          scale = Vector3.one,
          details = new List<DetailData>()
        });

      EditorGUILayout.Space();
      return;
    }

    for (int i = 0; i < list.Count; i++) {
      foldouts[i] = EditorGUILayout.Foldout(foldouts[i], $"Level {i}", true);

      if (!foldouts[i])
        continue;

      var step = list[i];
      EditorGUI.indentLevel++;

      step.material = (Material)EditorGUILayout.ObjectField("Material", step.material, typeof(Material), false);
      step.scale = EditorGUILayout.Vector3Field("Scale", step.scale);

      EditorGUILayout.LabelField("Details", EditorStyles.boldLabel);

      if (step.details == null)
        step.details = new List<DetailData>();

      var det = step.details;

      for (int j = 0; j < det.Count; j++) {
        DetailData d = det[j];
        EditorGUILayout.BeginVertical("box");
        d.prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", d.prefab, typeof(GameObject), false);

        if (GUILayout.Button("Spawn Detail", GUILayout.Width(100))) {
          var root = window.previewInstance;

          if (root == null) {
            Debug.LogWarning("Spawn Preview First");
          } else if (d.prefab == null) {
            Debug.LogWarning("DetailData.prefab is null.");
          } else {
            string childName = $"L{i}_D{j}";
            var existing = root.transform.Find(childName);

            if (existing != null) {
              Undo.DestroyObjectImmediate(existing.gameObject);
            }

            var inst = (GameObject)PrefabUtility.InstantiatePrefab(d.prefab, root.transform);
            inst.name = childName;
            Undo.RegisterCreatedObjectUndo(inst, "Spawn Detail");
            inst.transform.localPosition = d.localPosition;
            inst.transform.localEulerAngles = d.localEulerAngles;
            inst.transform.localScale = Vector3.one;
          }
        }



        if (GUILayout.Button(d.isHidden ? "Show" : "Hide", GUILayout.Width(50))) {
          var go = window.previewInstance.transform.Find($"L{i}_D{j}");

          if (go)
            go.gameObject.SetActive(d.isHidden);

          d.isHidden = !d.isHidden;
        }

        if (GUILayout.Button("Capture", GUILayout.Width(60))) {
          var go = window.previewInstance.transform.Find($"L{i}_D{j}");

          if (go) {
            d.localPosition = go.localPosition;
            d.localEulerAngles = go.localEulerAngles;
            d.localScale = go.localScale;
          }
        }

        if (GUILayout.Button("Remove", GUILayout.Width(60))) {
          det.RemoveAt(j);
          j--;
          EditorGUILayout.EndVertical();
          continue;
        }

        d.localPosition = EditorGUILayout.Vector3Field("Position", d.localPosition);
        d.localEulerAngles = EditorGUILayout.Vector3Field("Rotation", d.localEulerAngles);
        d.localScale = EditorGUILayout.Vector3Field("Scale", d.localScale);

        EditorGUILayout.EndVertical();
      }

      if (GUILayout.Button("+ Add Detail")) {
        det.Add(new DetailData());
      }

      EditorGUILayout.BeginHorizontal();

      if (GUILayout.Button("Add Level"))
        list.Add(new UpgradeStep {
          material = null,
          scale = Vector3.one,
          details = new List<DetailData>()
        });


      if (GUILayout.Button("Remove Level")) {
        list.RemoveAt(i);
        foldouts.RemoveAt(i);
        break;
      }

      EditorGUILayout.EndHorizontal();
      EditorGUI.indentLevel--;
    }

    EditorGUILayout.Space();
  }
}