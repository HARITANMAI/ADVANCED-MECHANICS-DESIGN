using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class InputMoveAuthoring : MonoBehaviour
{
    private class Baker : Baker<InputMoveAuthoring>
    {
        public override void Bake(InputMoveAuthoring authoring)
        {
            Entity e = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(e, new InputMoveComponent
            {
                value = float2.zero
            });
            SetComponentEnabled<InputMoveComponent>(e, false);
        }
    }
}

public struct InputMoveComponent : IComponentData, IEnableableComponent
{
    public float2 value;
}