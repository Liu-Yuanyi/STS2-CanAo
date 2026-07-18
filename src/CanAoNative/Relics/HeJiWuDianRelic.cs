using CanAoNative.Rules.StarMoon;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Relics;

/// <summary>
/// 合击武典：每打出 4 张星月合击，下一张星月合击的效果翻倍。
/// The armed strike's damage and block base values are doubled at
/// generation time, before any FengWei or other modifiers apply.
/// </summary>
public sealed class HeJiWuDianRelic :
    RelicModel,
    IAfterStarMoonPlayed,
    IAfterStarMoonGenerated
{
    private int _playedCount;
    private bool _nextStrikeDoubled;

    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(4)
    ];

    public Task AfterStarMoonPlayed(
        PlayerChoiceContext choiceContext,
        StarMoonPlayedContext context)
    {
        if (!ReferenceEquals(context.Player, Owner))
            return Task.CompletedTask;

        _playedCount++;

        if (_playedCount >= DynamicVars.Cards.IntValue)
        {
            _playedCount = 0;
            _nextStrikeDoubled = true;
        }

        return Task.CompletedTask;
    }

    public Task AfterStarMoonGenerated(
        PlayerChoiceContext choiceContext,
        StarMoonGenerationContext context)
    {
        if (!_nextStrikeDoubled
            || !ReferenceEquals(context.Player, Owner))
        {
            return Task.CompletedTask;
        }

        _nextStrikeDoubled = false;
        Flash();

        context.Card.DynamicVars.Damage.BaseValue *= 2m;
        context.Card.DynamicVars.Block.BaseValue *= 2m;

        return Task.CompletedTask;
    }

    public override Task BeforeCombatStart()
    {
        _playedCount = 0;
        _nextStrikeDoubled = false;
        return Task.CompletedTask;
    }
}
