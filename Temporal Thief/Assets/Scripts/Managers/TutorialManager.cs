using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    // Session-only tutorial progress:
    // - survives scene reloads (paradox)
    // - resets when the game is closed and reopened
    static int sessionStep = 0;
    static bool sessionInitialized = false;

    public GameObject objectivePanel;
    public TMP_Text objectiveText;

    public GameObject popupPanel;
    public TMP_Text popupText;
    public Button continueButton;

    public PauseManager pauseMenu;

    int step = 0;
    bool waitingForAction = false;

    private float _lookProgress = 0f;
    public float lookGoal = 60f;

    private bool _didW = false;
    private bool _didA = false;
    private bool _didS = false;
    private bool _didD = false;

    // When we reach this step (or higher), we consider tutorial "finished"
    public int tutorialCompleteStep = 12;

    void Start()
    {
        continueButton.onClick.AddListener(ContinueClicked);

        // Init session step on first load of the app
        if (!sessionInitialized)
        {
            sessionInitialized = true;
            sessionStep = 0; // new app run starts tutorial from beginning
            DontDestroyOnLoad(gameObject);
        }

        popupPanel.SetActive(false);
        objectivePanel.SetActive(true);

        ShowStep(sessionStep);
    }

    void Update()
    {
        if (!waitingForAction) return;

        switch (step)
        {
            case 1:
                CheckLookAround();
                break;
            case 2:
                CheckJump();
                break;
            case 3:
                CheckWASD();
                break;
            case 7:
                CheckEchoKey();
                break;
        }
    }

    void ShowStep(int newStep)
    {
        step = newStep;

        // Keep progress across scene reloads (paradox), but not across app restarts
        sessionStep = step;

        waitingForAction = false;

        // Tutorial complete check (NO PlayerPrefs now)
        if (step >= tutorialCompleteStep)
        {
            popupPanel.SetActive(false);
            objectivePanel.SetActive(false);
            Pause(false);
            HideCursor();
            return;
        }

        if (step == 0)
        {
            Pause(true);
            ShowPopup("Welcome! Follow the objectives at the top of the screen.\nClick Continue to begin.");
            SetObjective("Objective: Get ready.");
        }
        else if (step == 1)
        {
            Pause(false);
            HidePopup();
            SetObjective("Objective: Move your mouse to look around.");
            waitingForAction = true;
            _lookProgress = 0f;
        }
        else if (step == 2)
        {
            Pause(false);
            HidePopup();
            SetObjective("Objective: Press Space to jump.");
            waitingForAction = true;
        }
        else if (step == 3)
        {
            Pause(false);
            HidePopup();
            SetObjective("Objective: Press W, A, S and D.");
            waitingForAction = true;
            _didW = _didA = _didS = _didD = false;
        }
        else if (step == 4)
        {
            Pause(true);
            ShowPopup("Next: Pressure Pads open doors when held.\nClick Continue.");
            SetObjective("Objective: Learn pressure pads.");
        }
        else if (step == 5)
        {
            Pause(false);
            HidePopup();
            SetObjective("Objective: Stand on the pressure pad until the door opens.");
            waitingForAction = true;
        }
        else if (step == 6)
        {
            Pause(true);
            ShowPopup("You may not have time to reach the door before it closes.\nYou will need an Echo to hold the pressure pad.\nEchoes replay your actions, including interactions!\nClick Continue.");
            SetObjective("Objective: Learn echoes.");
        }
        else if (step == 7)
        {
            Pause(false);
            HidePopup();
            SetObjective("Objective: Press E to create an Echo.");
            waitingForAction = true;
        }
        else if (step == 8)
        {
            Pause(true);
            ShowPopup("See what happens when an echo spots you.. \nClick Continue.");
            SetObjective("Objective: Avoid paradox.");
        }
        else if (step == 9)
        {
            Pause(false);
            HidePopup();
            SetObjective("Walk across the green line of sight..");
            // waitingForAction is not needed here because EchoCaught() advances the step
        }
        else if (step == 10)
        {
            Pause(true);
            ShowPopup("WARNING: Do NOT let your Echo see you.\nThat causes a paradox.\nClick Continue.");
            SetObjective("Objective: Dont let that happen again");
        }
        else if (step == 11)
        {
            Pause(false);
            HidePopup();
            SetObjective("Objective: Exit the room before the door closes.");
            // Advance immediately to completion after a short delay or on exit
            Invoke(nameof(CompleteTutorial), 2f); // Give time to read
        }
        else if (step == 12)
        {
            Pause(true);
            ShowPopup("Tutorial Complete!\nYou've learned the basics of Temporal Thief.\nUse echoes to manipulate time and solve puzzles.\nClick Continue to play.");
            SetObjective("Objective: Enjoy the game!");
        }
    }

    void NextStep() => ShowStep(step + 1);
    void ContinueClicked() => NextStep();

    void SetObjective(string text) => objectiveText.text = text;

    void ShowPopup(string text)
    {
        popupPanel.SetActive(true);
        popupText.text = text;
        ShowCursor();
    }

    void HidePopup()
    {
        popupPanel.SetActive(false);
        HideCursor();
    }

    void Pause(bool paused)
    {
        if (pauseMenu != null) pauseMenu.SetPausedNoMenu(paused);
        else Time.timeScale = paused ? 0f : 1f;
    }

    void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void CheckLookAround()
    {
        float mx = Mathf.Abs(Input.GetAxis("Mouse X"));
        float my = Mathf.Abs(Input.GetAxis("Mouse Y"));
        _lookProgress += (mx + my) * 10f;

        if (_lookProgress >= lookGoal)
        {
            NextStep();
            Debug.Log("Tutorial: Player looked around");
        }
    }

    void CheckJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextStep();
            Debug.Log("Tutorial: Space key pushed");
        }
    }

    void CheckWASD()
    {
        if (Input.GetKeyDown(KeyCode.W)) _didW = true;
        if (Input.GetKeyDown(KeyCode.A)) _didA = true;
        if (Input.GetKeyDown(KeyCode.S)) _didS = true;
        if (Input.GetKeyDown(KeyCode.D)) _didD = true;

        if (_didW && _didA && _didS && _didD)
        {
            NextStep();
            Debug.Log("Tutorial: Movement keys pressed.");
        }
    }

    void CheckEchoKey()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            NextStep();
            Debug.Log("Tutorial: Key E Pushed");
        }
    }

    public void PressurePadComplete()
    {
        if (step == 5)
        {
            NextStep();
            Debug.Log("Tutorial: Pressure pad stepped on");
        }
    }

    public void EchoCreated()
    {
        if (step == 7)
        {
            NextStep();
            Debug.Log("Tutorial: Echo Created");
        }
    }

    void CompleteTutorial()
    {
        NextStep(); // To step 12
    }
}