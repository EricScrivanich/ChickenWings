
// [CreateAssetMenuAttribute(fileName = "JetPackPigSetup", menuName = "IndividualSetups")]
[System.Serializable]
public class JetPackPigSO : EnemyData
{
    public JetPackPigData[] data;
    public override void InitializeEnemy(EnemyPoolManager manager)
    {
    
        for (int i = 0; i < data.Length; i++)
        {
            manager.GetJetPackPig(data[i].position, data[i].scale, data[i].speed);
        }

    }

   

    // Start is called before the first frame update

}
