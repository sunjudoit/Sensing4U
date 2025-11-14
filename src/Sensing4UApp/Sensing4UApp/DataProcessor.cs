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
        /// Analyzes the values in the current dataset using the user-entered upper and lower bounds.
        /// Returns a 2D array of colors  corresponding to each data point.
        /// </summary>
        /// <param name="lower">The lower bound.</param>
        /// <param name="upper">The upper bound.</param>
        /// <returns>
        /// A Color[,] 2D array where each row represents a data point and the single column
        /// contains the mapped color, or null if the dataset is empty.
        /// </returns>
        public Color[,] ApplyColor(double lower, double upper)
        {
            var currentDataset = GetCurrent();

            // Check if the dataset is null or contains no data points.
            if (currentDataset == null || currentDataset.Count == 0)
                return null;

            int rowCount = currentDataset.Count;
            int colCount = 1;

            Color[,] color = new Color[rowCount, colCount];

            // 1:1 mapping between a list of data and a 2D array of colors.
           for (int i = 0; i < rowCount; i++)
            {
                double value = currentDataset[i].Value;

                if (value > upper)
                    color[i, 0] = Color.Red;
                else if (value < lower)
                    color[i, 0] = Color.Blue;
                else
                    color[i, 0] = Color.Green;  
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

 
            currentDataset.Sort((a, b) => a.Value.CompareTo(b.Value)); 
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

                if (midValue == target)
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



        /// <summary>
        /// Calculates the average of the 'Value' property  in the currently active dataset.
        /// </summary>
        /// <returns>
        /// The calculated average as a double, or double.NaN if the dataset is null or empty.
        /// </returns>
        public double AverageData()
        {
            var currentDataset = GetCurrent();

            if (currentDataset == null || currentDataset.Count == 0)
                return double.NaN;

            int count = currentDataset.Count;
            double sum = 0;
            foreach (var data in currentDataset)
            {
                sum += data.Value;
            }
            return sum / count;

        }

    }
}
