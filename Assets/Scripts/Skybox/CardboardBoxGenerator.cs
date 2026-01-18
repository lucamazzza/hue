using UnityEngine;

public class CardboardBoxGenerator : MonoBehaviour
{
    [Header("Impostazioni Scatola")]
    public float size = 50f;
    public Material boxMaterial;

    void Start()
    {
        if (boxMaterial == null)
        {
            Debug.LogError("Per favore, assegna un materiale allo script!");
            return;
        }
        CreateInvertedCube();
    }

    void CreateInvertedCube()
    {
        GameObject box = new GameObject("CardboardSkybox");
        MeshFilter meshFilter = box.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = box.AddComponent<MeshRenderer>();

        GameObject tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Mesh mesh = Instantiate(tempCube.GetComponent<MeshFilter>().sharedMesh);
        Destroy(tempCube);

        Vector3[] normals = mesh.normals;
        for (int i = 0; i < normals.Length; i++) normals[i] = -normals[i];
        mesh.normals = normals;

        int[] triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int temp = triangles[i];
            triangles[i] = triangles[i + 1];
            triangles[i + 1] = temp;
        }
        mesh.triangles = triangles;

        meshFilter.mesh = mesh;
        meshRenderer.material = boxMaterial;
        
        box.transform.localScale = new Vector3(size, size, size);
        box.transform.position = Vector3.zero;
    }
}