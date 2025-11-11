using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing4UApp
{
    public class DataProcessor
    {
        /// The single, static instance created upon program startup
        private static readonly DataProcessor _instance = new DataProcessor();

        // Provides global access points
        public static DataProcessor Instance => _instance;

        private readonly List<List<SensorData>> DatasetCollection;
        private int currentIndex;

        private DataProcessor() 
        {
            DatasetCollection = new List<List<SensorData>>();
            currentIndex = -1; // -1 indicates no dataset is currently loaded
        }

        /// <summary>
        /// Adds a new loaded dataset to the collection and sets it as the current active dataset.
        /// </summary>
        /// <param name="dataset">The list of SensorData to add.</param>
        public void AddLoadedDataset(List<SensorData> dataset)
        {
            if (dataset == null || dataset.Count == 0)
                return;
            DatasetCollection.Add(dataset);
            currentIndex = DatasetCollection.Count - 1;
        }

        /// <summary>
        /// Retrieves the currently active dataset based on currentIndex.
        /// </summary>
        /// <returns>The active List<SensorData> or null if no dataset is active.</returns>
        public List<SensorData> GetCurrent()
        {

            if (currentIndex < 0 || currentIndex >= DatasetCollection.Count)
                return null;
            return DatasetCollection[currentIndex];

        }
        /// <summary>
        /// Moves the currentIndex to the next dataset in the collection, if available.
        /// </summary>
        /// <returns>The next List<SensorData> or the current one if already at the end.</returns>
        public List<SensorData> Next()
        {
            if (DatasetCollection.Count == 0)
                return null;
            
            if (currentIndex < DatasetCollection.Count - 1) //if it's not pointing to the last item.
                currentIndex++;

            return DatasetCollection[currentIndex];
        }
        /// <summary>
        /// Moves the currentIndex to the previous dataset in the collection, if available.
        /// </summary>
        /// <returns>The previous List<SensorData> or the current one if already at the beginning.</returns>
        public List<SensorData> Prev()
        {
            if (DatasetCollection.Count == 0)
                return null;

            if (currentIndex > 0) //if it's not at the beginning
                currentIndex--;

            return DatasetCollection[currentIndex];
        }
    }
}
