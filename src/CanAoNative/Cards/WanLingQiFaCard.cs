using CanAoNative.Pools;
using CanAoNative.Rules.Edict;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Cards;

/// <summary>
/// 万令齐发：将 X（X+1）张【诏令】加入你的手牌。消耗。
/// </summary>
public sealed class WanLingQiFaCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/wan_ling_qi_fa.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/wan_ling_qi_fa.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override bool HasEnergyCostX => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<EdictCard>()
    ];

    public WanLingQiFaCard()
        : base(
            canonicalEnergyCost: 0,
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
                "WanLing QiFa requires a card owner.");

        int count = ResolveEnergyXValue();

        if (IsUpgraded)
            count++;

        await EdictService.Generate(
            choiceContext,
            owner,
            count);
    }
}
