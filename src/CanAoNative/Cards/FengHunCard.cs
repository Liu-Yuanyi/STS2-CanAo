using CanAoNative.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using CanAoNative.Pools;

namespace CanAoNative.Cards;

/// <summary>
/// 凤魂：每回合前 1 次有牌被消耗时，各抽 1 张牌。升级后费用降为 0。
/// </summary>
public sealed class FengHunCard : CardModel
{
    public override string PortraitPath => CardModel.MissingPortraitPath;
    protected override string PortraitPngPath => CardModel.MissingPortraitPath;

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];

    public FengHunCard()
        : base(
            canonicalEnergyCost: 1,
            type: CardType.Power,
            rarity: CardRarity.Uncommon,
            targetType: TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await PowerCmd.Apply<FengHunPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars.Cards.BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
