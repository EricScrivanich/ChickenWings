using UnityEngine;


[CreateAssetMenu(fileName = "TutorialData", menuName = "Setups/TutorialData")]
public class TutorialData : ScriptableObject
{
    private SpecialStateInputSystem inputSystem;
    private TutorialButtonHandler buttonHandler;
    public PlayerID playerID;
    private SpawnStateManager spawnManager;
    [SerializeField] private int buttonType = -1;
    [SerializeField] private bool startButtonHidden = true;
    [SerializeField] private bool startInputsLocked;



    [Header("Message String, replace with INPUT text")]
    [SerializeField] private string[] messages;
    [SerializeField] private string[] inputReplaceKeyboard;
    [SerializeField] private string[] inputReplaceGamepad;
    [SerializeField] private string[] inputReplaceTouchScreen;


    [SerializeField] private string[] allowedInput;



    private int currentInputCheckIndex = 0;
    private int currentBubbleIndex = 0;
    private int currentVectorIndex = 0;
    public int[] shownAmmos;

    private FlashGroup currentFlashMessage;
    private int currentMessageIndex;

    public GameObject bubblePrefab;
    public GameObject messagePrefab;
    private bool hasFinished = false;

    private int currentStepIndex;
    [SerializeField] private ushort[] stepsForActions;
    private bool pausedWaveTime = false;
    private bool pausedGameTime = false;



    public enum Type
    {
        None,
        SpawnBubble,
        ShowMessage,
        WaitForInput,
        StopWaveTime,
        StopGameTime,
        ResumeWaveTime,
        ResumeGameTime,
        StopAllButtons,
        ShowHiddenButtons,
        ResumeWaveTimeAndShowHidden,
        Next



    }
    [SerializeField] private Type[] actionsForStep;
    [SerializeField] private Type[] actionsForEnterBubble;
    [SerializeField] private Type[] actionsForExitBubble;

    [SerializeField] private Vector2[] vectorValues;
    [SerializeField] private Vector2[] bubbleSuctionAndDelayDurations;



    private string deviceType;


    public int GetButtonTypes()
    {
        return buttonType;
    }


    public void Initialize(SpawnStateManager m, ushort currentStep)
    {
        inputSystem = null;
        buttonHandler = null;

        Debug.Log("Initialize TutorialData with currentStep: " + currentStep);
        if (inputSystem == null)
        {
            inputSystem = GameObject.Find("Player").GetComponent<SpecialStateInputSystem>();
            inputSystem.SetTutorialData(this);

            if (currentStep == 0 && startInputsLocked)
            {
                inputSystem.SetAllowedInputs(false, null);
            }



        }


        if (currentStep > stepsForActions[stepsForActions.Length - 1])
        {

            Debug.Log("Current step is greater than last step, finishing tutorial.");
            m.HandleCheckForTutorial(false);
            if (inputSystem != null)
            {
                inputSystem.enabled = true;
            }
            return;
        }
        spawnManager = m;

        pausedWaveTime = false;
        pausedGameTime = false;


        currentBubbleIndex = 0;
        currentInputCheckIndex = 0;
        currentMessageIndex = 0;
        currentStepIndex = 0;
        currentVectorIndex = 0;
        deviceType = inputSystem.ReturnDeviceType();
        bool hideButton = startButtonHidden;

        for (int i = 0; i < stepsForActions.Length; i++)
        {
            if (stepsForActions[i] >= currentStep)
            {
                currentStepIndex = i;
                Debug.Log("Current step index set to: " + currentStepIndex);
                m.HandleCheckForTutorial(true);
                if (buttonHandler == null && GameObject.Find("Buttons").GetComponent<TutorialButtonHandler>() != null)
                {

                    buttonHandler = GameObject.Find("Buttons").GetComponent<TutorialButtonHandler>();
                    buttonHandler.InitializeTutorialButtons(buttonType, hideButton);
                    if (inputSystem != null && hideButton)
                    {
                        inputSystem.SetTempLockInput(buttonType);
                        inputSystem.enabled = true;
                    }
                }
                return;
            }
            else
            {
                switch (actionsForStep[i])
                {
                    case Type.SpawnBubble:
                        if (hideButton && actionsForEnterBubble[currentBubbleIndex] == Type.ShowHiddenButtons) hideButton = false;
                        currentBubbleIndex++;
                        currentVectorIndex++;
                        currentMessageIndex++;
                        break;
                    case Type.ShowMessage:
                        currentMessageIndex++;
                        break;
                    case Type.WaitForInput:
                        currentInputCheckIndex++;
                        break;


                }
            }
        }

        if (inputSystem != null)
        {
            inputSystem.enabled = true;
        }


        m.HandleCheckForTutorial(false);


    }
    public void NextSpawnStep(ushort ss)
    {
        if (stepsForActions[currentStepIndex] == ss)
        {

            for (int i = currentStepIndex; i < actionsForStep.Length; i++)
            {
                if (stepsForActions[i] == ss)
                {
                    DoAction(actionsForStep[i]);
                    if (i == actionsForStep.Length - 1)
                    {

                        spawnManager.HandleCheckForTutorial(false);
                        return;
                    }

                }
                else if (stepsForActions[i] > ss)
                {
                    currentStepIndex = i;
                    break;
                }
            }




        }

    }

