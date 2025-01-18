using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RM_MST
{
    // A stage wall. These are used to prevent objects from going out of bounds.
    public class StageWall : MonoBehaviour
    {
        // The collider.
        public new BoxCollider2D collider;

        // Start is called before the first frame update
        void Start()
        {
            // If the collider is not set, set it.
            if(collider == null)
                collider = GetComponent<BoxCollider2D>();
        }

        // OnCollisionEnter2D is called when this collider2D/rigidbody2D has begun touching another collider2D/rigidbody2D.
        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Possible objects.
            LaserShot laserShot;
            Meteor meteor;

            // Laser Shot
            if(collision.gameObject.TryGetComponent(out laserShot))
            {
                // Kill the laser shot.
                laserShot.Kill(false);
            }
            // Meteor
            else if(collision.gameObject.TryGetComponent(out meteor))
            {
                // Cancel out velocity.
                meteor.rigidbody.velocity = Vector3.zero;
            }
        }
    }
}