using UnityEditor;
namespace KingdomOfNight
{
    public partial class SceneLoader
    {
#if UNITY_EDITOR
        [MenuItem("Scenes/BrickBoi")]
        public static void LoadBrickBoi() { OpenScene("Assets/scenes/BrickBoi.unity"); }
        [MenuItem("Scenes/Boss/Deviled Egg")]
        public static void LoadDeviledEgg() { OpenScene("Assets/scenes/Boss/Deviled Egg.unity"); }
        [MenuItem("Scenes/Boss/DrumStick")]
        public static void LoadDrumStick() { OpenScene("Assets/scenes/Boss/DrumStick.unity"); }
        [MenuItem("Scenes/Boss/Helicopter")]
        public static void LoadHelicopter() { OpenScene("Assets/scenes/Boss/Helicopter.unity"); }
        [MenuItem("Scenes/Boss/KingPig")]
        public static void LoadKingPig() { OpenScene("Assets/scenes/Boss/KingPig.unity"); }
        [MenuItem("Scenes/Levels/EnemyVideo")]
        public static void LoadEnemyVideo() { OpenScene("Assets/scenes/Levels/EnemyVideo.unity"); }
        [MenuItem("Scenes/Levels/Level1")]
        public static void LoadLevel1() { OpenScene("Assets/scenes/Levels/Level1.unity"); }
        [MenuItem("Scenes/Levels/Level10")]
        public static void LoadLevel10() { OpenScene("Assets/scenes/Levels/Level10.unity"); }
        [MenuItem("Scenes/Levels/Level11")]
        public static void LoadLevel11() { OpenScene("Assets/scenes/Levels/Level11.unity"); }
        [MenuItem("Scenes/Levels/Level12")]
        public static void LoadLevel12() { OpenScene("Assets/scenes/Levels/Level12.unity"); }
        [MenuItem("Scenes/Levels/Level2")]
        public static void LoadLevel2() { OpenScene("Assets/scenes/Levels/Level2.unity"); }
        [MenuItem("Scenes/Levels/Level3")]
        public static void LoadLevel3() { OpenScene("Assets/scenes/Levels/Level3.unity"); }
        [MenuItem("Scenes/Levels/Level4")]
        public static void LoadLevel4() { OpenScene("Assets/scenes/Levels/Level4.unity"); }
        [MenuItem("Scenes/Levels/Level5")]
        public static void LoadLevel5() { OpenScene("Assets/scenes/Levels/Level5.unity"); }
        [MenuItem("Scenes/Levels/Level6")]
        public static void LoadLevel6() { OpenScene("Assets/scenes/Levels/Level6.unity"); }
        [MenuItem("Scenes/Levels/Level7")]
        public static void LoadLevel7() { OpenScene("Assets/scenes/Levels/Level7.unity"); }
        [MenuItem("Scenes/Levels/Level8 1")]
        public static void LoadLevel81() { OpenScene("Assets/scenes/Levels/Level8 1.unity"); }
        [MenuItem("Scenes/Levels/Level8")]
        public static void LoadLevel8() { OpenScene("Assets/scenes/Levels/Level8.unity"); }
        [MenuItem("Scenes/Levels/Level9")]
        public static void LoadLevel9() { OpenScene("Assets/scenes/Levels/Level9.unity"); }
        [MenuItem("Scenes/Main/Basics")]
        public static void LoadBasics() { OpenScene("Assets/scenes/Main/Basics.unity"); }
        [MenuItem("Scenes/Main/BasicsPig")]
        public static void LoadBasicsPig() { OpenScene("Assets/scenes/Main/BasicsPig.unity"); }
        [MenuItem("Scenes/Main/BasicsSlowed")]
        public static void LoadBasicsSlowed() { OpenScene("Assets/scenes/Main/BasicsSlowed.unity"); }
        [MenuItem("Scenes/Main/MainMenu")]
        public static void LoadMainMenu() { OpenScene("Assets/scenes/Main/MainMenu.unity"); }
        [MenuItem("Scenes/Main/TutorialVideos")]
        public static void LoadTutorialVideos() { OpenScene("Assets/scenes/Main/TutorialVideos.unity"); }
        [MenuItem("Scenes/TimeTrial/_Mountains 1")]
        public static void Load_Mountains1() { OpenScene("Assets/scenes/TimeTrial/_Mountains 1.unity"); }
        [MenuItem("Scenes/TimeTrial/_Mountains 2")]
        public static void Load_Mountains2() { OpenScene("Assets/scenes/TimeTrial/_Mountains 2.unity"); }
        [MenuItem("Scenes/TimeTrial/_Mountains")]
        public static void Load_Mountains() { OpenScene("Assets/scenes/TimeTrial/_Mountains.unity"); }
#endif
    }
}