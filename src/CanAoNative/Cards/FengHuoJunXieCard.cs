using CanAoNative.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using CanAoNative.Pools;

namespace CanAoNative.Cards;

/// <summary>
/// 凤火军械：每回合开始时，获得一张火刃（升级版获得火刃+）。
/// 未升级与升级版是两个独立 Power，分别叠层、分别产牌。
/// </summary>
public sealed class FengHuoJunXieCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/feng_huo_jun_xie.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/feng_huo_jun_xie.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<HuoRenCard>(IsUpgraded)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];

    public FengHuoJunXieCard()
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
        // 未升级与升级版是两个独立 Power，分别叠层、分别产牌。
        if (IsUpgraded)
        {
            await PowerCmd.Apply<FengHuoJunXieUpgradedPower>(
                choiceContext,
                Owner.Creature,
                DynamicVars.Cards.BaseValue,
                Owner.Creature,
                this);
        }
        else
        {
            await PowerCmd.Apply<FengHuoJunXiePower>(
                choiceContext,
                Owner.Creature,
                DynamicVars.Cards.BaseValue,
                Owner.Creature,
                this);
        }
    }
}
