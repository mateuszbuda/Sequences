using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using System.Reflection;
using System.IO;
using AWK;
using System.Drawing;

namespace AWK_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool playing = false;

        double speed = 1.0;
        int length = 10;
        int listSize = 4;
        int colors = 4;

        int i = 0;
        int tick = 0;
        Random r = new Random();

        const int ellipseSize = 36;
        const int gridColumnWidth = ellipseSize + 2;

        SolidColorBrush[] b = {   System.Windows.Media.Brushes.Red, 
                                  System.Windows.Media.Brushes.LawnGreen, 
                                  System.Windows.Media.Brushes.Yellow, 
                                  System.Windows.Media.Brushes.LightBlue,
                                  System.Windows.Media.Brushes.Aqua,
                                  System.Windows.Media.Brushes.HotPink,
                                  System.Windows.Media.Brushes.Salmon,
                                  System.Windows.Media.Brushes.DarkMagenta,
                                  System.Windows.Media.Brushes.Peru,
                                  System.Windows.Media.Brushes.Tan
                              };

        Grid seq;
        Grid ind;
        Grid list;
        Canvas img;
        Button pause;
        Ellipse[] ellipses;
        Label[] rands;
        DispatcherTimer timer = null;
        BitmapImage arrow;
        Bitmap image;

        Sequence S;


        public MainWindow()
        {
            InitializeComponent();
            pause = this.PauseResume_Button;
            pause.IsEnabled = false;
            Next.IsEnabled = false;
        }

        private void Init_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                length = Convert.ToInt32(this.length_TextBox.Text);
                listSize = Convert.ToInt32(this.listSize_TextBox.Text);
                colors = Convert.ToInt32(this.colors_TextBox.Text);
            }
            catch (FormatException)
            {
                length = 0;
                listSize = 4;
                colors = 4;
            }

            if (colors < listSize || colors > 10)
            {
                colors = 4;
                listSize = 4;
            }

            init();

            if (timer == null)
            {
                timer = new System.Windows.Threading.DispatcherTimer();
                timer.Tick += new EventHandler(dispatcherTimer_Tick);
                timer.Interval = new TimeSpan(0, 0, 0, 0, (int)(speed * 1000));
                timer.Start();
            }

            pause.IsEnabled = true;
            pause.Content = "Pause";
            Next.IsEnabled = true;
            playing = true;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (playing)
            {
                int h = 0;

                if (tick % 2 == 0 && i < S.seq.GetLength(0))
                {
                    S.seq[i] = r.Next(0, listSize);
                    this.Rand.Content = Convert.ToString(S.seq[i]);

                    ellipses[i].Fill = b[S.list[i, S.seq[i]]];
                    rands[i].Content = Convert.ToString(S.seq[i]);

                    h = S.isRepetitive(i);

                    i = i - h + 1;

                    S.cnt++;
                    this.Loop.Content = Convert.ToString(S.cnt);
                }

                if (tick % 2 != 0 && i < S.seq.GetLength(0))
                {
                    ind.Children.Clear();
                    img = new Canvas()
                    {
                        Name = "arrow",
                        Background = new ImageBrush()
                        {
                            //ImageSource = new BitmapImage(new Uri(@"Resources/Down-Arrow-Icon.jpg", UriKind.RelativeOrAbsolute)),
                            ImageSource = arrow,
                        }
                    };

                    this.I.Content = Convert.ToString(i);

                    ind.Children.Add(img);
                    Grid.SetColumn(img, i);

                    scroll.ScrollToHorizontalOffset((double)(i - (int)(this.ActualWidth / 2 / gridColumnWidth)) * (scroll.ScrollableWidth + this.ActualWidth) / (double)length);
                    //scroll.ScrollToRightEnd();

                    int j = i;
                    while (j < ellipses.GetLength(0) && ellipses[j].Fill != System.Windows.Media.Brushes.White)
                    {
                        ellipses[j].Fill = System.Windows.Media.Brushes.White;
                        rands[j].Content = "";
                        j++;
                    }
                }

                tick++;
            }
        }

        private void step()
        {
            int h = 0;

            if (tick % 2 == 0 && i < S.seq.GetLength(0))
            {
                S.seq[i] = r.Next(0, listSize);
                this.Rand.Content = Convert.ToString(S.seq[i]);

                ellipses[i].Fill = b[S.list[i, S.seq[i]]];
                rands[i].Content = Convert.ToString(S.seq[i]);

                h = S.isRepetitive(i);

                i = i - h + 1;

                S.cnt++;
                this.Loop.Content = Convert.ToString(S.cnt);
            }

            if (tick % 2 != 0 && i < S.seq.GetLength(0))
            {
                ind.Children.Clear();
                img = new Canvas()
                {
                    Name = "arrow",
                    Background = new ImageBrush()
                    {
                        //ImageSource = new BitmapImage(new Uri(@"Resources/Down-Arrow-Icon.jpg", UriKind.Relative)),
                        ImageSource = arrow,
                    }
                };

                this.I.Content = Convert.ToString(i);

                ind.Children.Add(img);
                Grid.SetColumn(img, i);

                int j = i;
                while (j < ellipses.GetLength(0) && ellipses[j].Fill != System.Windows.Media.Brushes.White)
                {
                    ellipses[j].Fill = System.Windows.Media.Brushes.White;
                    rands[j].Content = "";
                    j++;
                }
            }

            tick++;
        }

        private void init()
        {
            this.i = 0;
            scroll.ScrollToHome();

            S = new Sequence(length, 0, colors - 1, listSize);

            this.I.Content = Convert.ToString(i);
            this.Loop.Content = Convert.ToString(S.cnt);

            seq = this.Sequence;
            ind = this.Index;
            list = this.Lists;

            seq.ColumnDefinitions.Clear();
            seq.Children.Clear();

            ind.ColumnDefinitions.Clear();
            ind.Children.Clear();

            list.ColumnDefinitions.Clear();
            list.Children.Clear();

            for (int k = 0; k < length; k++)
            {
                seq.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(gridColumnWidth),
                });

                ind.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(gridColumnWidth),
                });

                list.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(gridColumnWidth),
                });
            }

            ellipses = new Ellipse[length];
            rands = new Label[length];

            for (int k = 0; k < length; k++)
            {
                Ellipse e = new Ellipse
                {
                    Width = ellipseSize,
                    Height = ellipseSize,
                    Fill = System.Windows.Media.Brushes.White,
                    Stroke = System.Windows.Media.Brushes.Black,
                };

                Label r = new Label
                {
                    Content = "",
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                };

                seq.Children.Add(e);
                seq.Children.Add(r);

                Grid.SetColumn(e, k);
                Grid.SetColumn(r, k);

                ellipses[k] = e;
                rands[k] = r;

                list.RowDefinitions.Clear();

                for (int j = 0; j < S.listSize; j++)
                    list.RowDefinitions.Add(new RowDefinition
                    {
                        Height = new GridLength(ellipseSize),
                    });

                for (int j = 0; j < S.listSize; j++)
                {
                    Ellipse f = new Ellipse
                    {
                        Width = ellipseSize,
                        Height = ellipseSize,
                        Fill = b[S.list[k, j]],
                        Stroke = System.Windows.Media.Brushes.Black,
                    };

                    Label l = new Label
                    {
                        Content = Convert.ToString(j),
                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    };

                    list.Children.Add(f);
                    list.Children.Add(l);
                    Grid.SetColumn(f, k);
                    Grid.SetColumn(l, k);
                    Grid.SetRow(f, j);
                    Grid.SetRow(l, j);
                }
            }

            if (length != 0)
            {
                img = new Canvas()
                {
                    Name = "arrow",
                    Background = new ImageBrush()
                    {
                        //ImageSource = new BitmapImage(new Uri(@"Resources/Down-Arrow-Icon.jpg", UriKind.RelativeOrAbsolute)),
                        //ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/Arrow.jpeg", UriKind.RelativeOrAbsolute)),
                        ImageSource = arrow,
                    }
                };

                ind.Children.Add(img);
                Grid.SetColumn(img, 0);
            }

            this.Height = ellipseSize * (listSize + 6);
        }

        private void PauseResume_Button_Click(object sender, RoutedEventArgs e)
        {
            if (playing)
            {
                playing = false;
                pause.Content = "Resume";
            }

            else
            {
                playing = true;
                pause.Content = "Pause";
            }
        }

        private void step(object sender, RoutedEventArgs e)
        {
            if (!playing)
                step();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            speed = 4 - e.NewValue * 2;
            if (timer != null)
            {
                timer.Interval = new TimeSpan(0, 0, 0, 0, (int)(speed * 1000));
                System.Console.WriteLine((int)(speed * 1000));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                Stream myStream = myAssembly.GetManifestResourceStream("AWK_.Resources.Arrow.jpeg");
                image = new Bitmap(myStream);

                using (MemoryStream memory = new MemoryStream())
                {
                    image.Save(memory, System.Drawing.Imaging.ImageFormat.Jpeg);
                    memory.Position = 0;
                    arrow = new BitmapImage();
                    arrow.BeginInit();
                    memory.Seek(0, SeekOrigin.Begin);
                    arrow.StreamSource = memory;
                    arrow.CacheOption = BitmapCacheOption.OnLoad;
                    arrow.EndInit();
                }
            }
            catch
            {
                MessageBox.Show("Error accessing resources!");
            }

        }

    } // class MainWindow
} // namespace AWK_
