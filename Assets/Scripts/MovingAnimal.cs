using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingAnimal : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;

    public float speed = 1f;

    private Vector3 targetPosition;
    // Start is called before the first frame update
    void Start()
    {
        targetPosition = pointB.position;
    }

    // Update is called once per frame
    void Update()
    {
        LoopMovement();
    }

    private void LoopMovement() 
    {
        //every frame move towards the targetPosition
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        //is it reached destination
        if(Vector3.Distance(transform.position,targetPosition) < 0.1f)
        {
            //if it reached destination, then switch targetPosition
            //switching means if current targetPosition A, then switch to B
            //else, switch to B
            if (targetPosition == pointB.position) {
                targetPosition = pointA.position;
                transform.eulerAngles = new Vector3(0, 180, 0);
                
            }
            else {
                targetPosition = pointB.position;
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            
        }
    }

}
