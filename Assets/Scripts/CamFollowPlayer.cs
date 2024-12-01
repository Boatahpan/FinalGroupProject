using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player; // ตัวละครที่กล้องจะติดตาม
    public Vector3 offset;   // ระยะห่างระหว่างกล้องและตัวละคร
    public float rotationSpeed = 100f; // ความเร็วในการหมุนกล้อง
    private float currentRotationY = 0f; // การเก็บมุมหมุนในแกน Y
    private float currentRotationX = 0f; // การเก็บมุมหมุนในแกน X

    void Start()
    {
        // ตั้งค่าเริ่มต้นให้กล้องเริ่มต้นที่ตำแหน่ง offset จากตัวละคร
        currentRotationY = player.eulerAngles.y;
    }

    void LateUpdate()
    {
        // ตรวจจับการคลิกขวาค้างไว้
        if (Input.GetMouseButton(1)) // ปุ่มคลิกขวา
        {
            float mouseX = Input.GetAxis("Mouse X"); // การเคลื่อนที่ของเมาส์ในแนวนอน
            float mouseY = Input.GetAxis("Mouse Y"); // การเคลื่อนที่ของเมาส์ในแนวตั้ง

            // อัปเดตมุมการหมุน
            currentRotationY += mouseX * rotationSpeed * Time.deltaTime;
            currentRotationX -= mouseY * rotationSpeed * Time.deltaTime;

            // จำกัดมุมการหมุนในแกน X (เช่น ไม่ให้หมุนมองทะลุศีรษะหรือพื้น)
            currentRotationX = Mathf.Clamp(currentRotationX, -20f, 60f);
        }

        // คำนวณการหมุนของกล้อง
        Quaternion cameraRotation = Quaternion.Euler(currentRotationX, currentRotationY, 0);

        // ตั้งตำแหน่งของกล้องให้อยู่ตาม offset ที่หมุนแล้ว
        transform.position = player.position + cameraRotation * offset;

        // ให้กล้องหันไปมองตัวละครเสมอ
        transform.LookAt(player);
    }
}