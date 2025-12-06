
public interface IPositionerObject 
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetData(int type, float f, float[] intervals = null, float[] times = null);

    public void SetPercent(float percent);
    public void DoUpdateTransform();

    public void DestroyObject();
}
