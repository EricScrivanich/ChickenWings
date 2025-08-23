using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Levels/LevelChallenges")]
public class LevelChallenges : ScriptableObject
{
    [SerializeField] private LevelManagerID lvlID;

    public Vector3Int LevelWorldAndNumber { get; private set; }
    public int LevelDifficulty { get; private set; } // Default to 0, can be set externally
    public enum ChallengeTypes
    {
        None,
        CompleteLevel,
        CheckLives,
        CheckPigs,
        CheckCertainNonAllowedInputs,
        CheckAmmo,
        CheckPigsByAmmoTypeWithPigType,
        CheckPigsByBulletIdWithPigType,
        CheckCompletedRings,
        CheckTotalBarns,
        CheckSpecificBarn,
        CheckEachBarn
    }


    [SerializeField] private ChallengeTypes[] challenges;
    public string[] challengeTexts;
    public string[] challengeDifficulties;

    public int NumberOfChallenges => challenges.Length;

    [SerializeField] private int LifeTarget;
    [SerializeField] private int EggAmmoTarget;
    [SerializeField] private int ShotgunAmmoTarget;
    [SerializeField] private int[] TrackedPigs;
    [SerializeField] private int[] TrackedPigAmounts;

    [SerializeField] private Vector3Int[] TrackedPigsByBulletType_PigType_Amount;
    [SerializeField] private int[] TrackedPigAmountsByBullet;
    [SerializeField] private int[] TrackedPigIDWithPigType;
    [SerializeField] private int[] TrackedPigAmountsByID;

    [SerializeField] private List<int> CertainNonAllowedInputs;
    [SerializeField] private Vector2Int ringsTypeAndAmount;
    private bool trackPigs;
    private bool trackInputs;
    private bool trackRings;

    [SerializeField] private int totalBarnEggAmount;
    [SerializeField] private int specificBarnEggAmount;
    [SerializeField] private Vector2Int eachBarnAmount;




    public List<Vector3Int> KilledPigs { get; private set; }
    public List<int> PlayerInputs { get; private set; }
    public List<int> CompletedRings { get; private set; }

    public List<int> BarnsHitWithEgg { get; private set; }
    public int Lives { get; private set; }
    public short[] Ammos { get; private set; }
    public ushort CurrentCheckPoint { get; private set; }
    public bool UsedCheckpoint { get; private set; }


    public void ResetData(Vector3Int lvlNum, int difficulty, short[] startingAmmos, int startingLives)
    {
        Debug.Log("Resetting LevelChallenges data for level: " + lvlNum + " with difficulty: " + difficulty);
        LevelWorldAndNumber = lvlNum;
        LevelDifficulty = difficulty;




        Lives = startingLives;
        Ammos = new short[startingAmmos.Length]; // Assuming 2 types of ammo: Egg and Shotgun
        for (int i = 0; i < startingAmmos.Length; i++)
        {
            Ammos[i] = startingAmmos[i]; // Initialize all ammo types to 0
        }

        // Assuming 2 types of ammo: Egg and Shotgun
        CurrentCheckPoint = 0;
        UsedCheckpoint = false;
        SetTrackingBools();


    }

    private void SetTrackingBools()
    {
        trackPigs = false;
        trackInputs = false;
        trackRings = false;

        for (int i = 0; i < challenges.Length; i++)
        {
            switch (challenges[i])
            {
                case ChallengeTypes.CheckPigs:
                case ChallengeTypes.CheckPigsByAmmoTypeWithPigType:
                case ChallengeTypes.CheckPigsByBulletIdWithPigType:
                    KilledPigs = new List<Vector3Int>();
                    trackPigs = true;
                    break;
                case ChallengeTypes.CheckCertainNonAllowedInputs:
                    trackInputs = true;
                    PlayerInputs = new List<int>();
                    break;
                case ChallengeTypes.CheckCompletedRings:
                    CompletedRings = new List<int>();
                    trackRings = true;
                    break;


                case ChallengeTypes.CheckTotalBarns:
                case ChallengeTypes.CheckSpecificBarn:
                case ChallengeTypes.CheckEachBarn:

                    BarnsHitWithEgg = new List<int>();

                    break;



                default:
                    break;
            }
        }
    }
    public void SetCheckPoint(ushort checkPoint)
    {
        CurrentCheckPoint = checkPoint;
    }

