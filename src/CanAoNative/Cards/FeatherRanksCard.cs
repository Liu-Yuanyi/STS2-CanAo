using CanAoNative.Rules.YuHuo;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using CanAoNative.Pools;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Cards;

/// <summary>
/// 羽列千军：保留。浴火。造成 9（12）点伤害；
/// 因浴火触发时改为对所有敌人造成伤害。
/// This is the first production card that consumes YuHuoExecutionContext.
/// </summary>
public sealed class FeatherRanksCard : CardModel, IIntrinsicYuHuo
{
    public override string PortraitPath => "res://images/card_portraits/canao/feather_ranks.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/feather_ranks.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    public bool HasIntrinsicYuHuo => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Retain
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move)
    ];

    public FeatherRanksCard()
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
        AttackCommand attack =
            DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .WithHitFx("vfx/vfx_attack_slash")
                .FromCard(this, cardPlay);

        if (YuHuoService.IsTriggeredByYuHuo(this))
        {
            Player? owner = Owner;
            ICombatState? combatState =
                CombatState ?? owner?.Creature?.CombatState;

            if (combatState == null)
            {
                throw new InvalidOperationException(
                    "Feather Ranks requires an active combat state.");
            }

            await attack
                .TargetingAllOpponents(combatState)
                .Execute(choiceContext);

            return;
        }

        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await attack
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
