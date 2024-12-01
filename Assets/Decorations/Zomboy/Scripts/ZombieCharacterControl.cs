using UnityEngine;
using System.Collections;

public class ZombieCharacterControl : MonoBehaviour
{
    private enum ControlMode
    {
        /// <summary>
        /// Up moves the character forward, left and right turn the character gradually and down moves the character backwards
        /// </summary>
        Tank,
        /// <summary>
        /// Character freely moves in the chosen direction from the perspective of the camera
        /// </summary>
        Direct
    }

    [SerializeField] private float m_moveSpeed = 2;
    [SerializeField] private float m_turnSpeed = 200;

    [SerializeField] private Animator m_animator = null;
    [SerializeField] private Rigidbody m_rigidBody = null;

    [SerializeField] private ControlMode m_controlMode = ControlMode.Direct;

    private float m_currentV = 0;
    private float m_currentH = 0;

    private readonly float m_interpolation = 10;

    private Vector3 m_currentDirection = Vector3.zero;
    public int jumpspeed = 0;
    private bool istouching = false;
    Rigidbody rb;

    public TMPro.TextMeshProUGUI textUI;
    private int totalScore = 6;
    private int currentScore = 0;

    public GameObject Warp;
    public GameObject heartPrefab;
    public GameObject boosterPrefab;
    private GameObject spawnedBooster;

    private bool isSpeedBoost = false;

    //public float boostSpeed = 3; // ความเร็วที่เพิ่ม
    public float boostDuration = 10f; // ระยะเวลาที่เพิ่มความเร็ว

    private int maxCount = 5;   // จำนวนสูงสุดของ Prefab ที่ต้องการสร้าง
    private int currentCount = 0; // ตัวนับจำนวน Prefab ที่สร้างแล้ว

    public AudioClip HeartSound;
    public AudioClip BoosterSound;
    public AudioClip WarpSound;
    private AudioSource audioSource;

    private void Awake()
    {
        if (!m_animator) { gameObject.GetComponent<Animator>(); }
        if (!m_rigidBody) { gameObject.GetComponent<Animator>(); }
    }

private void FixedUpdate()
    {
        switch (m_controlMode)
        {
            case ControlMode.Direct:
                DirectUpdate();
                break;

            case ControlMode.Tank:
                TankUpdate();
                break;

            default:
                Debug.LogError("Unsupported state");
                break;
        }
    }

private void TankUpdate()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");
        rb = GetComponent<Rigidbody>();
        textUI.text = "HEART " + currentScore + "/6";

        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

        transform.position += transform.forward * m_currentV * m_moveSpeed * Time.deltaTime;
        transform.Rotate(0, m_currentH * m_turnSpeed * Time.deltaTime, 0);

        m_animator.SetFloat("MoveSpeed", m_currentV);
        if (Input.GetKeyDown(KeyCode.Space) && istouching) {
            rb.AddForce(new Vector3(0f, 1*jumpspeed, 0f),ForceMode.Impulse);
        }        
    }

    private void DirectUpdate()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");
        rb = GetComponent<Rigidbody>();
        textUI.text = "HEART " + currentScore + "/6";

        Transform camera = Camera.main.transform;

        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

        Vector3 direction = camera.forward * m_currentV + camera.right * m_currentH;

        float directionLength = direction.magnitude;
        direction.y = 0;
        direction = direction.normalized * directionLength;

        if (direction != Vector3.zero)
        {
            m_currentDirection = Vector3.Slerp(m_currentDirection, direction, Time.deltaTime * m_interpolation);

            transform.rotation = Quaternion.LookRotation(m_currentDirection);
            transform.position += m_currentDirection * m_moveSpeed * Time.deltaTime;

            m_animator.SetFloat("MoveSpeed", direction.magnitude);
        }
        if (Input.GetKeyDown(KeyCode.Space) && istouching) {
            rb.AddForce(new Vector3(0f, 1*jumpspeed, 0f),ForceMode.Impulse);
        }        
    }

    void Start()
    {
    audioSource = GetComponent<AudioSource>();
    }



    private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.tag=="Floor") {
                istouching = false;
            }
        }
    private void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.tag=="Floor") {
                istouching = true;
            }
        }
    private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Collectable")) 
            {
                CollectHeart(other.gameObject);
            }
            if (other.gameObject.CompareTag("Booster"))
            {
                isSpeedBoost = Random.value > 0.5f;
                
                if (isSpeedBoost)
                {
                StartCoroutine(SpeedBooster(other.gameObject));
                }
                else
                {
                StartCoroutine(JumpBooster(boosterPrefab));
                }
            }
        }

    private void CollectHeart(GameObject heartPrefab)
        {
            heartPrefab.SetActive(false);
            currentScore++;
            StartCoroutine(RespawHeart(heartPrefab));
            audioSource.PlayOneShot(HeartSound);

        if (currentScore >= totalScore)
        {
            Warp.SetActive(true); // แสดง Secret Item
            audioSource.PlayOneShot(WarpSound);
            Debug.Log("Secret Warp Unlocked!");
        }
        }
        
    private IEnumerator RespawHeart(GameObject heartPrefab)
        {
        if (currentCount < maxCount)
            {
            yield return new WaitForSeconds(1f);
            heartPrefab.transform.position = new Vector3(Random.Range(-13f,15f), 3, Random.Range(-15f,13f));
            heartPrefab.SetActive(true);
            currentCount++;
            }
        }
    
    private IEnumerator SpeedBooster(GameObject boosterPrefab)
        {
            boosterPrefab.SetActive(false);
            jumpspeed = 5;
            Debug.Log("Jump Speed UP!");
            StartCoroutine(RespawBooster(boosterPrefab));
            audioSource.PlayOneShot(BoosterSound);
            yield return new WaitForSeconds(boostDuration); // รอเวลาที่กำหนด
            jumpspeed = 3; // คืนค่าความเร็วเดิม
            Debug.Log("Jump Speed Down!");
        }

        private IEnumerator JumpBooster(GameObject boosterPrefab)
        {
            boosterPrefab.SetActive(false);
            m_moveSpeed = 7;
            Debug.Log("Speed UP!");
            StartCoroutine(RespawBooster(boosterPrefab));
            yield return new WaitForSeconds(boostDuration); // รอเวลาที่กำหนด
            m_moveSpeed = 2; // คืนค่าความเร็วเดิม
            Debug.Log("Speed Down!");
        }

    /*private void SpawBooster()
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-13f, 15f), 2, Random.Range(-15f, 13f));
            Instantiate(boosterPrefab, spawnPosition, Quaternion.identity); // สร้างวัตถุจาก Prefab
        }*/    

    private IEnumerator RespawBooster(GameObject boosterPrefab)
        {
            yield return new WaitForSeconds(9f);
            boosterPrefab.transform.position = new Vector3(Random.Range(-13f,15f), 3, Random.Range(-15f,13f));
            boosterPrefab.SetActive(true);
        }
        
}

