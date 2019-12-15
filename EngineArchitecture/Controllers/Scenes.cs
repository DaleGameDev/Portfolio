using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace TheLastImperial.Controllers
{   
    //base class for all scenes  
    public abstract class Scenes 
    {
        //All scenes have their own Draw update and load methods 
        public abstract void Draw();
        public abstract void Update();
        public abstract void Load();
    }

    class MainMenu : Scenes
    {
        //Stores its own background and can be unloaded if needed 
        Texture2D texture;
        SpriteBatch spriteBatch;
        private GraphicsDevice device;
        private ContentManager Content;

        //Constructor 
        public MainMenu(GraphicsDevice device, ContentManager content)
        {
            this.device = device;
            Content = content;
            spriteBatch = new SpriteBatch(this.device);
        }


        public override void Draw()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(texture, new Rectangle(0, 0, device.Viewport.Width, device.Viewport.Height), Color.AliceBlue);
            spriteBatch.End(); 
        }

        public override void Load()
        {
            texture = Content.Load<Texture2D>("Background-1");
        }

        public override void Update()
        {
            
        }
    }

    class GameScene : Scenes
    {
        //Game scene currently doesn't execute anything as it is not needed at the moment 
        private GraphicsDevice device;
        private ContentManager Content;

        public GameScene(GraphicsDevice device, ContentManager content)
        {
            this.device = device;
            Content = content;
        }

        public override void Draw()
        {

        }

        public override void Load()
        {
           
        }

        public override void Update()
        {

        }
    }

    //Not yet implemented
    class PlayerMenu : Scenes
    {
        private GraphicsDevice device;
        private ContentManager Content;

        public PlayerMenu(GraphicsDevice device, ContentManager content)
        {
            this.device = device;
            Content = content;
        }

        public override void Draw()
        {

        }

        public override void Load()
        {
            
        }

        public override void Update()
        {

        }
    }

    //follows the same structure as main menu 
    class RestartScene : Scenes
    {
        private GraphicsDevice device;
        private ContentManager Content;
        Texture2D texture;
        SpriteBatch spriteBatch;
        

        public RestartScene(GraphicsDevice device, ContentManager content)
        {
            this.device = device;
            Content = content;
            spriteBatch = new SpriteBatch(this.device);
        }

        public override void Draw()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(texture, new Rectangle(0, 0, device.Viewport.Width, device.Viewport.Height), Color.AliceBlue);
            spriteBatch.End();
            
        }

        public override void Load()
        {
            texture = Content.Load<Texture2D>("Background-2");
            
        }

        public override void Update()
        {

        }
    }

    //Used to change scenes 
     public class SceneSwitcher
     {
        MainMenu mainMenu;
        GameScene gameScene;
        PlayerMenu playerMenu;
        RestartScene restartScene;

        List<Scenes> scenes;
        int index;
        SceneList currentScene;

        public SceneSwitcher(GraphicsDevice device,ContentManager content)
        {
            scenes = new List<Scenes>();
            index = new int();
            mainMenu = new MainMenu(device,content);
            gameScene = new GameScene(device, content);
            playerMenu = new PlayerMenu(device, content);
            restartScene = new RestartScene(device, content);
        }


        public void Intialise()
        {
            scenes.Add(mainMenu);
            scenes.Add(gameScene);
            scenes.Add(playerMenu);
            scenes.Add(restartScene);
        }

        public void SwitchScene(SceneList scene)
        {
            switch (scene)
            {
                case SceneList.mainMenu:
                    index = 0;
                    currentScene = SceneList.mainMenu;
                    break;
                case SceneList.gameScene:
                    index = 1;
                    currentScene = SceneList.gameScene;
                    break;
                case SceneList.playerMenu:
                    index = 2;
                    currentScene = SceneList.playerMenu;
                    break;
                case SceneList.restartScene:
                    index = 3;
                    currentScene = SceneList.restartScene;
                    break;
            }
        }

        public void Update()
        {
            scenes[index].Update();
        }

        public void Draw()
        {
            scenes[index].Draw();
        }

        public void Load()
        {
            scenes[index].Load();
        }

        SceneList CurrentScene
        {
            get { return currentScene; }           
        }


    }

    //List of the scenes 
    public enum SceneList
    {
        mainMenu,
        gameScene,
        playerMenu,
        restartScene
    }


}
