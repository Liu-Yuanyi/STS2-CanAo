using CanAoNative.Pools;
using CanAoNative.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Cards;

/// <summary>
/// 最后一舞：造成 12（15）点伤害，获得 1（2）费，抽满手牌。
/// 本回合结束时，每拥有一张手牌，失去 2 点生命。消耗。
/// </summary>
public sealed class ZuiHouYiWuCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/zui_hou_yi_wu.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/zui_hou_yi_wu.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12m, ValueProp.Move),
        new EnergyVar(1),
        new CardsVar(2)
    ];

    public ZuiHouYiWuCard()
        : base(
            canonicalEnergyCost: 0,
            type: CardType.Attack,
            rarity: CardRarity.Rare,
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
                "ZuiHou YiWu requires a card owner.");

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        await PlayerCmd.GainEnergy(
            DynamicVars.Energy.IntValue,
            owner);

        int toDraw =
            CardPile.MaxCardsInHand - owner.PlayerCombatState.Hand.Cards.Count;

        if (toDraw > 0)
            await CardPileCmd.Draw(choiceContext, (decimal)toDraw, owner);

        await PowerCmd.Apply<ZuiHouYiWuPower>(
            choiceContext,
            owner.Creature,
            DynamicVars.Cards.IntValue,
            owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars.Energy.UpgradeValueBy(1m);
    }
}
