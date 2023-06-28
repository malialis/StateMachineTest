using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    [SerializeField] private Animator _anim;

    [SerializeField] private float moveX = 0.0f;
    [SerializeField] private float moveY = 0.0f;
    [SerializeField] private bool isMoving = false;


    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMotion();
    }

    private void UpdateMotion()
    {
        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");

        isMoving = !Mathf.Approximately(moveX, 0f);


        _anim.SetFloat("moveX", moveX);
        _anim.SetBool("isMoving", isMoving);
        

        
    }

}
