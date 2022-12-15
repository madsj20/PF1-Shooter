using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemySounds : MonoBehaviour
{
    public AudioSource bruh;

    public AudioClip[] bruhs;
    
    void Start()
    {
        StartCoroutine(playSound());
        
    }

    
    IEnumerator playSound()
    {
        Debug.Log("started");
        int waits = Random.Range(5,30);
        int randomsound = Random.Range(0, 3);
        yield return new WaitForSeconds(waits);
        bruh.clip = bruhs[randomsound];
        bruh.Play();
        Debug.Log("finnished");
        StartCoroutine(playSound());

    }


    void Update()
    {
        
    }
}
