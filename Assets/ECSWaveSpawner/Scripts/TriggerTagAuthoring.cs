using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TriggerTagAuthoring : MonoBehaviour
{
    private class Baker : Baker<TriggerTagAuthoring>
    {
        public override void Bake(TriggerTagAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new TriggerTag());
        }
    }
}

public struct TriggerTag : IComponentData
{

}