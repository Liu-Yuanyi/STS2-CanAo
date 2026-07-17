using CanAoNative.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Cards;

/// <summary>
/// A skill card that applies CanAoProbePower (3 stacks).
/// Used to validate the power lifecycle: apply, stack, display, turn-end
/// tick down, and save/reload.
/// </summary>
public sealed class CanAoProbePowerCard : CardModel
{
    public override string PortraitPath => CardModel.MissingPortraitPath;

    protected override string PortraitPngPath => CardModel.MissingPortraitPath;

    public CanAoProbePowerCard()
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
        await PowerCmd.Apply<CanAoProbePower>(
            choiceContext,
            Owner.Creature,
            3m,
            Owner.Creature,
            this);
    }
}
