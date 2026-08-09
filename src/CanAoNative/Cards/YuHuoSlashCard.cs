using CanAoNative.Rules.YuHuo;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using CanAoNative.Pools;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Cards;

/// <summary>
/// YuHuo Slash (浴火斩): test card for the 浴火 (YuHuo) mechanism.
/// 1-cost Attack, 6(8) damage. 浴火, Exhaust.
///
/// When played normally by the player:
///   Play → 6 damage → Exhaust keyword triggers → 浴火 intercepts →
///   Auto-play → 6 damage → Exhaust keyword → recursion guard blocks → exhausted
/// Result: 12(16) damage for 1 energy (2 plays)
///
/// When force-exhausted without being played (e.g. by Ethereal or Havoc):
///   浴火 intercepts → Auto-play → 6(8) damage → exhausted
/// Result: 6(8) damage for 0 energy (1 play)
/// </summary>
public sealed class YuHuoSlashCard : CardModel, IIntrinsicYuHuo
{
    public override string PortraitPath => "res://images/card_portraits/canao/yu_huo_slash.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/yu_huo_slash.png";

    /// <summary>Use CanAoCardPool for visual rendering.</summary>
    public override CardPoolModel Pool => ModelDb.CardPool<CanAoCardPool>();

    /// <summary>浴火 trait — identified by IIntrinsicYuHuo interface.</summary>
    public bool HasIntrinsicYuHuo => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move)
    ];

    public YuHuoSlashCard()
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

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
