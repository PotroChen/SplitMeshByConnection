using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MeshSplit : MonoBehaviour {

    #region PUBLIC_VARIABLES
    public static List<int> allIndices = new List<int>();
    public static List<int> newIndices = new List<int>();
    public static List<int> restIndices = new List<int>();
    #endregion

    #region PRIVATE_VARIABLES
    #endregion

    #region PUBLIC_FUNCTION
    [MenuItem("SeparateMesh/SplitMeshByConnection")]
    public static void SplitMeshMenu()
    {
        GameObject[] targetObjGroup = Selection.gameObjects;
        foreach (GameObject targetObj in targetObjGroup)
        {
            SplitMesh(targetObj.GetComponent<MeshRenderer>(), targetObj.GetComponent<MeshFilter>().sharedMesh);
        }
    }
    #endregion

    #region PRIVATE_FUNCTION
    [MenuItem("SeparateMesh/SplitMeshByConnection")]
    private static void SplitMesh(MeshRenderer meshRenderer, Mesh mesh)
    {
        //int nameIndex = 0;//新生成物体的序列号name_index
        //string filePath;
        int[] indices = mesh.triangles;
        Vector3[] verts = mesh.vertices;

        //list all indices
        for (int i = 0; i < indices.Length; i++)
        {
            allIndices.Add(indices[i]);
            restIndices.Add(indices[i]);
        }

        while (restIndices.Count > 0)
        {
            newIndices.Clear();
            //Get first triangle
            for (int i = 0; i < 3; i++)
            {
                newIndices.Add(restIndices[i]);
            }
            for (int i = 1; i < restIndices.Count / 3; i++)
            {
                if (newIndices.Contains(restIndices[(i * 3) + 0]) || newIndices.Contains(restIndices[(i * 3) + 1]) || newIndices.Contains(restIndices[(i * 3) + 2]))
                {
                    for (int q = 0; q < 3; q++)
                    {
                        newIndices.Add(restIndices[(i * 3) + q]);
                    }
                }
            }
            restIndices.Clear();
            for (int n = 0; n < allIndices.Count; n++)
            {
                if (!newIndices.Contains(allIndices[n]))
                {
                    restIndices.Add(allIndices[n]);
                }
            }
            allIndices.Clear();
            for (int i = 0; i < restIndices.Count; i++)
            {
                allIndices.Add(restIndices[i]);
            }

            mesh.triangles = restIndices.ToArray();

            Mesh newMesh = new Mesh();
            newMesh.vertices = verts;
            newMesh.triangles = newIndices.ToArray();

            newMesh.RecalculateNormals();
            
            GameObject newGameObject = new GameObject("newGameObject");
            newGameObject.AddComponent<MeshRenderer>().material = meshRenderer.sharedMaterial;
            newGameObject.AddComponent<MeshFilter>().mesh = newMesh;

            Vector3 worldPos = newGameObject.transform.TransformPoint(newGameObject.AddComponent<SphereCollider>().center);
            Debug.Log(worldPos.x+" "+ worldPos.y + " "+ worldPos.z);
            //filePath = EditorUtility.SaveFilePanelInProject("Save Mesh in Assets",
            //                                                meshRenderer.name + nameIndex.ToString() + ".asset",
            //                                                "asset", "Please enter a file name to save the Mesh to ");

            //if (filePath != "")
            //{
            //    AssetDatabase.CreateAsset(newMesh, filePath);
            //    AssetDatabase.SaveAssets();
            //}
            //nameIndex++;
        }
        DestroyImmediate(meshRenderer.gameObject);
    }
    #endregion
}
