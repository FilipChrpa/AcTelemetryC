using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ScottPlot;
using ScottPlot.Plottables;
using AcTelemetryC;

namespace AcTelemetryUI
{
    public partial class MainWindow : Window
    {
        private readonly DataReader _reader = new();
        private bool _isConnected = false;

        private int _lastSpeed = -1;
        private int _lastRpm = -1;
        private int _lastGear = -1;

        // Pedal Telemetry
        private const int BufferCapacity = 900;
        private DataStreamer _gasStreamer = null!;
        private DataStreamer _brakeStreamer = null!;

        public MainWindow()
        {
            InitializeComponent();
            SetupPlot();

            CompositionTarget.Rendering += OnRendering;
        }

        private void SetupPlot()
        {
            var plot = PedalsPlot.Plot;

            PedalsPlot.UserInputProcessor.IsEnabled = false;

            // Graph Bacground
            plot.FigureBackground.Color = ScottPlot.Color.FromHex("#1E1E1E");
            plot.DataBackground.Color = ScottPlot.Color.FromHex("#121212");
            plot.Axes.Color(ScottPlot.Color.FromHex("#555555"));
            plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#222222");

            // Pedal Travel 0 - 100%
            plot.Axes.Left.Min = 0;
            plot.Axes.Left.Max = 105;

            plot.Axes.Bottom.Min = 0;
            plot.Axes.Bottom.Max = BufferCapacity;

            // Hiding bottom axes
            plot.Axes.Bottom.TickLabelStyle.IsVisible = false;
            plot.Axes.Bottom.FrameLineStyle.IsVisible = false;

            // Throttle
            _gasStreamer = plot.Add.DataStreamer(BufferCapacity);
            _gasStreamer.Color = new ScottPlot.Color(0, 230, 118, 220);
            _gasStreamer.LineWidth = 2.5f;
            _gasStreamer.LegendText = "Gas";
            _gasStreamer.ManageAxisLimits = false;
            _gasStreamer.ViewScrollLeft();

            // Brake
            _brakeStreamer = plot.Add.DataStreamer(BufferCapacity);
            _brakeStreamer.Color = new ScottPlot.Color(255, 23, 68, 220);
            _brakeStreamer.LineWidth = 2.5f;
            _brakeStreamer.LegendText = "Brake";
            _brakeStreamer.ManageAxisLimits = false;
            _brakeStreamer.ViewScrollLeft();

            //Dissable legend and anti-aliasing
            plot.Legend.IsVisible = false;
            _gasStreamer.LineStyle.AntiAlias = false;
            _brakeStreamer.LineStyle.AntiAlias = false;
            //PedalsPlot.Refresh();
        }

        private void OnRendering(object? sender, EventArgs e)
        {
            if (!_isConnected)
            {
                if (_reader.TryConnect())
                {
                    _isConnected = true;
                    StatusText.Text = "Connected";
                    StatusText.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 230, 118));
                }
                return;
            }

            if (_reader.TryRead(out PhysicsData data))
            {
                int currentSpeed = (int)Math.Round(data.SpeedKmh);
                if (currentSpeed != _lastSpeed)
                {
                    SpeedText.Text = currentSpeed.ToString();
                    _lastSpeed = currentSpeed;
                }

                if (data.Rpms != _lastRpm)
                {
                    RpmText.Text = data.Rpms.ToString();
                    _lastRpm = data.Rpms;
                }

                if (data.Gear != _lastGear)
                {
                    GearText.Text = FormatGear(data.Gear);
                    _lastGear = data.Gear;
                }

                // AC pedals 0.0 - 1.0 
                _gasStreamer.Add(data.Gas * 100.0);
                _brakeStreamer.Add(data.Brake * 100.0);
                PedalsPlot.Refresh();
            }
            else
            {
                _isConnected = false;
                StatusText.Text = "Waiting";
                StatusText.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 165, 0));
            }
        }

        private static string FormatGear(int acGear) => acGear switch
        {
            0 => "R",
            1 => "N",
            _ => (acGear - 1).ToString()
        };

        protected override void OnClosed(EventArgs e)
        {
            CompositionTarget.Rendering -= OnRendering;
            _reader.Dispose();
            base.OnClosed(e);
        }
    }
}