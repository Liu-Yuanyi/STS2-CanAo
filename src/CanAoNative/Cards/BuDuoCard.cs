using CanAoNative.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using CanAoNative.Pools;

namespace CanAoNative.Cards;

/// <summary>
/// 不堕：你的星月合击不受小于 0 的凤威影响。升级后获得固有。
/// </summary>
public sealed class BuDuoCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/bu_duo.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/bu_duo.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<StarMoonStrike>(),
        HoverTipFactory.FromPower<FengWeiPower>()
    ];

    public BuDuoCard()
        : base(
            canonicalEnergyCost: 1,
            type: CardType.Power,
            rarity: CardRarity.Uncommon,
            targetType: TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await PowerCmd.Apply<BuDuoPower>(
            choiceContext,
            Owner.Creature,
            1m,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}