    public void LoadData(Vector3Int lvlWorldNumb, int difficulty, TemporaryLevelCheckPointData tempData)
    {
        LevelWorldAndNumber = lvlWorldNumb;
        LevelDifficulty = difficulty;
        KilledPigs = new List<Vector3Int>(tempData.KilledPigs);
        Lives = tempData.CurrentLives;
        Ammos = tempData.CurrentAmmos;
        CurrentCheckPoint = tempData.CurrentCheckPoint;
        PlayerInputs =
        CompletedRings = new List<int>(tempData.CompletedRings);
        BarnsHitWithEgg = new List<int>(tempData.CurrentEggedBarns); // Reset or load from saved data if needed
        SetTrackingBools();
        UsedCheckpoint = true;

    }
    public void SetDifficulty(int difficulty)
    {
        LevelDifficulty = difficulty;
    }

    public void AddKillPig(Vector3Int pigData)
    {
        if (!trackPigs) return; // If not tracking pigs, do nothing
        if (KilledPigs == null) KilledPigs = new List<Vector3Int>();
        KilledPigs.Add(pigData);
    }
    public void AddPlayerInput(int input)
    {
        if (!trackInputs && !CertainNonAllowedInputs.Contains(input)) return; // If not tracking inputs, do nothing
        if (PlayerInputs == null) PlayerInputs = new List<int>();
        PlayerInputs.Add(input);
    }
    public void AddCompletedRing(int ringType)
    {
        if (!trackRings) return; // If not tracking rings, do nothing
        if (CompletedRings == null) CompletedRings = new List<int>();
        CompletedRings.Add(ringType);
    }
    public void AddBarnHitWithEgg(int id)
    {

        if (id >= BarnsHitWithEgg.Count)
        {
            for (int i = BarnsHitWithEgg.Count; i <= id; i++)
            {
                BarnsHitWithEgg.Add(0); // Initialize new barn entries to 0
            }

        }
        BarnsHitWithEgg[id]++; // Increment the count for the specific barn ID
    }

    public void EditCurrentAmmos(int ammoType, int amount)
    {

        if (Ammos == null) Ammos = new short[4]; // Assuming 2 types of ammo: Egg and Shotgun

        if (ammoType >= Ammos.Length)
        {
            short[] newAmmos = new short[ammoType + 1];
            for (int i = 0; i < Ammos.Length; i++)
            {
                newAmmos[i] = Ammos[i];
            }
            Ammos = newAmmos;
        }
        Ammos[ammoType] = (short)amount;

    }
    public void EditCurrentLives(int lives)
    {
        Lives = lives;

    }




    public int GetAmountOfChallenges()
    {
        return challenges.Length;
    }

    public int ReturnAmountOfNeededProgressTexts(int index)
    {
        var c = challenges[index];

        switch (c)
        {
            case ChallengeTypes.CheckPigs:

                if (TrackedPigs == null || TrackedPigs.Length == 0) return 0;

                return TrackedPigs.Length;

                break;


            case ChallengeTypes.CheckPigsByAmmoTypeWithPigType:
                if (TrackedPigsByBulletType_PigType_Amount.Length == 0) return 0;

                return 1;
            case ChallengeTypes.CheckCompletedRings:
                if (ringsTypeAndAmount.y <= 1) return 0;
                else return 1;
            case ChallengeTypes.CheckTotalBarns:
            case ChallengeTypes.CheckSpecificBarn:
            case ChallengeTypes.CheckEachBarn:
                return 1;










        }
        return 0;

    }

