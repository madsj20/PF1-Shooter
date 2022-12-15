using QuickStart;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Examples.AdditiveScenes;


namespace QuickStart
{

    [RequireComponent(typeof(AudioSource))]
    
    public class playersounds : NetworkBehaviour
    {

        private AudioSource source;
        public AudioClip hurt;
        public AudioClip death;
        public AudioClip pew;
        public AudioClip change;
        
    

        
        void Start()
        {
            source = this.gameObject.AddComponent<AudioSource>();
            source.spatialBlend = 1f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.maxDistance = 50f;



        }

        [Command]
        public void CmdSend()
        {
            RpcSennd();
        }
        [ClientRpc]
        public void RpcSennd()
        {
            source.clip = change;
            source.Play();
        }

        [Command]
        public void CmdPew()
        {
            RpcPew();
        }
        [ClientRpc]
        public void RpcPew()
        {
            source.clip = pew;
            source.Play();
        }

        [Command]
        public void CmdAuchie()
        {
            RpcAuchie();
        }
        [ClientRpc]
        public void RpcAuchie()
        {
            source.clip = hurt;
            source.Play();
        }

        [Command]
        public void CmdDeath()
        {
            RpcDeath();
        }
        [ClientRpc]
        public void RpcDeath()
        {
            source.clip = death;
            source.Play();
        }


    }
}