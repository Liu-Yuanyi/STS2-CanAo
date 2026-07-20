using CanAoNative.Pools;
using CanAoNative.Rules.StarMoon;
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
/// 星月终式：造成 8（11）点伤害。本局游戏每打出过 1 张【星月合击】，重复一次。
/// Uses GetPlayedThisCombat (per-combat counter, analogous to PullFromBelow).
/// </summary>
public sealed class XingYueZhongShiCard : CardModel
{
    public override string PortraitPath => CardModel.MissingPortraitPath;
    protected override string PortraitPngPath => CardModel.MissingPortraitPath;

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<StarMoonStrike>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move)
    ];

    public XingYueZhongShiCard()
        : base(
            canonicalEnergyCost: 2,
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
                "XingYue ZhongShi requires a card owner.");

        // Per-combat counter: each Star-Moon Strike played this combat
        // adds one extra hit. The first hit is always free.
        // Modeled after PullFromBelow (亡魂牵引) which counts Ethereal
        // plays via CombatManager.History.
        int hitCount =
            1 + StarMoonService.GetPlayedThisCombat(owner);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(hitCount)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
