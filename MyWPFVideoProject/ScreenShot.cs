using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ASPK.APM.Wrappers;
using System.Threading;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing;

namespace MyWPFVideoProject
{
    class ScreenShot 
    {
       public StackPanel stck = new StackPanel();
       public System.Windows.Controls.Image preview = new System.Windows.Controls.Image();
       public System.Windows.Controls.Image deleteImage = new System.Windows.Controls.Image();
       public Bitmap rawImage ;
       public bool  deleteFlag = false;

       #region get\set stuff
       public ScreenShot()
        {
            
        }

        public void previewSet(Bitmap bitmap)
        {
            preview.Source = ToBitmapSource(bitmap);
            
        }
        public void previewSet(BitmapImage bitmap)
        {
            preview.Source = bitmap;
           // preview.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(preview_MouseLeftButtonDown);
        }

      
         public System.Windows.Controls.Image previewGet()
        {
              return preview;
        }


         public void deleteImageSet(BitmapImage bitmap)
         {
             deleteImage.Source = bitmap;
             deleteImage.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(deleteImage_MouseLeftButtonDown);
         }
       

         void deleteImage_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
         {
             deleteFlag = true;
         }

         public System.Windows.Controls.Image deleteImageGet()
         {
             return deleteImage;
         }

         public void rawImageSet(Bitmap bitmap)
         {
              rawImage = bitmap;
         }

         public BitmapSource rawImageGet()
         {
             return ToBitmapSource(rawImage);
         }
       #endregion

         public  BitmapSource ToBitmapSource(System.Drawing.Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(source.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
              System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
        }

       

       public StackPanel CreateListBoxElement(int previewWidth, int deleteImgWidth)
        {

            
                this.deleteImage.Source = deleteImage.Source;
                this.deleteImage.Height = deleteImgWidth;
                this.deleteImage.Width = deleteImgWidth;
               // this.preview.Source = ToBitmapSource(rawImage);
                this.preview.Height = previewWidth;
                this.preview.Width = previewWidth;
                this.deleteImage.HorizontalAlignment = HorizontalAlignment.Right;
                this.deleteImage.VerticalAlignment = VerticalAlignment.Bottom;

                stck.Orientation = Orientation.Horizontal;
                stck.Children.Add(preview);
                stck.Children.Add(deleteImage);
                return stck;
            

        }


        
    }
}
