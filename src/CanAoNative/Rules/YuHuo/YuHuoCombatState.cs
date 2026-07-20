using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Rules.YuHuo;

/// <summary>
/// State owned by one combat. Card keys use reference identity because two
/// copies of the same card model must remain distinguishable.
/// </summary>
public sealed class YuHuoCombatState
{
    private readonly Dictionary<CardModel, TemporaryYuHuoGrant> _temporaryGrants =
        new(ReferenceEqualityComparer.Instance);

    private readonly Dictionary<CardModel, Player> _permanentGrants =
        new(ReferenceEqualityComparer.Instance);

    private readonly HashSet<CardModel> _resolving =
        new(ReferenceEqualityComparer.Instance);

    private readonly Dictionary<CardModel, YuHuoExecutionContext> _activeContexts =
        new(ReferenceEqualityComparer.Instance);

    public void GrantUntilTurnEnd(CardModel card, Player owner, int currentTurn)
    {
        ArgumentNullException.ThrowIfNull(card);
        ArgumentNullException.ThrowIfNull(owner);

        _temporaryGrants[card] =
            new TemporaryYuHuoGrant(
                owner,
                GrantedTurn: currentTurn,
                ExpiresAfterTurn: currentTurn);
    }

    /// <summary>
    /// Grants 浴火 without a turn expiry. Combat-scoped like all YuHuo state:
    /// the grant lives until the card leaves the combat or the combat ends.
    /// </summary>
    public void GrantPermanent(CardModel card, Player owner)
    {
        ArgumentNullException.ThrowIfNull(card);
        ArgumentNullException.ThrowIfNull(owner);

        _permanentGrants[card] = owner;
    }

    public bool HasPermanentYuHuo(CardModel card, Player owner)
    {
        return _permanentGrants.TryGetValue(card, out Player? grantOwner)
               && ReferenceEquals(grantOwner, owner);
    }

    public bool HasTemporaryYuHuo(
        CardModel card,
        Player owner,
        int currentTurn)
    {
        if (!_temporaryGrants.TryGetValue(card, out TemporaryYuHuoGrant? grant))
            return false;

        if (!ReferenceEquals(grant.Owner, owner)
            || currentTurn > grant.ExpiresAfterTurn)
        {
            _temporaryGrants.Remove(card);
            return false;
        }

        return true;
    }

    public void RemoveExpiredForPlayers(IEnumerable<Player> players)
    {
        HashSet<Player> endingPlayers =
            new(players, ReferenceEqualityComparer.Instance);

        if (endingPlayers.Count == 0 || _temporaryGrants.Count == 0)
            return;

        List<CardModel> expired = [];

        foreach ((CardModel card, TemporaryYuHuoGrant grant) in _temporaryGrants)
        {
            if (!endingPlayers.Contains(grant.Owner))
                continue;

            Player owner = grant.Owner;
            int currentTurn = owner.PlayerCombatState.TurnNumber;
            if (grant.ExpiresAfterTurn <= currentTurn)
                expired.Add(card);
        }

        foreach (CardModel card in expired)
            _temporaryGrants.Remove(card);
    }

    public bool TryBeginResolution(CardModel card) =>
        _resolving.Add(card);

    public bool IsResolving(CardModel card) =>
        _resolving.Contains(card);

    public void EndResolution(CardModel card)
    {
        _activeContexts.Remove(card);
        _resolving.Remove(card);
    }

    public void BeginTrigger(YuHuoExecutionContext context)
    {
        _activeContexts[context.Card] = context;
    }

    public void EndTrigger(CardModel card)
    {
        _activeContexts.Remove(card);
    }

    public bool TryGetExecutionContext(
        CardModel card,
        out YuHuoExecutionContext? context)
    {
        return _activeContexts.TryGetValue(card, out context);
    }

    private sealed record TemporaryYuHuoGrant(
        Player Owner,
        int GrantedTurn,
        int ExpiresAfterTurn);
}
