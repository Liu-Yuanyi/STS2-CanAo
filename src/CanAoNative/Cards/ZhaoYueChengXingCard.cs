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
/// 照月成星：失去 1 月，获得 2 星，下回合开始时获得 2 星。
/// </summary>
public sealed class ZhaoYueChengXingCard : CardModel
{
    public override string PortraitPath => CardModel.MissingPortraitPath;
    protected override string PortraitPngPath => CardModel.MissingPortraitPath;
    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StarPower>(),
        HoverTipFactory.FromPower<MoonPower>()
    ];

    public ZhaoYueChengXingCard()
        : base(
            canonicalEnergyCost: 1,
            type: CardType.Skill,
            rarity: CardRarity.Uncommon,
            targetType: TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        Player owner = Owner
            ?? throw new InvalidOperationException(
                "ZhaoYue ChengXing requires a card owner.");

        if (owner.Creature.GetPower<MoonPower>() is not
            { Amount: > 0 } moonPower)
        {
            return;
        }

        await PowerCmd.ModifyAmount(
            choiceContext,
            moonPower,
            -1m,
            owner.Creature,
            this);

        await PowerCmd.Apply<StarPower>(
            choiceContext,
            owner.Creature,
            2m,
            owner.Creature,
            this);

        // Deferred 2 stars at next turn start.
        // A tiny power that auto-removes after granting the stars.
        await PowerCmd.Apply<ZhaoYueChengXingPower>(
            choiceContext,
            owner.Creature,
            1m,
            owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        // No upgrade — card is already complete at base.
    }
}
