using CanAoNative.Powers;
using CanAoNative.Rules.FengWei;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using CanAoNative.Pools;

namespace CanAoNative.Cards;

/// <summary>
/// 王权：消耗手牌中所有诏令。每消耗 1 张，获得 1 费并抽 2 张牌。
/// </summary>
public sealed class WangQuanCard : CardModel
{
    public override string PortraitPath => CardModel.MissingPortraitPath;
    protected override string PortraitPngPath => CardModel.MissingPortraitPath;

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<EdictCard>()
    ];

    public WangQuanCard()
        : base(
            canonicalEnergyCost: 2,
            type: CardType.Skill,
            rarity: CardRarity.Rare,
            targetType: TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        Player owner = Owner
            ?? throw new InvalidOperationException(
                "WangQuan requires a card owner.");

        List<EdictCard> edicts = owner.PlayerCombatState.Hand.Cards
            .OfType<EdictCard>()
            .ToList();

        foreach (EdictCard edict in edicts)
        {
            await CardCmd.Exhaust(choiceContext, edict);

            await PlayerCmd.GainEnergy(1, owner);
            await CardPileCmd.Draw(choiceContext, 2m, owner);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
