using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct EnemyUpdateSystem : ISystem
{
    private ComponentLookup<LocalToWorld> localToWorldLookUp;
    private Entity playerEntity;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        localToWorldLookUp = state.GetComponentLookup<LocalToWorld>();
		state.RequireForUpdate<PlayerTag>();
	}

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (playerEntity == Entity.Null)
        {
            playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        }

        EntityCommandBuffer.ParallelWriter ecb = GetECB(ref state);

        localToWorldLookUp.Update(ref state);

        EnemyUpdateJob job = new EnemyUpdateJob
        {
            ecb = ecb,
            localToWorldLookUp = localToWorldLookUp,
            deltaTime = SystemAPI.Time.DeltaTime,
            playerEntity = playerEntity,
        };
        job.ScheduleParallel();

    }

    private EntityCommandBuffer.ParallelWriter GetECB(ref SystemState state)
    {
        BeginSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();

        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        return ecb.AsParallelWriter();
    }
}

[BurstCompile]
public partial struct EnemyUpdateJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ecb;
    public Entity playerEntity;
    public float deltaTime;

    [ReadOnly]
    public ComponentLookup<LocalToWorld> localToWorldLookUp;

    public void Execute([ChunkIndexInQuery] int index, in EnemyComponent enemyComponent, in Entity entity, in LocalToWorld entityL2W)
    {
        LocalToWorld playerL2W = localToWorldLookUp[playerEntity];
        float3 playerWorldPos = playerL2W.Position;

        float3 targetVec = playerWorldPos - entityL2W.Position;
        targetVec = math.normalize(targetVec) * deltaTime * enemyComponent.speed;

        float3 targetPos = entityL2W.Position + targetVec;

        LocalTransform transform = LocalTransform.FromPosition(targetPos);
        ecb.SetComponent<LocalTransform>(index, entity, transform);
    }
}