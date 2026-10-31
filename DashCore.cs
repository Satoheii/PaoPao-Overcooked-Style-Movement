using System;
using BepInEx.Configuration;
using PawPaw.Game.Units.Common.State;
using UnityEngine;

namespace Satohei.PaoPao.OvercookedStyleMovement;

internal static class DashCore
{
    internal const bool DefaultEnabled = true;
    internal const float DefaultPeakSpeedMultiplier = 3f;
    internal const float DefaultDashDuration = 0.3f;
    internal const float DefaultDashCooldown = 0.4f;
    internal const float DefaultDashTurnSpeed = 20f;
    internal const float MinimumPeakSpeedMultiplier = 1f;
    internal const float MaximumPeakSpeedMultiplier = 5f;
    internal const float MinimumDashCooldown = 0f;
    internal const float MaximumDashCooldown = 5f;
    internal const float MinimumDashTurnSpeed = 1f;
    internal const float MaximumDashTurnSpeed = 100f;
    private static ConfigEntry<bool> Enabled { get; set; } = null!;
    private static ConfigEntry<float> PeakSpeedMultiplier { get; set; } = null!;
    private static ConfigEntry<float> DashCooldown { get; set; } = null!;
    private static ConfigEntry<float> DashTurnSpeed { get; set; } = null!;

    internal static void Initialize(ConfigFile config)
    {
        Enabled = config.Bind("Overcooked Dash", "Enabled", DefaultEnabled, "Enable the Overcooked-style timed velocity blend.");
        PeakSpeedMultiplier = config.Bind("Overcooked Dash", "PeakSpeedMultiplier", DefaultPeakSpeedMultiplier, "Peak dash speed relative to sprint speed before the sinusoidal decay.");
        DashCooldown = config.Bind("Overcooked Dash", "DashCooldown", DefaultDashCooldown, "Time in seconds between dash starts.");
        DashTurnSpeed = config.Bind("Overcooked Dash", "TurnSpeed", DefaultDashTurnSpeed, "Dash turning speed in radians per second.");
    }

    internal static void Enter(DashingMovementState state)
    {
        if (!IsActive(state))
        {
            return;
        }

        var sprintSpeed = Math.Max(0f, state._movementConfig.SprintSpeed);
        if (!float.IsFinite(sprintSpeed) || sprintSpeed <= 0f)
        {
            return;
        }

        state._predictedTime = DefaultDashDuration;
        state._elapsedTime = 0f;
        var configuredCooldown = Math.Clamp(DashCooldown.Value, MinimumDashCooldown, MaximumDashCooldown);
        if (!float.IsFinite(configuredCooldown)) configuredCooldown = DefaultDashCooldown;
        var originalCooldown = Math.Max(0f, state._movementConfig.DashCooldown);
        if (float.IsFinite(originalCooldown)) state._status.LastDashTime += configuredCooldown - originalCooldown;
    }

    internal static bool UpdateRotation(DashingMovementState state, ref Quaternion currentRotation, float deltaTime)
    {
        if (!IsActive(state))
        {
            return false;
        }

        var facingDirection = currentRotation * Vector3.forward;
        if (!IsFinite(facingDirection) || !float.IsFinite(deltaTime) || deltaTime < 0f)
        {
            return false;
        }
        facingDirection.y = 0f;
        if (facingDirection.sqrMagnitude <= Mathf.Epsilon)
        {
            facingDirection = state._enterMoveVector;
        }

        if (facingDirection.sqrMagnitude <= Mathf.Epsilon)
        {
            facingDirection = Vector3.forward;
        }

        facingDirection.Normalize();
        var inputDirection = state._status.MoveVector;
        inputDirection.y = 0f;
        if (!IsFinite(inputDirection))
        {
            return false;
        }
        if (inputDirection.sqrMagnitude > Mathf.Epsilon)
        {
            inputDirection.Normalize();
            var turnSpeed = Math.Clamp(DashTurnSpeed.Value, MinimumDashTurnSpeed, MaximumDashTurnSpeed);
            if (!float.IsFinite(turnSpeed)) turnSpeed = DefaultDashTurnSpeed;
            facingDirection = Vector3.RotateTowards(facingDirection, inputDirection, turnSpeed * deltaTime, 0f);
            facingDirection.Normalize();
            currentRotation = Quaternion.LookRotation(facingDirection, Vector3.up);
        }

        state._enterMoveVector = facingDirection;
        return true;
    }

