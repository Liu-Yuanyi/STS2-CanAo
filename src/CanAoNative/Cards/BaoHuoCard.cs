using CanAoNative.Pools;
using CanAoNative.Rules;
using CanAoNative.Rules.YuHuo;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Cards;

/// <summary>
/// 抱火：浴火。获得 4（6）点格挡。若本牌因浴火而触发，额外获得 3（4）点格挡。
/// </summary>
public sealed class BaoHuoCard : CardModel, IIntrinsicYuHuo
{
    public override string PortraitPath => "res://images/card_portraits/canao/bao_huo.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/bao_huo.png";
    public override bool GainsBlock => true;

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        CanAoHoverTips.YuHuo
    ];

    public bool HasIntrinsicYuHuo => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(4m, ValueProp.Move),
        new CardsVar(3)
    ];

    public BaoHuoCard()
        : base(
            canonicalEnergyCost: 0,
            type: CardType.Skill,
            rarity: CardRarity.Common,
            targetType: TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        Player owner = Owner
            ?? throw new InvalidOperationException(
                "BaoHuo requires a card owner.");

        decimal block = DynamicVars.Block.BaseValue;

        if (YuHuoService.IsTriggeredByYuHuo(this))
            block += DynamicVars.Cards.BaseValue;

        await CreatureCmd.GainBlock(
            owner.Creature,
            block,
            ValueProp.Move,
            cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
