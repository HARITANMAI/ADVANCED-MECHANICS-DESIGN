using Unity.Entities;
using UnityEngine;

public class EnemySpawnerAuthoring : MonoBehaviour
{
    public GameObject prefabToSpawn;

    [Range(0.1f, 1.0f)] public float spawnRate = 0.5f;

    public int spawnCount;

    public int spawnMax;

    [BakingType]
    private class Baker : Baker<EnemySpawnerAuthoring>
    {
        public override void Bake(EnemySpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new EnemySpawnerComponent
            {
                prefabToSpawn = GetEntity(authoring.prefabToSpawn, TransformUsageFlags.Dynamic),

                spawnerPosition = authoring.transform.position,
                spawnRate = authoring.spawnRate,
                timer = 0.0f,
                spawnCount = authoring.spawnCount,
                spawnMax = authoring.spawnMax
            });
        }
    }
}