using System;
using HarmonyLib;
using PawPaw.Game.Units.Common.State;
using UnityEngine;

namespace Satohei.PaoPao.OvercookedStyleMovement;

[HarmonyPatch]
internal static class MovementPatches
{
    [HarmonyPatch(typeof(DefaultMovementState), nameof(DefaultMovementState.UpdateRotation))]
    [HarmonyPrefix]
    private static void ApplyTurning(DefaultMovementState __instance, out float __state)
    {
        __state = TurningCore.Apply(__instance);
    }

    [HarmonyPatch(typeof(DefaultMovementState), nameof(DefaultMovementState.UpdateRotation))]
    [HarmonyFinalizer]
    private static Exception RestoreTurning(DefaultMovementState __instance, float __state, Exception __exception)
    {
        TurningCore.Restore(__instance, __state);
        return __exception;
    }

    [HarmonyPatch(typeof(DashingMovementState), nameof(DashingMovementState.Enter))]
    [HarmonyPostfix]
    private static void EnterDash(DashingMovementState __instance)
    {
        DashCore.Enter(__instance);
    }

    [HarmonyPatch(typeof(DashingMovementState), nameof(DashingMovementState.Exit))]
    [HarmonyPrefix]
    private static void ExitDash(DashingMovementState __instance)
    {
        DashCore.PrepareExit(__instance);
    }

    [HarmonyPatch(typeof(DashingMovementState), nameof(DashingMovementState.UpdateRotation))]
    [HarmonyPrefix]
    private static bool UpdateDashRotation(DashingMovementState __instance, ref Quaternion currentRotation, float deltaTime)
    {
        return !DashCore.UpdateRotation(__instance, ref currentRotation, deltaTime);
    }

    [HarmonyPatch(typeof(DashingMovementState), nameof(DashingMovementState.UpdateVelocity))]
    [HarmonyPostfix]
    private static void UpdateDashVelocity(DashingMovementState __instance, ref Vector3 currentVelocity, float deltaTime)
    {
        DashCore.UpdateVelocity(__instance, ref currentVelocity, deltaTime);
    }

    [HarmonyPatch(typeof(DashingMovementState), nameof(DashingMovementState.AfterCharacterUpdate))]
    [HarmonyPrefix]
    private static void PrepareDashAfterCharacterUpdate(DashingMovementState __instance, out Vector3 __state)
    {
        __state = DashCore.PrepareAfterCharacterUpdate(__instance);
    }

    [HarmonyPatch(typeof(DashingMovementState), nameof(DashingMovementState.AfterCharacterUpdate))]
    [HarmonyFinalizer]
    private static Exception RestoreDashAfterCharacterUpdate(DashingMovementState __instance, Vector3 __state, Exception __exception)
    {
        DashCore.RestoreAfterCharacterUpdate(__instance, __state);
        return __exception;
    }

}