    public Vector2 ReturnCurrentProgressByChallengeIndex(int index, int challengeIndex)
    {
        var c = challenges[index];

        Vector2 vect = Vector2.zero;

        int xProgess = 0;
        int yTotal = 0;


        switch (c)
        {
            case ChallengeTypes.CheckPigs:

                if (TrackedPigs == null || TrackedPigs.Length == 0) return vect;



                yTotal = TrackedPigAmounts[challengeIndex];

                Debug.LogError("Y AMOUNT OF PIGS IS: " + yTotal);



                if (TrackedPigs[challengeIndex] == -1)
                {


                    xProgess = KilledPigs.Count;



                }

                // if (CheckNumberOfCertainIntInKilledPigs(KilledPigs, TrackedPigs[0]) >= TrackedPigAmounts[0])
                else
                {
                    xProgess = CountPigsOfType(TrackedPigs[challengeIndex]);


                }


                vect = new Vector2(xProgess, yTotal);
                Debug.LogError("VECT IS: " + vect);
                break;


            case ChallengeTypes.CheckPigsByAmmoTypeWithPigType:



                if (TrackedPigsByBulletType_PigType_Amount.Length == 0) return vect;

                yTotal = TrackedPigsByBulletType_PigType_Amount[index].z;

                xProgess = CountPigsByBulletType(TrackedPigsByBulletType_PigType_Amount[index].x, TrackedPigsByBulletType_PigType_Amount[index].y);

                vect = new Vector2(xProgess, yTotal);





                break;

            case ChallengeTypes.CheckCompletedRings:
                int x = 0;
                for (int i = 0; i < CompletedRings.Count; i++)
                {
                    if (CompletedRings[i] == ringsTypeAndAmount.x)
                    {
                        x++;
                    }
                }

                vect = new Vector2(x, ringsTypeAndAmount.y);

                break;

            case ChallengeTypes.CheckTotalBarns:
                int p1 = 0;
                if (BarnsHitWithEgg.Count > 0) p1 = BarnsHitWithEgg.Sum();
                vect = new Vector2(p1, totalBarnEggAmount);
                break;
            case ChallengeTypes.CheckSpecificBarn:
                int p2 = 0;
                if (BarnsHitWithEgg.Count > 0) p2 = BarnsHitWithEgg.Max();

                vect = new Vector2(p2, specificBarnEggAmount);
                break;
            case ChallengeTypes.CheckEachBarn:
                int p3 = 0;
                if (BarnsHitWithEgg.Count > 0) p3 = BarnsHitWithEgg.Count(x => x >= eachBarnAmount.y);
                vect = new Vector2(p3, eachBarnAmount.x);
                break;
                // check number of indexes in BarnsHitWithEgg that are greater than or equal to eachBarnAmount



        }
        return vect;

    }

