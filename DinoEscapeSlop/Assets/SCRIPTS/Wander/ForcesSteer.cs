using UnityEngine;

public class ForcesSteer : MonoBehaviour
{
            public GameObject target;
            
            //follow strength, higher = faster, smaller arc;
            private float smooothness = 0.5f;

            private float repel = 0.5f; // fuerza de repulsion, mayor = mas fuerte rango 0-1

            private float radius = 10f; // cada cuadricula de unity son 10m

            private Vector3 counterforce = new Vector3(0,0,0);
            private Vector3 force = new Vector3(0,0,0);
            private Vector3 desired = new Vector3(0,0,0);

            //MAXSPEED rotation interpolation factor
            private float maxspeed = 2.5F; 
            
         // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {            
           Debug.Log("Target: " + target.transform.position);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            rotation();
            
        //FORCE FORWARD
            //--------------------------------------------------------------------------------------------------------------
            force = Vector3.Lerp(this.transform.position, target.transform.position, smooothness * Time.deltaTime);

            if(force.z<0)
            {
                force = new Vector3(0, 0, -force.z);
            }
            else
            {
                force = new Vector3(0, 0, force.z);
            }

            //ARRIVAL (balance de fuerzas en el centro, contrafuerza progresivamente superior a la fuerza de empuje) 
            //TODO Rebajar ambas fuerzas progresivamente

            //distancia absoluta
            desired = target.transform.position - this.transform.position;
            float distance = Mathf.Sqrt(desired.x*desired.x + desired.z*desired.z);
            if(distance<0) distance = -distance;

            //Debug.Log("Distance: " + distance);

            
            if(distance<radius)
            {
                Debug.Log("Ratio: " + (1f- distance / radius )); //Range 0-1, 0 = center, 1 = radius
                //counterforce = new Vector3(0, 0, -this.GetComponent<ConstantForce>().relativeForce.z);
                counterforce = new Vector3(0, 0, force.z* (1f+(1 - distance/radius))*repel); 
                force = new Vector3(0, 0, force.z);
                this.GetComponent<ConstantForce>().relativeForce = force - counterforce; 

            }
            else this.GetComponent<ConstantForce>().relativeForce = force;  
        }

        private void rotation()
        {
            //ROTATION
            //1.DESIRED
            desired = target.transform.position - this.transform.position;

            //TURNING
             // Generate a Quaternion that faces that direction
            // Passing the up direction (Vector3.up) as the second argument keeps the head from tilting NO HACE FALTA
            Quaternion targetRotation = Quaternion.LookRotation(desired);

           
            // 2. Interpolate between the current and target rotations with Slerp
            // Moving a fixed fraction toward the target each frame produces
            // a natural turn that eases out near the end
            transform.rotation = Quaternion.Slerp
            (
                this.transform.rotation, // Current rotation
                targetRotation,     // Target rotation
                Time.deltaTime * maxspeed // Interpolation factor
            );
        }
}

