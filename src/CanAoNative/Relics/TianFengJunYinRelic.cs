using CanAoNative.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Relics;

/// <summary>
/// 天凤军印：你每次打出诏令后，获得 4 点格挡。
/// Uses the game's own card-play hook directly, mirroring native relics.
/// </summary>
public sealed class TianFengJunYinRelic : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(4m, ValueProp.Unpowered)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<EdictCard>(),
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];

    public override async Task AfterCardPlayedLate(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (cardPlay.Card is not EdictCard
            || cardPlay.Card.Owner != Owner)
        {
            return;
        }

        Flash();

        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block,
            null);
    }
}
