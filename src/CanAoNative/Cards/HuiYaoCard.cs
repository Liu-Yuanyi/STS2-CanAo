using CanAoNative.Pools;
using CanAoNative.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Cards;

/// <summary>
/// 辉耀：失去所有格挡，每失去 2 格挡，获得 1 星。消耗。
/// </summary>
public sealed class HuiYaoCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/hui_yao.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/hui_yao.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StarPower>()
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    public HuiYaoCard()
        : base(
            canonicalEnergyCost: 1,
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
                "HuiYao requires a card owner.");

        decimal block = owner.Creature.Block;

        if (block <= 0m)
            return;

        // 设计是"失去所有格挡"：不足 2 点的零头也必须失去。
        owner.Creature.LoseBlockInternal(block);

        int stars = (int)(block / 2m);

        if (stars <= 0)
            return;

        await PowerCmd.Apply<StarPower>(
            choiceContext,
            owner.Creature,
            stars,
            owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
