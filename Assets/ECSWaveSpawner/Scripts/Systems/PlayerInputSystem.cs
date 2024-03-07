using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class PlayerInputSystem : SystemBase
{
    private PlayerActions inputActions;

    protected override void OnCreate()
    {
        inputActions = new PlayerActions();

        RequireForUpdate<PlayerTag>();
    }

    protected override void OnStartRunning()
    {
        inputActions.Enable();
        inputActions.Simple.Move.started += Handle_MoveStarted;
        inputActions.Simple.Move.performed += Handle_MovePerformed;
        inputActions.Simple.Move.canceled += Handle_MoveCanceled;
    }

    protected override void OnStopRunning()
    {
        inputActions.Simple.Move.started -= Handle_MoveStarted;
        inputActions.Simple.Move.performed -= Handle_MovePerformed;
        inputActions.Simple.Move.canceled -= Handle_MoveCanceled;
        inputActions.Disable();
    }
    protected override void OnUpdate()
    {
        return;
    }

    private void Handle_MoveStarted(InputAction.CallbackContext context)
    {
        //find the PlayerTag entity that also has the inputmoveComp disabled
        foreach ((RefRO<PlayerTag> tag, Entity e) in SystemAPI.Query<RefRO<PlayerTag>>().WithDisabled<InputMoveComponent>().WithEntityAccess())
        {
            //enable the inputComp
            SystemAPI.SetComponentEnabled<InputMoveComponent>(e, true);
        }
    }

    private void Handle_MovePerformed(InputAction.CallbackContext context)
    {
        //find the inputMoveComp that is enabled with the PlayerTag
        foreach (RefRW<InputMoveComponent> inputMoveComp in SystemAPI.Query<RefRW<InputMoveComponent>>().WithAll<PlayerTag>())
        {
            //set the value of inputComp as the context readValue
            inputMoveComp.ValueRW.value = context.ReadValue<Vector2>();
        }
    }

    private void Handle_MoveCanceled(InputAction.CallbackContext context)
    {
        //TODO: find the PlayerTag entity that also has the inputmoveComp enable
        foreach (EnabledRefRW<InputMoveComponent> inputMoveComp in SystemAPI.Query<EnabledRefRW<InputMoveComponent>>().WithAll<PlayerTag>())
        {
            //disable the inputComp
            inputMoveComp.ValueRW = false;
        }
    }

}