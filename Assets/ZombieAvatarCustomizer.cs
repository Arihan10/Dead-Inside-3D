using UnityEngine;

public class ZombieAvatarCustomizer : MonoBehaviour
{
    public static ZombieAvatarCustomizer Instance { get; private set; }
    [SerializeField] private GameObject[] zombieAvatars;
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

    public GameObject CreateCustomizedZombie(int headIndex, int bodyIndex, int legIndex, int feetIndex, int bodyTypeIndex, bool isMale)  
    {
        // Body type indices: 0 - fat, 1 - fit, 2 - slim
        GameObject newZombie = Instantiate(zombieAvatars[bodyTypeIndex + (isMale ? 0 : 3)]);

        Transform characterMedium = newZombie.transform.Find("characterMedium");

        SkinnedMeshRenderer meshRenderer = characterMedium.GetComponent<SkinnedMeshRenderer>();

        Material[] materials = meshRenderer.materials;
        
        // 0 is head, 1 is body, 2 is leg, 3 is hand, 4 is feet
        materials[0] = headMaterials[headIndex];
        materials[1] = bodyMaterials[bodyIndex];
        materials[2] = legMaterials[legIndex]; // materials[3] is the hand
        materials[4] = feetMaterials[feetIndex];

        meshRenderer.materials = materials;

        return newZombie;
    }
}
