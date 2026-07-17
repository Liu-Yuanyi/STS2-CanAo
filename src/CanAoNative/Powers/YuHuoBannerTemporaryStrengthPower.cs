using CanAoNative.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace CanAoNative.Powers;

/// <summary>
/// Native temporary Strength bookkeeping for 浴火军旗.
/// Applying this power adds Strength immediately; the base class removes the
/// accumulated Strength at the end of the owner's turn.
/// </summary>
public sealed class YuHuoBannerTemporaryStrengthPower :
    TemporaryStrengthPower
{
    public override AbstractModel OriginModel =>
        ModelDb.Card<YuHuoBannerCard>();
}
