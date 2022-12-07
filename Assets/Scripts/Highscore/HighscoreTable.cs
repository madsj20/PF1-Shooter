using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;




public class HighscoreTable : MonoBehaviour
{

    private Transform entryContainer;
    private Transform entryTemplate;

    private List<string> names = new List<string>();
    private List<string> scores = new List<string>();

    private float templateHeight = 40f;
    private int i;
   
    
    string Tname;
    


    private void Start() 
    {
        
        names = this.GetComponent<DBScript>().names;
        scores = this.GetComponent<DBScript>().scores;

        //finds the container and the template for the scores
        entryContainer = transform.Find("entryParent");
        entryTemplate = entryContainer.Find("entryTemplate");

        //hides the og template cuz its ugly
        entryTemplate.gameObject.SetActive(false);


        
        //to make only 5 scores show
        for (i = 0; i < names.Count; i++)
        {
            //we make a clone of the og template here 
            Transform entryTransform = Instantiate(entryTemplate, entryContainer);

            //gets new clones transform height
            RectTransform entryRectTranform = entryTransform.GetComponent<RectTransform>();

            //places it on the ancored position with a given space between
            entryRectTranform.anchoredPosition = new Vector2(0, -templateHeight * i);

            //shows the new entry beacuse the original is hidden, so the clone was too
            entryTransform.gameObject.SetActive(true);

            //we do this cause we dont want the rank 0 to be a thing
            int Nscore = i + 1;

            //we a do a little naming of the ranks
            string rank;
            switch (Nscore)
            {
                case 1:
                    rank = "1st"; 
                    break;
                case 2:
                    rank = "2nd";
                    break;
                case 3:
                    rank = "3rd";
                    break;
                default:
                    rank = Nscore + "th";
                    break;


            }


            //Let us change the text lads
            entryTransform.Find("posText").GetComponent<TMP_Text>().text = rank;

            
            entryTransform.Find("scoreText").GetComponent<TMP_Text>().text = scores[i];
            entryTransform.Find("nameText").GetComponent<TMP_Text>().text = names[i];


        }


    }
}
