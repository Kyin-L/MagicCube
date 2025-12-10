using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MagicCubeCreator : MonoBehaviour
{
    [InspectorButton("Create")]
    public bool create;


    public GameObject prefab;
    public Material[] materials;
    public GameObject[,,] cubes;
    public GameObject[,] blocks;

    private void Create()
    {
        GetPrefab();
        GetMaterials();
        CreateCube();
    }

    private void GetPrefab()
    {
        if(prefab == null)
            prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/MagicCube/Prefabs/Block.prefab");
    }

    private void GetMaterials()
    {
        if(materials == null || materials.Length == 0)
            materials = new Material[6];


        for (int i = 0; i < 6; i++)
        {
            materials[i] = AssetDatabase.LoadAssetAtPath<Material>(string.Format("Assets/MagicCube/Materials/mc{0}.mat", i+1));
        }
    }

    private void CreateCube()
    {
        if (cubes == null || cubes.Length == 0)
            cubes = new GameObject[3, 3, 3];

        for (int y = 0; y < 3; y++)
        {
            for (int z = 0; z < 3; z++)
            {
                for (int x = 0; x < 3; x++)
                {
                    GameObject cube = new GameObject(string.Format("Cube{0}{1}{2}", x, y, z));
                    cube.transform.parent = transform;
                    cube.transform.localPosition = new Vector3(x - 1, 1 - y, 1 - z);
                    cube.AddComponent<BoxCollider>();
                    AddBlock(cube.transform, x, y, z);
                    cube.AddComponent<Cube>().offset = cube.transform.localPosition;
                    cube.layer = LayerMask.NameToLayer("MagicCube");
                    cubes[x, y, z] = cube;
                }
            }
        }
    }

    private void AddBlock(Transform cube, int x, int y, int z)
    {
        if (x == 0)
            AddLeftBlock(cube);
        else if (x == 2)
            AddRightBlock(cube);

        if (y == 0)
            AddUpBlock(cube);
        else if (y == 2)
            AddDownBlock(cube);

        if (z == 0)
            AddBehindBlock(cube);
        else if (z == 2)
            AddForwardBlock(cube);
    }

    private void AddLeftBlock(Transform cube)
    {
        GameObject block = Instantiate(prefab, cube);
        block.transform.localPosition = new Vector3(-0.5f, 0, 0);
        block.transform.localRotation = Quaternion.Euler(0, 90, 0);
        block.GetComponent<MeshRenderer>().material = materials[3];
    }

    private void AddRightBlock(Transform cube)
    {
        GameObject block = Instantiate(prefab, cube);
        block.transform.localPosition = new Vector3(0.5f, 0, 0);
        block.transform.localRotation = Quaternion.Euler(0, -90, 0);
        block.GetComponent<MeshRenderer>().material = materials[1];
    }

    private void AddUpBlock(Transform cube)
    {
        GameObject block = Instantiate(prefab, cube);
        block.transform.localPosition = new Vector3(0, 0.5f, 0);
        block.transform.localRotation = Quaternion.Euler(90, 0, 0);
        block.GetComponent<MeshRenderer>().material = materials[4];
    }

    private void AddDownBlock(Transform cube)
    {
        GameObject block = Instantiate(prefab, cube);
        block.transform.localPosition = new Vector3(0, -0.5f, 0);
        block.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        block.GetComponent<MeshRenderer>().material = materials[5];
    }
    private void AddBehindBlock(Transform cube)
    {
        GameObject block = Instantiate(prefab, cube);
        block.transform.localPosition = new Vector3(0, 0, 0.5f);
        block.transform.localRotation = Quaternion.Euler(0, 180, 0);
        block.GetComponent<MeshRenderer>().material = materials[2];
    }
    private void AddForwardBlock(Transform cube)
    {
        GameObject block = Instantiate(prefab, cube);
        block.transform.localPosition = new Vector3(0, 0, -0.5f);
        block.transform.localRotation = Quaternion.Euler(0, 0, 0);
        block.GetComponent<MeshRenderer>().material = materials[0];
    }
}
