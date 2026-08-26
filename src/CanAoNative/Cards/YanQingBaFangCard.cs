using CanAoNative.Pools;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Cards;

/// <summary>
/// 宴请八方（v12 重做）：稀有能力牌，2 费。（多人游戏专属）
/// 所有玩家恢复 3（5）点生命，抽 1 张牌。
/// </summary>
public sealed class YanQingBaFangCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/yan_qing_ba_fang.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/yan_qing_ba_fang.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    public override CardMultiplayerConstraint MultiplayerConstraint =>
        CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Heal", 3m),
        new CardsVar(1)
    ];

    public YanQingBaFangCard()
        : base(
            canonicalEnergyCost: 2,
            type: CardType.Power,
            rarity: CardRarity.Rare,
            targetType: TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (CombatState is not { } combatState)
            return;

        foreach (Player player in combatState.Players)
        {
            await CreatureCmd.Heal(
                player.Creature,
                DynamicVars["Heal"].BaseValue);

            await CardPileCmd.Draw(
                choiceContext,
                DynamicVars.Cards.BaseValue,
                player);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Heal"].UpgradeValueBy(2m);
    }
}
