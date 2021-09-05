using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace C500Pro
{
    [Serializable()]
    class C500 
    {
        public string HocTrucTuyen { get; set; }

        public List<string> UngDung { get; set; }

        public void ThemUngDung(string app)
        {
            if (UngDung == null)
                UngDung = new List<string>();
            UngDung.Add(app);
        }

        
        public static void WriteToBinaryFile<T>( T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Open("c500", append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }

        public static T ReadFromBinaryFile<T>()
        {
            using (Stream stream = File.Open("c500", FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }        
    }
}
