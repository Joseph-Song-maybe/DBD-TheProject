using UnityEngine;
using UnityEditor;

public class TownTextureFixer : EditorWindow
{
    [MenuItem("Tools/Fix Town Textures")]
    static void ShowWindow()
    {
        GetWindow<TownTextureFixer>("Texture Fixer");
    }

    void OnGUI()
    {
        GUILayout.Label("Town Texture Repair", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Create Default Materials"))
        {
            CreateDefaultMaterials();
        }
        
        if (GUILayout.Button("Apply Materials to Town"))
        {
            ApplyMaterialsToTown();
        }
    }

    void CreateDefaultMaterials()
    {
        string[] materials = { "Building", "Roof", "Ground", "Wood" };
        
        foreach (string matName in materials)
        {
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = GetColor(matName);
            AssetDatabase.CreateAsset(mat, $"Assets/Materials/Town/{matName}.mat");
        }
        
        AssetDatabase.SaveAssets();
        Debug.Log("Default materials created!");
    }

    Color GetColor(string name)
    {
        switch (name)
        {
            case "Building": return new Color(0.8f, 0.7f, 0.6f);
            case "Roof": return new Color(0.6f, 0.3f, 0.2f);
            case "Ground": return new Color(0.5f, 0.4f, 0.3f);
            case "Wood": return new Color(0.7f, 0.5f, 0.3f);
            default: return Color.white;
        }
    }

    void ApplyMaterialsToTown()
    {
        GameObject town = GameObject.Find("town");
        if (town != null)
        {
            Renderer[] renderers = town.GetComponentsInChildren<Renderer>();
            Material buildingMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Town/Building.mat");
            
            foreach (Renderer r in renderers)
            {
                if (buildingMat != null)
                    r.material = buildingMat;
            }
            
            Debug.Log("Materials applied to town objects!");
        }
    }
}