    public int ReturnChallengeCompletion(int index, bool forFinish) // New logic 
    {

        switch (challenges[index])
        {
            case ChallengeTypes.None:
                return 0;


            case ChallengeTypes.CompleteLevel:
                if (forFinish)
                    return 1;
                else return 0;


            case ChallengeTypes.CheckLives:


                if (Lives >= LifeTarget)
                {
                    if (forFinish) return 1;
                    else return 0;
                }
                else return -1;




            case ChallengeTypes.CheckPigs:

                if (TrackedPigs == null || TrackedPigs.Length == 0) return 0;



                bool fullyCompleted = true;
                for (int i = 0; i < TrackedPigs.Length; i++)
                {
                    if (TrackedPigs[i] == -1)
                    {
                        int totalPigsKilled = KilledPigs.Count;

                        if (totalPigsKilled < TrackedPigAmounts[i])
                        {
                            fullyCompleted = false;
                            break;
                        }

                    }

                    // if (CheckNumberOfCertainIntInList(list, TrackedPigs[0]) >= TrackedPigAmounts[0])
                    else if (CountPigsOfType(TrackedPigs[i]) < TrackedPigAmounts[i])
                    {
                        fullyCompleted = false;
                        break;

                    }
                }

                if (fullyCompleted) return 1;
                else if (forFinish) return -1;
                break;




            case ChallengeTypes.CheckAmmo:




                if (Ammos[0] >= EggAmmoTarget && Ammos[1] >= ShotgunAmmoTarget)
                {
                    if (forFinish) return 1;
                    else return 0;
                }

                else return -1;

                break;

            case ChallengeTypes.CheckCertainNonAllowedInputs:



                foreach (var item in CertainNonAllowedInputs)
                {
                    if (PlayerInputs.Contains(item)) return -1;

                }
                if (forFinish)
                {
                    return 1;
                }
                break;



            case ChallengeTypes.CheckPigsByAmmoTypeWithPigType:
                if (TrackedPigsByBulletType_PigType_Amount.Length == 0) return 0;

                if (CountPigsByBulletType(TrackedPigsByBulletType_PigType_Amount[index].x,
                    TrackedPigsByBulletType_PigType_Amount[index].y) >= TrackedPigsByBulletType_PigType_Amount[index].z)
                {
                    return 1;
                }
                else if (forFinish)
                {
                    return -1; // Challenge not completed yet
                }
                break;



            case ChallengeTypes.CheckPigsByBulletIdWithPigType:
                if (TrackedPigIDWithPigType.Length == 0 || TrackedPigAmountsByID.Length == 0)
                {
                    if (forFinish) return -1;
                    else return 0;
                }

                if (LargestGroupByBulletIDAndType(

                        TrackedPigIDWithPigType[0])
                    >= TrackedPigAmountsByID[0])
                    return 1;

                else if (forFinish)
                {
                    return -1; // Challenge not completed yet
                }
                break;

            case ChallengeTypes.CheckCompletedRings:


                int checkFor = ringsTypeAndAmount.x;
                int amount = ringsTypeAndAmount.y;
                int currentCount = 0;

                for (int i = 0; i < CompletedRings.Count; i++)
                {
                    if (CompletedRings[i] == checkFor)
                    {
                        currentCount++;
                    }
                }

                if (currentCount >= amount) return 1;


                if (forFinish) return -1;
                break;

            case ChallengeTypes.CheckTotalBarns:

                if (BarnsHitWithEgg.Count > 0 && BarnsHitWithEgg.Sum() >= totalBarnEggAmount)
                    return 1;
                else if (forFinish) return -1;

                break;
            case ChallengeTypes.CheckSpecificBarn:
                if (BarnsHitWithEgg.Count > 0 && BarnsHitWithEgg.Max() >= specificBarnEggAmount)
                    return 1;
                else if (forFinish) return -1;
                break;

            case ChallengeTypes.CheckEachBarn:
                if (BarnsHitWithEgg.Count > 0 && BarnsHitWithEgg.Count(x => x >= eachBarnAmount.y) >= eachBarnAmount.x)
                    return 1;
                else if (forFinish) return -1;
                break;

        }
        return 0;

    }

