using System.Text;
using SmithingPlus.Util;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;

namespace SmithingPlus.ClientTweaks;

public class CollectibleBehaviorQuenchableInfo(CollectibleObject collObj) : CollectibleBehavior(collObj)
{
    public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
    {
        base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);
        if (inSlot.Itemstack is not { } itemStack)
            return;
        var temperature =
            (int)itemStack.Collectible.GetTemperature(world, itemStack);

        var metalProps = itemStack.Collectible
            .GetBehavior<CollectibleBehaviorQuenchable>()
            ?.GetMetalProps();

        if (metalProps == null) return;
        var quenchIteration = itemStack.Attributes.GetInt("quenchIteration");
        var temperIteration = itemStack.Attributes.GetInt("temperIteration");

        var localizedStringQ = Lang.Get("itemstack-quenchable",
            metalProps.quenchMinTemp,
            metalProps.quenchMaxTemp);

        if (temperature > metalProps.quenchMinTemp && temperature < metalProps.quenchMaxTemp)
        {
            var replacementQ = ShowWorkablePatches.TemperatureRangeRegex().Replace(localizedStringQ,
                ShowWorkablePatches.SetColor("$1", Constants.QuenchableColor));

            dsc.Replace(localizedStringQ, replacementQ);
        }

        if (quenchIteration <= temperIteration) return;
        var localizedStringT = Lang.Get("itemstack-temperable",
            metalProps.temperMinTemp,
            metalProps.temperMaxTemp);

        if (temperature <= metalProps.temperMinTemp || temperature >= metalProps.temperMaxTemp) return;
        var replacementT = ShowWorkablePatches.TemperatureRangeRegex().Replace(localizedStringT,
            ShowWorkablePatches.SetColor("$1", Constants.QuenchableColor));

        dsc.Replace(localizedStringT, replacementT);
    }
}