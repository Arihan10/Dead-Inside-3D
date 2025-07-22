using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Zombie : MonoBehaviour
{
    private bool engaged = false;
    
    
    [SerializeField]
    private Animator animator;

    private Vector3 old_pos;
    private static readonly int SqrSpeed = Animator.StringToHash("sqr_speed");

    [SerializeField]
    private Transform skin;
    
    [SerializeField]
    private GameObject realskin;

    private Vector3 lerpVel = Vector3.zero;
    
    [SerializeField]
    public GameObject savedParticles;

    public void Awake()
    {
        //GetComponent<XRSimpleInteractable>().interactionManager = FindFirstObjectByType<XRInteractionManager>();
    }
    
    public void Engage()
    {
        engaged = true;
        Debug.Log("eng");
        skin.gameObject.layer = LayerMask.NameToLayer("Highlight");
        realskin.layer = LayerMask.NameToLayer("Highlight");
        GetComponent<ZombieNPC>().Select();
    }
    
    public void Disengage()
    {
        engaged = false;
        Debug.Log("diseng");
        skin.gameObject.layer = LayerMask.NameToLayer("Default");
        realskin.gameObject.layer = LayerMask.NameToLayer("Default");
        GetComponent<ZombieNPC>().Deselect();
    }

    public void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        Vector3 vel = transform.position - old_pos;
        old_pos = transform.position;
        lerpVel = Vector3.Lerp(lerpVel, vel, Time.deltaTime * 7.0f);

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            animator.SetFloat(SqrSpeed, lerpVel.sqrMagnitude);
            
            // does not matter too much...
            if (lerpVel.sqrMagnitude < 70.0f)
            {
                skin.rotation = Quaternion.Slerp(skin.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(lerpVel.normalized, Vector3.up), Vector3.up), 20.0f * Time.deltaTime);
            }
        }
    }
    
    // called from NavMan when close enough for reach
    public void Attack(Quaternion rot)
    {
        animator.Play("Attack");
        skin.rotation = rot; // snap to rot !!!
    }
    
    public void Die()
    {
        Instantiate(savedParticles, transform.position, Quaternion.identity);
    }
}
