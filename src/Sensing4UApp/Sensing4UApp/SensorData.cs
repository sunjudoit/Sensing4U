using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sensing4UApp
{
    /// <summary>
    /// The SensorData class represents a single row(Label , Value )of sensor information.
    /// This class is used across the application to store and manage individual sensor readings.
    /// </summary>
    public class SensorData
    {
        /// <summary>
        /// Allows reading and writing of the sensor’s label or ID.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Allows reading and writing of the sensor’s value.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Constructor used to initialize a new SensorData object with a label and value.
        /// </summary>
        /// <param name="label">The ID of the sensor.</param>
        /// <param name="value">The recorded value for the sensor.</param>
        public SensorData(string label, double value) 
        {
            Label = label;
            Value = value;
        }

        
    }


}
