﻿using Knapcode.ExplorePackages.Worker;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Knapcode.ExplorePackages.Worker
{
    public class MetricWrapper : IMetric
    {
        private Metric _inner;

        public MetricWrapper(Metric metric)
        {
            _inner = metric;
        }

        public bool TrackValue(double metricValue, string dimension1Value)
        {
            return _inner.TrackValue(metricValue, dimension1Value);
        }

        public bool TrackValue(double metricValue, string dimension1Value, string dimension2Value)
        {
            return _inner.TrackValue(metricValue, dimension1Value, dimension2Value);
        }
    }
}
