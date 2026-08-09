using CanAoNative.Cards;
using CanAoNative.Pools;
using CanAoNative.Relics;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;

namespace CanAoNative.Characters;

/// <summary>
/// 残傲。视觉资源当前复用铁甲战士作为占位，后续替换为专属素材。
/// </summary>
public sealed class CanAo : CharacterModel
{
    public override CharacterGender Gender => CharacterGender.Masculine;
    protected override CharacterModel? UnlocksAfterRunAs => null;
    public override Color NameColor => new("E8A33D");
    public override int StartingHp => 72;
    public override int StartingGold => 99;

    public override CardPoolModel CardPool =>
        ModelDb.CardPool<CanAoCardPool>();
    public override RelicPoolModel RelicPool =>
        ModelDb.RelicPool<CanAoRelicPool>();
    public override PotionPoolModel PotionPool =>
        ModelDb.PotionPool<CanAoPotionPool>();

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<CanAoStrikeCard>(),
        ModelDb.Card<CanAoStrikeCard>(),
        ModelDb.Card<CanAoStrikeCard>(),
        ModelDb.Card<CanAoDefendCard>(),
        ModelDb.Card<CanAoDefendCard>(),
        ModelDb.Card<CanAoDefendCard>(),
        ModelDb.Card<FengYuCanHuoCard>(),
        ModelDb.Card<JiHuoCard>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<DiGuoNianBiaoRelic>()
    ];

    public override float AttackAnimDelay => 0.15f;
    public override float CastAnimDelay => 0.25f;

    // AI-generated placeholder visuals — to be iterated on.
    protected override string IconPath =>
        "res://scenes/ui/character_icons/can_ao_icon.tscn";
    protected override string CharacterSelectIconPath =>
        "res://images/characters/char_select_can_ao.png";
    protected override string CharacterSelectLockedIconPath =>
        "res://images/characters/char_select_locked_can_ao.png";
    protected override string MapMarkerPath =>
        "res://images/ui/top_panel/character_icon_can_ao.png";
    public override string CharacterSelectSfx =>
        "event:/sfx/characters/ironclad/ironclad_select";
    public override string CharacterTransitionSfx =>
        "event:/sfx/ui/wipe_ironclad";

    public override List<string> GetArchitectAttackVfx() =>
    [
        "vfx/vfx_attack_slash"
    ];
}
