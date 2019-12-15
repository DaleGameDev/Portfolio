using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;



namespace TheLastImperial
{
    //Simple class thats handles the creation and movement of the camera 
    class Camera
    {
        private Vector3 cameraPosition;
        private Vector3 cameraView;
        private Matrix viewMatrix;
        private Matrix projectMatrix;
        private float aspectRatio;


        public Matrix ViewMatrix
        {
            get { return viewMatrix; }
            set { }

        }

        public Matrix ProjectMatrix
        {
            get { return projectMatrix; }
            set { }
        }


        public float AspectRatio
        {
            get { return aspectRatio; }
            set { aspectRatio = value; }
        }



        public Camera()
        {
        }

        //initialization 
        public void Ini()
        {
            projectMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1.0f, 25000.0f);
        }

        //Update that is called per frame is moved based on player position
        public void Update(Vector3 playerPosition)
        {
           
            cameraView = playerPosition;
            cameraPosition = (playerPosition + new Vector3(10.0f, 20000.0f, 10.0f));

            //cameraView = Vector3.Zero;
            //cameraPosition = ( new Vector3(10.0f, 20000.0f, 10.0f));

            viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraView, Vector3.Up);


        }



    }
}
