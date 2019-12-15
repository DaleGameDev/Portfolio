using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLastImperial
{
    //Class used to store info from xml save file
    public class Save
    {
        public float Material { get; set; }
        public float Distance { get; set; }
        public int Level { get; set; }
        private static Save privateSaves = null;
        public static Save ReadSaves
        {
            get
            {
                if (privateSaves == null)
                    privateSaves = new Save();
                return privateSaves;
            }
            set { privateSaves = value; }
        }
    }

}
