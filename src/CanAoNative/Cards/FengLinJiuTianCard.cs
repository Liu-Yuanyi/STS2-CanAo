using CanAoNative.Pools;
using CanAoNative.Rules.FengWei;
using CanAoNative.Rules.YuHuo;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Cards;

/// <summary>
/// 凤临九天：浴火。获得 1（2）点凤威。
/// 手牌中的所有非能力牌本回合内获得浴火。
/// </summary>
public sealed class FengLinJiuTianCard : CardModel, IIntrinsicYuHuo
{
    public override string PortraitPath => CardModel.MissingPortraitPath;
    protected override string PortraitPngPath => CardModel.MissingPortraitPath;

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    public bool HasIntrinsicYuHuo => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];

    public FengLinJiuTianCard()
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
                "FengLin JiuTian requires a card owner.");

        if (CombatState is not { } combatState)
            return;

        await FengWeiService.GainPermanent(
            choiceContext,
            owner,
            DynamicVars.Cards.IntValue,
            this);

        foreach (CardModel card in owner.PlayerCombatState.Hand.Cards)
        {
            if (card.Type == CardType.Power)
                continue;

            YuHuoService.GrantUntilTurnEnd(
                card,
                owner,
                combatState);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
