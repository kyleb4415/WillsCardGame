using Godot;
using System;

public class StatusEffect
{
    public Effect Effect { get; set; }
    public int Duration { get; set; }
    public int Damage { get; set; }
    public int Health { get; set; }
    public bool Boost { get; set; }
    public StatusEffect(Effect effect, int duration, int damage, bool boost)
    {
        this.Effect = effect;
        this.Duration = duration;
        this.Damage = damage;
        this.Boost = boost;
    }
    public StatusEffect(Effect effect, int duration, int damage, int health, bool boost)
    {
        this.Effect = effect;
        this.Duration = duration;
        this.Damage = damage;
        this.Health = health;
        this.Boost = boost;
    }

}

public enum Effect 
{
    Poison,
    Fire,
    Infection
}

