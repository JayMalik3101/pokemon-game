using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState
    {
        WORLD,
        BATTLE,
        MENU,
        MESSAGEBOX,
        DEX,
        PARTY
    }
    [HideInInspector]
    public GameState State;
    private GameState OldState;

    // UI
    [HideInInspector]
    public GameObject WorldUI, BattleUI, DexUI, PartyUI;

    public Sprite PlayerBackSprite;

    // game
    public int StartingPokeballs = 20;
    public float TextSpeed = 0.5f;
    private string msgBoxText;
    private string textShowing;
    private float textTime;
    private bool isEnd;

    [HideInInspector]
    public Party Team;
    private BattleManager battleManager;

    // debug
    private bool isError = false;
    private int errorType = 0;
    private string errorString = "";
    private string version = "1.0";

    void Awake()
    {
        instance = this;
        StartCoroutine(LoadInterface());
    }

    IEnumerator LoadInterface()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync("InterfaceScene", LoadSceneMode.Additive);
        yield return async;

        GameObject canvas = GameObject.Find("Canvas");
        WorldUI = canvas.transform.Find("World").gameObject;
        BattleUI = canvas.transform.Find("Battle").gameObject;
        DexUI = canvas.transform.Find("Pokedex").gameObject;
        PartyUI = canvas.transform.Find("Party").gameObject;

        Init();
    }

    // Use this for initialization
    void Init()
    {
        Team = PartyUI.GetComponent<Party>();
        Team.Init();

        battleManager = GetComponent<BattleManager>();

        State = GameState.WORLD;
        OldState = State;
        BattleUI.SetActive(false);

        #region WorldUI
        GameObject menuBox = WorldUI.transform.Find("Menu").gameObject;
        foreach (Transform obj in menuBox.transform)
        {
            if (obj.name.Equals("Pokedex"))
                obj.GetComponent<Button>().onClick.AddListener(() => SelectMenuItem(0));
            if (obj.name.Equals("Pokemon"))
                obj.GetComponent<Button>().onClick.AddListener(() => SelectMenuItem(1));
            if (obj.name.Equals("Quit"))
                obj.GetComponent<Button>().onClick.AddListener(() => SelectMenuItem(2));
        }

        foreach (Transform obj in WorldUI.transform)
            obj.gameObject.SetActive(false);

        WorldUI.SetActive(true);
        #endregion

        #region PartyUI
        GameObject closeParty = PartyUI.transform.Find("Panel/Close").gameObject;
        closeParty.GetComponent<Button>().onClick.AddListener(() => CloseParty());

        PartyUI.SetActive(false);
        #endregion

        #region DexUI
        GameObject closeDex = DexUI.transform.Find("Panel/Close").gameObject;
        closeDex.GetComponent<Button>().onClick.AddListener(() => CloseDex());
        DexUI.GetComponent<Pokedex>().Init();
        DexUI.GetComponent<Pokedex>().Refresh();
        DexUI.SetActive(false);
        #endregion

    }

    // Update is called once per frame
    void LateUpdate()
    {
        switch (State)
        {
            case GameState.WORLD:
                UpdateWorld();
                break;
            case GameState.MENU:
                UpdateMenu();
                break;
            case GameState.MESSAGEBOX:
                UpdateMessageBox();
                break;
            case GameState.BATTLE:
                battleManager.UpdateBattle();
                break;
            case GameState.DEX:
                UpdateDex();
                break;
            case GameState.PARTY:
                UpdateParty();
                break;
        }
    }

    private void UpdateWorld()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Invoke("OpenMenu", 0.1f);
        }

        if ((Team.AllDead || StartingPokeballs <= 0 || DexUI.GetComponent<Pokedex>().AllCaught) && !isEnd)
        {
            isEnd = true;
            if (Team.AllDead)
                ShowText("You're out of pokémon! It's over...");
            else if (StartingPokeballs <= 0)
                ShowText("Time's up! You're out of pokéballs!");
            else if (DexUI.GetComponent<Pokedex>().AllCaught)
                ShowText("Time's up! You've caught all our pokémon!");
        }
    }

    private void UpdateMenu()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            CloseMenu();
        }
    }

    private void UpdateMessageBox()
    {
        if (textShowing.Length == msgBoxText.Length)
        {
            WorldUI.transform.Find("MessageBox/Arrow").gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                if (!battleManager.StickyMessageBox)
                    WorldUI.transform.Find("MessageBox").gameObject.SetActive(false);
                else
                    WorldUI.transform.Find("MessageBox/Arrow").gameObject.SetActive(false);

                State = OldState;
                if (battleManager.BattlePhase != BattleManager.Phase.NONE)
                    battleManager.NextPhase();

                if (isEnd && State == GameState.WORLD)
                    SelectMenuItem(2);
            }
        }
        else
        {
            if (textShowing.Length > 1)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    textShowing = msgBoxText;
                    WorldUI.transform.Find("MessageBox/Text").GetComponent<Text>().text = textShowing;
                    return;
                }
            }

            textTime += Time.deltaTime;
            if (textTime > (TextSpeed / 10f))
            {
                textShowing += msgBoxText[textShowing.Length];
                WorldUI.transform.Find("MessageBox/Text").GetComponent<Text>().text = textShowing;
                textTime = 0;
            }
        }
    }

    private void UpdateDex()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            CloseDex();
        }
    }

    private void UpdateParty()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            CloseParty();
        }
    }

    public void OpenMenu()
    {
        if (State != GameState.WORLD)
            return;

        OldState = State;
        WorldUI.transform.Find("Menu").gameObject.SetActive(true);
        State = GameState.MENU;
    }

    public void CloseMenu()
    {
        WorldUI.transform.Find("Menu").gameObject.SetActive(false);
        State = OldState;
    }

    public void OpenDex()
    {
        if (State != GameState.WORLD)
            return;

        OldState = State;
        DexUI.SetActive(true);
        State = GameState.DEX;
    }

    public void OpenParty()
    {
        if (State != GameState.WORLD)
            return;

        OldState = State;
        PartyUI.SetActive(true);
        Team.PopulateParty();
        Team.RefreshBallInfo();
        State = GameState.PARTY;
    }

    public void CloseParty()
    {
        PartyUI.SetActive(false);
        State = OldState;
    }

    public void CloseDex()
    {
        DexUI.SetActive(false);
        State = OldState;
    }

    public void ShowText(string text)
    {
        if (State == GameState.MENU)
            return;

        OldState = State;
        msgBoxText = text;
        textShowing = "";
        textTime = 0;
        WorldUI.transform.Find("MessageBox").gameObject.SetActive(true);
        WorldUI.transform.Find("MessageBox/Arrow").gameObject.SetActive(false);
        WorldUI.transform.Find("MessageBox/Text").GetComponent<Text>().text = "";
        State = GameState.MESSAGEBOX;
    }

    public void SelectMenuItem(int index)
    {
        CloseMenu();
        switch (index)
        {
            case 0:
                OpenDex();
                break;
            case 1:
                OpenParty();
                break;
            case 2:
                {
                    Debug.Log("Shutting down...");
                    Application.Quit();
                }
                break;
        }
    }

    public void ActivateBattle(Monster monster)
    {
        if (battleManager.BattlePhase == BattleManager.Phase.NONE)
        {
            BattleUI.SetActive(true);
            OldState = State;
            State = GameState.BATTLE;
            battleManager.StartBattle(monster);
        }
    }

    void OnGUI()
    {
        GUIStyle style = GUI.skin.label;

        // version number
        style.fontSize = 10;
        style.fontStyle = FontStyle.Italic;
        style.normal.textColor = Color.white;
        GUI.Label(new Rect(10, Screen.height - 24, Screen.width, Screen.height), version, style);
    }
}
