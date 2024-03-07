using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;

[BurstCompile]
public partial struct EnemySpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (RefRW<EnemySpawnerComponent> spawner in SystemAPI.Query<RefRW<EnemySpawnerComponent>>())
        {
            UpdateSpawner(ref state, spawner);
        }
    }

    private void UpdateSpawner(ref SystemState state, RefRW<EnemySpawnerComponent> spawner)
    {
        if (spawner.ValueRO.spawnCount < spawner.ValueRW.spawnMax)
        {
            spawner.ValueRW.timer += SystemAPI.Time.DeltaTime;
            if (spawner.ValueRO.timer < spawner.ValueRO.spawnRate) { return; }

            SpawnEnemy(ref state, spawner);
            spawner.ValueRW.timer -= spawner.ValueRO.spawnRate;
            spawner.ValueRW.spawnCount++;
        }
    }

    private void SpawnEnemy(ref SystemState state, RefRW<EnemySpawnerComponent> spawner)
    {
        Entity entity = state.EntityManager.Instantiate(spawner.ValueRO.prefabToSpawn);
        LocalTransform localTransform = LocalTransform.FromPosition(spawner.ValueRO.spawnerPosition);
        state.EntityManager.SetComponentData(entity, localTransform);
    }
}