    public void DoAction(Type t)
    {

        Debug.Log("DoAction: " + t + " at step: " + currentStepIndex);
        switch (t)
        {
            case Type.None:
                return;
            case Type.ResumeWaveTimeAndShowHidden:

                DoAction(Type.ResumeWaveTime);
                DoAction(Type.ShowHiddenButtons);
                inputSystem.SetTempLockInput(-1);

                break;

            case Type.SpawnBubble:
                var b = Instantiate(bubblePrefab, vectorValues[currentVectorIndex], Quaternion.identity);
                b.GetComponent<TriggerNextSection>().InitiliazeBubble(this, currentBubbleIndex, bubbleSuctionAndDelayDurations[currentBubbleIndex].x, bubbleSuctionAndDelayDurations[currentBubbleIndex].y);
                currentBubbleIndex++;
                currentVectorIndex++;

                break;
            case Type.ShowMessage:
                var obj = Instantiate(messagePrefab, GameObject.Find("Canvas").transform);
                currentFlashMessage = obj.GetComponent<FlashGroup>();
                currentFlashMessage.ShowMessage(ReturnMessageForDevice());
                break;
            case Type.WaitForInput:
                string[] i = ReturnAllowedInputs();

                if (i == null)
                    inputSystem.SetAllowedInputs(true, null);
                else
                    inputSystem.SetAllowedInputs(false, i);

                break;
            case Type.StopWaveTime:
                spawnManager.HandleWaveTime(false);
                break;
            case Type.StopGameTime:
                Time.timeScale = 0;
                break;
            case Type.ResumeWaveTime:
                spawnManager.HandleWaveTime(true);
                break;
            case Type.ResumeGameTime:
                Time.timeScale = FrameRateManager.BaseTimeScale;
                break;

            case Type.ShowHiddenButtons:
                buttonHandler.FinishTween();
                inputSystem.SetTempLockInput(-1);

                break;

                // case Type.DelayForInput:
                //     spawnManager.DelayForInput(stepsForActions[currentStepIndex]);
                //     break;
                // case Type.StopAllButtons:
                //     spawnManager.StopAllButtons();
                //     break;
        }
    }

    public void HitCorrectInput()
    {

    }
    private int bubbleIDAddedCheck = 0;
    public void HandleBubble(bool entered, int ID)
    {
        if (entered)
        {
            string[] i = ReturnAllowedInputs();
            if (i == null)
                inputSystem.SetAllowedInputs(true, null);
            else
                inputSystem.SetAllowedInputs(false, i);

            DoAction(Type.ShowMessage);

            if (ID < actionsForEnterBubble.Length)
            {
                DoAction(actionsForEnterBubble[ID]);
                if (ID + 1 < actionsForEnterBubble.Length && actionsForEnterBubble[ID + 1] == Type.Next)
                {
                    DoAction(actionsForEnterBubble[ID]);
                }
            }




        }
        else
        {
            if (ID < actionsForExitBubble.Length)
                DoAction(actionsForExitBubble[ID]);
        }

    }

    private string[] ReturnAllowedInputs()
    {
        string s = allowedInput[currentInputCheckIndex];
        if (s == "Any") return null;
        // check for amount of spaces in s
        int spaceCount = 0;
        foreach (char c in s)
        {
            if (c == ' ')
            {
                spaceCount++;
            }
        }
        string[] inputs = new string[spaceCount + 1];
        // split string by spaces
        inputs = s.Split(' ');
        currentInputCheckIndex++;

        for (int i = 0; i < inputs.Length; i++)
        {
            Debug.Log("Input " + i + ": " + inputs[i]);
        }
        return inputs;


    }

    private string ReturnMessageForDevice()
    {
        string s = messages[currentMessageIndex];
        if (currentMessageIndex < inputReplaceKeyboard.Length)
        {
            switch (deviceType)
            {
                case "Keyboard":
                    s = s.Replace("INPUT", inputReplaceKeyboard[currentMessageIndex]);
                    break;
                case "Gamepad":
                    s = s.Replace("INPUT", inputReplaceGamepad[currentMessageIndex]);
                    break;
                case "TouchScreen":
                    s = s.Replace("INPUT", inputReplaceTouchScreen[currentMessageIndex]);
                    break;
            }
        }
        currentMessageIndex++;

        return s;



    }










}
