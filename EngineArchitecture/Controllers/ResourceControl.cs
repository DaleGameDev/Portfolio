using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TheLastImperial.Controllers
{
    //Class to contorl Resources that are spawned when an enemy dies
    class ResourceControl
    {   
        //List of Resouces
        public List<Entity.Resource> resources;
        //Model for resources 
        public Model resourceCrates;
        //Content manager for resources
        private ContentManager content;
        public ContentManager Content
        {
            get { return content; }
        }
        CollisionManager collisionM;


        //Constructor passes the collision manager to allow removing of resources directly from the collision manager list...
        //IserviceProvider is used to create the content manager
        public ResourceControl(IServiceProvider serviceProvider,CollisionManager collisionManager)
        {
            content = new ContentManager(serviceProvider, "Content");
            resources = new List<Entity.Resource>();
            collisionM = collisionManager;
        }

       
        public void Dispose()
        {
            resources.Clear();
        }

        //Upon death of an enemy adds a resource 
        public void AddResource(object source, EnemyDeathArgs e)
        {
            Entity.Resource resource = new Entity.Resource(e.position);
            resource.Collection += this.RemoveResource;
            collisionM.AddCollidable(resource);
            resources.Add(resource);
            if (resources.Count == 1) { Load(); }
        }

        //When a resource is collected removes it from the game world
        public void RemoveResource(object Source, EventArgs e)
        {
            Entity.Resource source = Source as Entity.Resource;
            collisionM.RemoveCollidable(source);
            resources.Remove(source);
            if (resources.Count == 0) { Content.Unload(); }
        }

       
        //Loads the resource content
        public void Load()
        {
            resourceCrates =  Content.Load<Model>("FBX/futuristic_ammo_box_01");
        }

        //Draw that is called by the main system 
        public void Draw(Camera camera)
        {
            if (resources != null)
            {
                for (int i = 0; i < resources.Count; i++)
                {
                    DrawModel(resourceCrates, resources[i].World, camera);
                    
                }
            }
        }

        //Not used 
        public void Update()
        {
            
        }

        //Draw model from the tutorials 
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
    }
}
