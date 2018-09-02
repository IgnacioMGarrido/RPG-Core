
namespace RPG.Core
{
    public interface IDamageable
    {
        void TakeDamage(float damage);
        void TakeHeal(float heal);
        float CalculateHitProbability(float damage, IDamageable target);
    }
}
