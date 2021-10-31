using System;
using UnityEngine;
//Static data.
//todo - replace itemType enum with a flat int that can be looked up in a table to get the type name
public enum ItemType { Weapon, UtilityWeapon, Shield, Power, Shocks, Tires, Consumable, Armor, None, Ammo, Car, UtilityMovement};
[CreateAssetMenu(fileName = "ItemDefinition", menuName = "Scriptables/EquippableItems/ItemDefinition", order = 2)]
[System.Serializable]
public class ItemDefinition : ScriptableObject //TODO: Should probably replace with something a bit more dynamic to avoid having unused fields on ItemDefinitions.
{
    [HideInInspector]public ItemType itemType;
    [Header("Common Item Properties")]
    public uint DefinitionID;
    public bool isAvailableToBuy;
    public GameObject ItemPrefab;
    public Sprite Icon;
    public Sprite typeIcon;
    public string Name;
    public int MaxStackSize;
    public int itemMass;
    public int EquipGroup;
    public string shortDescription;
    public string description;
    public string Manufacturer;
    public int cost;
    public Color accentColor;
    public bool canPurchaseMultiple;
    public Type itemTypeData { get { return this.GetType();} }
    //public UtilityEvaluator UtilityEvaluator;
    //[SerializeReference] public UseCondition[] UseConditions;

    public override string ToString() { return Name; }
}
