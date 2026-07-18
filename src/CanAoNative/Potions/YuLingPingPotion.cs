using CanAoNative.Cards;
using CanAoNative.Rules.Edict;
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
/// 御令瓶：将 2 张诏令+加入手牌。
/// </summary>
public sealed class YuLingPingPotion : PotionModel
{
    public override PotionRarity Rarity => PotionRarity.Rare;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.AnyPlayer;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<EdictCard>(upgrade: true)
    ];

    protected override async Task OnUse(
        PlayerChoiceContext choiceContext,
        Creature? target)
    {
        PotionModel.AssertValidForTargetedPotion(target);

        Player player = target.Player
            ?? throw new InvalidOperationException(
                "Imperial Edict Vial requires a player target.");

        await EdictService.Generate(
            choiceContext,
            player,
            DynamicVars.Cards.IntValue,
            PileType.Hand,
            upgraded: true);
    }
}
