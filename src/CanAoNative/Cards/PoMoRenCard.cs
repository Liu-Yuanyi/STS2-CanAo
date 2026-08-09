using CanAoNative.Pools;
using CanAoNative.Rules.YuHuo;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Cards;

/// <summary>
/// 破魔刃：浴火。造成 7（10）点伤害。施加 2（3）层易伤。
/// </summary>
public sealed class PoMoRenCard : CardModel, IIntrinsicYuHuo
{
    public override string PortraitPath => "res://images/card_portraits/canao/po_mo_ren.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/po_mo_ren.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    public bool HasIntrinsicYuHuo => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move),
        new CardsVar(2)
    ];

    public PoMoRenCard()
        : base(
            canonicalEnergyCost: 1,
            type: CardType.Attack,
            rarity: CardRarity.Uncommon,
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
                "PoMoRen requires a card owner.");

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        await PowerCmd.Apply<VulnerablePower>(
            choiceContext,
            cardPlay.Target,
            DynamicVars.Cards.IntValue,
            owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
