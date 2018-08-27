
namespace RPG.Core
{
    public interface IDamageable
    {
        //TODO: Separate TakeDamage and hit Behaviour
        void TakeDamage(float damage);
        float CalculateHitProbability(float damage, IDamageable target);
    }
}
