using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selecter_jump : MonoBehaviour
{
    public Selecter act_selecter;
    public RectTransform recttransform;

    public Gameplay gameplay;

    public GameObject this_panel;

    public int[] posYs = new int[3] { 323, 243, 163 };

    public int currselection;
    // Start is called before the first frame update
    void Start()
    {
        currselection = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (currselection == 0) { }
            else
            {
                currselection--;
                recttransform.anchoredPosition = new Vector2(257, posYs[currselection]);
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (currselection == 2) { }
            else
            {
                currselection++;
                recttransform.anchoredPosition = new Vector2(257, posYs[currselection]);
            }

        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (currselection == 0)
            {
                gameplay.Jump_Rock();

                
            }
            else if (currselection == 1)
            {
                gameplay.Jump_Scissor();

                
            }
            else if (currselection == 2)
            {
                gameplay.Jump_Paper();

                
            }

            
            this_panel.SetActive(false);
        }
    }
}