    internal static bool UpdateVelocity(DashingMovementState state, ref Vector3 currentVelocity, float deltaTime)
    {
        if (!IsActive(state))
        {
            return false;
        }

        var duration = DefaultDashDuration;
        if (duration <= 0f)
        {
            return true;
        }

        var facingDirection = state._enterMoveVector;
        facingDirection.y = 0f;
        if (facingDirection.sqrMagnitude <= Mathf.Epsilon)
        {
            facingDirection = state._character.transform.forward;
            facingDirection.y = 0f;
        }

        if (facingDirection.sqrMagnitude <= Mathf.Epsilon)
        {
            return true;
        }

        facingDirection.Normalize();
        var inputDirection = state._status.MoveVector;
        inputDirection.y = 0f;
        if (!IsFinite(inputDirection) || !IsFinite(facingDirection)) return false;
        inputDirection = Vector3.ClampMagnitude(inputDirection, 1f);

        if (!float.IsFinite(deltaTime) || deltaTime < 0f)
        {
            return false;
        }

        var runSpeed = Math.Max(0f, state._movementConfig.SprintSpeed);
        var peakSpeedMultiplier = PeakSpeedMultiplier.Value;
        if (!float.IsFinite(peakSpeedMultiplier))
        {
            peakSpeedMultiplier = DefaultPeakSpeedMultiplier;
        }
        var dashSpeed = runSpeed * Math.Clamp(peakSpeedMultiplier, MinimumPeakSpeedMultiplier, MaximumPeakSpeedMultiplier);
        var elapsedBeforeUpdate = state._elapsedTime - deltaTime;
        if (!float.IsFinite(runSpeed) || !float.IsFinite(dashSpeed) || !float.IsFinite(elapsedBeforeUpdate))
        {
            return false;
        }
        elapsedBeforeUpdate = Math.Clamp(elapsedBeforeUpdate, 0f, duration);
        var remainingTime = duration - elapsedBeforeUpdate;
        var dashWeight = 0.5f * (1f - Mathf.Cos(Mathf.PI * remainingTime / duration));
        var horizontalVelocity = inputDirection * runSpeed * (1f - dashWeight) + facingDirection * dashSpeed * dashWeight;
        horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, dashSpeed);
        if (!IsFinite(horizontalVelocity)) return false;

        currentVelocity.x = horizontalVelocity.x;
        currentVelocity.z = horizontalVelocity.z;
        return true;
    }

    internal static void PrepareExit(DashingMovementState state)
    {
        if (!IsActive(state))
        {
            return;
        }

        if (!float.IsFinite(state._elapsedTime) || !float.IsFinite(state._predictedTime))
        {
            return;
        }

        if (state._elapsedTime < state._predictedTime)
        {
            return;
        }

        var runSpeed = Math.Max(0f, state._movementConfig.SprintSpeed);
        var inputDirection = state._status.MoveVector;
        inputDirection.y = 0f;
        if (!float.IsFinite(runSpeed) || !IsFinite(inputDirection))
        {
            return;
        }

        inputDirection = Vector3.ClampMagnitude(inputDirection, 1f);
        var exitVelocity = state._startVelocity;
        exitVelocity.x = inputDirection.x * runSpeed;
        exitVelocity.z = inputDirection.z * runSpeed;
        state._startVelocity = exitVelocity;
    }

    internal static Vector3 PrepareAfterCharacterUpdate(DashingMovementState state)
    {
        var startPosition = state._startPosition;
        if (IsActive(state))
        {
            var currentPosition = state._motor.TransientPosition;
            state._startPosition = currentPosition;
        }

        return startPosition;
    }

    internal static void RestoreAfterCharacterUpdate(DashingMovementState state, Vector3 startPosition)
    {
        state._startPosition = startPosition;
    }

    private static bool IsActive(DashingMovementState state)
    {
        return Enabled.Value && state._character.IsOwner;
    }

    private static bool IsFinite(Vector3 value)
    {
        return float.IsFinite(value.x) && float.IsFinite(value.y) && float.IsFinite(value.z);
    }
}
