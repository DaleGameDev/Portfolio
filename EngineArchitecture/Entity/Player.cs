using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheLastImperial.Entity
{
    class Player : Ship
    {
        #region Memebers
        private GraphicsDevice graphicsDevice;
        public void Graphics(GraphicsDevice device)
        {
            graphicsDevice = device;
        }
        int count;
        public Matrix World
        {
            get { return world; }
        }
        private Matrix world;
        private float thrust;
        private float thrustStrafe;
        private const float RotationRate = 1.5f;
        private Vector2 RotationAmount = new Vector2();
        public Vector3 Direction;
        private Vector3 right;
        private const float ThrustForce = 24000.0f;
        private const float Mass = 1.0f;
        private const float yBind = 300.0f;
        public Vector3 Velocity;
        public Vector3 VelocityLateral;
        private float elapsed;
        private float DragFactor;
        private float DragLateral;
        private float distance;
        public float Distance
        {
            get { return distance; }
            set { distance = value; }
        }
        private float triggerDistance;
        private float time;
        private float shieldTimer;
        private float weaponTimer;
        private float fireTimer;
        private float cooldown;
        private int exp;
        public int Exp
        {
            get { return exp; }
            set { exp = value; }
        }
        private int level;
        public int Level
        {
            get { return level; }
            set { level = value; }
        }

        public Vector3 zero;

        public Vector3 Right
        {
            get { return right; }
        }

        private bool shields;
        private float shield;
        private float health;
        private float power;
        private float material;
        private bool weapons;
        private bool fire;
        private float weaponStrength = 100.0f;
        #endregion

        //Constructor
        public Player()
        {
            position = new Vector3(0.0f, yBind, 0.0f);
            Direction = Vector3.Forward;
            DragFactor = 1.0f;
            triggerDistance = 2000.0f;
            boundingSphere.Radius = 1000.0f;
            boundingSphere.Center = position;
            sensor.Radius = 8000.0f;
            sensor.Center = position;
            cooldown = 5.0f;
            shieldTimer = -cooldown;
            weaponTimer = -cooldown;
            fireTimer = -cooldown;
            shields = false;
            weapons = false;
            power = 150;
            material = 250;
            health = 100;
            shield = 100;
            DragLateral = 0.97f;
            count = 0;
            level = Save.ReadSaves.Level;
        }

        //Reset copies most of what the constructor sets
        public void Reset(object source,EventArgs e)
        {
            count = 0;
            position = new Vector3(0.0f, yBind, 0.0f);
            Direction = Vector3.Forward;
            DragFactor = 1.0f;
            triggerDistance = 2000.0f;
            boundingSphere.Radius = 1000.0f;
            boundingSphere.Center = position;
            sensor.Radius = 8000.0f;
            sensor.Center = position;
            cooldown = 5.0f;
            shieldTimer = -cooldown;
            weaponTimer = -cooldown;
            fireTimer = -cooldown;
            shields = false;
            weapons = false;
            power = 150;
            material = 250;
            health = 100;
            shield = 100;
            DragLateral = 0.97f;
        }


        #region Input resolvers 
        public void ThrustUp(Controllers.eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == Controllers.eButtonState.DOWN && thrust<0.4f)
            {
                thrust += 0.1f*elapsed;
            }
            else
            {
                thrust = 0.0f;
            }
        }

        public void ThrustLeft(Controllers.eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == Controllers.eButtonState.DOWN)
            {
                thrustStrafe = -0.3f;
            }
            else
            {
                thrustStrafe = 0.0f;
            }
        }

        public void ThrustRight(Controllers.eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == Controllers.eButtonState.DOWN)
            {
                thrustStrafe = 0.3f;
            }
            else
            {
                thrustStrafe = 0.0f;
            }
        }

        public void ThrustDown(Controllers.eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == Controllers.eButtonState.DOWN && thrust > -0.4f)
            {
                thrust -= 0.1f * elapsed;
            }
            else
            {
                thrust = 0.0f;
            }
        }

        public void BleedVelocity(Controllers.eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == Controllers.eButtonState.DOWN)
            {
                DragFactor = 0.95f;
            }
            else
            {
                DragFactor = 1.0f;
            }
        }


        public void MouseTurn(Controllers.eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == Controllers.eButtonState.DOWN)
            {
                int width = graphicsDevice.Viewport.Width;
                //We could use this code to find a value from -1.0 to 1.0
                //RotationAmount.X = -(float)(amount.X-width/2)/(float)width;
                //Or we can use this to check if it's left or right
                if (amount.X < (width / 2 + width / 4) && amount.X > (width / 2 - width / 4))
                {
                    if (amount.X - width / 2 > 0)
                        RotationAmount.X = -1.0f;
                    else
                        RotationAmount.X = 1.0f;
                }
            }
        }
        #endregion

        //Called every frame to update player 
        public void Update(GameTime gameTime)
        {
            elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            time += elapsed;
            RotationAmount = RotationAmount * RotationRate * elapsed;

            //movement and position update 
            Matrix rotationMatrix = Matrix.CreateFromAxisAngle(Right, RotationAmount.Y) *Matrix.CreateRotationY(RotationAmount.X);
            Direction = Vector3.TransformNormal(Direction, rotationMatrix);
            Direction.Normalize();
            right = Vector3.Cross(Direction, Vector3.Up);
            Vector3 force = Direction * thrust * ThrustForce;
            Vector3 acceleration = force / Mass;
            Velocity += acceleration * elapsed;
            Velocity *= DragFactor;
            Vector3 forceLateral = right * thrustStrafe * ThrustForce;
            Vector3 accelerationLateral = forceLateral / Mass;
            VelocityLateral += accelerationLateral * elapsed;
            VelocityLateral *= DragLateral;
            Position += Velocity * elapsed + VelocityLateral*elapsed;
            world = Matrix.Identity;
            world.Forward = Direction;
            world.Up = Vector3.Up;
            world.Right = right;
            world.Translation = Position;
            UpdateBlackBoard();
            Distance = Position.Length();
            DistanceCheck();
            position.Y = yBind;
            boundingSphere.Center = position;
            sensor.Center = position;

            //Degrading the players status 
            if (power > 0.0f)
            {
                power -= (elapsed);
                if (shields) { power -= (elapsed); }
                if (weapons) { power -= (elapsed); }
            }
            levelUp();
            //Check if player has died 
            if (power <= 0 || health <= 0)
            {
                count++;
                if (count == 1)
                {
                    PlayerDestroyed();
                }
                DragFactor = 0.0f;DragLateral = 0.0f;
            }
        }

        //Triggering a new scenario 
        private void DistanceCheck()
        {
            if (Distance > triggerDistance)
            {
                triggerDistance += 100000.0f;
                DistanceMet();
            }
        }
        //Event for when distance is met for the scenario control to switch to a new one
        public event EventHandler<EventArgs> TriggerScenarioDistanceMet;

        protected virtual void DistanceMet()
        {
            TriggerScenarioDistanceMet(this, EventArgs.Empty);
        }

        public event EventHandler<EventArgs> GameOver;

        protected virtual void PlayerDestroyed()
        {
            GameOver(this, EventArgs.Empty);
        }


        
       
        //resolving UI events
        public void OnShieldClicked(object source, EventArgs e)
        {
            if (time > shieldTimer + cooldown)
            {
                if (shields == true)
                {
                    shields = false;
                    shield = 0.0f;
                }
                else if (shields == false)
                {
                    shields = true;
                }
                shieldTimer = time;
            }
            
            Console.WriteLine(shields);
        }
        public void OnWeaponsClicked(object source,EventArgs e)
        {
            if (time > weaponTimer + cooldown)
            {
                if(weapons == true)
                {
                    weapons = false;
                }else if(weapons == false)
                {
                    weapons = true;
                }
                weaponTimer = time;
            }
            
            Console.WriteLine(weapons);
        }
        public void OnFireClicked(object source, EventArgs e)
        {
            if((time > fireTimer + cooldown) && (fire == false)&&(weapons==true))
            {
                fire = true;Console.WriteLine(fire);
            }
            
        }
        //resolving killing an enemy
        public void OnExpGained(object source,Controllers.EnemyDeathArgs e)
        {
            Exp += e.Experience;
        }

        //Checking if the player has gained a level 
        private void levelUp()
        {
            if (Exp >= 100)
            {
                level++;
                Exp = 0;
                if (cooldown > 1.0f) { cooldown -= (level / 2) / cooldown;}
                
                Console.WriteLine("level up");
            }
        }

        //Collision resolving 
        public override void InRange(Collidable obj)
        {
            Enemy OtherShip = obj as Enemy;
            if (OtherShip != null)
            {
                if (fire == true)
                {
                    if (OtherShip.Shield > 0.0f)
                    {
                        OtherShip.Shield -= weaponStrength;
                        fire = false;
                        fireTimer = time;
                        Console.WriteLine("ShieldDamage");
                    }
                    else
                    {
                        OtherShip.Health -= weaponStrength;
                        fire = false;
                        fireTimer = time;
                        Console.WriteLine("HealthDamage");
                    }
                }
            }
        }

        //Collision checker for collecting the resources 
        public override bool CollectObject(Collidable obj)
        {

            if (obj != null)
            {
                return BoundingSphere.Intersects(obj.BoundingResource);
            }
            return false;
        }


        //Collision resolving for collecting the resource
        public override void Collected(Collidable obj)
        {
           
            Resource resource = obj as Resource;
            if (resource != null)
            {
                power += resource.Power;
                material += resource.Material;
                resource.collected();
                Console.WriteLine(power);
            }
        }

        //Update the singleton that contains 
        private void UpdateBlackBoard()
        {
            Entity.PlayerInfo.playerInfo.Position = Position;
            Entity.PlayerInfo.playerInfo.Shield = shields;
            Entity.PlayerInfo.playerInfo.Weapons = weapons;
            Entity.PlayerInfo.playerInfo.Cooldown = cooldown;
            Entity.PlayerInfo.playerInfo.Level = level;
            Entity.PlayerInfo.playerInfo.Power = power;
            Entity.PlayerInfo.playerInfo.Distance = Distance;
            Entity.PlayerInfo.playerInfo.Material = material;
        }


    }

    //Singleton to store some playerinfo for any class to access if needed 
    public class PlayerInfo
    {
        private static PlayerInfo privatePlayerInfo = null;
        public static PlayerInfo playerInfo
        {
            get
            {
                if (privatePlayerInfo == null)
                    privatePlayerInfo = new PlayerInfo();
                return privatePlayerInfo;
            }
            set { privatePlayerInfo = value; }
        }
        private float material;
        public float Material
        {
            get { return material; }
            set { material = value; }
        }
        private float distance;
        public float Distance
        {
            get { return distance; }
            set{ distance = value; }
        }
        private float power;
        public float Power
        {
            get { return power; }
            set { power = value; }
        }
        private int level;
        public int Level
        {
            get { return level; }
            set { level = value; }
        }
        private Vector3 position;
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        private bool shield;
        public bool Shield
        {
            get { return shield; }
            set { shield = value; }
        }
        private bool weapons;
        public bool Weapons
        {
            get { return weapons; }
            set { weapons = value; }
        }
        private float cooldown;
        public float Cooldown
        {
            get { return cooldown; }
            set { cooldown = value; }
        }

    }


}
