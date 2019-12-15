using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TheLastImperial.Controllers
{
    //Draws and maintains of all the current enemies removing them when they die. 
    //Loads the content required for the enemies 
    class EnemyControl : IDisposable
    {
       private Entity.Enemy enemy0; private Entity.Enemy enemy1; private Entity.Enemy enemy2;
        private Entity.Enemy enemy3; private Entity.Enemy enemy4;

       

        public List<Entity.Enemy> enemies;
        //Stores the models used for the enemies 
        public List<Model> enemyModels;
        public ContentManager Content
        {
            get { return content; }
        }
        ContentManager content;

        //Constructor 
        public EnemyControl(IServiceProvider serviceProvider)
        {
            content = new ContentManager(serviceProvider, "Content");
            enemies = new List<Entity.Enemy>();
            enemyModels = new List<Model>();
        }

        // string test =  Entity.Scenario.scenario.EnemySetting0.enemyName;
        public void SpawnEnemies()
        {
            if (Scenario.scenario.EnemySetting0.Name != null)
            {
                enemy0 = new Entity.Enemy(Scenario.scenario.EnemySetting0.Name, Scenario.scenario.EnemySetting0.positionX,
                                          Scenario.scenario.EnemySetting0.positionZ, Scenario.scenario.EnemySetting0.state,
                                          Scenario.scenario.EnemySetting0.modelName,Scenario.scenario.EnemySetting0.exp);
                enemies.Add(enemy0);
                enemy0.Death += this.Death;
            }
            if (Scenario.scenario.EnemySetting1.Name != null)
            {
                enemy1 = new Entity.Enemy(Scenario.scenario.EnemySetting1.Name, Scenario.scenario.EnemySetting1.positionX,
                                          Scenario.scenario.EnemySetting1.positionZ, Scenario.scenario.EnemySetting1.state,
                                          Scenario.scenario.EnemySetting1.modelName, Scenario.scenario.EnemySetting1.exp);
                enemies.Add(enemy1);
                enemy1.Death += this.Death;
            }
            if (Scenario.scenario.EnemySetting2.Name != null)
            {
                enemy2 = new Entity.Enemy(Scenario.scenario.EnemySetting2.Name, Scenario.scenario.EnemySetting2.positionX,
                                          Scenario.scenario.EnemySetting2.positionZ, Scenario.scenario.EnemySetting2.state,
                                          Scenario.scenario.EnemySetting2.modelName, Scenario.scenario.EnemySetting2.exp);
                enemies.Add(enemy2);
                enemy2.Death += this.Death;
            }
            if (Scenario.scenario.EnemySetting3.Name != null)
            {
                enemy3 = new Entity.Enemy(Scenario.scenario.EnemySetting3.Name, Scenario.scenario.EnemySetting3.positionX,
                                          Scenario.scenario.EnemySetting3.positionZ, Scenario.scenario.EnemySetting3.state,
                                          Scenario.scenario.EnemySetting3.modelName, Scenario.scenario.EnemySetting3.exp);
                enemies.Add(enemy3);
                enemy3.Death += this.Death;
            }
            if (Scenario.scenario.EnemySetting4.Name != null)
            {
                enemy4 = new Entity.Enemy(Scenario.scenario.EnemySetting4.Name, Scenario.scenario.EnemySetting4.positionX,
                                          Scenario.scenario.EnemySetting4.positionZ, Scenario.scenario.EnemySetting4.state,
                                          Scenario.scenario.EnemySetting4.modelName, Scenario.scenario.EnemySetting4.exp);
                enemies.Add(enemy4);
                enemy4.Death += this.Death;
            }
        }
        //Method that responds to the event of an enemy's health reaching 0
        public void Death(object obj, EventArgs e)
        {
            for (int i = 0; i < enemies.Count;i++)
            {
                if (obj == enemies[i])
                {
                    enemyModels.Remove(enemyModels[i]);
                    // enemies[i] = null;
                    
                    ExpHandled(enemies[i].Exp,enemies[i].Position);
                    enemies.Remove(enemies[i]);
                    
                    break;
                }
            }
        }

        //Loads the enemies
        public void Load()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                enemyModels.Add(Content.Load<Model>(enemies[i].modelName));
            }
        }
        //Draws the enemies 
        public void Draw(Camera camera0)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                DrawModel(enemyModels[i],enemies[i].World,camera0);
            }
        }
        //calls update on all the enemies
        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Update(gameTime);
            }
        }
        //clears the content and any enemies left
        public void Dispose()
        {
            Content.Unload();
            enemies.Clear();
            enemyModels.Clear();
        }

        private void DrawModel(Model model, Matrix world, Camera camera0)
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
                    effect.View = camera0.ViewMatrix;
                    effect.Projection = camera0.ProjectMatrix;
                }
                mesh.Draw();
            }
        }

        //Event for when the enemy dies sends the exp contained in the enemy death args and the enemies position 
       public event EventHandler<EnemyDeathArgs> EnemyDeath;

       protected virtual void ExpHandled(int j,Vector3 pos)
       {
            EnemyDeath(this, new EnemyDeathArgs(j,pos));
       }



    }





    public class EnemyDeathArgs : EventArgs
    {
        public EnemyDeathArgs(int exp,Vector3 pos)
        {
            Experience = exp;
            position = pos;
           
        }
        public readonly int Experience;
        public readonly Vector3 position;
        public readonly Vector3 direction;
    }

}
