using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAct : MonoBehaviour
{
    public List<Ability> abilities;

    public float enemyPower;
    public int numberOfAbilitiesToSelect;

    public int atk_Value;

    public int final_atk_value;
    public int atk_Type;
    public string atk_name;

    public Gameplay gameplay;
    public bool collided = false;
    // Start is called before the first frame update

    
    void Start()
    {
        SetValues();
        InitializeAbilities();
        ShowSkillList();
        
    }

    private void SetValues()
    {
        enemyPower = gameplay.enemyPower;
        numberOfAbilitiesToSelect = (int)enemyPower / 10;

        if(numberOfAbilitiesToSelect > 6 )
        {
            numberOfAbilitiesToSelect = 6;
        } //최대 허용 갯수
    }

    Ability ab = new VoidSkill();

    private void InitializeAbilities()
    {
        List<Ability> allAbilities = new List<Ability>(9)
        {
        // 모든 가능한 능력을 여기에 추가
    
        new Break_R(),
        new Break_S(),
        new Break_P(),
        new Onslaught_R(),
        new Onslaught_S(),
        new Onslaught_P()

        //new Ex_Skill()
        };

        abilities = allAbilities.OrderBy(x => Random.value).Take(numberOfAbilitiesToSelect).ToList();
        abilities.Add(new Rush_R());
        abilities.Add(new Rush_S());
        abilities.Add(new Rush_P());
        //list완성

        
        while (abilities.Count < 9)
        {
            abilities.Add(ab); //빈 스킬 추가. 9만들기
        }
    }



    public void SelectAbilities() //gameplay에서 직접 실행.
    {
        ShuffleAbilities(); //섞고
        int index = 0;
        while (abilities[index] == ab)
        {
            index++;
        }
        final_atk_value = abilities[index].getAtkValue(enemyPower); // 1번 스킬 활성화
        atk_Type = abilities[index].getAtkType();
        atk_name = abilities[index].getSkillName();
    }

    private void ShuffleAbilities()
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            Ability temp = abilities[i];
            int randomIndex = Random.Range(0, abilities.Count);
            abilities[i] = abilities[randomIndex];
            abilities[randomIndex] = temp;
        }
    }

    public GameObject enemySkillListUI;
    public TextMeshProUGUI skillList1;
    public TextMeshProUGUI skillList2;
    public TextMeshProUGUI skillList3;
    public TextMeshProUGUI skillList4;
    public TextMeshProUGUI skillList5;
    public TextMeshProUGUI skillList6;
    public TextMeshProUGUI skillList7;
    public TextMeshProUGUI skillList8;
    public TextMeshProUGUI skillList9;

    public void ShowSkillList()
    {
        TextMeshProUGUI[] textComponents = enemySkillListUI.GetComponentsInChildren<TextMeshProUGUI>();

        if (textComponents.Length >= 9)
        {
            skillList1 = textComponents[0];
            skillList2 = textComponents[1];
            skillList3 = textComponents[2];
            skillList4 = textComponents[3];
            skillList5 = textComponents[4];
            skillList6 = textComponents[5];
            skillList7 = textComponents[6];
            skillList8 = textComponents[7];
            skillList9 = textComponents[8];
        }
        else
        {
            Debug.LogError("The instantiated prefab does not contain enough TextMeshProUGUI components.");
        }


        skillList1.text = abilities[0].getSkillName() + " " + abilities[0].getAtkValue(enemyPower);
        skillList2.text = abilities[1].getSkillName() + " " + abilities[1].getAtkValue(enemyPower);
        skillList3.text = abilities[2].getSkillName() + " " + abilities[2].getAtkValue(enemyPower);
        skillList4.text = abilities[3].getSkillName() + " " + abilities[3].getAtkValue(enemyPower);

        if (abilities[4] != ab)
        {
            skillList5.text = abilities[4].getSkillName() + " " + abilities[4].getAtkValue(enemyPower);
            
        }
        else
        {
            skillList5.text = "----------";
        }
        if (abilities[5] != ab)
        {
            skillList6.text = abilities[5].getSkillName() + " " + abilities[5].getAtkValue(enemyPower);
        }
        else
        {
            skillList6.text = "----------";
        }
        if (abilities[6] != ab)
        {
            skillList7.text = abilities[6].getSkillName() + " " + abilities[6].getAtkValue(enemyPower);
        }
        else
        {
            skillList7.text = "----------";
        }
        if (abilities[7] != ab)
        {
            skillList8.text = abilities[7].getSkillName() + " " + abilities[7].getAtkValue(enemyPower);
        }
        else
        {
            skillList8.text = "----------";
        }
        if (abilities[8] != ab)
        {
            skillList9.text = abilities[8].getSkillName() + " " + abilities[8].getAtkValue(enemyPower);
        }
        else
        {
            skillList9.text = "----------";
        }

        enemySkillListUI.SetActive(true);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        collided = true;
    }

}



