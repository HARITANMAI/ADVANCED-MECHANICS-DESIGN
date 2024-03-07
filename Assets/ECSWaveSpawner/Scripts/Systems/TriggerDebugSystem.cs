using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
[BurstCompile]
public partial struct TriggerDebugSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Dependency = new TriggerDebugJob
        {
            LookUpPhysicsMass = state.GetComponentLookup<PhysicsMass>(),
            LookupPhysicsVelocity = state.GetComponentLookup<PhysicsVelocity>(),
            LookUpPlayerTag = state.GetComponentLookup<PlayerTag>(),
            LookUpTriggerTag = state.GetComponentLookup<TriggerTag>(),
            deltaTime = SystemAPI.Time.DeltaTime

        }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }

    private partial struct TriggerDebugJob : ITriggerEventsJob
    {
        public float deltaTime;
        public ComponentLookup<PhysicsVelocity> LookupPhysicsVelocity;

        [ReadOnly] public ComponentLookup<PhysicsMass> LookUpPhysicsMass;
        [ReadOnly] public ComponentLookup<PlayerTag> LookUpPlayerTag;
        [ReadOnly] public ComponentLookup<TriggerTag> LookUpTriggerTag;

        public void Execute(TriggerEvent triggerEvent)
        {
            bool isBodyAPlayer = LookUpPlayerTag.HasComponent(triggerEvent.EntityA);
            bool isBodyBPlayer = LookUpPlayerTag.HasComponent(triggerEvent.EntityB);

            if (!isBodyAPlayer && !isBodyBPlayer) { return; }

            bool isBodyATrigger = LookUpTriggerTag.HasComponent(triggerEvent.EntityA);
            bool isBodyBTrigger = LookUpTriggerTag.HasComponent(triggerEvent.EntityB);

            if (!isBodyATrigger && !isBodyBTrigger) { return; }

            Entity playerEntity = isBodyAPlayer ? triggerEvent.EntityA : triggerEvent.EntityB;

            PhysicsVelocity playerVel = LookupPhysicsVelocity[playerEntity];
            playerVel.ApplyLinearImpulse(LookUpPhysicsMass[playerEntity], math.up() * 100f * deltaTime);
            LookupPhysicsVelocity[playerEntity] = playerVel;

            UnityEngine.Debug.Log(playerEntity);
        }
    }
}