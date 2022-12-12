using System.Collections.Generic;
using System.Collections;
using Mirror;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.InputSystem;

namespace QuickStart
{
    public class PlayerScript : NetworkBehaviour
    {
        //bullet player reference
        //public string playerRef;



        public TextMeshPro playerNameText;
        public GameObject namePlate;
        public GameObject weaponHolder;

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
        private ScoreController ScoreController;
        private DBScript DBScript;
        private Timer timer;

        private Weapon activeWeapon;
        private float weaponCooldownTime;

        public float speed = 1;
        private float walkingSpeed = 1;
        private float runningSpeed = 2.5f;

        public bool showWinner = false;

        public GameObject[] objectsToHide;
        [SyncVar(hook = nameof(OnChanged))]
        public bool isDead = false;

        [SyncVar] public int health = 4;

        [Header("Camera")]
        public Transform playerRoot;
        //public Transform playerCam;

        public float cameraSensitivity;

        float rotX;
        float rotY;

        [Header("Movement")]
        public CharacterController controller;
        Vector3 velocity;

        [Header("Input")]
        public InputAction move;
        public InputAction mouseX;
        public InputAction mouseY;

        private void OnEnable()
        {
            move.Enable();
            mouseX.Enable();
            mouseY.Enable();
        }

        void OnDisable()
        {
            move.Disable();
            mouseX.Disable();
            mouseY.Disable();
        }
        void Awake()
        {
            //playerRef = gameObject.connectionId;

            //allows all players to run this
            sceneScript = GameObject.Find("SceneReference").GetComponent<SceneReference>().sceneScript;
            ScoreController = GameObject.Find("ScoreController").GetComponent<ScoreController>();
            timer = GameObject.Find("TimerController").GetComponent<Timer>();
            DBScript = GameObject.Find("DBHandler").GetComponent<DBScript>();
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

        void Start()
        {
            /*if (isLocalPlayer)
            {
                playerCam.GetComponent<Camera>().enabled = false;
                return;
            }*/
            //Cursor.lockState = CursorLockMode.Locked;

            controller = GetComponent<CharacterController>();
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
            if (sceneScript.readyStatus != 0 && showWinner == false)
            {
                timer.timerIsRunning = true;
                Vector2 mouseInput = new Vector2(mouseX.ReadValue<float>() * cameraSensitivity, mouseY.ReadValue<float>() * cameraSensitivity);
                rotX -= mouseInput.y;
                rotX = Mathf.Clamp(rotX, -90, 90);
                rotY += mouseInput.x;
                Cursor.lockState = CursorLockMode.Locked;

                playerRoot.rotation = Quaternion.Euler(0f, rotY, 0f);
                Camera.main.transform.localRotation = Quaternion.Euler(rotX, 0f, 0f);

                //Player movement
                //Vector2 moveInput = move.ReadValue<Vector2>();
                //Vector3 moveVelocity = playerRoot.forward * moveInput.y + playerRoot.right * moveInput.x;
                //controller.Move(moveVelocity * speed * Time.deltaTime);
                //controller.Move(velocity * Time.deltaTime);
                float moveZ = Input.GetAxis("Vertical") * Time.deltaTime * 4f * speed;
                float moveX = Input.GetAxis("Horizontal") * Time.deltaTime * 4f * speed;
                transform.Translate(0, 0, moveZ);
                transform.Translate(moveX, 0, 0);

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
                if (Input.GetKeyDown(KeyCode.P))
                {
                    //Debug.Log(playerNameText.text + " score: "+ ScoreController.scores[playerNameText.text]);

                    var maxValue = ScoreController.scores.Values.Max();
                    var maxValueKey = ScoreController.scores.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
                    DBScript.AddScore(maxValueKey, maxValue);

                    Debug.Log(maxValue + " " + maxValueKey);

                }

            }
            else
            {
                timer.timerIsRunning = false;
            }
            weaponHolder.transform.parent = Camera.main.transform;
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
            bullet.GetComponent<Bullet>().playerRef = playerNameText.text;
            // PlayerBulletRef(bullet);
            Destroy(bullet, activeWeapon.weaponLife);

        }
        [Command]
        void PlayerBulletRef(GameObject bullet)
        {
            //  bullet.GetComponent<Bullet>().playerRef = "hejsa";

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
            timer.playerScript = this;
            sceneScript.playerScript = this;
            sceneScript.SetupScene();

            Camera.main.transform.SetParent(transform);
            Camera.main.transform.localPosition = new Vector3(0, 0.5f, 0);
            Camera.main.transform.localRotation = new Quaternion(0, 0, 0, 0);

            namePlate.transform.localPosition = new Vector3(0, -0.3f, 0.6f);
            namePlate.transform.Rotate(0, 0, 180);
            namePlate.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            string name = "Player" + Random.Range(100, 999);
            Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            CmdSetupPlayer(name, color);

            // Add the player's score to the scores dictionary
            //ScoreController.scores.Add(playerNameText.text, 0);
            initiatePlayerScore();
        }
        [Command(requiresAuthority = false)]
        void initiatePlayerScore()
        {
            ScoreController.scores.Add(playerNameText.text, 0);
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
                health = 4;
                foreach (var obj in objectsToHide)
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

        [Command(requiresAuthority = false)]
        public void CmdPlayerStatus(bool _Value)
        {
            // player info sent to server, then server changes sync var which updates, causing hooks to fire
            isDead = _Value;
        }

        [ServerCallback]
        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Bullet"))
            {
                Debug.Log("Hit by bullet");
                --health;
                NetworkServer.Destroy(other.gameObject);
                if (health == 0)
                {
                    Debug.Log("Dead");
                    CmdPlayerStatus(true);
                }
                //Debug.Log("Hitby:" + other.GetComponent<Bullet>().playerRef);
                //ScoreController.scores[other.gameObject.GetComponent<Bullet>().playerRef] += 1;

                AddScore(other.gameObject.GetComponent<Bullet>().playerRef);
                // Show Dictionary
                string scoresString = "";
                foreach (KeyValuePair<string, int> score in ScoreController.scores)
                {
                    scoresString += score.Key + ": " + score.Value + "\n";
                }
                ScoreController.scoresText.text = scoresString;
            }
        }

        [Command(requiresAuthority = false)]
        void AddScore(string other)
        {
            //Debug.Log("Hitby:" + other.GetComponent<Bullet>().playerRef);
            ScoreController.scores[other] += 1;
        }
    }
}