    public int CheckChallengeType(int challengeIndex)
    {
        var c = challenges[challengeIndex];


        switch (c)
        {
            case ChallengeTypes.None:
                return 0;
                break;

            case ChallengeTypes.CompleteLevel:
                return 1;
                break;

            case ChallengeTypes.CheckLives:
                int l = lvlID.ReturnPlayerLives();

                if (l >= LifeTarget) return 1;
                else return 0;

                break;

            case ChallengeTypes.CheckPigs:

                if (TrackedPigs == null || TrackedPigs.Length == 0) return 0;



                bool fullyCompleted = true;
                for (int i = 0; i < TrackedPigs.Length; i++)
                {
                    if (TrackedPigs[i] == -1)
                    {
                        int totalPigsKilled = KilledPigs.Count;

                        if (totalPigsKilled < TrackedPigAmounts[i])
                        {
                            fullyCompleted = false;
                            break;
                        }

                    }

                    // if (CheckNumberOfCertainIntInList(list, TrackedPigs[0]) >= TrackedPigAmounts[0])
                    else if (CountPigsOfType(TrackedPigs[i]) < TrackedPigAmounts[i])
                    {
                        fullyCompleted = false;
                        break;

                    }
                }

                if (fullyCompleted) return 2;
                else return 1;


                break;

            case ChallengeTypes.CheckAmmo:


                var amounts = lvlID.ReturnAmmoAmounts();

                if (amounts.x >= EggAmmoTarget && amounts.y >= ShotgunAmmoTarget)
                    return 1;
                else return 1;

                break;

            case ChallengeTypes.CheckCertainNonAllowedInputs:

                List<int> inputs = lvlID.ReturnPlayerInputs();

                foreach (var item in CertainNonAllowedInputs)
                {
                    if (inputs.Contains(item)) return 0;

                }

                return 1;
                break;

            case ChallengeTypes.CheckPigsByAmmoTypeWithPigType:
                if (TrackedPigsByBulletType_PigType_Amount.Length == 0) return 0;

                if (CountPigsByBulletType(TrackedPigsByBulletType_PigType_Amount[0].x,
                    TrackedPigsByBulletType_PigType_Amount[0].y) >= TrackedPigsByBulletType_PigType_Amount[0].z)
                {
                    return 2;
                }


                return 1;

            case ChallengeTypes.CheckPigsByBulletIdWithPigType:
                if (TrackedPigIDWithPigType.Length == 0 || TrackedPigAmountsByID.Length == 0) return 0;

                if (LargestGroupByBulletIDAndType(

                        TrackedPigIDWithPigType[0])
                    >= TrackedPigAmountsByID[0])
                    return 2;

                return 1;

            case ChallengeTypes.CheckCompletedRings:

                // if (ringsTypeAndAmount.Count == 0) return 0;
                var ringList = lvlID.ReturnRingData();
                if (ringList == null || ringList.Length < 1) return 1;
                bool complete = true;

                return 2;


        }

        return 0;


    }




    public bool CheckChallengeCompletion(int challengeIndex)
    {
        var c = challenges[challengeIndex];

        if (FrameRateManager.under1) return false;

        switch (c)
        {
            case ChallengeTypes.None:
                return false;
                break;

            case ChallengeTypes.CompleteLevel:
                Debug.LogError("LevelCompleted is: " + lvlID.LevelCompleted);
                return lvlID.LevelCompleted;
                break;

            case ChallengeTypes.CheckLives:
                int l = lvlID.ReturnPlayerLives();

                if (l >= LifeTarget) return true;
                else return false;

                break;

            case ChallengeTypes.CheckPigs:

                if (TrackedPigs == null || TrackedPigs.Length == 0) return false;



                bool fullyCompleted = true;
                for (int i = 0; i < TrackedPigs.Length; i++)
                {
                    if (TrackedPigs[i] == -1)
                    {
                        int totalPigsKilled = KilledPigs.Count;

                        if (totalPigsKilled < TrackedPigAmounts[i])
                        {
                            fullyCompleted = false;
                            break;
                        }

                    }

                    // if (CheckNumberOfCertainIntInKilledPigs(KilledPigs, TrackedPigs[0]) >= TrackedPigAmounts[0])
                    else if (CountPigsOfType(TrackedPigs[i]) < TrackedPigAmounts[i])
                    {
                        fullyCompleted = false;
                        break;

                    }
                }

                return fullyCompleted;




            case ChallengeTypes.CheckAmmo:


                var amounts = lvlID.ReturnAmmoAmounts();

                if (amounts.x >= EggAmmoTarget && amounts.y >= ShotgunAmmoTarget)
                    return true;
                else return false;

                break;


            case ChallengeTypes.CheckCertainNonAllowedInputs:

                List<int> inputs = lvlID.ReturnPlayerInputs();

                foreach (var item in CertainNonAllowedInputs)
                {
                    if (inputs.Contains(item)) return false;

                }

                return true;




                break;

            // case ChallengeTypes.CheckPigsByAmmoTypeWithPigType:
            //     if (TrackedPigsByBulletTypeWithPigType.Length == 0 || TrackedPigAmountsByBullet.Length == 0) return false;

            //     if (CountPigsByBulletType(

            //             TrackedPigsByBulletTypeWithPigType[0].x,
            //             TrackedPigsByBulletTypeWithPigType[0].y)
            //         >= TrackedPigAmountsByBullet[0])
            //         return true;

            //     return false;

            case ChallengeTypes.CheckPigsByBulletIdWithPigType:
                if (TrackedPigIDWithPigType.Length == 0 || TrackedPigAmountsByID.Length == 0) return false;

                if (LargestGroupByBulletIDAndType(

                        TrackedPigIDWithPigType[0])
                    >= TrackedPigAmountsByID[0])
                    return true;

                return false;

                // case ChallengeTypes.CheckCompletedRings:

                //     if (ringsTypeAndAmount.Count == 0) return false;
                //     var ringList = lvlID.ReturnRingData();
                //     if (ringList == null || ringList.Length < 1) return false;
                //     bool complete = true;
                //     foreach (var ring in ringsTypeAndAmount)
                //     {
                //         if (ringList[ring.x] < ring.y)
                //         {
                //             complete = false;
                //             break;
                //         }

                //     }
                //     return complete;

        }

        return false;


    }

