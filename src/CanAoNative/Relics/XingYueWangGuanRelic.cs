using CanAoNative.Powers;
using CanAoNative.Rules.StarMoon;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Relics;

/// <summary>
/// 星月王冠：每回合第一次获得凤威（永久或临时）时，获得 1 张星月合击。
/// </summary>
public sealed class XingYueWangGuanRelic : RelicModel
{
    private bool _gainedThisTurn;

    public override RelicRarity Rarity => RelicRarity.Rare;

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (_gainedThisTurn
            || amount <= 0
            || power is not (FengWeiPower or TemporaryFengWeiPower)
            || !ReferenceEquals(power.Owner.Player, Owner))
        {
            return;
        }

        _gainedThisTurn = true;
        Flash();

        await StarMoonService.Generate(
            choiceContext,
            Owner,
            1,
            applier,
            cardSource);
    }

    public override Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (ReferenceEquals(player, Owner))
            _gainedThisTurn = false;

        return Task.CompletedTask;
    }

    public override Task BeforeCombatStart()
    {
        _gainedThisTurn = false;
        return Task.CompletedTask;
    }
}
