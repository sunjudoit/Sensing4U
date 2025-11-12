using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

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

        /// <summary>
        /// Analyzes the values ​​in the current dataset using the user-entered upper and lower bounds.
        /// Returns a list of colors (Red, Blue, or Green) corresponding to each data point.
        /// </summary>
        /// <param name="lower">The lower bound.</param>
        /// <param name="upper">The upper bound.</param>
        /// <returns>A pointer to a List<Color> object, or null if the dataset is empty.</returns>
        public List<Color> ApplyColor(double lower, double upper)
        {
            var currentDataset = GetCurrent();

            // Check if the dataset is null or contains no data points.
            if (currentDataset == null || currentDataset.Count == 0)
                return null;

            var color = new List<Color>();

            foreach (var data in currentDataset)
            {
                if (data.Value > upper)
                    color.Add(Color.Red);       // Value exceeds the upper limit.
                else if (data.Value < lower)
                    color.Add(Color.Blue);      // Value is below the lower limit.
                else
                    color.Add(Color.Green);     // Value is within the user bound range.
            }

            return color;
        }
        /// <summary>
        /// Sorts the currently selected data set in ascending order by value.
        /// </summary>
        public void SortData()
        {
            var currentDataset = GetCurrent();
            if (currentDataset == null || currentDataset.Count == 0)
                return;

 
            currentDataset.Sort((a, b) => a.Value.CompareTo(b.Value)); // Timsort
        }
    
        /// <summary>
        /// Performs a binary search on the sorted current dataset for the given value.
        /// </summary>
        /// <param name="target">The double value to search for.</param>
        /// <returns>The index (row number) of the matching data point, or -1 if not found.</returns>
        public int BinarySearch(double target)
        {
            var currentDataset = GetCurrent();

            if (currentDataset == null || currentDataset.Count == 0)
                return -1;

            //Initialize the index for the search range.
            int startPoint = 0;
            int endPoint = currentDataset.Count()-1;
            int midPoint = 0;
            while (startPoint <= endPoint)
            {
                midPoint = (startPoint + endPoint) /2;
                double midValue = currentDataset[midPoint].Value;

                if (Math.Abs(midValue - target) < 0.001) //Comparing within a tolerance
                {
                    return midPoint;
                }
                else if (midValue < target)
                {
                    startPoint = midPoint + 1;
                }
                else
                {
                    endPoint = midPoint -1 ;
                }
            }

            return -1; // Target value was not found in the dataset.
        }

    }
}
