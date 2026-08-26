using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Powers;

/// <summary>
/// 万邦来朝（v12 重做）：你的回合开始时，将 Amount 张其他角色的随机
/// 能力牌加入手牌，它们获得虚无。叠加语义：层数 = 每回合加入张数
/// （Counter，R11 叠加统一）。卡池与随机源同原生创造性 AI / 虚空之唤。
/// </summary>
public sealed class WanBangLaiChaoPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal)
    ];

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player != Owner.Player || Amount <= 0)
            return;

        // 其他角色 = 全部角色卡池中排除自己。
        var candidates = ModelDb.AllCharacters
            .Where(c => c != player.Character)
            .SelectMany(c => c.CardPool.GetUnlockedCards(
                player.UnlockState,
                player.RunState.CardMultiplayerConstraint))
            .Where(c => c.Type == CardType.Power)
            .ToList();

        if (candidates.Count == 0)
            return;

        var rng = player.RunState.Rng.CombatCardGeneration;

        Flash();

        // 逐张发牌（同原生创造性 AI）：每张各走一次完整入牌视觉，
        // 批量一次加会在抽牌布局前被吞掉"飞入手牌"动画。
        for (int i = 0; i < Amount; i++)
        {
            CardModel? card = CardFactory
                .GetDistinctForCombat(player, candidates, 1, rng)
                .FirstOrDefault();

            if (card == null)
                break;

            CardCmd.ApplyKeyword(card, CardKeyword.Ethereal);

            await CardPileCmd.AddGeneratedCardToCombat(
                card,
                PileType.Hand,
                player);
        }
    }
}
