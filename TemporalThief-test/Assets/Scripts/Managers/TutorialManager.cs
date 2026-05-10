using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    public GameObject popupPanel;
    public Text popupText;
    public Button continueButton;
    public Text objectiveText;

    int step;
    bool waitingForAction;
    float lookProgress;
    bool didW, didA, didS, didD;

    public int CurrentStep => step;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        continueButton.onClick.AddListener(OnContinue);
        ShowStep(0);
    }

    void Update()
    {
        if (!waitingForAction)
            return;

        if (step == 1)
            TrackLook();
        else if (step == 2)
            if (Input.GetKeyDown(KeyCode.Space)) AdvanceStep();
        else if (step == 3)
            TrackMovementKeys();
    }

    void ShowStep(int newStep)
    {
        step = newStep;
        waitingForAction = false;

        if (step == 0)
        {
            ShowPopup("Welcome to Temporal Thief. Click Continue to begin.");
            SetObjective("Objective: Prepare for the tutorial.");
        }
        else if (step == 1)
        {
            HidePopup();
            SetObjective("Objective: Move your mouse to look around.");
            waitingForAction = true;
            lookProgress = 0f;
        }
        else if (step == 2)
        {
            HidePopup();
            SetObjective("Objective: Press Space to jump.");
            waitingForAction = true;
        }
        else if (step == 3)
        {
            HidePopup();
            SetObjective("Objective: Press W, A, S and D.");
            waitingForAction = true;
            didW = didA = didS = didD = false;
        }
        else if (step == 4)
        {
            ShowPopup("Next: Pressure plates open doors when held.");
            SetObjective("Objective: Learn pressure plates.");
        }
        else if (step == 5)
        {
            HidePopup();
            SetObjective("Objective: Stand on a pressure plate.");
            waitingForAction = true;
        }
        else if (step == 6)
        {
            ShowPopup("Press E to create an echo and record your run.");
            SetObjective("Objective: Create an echo.");
        }
        else if (step == 7)
        {
            HidePopup();
            SetObjective("Objective: Avoid being seen by the guard.");
            waitingForAction = true;
        }
        else
        {
            ShowPopup("Tutorial complete. Play the game using echoes and stealth.");
            SetObjective("Objective: Play.");
        }
    }

    void TrackLook()
    {
        lookProgress += Mathf.Abs(Input.GetAxis("Mouse X")) + Mathf.Abs(Input.GetAxis("Mouse Y"));
        if (lookProgress >= 6f)
            AdvanceStep();
    }

    void TrackMovementKeys()
    {
        if (Input.GetKeyDown(KeyCode.W)) didW = true;
        if (Input.GetKeyDown(KeyCode.A)) didA = true;
        if (Input.GetKeyDown(KeyCode.S)) didS = true;
        if (Input.GetKeyDown(KeyCode.D)) didD = true;

        if (didW && didA && didS && didD)
            AdvanceStep();
    }

    public void PressurePadComplete()
    {
        if (step == 5)
            AdvanceStep();
    }

    public void EchoCreated()
    {
        if (step == 6)
            AdvanceStep();
    }

    public void EchoCaught()
    {
        if (step == 7)
            AdvanceStep();
    }

    public void SetStep(int newStep)
    {
        if (newStep < 0)
            newStep = 0;

        ShowStep(newStep);
    }

    void AdvanceStep()
    {
        ShowStep(step + 1);
    }

    void OnContinue()
    {
        AdvanceStep();
    }

    void ShowPopup(string text)
    {
        if (popupPanel != null)
            popupPanel.SetActive(true);
        if (popupText != null)
            popupText.text = text;
    }

    void HidePopup()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    void SetObjective(string text)
    {
        if (objectiveText != null)
            objectiveText.text = text;
    }
}
