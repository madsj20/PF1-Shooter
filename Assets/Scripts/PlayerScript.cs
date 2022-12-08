using Mirror;
using UnityEngine;
using TMPro;

namespace QuickStart
{
    public class PlayerScript : NetworkBehaviour
    {
        public TextMeshPro playerNameText;
        public GameObject namePlate;

        private Material playerMaterialClone;

        [SyncVar(hook = nameof(OnNameChanged))]
        public string playerName;

        [SyncVar(hook = nameof(OnColorChanged))]
        public Color playerColor = Color.white;

        private int selectedWeaponLocal = 1;
        public GameObject[] weaponArray;

        [SyncVar(hook = nameof(OnWeaponChanged))]
        public int activeWeaponSynced = 1;

        private SceneScript sceneScript;

        private Weapon activeWeapon;
        private float weaponCooldownTime;

        private float speed = 1;
        private float walkingSpeed = 1;
        private float runningSpeed = 2.5f;

        public GameObject[] objectsToHide;
        [SyncVar(hook = nameof(OnChanged))]
        public bool isDead = false;

        [SyncVar] public int health = 4;


        void Awake()
        {
            //allows all players to run this
            sceneScript = GameObject.Find("SceneReference").GetComponent<SceneReference>().sceneScript;
            // disable all weapons
            foreach (var item in weaponArray)
                if (item != null)
                    item.SetActive(false);

            if (selectedWeaponLocal < weaponArray.Length && weaponArray[selectedWeaponLocal] != null)
            {
                activeWeapon = weaponArray[selectedWeaponLocal].GetComponent<Weapon>();
                sceneScript.UIAmmo(activeWeapon.weaponAmmo);
            }
        }

        void Update()
        {
            if (!isLocalPlayer)
            {
                // make non-local players run this
                // makes the namePlate always be readable by the other players
                namePlate.transform.LookAt(Camera.main.transform);
                return;
            }
            if (sceneScript.readyStatus != 0)
            {
                float moveX = Input.GetAxis("Horizontal") * Time.deltaTime * 110.0f;
                float moveZ = Input.GetAxis("Vertical") * Time.deltaTime * 4f * speed;

                transform.Rotate(0, moveX, 0);
                transform.Translate(0, 0, moveZ);

                if (Input.GetMouseButtonDown(1)) //Right mouse button to change weapon
                {
                    selectedWeaponLocal += 1;

                    if (selectedWeaponLocal > weaponArray.Length)
                        selectedWeaponLocal = 1;

                    CmdChangeActiveWeapon(selectedWeaponLocal);
                }


                if (Input.GetMouseButtonDown(0)) //Left mouse button to shoot
                {
                    if (activeWeapon && Time.time > weaponCooldownTime && activeWeapon.weaponAmmo > 0)
                    {
                        weaponCooldownTime = Time.time + activeWeapon.weaponCooldown;
                        activeWeapon.weaponAmmo -= 1;
                        sceneScript.UIAmmo(activeWeapon.weaponAmmo);
                        CmdShootRay();
                    }
                }

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    Debug.Log("Pressed Shift");
                    // Set current speed to run if shift is down
                    speed = runningSpeed;
                }
                else
                {
                    // Otherwise set current speed to walking speed
                    speed = walkingSpeed;
                }
            }
        }

        [Command]
        void CmdShootRay()
        {
            RpcFireWeapon();
        }

        [ClientRpc]
        void RpcFireWeapon()
        {
            //bulletAudio.Play();
            GameObject bullet = Instantiate(activeWeapon.weaponBullet, activeWeapon.weaponFirePosition.position, activeWeapon.weaponFirePosition.rotation);
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * activeWeapon.weaponSpeed;
            Destroy(bullet, activeWeapon.weaponLife);
        }

        void OnNameChanged(string _Old, string _New)
        {
            playerNameText.text = playerName;
        }

        void OnColorChanged(Color _Old, Color _New)
        {
            playerNameText.color = _New;
            playerMaterialClone = new Material(GetComponent<Renderer>().material);
            playerMaterialClone.color = _New;
            GetComponent<Renderer>().material = playerMaterialClone;
        }

        public override void OnStartLocalPlayer()
        {
            sceneScript.playerScript = this;
            sceneScript.SetupScene();

            Camera.main.transform.SetParent(transform);
            Camera.main.transform.localPosition = new Vector3(0, 0.65f, 0);

            namePlate.transform.localPosition = new Vector3(0, -0.3f, 0.6f);
            namePlate.transform.Rotate(0, 0, 180);
            namePlate.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            string name = "Player" + Random.Range(100, 999);
            Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            CmdSetupPlayer(name, color);
        }

        [Command]
        public void CmdSetupPlayer(string _name, Color _col)
        {
            playerName = _name;
            playerColor = _col;
            sceneScript.statusText = $"{playerName} joined.";
        }

        void OnWeaponChanged(int _Old, int _New)
        {
            // disable old weapon
            // in range and not null
            if (0 < _Old && _Old < weaponArray.Length && weaponArray[_Old] != null)
                weaponArray[_Old].SetActive(false);

            // enable new weapon
            // in range and not null
            if (0 < _New && _New < weaponArray.Length && weaponArray[_New] != null)
            {
                weaponArray[_New].SetActive(true);
                activeWeapon = weaponArray[activeWeaponSynced].GetComponent<Weapon>();
                if (isLocalPlayer)
                {
                    sceneScript.UIAmmo(activeWeapon.weaponAmmo);
                }
            }
        }

        [Command]
        public void CmdChangeActiveWeapon(int newIndex)
        {
            activeWeaponSynced = newIndex;
        }

        [Command]
        public void CmdSendPlayerMessage()
        {
            if (sceneScript)
                sceneScript.statusText = $"{playerName} says hello {Random.Range(10, 99)}";
        }
        
        void OnChanged(bool _Old, bool _New)
        {
            if (isDead == false)
            {
                    foreach(var obj in objectsToHide)
                {
                        obj.SetActive(true);
                }

                if (isLocalPlayer)
                {
                    this.transform.position = NetworkManager.startPositions[Random.Range(0, NetworkManager.startPositions.Count)].position;
                }
                sceneScript.statusText = "Player Respawned";
            }
            else if (isDead == true)
            {
                foreach (var obj in objectsToHide)
                {
                    obj.SetActive(false);
                }
                sceneScript.statusText = "Player Defeated";
            }
            if (isLocalPlayer)
            {
                sceneScript.SetupScene();
            }
        }  

        [Command]
        public void CmdPlayerStatus(bool _Value)
        {
            // player info sent to server, then server changes sync var which updates, causing hooks to fire
            isDead = _Value;
        }

        [ServerCallback]
        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Bullet")
            {
                --health;
                NetworkServer.Destroy(other.gameObject);
                /*if (health == 0)
                    NetworkServer.Destroy(gameObject);*/
            }
        }
    }
}