using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GetSubObjectCenterByCoonection : MonoBehaviour {
    #region PUBLIC_VARIABLES
    public static List<int> allIndices = new List<int>();
    public static List<int> newIndices = new List<int>();
    public static List<int> restIndices = new List<int>();
    #endregion

    #region PRIVATE_VARIABLES
    #endregion

    #region PUBLIC_FUNCTION
    [MenuItem("SeparateMesh/GetSubObjectCenterByCoonection")]
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

            //mesh.triangles = restIndices.ToArray();

            //1.创建新数组tempIndice，将newIndices去重复，并从小到大排序的结果填入
            //2.通过tempIndice对verts的映射，将顶点加入新的数组newVerts
            //3.newIndices所有元素减去tempIndice[0]也就是全部矫正归零（newIndices索引归零）
            //4.算出newVerts的算出newVerts中所有的点的平均值，再将verts所有值减去这个值（verts原点归零）

            List<int> tempIndices = new List<int>();
            foreach (int index in newIndices)
            {
                if (!tempIndices.Contains(index))
                    tempIndices.Add(index);
            }
            Vector3[] newVerts = new Vector3[tempIndices.Count];
            for (int i = 0; i < tempIndices.Count; i++)
            {
                newVerts[i] = verts[tempIndices[i]];
            }
            Vector3 sum = Vector3.zero;
            foreach (Vector3 item in newVerts)
            {
                sum += item;
            }
            Vector3 avg = sum / newVerts.Length;
            Debug.Log(avg);
            //tempIndices.Sort();
            //Vector3[] newVerts = new Vector3[tempIndices.Count];
            //for (int i = 0; i < tempIndices.Count; i++)
            //{
            //    newVerts[i] = verts[tempIndices[i]];
            //}
            //for (int i = 0; i < newIndices.Count; i++)
            //{
            //    newIndices[i] = newIndices[i] - tempIndices[0];
            //}
            //Vector3 sum = Vector3.zero;
            //foreach (Vector3 item in newVerts)
            //{
            //    sum += item;
            //}
            //Vector3 avg = sum / newVerts.Length;
            //for (int i = 0; i < newVerts.Length; i++)
            //{
            //    newVerts[i] = newVerts[i] - avg;
            //}

            //Mesh newMesh = new Mesh();
            //newMesh.vertices = newVerts;
            //newMesh.triangles = newIndices.ToArray();
            //newMesh.RecalculateNormals();

            //GameObject newGameObject = new GameObject("newGameObject");
            //newGameObject.transform.localPosition = avg;
            //newGameObject.AddComponent<MeshRenderer>().material = meshRenderer.sharedMaterial;
            //newGameObject.AddComponent<MeshFilter>().mesh = newMesh;
        }
        //DestroyImmediate(meshRenderer.gameObject);
    }
    #endregion
}
