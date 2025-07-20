using UnityEngine;

public class ZombieAvatarCustomizer : MonoBehaviour
{
    public static ZombieAvatarCustomizer Instance { get; private set; }

    [SerializeField] private GameObject zombieAvatar;
    [SerializeField] private Material[] headMaterials;
    [SerializeField] private Material[] bodyMaterials;
    [SerializeField] private Material[] legMaterials;
    [SerializeField] private Material[] feetMaterials;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public GameObject CreateCustomizedZombie(int headIndex, int bodyIndex, int legIndex, int feetIndex)
    {
        GameObject newZombie = Instantiate(zombieAvatar);
        
        Transform characterMedium = newZombie.transform.Find("characterMedium");

        SkinnedMeshRenderer meshRenderer = characterMedium.GetComponent<SkinnedMeshRenderer>();

        Material[] materials = meshRenderer.materials;
        
        // Set materials at specific indices (skipping index 3 for hands)
        if (materials.Length > 0 && headIndex < headMaterials.Length)
            materials[0] = headMaterials[headIndex];
        
        if (materials.Length > 1 && bodyIndex < bodyMaterials.Length)
            materials[1] = bodyMaterials[bodyIndex];
        
        if (materials.Length > 2 && legIndex < legMaterials.Length)
            materials[2] = legMaterials[legIndex];
        
        if (materials.Length > 4 && feetIndex < feetMaterials.Length)
            materials[4] = feetMaterials[feetIndex];

        meshRenderer.materials = materials;

        return newZombie;
    }
}
