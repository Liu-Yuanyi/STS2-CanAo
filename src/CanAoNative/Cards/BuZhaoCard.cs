using CanAoNative.Pools;
using CanAoNative.Rules.Edict;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Cards;

/// <summary>
/// 布诏：所有其他玩家获得一张【诏令】。多人游戏专属。
/// </summary>
public sealed class BuZhaoCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/bu_zhao.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/bu_zhao.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    public override CardMultiplayerConstraint MultiplayerConstraint =>
        CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<EdictCard>()
    ];

    public BuZhaoCard()
        : base(
            canonicalEnergyCost: 1,
            type: CardType.Skill,
            rarity: CardRarity.Uncommon,
            targetType: TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        Player owner = Owner
            ?? throw new InvalidOperationException(
                "BuZhao requires a card owner.");

        if (CombatState is not { } combatState)
            return;

        foreach (Player player in combatState.Players)
        {
            if (ReferenceEquals(player, owner))
                continue;

            await EdictService.Generate(
                choiceContext,
                player,
                1);
        }
    }
}
