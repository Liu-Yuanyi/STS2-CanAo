using CanAoNative.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Relics;

/// <summary>
/// 帝国年表：每场战斗第一次打出攻击牌时，获得 2 星；
/// 每场战斗第一次打出技能牌时，获得 2 月。
/// </summary>
public sealed class DiGuoNianBiaoRelic : RelicModel
{
    private bool _attackTriggeredThisCombat;
    private bool _skillTriggeredThisCombat;

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
            && !_attackTriggeredThisCombat)
        {
            _attackTriggeredThisCombat = true;
            Flash();

            await PowerCmd.Apply<StarPower>(
                choiceContext,
                Owner.Creature,
                2m,
                Owner.Creature,
                null);
        }
        else if (cardPlay.Card.Type == CardType.Skill
                 && !_skillTriggeredThisCombat)
        {
            _skillTriggeredThisCombat = true;
            Flash();

            await PowerCmd.Apply<MoonPower>(
                choiceContext,
                Owner.Creature,
                2m,
                Owner.Creature,
                null);
        }
    }

    public override Task BeforeCombatStart()
    {
        _attackTriggeredThisCombat = false;
        _skillTriggeredThisCombat = false;
        return Task.CompletedTask;
    }
}
