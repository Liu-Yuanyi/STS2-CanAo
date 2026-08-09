using CanAoNative.Powers;
using CanAoNative.Rules.YuHuo;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using CanAoNative.Pools;
using MegaCrit.Sts2.Core.ValueProps;

namespace CanAoNative.Cards;

/// <summary>
/// 浴火打击：浴火。造成 18（24）点伤害。若本牌因浴火触发，获得 1（2）月。
/// </summary>
public sealed class YuHuoStrikeCard : CardModel, IIntrinsicYuHuo
{
    public override string PortraitPath => "res://images/card_portraits/canao/yu_huo_strike.png";
    protected override string PortraitPngPath => "res://images/card_portraits/canao/yu_huo_strike.png";

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<MoonPower>()
    ];

    public bool HasIntrinsicYuHuo => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(18m, ValueProp.Move),
        new CardsVar(1)
    ];

    public YuHuoStrikeCard()
        : base(
            canonicalEnergyCost: 2,
            type: CardType.Attack,
            rarity: CardRarity.Uncommon,
            targetType: TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        if (!YuHuoService.IsTriggeredByYuHuo(this))
            return;

        Player owner = Owner
            ?? throw new InvalidOperationException(
                "YuHuo Strike requires a card owner.");

        await PowerCmd.Apply<MoonPower>(
            choiceContext,
            owner.Creature,
            DynamicVars.Cards.IntValue,
            owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6m);
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
