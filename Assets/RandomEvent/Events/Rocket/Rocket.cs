using System.Collections;
using UnityEngine;

public class Rocket : EventAction
{
    [SerializeField] GameObject rocket;
    [SerializeField] float speed = 20f;
    [SerializeField] GameObject viewObject;
    [SerializeField] GameObject rocketEffect;
    public override void StopEvent(){
        Destroy(rocketEffect);
    }
    public override void EventTrigger()
    {
        
        AudioManager.instance.Play("RocketWarning");
        rocketEffect = Instantiate(rocketEffect, new Vector3(0f, transform.position.y, 0), Quaternion.identity);
        viewObject.SetActive(false);
        StartCoroutine(StartMovement(3));
    }
    private IEnumerator StartMovement(float delay){
        yield return new WaitForSeconds(delay);
        
        AudioManager.instance.Play("RocketFlyby");
        viewObject.SetActive(true);
        MoveVelocity velocity = rocket.GetComponent<MoveVelocity>();
        if (transform.position.x < 0)
        {
            velocity.velocity = new Vector3(speed,0,0);
        }
        else
        {
            velocity.velocity = new Vector3(-speed,0,0);
        } 
        Destroy(gameObject, 3f);
    }
}
