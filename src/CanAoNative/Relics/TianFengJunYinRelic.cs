using CanAoNative.Rules.Edict;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Relics;

/// <summary>
/// 天凤军印：你每次打出诏令后，获得 4 点格挡。
/// </summary>
public sealed class TianFengJunYinRelic :
    RelicModel,
    IAfterEdictPlayed
{
    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(4m, ValueProp.Unpowered)
    ];

    public async Task AfterEdictPlayed(
        PlayerChoiceContext choiceContext,
        EdictPlayedContext context)
    {
        if (!ReferenceEquals(context.Player, Owner))
            return;

        Flash();

        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block,
            null);
    }
}
