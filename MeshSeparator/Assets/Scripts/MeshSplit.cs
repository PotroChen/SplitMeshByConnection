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
        int[] indices = mesh.triangles;
        Vector3[] verts = mesh.vertices;

        //allIndices和restIndices初始化赋值
        for (int i = 0; i < indices.Length; i++)
        {
            allIndices.Add(indices[i]);
            restIndices.Add(indices[i]);
        }
        while (restIndices.Count > 0)
        {
            newIndices.Clear();
            //得到第一个三角形
            for (int i = 0; i < 3; i++)
            {
                newIndices.Add(restIndices[i]);
            }

            //遍历剩下的三角形
            for (int i = 1; i < restIndices.Count / 3; i++)
            {
                //判断该三角形与newIndices是否有共用的顶点，若有，就该三角形的所有顶点加入newIndices
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

            //newIndices为分离出来的mesh的顶点索引信息，每三个值代表着一个三角面之间的关系，每个值代表着顶点在verts中的位置。
            //注：这时，我们不能将它直接作为索引信息数组赋给新的mesh中，因为它的值为代表着顶点在verts中的位置，而不是splitedVerts。

            //1.创建新数组verIndMap，将newIndices去重复，并从小到大排序的结果填入
            // 注：此时，处理完verIndMap为splitedVerts与verts之间的映射表，顶点splitedVerts[i]就对应着verts[verIndMap[i]]
            List<int> verIndMap = new List<int>();

            foreach (int index in newIndices)//去重复
            {
                if (!verIndMap.Contains(index))
                    verIndMap.Add(index);
            }

            verIndMap.Sort();//排序

            //2.我们要创建splitedVerts(分离出来的顶点的数组)
            Vector3[] splitedVerts = new Vector3[verIndMap.Count];
            //有了verIndMap的帮助，我们可以遍历verIndMap将Verts中我们所需要的顶点信息加入splitedVerts中
            for (int i = 0; i < verIndMap.Count; i++)
            {
                splitedVerts[i] = verts[verIndMap[i]];
            }

            //3.我们要创建splitedIndices(分离出来的Indice的数组)
            int[] splitedIndices = new int[newIndices.Count];
            //有了verIndMap的帮助，根据newnewIndices得到对应splitedVerts的新的索引信息splitedIndices
            for (int i = 0; i < newIndices.Count; i++)
            {
                for (int j = 0; j < verIndMap.Count; j++)
                {
                    if (newIndices[i] == verIndMap[j])
                        splitedIndices[i] = j;
                }
                    
            }

            //4.算出splitedVerts中所有的点的平均值，再将verts所有值减去这个值（verts原点归零）
            Vector3 sum = Vector3.zero;
            foreach (Vector3 item in splitedVerts)
                sum += item;

            Vector3 avg = sum / splitedVerts.Length;
            for (int i = 0; i < splitedVerts.Length; i++)
            {
                splitedVerts[i] = splitedVerts[i] - avg;
            }
            if (splitedVerts.Length < 4)
            {
                Debug.Log("此区块只有4个点，无法形成模型,故舍弃");
                return;
            }

            Mesh newMesh = new Mesh();
            newMesh.vertices = splitedVerts;
            newMesh.triangles = splitedIndices;
            newMesh.RecalculateNormals();

            GameObject newGameObject = new GameObject("newGameObject");
            newGameObject.transform.position = avg + meshRenderer.transform.position;
            newGameObject.AddComponent<MeshRenderer>().material = meshRenderer.sharedMaterial;
            newGameObject.AddComponent<MeshFilter>().mesh = newMesh;
        }
    }
    #endregion
}
