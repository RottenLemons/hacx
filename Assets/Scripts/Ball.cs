using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody rbody;

    public bool islaunched;

    public float density = 7750f; //kg/m^3
    public float dragCoefficient = 0.5f;
    public float volume = 0;
    public float initialVel = 70f; // m/s
    private float force;
    public float mass;
    float distToGround;
    public float r;
    float p = 1.225f; //density of air 1.225kg/m^3
    public float area;
    public Material window1;
    public Material window2;
    private int target;
    Vector3 vel;
    public bool final = false;

    /*    public void Setup(float velocity, float mass, float dragCoefficient)
        {
            this.initialVel = velocity;
            this.mass = mass;
            this.dragCoefficient = dragCoefficient;
        }*/

    float getDragCoefficient(bool useShorterCoefficientEquation = false)
    {
        float characteristic_length = SettingsMenu.instance.height;
        float kinematic_viscosity = 1.48E-5f;  //m2/s

        float re = rbody.velocity.normalized.magnitude * characteristic_length / kinematic_viscosity;

        if (useShorterCoefficientEquation) return (float) ( (24 / re) * Math.Pow(1 + 0.27 * re, 0.43) + 0.47 * (1 - Math.Exp(-0.04 * Math.Pow(re, 0.38))));

        return (float) (8 * 10E-6 * (Math.Pow(re / 6530, 2) + Math.Tanh(re) - 8 * Mathf.Log(re) / Mathf.Log(10))
                - 0.4119 * Math.Exp(-2.08E43 / Math.Pow(re + Math.Pow(re, 2), 4))
                - 2.1344 * Math.Exp((-Math.Pow(Math.Log(Math.Pow(re, 2) + 10.7563) / Math.Log(10), 2) + 9.9867)/re)
                + 0.1357 * Math.Exp(-(Math.Pow(re / 1620, 2) + 10370) / re)
                - ((8.5E-3 * (2 * Math.Log(Math.Tanh(Math.Tanh(re))) / Math.Log(10) - 2825.7162)) / re) + 2.4795);
    }

    Vector3 getDragForce(float dragCoefficient)
    {
        Debug.Log(dragCoefficient);
        return -0.5f * (p * rbody.velocity.sqrMagnitude * dragCoefficient * SettingsMenu.instance.GetArea() * rbody.velocity.normalized);
        // original code: idk why they multiply by velocity again at the end; please investigate
        // Vector3 dragForce = -0.5f * (p * rbody.velocity.sqrMagnitude * dragCoefficient * area * rbody.velocity.normalized);
    }

    void Awake()
    {
        islaunched = true;
        
        rbody = GetComponent<Rigidbody>();
        //distToGround = GetComponent<SphereCollider>().bounds.extents.y;

/*        dragCoefficient = SettingsMenu.instance.GetDragCoefficient();
*/        // get drag coefficient 

        r = transform.localScale.y * 0.5f;
/*        volume = (4 * Mathf.PI * r * r * r) / 3;*/
        rbody.mass = SettingsMenu.instance.GetMass(); // in grams
       
        area = Mathf.PI * r * r;

        initialVel = SettingsMenu.instance.GetInitVel();
        force = rbody.mass * initialVel;

        // Assuming that 'transform.forward' represents the direction the ball is facing.
        Vector3 facingDirection = transform.forward;

        // Calculate the angle between the forward vector and the upward direction.
        float angle = Mathf.Atan2(facingDirection.y, facingDirection.z) * Mathf.Rad2Deg;

        //float yComponent = Mathf.Cos(angle * Mathf.Deg2Rad) * force;
        //float zComponent = Mathf.Sin(angle * Mathf.Deg2Rad) * force;
        //Vector3 forceApplied = new Vector3(0, yComponent, zComponent);

        rbody.AddForce(facingDirection * force, ForceMode.Impulse);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsGrounded() || rbody.isKinematic)
        {
            //GetComponent<SphereCollider>().isTrigger = true;
            //rbody.velocity = Vector3.zero;
            //rbody.angularVelocity = Vector3.zero;
            //rbody.useGravity = false;
        }
        else
        {
            // Debug.Log(rbody.velocity.magnitude);
            SimulateInRealTime(Time.deltaTime);
            vel = rbody.velocity;
        }

    }

    bool IsGrounded()
    {
        return (transform.position.y < 0);
    }

    void SimulateInRealTime(float dt)
    {
        //Vector3 direction = -rbody.velocity.normalized;
        //float velocity = rbody.velocity.magnitude;
        //var forceAmount = (p * velocity * velocity * dragCoefficient * area) * 0.5f;
        //rbody.AddForce(direction * forceAmount);

            

/*        Vector3 dragForce = -0.5f * (p * rbody.velocity.sqrMagnitude * dragCoefficient * area * rbody.velocity.normalized);
*/        // WEE ZEN CHANGE THIS BASED ON RESEARCH
        rbody.AddForce(getDragForce(getDragCoefficient(true)), ForceMode.Force);

        // Calculate the relative velocity of the ball with respect to the wind
        //Vector3 relativeVelocity = rbody.velocity - SettingsMenu.instance.GetWindDirection() * SettingsMenu.instance.GetWindSpeed();
        //Vector3 dragForce = -0.5f * p * dragCoefficient * area * relativeVelocity.sqrMagnitude * relativeVelocity.normalized;
        //rbody.AddForce(dragForce, ForceMode.Force);
    }

    public void Simulate()
    {

    }

    public void setDragCoefficient(float newValue)
    {
        dragCoefficient = newValue;
    }

    public void SetTarget()
    {
       
        TrailRenderer window_1 = gameObject.GetComponent<TrailRenderer>();
        window_1.material = window1;

    }

    private void OnCollisionEnter(Collision other)
    {
        Culprit shooter = transform.root.GetComponent<Culprit>();
        if (other.transform.gameObject != MainGameManager.instance.posPicker.transform.gameObject)
        {
            shooter.travelling = false;
            rbody.isKinematic = true;

        }
        else
        {
            //Vector3 vel = rbody.velocity;
            shooter.travelling = false;
            rbody.isKinematic = true;
            transform.position = other.contacts[0].point;

            if (Vector3.Distance(other.contacts[0].point, other.transform.position) > 0.2f)
                return;


            shooter.hitWindow1 = true;

            Vector3 normal = other.contacts[0].normal;
            HitBall HB = new HitBall();
            HB.RelatedHumanGameObject = transform.parent.gameObject;
            HB.WindowHit = target;
            HB.DistanceFromCenterW1 = Vector3.Distance(transform.position, MainGameManager.instance.posPicker.transform.position);
            shooter.angle1 = Vector3.Angle(vel, -normal);
            shooter.angles = Quaternion.LookRotation(vel).eulerAngles;
            shooter.hitSpeed1 = vel.magnitude;

            HB.CalculateAccuracy();
            HB.Hitposition = transform.position;
            MainGameManager.instance.AddNewHitRegistryToList(HB);
            MainGameManager.instance.RegisteredHitsPeople.Add(shooter.gameObject);
        }
       /* if (final) {
            //Debug.Log("HIT TARGET: " + target);
            Vector3 normal = other.contacts[0].normal;
            HitBall HB = new HitBall();
            HB.RelatedHumanGameObject = transform.parent.gameObject;
            HB.WindowHit = target;
            HB.DistanceFromCenterW1 = Vector3.Distance(transform.position, MainGameManager.instance.posPicker.transform.position);
            shooter.angle1 = Vector3.Angle(vel, -normal);
            shooter.angles = Quaternion.LookRotation(vel).eulerAngles;
            shooter.hitSpeed1 = vel.magnitude;
            
            HB.CalculateAccuracy();
            HB.Hitposition = transform.position;
            MainGameManager.instance.AddNewHitRegistryToList(HB);
            MainGameManager.instance.RegisteredHitsPeople.Add(shooter.gameObject);
        }
*/

        if (target == 0 && (shooter.iterations1 < shooter.maxIterations))
            return;

       

        /*if (other.transform.gameObject != targets[target].gameObject)
        {
            shooter.travelling = false;
            rbody.isKinematic = true;
        }
        else
        {
            //Vector3 vel = rbody.velocity;
            Vector3 normal = other.contacts[0].normal;

            shooter.travelling = false;
            rbody.isKinematic = true;
            transform.position = other.contacts[0].point;

            if (Vector3.Distance(other.contacts[0].point, other.transform.position) > 0.2f)
                return;

            if (target == 0)
                shooter.hitWindow1 = true;

            else if (target == 1)
                shooter.hitWindow2 = true;



            //Debug.Log("HIT TARGET: " + target);

            HitBall HB = new HitBall();
            HB.RelatedHumanGameObject = transform.parent.gameObject;
            HB.WindowHit = target;
            if (target == 0)
            {
                HB.DistanceFromCenterW1 = Vector3.Distance(transform.position, targets[target].transform.position);
                shooter.angle1 = Vector3.Angle(vel, -normal);
                shooter.hitSpeed1 = vel.magnitude;
            }
            else
            {
                HB.DistanceFromCenterW2 = Vector3.Distance(transform.position, targets[target].transform.position);
                shooter.angle2 = Vector3.Angle(vel, -normal);
                shooter.hitSpeed2 = vel.magnitude;
            }
            HB.CalculateAccuracy();
            HB.Hitposition = transform.position;
            MainGameManager.instance.AddNewHitRegistryToList(HB);
        }*/
    } // Is this not double counting?
}
