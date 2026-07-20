using CanAoNative.Pools;
using CanAoNative.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Cards;

/// <summary>
/// 月斩：造成 5（6）点伤害两次。若你有星，获得 1（2）月。
/// </summary>
public sealed class YueZhanCard : CardModel
{
    public override string PortraitPath => CardModel.MissingPortraitPath;
    protected override string PortraitPngPath => CardModel.MissingPortraitPath;

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StarPower>(),
        HoverTipFactory.FromPower<MoonPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new CardsVar(1)
    ];

    public YueZhanCard()
        : base(
            canonicalEnergyCost: 1,
            type: CardType.Attack,
            rarity: CardRarity.Common,
            targetType: TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        Player owner = Owner
            ?? throw new InvalidOperationException(
                "YueZhan requires a card owner.");

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(2)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        if (owner.Creature.GetPower<StarPower>() is not { Amount: > 0 })
            return;

        await PowerCmd.Apply<MoonPower>(
            choiceContext,
            owner.Creature,
            DynamicVars.Cards.IntValue,
            owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
