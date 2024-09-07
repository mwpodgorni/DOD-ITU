using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct EnemySpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameplayInteractionSingleton>();
    }

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var dt = SystemAPI.Time.DeltaTime; 
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

        new EnemeySpawnJob
        {
            dt = dt,
            ecb = ecb
        }.Run();
        ecb.Playback(state.EntityManager);
    }
}

// public partial class EnemySpawnerSystem1 : SystemBase
// {
//     protected override void OnCreate()
//     {
//         RequireForUpdate<GameplayInteractionSingleton>();
//     }
//
//     protected override void OnUpdate()
//     {
//         var dt = SystemAPI.Time.DeltaTime; 
//         var ecb = new EntityCommandBuffer(World.UpdateAllocator.ToAllocator);
//
//         new EnemeySpawnJob
//         {
//             dt = dt,
//             ecb = ecb
//         }.Run();
//         /* Entities.ForEach((Entity entity, ref ChaserSpawner chaserSpawn, in Translation trans) =>
//          {
//              chaserSpawn.timer -= dt;
//              if (chaserSpawn.timer < 0)
//              {
//                  chaserSpawn.timer = chaserSpawn.timerDelay;
//                  Entity e = EntityManager.Instantiate(chaserSpawn.chaser); 
//                  EntityManager.SetComponentData(e, new Translation
//                  {
//                      Value = trans.Value + new float3(
//                          chaserSpawn.random.NextFloat(-11, 11),
//                          0,
//                          chaserSpawn.random.NextFloat(-8, 8))       
//                  });
//                  EntityManager.SetComponentData(e, new moveData
//                  {
//                      moveSpeed = chaserSpawn.random.NextFloat(2, 6),
//                      rotationSpeed = chaserSpawn.random.NextFloat(.3f, .7f)
//                  });
//              }
//          }).WithStructuralChanges().Run();
//          */
//         
//         ecb.Playback(EntityManager);
//     }
// }




[BurstCompile]
public partial struct EnemeySpawnJob : IJobEntity
{
    public float dt;
    public EntityCommandBuffer ecb; 
    public void Execute(ref ChaserSpawner chaserSpawn, in LocalTransform trans)
    {
        chaserSpawn.timer -= dt;
        if (chaserSpawn.timer < 0)
        {
            chaserSpawn.timer = chaserSpawn.timerDelay;
            Entity e = ecb.Instantiate(chaserSpawn.chaser); 
            
           ecb.SetComponent(e, new LocalTransform
           {
               Position = trans.Position + new float3(
                   chaserSpawn.random.NextFloat(-11, 11),
                   0,
                   chaserSpawn.random.NextFloat(-8, 8)), 
               Rotation = quaternion.identity,
               Scale = 1f
           });
            ecb.SetComponent(e, new moveData
            {
                moveSpeed = chaserSpawn.random.NextFloat(2, 6),
                rotationSpeed = chaserSpawn.random.NextFloat(.3f, .7f)
            });
        }
    }
}
