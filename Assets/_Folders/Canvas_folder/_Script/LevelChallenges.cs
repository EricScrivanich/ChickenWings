using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Levels/LevelChallenges")]
public class LevelChallenges : ScriptableObject
{
    [SerializeField] private LevelManagerID lvlID;
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
        CheckCompletedRings
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

    [SerializeField] private Vector2Int[] TrackedPigsByBulletTypeWithPigType;
    [SerializeField] private int[] TrackedPigAmountsByBullet;
    [SerializeField] private int[] TrackedPigIDWithPigType;
    [SerializeField] private int[] TrackedPigAmountsByID;

    [SerializeField] private List<int> CertainNonAllowedInputs;
    [SerializeField] private List<Vector2Int> ringsTypeAndAmount;

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
                if (TrackedPigsByBulletTypeWithPigType.Length == 0 || TrackedPigAmountsByBullet.Length == 0) return 0;

                return TrackedPigAmountsByBullet.Length;



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

                var list = lvlID.ReturnKilledPigs();

                yTotal = TrackedPigAmounts[challengeIndex];

                Debug.LogError("Y AMOUNT OF PIGS IS: " + yTotal);



                if (TrackedPigs[challengeIndex] == -1)
                {


                    xProgess = list.Count;



                }

                // if (CheckNumberOfCertainIntInList(list, TrackedPigs[0]) >= TrackedPigAmounts[0])
                else
                {
                    xProgess = CountPigsOfType(list, TrackedPigs[challengeIndex]);


                }


                vect = new Vector2(xProgess, yTotal);
                Debug.LogError("VECT IS: " + vect);
                break;


            case ChallengeTypes.CheckPigsByAmmoTypeWithPigType:

                yTotal = TrackedPigAmountsByBullet[challengeIndex];

                if (TrackedPigsByBulletTypeWithPigType.Length == 0 || TrackedPigAmountsByBullet.Length == 0) return vect;



                xProgess = CountPigsByBulletType(lvlID.ReturnKilledPigs(), TrackedPigsByBulletTypeWithPigType[challengeIndex].x, TrackedPigsByBulletTypeWithPigType[challengeIndex].y);

                vect = new Vector2(xProgess, yTotal);





