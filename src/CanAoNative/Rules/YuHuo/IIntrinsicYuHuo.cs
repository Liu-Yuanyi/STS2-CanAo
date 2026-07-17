using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Rules.YuHuo;

/// <summary>
/// Interface for cards that intrinsically have the 浴火 trait.
/// 浴火: Before this card is exhausted, it is auto-played for free.
///
/// This interface ONLY marks compile-time 浴火. For runtime (temporary) 浴火
/// granted by effects like 牺牲准备, use <see cref="YuHuoCombatState"/>.
/// Always query 浴火 status via <see cref="YuHuoService.HasYuHuo"/>.
/// </summary>
public interface IIntrinsicYuHuo
{
    bool HasIntrinsicYuHuo { get; }
}
