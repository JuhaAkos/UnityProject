using UnityEngine;

namespace JA {
    public class AnimatorHandler : MonoBehaviour
    {
        public Animator anim;
        int vertical;
        int horizontal;
        public bool canRotate;

        public void Initialize()
        {
            anim = GetComponent<Animator>();
            //vertical and horizontal will refer to the values in the animator element
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        //changes between animations
        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement)
        {
            
            #region Vertical
            float v = 0;

            //"animation snapping"
            //rounds up values, helps smoother transition between anim states
            //change between walk, run
            if (verticalMovement > 0 && verticalMovement < 0.55f) {
                v = 0.5f;
            }
            else if (verticalMovement > 0.55f) {
                v = 1;
            }
            else if (verticalMovement < 0 && verticalMovement > -0.55f) {
                v = -0.5f;
            }
            else if (verticalMovement < -0.55f) {
                v = -1;
            } else {
                v = 0;
            }
            #endregion

            #region Horizontal
            float h = 0;            

            if (horizontalMovement > 0 && horizontalMovement < 0.55f) {
                h = 0.5f;
            }
            else if (horizontalMovement > 0.55f) {
                h = 1;
            }
            else if (horizontalMovement < 0 && horizontalMovement > -0.55f) {
                h = -0.5f;
            }
            else if (horizontalMovement < -0.55f) {
                h = -1;
            } else {
                h = 0;
            }
            #endregion


            //set values... with damp time blend time so it doesn't snap from on into other
            anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
            //anim.SetFloat(vertical, v);
            anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);

            Debug.Log(anim.GetFloat(vertical));
        }

        public void CanRotate() {
            canRotate = true;
        }

        public void StopRotation(){
            canRotate = false;
        }
    }

}

