using CanAoNative.Pools;
using CanAoNative.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Cards;

/// <summary>
/// 观星：观星问月的选项令牌（选择后获得 2 星）。
/// </summary>
public sealed class GuanXingOptionCard : CardModel
{
    public override string PortraitPath => CardModel.MissingPortraitPath;
    protected override string PortraitPngPath => CardModel.MissingPortraitPath;

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StarPower>()
    ];

    public GuanXingOptionCard()
        : base(
            canonicalEnergyCost: 0,
            type: CardType.Skill,
            rarity: CardRarity.Token,
            targetType: TargetType.Self,
            shouldShowInCardLibrary: false)
    {
    }

    protected override Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }
}
