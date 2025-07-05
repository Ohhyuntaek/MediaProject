
using System.Collections.Generic;

public class DiabunnySkill : ISkill<Ally>
{
    public void Activate(Ally ally)
    {
        List<IDamageable> _detected = ally.DetectSkillTargets();
        foreach (var enemy in _detected)
        {
            enemy.TakeDamage(20);
        }
    }
}
