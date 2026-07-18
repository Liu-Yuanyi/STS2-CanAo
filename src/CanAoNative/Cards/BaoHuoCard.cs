using CanAoNative.Pools;
using CanAoNative.Rules.FengWei;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Cards;

/// <summary>
/// 抱火：获得 4（7）点格挡。若你的凤威大于 0，额外获得等量格挡。
/// </summary>
public sealed class BaoHuoCard : CardModel
{
    public override string PortraitPath => CardModel.MissingPortraitPath;
    protected override string PortraitPngPath => CardModel.MissingPortraitPath;
    public override bool GainsBlock => true;

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(4m, ValueProp.Move)
    ];

    public BaoHuoCard()
        : base(
            canonicalEnergyCost: 0,
            type: CardType.Skill,
            rarity: CardRarity.Common,
            targetType: TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        Player owner = Owner
            ?? throw new InvalidOperationException(
                "BaoHuo requires a card owner.");

        decimal block = DynamicVars.Block.BaseValue;

        decimal fengWei = FengWeiService.GetEffectiveAmount(owner);
        if (fengWei > 0m)
            block += fengWei;

        await CreatureCmd.GainBlock(
            owner.Creature,
            block,
            ValueProp.Move,
            cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
