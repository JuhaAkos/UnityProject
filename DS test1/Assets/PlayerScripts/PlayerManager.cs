using UnityEngine;

namespace JA {
public class PlayerManager : MonoBehaviour
{
    InputHandler inputHandler;
    Animator anim;
    PlayerLocomotion playerLocomotion;    
    CameraHandler cameraHandler;

    [Header("Player Flags")]
    public bool isInteracting;
    public bool isSprinting;


    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        isInteracting = anim.GetBool("isInteracting");

        inputHandler = GetComponent<InputHandler>();        
        playerLocomotion = GetComponent<PlayerLocomotion>();
        

        //START ADDED BECAUSE AWAKE WASN'T BEING CALLED AND CAMERAHANDLER DIDN'T GET INITIALIZED
        cameraHandler = CameraHandler.singleton;       
            
    }
    
    private void Awake()
        {
            cameraHandler = CameraHandler.singleton;
            //Debug.Log("CM1: " + (cameraHandler!= null));
        }

    void Update()
    {
        //frame update?
        //what is delta?
        float delta = Time.deltaTime;

        isInteracting = anim.GetBool("isInteracting");
        //Debug.Log("Interact? - " + inputHandler.isInteracting);        

        //FROM LOCOMOTION       
        inputHandler.TickInput(delta);
        playerLocomotion.HandleMovement(delta);
        playerLocomotion.HandleRollingAndSprinting(delta);
    }
    
    private void FixedUpdate(){
        float delta = Time.fixedDeltaTime;

        //Debug.Log("CM: " + (cameraHandler!= null));
        if (cameraHandler != null) {
            
            cameraHandler.FollowTarget(delta);
            cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);

        }
    }

    //method for reseting flags
    private void LateUpdate()
    {
        inputHandler.rollFlag = false;
        inputHandler.sprintFlag = false;
        isSprinting = inputHandler.b_Input;
    }

}
}