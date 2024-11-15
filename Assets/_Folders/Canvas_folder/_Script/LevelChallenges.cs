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
        CheckPigsByBulletIdWithPigType
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

    public int GetAmountOfChallenges()
    {
        return challenges.Length;
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

                if (TrackedPigs[0] == -1)
                {
                    int totalPigsKilled = list.Count;

                    if (totalPigsKilled >= TrackedPigAmounts[0]) return 2;
                    else return 1;
                }

                // if (CheckNumberOfCertainIntInList(list, TrackedPigs[0]) >= TrackedPigAmounts[0])
                if (CountPigsOfType(list, TrackedPigs[0]) >= TrackedPigAmounts[0])
                    return 2;

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

                if (CountPigsByBulletType(
                        lvlID.ReturnKilledPigs(),
                        TrackedPigsByBulletTypeWithPigType[0].y,
                        TrackedPigsByBulletTypeWithPigType[0].x)
                    >= TrackedPigAmountsByBullet[0])
                    return 2;

                return 1;

            case ChallengeTypes.CheckPigsByBulletIdWithPigType:
                if (TrackedPigIDWithPigType.Length == 0 || TrackedPigAmountsByID.Length == 0) return 0;

                if (LargestGroupByBulletIDAndType(
                        lvlID.ReturnKilledPigs(),
                        TrackedPigIDWithPigType[0])
                    >= TrackedPigAmountsByID[0])
                    return 2;

                return 1;


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

                if (TrackedPigs[0] == -1)
                {
                    int totalPigsKilled = list.Count;

                    if (totalPigsKilled >= TrackedPigAmounts[0]) return true;
                    else return false;
                }

                // if (CheckNumberOfCertainIntInList(list, TrackedPigs[0]) >= TrackedPigAmounts[0])
                if (CountPigsOfType(list, TrackedPigs[0]) >= TrackedPigAmounts[0])
                    return true;

                else return false;

                break;

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
