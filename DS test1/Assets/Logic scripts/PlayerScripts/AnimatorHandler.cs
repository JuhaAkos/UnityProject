using UnityEngine;

namespace JA {
    public class AnimatorHandler : AnimatorManager
    {        
        InputHandler inputHandler;
        PlayerManager playerManager;

        PlayerLocomotion playerLocomotion;
        int vertical;
        int horizontal;
        public bool canRotate;

        public void Initialize()
        {
            playerManager = GetComponentInParent<PlayerManager>();
            anim = GetComponent<Animator>();
            inputHandler = GetComponentInParent<InputHandler>();
            playerLocomotion = GetComponentInParent<PlayerLocomotion>();
            //vertical and horizontal will refer to the values in the animator element
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        //changes between animations blendtree
        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement, bool isSprinting)
        {

            #region Vertical
            float v = 0;

            //"animation snapping"
            //rounds up values, helps smoother transition between anim states
            //change between walk, run
            if (verticalMovement > 0 && verticalMovement < 0.55f)
            {
                v = 0.5f;
            }
            else if (verticalMovement > 0.55f)
            {
                v = 1;
            }
            else if (verticalMovement < 0 && verticalMovement > -0.55f)
            {
                v = -0.5f;
            }
            else if (verticalMovement < -0.55f)
            {
                v = -1;
            }
            else
            {
                v = 0;
            }
            #endregion

            #region Horizontal
            float h = 0;

            if (horizontalMovement > 0 && horizontalMovement < 0.55f)
            {
                h = 0.5f;
            }
            else if (horizontalMovement > 0.55f)
            {
                h = 1;
            }
            else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
            {
                h = -0.5f;
            }
            else if (horizontalMovement < -0.55f)
            {
                h = -1;
            }
            else
            {
                h = 0;
            }
            #endregion

            if (isSprinting)
            {
                v = 2;
                h = horizontalMovement;
            }


            //set values... with damp time blend time so it doesn't snap from on into other
            anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
            //anim.SetFloat(vertical, v);
            anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
        }

        public void CanRotate()
        {
            canRotate = true;
        }

        public void StopRotation()
        {
            canRotate = false;
        }

        //inits missing because its a unity based function?
        //this stops root motion from happening
        private void OnAnimatorMove()
        //private void FixedUpdate()
        {
            if (anim == null)
            {
                anim = GetComponent<Animator>();
                //Debug.Log("missing animator");
            }

            if (inputHandler == null)
            {
                inputHandler = GetComponentInParent<InputHandler>();
                //Debug.Log("missing handler");
            }

            if (playerLocomotion == null)
            {
                playerLocomotion = GetComponentInParent<PlayerLocomotion>();
                //Debug.Log("missing locomotion");
            }

            if (playerManager == null)
            {
                playerManager = GetComponentInParent<PlayerManager>();
                //Debug.Log("missing Pmanager");
            }

            if (playerManager.isInteracting == false)
            {
                return;
            }

            //enable root motion for animations workaround
            //anim.ApplyBuiltinRootMotion();            

            //when an aniamtion moves the character, the object is centered back onto the character
            float delta = Time.deltaTime;
            playerLocomotion.GetComponent<Rigidbody>().linearDamping = 0;
            //playerLocomotion.rigidbody.linearDamping = 0;
            Vector3 deltaPosition = anim.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta;
            playerLocomotion.GetComponent<Rigidbody>().linearVelocity = velocity;
        }

        public void EnableCombo()
        {
            anim.SetBool("canDoCombo", true);
        }

        public void DisableCombo()
        {
            anim.SetBool("canDoCombo", false);
        }

        public void EnableIsInvulnerable()
        {
            anim.SetBool("isInvulnerable", true);
        }
        
         public void DisableIsInvulnerable()
        {
            anim.SetBool("isInvulnerable", false);
        }
        
        
    }

}

