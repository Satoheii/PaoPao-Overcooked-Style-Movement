using System;
using BepInEx.Configuration;
using PawPaw.Game.Units.Common.State;

namespace Satohei.PaoPao.OvercookedStyleMovement;

internal static class TurningCore
{
    internal const bool DefaultEnabled = true;
    internal const float DefaultTurnSharpnessMultiplier = 2.5f;
    internal const float MinimumTurnSharpnessMultiplier = 0.1f;
    internal const float MaximumTurnSharpnessMultiplier = 20f;

    private static ConfigEntry<bool> Enabled { get; set; } = null!;
    private static ConfigEntry<float> TurnSharpnessMultiplier { get; set; } = null!;

    internal static void Initialize(ConfigFile config)
    {
        Enabled = config.Bind("Precise Turning", "Enabled", DefaultEnabled, "Enable faster player turning.");
        TurnSharpnessMultiplier = config.Bind("Precise Turning", "TurnSharpnessMultiplier", DefaultTurnSharpnessMultiplier, "Multiplier applied to the original turn sharpness while the local player rotates.");
    }

    internal static float Apply(DefaultMovementState state)
    {
        var originalSharpness = state._movementConfig.TurnSharpness;
        if (Enabled.Value && state._character.IsOwner)
        {
            var multiplier = TurnSharpnessMultiplier.Value;
            if (!float.IsFinite(multiplier))
            {
                multiplier = DefaultTurnSharpnessMultiplier;
            }
            state._movementConfig.TurnSharpness = originalSharpness * Math.Clamp(multiplier, MinimumTurnSharpnessMultiplier, MaximumTurnSharpnessMultiplier);
        }

        return originalSharpness;
    }

    internal static void Restore(DefaultMovementState state, float originalSharpness)
    {
        state._movementConfig.TurnSharpness = originalSharpness;
    }
}
