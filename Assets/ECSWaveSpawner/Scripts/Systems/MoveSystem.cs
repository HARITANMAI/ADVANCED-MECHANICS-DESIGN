using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct MoveSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<InputMoveComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        MoveJob job = new MoveJob
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };

        job.ScheduleParallel();
    }

    [WithAll(typeof(PlayerTag))]
    [BurstCompile]
    private partial struct MoveJob : IJobEntity
    {
        public float deltaTime;

        public void Execute(ref LocalTransform transform, in InputMoveComponent inMove)
        {
            transform = transform.Translate(new float3(inMove.value.x, 0f, inMove.value.y) * deltaTime /* speed multiplier */ );
        }
    }
}