using CanAoNative.Powers;
using CanAoNative.Rules.FengWei;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Potions;

/// <summary>
/// 凤威酒：获得 2 点凤威，在本回合获得 2 点凤威。
/// </summary>
public sealed class FengWeiJiuPotion : PotionModel
{
    public override PotionRarity Rarity => PotionRarity.Uncommon;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.AnyPlayer;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<FengWeiPower>(2m),
        new CardsVar(2)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FengWeiPower>()
    ];

    protected override async Task OnUse(
        PlayerChoiceContext choiceContext,
        Creature? target)
    {
        PotionModel.AssertValidForTargetedPotion(target);

        Player player = target.Player
            ?? throw new InvalidOperationException(
                "FengWei Wine requires a player target.");

        await FengWeiService.GainPermanent(
            choiceContext,
            player,
            DynamicVars["FengWeiPower"].BaseValue,
            null);

        await FengWeiService.ModifyTemporary(
            choiceContext,
            player,
            DynamicVars.Cards.BaseValue,
            null);
    }
}
