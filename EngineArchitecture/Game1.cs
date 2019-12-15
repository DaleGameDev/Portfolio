using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheLastImperial
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        //Classes 
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Entity.Player player;
        Model playerModel;
        Model groundModel;
        Model BoxModel;
        Controllers.SceneSwitcher sceneSwitcher;
        Controllers.EnemyControl enemyControl;
        Controllers.CollisionManager collisionManager;
        Controllers.SaveControl saveControl;
        Save save;
        
        Camera camera;
        Controllers.CommandManager commandManager;
        Controllers.UI userInterface;
        Controllers.ScenarioControl scenarioControl;
        Controllers.ResourceControl resourceControl;

        //Constructor 
        public Game1()
        {
            saveControl = new Controllers.SaveControl();
            graphics = new GraphicsDeviceManager(this);
            graphics.SupportedOrientations = DisplayOrientation.Portrait;
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
            camera = new Camera();
            player = new Entity.Player();
            commandManager = new Controllers.CommandManager();
            userInterface = new Controllers.UI(this);
            enemyControl = new Controllers.EnemyControl(Services);
            collisionManager = new Controllers.CollisionManager();
            resourceControl = new Controllers.ResourceControl(Services,collisionManager);
            scenarioControl = new Controllers.ScenarioControl();

            IsFixedTimeStep = false;

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            
        }

        //Answers event for when the player travels far enough and loads a new scenario... 
        //Also sets up the listeners for the death of an enemy 
        private void TriggerPoint(object source, EventArgs e)
        {
            enemyControl.Dispose();
            scenarioControl.TriggerScenario();
            enemyControl.SpawnEnemies();
            enemyControl.Load();
            sceneSwitcher.Load();
            for(int i = 0; i < enemyControl.enemies.Count; i++)
            {
                collisionManager.AddCollidable(enemyControl.enemies[i]);
            }
            for (int i = 0; i < enemyControl.enemies.Count; i++)
            {
                enemyControl.enemies[i].Death += this.Death0;
            }

        }

        //Called when an enemy is destroyed to update the collision manager
        private void Death0(object source, EventArgs e)
        {
            Entity.Collidable obj = source as Entity.Collidable;
            for(int i = 0; i < collisionManager.m_Collidables.Count; i++)
            {
                if(source == collisionManager.m_Collidables[i])
                {
                    collisionManager.RemoveCollidable(obj);
                    break;
                }
            }
        }

        //When an end game event occurs like the player losing all health...
        //Saves the game to an xml file and resets many classes 
        private void EndGame(object source, EventArgs e)
        {
            saveControl.saveGame();
            sceneSwitcher.SwitchScene(Controllers.SceneList.restartScene);
            sceneSwitcher.Load();
            userInterface.GameOver();
            enemyControl.Dispose();
            scenarioControl.Dispose();
            collisionManager.Dispose();
            resourceControl.Dispose();
            collisionManager.AddCollidable(player);
        }


        private void InitializeBindings()
        {
           // commandManager.AddKeyboardBinding(Keys.Escape, StopGame);
            commandManager.AddKeyboardBinding(Keys.W, player.ThrustUp);
            commandManager.AddKeyboardBinding(Keys.A, player.ThrustLeft);
            commandManager.AddKeyboardBinding(Keys.D, player.ThrustRight);
            commandManager.AddKeyboardBinding(Keys.S, player.ThrustDown);
            commandManager.AddKeyboardBinding(Keys.Space, player.BleedVelocity);
            commandManager.AddMouseBinding(Controllers.MouseButton.LEFT, player.MouseTurn);
        }
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        // Sets up listeners calls other initialize methods in classes 
        protected override void Initialize()
        {

            camera.AspectRatio = GraphicsDevice.Viewport.AspectRatio;
            camera.Ini();
            player.Graphics(GraphicsDevice);
            InitializeBindings();

            sceneSwitcher = new Controllers.SceneSwitcher(GraphicsDevice, Content);
            sceneSwitcher.Intialise();
            sceneSwitcher.SwitchScene(Controllers.SceneList.mainMenu);
            
            

            collisionManager.AddCollidable(player);
            

            userInterface.InitializeComponent();
            userInterface.Initialize();
            userInterface.ShieldClicked += player.OnShieldClicked;
            userInterface.WeaponsClicked += player.OnWeaponsClicked;
            userInterface.FireClicked += player.OnFireClicked;
            userInterface.StartClicked += this.GameScene;

            enemyControl.EnemyDeath += player.OnExpGained;
            enemyControl.EnemyDeath += resourceControl.AddResource;

            player.TriggerScenarioDistanceMet += TriggerPoint;
            player.GameOver += this.EndGame;
            userInterface.RestartClicked += player.Reset;
            userInterface.RestartClicked += this.GameScene;
            

            base.Initialize();
        }

        //Listener that calls to the scene switcher to switch the scene 
        private void GameScene(object source, EventArgs e)
        {
            sceneSwitcher.SwitchScene(Controllers.SceneList.gameScene);  
        }
 
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            playerModel = Content.Load<Model>("FBX/Ship");
            groundModel = Content.Load<Model>("Ground");
            //BoxModel = Content.Load<Model>("FBX/futuristic_ammo_box_01");
            sceneSwitcher.Load();
            // TODO: use this.Content to load your game content here

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            // TODO: Add your update logic here
            commandManager.Update();
            player.Update(gameTime);
            camera.Update(player.Position);
            userInterface.Update(gameTime);
            userInterface.UpdateButtons(gameTime);
            collisionManager.Update();
            enemyControl.Update(gameTime);

            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
           
            GraphicsDevice device = graphics.GraphicsDevice;

            device.Clear(Color.CornflowerBlue);
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            // TODO: Add your drawing code here
            //playerModel.Draw(Matrix.Identity, camera.ViewMatrix,camera.ProjectMatrix);
            
            //DrawModel(BoxModel, Matrix.Identity);
            //DrawModel(groundModel, Matrix.Identity);
            DrawModel(playerModel, player.World);
            enemyControl.Draw(camera);
            resourceControl.Draw(camera);

            sceneSwitcher.Draw();
            userInterface.Draw(gameTime);
           
            base.Draw(gameTime);
        }

        private void DrawModel(Model model, Matrix world)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * world;

                    // Use the matrices provided by the chase camera
                    effect.View = camera.ViewMatrix;
                    effect.Projection = camera.ProjectMatrix;
                }
                mesh.Draw();
            }
        }

    }



}
