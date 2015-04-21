using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using ASPK.APM.Wrappers;

namespace MyWPFVideoProject
{
	/// <summary>
	/// Interaction logic for MakeShotControl.xaml
	/// </summary>
	public partial class MakeShotControl : UserControl
	{
        bool firsttime = false;
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        List<ScreenShot> screenShotList = new List<ScreenShot>();

        bool deleteFlag = false;

        Bitmap tempBit;

        private FilterInfoCollection videoCaptureDevices;
        private VideoCaptureDevice MyDevice;
        private List<StackPanel> stackPanelList = new List<StackPanel>();


		public MakeShotControl()
		{
			this.InitializeComponent();
		}

        //Метод для проверки, на сканнере ли документ;
        private bool PhotoCheck(Bitmap temp)
        {
            ASPKPassportPageLayoutDll.FieldRecRectangles rectangles =
                    new ASPKPassportPageLayoutDll.FieldRecRectangles();
            //Магия
            double dpi = (int)(514.0 * temp.Width / 2590.0);

            int result = ASPKPassportPageLayoutDll.LayoutIR(temp, out rectangles, dpi);

            bool isDocumentOnScanner = (result == 0) ? true : false;

            return isDocumentOnScanner;
        }

        //Скриншот
        private void button4_Click(object sender, RoutedEventArgs e)
        {
            ScreenShot shot = new ScreenShot();

            string path;
            path = System.IO.Path.GetDirectoryName(
               System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);


          
            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(path + @"\delete.png", UriKind.RelativeOrAbsolute);
            src.EndInit();


            shot.deleteImageSet(src);

            shot.rawImageSet(tempBit);
            shot.preview.MouseLeftButtonDown+=new System.Windows.Input.MouseButtonEventHandler(preview_MouseLeftButtonDown);
            shot.deleteImage.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(deleteImage_MouseLeftButtonDown);
            PhotoCheck(shot.rawImage);
            bool yz = PhotoCheck(shot.rawImage);
            //Раскомментируйте, если нужно прогонять каждый кадр через алгоритм
            //if (!PhotoCheck(shot.rawImage))
            //{
            //    shot.previewSet(src);

            //}
            //else
            //{
            shot.previewSet(tempBit);
            //}
            screenShotList.Add(shot);
            listBox1.Items.Add(shot.CreateListBoxElement(75, 25));

        }
        //Удалить скриншот
        void deleteImage_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {

                int selectedIndex = listBox1.SelectedIndex;
                listBox1.Items.RemoveAt(selectedIndex);
                screenShotList.RemoveAt(selectedIndex);
            }
        }
        //щелчок по миниатюре
        void preview_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {

                ImageWindow imgWin = new ImageWindow();
                imgWin.SetImage1(screenShotList[listBox1.SelectedIndex].rawImageGet());
                imgWin.ShowDialog();

            }
        }
        //Удалить последний шот
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (listBox1.Items.Count != 0)
            {
                int a = listBox1.Items.Count;
                //ImageList.RemoveAt((int)listBox1.Items.Count-1);
                screenShotList.RemoveAt((int)listBox1.Items.Count - 1);
                listBox1.Items.RemoveAt(((int)listBox1.Items.Count - 1));
            }
        }
        //Тут все ясно
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            videoCaptureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            MyDevice = new VideoCaptureDevice(videoCaptureDevices[0].MonikerString);
            MyDevice.NewFrame += new NewFrameEventHandler(MyDevice_NewFrame);
            MyDevice.Start();
        }
        //Вызывается каждый новый кадр
        void MyDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap img = (Bitmap)eventArgs.Frame.Clone();
            tempBit = (Bitmap)eventArgs.Frame.Clone();
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, (SendOrPostCallback)delegate
            {
                IntPtr hBitmap = img.GetHbitmap();
                System.Windows.Media.Imaging.BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());


                DeleteObject(hBitmap);

                img.Dispose();
                GC.Collect();
                image1.Source = bitmapSource;

            }, null);
        }
        //Ниже идет остановка камеры, выгрузка и прочая подчистка. 
        //За неимением лучшего пришлось подвесить выгрузку на isVisible
        //под страхом 220В не касайтесь ISVisible этого контрола!!!  >_<
        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (firsttime == false)
                firsttime = true;
            else
            {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, (SendOrPostCallback)delegate
            {
                MyDevice.SignalToStop();
                MyDevice.WaitForStop();
                MyDevice.Source = null;
            }, null);
            }

        }
	}
}