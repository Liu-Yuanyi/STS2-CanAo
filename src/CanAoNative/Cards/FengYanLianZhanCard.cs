using CanAoNative.Pools;
using CanAoNative.Rules.Exhaust;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Cards;

/// <summary>
/// 凤焰连斩：造成 4 点伤害 3（4）次。若本回合消耗过牌，额外攻击 1（2）次。
/// </summary>
public sealed class FengYanLianZhanCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/feng_yan_lian_zhan.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/feng_yan_lian_zhan.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4m, ValueProp.Move),
        new DynamicVar("Hits", 3m),
        new CardsVar(1)
    ];

    public FengYanLianZhanCard()
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
                "FengYan LianZhan requires a card owner.");

        int hitCount = DynamicVars["Hits"].IntValue;

        if (ExhaustService.HasExhaustedThisTurn(owner))
            hitCount += DynamicVars.Cards.IntValue;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)

            .WithHitFx("vfx/vfx_attack_slash")
            .WithHitCount(hitCount)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Hits"].UpgradeValueBy(1m);
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
