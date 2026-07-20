using CanAoNative.Pools;
using CanAoNative.Powers;
using CanAoNative.Rules.FengWei;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Cards;

/// <summary>
/// 宴请八方：（多人游戏专属）虚无。所有玩家恢复等同于你的凤威点数的血量。
/// 消耗。升级后不虚无。
/// </summary>
public sealed class YanQingBaFangCard : CardModel
{
    public override string PortraitPath => CardModel.MissingPortraitPath;
    protected override string PortraitPngPath => CardModel.MissingPortraitPath;

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    public override CardMultiplayerConstraint MultiplayerConstraint =>
        CardMultiplayerConstraint.MultiplayerOnly;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Ethereal,
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FengWeiPower>()
    ];

    public YanQingBaFangCard()
        : base(
            canonicalEnergyCost: 3,
            type: CardType.Skill,
            rarity: CardRarity.Rare,
            targetType: TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        Player owner = Owner
            ?? throw new InvalidOperationException(
                "YanQing BaFang requires a card owner.");

        if (CombatState is not { } combatState)
            return;

        decimal heal = Math.Max(
            0m,
            FengWeiService.GetEffectiveAmount(owner));

        if (heal <= 0m)
            return;

        foreach (Player player in combatState.Players)
        {
            await CreatureCmd.Heal(
                player.Creature,
                heal);
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Ethereal);
    }
}
