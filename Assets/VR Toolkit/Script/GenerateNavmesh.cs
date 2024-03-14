using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.Interaction.Toolkit;

public class GenerateNavmesh : MonoBehaviour
{
    [SerializeField] private bool m_GenerateAsset = false;
    // Start is called before the first frame update
    void OnValidate()
    {
        if (!m_GenerateAsset) return;

        var triangulation = NavMesh.CalculateTriangulation();

        Debug.Log(triangulation.vertices.Length);
        Debug.Log(triangulation.indices.Length);

        var mesh = new Mesh
        {
            vertices = triangulation.vertices,
            triangles = triangulation.indices,
        };

        Debug.Log(mesh);

        //GetComponent<MeshFilter>().sharedMesh = mesh;
        //UnityEditor.AssetDatabase.CreateAsset(mesh, "Assets/Teillo/Teleport Navmesh/teleportArea.asset");
        //UnityEditor.AssetDatabase.CreateAsset(GetComponent<MeshFilter>().sharedMesh, "Assets/Teillo/Teleport Navmesh/teleportArea.fbx");

        //UnityEditor.AssetDatabase.SaveAssets();

        m_GenerateAsset = false;
    }

}
