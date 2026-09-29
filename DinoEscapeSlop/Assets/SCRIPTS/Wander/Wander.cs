using UnityEngine;

public class Wander : MonoBehaviour
{
       
            GameObject target;
            GameObject Dino;
            Vector2 position = new Vector2(0,0);

            //CURRENT
            Vector2 velocity = new Vector2(0, 0);

            //MAXSPEED
            float maxspeed = 5F;

            //MAXFORCE
            float maxforce = 0.2F;
            
            Vector2 acceleration = new Vector2(0, 0);

         // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //TEMPORAL
            GameObject target = GameObject.Find("Target");
            
            //TEMPORAL
            GameObject Dino =  GameObject.Find("Dino");

            position = new Vector2(Dino.transform.position.x, Dino.transform.position.y);
            Debug.Log("Position: " + position.x + " " + position.y);

        }

            // Update is called once per frame
        void Update()
        {

            seek(target, Dino);

            //APLICAR ESTO A LAS FORCES del dinosaurio
            Dino.GetComponent<ConstantForce>().force = new Vector3(acceleration.x, acceleration.y, 0);

            /*
            velocity+=acceleration;

            position+=velocity;+
            */

            acceleration*= 0;
            //Clear acceleration after it’s been applied. 
        }
 
        void seek(GameObject Goal, GameObject Vehicle) 
        {
            //DESIRED
            Vector2 desired = Goal.transform.position - Vehicle.transform.position;
            if(Mathf.Sqrt((desired.x* desired.x) + (desired.y * desired.y)) > maxspeed) desired =  new Vector2(desired.x/desired.magnitude* maxspeed, desired.y/desired.magnitude* maxspeed) ;

            //STEERING
            Vector2 steer = desired - velocity;
            if(Mathf.Sqrt((steer.x * steer.x) + (steer.y * steer.y)) > maxforce) steer = new Vector2(steer.x/steer.magnitude* maxforce, steer.y/steer.magnitude* maxforce) ;

            applyForce(steer);
        }

        void applyForce(Vector2 force)
        {
            acceleration = force;
        }
}

