using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBullet : MonoBehaviour
{
    public NavMeshAgent enemy;
    public Transform player;

    [SerializeField] private float timer = 2;
    private float bulletTime;
    
    public GameObject enemyBullet;
    public Transform spawnPoint;
    public float enemySpeed;
    public float rotationSpeed = 5f; // ความเร็วในการหมุน

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //enemy.SetDestination(player.position);
        ShootAtPlayer();
        Vector3 direction = player.position - transform.position; // คำนวณทิศทางไปยัง Player
        direction.y = 0f; // ตั้งค่าทิศทางการหมุนเฉพาะแกน Y (ถ้าต้องการหันหน้าในแนวนอน)
        Quaternion targetRotation = Quaternion.LookRotation(direction); // คำนวณ Quaternion สำหรับหมุน Enemy
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed); // หมุน Enemy ไปยังเป้าหมายอย่างนุ่มนวล
    }

    void ShootAtPlayer()
    {
        bulletTime -= Time.deltaTime;

        if (bulletTime > 0) return;

        bulletTime = timer;

        // หาตำแหน่งของ Player
        Vector3 playerPosition = player.transform.position;

        // คำนวณทิศทางจาก spawnPoint ไปยัง Player
        Vector3 directionToPlayer = (playerPosition - spawnPoint.transform.position).normalized;

        // ปรับ rotation ของ spawnPoint ให้ชี้ไปทาง Player
        spawnPoint.transform.rotation = Quaternion.LookRotation(directionToPlayer);

        GameObject bulletObj = Instantiate(enemyBullet, spawnPoint.transform.position, spawnPoint.transform.rotation) as GameObject;
        Rigidbody bulletRig = bulletObj.GetComponent<Rigidbody>();
        bulletRig.AddForce(bulletRig.transform.forward * enemySpeed);
        Destroy(bulletObj, 1f);
    }
}
