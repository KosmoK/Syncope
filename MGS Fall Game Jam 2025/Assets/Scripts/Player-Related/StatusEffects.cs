using UnityEngine;

public class StatusEffects
{ // Parent class
    private float duration;
    public StatusEffects(float duration)
    {
        this.duration = duration;
    }

    public float timeLeft()
    {
        return duration;
    }
}
public class DamageOverTime : StatusEffects
{
    private int damagePerTick;
    private float timePerTick;

    public DamageOverTime(float duration, int damagePerTick, float timePerTick) : base(duration)
    {
        this.damagePerTick = damagePerTick;
        this.timePerTick = timePerTick;
    }
}