using CanAoNative.Pools;
using CanAoNative.Powers;
using CanAoNative.Rules.StarMoon;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Cards;

/// <summary>
/// 坠星为月：若你有星，失去 1 星，获得 2 月。
/// 若因此生成【星月合击】，抽 1 张牌。消耗。升级后不消耗。
/// </summary>
public sealed class ZhuiXingWeiYueCard : CardModel
{
    public override string PortraitPath => CardModel.MissingPortraitPath;
    protected override string PortraitPngPath => CardModel.MissingPortraitPath;

    public override CardPoolModel Pool =>
        ModelDb.CardPool<CanAoCardPool>();

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StarPower>(),
        HoverTipFactory.FromPower<MoonPower>(),
        HoverTipFactory.FromCard<StarMoonStrike>()
    ];

    public ZhuiXingWeiYueCard()
        : base(
            canonicalEnergyCost: 0,
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
                "ZhuiXing WeiYue requires a card owner.");

        if (owner.Creature.GetPower<StarPower>() is not
            { Amount: > 0 } starPower)
        {
            return;
        }

        int generatedBefore =
            StarMoonService.GetGeneratedThisTurn(owner);

        await PowerCmd.ModifyAmount(
            choiceContext,
            starPower,
            -1m,
            owner.Creature,
            this);

        await PowerCmd.Apply<MoonPower>(
            choiceContext,
            owner.Creature,
            2m,
            owner.Creature,
            this);

        if (StarMoonService.GetGeneratedThisTurn(owner)
            == generatedBefore)
        {
            return;
        }

        await CardPileCmd.Draw(choiceContext, 1m, owner);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}
