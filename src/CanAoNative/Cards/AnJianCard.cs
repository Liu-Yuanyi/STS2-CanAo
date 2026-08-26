using CanAoNative.Pools;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Cards;

/// <summary>
/// 按剑（v12 新增）：普通技能，1 费。获得 8（11）点格挡。
/// 将 1 张手牌置于抽牌堆顶。（实现参考原生微光 Glimmer）
/// </summary>
public sealed class AnJianCard : CardModel
{
    public override string PortraitPath => "res://images/card_portraits/canao/an_jian.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/an_jian.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8m, ValueProp.Move),
        new CardsVar(1)
    ];

    public AnJianCard()
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
                "AnJian requires a card owner.");

        await CreatureCmd.GainBlock(
            owner.Creature,
            DynamicVars.Block,
            cardPlay);

        CardModel[] selected =
            (await CardSelectCmd.FromHand(
                choiceContext,
                owner,
                new CardSelectorPrefs(
                    SelectionScreenPrompt,
                    DynamicVars.Cards.IntValue),
                null,
                this))
            .ToArray();

        if (selected.Length != 0)
        {
            await CardPileCmd.Add(
                selected,
                PileType.Draw,
                CardPilePosition.Top);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
