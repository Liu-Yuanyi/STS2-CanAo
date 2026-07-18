using CanAoNative.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Relics;

/// <summary>
/// 帝国史册：每回合第一次打出攻击牌时，获得 1 星；
/// 每回合第一次打出技能牌时，获得 1 月。
/// 帝国年表的 Orobas 升级版。
/// </summary>
public sealed class DiGuoShiCeRelic : RelicModel
{
    private bool _attackTriggeredThisTurn;
    private bool _skillTriggeredThisTurn;

    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StarPower>(),
        HoverTipFactory.FromPower<MoonPower>()
    ];

    public override async Task AfterCardPlayedLate(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner)
            return;

        if (cardPlay.Card.Type == CardType.Attack
            && !_attackTriggeredThisTurn)
        {
            _attackTriggeredThisTurn = true;
            Flash();

            await PowerCmd.Apply<StarPower>(
                choiceContext,
                Owner.Creature,
                1m,
                Owner.Creature,
                null);
        }
        else if (cardPlay.Card.Type == CardType.Skill
                 && !_skillTriggeredThisTurn)
        {
            _skillTriggeredThisTurn = true;
            Flash();

            await PowerCmd.Apply<MoonPower>(
                choiceContext,
                Owner.Creature,
                1m,
                Owner.Creature,
                null);
        }
    }

    public override Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (ReferenceEquals(player, Owner))
        {
            _attackTriggeredThisTurn = false;
            _skillTriggeredThisTurn = false;
        }

        return Task.CompletedTask;
    }
}
