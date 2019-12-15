using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TheLastImperial.Entity
{
    //Class that contains the information of a resource created by the resouce control class...
    //Inherits from Collidable to allow for easy collision detection in the collision manager
    class Resource:Collidable
    {
        //Constructor takes the position of the defeated enemy 
        public Resource(Vector3 pos)
        {
            Position = pos;
            position.Y = 300.0f;
            world = Matrix.Identity;
            world.Translation = Position;
            Console.WriteLine(pos);
            boundingResource.Radius = 200.0f;
            boundingResource.Center = Position; 
            Random rand =new Random();
            power = rand.Next(25);
            Material = rand.Next(25);
        }

        //Member variables

        #region Memebers
        private float power;
        public float Power
        {
            get { return power; }
            set { power = value; }
        }
        private float material;
        public float Material
        {
            get { return material; }
            set { material = value; }
        }

        private Vector3 position;
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Matrix World
        {
            get { return world; }
        }
        private Matrix world;
        #endregion

        //Event for collision response when collected by the player 
        public void collected()
        {
            Collection(this, EventArgs.Empty);
        }
        public event EventHandler<EventArgs> Collection;

        


    }


    
}
