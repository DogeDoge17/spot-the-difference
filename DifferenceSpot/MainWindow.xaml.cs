using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DifferenceSpot
{
    public partial class MainWindow : Window
    {
        OpenFileDialog openFile1 = new OpenFileDialog();
        static System.Windows.Controls.Image processedImage = new System.Windows.Controls.Image();
        static System.Windows.Controls.Image processedImage2 = new System.Windows.Controls.Image();
        OpenFileDialog openFile2 = new OpenFileDialog();

        bool imageOneChosen = false;
        bool imageTwoChosen = false;
        DirectBitmap processedBitmap;
        DirectBitmap directOne;
        DirectBitmap directTwo;
        Bitmap bitOne;
        Bitmap bitTwo;
        BitmapFrame firstImage;
        BitmapFrame secondImage;

        BmpPixelSnoop pixelSnoop1;
        BmpPixelSnoop pixelSnoop2;
        BitmapFrame referenceImage;


#pragma warning disable CS8618 
        public MainWindow()
#pragma warning restore CS8618 
        {
            InitializeComponent();

            openFile1.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg;*.jpg";
            openFile2.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg;*.jpg";
            Update();
        }

        async void Update()
        {
            if (imageOneChosen == false && imageTwoChosen == false)
            {
                compareImages.Background = System.Windows.Media.Brushes.Red;
            }
            else if (imageTwoChosen == false)
            {
                compareImages.Background = System.Windows.Media.Brushes.Red;
            }
            else if (imageOneChosen == false)
            {
                compareImages.Background = System.Windows.Media.Brushes.Red;
            }
            else if (firstImage.Width != secondImage.Width || firstImage.Height != secondImage.Height)
            {
                compareImages.Background = System.Windows.Media.Brushes.Yellow;
            }
            else
            {
                compareImages.Background = System.Windows.Media.Brushes.Green;
            }

            foundRLbl.Content = MathF.Round(((float)foundRSlider.Value));
            foundGLbl.Content = MathF.Round(((float)foundGSlider.Value));
            foundBLbl.Content = MathF.Round(((float)foundBSlider.Value));

            nothingRLbl.Content = MathF.Round(((float)nothingRSlider.Value));
            nothingGLbl.Content = MathF.Round(((float)nothingGSlider.Value));
            nothingBLbl.Content = MathF.Round(((float)nothingBSlider.Value));

            await Task.Delay(250);
            Update();
        }

            

        void ProcessImage()
        {



            try
            {
                if (firstImage.Width <= secondImage.Width)
                {
                    referenceImage = firstImage;
                }
                else
                {
                    referenceImage = secondImage;
                }
            }catch
            {
                MessageBox.Show("What did I tell you about the size????? Come on man, Get with the program.", "You Messed Up", MessageBoxButton.OK, MessageBoxImage.Error);
            }


            Window imageWindow = new Window();
            if ((bool)autoSizeChckBx.IsChecked)
            {
                imageWindow.Width = referenceImage.Width;
                imageWindow.Height = referenceImage.Height;
                imageWindow.ResizeMode = ResizeMode.NoResize;
            }
            else
            {
                imageWindow.Width = 400;
                imageWindow.Height = 400;
                imageWindow.ResizeMode = ResizeMode.CanResize;
            }

            imageWindow.WindowStyle = WindowStyle.ToolWindow;

            imageWindow.Title = "Image Result";
            imageWindow.Content = processedImage;
            imageWindow.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
            imageWindow.Show();

            processedBitmap = new DirectBitmap(((int)firstImage.Width), ((int)firstImage.Height));

            directOne = new DirectBitmap(((int)firstImage.Width), ((int)firstImage.Height));
            directOne.ImportBitmap(GetBitmap(firstImage));

            directTwo = new DirectBitmap(((int)secondImage.Width), ((int)secondImage.Height));
            directTwo.ImportBitmap(GetBitmap(secondImage));

            bitOne = GetBitmap(directOne.ConvertToSource());
            bitTwo = GetBitmap(directTwo.ConvertToSource());

            pixelSnoop1 = new BmpPixelSnoop(bitOne);
            pixelSnoop2 = new BmpPixelSnoop(bitTwo);


            for (int x = 0; x < referenceImage.Width - 1; x++)
            {
                for (int y = 0; y < referenceImage.Height - 1; y++)
                {
                    if (pixelSnoop1.GetPixel(x, y).ToArgb() == pixelSnoop2.GetPixel(x, y).ToArgb())
                    {

                        processedBitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(255, (int)nothingRSlider.Value, (int)nothingGSlider.Value, (int)nothingBSlider.Value));
                    }
                    else
                    {
                        processedBitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(255, (int)foundRSlider.Value, (int)foundGSlider.Value, (int)foundBSlider.Value));
                    }
                }
            }

            imageWindow.Width = processedBitmap.Width;
            imageWindow.Height = processedBitmap.Height;

            processedImage.Source = processedBitmap.ConvertToSource();


        }




        Bitmap GetBitmap(BitmapSource source)
        {
            Bitmap bmp = new Bitmap(
              source.PixelWidth,
              source.PixelHeight,
              System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            BitmapData data = bmp.LockBits(
              new Rectangle(System.Drawing.Point.Empty, bmp.Size),
              ImageLockMode.WriteOnly,
              System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            source.CopyPixels(
              Int32Rect.Empty,
              data.Scan0,
              data.Height * data.Stride,
              data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }


        private void chooseImageOneBtn_Click(object sender, RoutedEventArgs e)
        {
            openFile1.ShowDialog();
            try
            {
                imageOne.Source = BitmapFrame.Create(new Uri(openFile1.FileName, UriKind.Relative));
                firstImage = BitmapFrame.Create(new Uri(openFile1.FileName, UriKind.Relative));
                bitOne = (Bitmap)Bitmap.FromFile(openFile1.FileName);
                imageOneChosen = true;
            }
            catch
            {
                MessageBox.Show("Failed to open image", "Failure", MessageBoxButton.OK);
            }
        }

        private void chooseImageTwoBtn_Click(object sender, RoutedEventArgs e)
        {
            openFile2.ShowDialog();
            try
            {
                imageTwo.Source = BitmapFrame.Create(new Uri(openFile2.FileName, UriKind.Relative));
                secondImage = BitmapFrame.Create(new Uri(openFile2.FileName, UriKind.Relative));
                bitTwo = (Bitmap)Bitmap.FromFile(openFile2.FileName);
                imageTwoChosen = true;
            }
            catch
            {
                MessageBox.Show("Failed to open image", "Failure", MessageBoxButton.OK);
            }
        }

        private void compareImages_Click(object sender, RoutedEventArgs e)
        {
            if (imageOneChosen == false && imageTwoChosen == false)
            {
                MessageBox.Show("You must select an image for each box.", "You Messed Up", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (imageTwoChosen == false)
            {
                MessageBox.Show("You must select the second image.", "You Messed Up", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (imageOneChosen == false)
            {
                MessageBox.Show("You must select the first image.", "You Messed Up", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (firstImage.Width != secondImage.Width || firstImage.Height != secondImage.Height)
            {
                MessageBox.Show("Images are recommended to be the same size to comapare.", "You are you sure about that?", MessageBoxButton.OK, MessageBoxImage.Warning);
                ProcessImage();

            }
            else
            {
                ProcessImage();
            }
        }


    }

    public class DirectBitmap : IDisposable
    {
        public Bitmap Bitmap;
        public Int32[] Bits { get; private set; }
        public bool Disposed { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        protected GCHandle BitsHandle { get; private set; }

        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Bits = new Int32[width * height];
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            if (Bitmap == null)
                Bitmap = new Bitmap(width, height, width * 4, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
        }

        public void SetPixel(int x, int y, System.Drawing.Color colour)
        {
            int index = x + (y * Width);
            int col = colour.ToArgb();

            Bits[index] = col;
        }


        public System.Drawing.Color GetPixel(int x, int y)
        {
            int index = x + (y * Width);
            int col = Bits[index];
            System.Drawing.Color result = System.Drawing.Color.FromArgb(col);
            //Debug.WriteLine(result);
            return result;
        }

        public BitmapSource ConvertToSource()
        {
            var bitmapData = Bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, Bitmap.Width, Bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, Bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                Bitmap.HorizontalResolution, Bitmap.VerticalResolution,
                PixelFormats.Pbgra32, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            Bitmap.UnlockBits(bitmapData);

            return bitmapSource;
        }


        public void ImportBitmap(Bitmap map)
        {
            Bitmap = map;
        }

        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            Bitmap.Dispose();
            BitsHandle.Free();
        }
    }

    unsafe class BmpPixelSnoop : IDisposable
    {
        private readonly Bitmap wrappedBitmap;

        private BitmapData data = null;

        private readonly byte* scan0;

        private readonly int depth;

        private readonly int stride;

        private readonly int width;

        private readonly int height;

        public BmpPixelSnoop(Bitmap bitmap)
        {
            wrappedBitmap = bitmap ?? throw new ArgumentException("Bitmap parameter cannot be null", "bitmap");


            if (wrappedBitmap.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                throw new System.ArgumentException("Only PixelFormat.Format32bppArgb is supported", "bitmap");

            width = wrappedBitmap.Width;
            height = wrappedBitmap.Height;

            var rect = new Rectangle(0, 0, wrappedBitmap.Width, wrappedBitmap.Height);

            try
            {
                data = wrappedBitmap.LockBits(rect, ImageLockMode.ReadWrite, wrappedBitmap.PixelFormat);
            }
            catch (Exception ex)
            {
                throw new System.InvalidOperationException("Could not lock bitmap, is it already being snooped somewhere else?", ex);
            }

            depth = Bitmap.GetPixelFormatSize(data.PixelFormat) / 8;

            scan0 = (byte*)data.Scan0.ToPointer();

            stride = data.Stride;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (wrappedBitmap != null)
                    wrappedBitmap.UnlockBits(data);
            }

        }

        private byte* PixelPointer(int x, int y)
        {
            return scan0 + y * stride + x * depth;
        }

        public System.Drawing.Color GetPixel(int x, int y)
        {

            if (x < 0 || y < 0 || x >= width || y >= height)
                throw new ArgumentException("x or y coordinate is out of range");

            int a, r, g, b;

            byte* p = PixelPointer(x, y);

            b = *p++;
            g = *p++;
            r = *p++;
            a = *p;

            return System.Drawing.Color.FromArgb(a, r, g, b);
        }

        public void SetPixel(int x, int y, System.Drawing.Color col)
        {

            if (x < 0 || y < 0 || x >= width || y >= height)
                throw new ArgumentException("x or y coordinate is out of range");

            byte* p = PixelPointer(x, y);


            *p++ = col.B;
            *p++ = col.G;
            *p++ = col.R;
            *p = col.A;
        }
        public int Width { get { return width; } }

        public int Height { get { return height; } }
    }
}
