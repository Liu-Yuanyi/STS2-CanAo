using CanAoNative.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CanAoNative.Powers;

/// <summary>
/// Native temporary Strength bookkeeping for 淬火.
/// Applying this power adds Strength immediately; the base class removes the
/// accumulated Strength at the end of the owner's turn.
/// </summary>
public sealed class CuiHuoTemporaryStrengthPower :
    TemporaryStrengthPower
{
    public override AbstractModel OriginModel =>
        ModelDb.Card<CuiHuoCard>();
}
