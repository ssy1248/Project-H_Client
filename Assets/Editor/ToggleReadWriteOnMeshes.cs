using UnityEditor;
using UnityEngine;

public class ToggleReadWriteOnMeshes : EditorWindow
{
    [MenuItem("Tools/Enable/Disable Read/Write on Meshes")]
    static void ToggleReadWrite()
    {
        string[] allMeshAssets = AssetDatabase.FindAssets("t:Mesh");

        foreach (var guid in allMeshAssets)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);

            if (mesh != null)
            {
                // Importer ���� ��������
                var importSettings = AssetImporter.GetAtPath(assetPath) as ModelImporter;

                if (importSettings != null)
                {
                    // Read/Write Enabled Ȱ��ȭ �Ǵ� ��Ȱ��ȭ
                    bool currentSetting = importSettings.isReadable;

                    // ���¿� ���� ���
                    importSettings.isReadable = !currentSetting;

                    AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                    Debug.Log($"{(importSettings.isReadable ? "Enabled" : "Disabled")} Read/Write for {assetPath}");
                }
            }
        }

        Debug.Log("Toggle Read/Write for All Meshes.");
    }
}
