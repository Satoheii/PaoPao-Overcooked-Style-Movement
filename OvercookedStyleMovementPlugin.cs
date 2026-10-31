using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace Satohei.PaoPao.OvercookedStyleMovement;

[BepInPlugin("com.satohei.paopao.overcookedstylemovement", "Overcooked-Style Movement", "1.0.0")]
public sealed class OvercookedStyleMovementPlugin : BasePlugin
{
    private Harmony _harmony = null!;

    public override void Load()
    {
        TurningCore.Initialize(Config);
        DashCore.Initialize(Config);
        _harmony = new Harmony("com.satohei.paopao.overcookedstylemovement");
        _harmony.PatchAll(typeof(OvercookedStyleMovementPlugin).Assembly);
        Log.LogInfo("Overcooked-Style Movement 1.0.0 loaded.");
    }

    public override bool Unload()
    {
        _harmony?.UnpatchSelf();
        return true;
    }
}
