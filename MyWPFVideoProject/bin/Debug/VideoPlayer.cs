using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Drawing;
using System.Windows.Media.Imaging;


public abstract class VideoPlayer : DependencyObject, IDisposable
{
    [System.Runtime.InteropServices.DllImport("gdi32.dll")]
    public static extern bool DeleteObject(IntPtr hObject);

    private bool _isThreadAlive;
    private Thread _thread;

    public VideoPlayer()
    {
        _isThreadAlive = false;
    }

    /// <summary>
    /// Проверка готовности устройства к передаче изображений
    /// </summary>
    /// <returns></returns>
    public abstract bool isDeviceReady();
    /// <summary>
    /// Снять текущее изображение
    /// </summary>
    /// <returns></returns>
    public abstract Bitmap getCurrentImage();

    public void BeginPlay()
    {
        if (isDeviceReady())
        {
            _isThreadAlive = true;
            _thread = new Thread(new ThreadStart(ThreadFunk)) { IsBackground = true };
            _thread.Start();
        }
    }

    private void ThreadFunk()
    {
        while (_isThreadAlive)
        {
            try
            {
                IntPtr pImage = IntPtr.Zero;
                //viewbox.LayoutTransform = new ScaleTransform(scale, scale);

                Frame = getCurrentImage();
                if (Frame != null)
                {
                    //Bitmap innerFrame = Frame;
                    IntPtr hBitmap = Frame.GetHbitmap();
                    if (hBitmap != IntPtr.Zero)
                    {
                        this.Dispatcher.Invoke(new Action(
                                    () =>
                                    {
                                        CVCameraImage = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                            hBitmap, IntPtr.Zero,
                                            Int32Rect.Empty,
                                            BitmapSizeOptions.FromEmptyOptions());
                                    }));

                        DeleteObject(hBitmap);
                    }
                }
                Thread.Sleep(40);
            }
            catch (Exception exc)
            { string str = exc.Message; }
        }
    }

    public void Stop()
    {
        if (_isThreadAlive)
        {
            _isThreadAlive = false;
            //_thread.Join(Timeout.Infinite);
        }
    }

    public static DependencyProperty CVCameraImageProperty = DependencyProperty.Register(
            "CVCameraImage",
            typeof(BitmapSource),
            typeof(VideoPlayer),
            new PropertyMetadata(null));


    public BitmapSource CVCameraImage
    {
        get
        {
            return (BitmapSource)GetValue(CVCameraImageProperty);
        }
        private set
        {
            SetValue(CVCameraImageProperty, value);
        }
    }

    private Bitmap _frame;
    public Bitmap Frame
    {
        get
        {
            //lock (lockObje)
            //{
            return _frame;
            // }
        }
        private set
        {
            // lock (lockObje)
            // {
            _frame = value;
            //  }
        }
    }

    public void Dispose()
    {
    }

}



