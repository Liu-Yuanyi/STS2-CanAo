using CanAoNative.Rules.Edict;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using CanAoNative.Pools;

namespace CanAoNative.Cards;

/// <summary>
/// 密诏：将 1 张诏令加入弃牌堆。升级后加入的是诏令+。不消耗。
/// </summary>
public sealed class MiZhaoCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/mi_zhao.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/mi_zhao.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<EdictCard>(IsUpgraded)
    ];

    public MiZhaoCard()
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
        await EdictService.Generate(
            choiceContext,
            Owner,
            1,
            PileType.Discard,
            IsUpgraded);
    }
}
