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
using AcTelemetryC;

namespace AcTelemetryUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DataReader _reader = new();
        private bool _isConnected = false;

        private int _lastSpeed = -1;
        private int _lastRpm = -1;
        private int _lastGear = -1;
        public MainWindow()
        {
            InitializeComponent();

            CompositionTarget.Rendering += OnRendering;
        }

        private void OnRendering(object sender, EventArgs e)
        {
            if (!_isConnected)
            {
                if (_reader.TryConnect())
                {
                    _isConnected = true;
                    StatusText.Text = "Connected";
                    StatusText.Foreground = new SolidColorBrush(Color.FromRgb(0, 230, 118));
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

                GasBar.Value = data.Gas * 100f;
                BrakeBar.Value = data.Brake * 100f;
            }
            else
            {
                _isConnected = false;
                StatusText.Text = "Waiting";
                StatusText.Foreground = new SolidColorBrush(Color.FromRgb(255, 165, 0));
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