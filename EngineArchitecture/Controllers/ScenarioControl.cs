using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace TheLastImperial.Controllers
{
    //Loads in a scenario 
    class ScenarioControl : IDisposable
    {

        public List<string> list_scenarios;
        private int index;

        //Constructor Adds all the scenarios (xml files) to the list 
       public ScenarioControl()
        {
            list_scenarios = new List<string>();
            list_scenarios.Add("Content/scenario1.xml"); list_scenarios.Add("Content/scenario2.xml"); list_scenarios.Add("Content/scenario3.xml");
            index = 0;
        }



        public void Dispose()
        {
            index = 0;
        }

        //Calls read Scenario when and makes sure the a scenrio is read that is out of the scope of the list
        public void TriggerScenario()
        {
            if (index > list_scenarios.Count-1)
            {
                index = 0;
            }
            ReadXML(list_scenarios[index]);
            index++;
        }

        //Taken from the tutorial
        public void ReadXML(string filename)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    Scenario.scenario = (Scenario)new XmlSerializer(typeof(Scenario)).Deserialize(reader.BaseStream);
                }
            }
            catch (Exception e)
            {
                // If we've caught an exception, output an error message
                // describing the error
                Console.WriteLine("ERROR: XML File could not be deserialized!");
                Console.WriteLine("Exception Message: " + e.Message);
            }
        }

        //Used in future when scenario also holds info for the background 
        public void Draw()
        {

        }

    }
}
