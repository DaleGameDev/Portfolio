using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace TheLastImperial.Controllers
{
    //Reads the save file 
    class SaveControl
    {
        public SaveControl()
        {
            ReadXML("SaveFile.xml");
        }

        //Saves the game or creates a save file if there isn't one 
        public void saveGame()
        {
            Save save = new Save() { Distance = Entity.PlayerInfo.playerInfo.Distance, Material = Entity.PlayerInfo.playerInfo.Material, Level = Entity.PlayerInfo.playerInfo.Level };
            var fileStream = new FileStream("SaveFile.xml", FileMode.Create);

            new XmlSerializer(typeof(Save)).Serialize(fileStream, save);
            fileStream.Close();
        }

        public void ReadXML(string filename)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    Save.ReadSaves = (Save)new XmlSerializer(typeof(Save)).Deserialize(reader.BaseStream);
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
    }
}