    public int CheckPigData(int targetInt, int component)
    {
        if (KilledPigs == null || KilledPigs.Count == 0) return 0;

        int amount = 0;

        switch (component)
        {
            case 0: // Check x component
                for (int i = 0; i < KilledPigs.Count; i++)
                {
                    if (KilledPigs[i].x == targetInt) amount++;
                }
                break;

            case 1: // Check y component
                for (int i = 0; i < KilledPigs.Count; i++)
                {
                    if (KilledPigs[i].y == targetInt) amount++;
                }
                break;

            case 2: // Check z component
                for (int i = 0; i < KilledPigs.Count; i++)
                {
                    if (KilledPigs[i].z == targetInt) amount++;
                }
                break;

            default:
                Debug.LogWarning("Invalid component specified. Use 0 for x, 1 for y, or 2 for z.");
                return 0; // Return 0 if an invalid component is passed
        }

        return amount;
    }

    public int CountPigsOfType(int pigType)
    {
        if (KilledPigs == null || KilledPigs.Count == 0) return 0;

        return KilledPigs.Count(pig => pig.x == pigType);
    }

    // 2. Return the amount of pigs killed by a certain bullet type
    public int CountPigsByBulletType(int bulletType, int targetPigType = -1)
    {
        if (KilledPigs == null || KilledPigs.Count == 0) return 0;

        // Count pigs that match the bulletType and optionally the pig type
        return KilledPigs.Count(pig => pig.y == bulletType && (targetPigType == -1 || pig.x == targetPigType));
    }

    // 3. Return the length of the largest group of pigs that share the same bulletID and optionally match a type
    public int LargestGroupByBulletIDAndType(int targetType)
    {
        if (KilledPigs == null || KilledPigs.Count == 0) return 0;

        // Group pigs by bullet ID, filtering by targetType and ignoring bulletID == -1
        var groupedByBulletID = KilledPigs
            .Where(pig => (targetType == -1 || pig.x == targetType) && pig.z != -1) // Filter by type and ignore bulletID -1
            .GroupBy(pig => pig.z); // Group by bullet ID

        // Find the largest group size
        int largestGroupSize = groupedByBulletID.Any() ? groupedByBulletID.Max(group => group.Count()) : 0;

        return largestGroupSize;
    }
    // public bool CheckIfCompleted(int challengeIndex,int singleInt, int[] arrayInt)
    // {
    //     var challengeType = challenges[challengeIndex];

    // }

}
