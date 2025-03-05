public interface IDamageable
{
    public bool CanPerfectScythe { get; }
    void Damage(int damageAmount = 1, int type = -1, int id = -1);

}

