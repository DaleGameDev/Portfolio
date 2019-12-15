using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TheLastImperial.Entity
{
    public class Collidable
    {
        #region Bounding Types 
        //Used for basic collision 
        protected BoundingSphere boundingSphere = new BoundingSphere();
        public BoundingSphere BoundingSphere
        {
            get { return boundingSphere; }
        }
        //Used for the players firing range
        protected BoundingSphere sensor = new BoundingSphere();
        public BoundingSphere Sensor
        {
            get { return sensor; }
        }
        //Used to collect resources
        protected BoundingSphere boundingResource = new BoundingSphere();
        public BoundingSphere BoundingResource
        {
            get { return boundingResource; }
        }
        #endregion

        #region Member Functions
        //Collision tests and responses that are to be implemented by sub classes 
        public virtual bool CollisionTest(Collidable obj)
        {
            return false;
        }

        public virtual void OnCollision(Collidable obj)
        {
        }


        public virtual bool RangeCheck(Collidable obj)
        {
            return false;
        }

        public virtual void InRange(Collidable obj)
        {
        }

        public virtual bool CollectObject(Collidable obj)
        {
            return false;
        }

        public virtual void Collected(Collidable obj)
        {
        }
        #endregion


    }
}
