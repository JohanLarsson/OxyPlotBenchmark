using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlotBenchmark.Annotations;

namespace OxyPlotBenchmark
{
    public class Vm : INotifyPropertyChanged
    {
        public Vm()
        {
            PlotModel = new PlotModel("Matts Title", "subtitle");
            PlotModel.Axes.Add(new LinearAxis(AxisPosition.Bottom, "X"));
            PlotModel.Axes.Add(new LinearAxis(AxisPosition.Left, "Y"));
            _lineSeries = new LineSeries(OxyColors.Blue, title: "LineTitle");
            _lineSeries.DataFieldX = "X";
            _lineSeries.DataFieldY = "Y";
            PlotModel.Series.Add(_lineSeries);
            NumberOfPoints = 64000;
            MillisecondRefreshRate = 1000;
            Updater();

        }

        private int series = 0;
        private LineSeries _lineSeries;
        private double[] _xs;
        private List<double[]> _yss;

        public PlotModel PlotModel { get; set; }

        private int _millisecondRefreshRate;
        public int MillisecondRefreshRate
        {
            get { return _millisecondRefreshRate; }
            set
            {
                if (value == _millisecondRefreshRate) return;
                _millisecondRefreshRate = value;
                OnPropertyChanged();
            }
        }

        private long _actualRefreshRate;
        public long ActualRefreshRate
        {
            get { return _actualRefreshRate; }
            set
            {
                if (value == _actualRefreshRate) return;
                _actualRefreshRate = value;
                OnPropertyChanged();
            }
        }

        private int _numberOfPoints;
        public int NumberOfPoints
        {
            get { return _numberOfPoints; }
            set
            {
                if (value == _numberOfPoints) return;
                _numberOfPoints = value;
                OnPropertyChanged();
                UpdateNumberOfPoints();
            }
        }
        
        public void UpdateNumberOfPoints()
        {
            var r = new Random();
            _xs = Enumerable.Range(1, NumberOfPoints).Select(Convert.ToDouble).ToArray();
            double[] y1 = _xs.Select(x => Math.Sin(x) * (Math.Pow(Math.E, -0.2*x) + r.NextDouble() ) ).ToArray();
            double[] y2 = _xs.Select(x => Math.Sin(x) * (Math.Pow(Math.E, -0.2 * x) + r.NextDouble())).ToArray();
            double[] y3 = _xs.Select(x => Math.Sin(x) * (Math.Pow(Math.E, -0.2 * x) + r.NextDouble())).ToArray();
            double[] y4 = _xs.Select(x => Math.Sin(x) * (Math.Pow(Math.E, -0.2 * x) + r.NextDouble())).ToArray();
            _yss = new List<double[]>
                {
                    y1, y2, y3, y4
                };

        }

        public void Update()
        {
            if (series > 3)
                series = 0;
            var points = new DataPoint[NumberOfPoints];
            for (int i = 0; i < _xs.Length; i++)
            {
                points[i] = new DataPoint(_xs[i], _yss[series][i]);
            }
            series++;
            _lineSeries.ItemsSource = points;
            PlotModel.InvalidatePlot(true);
        }

        public async Task Updater()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (true)
            {
                stopwatch.Restart();
                Update();
                await Task.Delay(MillisecondRefreshRate);
                ActualRefreshRate = stopwatch.ElapsedMilliseconds;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
