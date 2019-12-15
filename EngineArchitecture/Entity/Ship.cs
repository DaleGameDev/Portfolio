using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TheLastImperial.Entity
{
    //The base class for the player and the enemies 
    class Ship: Collidable
    {

       
        //firing distance for the player 
        private const float sensorLength = 8000.0f;

        protected Vector3 position;
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        //checking the collision 
        public override bool CollisionTest(Collidable obj)
        {
            if (obj != null)
            {
                return BoundingSphere.Intersects(obj.BoundingSphere);
            }

            return false;
        }
        //resolving the collision 
        public override void OnCollision(Collidable obj)
        {
            // Cast the object as a ship
            Ship otherShip = obj as Ship;
            if (otherShip != null)
            {
                // The collision normal is the direction in which the collision occurred
                // We want the ships to react in this direction
                Vector3 collisionNormal = Vector3.Normalize(otherShip.BoundingSphere.Center - BoundingSphere.Center);

                // The distance between the two ships can be calculated using the centers of the two bounding spheres
                float distance = Vector3.Distance(otherShip.BoundingSphere.Center, BoundingSphere.Center);

                // The penetration depth determines how much the two spheres have intersected
                float penetrationDepth = (otherShip.BoundingSphere.Radius + BoundingSphere.Radius) - distance;
                penetrationDepth += 10.0f;

                // Negate the collision normal as we want to act in the opposite direction
                // of the collision and multiply by how much the spheres have intersected
                
                    AddPosition(-collisionNormal * penetrationDepth);
  
            }
        }

        
        //checkign if the enemy is in range of the player 
        public override bool RangeCheck(Collidable obj)
        {
            if (obj != null)
            {
                return Sensor.Intersects(obj.BoundingSphere);
            }
            return false;
        }

        //used in collision
        public void AddPosition(Vector3 p)
        {
            position += p;
            boundingSphere.Center = position;
            
        }

       


    }
}
