using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // Image ������Ʈ�� ���� �ʿ�
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class Gameplay : MonoBehaviour
{
    public int currentRound; //���� ����
    public int PlayerHP = 100; //�÷��̾� ü��
    public float HP_Decrease_multiplier = 1.0f; //ü�°��� ����

    public float ATB = 0; //�ൿ ������, AbilityPoint.
    public float ATB_Multiplier = 1.0f; //ATB ������ ����
    public float ATB_Increase; //���� ��·�.

    public int score;

    public int[] enemyActType = new int[]{1,2,3,4};

    public int turn;

    public Selecter actPanelSelecter; //�ϴ� �г� ������ Ŭ����.

    public int boost; //�ν�Ʈ.
    public float enemyPower = 10.0f; //�� ���� �����


    public int playerCondition; //���� == 0 , ���� == 1, �� == 2, 3 --> Ư������

    public EnemyAct enemyAct; //�ν��Ͻ��� Ŭ���� ����� ����
    public GameObject enemyPrefab; //����ϴ� ��
    public GameObject enemyInstance; //�� ���� �ν��Ͻ�

    public GameObject player; //�÷��̾� ������Ʈ
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
            Application.Quit(); //���� ����
        }


    }

    #region TurnManagement

    public void TurnStart() //�ൿ �� �ؾ� �� ��
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
        
    } //-> ui�Է�����

    public Rigidbody2D EnemyRB; //������ �Ҵ�
    public BoxCollider2D EnemyBoxCollider;
    public Rigidbody2D PlayerRB;

    public GameObject EnemySkillListUI;

    void SpawnEnemy()
    {
        // �� �ν��Ͻ�ȭ
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


    public void TurnProcess() //�ൿ ������ ���
    {//�÷��̾�� ���ÿϷ�
        enemyAct.SelectAbilities();
        

        int enemyAtkValue = enemyAct.final_atk_value; //���� ���� ���� ���ݿ� ���� ���ݷ�
        int enemyAtkType = enemyAct.atk_Type; //��������� Ÿ��.
        string enemySkillName = enemyAct.atk_name;

        ShowSkillUI(enemyAtkType, enemySkillName);

       //���̻����� ���ڵ��� ����
       if((enemyAtkType == 0 && playerCondition == 2) || 
            (enemyAtkType == 1 && playerCondition == 0) || 
                (enemyAtkType == 2 && playerCondition == 1)) //�÷��̾��� �¸�
       {
            Vector2 currentPlayerPosition = player.transform.position;
            player.transform.position = new Vector2(currentPlayerPosition.x, currentPlayerPosition.y + 0.25f * boost) ; //�÷��̾� ���.
            ATB += ATB_Multiplier * (29 + boost * 3);
            EnemyRB.velocity = new Vector2(30, -5); //�������
            
        }
        
        else if ((enemyAtkType == 0 && playerCondition == 0) ||
             (enemyAtkType == 1 && playerCondition == 1) ||
                 (enemyAtkType == 2 && playerCondition == 2)) //���������� ���º�.
        {
            Vector2 currentPlayerPosition = player.transform.position;
            //player.transform.position = new Vector2(currentPlayerPosition.x, currentPlayerPosition.y + 0.5f * boost); //�÷��̾� ���.
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
                (enemyAtkType == 0 && playerCondition == 1)) //�÷��̾� �й�
       {
            Vector2 currentPlayerPosition = player.transform.position;
            player.transform.position = new Vector2(currentPlayerPosition.x, currentPlayerPosition.y - 0.25f * boost); //�÷��̾� �ϰ�
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
            player.transform.position = new Vector2(player.transform.position.x, 4.5f); //õ�� ����
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
        // 3�� �� ������ �ʿ��� ��� ���⿡ �߰��ϸ� �˴ϴ�.
        // �浹 ����� Ȯ���ϴ� �޼ҵ�
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
        // �÷��̾� ��ų UI ó��
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

        // �� ��ų UI ó��
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
        StartCoroutine(ShowAndHideUI(3f)); // UI�� �����ְ� ����� Coroutine ����
    }

    IEnumerator ShowAndHideUI(float delay)
    {
        playerSkillUI.SetActive(true);
        EnemySkillUI.SetActive(true);
        yield return new WaitForSeconds(delay); // 3�ʰ� ���
        playerSkillUI.SetActive(false);
        EnemySkillUI.SetActive(false);
    }

    public TextMeshProUGUI ScoreText;
    public void TurnEnd() //��� ��
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
        yield return new WaitForSeconds(delay); // 3�ʰ� ���
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
