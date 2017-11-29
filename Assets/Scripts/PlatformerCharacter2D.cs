using System;
using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class PlatformerCharacter2D : MonoBehaviour
    {
        [SerializeField] private float m_MaxSpeed = 4f;                    // The fastest the player can travel in the x axis.
        [SerializeField] private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
        [Range(0, 1)] [SerializeField] private float m_JumpSpeed = .5f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
        [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
        [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.
        private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        private Animator m_Anim;            // Reference to the player's animator component.
        private Rigidbody2D m_Rigidbody2D; // refrence to sprite so we can change it
        private Transform playerSprite;
        private Inventory inv;
        private bool m_FacingRight = true;  // For determining which way the player is currently facing.

        public bool killOffscreen = true;

        private void Awake()
        {
            // Setting up references.
            m_GroundCheck = transform.Find("GroundCheck");
            m_CeilingCheck = transform.Find("CeilingCheck");
            inv = GetComponent<Inventory>();
            m_Anim = GetComponent<Animator>();
            playerSprite = transform.Find("Sprites");
            if (playerSprite == null)
            {
                Debug.LogError("player is messed up");
            }
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        public bool dir()
        {
            return m_FacingRight;
        }
        void OnTriggerEnter2D(Collider2D other)
        {
			if (other.tag == "hazard") {
				inv.Damage (1);
			} else if (other.tag == "powerup") {
				//powerup stuff
				if (other.name == "EnergyPowerup" || other.name == "EnergyPowerup(Clone)") {
					GetComponentInChildren<WeaponController> ().is_powered_up = true;
					Destroy (other.gameObject);
				} else if (other.name == "ShieldPowerup" || other.name == "ShieldPowerup(Clone)") {
					//do something here
					GetComponent<Inventory>().is_powered_up = true;
					Destroy (other.gameObject);
				}
			}
        }
		void OnCollisionEnter2D(Collision2D other){
			if (other.gameObject.tag == "hazard" && other.collider is CapsuleCollider2D == false) {
				inv.Damage (1);
			}
		}
        private void FixedUpdate()
        {
            m_Grounded = false;

            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                    m_Grounded = true;
            }
            m_Anim.SetBool("Ground", m_Grounded);

            // Set the vertical animation
            m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);

			//this isn't great, but it does one-hit-kill
			if (transform.position.y < -5 && killOffscreen) {
				GetComponent<Inventory> ().Damage (100);
			}
			//checks left and right
			if (Mathf.Abs (transform.position.x) > 13.5f && killOffscreen) {
				GetComponent<Inventory> ().Damage (100);
			}
        }


        public void Move(float move, bool crouch, int jump)
        {



            //only control the player if grounded or airControl is turned on
            if (m_Grounded || m_AirControl)
            {
                // Reduce the speed if crouching by the crouchSpeed multiplier
                move = (!m_Grounded ? move*m_JumpSpeed : move);

                // The Speed animator parameter is set to the absolute value of the horizontal input.
                m_Anim.SetFloat("Speed", Mathf.Abs(move));



                // Move the character
				//if (!PlayerControllers.getPlayerController (GetComponent<Platformer2DUserControl> ().player_num).RightTrigger.IsPressed) {
				if (move != 0) {
					m_Rigidbody2D.velocity = new Vector2 (move * m_MaxSpeed, m_Rigidbody2D.velocity.y);
				} 
				//} 



				if (m_Rigidbody2D.velocity.x > 11f) {
					m_Rigidbody2D.velocity = new Vector2 (11f, m_Rigidbody2D.velocity.y);
				}
				if (m_Rigidbody2D.velocity.x < -11f) {
					m_Rigidbody2D.velocity = new Vector2 (-11f, m_Rigidbody2D.velocity.y);
				}


                // If the input is moving the player right and the player is facing left...
                if (move > 0 && !m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
                    // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && m_FacingRight)
                {
                    Flip();
                }
            }

			//XXX jump code
			/////////////////////////////////////////////////////////////////////////
			/*
            // If the player should jump...
            if (m_Grounded && jump == 1)
            {
                // Add a vertical force to the player.
                m_Grounded = false;
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            }
			// if player does short hop, ends jup early
			if (m_Rigidbody2D.velocity.y > 0 && jump == 2) { 
				// stop their upwards motion
				m_Rigidbody2D.velocity = Vector2.zero;
			}
			*/
			/////////////////////////////////////////////////////////////////////////
        }


        private void Flip()
        {
            // Switch the way the player is labelled as facing.
            m_FacingRight = !m_FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

}
