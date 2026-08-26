using CanAoNative.Pools;
using CanAoNative.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Cards;

/// <summary>
/// 观星问月（v12 重做）：选择：获得 2（3）星，或获得 2（3）月。
/// 选项令牌与自身升级状态同步（选项文本随之显示 2 或 3）。
/// </summary>
public sealed class GuanXingWenYueCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/guan_xing_wen_yue.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/guan_xing_wen_yue.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StarPower>(),
        HoverTipFactory.FromPower<MoonPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2)
    ];

    public GuanXingWenYueCard()
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
                "GuanXing WenYue requires a card owner.");

        if (CombatState is not { } combatState)
            return;

        CardModel guanXing =
            combatState.CreateCard<GuanXingOptionCard>(owner);
        CardModel wenYue =
            combatState.CreateCard<WenYueOptionCard>(owner);

        // 选项令牌与自身升级同步，使选项文本显示正确的 2/3。
        if (IsUpgraded)
        {
            CardCmd.Upgrade(guanXing);
            CardCmd.Upgrade(wenYue);
        }

        CardModel? choice =
            await CardSelectCmd.FromChooseACardScreen(
                choiceContext,
                [guanXing, wenYue],
                owner,
                canSkip: false);

        if (choice is GuanXingOptionCard)
        {
            await PowerCmd.Apply<StarPower>(
                choiceContext,
                owner.Creature,
                DynamicVars.Cards.IntValue,
                owner.Creature,
                this);
        }
        else if (choice is WenYueOptionCard)
        {
            await PowerCmd.Apply<MoonPower>(
                choiceContext,
                owner.Creature,
                DynamicVars.Cards.IntValue,
                owner.Creature,
                this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
