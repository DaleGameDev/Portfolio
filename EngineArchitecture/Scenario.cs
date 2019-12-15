using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TheLastImperial
{
    //the classes contained in the xml files
    public class EnemySettings
    {
        public string Name = null;
        public float positionX = 0.0f;
        public float positionZ = 0.0f;
        public string modelName = null;
        public string state = null;
        public int exp = 0;
    }

    public class StoryText
    {
        public string part1 = null;
        public string part2 = null;
        public string part3 = null;
        public string part4 = null;
    }

    public class Setting
    {
        public string modelName = null;
    }

    //The class for reading the scenario sml files into 
    public class Scenario 
    {
        private static Scenario privateScenario = null;
        public static Scenario scenario
        {
            get
            {
                if (privateScenario == null)
                    privateScenario = new Scenario();
                return privateScenario;
            }
            set { privateScenario = value; }
        }
        public EnemySettings EnemySetting0;
        public EnemySettings EnemySetting1;
        public EnemySettings EnemySetting2;
        public EnemySettings EnemySetting3;
        public EnemySettings EnemySetting4;
        public StoryText StoryText;
        public Setting Setting;
    }


    

}
