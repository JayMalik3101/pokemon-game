using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public enum Phase
    {
        NONE,
        INTRO,
        INTRO_CALL,
        PLAYER_TURN,
        ENEMY_TURN,
        CATCH_INTRO,
        CATCH_ATTEMPT,
        CATCH_SUCCESS,
        WIN,
        LOSE,
        ESCAPE,
        END
    }
    public Phase BattlePhase;
    [HideInInspector]
    public bool StickyMessageBox;

    private Monster Enemy;
    private Monster PlayerMonster;
    private GameObject EnemyObj;

    private Animator ThrowAnimation;

    private bool isAttacking;
    private bool isDefending;
    private bool msgWait = false;
    private bool ballThrown = false;
    private float WiggleTime, wiggleTimeNow;
    private bool willCatch = false;
    private bool playerSwitchPokemon = false;

    [Header("Battle Messages")]
    public string[] Messages;

    // Use this for initialization
    void Start()
    {
        BattlePhase = Phase.NONE;
    }

    public void UpdateBattle()
    {
        switch (BattlePhase)
        {
            case Phase.INTRO:
                if (!msgWait)
                {
                    GameManager.instance.DexUI.GetComponent<Pokedex>().See(Enemy.Name);
                    GameManager.instance.ShowText(FormatText(Messages[0]));
                    msgWait = true;
                }
                break;
            case Phase.INTRO_CALL:
                GameManager.instance.BattleUI.transform.Find("MyPokemon").GetComponent<Image>().sprite = PlayerMonster.TextureBack;
                GameManager.instance.BattleUI.transform.Find("MyHealth").gameObject.SetActive(true);
                GameManager.instance.BattleUI.transform.Find("MyHealth/MaxHP").GetComponent<Text>().text = PlayerMonster.Health.ToString();
                GameManager.instance.BattleUI.transform.Find("MyHealth/CurHP").GetComponent<Text>().text = PlayerMonster.CurrentHealth.ToString();
                GameManager.instance.BattleUI.transform.Find("MyHealth/Name").GetComponent<Text>().text = PlayerMonster.Name;
                GameManager.instance.BattleUI.transform.Find("CommandBox").gameObject.SetActive(true);
                GameManager.instance.BattleUI.transform.Find("MyHealth/Life").GetComponent<Image>().fillAmount = (float)PlayerMonster.CurrentHealth / (float)PlayerMonster.Health;

                if (Enemy.Agility > PlayerMonster.Agility)
                    BattlePhase = Phase.ENEMY_TURN;
                else
                    BattlePhase = Phase.PLAYER_TURN;
                break;
            case Phase.PLAYER_TURN:
                if (PlayerMonster.CurrentHealth <= 0)
                {
                    ballThrown = false;
                    isAttacking = false;
                    if (GameManager.instance.Team.Team.Count > 1)
                    {
                        bool gameOver = true;
                        for (int p = 0; p < GameManager.instance.Team.Team.Count; p++)
                        {
                            if (GameManager.instance.Team.Team[p].CurrentHealth > 0)
                            {
                                playerSwitchPokemon = true;
                                GameManager.instance.ShowText(FormatText(Messages[9], GameManager.instance.Team.Team[p]));
                                PlayerMonster = GameManager.instance.Team.Team[p];
                                gameOver = false;
                                break;
                            }
                        }
                        if (gameOver)
                        {
                            GameManager.instance.ShowText(FormatText(Messages[10]));
                        }
                    }
                    else
                    {
                        GameManager.instance.ShowText(FormatText(Messages[10]));
                    }
                }
                break;
            case Phase.ENEMY_TURN:
                if (Enemy.CurrentHealth <= 0)
                {
                    GameManager.instance.BattleUI.transform.Find("EnemyPokemon").GetComponent<Image>().sprite = null;
                    GameManager.instance.ShowText(FormatText(Messages[11]));
                    break;
                }
                GameManager.instance.BattleUI.transform.Find("CommandBox").gameObject.SetActive(false);
                GameManager.instance.ShowText(FormatText(Messages[1]));
                isAttacking = true;
                break;
            case Phase.ESCAPE:
                GameManager.instance.ShowText(FormatText(Messages[2]));
                break;
            case Phase.CATCH_INTRO:
                ThrowAnimation.SetBool("IsThrown", true);
                DetermineWiggleTime();
                NextPhase();
                break;
            case Phase.CATCH_ATTEMPT:
                if (ThrowAnimation.GetCurrentAnimatorStateInfo(0).IsName("Waiting"))
                {
                    GameManager.instance.BattleUI.transform.Find("EnemyPokemon").GetComponent<Image>().sprite = null;
                }
                wiggleTimeNow += Time.deltaTime;
                if (wiggleTimeNow > WiggleTime)
                {
                    if (willCatch)
                    {
                        GameManager.instance.ShowText(FormatText(Messages[6]));
                        ThrowAnimation.SetBool("IsCaught", true);
                    }
                    else
                    {
                        ResetBallAnim();
                        GameManager.instance.BattleUI.transform.Find("EnemyPokemon").GetComponent<Image>().sprite = Enemy.TextureFront;
                        GameManager.instance.ShowText(FormatText(Messages[7]));
                    }
                }
                break;
            case Phase.CATCH_SUCCESS:
                if (!msgWait)
                {
                    ResetBallAnim();
                    GameManager.instance.ShowText(FormatText(Messages[8]));
                    msgWait = true;
                }
                break;
            case Phase.WIN:
                if (!msgWait)
                {
                    GameManager.instance.ShowText(FormatText(Messages[12]));
                    msgWait = true;
                }
                break;
            case Phase.LOSE:
                if (!msgWait)
                {
                    GameManager.instance.ShowText(FormatText(Messages[13]));
                    msgWait = true;
                }
                break;
        }
    }

    public void NextPhase()
    {
        switch (BattlePhase)
        {
            case Phase.INTRO:
                msgWait = false;
                BattlePhase = Phase.INTRO_CALL;
                break;
            case Phase.PLAYER_TURN:
                if (PlayerMonster.CurrentHealth <= 0)
                {
                    ballThrown = false;
                    StickyMessageBox = false;
                    isAttacking = false;
                    isDefending = false;
                    GameManager.instance.BattleUI.transform.Find("CommandBox").gameObject.SetActive(false);
                    BattlePhase = Phase.LOSE;
                    break;
                }
                else
                {
                    if (isAttacking)
                    {
                        Enemy.CurrentHealth -= Mathf.Clamp(PlayerMonster.Attack - Enemy.Defense, 0, int.MaxValue);
                        GameManager.instance.BattleUI.transform.Find("TheirHealth/Life").GetComponent<Image>().fillAmount = (float)Enemy.CurrentHealth / (float)Enemy.Health;
                        isAttacking = false;
                    }

                    if (ballThrown)
                        BattlePhase = Phase.CATCH_INTRO;
                    else
                        BattlePhase = Phase.ENEMY_TURN;
                    break;
                }
            case Phase.ENEMY_TURN:
                if (Enemy.CurrentHealth <= 0)
                {
                    ballThrown = false;
                    StickyMessageBox = false;
                    isAttacking = false;
                    isDefending = false;
                    GameManager.instance.BattleUI.transform.Find("CommandBox").gameObject.SetActive(false);
                    BattlePhase = Phase.WIN;
                    break;
                }
                else
                {
                    if (isAttacking)
                    {
                        if (isDefending)
                            PlayerMonster.CurrentHealth -= Mathf.Clamp(Enemy.Attack - (Enemy.Defense * 2), 0, int.MaxValue);
                        else
                            PlayerMonster.CurrentHealth -= Mathf.Clamp(Enemy.Attack - PlayerMonster.Defense, 0, int.MaxValue);
                        if (PlayerMonster.CurrentHealth < 0)
                            PlayerMonster.CurrentHealth = 0;
                        GameManager.instance.BattleUI.transform.Find("MyHealth/Life").GetComponent<Image>().fillAmount = (float)PlayerMonster.CurrentHealth / (float)PlayerMonster.Health;
                        UpdateHealth();
                        isAttacking = false;
                    }

                    GameManager.instance.BattleUI.transform.Find("CommandBox").gameObject.SetActive(true);
                    isDefending = false;
                    if (playerSwitchPokemon)
                        BattlePhase = Phase.INTRO_CALL;
                    else
                        BattlePhase = Phase.PLAYER_TURN;
                    break;
                }
            case Phase.ESCAPE:
                EndBattle();
                break;
            case Phase.CATCH_INTRO:
                GameManager.instance.StartingPokeballs--;
                BattlePhase = Phase.CATCH_ATTEMPT;
                break;
            case Phase.CATCH_ATTEMPT:
                if (wiggleTimeNow > WiggleTime)
                {
                    StickyMessageBox = false;
                    if (willCatch)
                    {
                        ThrowAnimation.SetBool("IsCaught", true);
                        BattlePhase = Phase.CATCH_SUCCESS;
                    }
                    else
                    {
                        ballThrown = false;
                        ResetBallAnim();
                        BattlePhase = Phase.ENEMY_TURN;
                    }
                }
                break;
            case Phase.CATCH_SUCCESS:
                ballThrown = false;
                ResetBallAnim();
                GameManager.instance.Team.Add(Enemy);
                EndBattle();
                break;
            case Phase.WIN:
            case Phase.LOSE:
                EndBattle();
                break;
        }
    }

    public void StartBattle(Monster monster)
    {
        FixButtons();
        ThrowAnimation = GameManager.instance.BattleUI.transform.Find("Pokeball").GetComponent<Animator>();

        EnemyObj = new GameObject();
        EnemyObj.AddComponent<Monster>();
        Enemy = EnemyObj.GetComponent<Monster>();
        Enemy.Name = monster.Name;
        Enemy.TextureFront = monster.TextureFront;
        Enemy.TextureBack = monster.TextureBack;
        Enemy.CurrentHealth = monster.Health;
        Enemy.Health = monster.Health;
        Enemy.Attack = monster.Attack;
        Enemy.Defense = monster.Defense;
        Enemy.Agility = monster.Agility;
        Enemy.CatchRate = monster.CatchRate;

        isDefending = false;
        PlayerMonster = GameManager.instance.Team.Team[0];
        GameManager.instance.BattleUI.transform.Find("MyHealth").gameObject.SetActive(false);

        Enemy.CurrentHealth = Enemy.Health;


        GameManager.instance.BattleUI.transform.Find("EnemyPokemon").GetComponent<Image>().sprite = Enemy.TextureFront;
        GameManager.instance.BattleUI.transform.Find("MyPokemon").GetComponent<Image>().sprite = GameManager.instance.PlayerBackSprite;
        GameManager.instance.BattleUI.transform.Find("TheirHealth").gameObject.SetActive(true);
        GameManager.instance.BattleUI.transform.Find("TheirHealth/Name").GetComponent<Text>().text = Enemy.Name;
        GameManager.instance.BattleUI.transform.Find("TheirHealth/Life").GetComponent<Image>().fillAmount = 1f;
        BattlePhase = Phase.INTRO;
    }

    private void FixButtons()
    {
        GameManager.instance.BattleUI.transform.Find("CommandBox").gameObject.SetActive(true);
        GameObject cmdBox = GameManager.instance.BattleUI.transform.Find("CommandBox/Panel").gameObject;
        // reset all
        foreach (Transform obj in cmdBox.transform)
        {
            if (obj.GetComponent<Button>())
                obj.GetComponent<Button>().onClick.RemoveAllListeners();
        }

        // fix all
        foreach (Transform obj in cmdBox.transform)
        {
            if (obj.name.Equals("Attack"))
                obj.GetComponent<Button>().onClick.AddListener(() => Attack());
            if (obj.name.Equals("Defend"))
                obj.GetComponent<Button>().onClick.AddListener(() => Defend());
            if (obj.name.Equals("Throw"))
                obj.GetComponent<Button>().onClick.AddListener(() => Throwball());
            if (obj.name.Equals("Run"))
                obj.GetComponent<Button>().onClick.AddListener(() => Run());
        }
        GameManager.instance.BattleUI.transform.Find("CommandBox").gameObject.SetActive(false);
    }

    public void EndBattle()
    {
        GameManager.instance.DexUI.GetComponent<Pokedex>().Refresh();
        GameManager.instance.BattleUI.SetActive(false);
        GameManager.instance.BattleUI.transform.Find("CommandBox").gameObject.SetActive(false);
        GameManager.instance.State = GameManager.GameState.WORLD;
        BattlePhase = Phase.NONE;
        isDefending = false;
        msgWait = false;
    }

    private void UpdateHealth()
    {
        GameManager.instance.BattleUI.transform.Find("MyHealth/CurHP").GetComponent<Text>().text = PlayerMonster.CurrentHealth.ToString();
    }

    public void Attack()
    {
        GameManager.instance.BattleUI.transform.Find("CommandBox").gameObject.SetActive(false);
        GameManager.instance.ShowText(FormatText(Messages[3]));
        isAttacking = true;
    }

    private string FormatText(string text, Monster next = null)
    {
        // insert enemy name
        string replaceOne = text.Replace("%e", Enemy.Name);

        // insert player name
        string result = replaceOne.Replace("%p", PlayerMonster.Name);

        if (next != null)
        {
            string replaceTwo = result.Replace("%p2", next.Name);
            return replaceTwo;
        }

        return result;
    }

    public void Defend()
    {
        GameManager.instance.BattleUI.transform.Find("CommandBox").gameObject.SetActive(false);
        GameManager.instance.ShowText(FormatText(Messages[4]));
        isDefending = true;
    }

    public void Throwball()
    {
        StickyMessageBox = true;
        GameManager.instance.BattleUI.transform.Find("CommandBox").gameObject.SetActive(false);
        GameManager.instance.ShowText(FormatText(Messages[5]));
        ballThrown = true;
    }

    public void Run()
    {
        BattlePhase = Phase.ESCAPE;
    }

    private void ResetBallAnim()
    {
        ThrowAnimation.SetBool("IsThrown", false);
        ThrowAnimation.SetBool("IsCaught", false);
        ThrowAnimation.GetComponent<RectTransform>().anchoredPosition = new Vector2(-900, -170);
    }

    private void DetermineWiggleTime()
    {
        wiggleTimeNow = 0;
        float chance = Random.Range(0, 101f);
        float catchRate = Enemy.CatchRate;
        if (chance < catchRate)
        {
            WiggleTime = 6;
            willCatch = true;
        }
        else
        {
            WiggleTime = Random.Range(2, 6);
            willCatch = false;
        }
    }
}
