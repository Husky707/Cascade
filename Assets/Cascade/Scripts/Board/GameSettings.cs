using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGameSettings", menuName = "GameSetup/GameSettings")]
[System.Serializable]
public class GameSettings : ScriptableObject
{

    public bool AllowAbilities => _allowAbilities;
    [Header("Ability Settings")][SerializeField] private bool _allowAbilities = true;
    public int MaxAbilityUses => _maxAbilityUses;
    [SerializeField] private int _maxAbilityUses = 6;
    public int[] MaxAbilityValueUses => _maxAbilityValueUses;
    [SerializeField] private int[] _maxAbilityValueUses = null;
    public AbilityTypeSettings[] Abilities => _abilities;
    [SerializeField] private AbilityTypeSettings[] _abilities = null;


    public bool FirstPlacementOnHome => _firstPlacementOnHome;
    [Header("Placement Settings")][SerializeField] private bool _firstPlacementOnHome = true;
    public bool FirstPlacementBasic => _firstPlacementBasic;
    [SerializeField] private bool _firstPlacementBasic = false;
    public bool MustPlaceEntireAbility => _mustPlaceEntireAbility;
    [SerializeField] private bool _mustPlaceEntireAbility = true;
    public ePlacementZones PlacementRule => _placementRule;
    [SerializeField] private ePlacementZones _placementRule = ePlacementZones.Diagonal;


    public int NumPlayers => _numPlayers;
    [Header("Player Settings")][SerializeField] private int _numPlayers = 2;
    public ColorAssignments[] ColorSettings => _colorSettings;
    [SerializeField] private ColorAssignments[] _colorSettings = null;
    public eColors[] TurnOrder => _turnOrder;
    [SerializeField] private eColors[] _turnOrder = null;


    public uint StartingDice => _startingDice;
    [Header("Other Settings")] [SerializeField] private uint _startingDice = 20;
    public uint MaxRounds => _maxRounds;
    [SerializeField] private uint _maxRounds = 40;

}

    [System.Serializable]
    public class AbilityTypeSettings
    {
        // -1 implies unlimited
        public eDicePlacers[] Type => _ability;
        [SerializeField] private eDicePlacers[] _ability = null;
        public uint[] AvailableValues => _availabeAbilityValues;
        [SerializeField] private uint[] _availabeAbilityValues = null;
        //-1 infers unlimited uses
        public int[] MaxValueUses => _maxAbilityValueUses;
        [SerializeField] private int[] _maxAbilityValueUses = null;
        public int[] MaxUses => _maxAbilityTypeUses;
        [SerializeField] private int[] _maxAbilityTypeUses = null;
    }


    [System.Serializable]
    public class ColorAssignments
    {
        public eColors Color => _color;
        [SerializeField] private eColors _color = eColors.Noone;
        public int PlayerAssignment => _playerAssignment;
        [SerializeField] private int _playerAssignment = 0;
        public eTileType ColorHome => _colorHome;
        [SerializeField] private eTileType _colorHome = eTileType.Basic;

    }