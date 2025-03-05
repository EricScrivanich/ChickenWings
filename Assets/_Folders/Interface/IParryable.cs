

public interface IParryable
{
    public float parryWindow { get; }
    void Glow(float duration);

    void Parry();

}
