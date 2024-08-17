using System.Collections.Generic;
using Godot;

namespace BitBuster.items;

public static class ItemScenes
{
    public static Dictionary<int, PackedScene> GetCompleteNormalItemDictionary()
    {
        Dictionary<int, PackedScene> itemDict = new Dictionary<int, PackedScene>();
        
        itemDict.Add(0, GD.Load<PackedScene>("res://scenes/subscenes/items/normal/0_battery.tscn"));
        itemDict.Add(1, GD.Load<PackedScene>("res://scenes/subscenes/items/normal/1_bomb_bag.tscn"));
        itemDict.Add(2, GD.Load<PackedScene>("res://scenes/subscenes/items/normal/2_brass_gear.tscn"));
        itemDict.Add(3, GD.Load<PackedScene>("res://scenes/subscenes/items/normal/3_controller.tscn"));
        itemDict.Add(4, GD.Load<PackedScene>("res://scenes/subscenes/items/normal/4_crew_rations.tscn"));
        itemDict.Add(5, GD.Load<PackedScene>("res://scenes/subscenes/items/normal/5_cyclops_eye.tscn"));
        itemDict.Add(6, GD.Load<PackedScene>("res://scenes/subscenes/items/normal/6_extra_ammo.tscn"));
        itemDict.Add(7, GD.Load<PackedScene>("res://scenes/subscenes/items/normal/7_fury_of_thunder.tscn"));
        itemDict.Add(8, GD.Load<PackedScene>("res://scenes/subscenes/items/normal/8_future_proofing.tscn"));
        itemDict.Add(9, GD.Load<PackedScene>("res://scenes/subscenes/items/normal/9_fuzzy_die.tscn"));
        itemDict.Add(10, GD.Load<PackedScene>("res://scenes/subscenes/items/normal/10_greedy_pot.tscn"));
        itemDict.Add(11, GD.Load<PackedScene>("res://scenes/subscenes/items/normal/11_heart_container.tscn"));
        itemDict.Add(12, GD.Load<PackedScene>("res://scenes/subscenes/items/normal/12_heros_sword.tscn"));
        itemDict.Add(13, GD.Load<PackedScene>("res://scenes/subscenes/items/normal/13_holy_book.tscn"));
        itemDict.Add(14, GD.Load<PackedScene>("res://scenes/subscenes/items/normal/14_kunai.tscn"));
        itemDict.Add(15, GD.Load<PackedScene>("res://scenes/subscenes/items/normal/15_piercing_rounds.tscn"));
        itemDict.Add(16, GD.Load<PackedScene>("res://scenes/subscenes/items/normal/16_rocket_science.tscn"));
        itemDict.Add(17, GD.Load<PackedScene>("res://scenes/subscenes/items/normal/17_rubber_bullets.tscn"));
        itemDict.Add(18, GD.Load<PackedScene>("res://scenes/subscenes/items/normal/18_shield_generator.tscn"));
        itemDict.Add(19, GD.Load<PackedScene>("res://scenes/subscenes/items/normal/19_spotted_shroom.tscn"));
        itemDict.Add(20, GD.Load<PackedScene>("res://scenes/subscenes/items/normal/20_steel_plating.tscn"));
        itemDict.Add(21, GD.Load<PackedScene>("res://scenes/subscenes/items/normal/21_super_bomb.tscn"));
        itemDict.Add(22, GD.Load<PackedScene>("res://scenes/subscenes/items/normal/22_super_missile.tscn"));
        itemDict.Add(23, GD.Load<PackedScene>("res://scenes/subscenes/items/normal/23_warhead.tscn"));
        itemDict.Add(24, GD.Load<PackedScene>("res://scenes/subscenes/items/normal/24_wave_beam.tscn"));
        itemDict.Add(25, GD.Load<PackedScene>("res://scenes/subscenes/items/normal/25_wide_plush.tscn"));

        return itemDict;
    } 
    
    public static Dictionary<int, PackedScene> GetCompletePickupItemDictionary()
    {
        Dictionary<int, PackedScene> itemDict = new Dictionary<int, PackedScene>();
        
        itemDict.Add(0, GD.Load<PackedScene>("res://scenes/subscenes/items/pickup/0_bomb.tscn"));
        itemDict.Add(1, GD.Load<PackedScene>("res://scenes/subscenes/items/pickup/1_card.tscn"));
        itemDict.Add(2, GD.Load<PackedScene>("res://scenes/subscenes/items/pickup/2_healthpack.tscn"));
        itemDict.Add(3, GD.Load<PackedScene>("res://scenes/subscenes/items/pickup/3_credit.tscn"));

        return itemDict;
    } 
}