                break;


        }
        return vect;

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

                var list = lvlID.ReturnKilledPigs();

                bool fullyCompleted = true;
                for (int i = 0; i < TrackedPigs.Length; i++)
                {
                    if (TrackedPigs[i] == -1)
                    {
                        int totalPigsKilled = list.Count;

                        if (totalPigsKilled < TrackedPigAmounts[i])
                        {
                            fullyCompleted = false;
                            break;
                        }

                    }

                    // if (CheckNumberOfCertainIntInList(list, TrackedPigs[0]) >= TrackedPigAmounts[0])
                    else if (CountPigsOfType(list, TrackedPigs[i]) < TrackedPigAmounts[i])
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
                if (TrackedPigsByBulletTypeWithPigType.Length == 0 || TrackedPigAmountsByBullet.Length == 0) return 0;

                if (CountPigsByBulletType(lvlID.ReturnKilledPigs(), TrackedPigsByBulletTypeWithPigType[0].x,
                    TrackedPigsByBulletTypeWithPigType[0].y) >= TrackedPigAmountsByBullet[0])
                {
                    return 2;
                }


                return 1;

            case ChallengeTypes.CheckPigsByBulletIdWithPigType:
                if (TrackedPigIDWithPigType.Length == 0 || TrackedPigAmountsByID.Length == 0) return 0;

                if (LargestGroupByBulletIDAndType(
                        lvlID.ReturnKilledPigs(),
                        TrackedPigIDWithPigType[0])
                    >= TrackedPigAmountsByID[0])
                    return 2;

                return 1;

            case ChallengeTypes.CheckCompletedRings:

                if (ringsTypeAndAmount.Count == 0) return 0;
                var ringList = lvlID.ReturnRingData();
                if (ringList == null || ringList.Length < 1) return 1;
                bool complete = true;
                foreach (var ring in ringsTypeAndAmount)
                {
                    if (ringList[ring.x] < ring.y)
                    {
                        return 1;

                    }

                }
                return 2;


        }

        return 0;


    }




    public bool CheckChallengeCompletion(int challengeIndex)
    {
        var c = challenges[challengeIndex];

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

                var list = lvlID.ReturnKilledPigs();

                bool fullyCompleted = true;
                for (int i = 0; i < TrackedPigs.Length; i++)
                {
                    if (TrackedPigs[i] == -1)
                    {
                        int totalPigsKilled = list.Count;

                        if (totalPigsKilled < TrackedPigAmounts[i])
                        {
                            fullyCompleted = false;
                            break;
                        }

                    }

                    // if (CheckNumberOfCertainIntInList(list, TrackedPigs[0]) >= TrackedPigAmounts[0])
                    else if (CountPigsOfType(list, TrackedPigs[i]) < TrackedPigAmounts[i])
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

            case ChallengeTypes.CheckPigsByAmmoTypeWithPigType:
                if (TrackedPigsByBulletTypeWithPigType.Length == 0 || TrackedPigAmountsByBullet.Length == 0) return false;

                if (CountPigsByBulletType(
                        lvlID.ReturnKilledPigs(),
                        TrackedPigsByBulletTypeWithPigType[0].x,
                        TrackedPigsByBulletTypeWithPigType[0].y)
                    >= TrackedPigAmountsByBullet[0])
                    return true;

                return false;

            case ChallengeTypes.CheckPigsByBulletIdWithPigType:
                if (TrackedPigIDWithPigType.Length == 0 || TrackedPigAmountsByID.Length == 0) return false;

                if (LargestGroupByBulletIDAndType(
                        lvlID.ReturnKilledPigs(),
                        TrackedPigIDWithPigType[0])
                    >= TrackedPigAmountsByID[0])
                    return true;

                return false;

            case ChallengeTypes.CheckCompletedRings:

                if (ringsTypeAndAmount.Count == 0) return false;
                var ringList = lvlID.ReturnRingData();
                if (ringList == null || ringList.Length < 1) return false;
                bool complete = true;
                foreach (var ring in ringsTypeAndAmount)
                {
                    if (ringList[ring.x] < ring.y)
                    {
                        complete = false;
                        break;
                    }

                }
                return complete;

        }

        return false;


    }

    public int CheckPigData(List<Vector3Int> l, int targetInt, int component)
    {
        if (l == null || l.Count == 0) return 0;

        int amount = 0;

        switch (component)
        {
            case 0: // Check x component
                for (int i = 0; i < l.Count; i++)
                {
                    if (l[i].x == targetInt) amount++;
                }
                break;

            case 1: // Check y component
                for (int i = 0; i < l.Count; i++)
                {
                    if (l[i].y == targetInt) amount++;
                }
                break;

            case 2: // Check z component
                for (int i = 0; i < l.Count; i++)
                {
                    if (l[i].z == targetInt) amount++;
                }
                break;

            default:
                Debug.LogWarning("Invalid component specified. Use 0 for x, 1 for y, or 2 for z.");
                return 0; // Return 0 if an invalid component is passed
        }

        return amount;
    }

    public int CountPigsOfType(List<Vector3Int> killedPigs, int pigType)
    {
        if (killedPigs == null || killedPigs.Count == 0) return 0;

        return killedPigs.Count(pig => pig.x == pigType);
    }

    // 2. Return the amount of pigs killed by a certain bullet type
    public int CountPigsByBulletType(List<Vector3Int> killedPigs, int bulletType, int targetPigType = -1)
    {
        if (killedPigs == null || killedPigs.Count == 0) return 0;

        // Count pigs that match the bulletType and optionally the pig type
        return killedPigs.Count(pig => pig.y == bulletType && (targetPigType == -1 || pig.x == targetPigType));
    }

    // 3. Return the length of the largest group of pigs that share the same bulletID and optionally match a type
    public int LargestGroupByBulletIDAndType(List<Vector3Int> killedPigs, int targetType)
    {
        if (killedPigs == null || killedPigs.Count == 0) return 0;

        // Group pigs by bullet ID, filtering by targetType and ignoring bulletID == -1
        var groupedByBulletID = killedPigs
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
