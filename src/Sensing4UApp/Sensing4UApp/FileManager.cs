using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sensing4UApp
{
    /// <summary>
    /// The FileManager class is responsible for reading, loading, 
    /// and saving modified sensor data to binary (.bin) files.
    /// </summary>
    public class FileManager
    {
        public List<SensorData> LoadFile(string path) 
        {
            if (!File.Exists(path))
            {
                // Throws an exception if the file does not exist
                throw new FileNotFoundException("Selected file does not exist.", path);
            }
            
            List<SensorData> dataSet = new List<SensorData>();

            // Open the file using FileStream and BinaryReader.
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {         
                // Read all double values in sequence until the end of the file.
                while (fs.Position < fs.Length)
                {

                    //int strLen = br.ReadInt32();
                    //string label = Encoding.UTF8.GetString(br.ReadBytes(strLen));
                    string label = br.ReadString();
                    double value = br.ReadDouble();

                    dataSet.Add(new SensorData(label, value));
                   

                }
            }
            return dataSet;
        }

        /// <summary>
        /// Saves the provided list of SensorData into a binary (.bin) file.
        /// Only the Value property is written sequentially as double values.
        /// </summary>
        /// <param name="path">The destination file path.</param>
        /// <param name="data">The list of SensorData objects to save.</param>
        /// <returns>True if the save operation is successful; otherwise, false.</returns>
        public bool SaveFile(string path, List<SensorData> data)
        {
            try
            {
                // Create and write to the file using FileStream and BinaryWriter.
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    // Iterate over every SensorData object in the data list.
                    foreach (SensorData dataItem in data)
                    {
                        bw.Write(dataItem.Label);
                        bw.Write(dataItem.Value);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
