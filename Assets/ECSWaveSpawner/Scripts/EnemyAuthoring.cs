using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
    [Range(0.1f, 15f)] public float speed = 1f;

    private class Baker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnemyComponent
            {
                speed = authoring.speed
            });
        }
    }
}

public struct EnemyComponent : IComponentData
{
    public float speed;
}