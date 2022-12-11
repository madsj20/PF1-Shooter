using UnityEngine;
using Mirror;
using System.Collections.Generic;
using TMPro;

 
namespace QuickStart
{
public class ScoreController : NetworkBehaviour

{
    
    // Dictionary to store the scores for each player
    public SyncDictionary<string, int> scores = new SyncDictionary<string, int>();
    // Reference to the UI text field that will display the scores
    public TMP_Text scoresText;
}
}