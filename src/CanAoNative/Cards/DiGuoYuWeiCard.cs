using CanAoNative.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using CanAoNative.Pools;

namespace CanAoNative.Cards;

/// <summary>
/// 帝国余威：你每回合第二次打出诏令时，获得 1 点凤威。升级后获得固有。
/// </summary>
public sealed class DiGuoYuWeiCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/di_guo_yu_wei.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/di_guo_yu_wei.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<EdictCard>(),
        HoverTipFactory.FromPower<FengWeiPower>()
    ];

    public DiGuoYuWeiCard()
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
        await PowerCmd.Apply<DiGuoYuWeiPower>(
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
