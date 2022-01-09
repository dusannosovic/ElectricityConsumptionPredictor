using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PredictionModel
{
    class TraningOptions
    {
        public int BatchSize { get; set; } = 4;
        public int EpochNumber { get; set; } = 350;
        public string CostFunction { get; set; } = "mean_squared_error";
        public string Optimizer { get; set; } = "adamax";
        public string KernelInitializer { get; set; } = "random_normal";
        public string ActivationFunction { get; set; } = "sigmoid";
        public int NumberOfHiddenLayers { get; set; } = 4;
        public int NumberOfNeuronsInFirstHiddenLayer { get; set; } = 8;
        public int NumberOfNeuronsInOtherHiddenLayers { get; set; } = 6;
        public int Verbose { get; set; } = 2;

        public int InputDim { get; set; } = 13;

        public TraningOptions()
        {

        }
    }
}
