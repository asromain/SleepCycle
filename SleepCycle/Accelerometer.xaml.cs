using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.Devices.Sensors;
using Windows.UI.Xaml.Shapes;
using Windows.UI;

// Pour en savoir plus sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=391641

namespace SleepCycle
{
    public sealed partial class MainPageBackup : Page
    {
        private double iterateur;
        private Accelerometer accelerometer;

        private AccelerometerReading previousReading = null;
        private AccelerometerReading currentReading = null;

        public MainPageBackup()
        {
            this.InitializeComponent();

            iterateur = 0;
            // GetDefault retourne une valeur null si pas d'accelero
            accelerometer = Accelerometer.GetDefault();

            // test l'existence d'un accelerometre sur le portable
            if (accelerometer != null)
            {
                startAccelerometer(false);
            }

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoqué lorsque cette page est sur le point d'être affichée dans un frame.
        /// </summary>
        /// <param name="e">Données d'événement décrivant la manière dont l'utilisateur a accédé à cette page.
        /// Ce paramètre est généralement utilisé pour configurer la page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: préparer la page pour affichage ici.

            // TODO: si votre application comporte plusieurs pages, assurez-vous que vous
            // gérez le bouton Retour physique en vous inscrivant à l’événement
            // Événement Windows.Phone.UI.Input.HardwareButtons.BackPressed.
            // Si vous utilisez le NavigationHelper fourni par certains modèles,
            // cet événement est géré automatiquement.
        }

        /**
                 *   Appelee lorsque l'accelerometre detecte un evenement ReadingChanged
                 */
        private async void myHandler(object sender, AccelerometerReadingChangedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => updateMyScreen(e));
        }

        private void updateMyScreen(AccelerometerReadingChangedEventArgs e)
        {
            // updates the textblocks

            previousReading = currentReading;
            currentReading = e.Reading;

            if (previousReading != null)
            {

                double xValue = currentReading.AccelerationX - previousReading.AccelerationX;
                double yValue = currentReading.AccelerationY - previousReading.AccelerationY;
                double zValue = currentReading.AccelerationZ - previousReading.AccelerationZ;

                if (xValue < 0) xValue -= 2 * xValue;
                if (yValue < 0) yValue -= 2 * yValue;
                if (zValue < 0) zValue -= 2 * zValue;

                data_width.Text = String.Format("{0,5:0.00}", xValue);
                data_length.Text = String.Format("{0,5:0.00}", yValue);
                data_depth.Text = String.Format("{0,5:0.00}", zValue);

                // draws on the canvas

                double currentXOnGraph = Math.Abs((xValue * 200) - 200);
                double currentYOnGraph = Math.Abs((yValue * 200) - 200);
                double currentZOnGraph = Math.Abs((zValue * 200) - 200);

                Rectangle xPoint = new Rectangle();
                Rectangle yPoint = new Rectangle();
                Rectangle zPoint = new Rectangle();

                xPoint.Fill = new SolidColorBrush(Colors.Red);
                yPoint.Fill = new SolidColorBrush(Colors.Blue);
                zPoint.Fill = new SolidColorBrush(Colors.Green);

                // set the pixel size 

                xPoint.Width = 1;
                xPoint.Height = 2;
                yPoint.Width = 1;
                yPoint.Height = 2;
                zPoint.Width = 1;
                zPoint.Height = 2;

                // These pixels will be "pasted into" the canvas by setting their position 
                // according to the currentX/Y/Z-on-graph values. To set their position 
                // relative to the canvas, pass the canvas properties on to the pixels via 
                // the SetValue method. Use a generic iterator to determine the distance 
                // the pixel should be from the left side of the canvas.

                xPoint.SetValue(Canvas.LeftProperty, iterateur);
                xPoint.SetValue(Canvas.TopProperty, currentXOnGraph);
                yPoint.SetValue(Canvas.LeftProperty, iterateur);
                yPoint.SetValue(Canvas.TopProperty, currentYOnGraph);
                zPoint.SetValue(Canvas.LeftProperty, iterateur);
                zPoint.SetValue(Canvas.TopProperty, currentZOnGraph);

                // finally, associate pixels with the canvas

                my_canvas.Children.Add(xPoint);
                my_canvas.Children.Add(yPoint);
                my_canvas.Children.Add(zPoint);

                if (iterateur == 399)
                {
                    my_canvas.Children.Clear();
                    iterateur = 0;
                }
                else
                {
                    iterateur++;
                }
            }

        }
        
        /**
         *  Handler boutton start_and_stop
         */
        private void start_and_stop_Click(object sender, RoutedEventArgs e)
        {
            if (start_and_stop.Content.Equals("PAUSE"))
            {
                // stop
                start_and_stop.Content = "START";
                startAccelerometer(false);
            }
            else if (start_and_stop.Content.Equals("START"))
            {
                // start
                startAccelerometer(true);
                start_and_stop.Content = "PAUSE";
            }
        }

        /**
         *   Demarrer ou arreter l'accelero
         */
        private void startAccelerometer(Boolean directive)
        {
            if (directive == true)
            {
                accelerometer.ReadingChanged += new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(myHandler);
            }
            else
            {
                accelerometer.ReadingChanged -= new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(myHandler);
            }
        }
    }
}
