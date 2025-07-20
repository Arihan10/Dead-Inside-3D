using UnityEngine;
using System.Collections.Generic;

public class GrassSpawner : MonoBehaviour
{
    public Mesh grassMesh;
    public Material grassMaterial;
    public int instancesPerAxis = 100;
    public float areaSize = 10f;

    List<Matrix4x4> matrices = new List<Matrix4x4>();

    void Start()
    {
        float spacing = areaSize / instancesPerAxis;
        Vector3 center = transform.position - Vector3.one * areaSize * 0.5f;

        for (int x = 0; x < instancesPerAxis; x++)
        {
            for (int z = 0; z < instancesPerAxis; z++)
            {
                Vector3 pos = center + new Vector3(x * spacing, 0f, z * spacing);
                Quaternion rot = Quaternion.Euler(0f, Random.Range(0, 360f), 0f);
                Vector3 scale = Vector3.one * Random.Range(0.8f, 1.2f);
                Matrix4x4 mat = Matrix4x4.TRS(pos, rot, scale);
                matrices.Add(mat);
            }
        }
    }

    void Update()
    {
        int batchSize = 1023; // Max per call
        for (int i = 0; i < matrices.Count; i += batchSize)
        {
            Graphics.DrawMeshInstanced(
                grassMesh,
                0,
                grassMaterial,
                matrices.GetRange(i, Mathf.Min(batchSize, matrices.Count - i))
            );
        }
    }
}
