using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TheLastImperial.Entity
{
    //Entity class that saves all the information of the enemy ships 
    class Enemy:Ship
    {

        #region Members
        private bool Attacked;
        private bool NearDeath;
        private float shield;
        public float Shield
        {
            get { return shield; }
            set { shield = value; }
        }

        private string name;
        private const float yBind = 300.0f;
        public Vector3 Direction;
        public Vector3 Up;
        public Vector3 Right;
        public Vector3 Velocity;
        private float elapsed;
        private string stateName;
        public string modelName;
        private float health;
        public float Health
        {
            get { return health; }
            set { health = value; }
        }
        private int exp;
        public int Exp
        {
            get { return exp; }
            set { exp = value; }
        }

        private float thrust;
        public float Thrust
        {
            get { return thrust; }
            set { thrust = value; }
        }

        Matrix world;
        public Matrix World{
            get { return world; }
        }

        private Controllers.AI ai;
        #endregion

        //Constructor 
        public Enemy(string Name,float X,float Z,string StateName, string ModelName, int exp)
        {
            //These values are read from the exml file 
            name = Name;
            position.X = X;
            position.Z = Z;
            position.Y = yBind;
            stateName = StateName;
            modelName = ModelName;
            Exp = exp;
            Initialise();
            //General Setting for all enemies
            position += Entity.PlayerInfo.playerInfo.Position;
            boundingSphere.Radius = 1000.0f;
            boundingSphere.Center = position;
            Thrust = 40000.0f;
            sensor.Radius = 8000.0f;
            sensor.Center = position;
            Health = 100.0f;
            Shield = 100.0f;
        }

        
        //Sets up the AI for the enemy with a starting state stored in the xml file 
        private void Initialise()
        {
            ai = new Controllers.AI(this);

            Controllers.AngryState angryState = new Controllers.AngryState();
            Controllers.NuetralState nuetralState = new Controllers.NuetralState();
            Controllers.FriendlyState friendlyState = new Controllers.FriendlyState();
            Controllers.FleeState fleeState = new Controllers.FleeState();

            ai.AddState(angryState);
            ai.AddState(nuetralState);
            ai.AddState(friendlyState);
            ai.AddState(fleeState);

            //friednly state
            friendlyState.AddTransition(new Controllers.Transition(angryState, () => Attacked));

            //angry state
            angryState.AddTransition(new Controllers.Transition(fleeState, () => NearDeath));

            //nuetral state
            nuetralState.AddTransition(new Controllers.Transition(fleeState, () => NearDeath));
            nuetralState.AddTransition(new Controllers.Transition(angryState, () => Attacked));


            //fleeState no transition out of 


            ai.StartState(stateName);
        }


        //Updates the enemies status per frame 
        public void Update(GameTime gameTime)
        {
            if (shield < 100.0f&&!Attacked) { Attacked = true; }
            elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            ai.Update(gameTime);
            Right = Vector3.Cross(Direction, Vector3.Up);
            Velocity = Direction * Thrust*elapsed;
            Position += Velocity * elapsed;
            world = Matrix.Identity;
            world.Forward = Direction;
            world.Up = Vector3.Up;
            world.Right = Right;
            world.Translation = Position;
            boundingSphere.Center = Position;
            sensor.Center = position;
            //position.Y = yBind;
            if(Health <= 0.0f)
            {
                Dead();
                
                //Console.WriteLine("working like a charm");
            }
        }


        //Event sent when enemy dies 
        public event EventHandler<EventArgs> Death;

        public void Dead()
        {
            Death(this, EventArgs.Empty);
        }

    }
}
