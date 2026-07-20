using CanAoNative.Pools;
using CanAoNative.Powers;
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
/// 星辉护阵：获得 7（11）点格挡。获得 1 星。
/// </summary>
public sealed class XingHuiHuZhenCard : CardModel
{
    public override string PortraitPath => CardModel.MissingPortraitPath;
    protected override string PortraitPngPath => CardModel.MissingPortraitPath;
    public override bool GainsBlock => true;

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StarPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(7m, ValueProp.Move)
    ];

    public XingHuiHuZhenCard()
        : base(
            canonicalEnergyCost: 1,
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
                "XingHui HuZhen requires a card owner.");

        await CreatureCmd.GainBlock(
            owner.Creature,
            DynamicVars.Block,
            cardPlay);

        await PowerCmd.Apply<StarPower>(
            choiceContext,
            owner.Creature,
            1m,
            owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
    }
}
