using HarmonyLib;
using JetBrains.Annotations;
using SmithingPlus.Util;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace SmithingPlus.BitsRecovery;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
[HarmonyPatch(typeof(CollectibleBehaviorQuenchable), "IsGettingCooled")]
[HarmonyPatchCategory(Core.BitsRecoveryCategory)]
public class QuenchRecoveryPatches
{
    [HarmonyPrefix]
    public static void IsGettingCooled_Prefix(
        IWorldAccessor world,
        ItemSlot slot,
        Vec3d pos,
        float dt,
        float temperature,
        out ItemStack __state
    )
    {
        __state = slot.Itemstack?.Clone();
    }

    [HarmonyPostfix]
    public static void IsGettingCooled_Postfix(
        IWorldAccessor world,
        ItemSlot slot,
        Vec3d pos,
        float dt,
        float temperature,
        CollectibleBehaviorQuenchable __instance,
        ItemStack __state
    )
    {
        if (slot.Itemstack != null) return;
        if (__state == null) return;

        var bitsStack = __state.GetShatteredBitsStack(world.Api);
        if (bitsStack == null)
            //Core.Logger.VerboseDebug($"[BitsRecovery] No metal bits recovered for {__state.Collectible.Code}");
            return;

        world.SpawnItemEntity(bitsStack, pos, new Vec3d(0, 0.5, 0));
        //Core.Logger.VerboseDebug(
        //$"[BitsRecovery] Spawned {bitsStack.StackSize} metal bits from shattered {__state.Collectible.Code}");
    }
}