#region SkillS_Abstract_Class


public abstract class Ability
{
    public abstract int getAtkValue (float enemyPower);
    public abstract int getAtkType ();

    public abstract string getSkillName();
}

public class Rush_R : Ability
{
    public override int getAtkValue(float enemyPower)
    {
        // 추가적인 활성화 로직

        return (int)enemyPower; // 곱하기 1배
    }

    public override int getAtkType()
    {
        return 0;
    }

    public override string getSkillName()
    {
        return "Rush Rock";
    }
}

public class Rush_S : Ability
{
    public override int getAtkValue(float enemyPower)
    {

        return (int)enemyPower; // 곱하기 1배
        // 추가적인 활성화 로직
    }

    public override int getAtkType()
    {
        return 1;
    }

    public override string getSkillName()
    {
        return "Rush Scissor";
    }
}

public class Rush_P : Ability
{
    public override int getAtkValue(float enemyPower)
    {

        return (int)enemyPower; // 곱하기 1배
        // 추가적인 활성화 로직
    }

    public override int getAtkType()
    {
        return 2;
    }
    public override string getSkillName()
    {
        return "Rush Paper";
    }
}

public class Break_R : Ability
{
    public override int getAtkValue(float enemyPower)
    {

        return (int)enemyPower * 2; // 곱하기 2배
        // 추가적인 활성화 로직
    }

    public override int getAtkType()
    {
        return 0;
    }
    public override string getSkillName()
    {
        return "Breaker Rock";
    }
}

public class Break_S : Ability
{
    public override int getAtkValue(float enemyPower)
    {
       return (int)enemyPower * 2;
    }

    public override int getAtkType()
    {
        return 1;
    }
    public override string getSkillName()
    {
        return "Breaker Scissor";
    }
}

public class Break_P : Ability
{
    public override int getAtkValue(float enemyPower)
    {

        return (int)enemyPower * 2; // 곱하기 2배
        // 추가적인 활성화 로직
    }

    public override int getAtkType()
    {
        return 2;
    }
    public override string getSkillName()
    {
        return "Breaker Paper";
    }
}

public class Onslaught_R : Ability
{
    public override int getAtkValue(float enemyPower)
    {

        return (int)enemyPower * 3; // 곱하기 3배
        // 추가적인 활성화 로직
    }

    public override int getAtkType()
    {
        return 0;
    }
    public override string getSkillName()
    {
        return "Onslaught Rock";
    }
}

public class Onslaught_S : Ability
{
    public override int getAtkValue(float enemyPower)
    {

        return (int)enemyPower * 3; // 곱하기 3배
        // 추가적인 활성화 로직
    }

    public override int getAtkType()
    {
        return 1;
    }
    public override string getSkillName()
    {
        return "Onslaught Scissor";
    }
}

public class Onslaught_P : Ability
{
    public override int getAtkValue(float enemyPower)
    {

        return (int)enemyPower * 3; // 곱하기 3배
        // 추가적인 활성화 로직
    }

    public override int getAtkType()
    {
        return 2;
    }
    public override string getSkillName()
    {
        return "Onslaught Paper";
    }
}

public class VoidSkill : Ability
{
    public override int getAtkValue(float enemyPower)
    {

        return (int)enemyPower * 3; // 곱하기 3배
        // 추가적인 활성화 로직
    }

    public override int getAtkType()
    {
        return 0;
    }
    public override string getSkillName()
    {
        return "";
    }
}

#endregion