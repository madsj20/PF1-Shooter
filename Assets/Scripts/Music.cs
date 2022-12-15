using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using static Unity.VisualScripting.Member;

namespace QuickStart
{
    [RequireComponent(typeof(AudioSource))]
    public class Music : NetworkBehaviour
    {
        public AudioSource musicmann;

        public AudioClip music;

        void Start()
        {
            startmusic();
        }

        [Command]
        private void startmusic()
        {
            RpcSennd();
        }
        [ClientRpc]
        private void RpcSennd()
        {
            musicmann.clip = music;
            musicmann.Play();
        }


    }
}