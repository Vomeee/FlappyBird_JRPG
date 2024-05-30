using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // Image 컴포넌트를 위해 필요
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class Gameplay : MonoBehaviour
{
    public int currentRound; //현재 라운드
    public int PlayerHP = 100; //플레이어 체력
    public float HP_Decrease_multiplier = 1.0f; //체력감소 배율

    public float ATB = 0; //행동 게이지, AbilityPoint.
    public float ATB_Multiplier = 1.0f; //ATB 증가량 배율
    public float ATB_Increase; //실제 상승량.

    public int score;

    public int[] enemyActType = new int[]{1,2,3,4};

    public int turn;

    public Selecter actPanelSelecter; //하단 패널 포인터 클래스.

    public int boost; //부스트.
    public float enemyPower = 10.0f; //적 고유 대미지


    public int playerCondition; //바위 == 0 , 가위 == 1, 보 == 2, 3 --> 특수상태

    public EnemyAct enemyAct; //인스턴스의 클래스 저장용 변수
    public GameObject enemyPrefab; //사용하는 적
    public GameObject enemyInstance; //그 적의 인스턴스

    public GameObject player; //플레이어 오브젝트
    public string playerSkillName;

    public bool collided;


    void Start()
    {
        boost = 1;
        turn = 0;
        currentRound = 0;
        enemyPower = 10.0f;
        //collided = false;
        score = 0;

        TurnStart();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit(); //게임 종료
        }


    }

    #region TurnManagement

    public void TurnStart() //행동 전 해야 할 일
    {
        
        turn++;
        enemyPower += 1.0f;
        collided = false;

        if (enemyInstance == null)
        {
            SpawnEnemy();
        }
        actPanelSelecter.is_being_controlled = true;
        playerCondition = 3;
        
    } //-> ui입력으로

    public Rigidbody2D EnemyRB; //프리팹 할당
    public BoxCollider2D EnemyBoxCollider;
    public Rigidbody2D PlayerRB;

    public GameObject EnemySkillListUI;

    void SpawnEnemy()
    {
        // 적 인스턴스화
        enemyInstance = Instantiate(enemyPrefab, new Vector2(-3, -25 + enemyPower * 0.065f), Quaternion.identity);

        enemyAct = enemyInstance.GetComponent<EnemyAct>();
        EnemyRB = enemyInstance.GetComponent<Rigidbody2D>();
        EnemyBoxCollider = enemyInstance.GetComponent<BoxCollider2D>();
        enemyAct.gameplay = this;

        enemyAct.enemySkillListUI = EnemySkillListUI;

    }

    private bool isRegening;
    private int RegenEndTurn;
    private int HasteEndTurn;
    private int GuardEndTurn;


    public void TurnProcess() //행동 선택후 대결
    {//플레이어는 선택완료
        enemyAct.SelectAbilities();
        

        int enemyAtkValue = enemyAct.final_atk_value; //현재 적의 현재 공격에 대한 공격력
        int enemyAtkType = enemyAct.atk_Type; //현재공격의 타입.
        string enemySkillName = enemyAct.atk_name;

        ShowSkillUI(enemyAtkType, enemySkillName);

       //아이사츠는 닌자들의 예절
       if((enemyAtkType == 0 && playerCondition == 2) || 
            (enemyAtkType == 1 && playerCondition == 0) || 
                (enemyAtkType == 2 && playerCondition == 1)) //플레이어의 승리
       {
            Vector2 currentPlayerPosition = player.transform.position;
            player.transform.position = new Vector2(currentPlayerPosition.x, currentPlayerPosition.y + 0.25f * boost) ; //플레이어 상승.
            ATB += ATB_Multiplier * (29 + boost * 3);
            EnemyRB.velocity = new Vector2(30, -5); //사라져라
            
        }
        
        else if ((enemyAtkType == 0 && playerCondition == 0) ||
             (enemyAtkType == 1 && playerCondition == 1) ||
                 (enemyAtkType == 2 && playerCondition == 2)) //가위바위보 무승부.
        {
            Vector2 currentPlayerPosition = player.transform.position;
            //player.transform.position = new Vector2(currentPlayerPosition.x, currentPlayerPosition.y + 0.5f * boost); //플레이어 상승.
            ATB += ATB_Multiplier * (19 + boost * 3);
            StartCoroutine(WaitForCollide(3f));

            if (player.transform.position.y < enemyInstance.transform.position.y + 24.0)
            {
               PlayerHP -= (int)(HP_Decrease_multiplier * enemyAtkValue);
               player.transform.position = new Vector2(currentPlayerPosition.x, player.transform.position.y - 1.5f);
            }

        }

       else if ((enemyAtkType == 2 && playerCondition == 0) ||
                (enemyAtkType == 1 && playerCondition == 2) ||
                (enemyAtkType == 0 && playerCondition == 1)) //플레이어 패배
       {
            Vector2 currentPlayerPosition = player.transform.position;
            player.transform.position = new Vector2(currentPlayerPosition.x, currentPlayerPosition.y - 0.25f * boost); //플레이어 하강
            ATB += ATB_Multiplier * (9 + boost * 3);
            StartCoroutine(WaitForCollide(2f));

            if (player.transform.position.y < enemyInstance.transform.position.y + 24.0)
            {
                PlayerHP -= (int)(HP_Decrease_multiplier * enemyAtkValue);
                player.transform.position = new Vector2(currentPlayerPosition.x, player.transform.position.y - 1.5f);
                
            }
       }
        else
        {
            Vector2 currentPlayerPosition = player.transform.position;

            if (playerCondition == 3 || playerCondition == 4) //cure, guard
            {
                StartCoroutine(WaitForCollide(2f));
                if (player.transform.position.y < enemyInstance.transform.position.y + 24.0)
                {
                    PlayerHP -= (int)(HP_Decrease_multiplier * enemyAtkValue);
                    player.transform.position = new Vector2(currentPlayerPosition.x, player.transform.position.y - 1.5f);

                }
            }
            else if (playerCondition == 5) //pre-emptive
            {
                player.transform.position = new Vector2(currentPlayerPosition.x, player.transform.position.y + 2f);

                StartCoroutine(WaitForCollide(2f));
                if (player.transform.position.y < enemyInstance.transform.position.y + 24.0)
                {
                    PlayerHP -= (int)(HP_Decrease_multiplier * enemyAtkValue);
                    player.transform.position = new Vector2(currentPlayerPosition.x, player.transform.position.y - 1.5f);

                }
            }

            else if (playerCondition == 6) //Boost
            {

                StartCoroutine(WaitForCollide(2f));
                if (player.transform.position.y < enemyInstance.transform.position.y + 24.0)
                {
                    PlayerHP -= (int)(HP_Decrease_multiplier * enemyAtkValue);
                    player.transform.position = new Vector2(currentPlayerPosition.x, player.transform.position.y - 1f);

                }
            }







        }

       if(isRegening)
       {
            PlayerHP += 20 + boost;
            if (turn == RegenEndTurn) isRegening = false;
       }
       if(turn == HasteEndTurn) 
       {
            ATB_Multiplier = 1f;
       }
       if(turn == GuardEndTurn)
       {
            HP_Decrease_multiplier = 1f;
       }

       if(player.transform.position.y > 4.5)
        {
            player.transform.position = new Vector2(player.transform.position.x, 4.5f); //천장 방지
        }

        currentHPUI.text = PlayerHP.ToString();
        currentATBUI.text = ATB.ToString();
        currentBOOSTUI.text = boost.ToString();

        IsDie(PlayerHP, player.transform.position.y, turn);

        StartCoroutine(WaitForTurnEnd(1f));
    }

    public void IsDie(int hp, float ypos, int turn)
    {
        if(hp <= 0 || ypos < -2.8)
        {
            PlayerPrefs.SetInt("Score", score);
            SceneManager.LoadScene("GameOverScene");
        }
    }

    public TextMeshProUGUI currentHPUI;
    public TextMeshProUGUI currentATBUI;
    public TextMeshProUGUI currentBOOSTUI;



    IEnumerator WaitForCollide(float delay)
    {
        EnemyMove();
        Debug.Log("on");
        collided = enemyAct.collided;
        yield return new WaitForSeconds(delay);
        Debug.Log("off");
        // 3초 후 로직이 필요할 경우 여기에 추가하면 됩니다.
        // 충돌 결과를 확인하는 메소드
    }

    IEnumerator WaitForTurnEnd(float delay)
    {
        yield return new WaitForSeconds(delay);
        TurnEnd();
    }

    
    public void EnemyMove()
    {
        EnemyRB.velocity = new Vector2(-100, -5);
    }

    public GameObject playerSkillUI;
    public GameObject EnemySkillUI;
    public Canvas playerSkillCanvas;
    public Canvas enemySkillCanvas;

    public TextMeshProUGUI playerSkillText;
    public TextMeshProUGUI enemySkillText;
    public void ShowSkillUI(int enemyAtkType, string enemySkillName)
    {
        // 플레이어 스킬 UI 처리
        Image[] playerImages = playerSkillCanvas.GetComponentsInChildren<Image>();
        foreach (Image img in playerImages)
        {
            img.color = playerCondition switch
            {
                0 => Color.red,
                1 => Color.green,
                2 => Color.blue,
                _ => Color.yellow
            };
        }

        

        playerSkillText.text = playerSkillName;

        // 적 스킬 UI 처리
        Image[] enemyImages = enemySkillCanvas.GetComponentsInChildren<Image>();
        foreach (Image img in enemyImages)
        {
            img.color = enemyAtkType switch
            {
                0 => Color.red,
                1 => Color.green,
                2 => Color.blue,
                _ => Color.yellow
            };
        }

        /*TextMeshPro[] enemyTexts = enemySkillCanvas.GetComponentsInChildren<TextMeshPro>();
        foreach (TextMeshPro txt in enemyTexts)
        {
            txt.text = enemySkillName;
        }*/
        enemySkillText.text = enemySkillName;
        StartCoroutine(ShowAndHideUI(3f)); // UI를 보여주고 숨기는 Coroutine 실행
    }

    IEnumerator ShowAndHideUI(float delay)
    {
        playerSkillUI.SetActive(true);
        EnemySkillUI.SetActive(true);
        yield return new WaitForSeconds(delay); // 3초간 대기
        playerSkillUI.SetActive(false);
        EnemySkillUI.SetActive(false);
    }

    public TextMeshProUGUI ScoreText;
    public void TurnEnd() //대결 후
    {
        score += turn;
        score += boost;
        ScoreText.text = "Score  " + score;
        

        StartCoroutine(JustWait(2f));
        if (enemyInstance != null)
        {
            Destroy(enemyInstance);
        }
    }

    IEnumerator JustWait(float delay)
    {
        yield return new WaitForSeconds(delay); // 3초간 대기
        Debug.Log("11234");
        
        TurnStart();
    }

    #endregion

    #region PlayerAct

    public void Jump_Rock()
    {
        playerCondition = 0;
        playerSkillName = "Jump Rock";
        TurnProcess();
    }
    public void Jump_Scissor()
    {
        playerCondition = 1;
        playerSkillName = "Jump Scissor";
        TurnProcess();
    }
    public void Jump_Paper()
    {
        playerCondition = 2;
        playerSkillName = "Jump Paper";
        TurnProcess();
    }

    public void Stay()
    {
        playerCondition = 3;
        player.transform.position = new Vector2(player.transform.position.x, player.transform.position.y - 0.3f);
        playerSkillName = "Stay";
        TurnProcess();
    }

    public void Ability_Guard()
    {
        ATB -= 50;
        playerSkillName = "Guard";      
        HP_Decrease_multiplier = 0.5f;
        GuardEndTurn = turn + 5;
        playerCondition = 3;
        TurnProcess();


    }
    

    public void Ability_Cure()
    {
        playerSkillName = "Cure";
        ATB -= 60;
        PlayerHP += 50 + boost * 5;

        playerCondition = 4;
        TurnProcess();
    }

    public void Ability_PreEmptive()
    {
        playerSkillName = "Pre-Emptive";
        ATB -= 60;
        //player.transform.position = new Vector2(player.transform.position.x, player.transform.position.y + 3f);
        playerCondition = 5;
        TurnProcess();
    }

    public void Ability_Boost()
    {
        playerSkillName = "Boost";
        ATB -= 30;
        boost++;
        playerCondition = 6;
        TurnProcess();
    }

    public void Ability_Regen()
    {
        playerSkillName = "Regen";
        ATB -= 70;
        playerCondition = 6;
        TurnProcess();
        isRegening = true;
        RegenEndTurn = turn + 5;
    }

    public void Ability_Haste()
    {
        playerSkillName = "Haste";
        ATB -= 100;
        playerCondition = 6;
        ATB_Multiplier = 2.0f;
        TurnProcess();
        HasteEndTurn = turn + 5;
    }
    #endregion

    #region enemyAct





    #endregion
}
