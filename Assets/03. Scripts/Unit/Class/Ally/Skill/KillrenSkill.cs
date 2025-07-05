
using System.Collections.Generic;

public class KillrenSkill : ISkill<Ally>
{
    public void Activate(Ally ally)
    {
        List<IDamageable> _detected = ally.DetectTargets();
        foreach (var e in _detected)
        {
            if (e is Enemy enemy)
            {
                enemy.ApplyStun(5f);
            }
        }
    }
}
