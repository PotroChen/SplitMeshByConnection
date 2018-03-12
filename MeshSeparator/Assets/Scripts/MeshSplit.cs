using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MeshSplit : MonoBehaviour
{

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

            //Debug.Log(restIndices.Count + " " + allIndices.Count +" "+newIndices.Count);

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

            List<int> verIndMap = new List<int>();

            foreach (int index in newIndices)//去重复
            {
                if (!verIndMap.Contains(index))
                    verIndMap.Add(index);
            }

            verIndMap.Sort();//排序
            Vector3[] resVerts = new Vector3[verIndMap.Count];
            int[] resIndices = new int[newIndices.Count];

            for (int i = 0; i < verIndMap.Count; i++)
            {
                resVerts[i] = verts[verIndMap[i]];
            }

            for (int i = 0; i < newIndices.Count; i++)
            {
                for (int j = 0; j < verIndMap.Count; j++)
                {
                    if (newIndices[i] == verIndMap[j])
                        resIndices[i] = j;
                }

            }

            Vector3 sum = Vector3.zero;
            foreach (Vector3 item in resVerts)
                sum += item;

            Vector3 avg = sum / resVerts.Length;
            for (int i = 0; i < resVerts.Length; i++)
            {
                resVerts[i] = resVerts[i] - avg;
            }
            if (resVerts.Length < 4)
            {
                Debug.Log("此区块只有4个点，无法形成模型,故舍弃");
                continue;
            }
            Mesh newMesh = new Mesh();
            newMesh.vertices = resVerts;
            newMesh.triangles = resIndices;
            newMesh.RecalculateNormals();

            GameObject newGameObject = new GameObject("newGameObject");
            newGameObject.transform.position = avg + meshRenderer.transform.position;
            newGameObject.AddComponent<MeshRenderer>().material = meshRenderer.sharedMaterial;
            newGameObject.AddComponent<MeshFilter>().mesh = newMesh;
        }
    }
    #endregion
}
