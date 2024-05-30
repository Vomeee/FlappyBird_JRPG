using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class Selecter_ability : MonoBehaviour
{
    // Start is called before the first frame update
    public RectTransform recttransform;
    int[] posYs = new int[] {305, 225, 145, 65, -15, -95, -175};
    public int currselection;
    public Gameplay gameplay;

    public GameObject OriginalUI;

    public Selecter act_selecter;

    public TextMeshProUGUI SkillInstructiontext;


    void Start()
    {
        recttransform.anchoredPosition = new Vector2(150, 305);
        currselection = 0;
    }

    void setSkillInstruction(int curr_selection)
    {
        if(curr_selection == 0) //guard
        {
            SkillInstructiontext.text = "Reduce damage taken for 5turns";
        }
        else if (curr_selection == 1) //cure
        {
            SkillInstructiontext.text = "Heal, Relies on Boost. ";
        }
        else if (curr_selection == 2) //Preemptive
        {
            SkillInstructiontext.text = "Jump high before enemy attack. \n Relies on boost.";
        }
        else if (curr_selection == 3) //Boost
        {
            SkillInstructiontext.text = "Boost + 1, Score up. \n Movement Up. Heal Multiplies.";
        }
        else if (curr_selection == 4) //Regen
        {
            SkillInstructiontext.text = "Heal for 5turns, \n Relies on Boost.";
        }
        else if(curr_selection == 5)
        {
            SkillInstructiontext.text = "Magnify AP taken for 5 turns.";
        }
        else
        {
            SkillInstructiontext.text = "Get back to main selection.";
        }
    }

    // Update is called once per frame
    void Update()
    {
        float currentATB = gameplay.ATB;
        if(Input.GetKeyDown(KeyCode.W))
        {
            if(currselection == 0) { }
            else
            {
                currselection--;
                setSkillInstruction(currselection);
                recttransform.anchoredPosition = new Vector2(150, posYs[currselection]);
            }
        }

        if(Input.GetKeyDown(KeyCode.S))
        {
            if(currselection == 6) { }
            else
            {
                currselection++;
                setSkillInstruction(currselection);
                recttransform.anchoredPosition = new Vector2(150, posYs[currselection]);
            }

        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if(currselection == 0 && currentATB >= 50)
            {
                gameplay.Ability_Guard();

                
            }
            else if(currselection == 1 && currentATB >= 60) 
            {
                gameplay.Ability_Cure();

                
            }
            else if (currselection == 2 && currentATB >= 70)
            {
                gameplay.Ability_PreEmptive();

                
            }
            else if (currselection == 3 && currentATB >= 30)
            {
                gameplay.Ability_Boost();

                
            }
            else if (currselection == 4 && currentATB >= 70)
            {
                gameplay.Ability_Regen();

                
            }
            else if (currselection == 5 && currentATB >= 100)
            {
                gameplay.Ability_Haste();

                
            }
            else
            {
                act_selecter.is_being_controlled = true;
            }

            //act_selecter.is_being_controlled = true;
            OriginalUI.SetActive(false);
        }
    }


}
