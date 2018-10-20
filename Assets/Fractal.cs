using UnityEngine;
using System.Collections;

public class Fractal : MonoBehaviour
{

    public Mesh[] meshes;
    public Material material;

    public int maxDepth;
    public float spawnProbability;
    public float maxRotationSpeed;
    public float maxTwist;

    private float rotationSpeed;

    private int depth;
    private bool root = true;

    private Material[,] materials;

    private void InitializeMaterials()
    {
        materials = new Material[maxDepth + 1,2];
        for (int i = 0; i <= maxDepth; i++)
        {
            float t = i / (maxDepth - 1f);
            t *= t;
            materials[i,0] = new Material(material);
            materials[i,0].color = Color.Lerp(Color.white, Color.yellow, t);
            materials[i, 1] = new Material(material);
            materials[i, 1].color = Color.Lerp(Color.white, Color.cyan, t);
        } 
        materials[maxDepth,0].color = Color.magenta;
        materials[maxDepth, 1].color = Color.red;
    }

    private void Start()
    {
        rotationSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);
        transform.Rotate(Random.Range(-maxTwist, maxTwist), 0f, 0f);
        if (materials == null)
        {
            InitializeMaterials();
        }
        gameObject.AddComponent<MeshFilter>().mesh = meshes[Random.Range(0, meshes.Length)];
        gameObject.AddComponent<MeshRenderer>().material = materials[depth, Random.Range(0, 2)];
        if (depth < maxDepth)
        {
            StartCoroutine(CreateChildren());
        }
    }

    private static Vector3[] childDirections = {
        Vector3.up,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back
    };
    private static Quaternion[] childOrientations = {
        Quaternion.identity,
        Quaternion.Euler(0f, 0f, -90f),
        Quaternion.Euler(0f, 0f, 90f),
        Quaternion.Euler(90f, 0f, 0f),
        Quaternion.Euler(-90f, 0f, 0f)
    };

    private IEnumerator CreateChildren()
    {
        for (int i = 0; i < childDirections.Length; i++)
        {
            if (Random.value < spawnProbability)
            {

                yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
                new GameObject("Fractal Child").AddComponent<Fractal>().
                    Initialize(this, i);
            }
        }
    }

    public float childScale;

    private void Initialize(Fractal parent, int childIndex)
    {
        meshes = parent.meshes;
        materials = parent.materials;
        material = parent.material;
        maxDepth = parent.maxDepth;
        root = false;
        maxRotationSpeed = parent.maxRotationSpeed;
        spawnProbability = parent.spawnProbability;
        maxTwist = parent.maxTwist;
        depth = parent.depth + 1;
        childScale = parent.childScale;
        transform.parent = parent.transform;
        transform.localPosition = childDirections[childIndex] * (0.5f + 0.5f * childScale);
        transform.localScale = Vector3.one * childScale;
        transform.localRotation = childOrientations[childIndex];


    }

    Color[] rainbow =
    {
        Color.red,
        Color.cyan,
        Color.yellow,
        Color.green,
        Color.blue,
        Color.magenta
    };

    private void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
        float scale = 
            Mathf.Clamp(transform.localScale.x * Random.Range(1f - 0.01f, 1f + 0.01f), 0.001f, 2f)
            / transform.localScale.x;
        transform.localScale *= scale;
        if (root)
        {
            Material randomMaterial = materials[Random.Range(0, maxDepth+1), Random.Range(0, 2)];

            randomMaterial.color = Color.Lerp(randomMaterial.color, rainbow[Random.Range(0, rainbow.Length + 1)], 0.1f);
        }
        
    }

}