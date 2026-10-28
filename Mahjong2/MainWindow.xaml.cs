using System;
using System.Collections.Generic;
using System.IO;
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

namespace Mahjong2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int numberOfBricksLeft = 152;
        DispatcherTimer timerRemove = new DispatcherTimer();
        //ska vara 152 st tot är 49 bilder (000-025 26*4=104 + (026-049) 24*2=48 tot 152 )
        public static List<ImageBrush> allBricksBr = new List<ImageBrush>();//53 st bilder 0-23 4 st sen 2 tot = 132 + 20
        Random rnd = new Random();
        List<int> ai = new List<int>();
        public delegate void UpdateTextCallback(string message);
        List<Brick> allBricksL1 = new List<Brick>();
        List<Brick> allBricksL2 = new List<Brick>();
        List<Brick> allBricksL3 = new List<Brick>();
        List<Brick> allBricksL4 = new List<Brick>();
        List<Brick> allBricksL5 = new List<Brick>();
        List<Brick> allBricksL6 = new List<Brick>();
        List<Brick> allBricksL7 = new List<Brick>();
        List<Brick> allBricksL8 = new List<Brick>();
        DateTime startTime;
        public MainWindow()
        {
            InitializeComponent();
            LoadBricksBr();

            TxtBlInfo.Text = "";
            TxtBlWellDone.Visibility = Visibility.Hidden;

            timerRemove.Tick += new EventHandler(timerRemove_Tick);
            timerRemove.Interval = TimeSpan.FromMilliseconds(20);

            BtnReload.IsEnabled = false;
        }

        private void BtnReload_Click(object sender, RoutedEventArgs e)
        {
            gameIsOn = false;
            numberOfBricksLeft = 152;
            CanBricks.Children.Clear();
            CanBricks.Children.Add(TxtBlWellDone);
            TxtBlWellDone.Visibility = Visibility.Hidden;
            LoadBricks();
            BtnReload.IsEnabled = false;
        }

        void LoadBricksBr()
        {
            ClearBricks();

            for (int i = 0; i < 50; i++)
            {
                //myBrushB.ImageSource = new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), @"Images\Img_000.jpg"));
                string fileNm = "Images\\Img_" + i.ToString("000") + ".jpg";
                ImageBrush br = new ImageBrush();
                br.ImageSource = new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), fileNm));
                allBricksBr.Add(br);
            }
        }

        private void CanBricks_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBricks();
        }
        void ClearBricks()
        {
            allBricksL1.Clear();
            allBricksL2.Clear();
            allBricksL3.Clear();
            allBricksL4.Clear();
            allBricksL5.Clear();
            allBricksL6.Clear();
            allBricksL7.Clear();
            allBricksL8.Clear();

            ai.Clear();
            for (int i = 0; i < 26; i++)
            {
                //int index = rnd.Next(34);
                ai.Add(i);
                ai.Add(i);
                ai.Add(i);
                ai.Add(i);
            }
            for (int i = 26; i < 50; i++)
            {
                //int index = rnd.Next(35, allBricksBr.Count - 1);
                ai.Add(i);
                ai.Add(i);
            }

            //Shuffle
            int n = ai.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                int value = ai[k];
                ai[k] = ai[n];
                ai[n] = value;
            }

        }
        void LoadBricks()
        {
            ClearBricks();

            double shad = 13;
            double leftStart = 20;
            double left = 20, top = 20, width = 70, height = 82;//width = 53, height = 65; width = 63, height = 75;
            int brC = 0;
            int lineIndex = 0;
            bool isSel = false;

            //Line 1 12 + en i var hörna
            for (int i = 0; i < 12; i++)
            {
                if (i == 0 || i == 11)
                {
                    isSel = true;
                }
                else
                {
                    isSel = false;
                }
                Brick brick = new Brick(left, top, width, height, CanBricks, allBricksBr[ai[brC]]);
                if (brC == allBricksBr.Count)
                {
                    brC = 0;
                }
                brick.responseEvent += Br_responseEvent;
                brick.Layer = 1;
                brick.Line = 1;
                brick.Column = i + 1;
                brick.MyIndex = lineIndex;
                brick.BrushIndex = ai[brC];
                brick.IsSelectable = isSel;
                allBricksL1.Add(brick);
                if (i == 1)
                {
                    allBricksL1[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL1[i].LeftNabour = lineIndex - 1;
                }
                else if (i > 1)
                {
                    allBricksL1[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL1[lineIndex].LeftNabour = lineIndex - 1;
                }
                lineIndex++;
                left += width;
                brC++;
            }
            //layer 2
            left = leftStart + width * 0;
            for (int i = 0; i < 2; i++)
            {
                isSel = true;
                Brick brick = new Brick(left - shad, top - shad, width, height, CanBricks, allBricksBr[ai[brC]]);//106*130
                brick.responseEvent += Br_responseEvent;
                brick.Layer = 2;
                brick.Line = 1;
                brick.Column = (i*11) + 1;
                brick.MyIndex = lineIndex;
                brick.BrushIndex = ai[brC];
                brick.IsSelectable = isSel;
                allBricksL1.Add(brick);
                lineIndex++;
                brC++;
                left += width * 11;
                //nabour -1 def
            }

            //rad 2 8 i lager 1(2)
            lineIndex = 0;
            left = leftStart + width * 2;
            top += height;
            for (int i = 0; i < 8; i++)
            {
                if (i == 0 || i == 7)
                {
                    isSel = true;
                }
                else
                {
                    isSel = false;
                }
                Brick brick = new Brick(left, top, width, height, CanBricks, allBricksBr[ai[brC]]);//106*130
                brick.responseEvent += Br_responseEvent;
                brick.Layer = 1;
                brick.Line = 2;
                brick.Column = i + 3;
                brick.MyIndex = lineIndex;
                brick.BrushIndex = ai[brC];
                brick.IsSelectable = isSel;
                allBricksL2.Add(brick);
                if (i == 1)
                {
                    allBricksL2[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL2[i].LeftNabour = lineIndex - 1;
                }
                else if (i > 1)
                {
                    allBricksL2[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL2[lineIndex].LeftNabour = lineIndex - 1;
                }
                lineIndex++;
                left += width;
                brC++;
            }
            // rad 2 6 st i lager 2(2)
            left = leftStart + width * 3;
            for (int i = 0; i < 6; i++)
            {
                if (i == 0 || i == 5)
                {
                    isSel = true;
                }
                else
                {
                    isSel = false;
                }
                Brick brick = new Brick(left - shad, top - shad, width, height, CanBricks, allBricksBr[ai[brC]]);//106*130
                brick.responseEvent += Br_responseEvent;
                brick.Layer = 2;
                brick.Line = 2;
                brick.Column = i + 4;
                brick.MyIndex = lineIndex;
                brick.BrushIndex = ai[brC];
                brick.IsSelectable = isSel;
                allBricksL2.Add(brick);
                if (i == 1)
                {
                    allBricksL2[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL2[i].LeftNabour = lineIndex - 1;
                }
                else if (i > 1)
                {
                    allBricksL2[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL2[lineIndex].LeftNabour = lineIndex - 1;
                }
                lineIndex++;
                left += width;
                brC++;
            }

            //rad 3 10 i lager 1(3)
            lineIndex = 0;
            left = leftStart + width * 1;
            top += height;
            for (int i = 0; i < 10; i++)
            {
                if (i == 0 || i == 9)
                {
                    isSel = true;
                }
                else
                {
                    isSel = false;
                }
                Brick brick = new Brick(left, top, width, height, CanBricks, allBricksBr[ai[brC]]);//106*130
                brick.responseEvent += Br_responseEvent;
                brick.Layer = 1;
                brick.Line = 3;
                brick.Column = i + 2;
                brick.MyIndex = lineIndex;
                brick.BrushIndex = ai[brC];
                brick.IsSelectable = isSel;
                allBricksL3.Add(brick);
                if (i == 1)
                {
                    allBricksL3[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL3[lineIndex].LeftNabour = lineIndex - 1;
                }
                else if (i > 1)
                {
                    allBricksL3[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL3[lineIndex].LeftNabour = lineIndex - 1;
                }
                lineIndex++;
                left += width;
                brC++;
            }
            // rad 3 6 st i lager 2(3) + en i var hörna
            left = leftStart + width * 3;
            for (int i = 0; i < 6; i++)
            {
                if (i == 0 || i == 5)
                {
                    isSel = true;
                }
                else
                {
                    isSel = false;
                }
                Brick brick = new Brick(left - shad, top - shad, width, height, CanBricks, allBricksBr[ai[brC]]);//106*130
                brick.responseEvent += Br_responseEvent;
                brick.Layer = 2;
                brick.Line = 3;
                brick.Column = i + 4;
                brick.MyIndex = lineIndex;
                brick.BrushIndex = ai[brC];
                brick.IsSelectable = isSel;
                allBricksL3.Add(brick);
                if (i == 1)
                {
                    allBricksL3[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL3[lineIndex].LeftNabour = lineIndex - 1;
                }
                else if (i > 1)
                {
                    allBricksL3[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL3[lineIndex].LeftNabour = lineIndex - 1;
                }
                lineIndex++;
                left += width;
                brC++;
            }
            //En i var hörna
            left = leftStart + width * 1;
            for (int i = 0; i < 2; i++)
            {
                isSel = true;
                Brick brick = new Brick(left - shad * 1, top - shad * 1, width, height, CanBricks, allBricksBr[ai[brC]]);//106*130
                brick.responseEvent += Br_responseEvent;
                brick.Layer = 2;
                brick.Line = 3;
                brick.Column = 2 + (i*9);
                brick.MyIndex = lineIndex;
                brick.BrushIndex = ai[brC];
                brick.IsSelectable = isSel;
                allBricksL3.Add(brick);
                if (i == 1)
                {
                    allBricksL3[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL3[lineIndex].LeftNabour = lineIndex - 1;
                }
                else if (i > 1)
                {
                    allBricksL3[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL3[lineIndex].LeftNabour = lineIndex - 1;
                }
                lineIndex++;
                left += width * 9;
                brC++;
            }
            // rad 3 4 st i lager 3(3)
            left = leftStart + width * 4;
            for (int i = 0; i < 4; i++)
            {
                if (i == 0 || i == 3)
                {
                    isSel = true;
                }
                else
                {
                    isSel = false;
                }
                Brick brick = new Brick(left - shad * 2, top - shad * 2, width, height, CanBricks, allBricksBr[ai[brC]]);//106*130
                brick.responseEvent += Br_responseEvent;
                brick.Layer = 3;
                brick.Line = 3;
                brick.Column = i + 5;
                brick.MyIndex = lineIndex;
                brick.BrushIndex = ai[brC];
                brick.IsSelectable = isSel;
                allBricksL3.Add(brick);
                if (i == 1)
                {
                    allBricksL3[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL3[lineIndex].LeftNabour = lineIndex - 1;
                }
                else if (i > 1)
                {
                    allBricksL3[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL3[lineIndex].LeftNabour = lineIndex - 1;
                }
                lineIndex++;
                brC++;//46
                left += width;
            }

            //rad 4 12 i lager 1(4)
            lineIndex = 0;
            left = leftStart + width * 0;
            top += height;
            for (int i = 0; i < 12; i++)
            {
                if (i == 0 || i == 11)
                {
                    isSel = true;
                }
                else
                {
                    isSel = false;
                }
                Brick brick = new Brick(left, top, width, height, CanBricks, allBricksBr[ai[brC]]);//106*130
                brick.responseEvent += Br_responseEvent;
                brick.Layer = 1;
                brick.Line = 4;
                brick.Column = i + 1;
                brick.MyIndex = lineIndex;
                brick.BrushIndex = ai[brC];
                brick.IsSelectable = isSel;
                allBricksL4.Add(brick);
                if (i == 1)
                {
                    allBricksL4[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL4[lineIndex].LeftNabour = lineIndex - 1;
                }
                else if (i > 1)
                {
                    allBricksL4[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL4[lineIndex].LeftNabour = lineIndex - 1;
                }
                lineIndex++;
                brC++;
                left += width;
            }
            // rad 4 6 st i lager 2(4) + en i var hörna
            left = leftStart + width * 3;
            for (int i = 0; i < 6; i++)
            {
                if (i == 0 || i == 5)
                {
                    isSel = true;
                }
                else
                {
                    isSel = false;
                }
                Brick brick = new Brick(left - shad, top - shad, width, height, CanBricks, allBricksBr[ai[brC]]);//106*130
                brick.responseEvent += Br_responseEvent;
                brick.Layer = 2;
                brick.Line = 4;
                brick.Column = i + 4;
                brick.MyIndex = lineIndex;
                brick.BrushIndex = ai[brC];
                brick.IsSelectable = isSel;
                allBricksL4.Add(brick);
                if (i == 1)
                {
                    allBricksL4[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL4[lineIndex].LeftNabour = lineIndex - 1;
                }
                else if (i > 1)
                {
                    allBricksL4[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL4[lineIndex].LeftNabour = lineIndex - 1;
                }
                lineIndex++;
                left += width;
                brC++;
            }
            //En i var hörna
            left = leftStart + width * 0;
            for (int i = 0; i < 2; i++)
            {
                isSel = true;
                Brick brick = new Brick(left - shad * 1, top - shad * 1, width, height, CanBricks, allBricksBr[ai[brC]]);//106*130
                brick.responseEvent += Br_responseEvent;
                brick.Layer = 2;
                brick.Line = 4;
                brick.Column = 1 + (i * 11);
                brick.MyIndex = lineIndex;
                brick.BrushIndex = ai[brC];
                brick.IsSelectable = isSel;
                allBricksL4.Add(brick);
                left += width * 11;
                brC++;
                lineIndex++;
            }
            // rad 4 4 st i lager 3(4)
            left = leftStart + width * 4;
            for (int i = 0; i < 4; i++)
            {
                if (i == 0 || i == 3)
                {
                    isSel = true;
                }
                else
                {
                    isSel = false;
                }
                Brick brick = new Brick(left - shad * 2, top - shad * 2, width, height, CanBricks, allBricksBr[ai[brC]]);//106*130
                brick.responseEvent += Br_responseEvent;
                brick.Layer = 3;
                brick.Line = 4;
                brick.Column = i + 5;
                brick.MyIndex = lineIndex;
                brick.BrushIndex = ai[brC];
                brick.IsSelectable = isSel;
                allBricksL4.Add(brick);
                if (i == 1)
                {
                    allBricksL4[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL4[lineIndex].LeftNabour = lineIndex - 1;
                }
                else if (i > 1)
                {
                    allBricksL4[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL4[lineIndex].LeftNabour = lineIndex - 1;
                }
                brC++;
                left += width;
                lineIndex++;
            }
            // rad 4 2 st i lager 4(4)
            left = leftStart + width * 5;
            for (int i = 0; i < 2; i++)
            {
                if (i == 0 || i == 1)
                {
                    isSel = true;
                }
                else
                {
                    isSel = false;
                }
                Brick brick = new Brick(left - shad * 3, top - shad * 3, width, height, CanBricks, allBricksBr[ai[brC]]);//106*130
                brick.responseEvent += Br_responseEvent;
                brick.Layer = 4;
                brick.Line = 4;
                brick.Column = i + 6;
                brick.MyIndex = lineIndex;
                brick.BrushIndex = ai[brC];
                brick.IsSelectable = isSel;
                allBricksL4.Add(brick);
                if (i == 1)
                {
                    allBricksL4[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL4[lineIndex].LeftNabour = lineIndex - 1;
                }
                else if (i > 1)
                {
                    allBricksL4[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL4[lineIndex].LeftNabour = lineIndex - 1;
                }
                lineIndex++;
                brC++;//70
                left += width;
            }

            //rad 5 12 i lager 1(4)
            lineIndex = 0;
            left = leftStart + width * 0;
            top += height;
            for (int i = 0; i < 12; i++)
            {
                if (i == 0 || i == 11)
                {
                    isSel = true;
                }
                else
                {
                    isSel = false;
                }
                Brick brick = new Brick(left, top, width, height, CanBricks, allBricksBr[ai[brC]]);//106*130
                brick.responseEvent += Br_responseEvent;
                brick.Layer = 1;
                brick.Line = 5;
                brick.Column = i + 1;
                brick.MyIndex = lineIndex;
                brick.BrushIndex = ai[brC];
                brick.IsSelectable = isSel;
                allBricksL5.Add(brick);
                if (i == 1)
                {
                    allBricksL5[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL5[lineIndex].LeftNabour = lineIndex - 1;
                }
                else if (i > 1)
                {
                    allBricksL5[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL5[lineIndex].LeftNabour = lineIndex - 1;
                }
                lineIndex++;
                brC++;
                left += width;
            }
            // rad 5 6 st i lager 2(4) + en i var hörna
            left = leftStart + width * 3;
            for (int i = 0; i < 6; i++)
            {
                if (i == 0 || i == 5)
                {
                    isSel = true;
                }
                else
                {
                    isSel = false;
                }
                Brick brick = new Brick(left - shad, top - shad, width, height, CanBricks, allBricksBr[ai[brC]]);//106*130
                brick.responseEvent += Br_responseEvent;
                brick.Layer = 2;
                brick.Line = 5;
                brick.Column = i + 4;
                brick.MyIndex = lineIndex;
                brick.BrushIndex = ai[brC];
                brick.IsSelectable = isSel;
                allBricksL5.Add(brick);
                if (i == 1)
                {
                    allBricksL5[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL5[lineIndex].LeftNabour = lineIndex - 1;
                }
                else if (i > 1)
                {
                    allBricksL5[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL5[lineIndex].LeftNabour = lineIndex - 1;
                }
                lineIndex++;
                left += width;
                brC++;
            }
            //En i var hörna
            left = leftStart + width * 0;
            for (int i = 0; i < 2; i++)
            {
                isSel = true;
                Brick brick = new Brick(left - shad * 1, top - shad * 1, width, height, CanBricks, allBricksBr[ai[brC]]);//106*130
                brick.responseEvent += Br_responseEvent;
                brick.Layer = 2;
                brick.Line = 5;
                brick.Column = 1 + (i * 11);
                brick.MyIndex = lineIndex;
                brick.BrushIndex = ai[brC];
                brick.IsSelectable = isSel;
                allBricksL5.Add(brick);
                left += width * 11;
                brC++;
                lineIndex++;
            }
            // rad 5 4 st i lager 3(4)
            left = leftStart + width * 4;
            for (int i = 0; i < 4; i++)
            {
                if (i == 0 || i == 3)
                {
                    isSel = true;
                }
                else
                {
                    isSel = false;
                }
                Brick brick = new Brick(left - shad * 2, top - shad * 2, width, height, CanBricks, allBricksBr[ai[brC]]);//106*130
                brick.responseEvent += Br_responseEvent;
                brick.Layer = 3;
                brick.Line = 5;
                brick.Column = i + 5;
                brick.MyIndex = lineIndex;
                brick.BrushIndex = ai[brC];
                brick.IsSelectable = isSel;
                allBricksL5.Add(brick);
                if (i == 1)
                {
                    allBricksL5[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL5[lineIndex].LeftNabour = lineIndex - 1;
                }
                else if (i > 1)
                {
                    allBricksL5[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL5[lineIndex].LeftNabour = lineIndex - 1;
                }
                lineIndex++;
                brC++;
                left += width;
            }
            // rad 5 2 st i lager 4(4)
            left = leftStart + width * 5;
            for (int i = 0; i < 2; i++)
            {
                if (i == 0 || i == 1)
                {
                    isSel = true;
                }
                else
                {
                    isSel = false;
                }
                Brick brick = new Brick(left - shad * 3, top - shad * 3, width, height, CanBricks, allBricksBr[ai[brC]]);//106*130
                brick.responseEvent += Br_responseEvent;
                brick.Layer = 4;
                brick.Line = 5;
                brick.Column = i + 6;
                brick.MyIndex = lineIndex;
                brick.BrushIndex = ai[brC];
                brick.IsSelectable = isSel;
                allBricksL5.Add(brick);
                if (i == 1)
                {
                    allBricksL5[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL5[lineIndex].LeftNabour = lineIndex - 1;
                }
                else if (i > 1)
                {
                    allBricksL5[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL5[lineIndex].LeftNabour = lineIndex - 2;
                }
                brC++;
                left += width;
                lineIndex++;
            }

            //rad 6 10 i lager 1(3)
            lineIndex = 0;
            left = leftStart + width * 1;
            top += height;
            for (int i = 0; i < 10; i++)
            {
                if (i == 0 || i == 9)
                {
                    isSel = true;
                }
                else
                {
                    isSel = false;
                }
                Brick brick = new Brick(left, top, width, height, CanBricks, allBricksBr[ai[brC]]);//106*130
                brick.responseEvent += Br_responseEvent;
                brick.Layer = 1;
                brick.Line = 6;
                brick.Column = i + 2;
                brick.MyIndex = lineIndex;
                brick.BrushIndex = ai[brC];
                brick.IsSelectable = isSel;
                allBricksL6.Add(brick);
                if (i == 1)
                {
                    allBricksL6[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL6[lineIndex].LeftNabour = lineIndex - 1;
                }
                else if (i > 1)
                {
                    allBricksL6[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL6[lineIndex].LeftNabour = lineIndex - 1;
                }
                brC++;
                left += width;
                lineIndex++;
            }
            // rad 6 6 st i lager 2(3)
            left = leftStart + width * 3;
            for (int i = 0; i < 6; i++)
            {
                if (i == 0 || i == 5)
                {
                    isSel = true;
                }
                else
                {
                    isSel = false;
                }
                Brick brick = new Brick(left - shad, top - shad, width, height, CanBricks, allBricksBr[ai[brC]]);//106*130
                brick.responseEvent += Br_responseEvent;
                brick.Layer = 2;
                brick.Line = 6;
                brick.Column = i + 4;
                brick.MyIndex = lineIndex;
                brick.BrushIndex = ai[brC];
                brick.IsSelectable = isSel;
                allBricksL6.Add(brick);
                if (i == 1)
                {
                    allBricksL6[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL6[lineIndex].LeftNabour = lineIndex - 1;
                }
                else if (i > 1)
                {
                    allBricksL6[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL6[lineIndex].LeftNabour = lineIndex - 1;
                }
                left += width;
                brC++;
                lineIndex++;
            }
            //En i var hörna
            left = leftStart + width * 1;
            for (int i = 0; i < 2; i++)
            {
                isSel = true;
                Brick brick = new Brick(left - shad * 1, top - shad * 1, width, height, CanBricks, allBricksBr[ai[brC]]);//106*130
                brick.responseEvent += Br_responseEvent;
                brick.Layer = 2;
                brick.Line = 6;
                brick.Column = 2 + (i * 9);
                brick.MyIndex = lineIndex;
                brick.BrushIndex = ai[brC];
                brick.IsSelectable = isSel;
                allBricksL6.Add(brick);
                left += width * 9;
                brC++;
                lineIndex++;
            }
            // rad 6 4 st i lager 3(3)
            left = leftStart + width * 4;
            for (int i = 0; i < 4; i++)
            {
                if (i == 0 || i == 3)
                {
                    isSel = true;
                }
                else
                {
                    isSel = false;
                }
                Brick brick = new Brick(left - shad * 2, top - shad * 2, width, height, CanBricks, allBricksBr[ai[brC]]);//106*130
                brick.responseEvent += Br_responseEvent;
                brick.Layer = 3;
                brick.Line = 6;
                brick.Column = i + 5;
                brick.MyIndex = lineIndex;
                brick.BrushIndex = ai[brC];
                brick.IsSelectable = isSel;
                allBricksL6.Add(brick);
                if (i == 1)
                {
                    allBricksL6[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL6[lineIndex].LeftNabour = lineIndex - 1;
                }
                else if (i > 1)
                {
                    allBricksL6[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL6[lineIndex].LeftNabour = lineIndex - 1;
                }
                brC++;
                left += width;
                lineIndex++;
            }

            //rad 7 8 i lager 1(2)
            left = leftStart + width * 2;
            top += height;
            lineIndex = 0;
            for (int i = 0; i < 8; i++)
            {
                if (i == 0 || i == 7)
                {
                    isSel = true;
                }
                else
                {
                    isSel = false;
                }
                Brick brick = new Brick(left, top, width, height, CanBricks, allBricksBr[ai[brC]]);//106*130
                brick.responseEvent += Br_responseEvent;
                brick.Layer = 1;
                brick.Line = 7;
                brick.Column = i + 3;
                brick.MyIndex = lineIndex;
                brick.BrushIndex = ai[brC];
                brick.IsSelectable = isSel;
                allBricksL7.Add(brick);
                if (i == 1)
                {
                    allBricksL7[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL7[lineIndex].LeftNabour = lineIndex - 1;
                }
                else if (i > 1)
                {
                    allBricksL7[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL7[lineIndex].LeftNabour = lineIndex - 1;
                }
                brC++;
                left += width;
                lineIndex++;
            }
            // rad 7 6 st i lager 2(2)
            left = leftStart + width * 3;
            for (int i = 0; i < 6; i++)
            {
                if (i == 0 || i == 5)
                {
                    isSel = true;
                }
                else
                {
                    isSel = false;
                }
                Brick brick = new Brick(left - shad, top - shad, width, height, CanBricks, allBricksBr[ai[brC]]);//106*130
                brick.responseEvent += Br_responseEvent;
                brick.Layer = 2;
                brick.Line = 7;
                brick.Column = i + 4;
                brick.MyIndex = lineIndex;
                brick.BrushIndex = ai[brC];
                brick.IsSelectable = isSel;
                allBricksL7.Add(brick);
                if (i == 1)
                {
                    allBricksL7[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL7[lineIndex].LeftNabour = lineIndex - 1;
                }
                else if (i > 1)
                {
                    allBricksL7[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL7[lineIndex].LeftNabour = lineIndex - 1;
                }
                left += width;
                brC++;
                lineIndex++;
            }

            //Rad 8 12 och en i var hörna
            left = leftStart + width * 0;
            top += height;
            lineIndex = 0;
            for (int i = 0; i < 12; i++)
            {
                if (i == 0 || i == 11)
                {
                    isSel = true;
                }
                else
                {
                    isSel = false;
                }
                Brick brick = new Brick(left, top, width, height, CanBricks, allBricksBr[ai[brC]]);//106*130
                brick.responseEvent += Br_responseEvent;
                brick.Layer = 1;
                brick.Line = 8;
                brick.Column = i + 1;
                brick.MyIndex = lineIndex;
                brick.BrushIndex = ai[brC];
                brick.IsSelectable = isSel;
                allBricksL8.Add(brick);
                if (i == 1)
                {
                    allBricksL8[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL8[lineIndex].LeftNabour = lineIndex - 1;
                }
                else if (i > 1)
                {
                    allBricksL8[lineIndex - 1].RightNabour = lineIndex;
                    allBricksL8[lineIndex].LeftNabour = lineIndex - 1;
                }
                brC++;
                left += width;
                lineIndex++;
            }
            //En i var hörna
            left = leftStart + width * 0;
            for (int i = 0; i < 2; i++)
            {
                isSel = true;
                Brick brick = new Brick(left - shad, top - shad, width, height, CanBricks, allBricksBr[ai[brC]]);//106*130
                brick.responseEvent += Br_responseEvent;
                brick.Line = 8;
                brick.Column = (i*11) + 1;
                brick.Layer = 2;
                brick.MyIndex = lineIndex;
                brick.BrushIndex = ai[brC];
                brick.IsSelectable = isSel;
                allBricksL8.Add(brick);
                brC++;
                left += width * 11;
                lineIndex++;
            }

            int parLeft = SelectableBricksLeft();
            TxtBlInfo.Text = parLeft + " Selectable pairs";

        }

        bool gameIsOn = false;
        Brick prevBr;
        private void Br_responseEvent(object sender, string msg)
        {
            Brick br = sender as Brick;
            if (msg.IndexOf("Left:") != -1)
            {
                TxtBlInfo.Text = msg;
                return;
            }
            if (msg.IndexOf("NULL") != -1)
            {
                TxtBlInfo.Text = "";
                return;
            }
            string[] del = msg.Split(':');
            int i = int.Parse(del[0]);

            if (msg.IndexOf(":Diselected") != -1)
            {
                prevBr = null;
            }
            else if (msg.IndexOf(":Selected") != -1)
            {
                if(!CheckSelectionOK(br))
                {
                    br.Deselect();
                    return;
                }
                if (!gameIsOn)
                {
                    gameIsOn = true;
                    startTime = DateTime.Now;
                }

                if (prevBr == null)
                {
                    prevBr = br;
                }
                else if (prevBr.BrushIndex == br.BrushIndex || br.BrushIndex > 30 && prevBr.BrushIndex > 30)
                {
                    //it is par
                    RemoveBrick(prevBr);
                    //CanBricks.Children.Remove(prevBr.MyRect);
                    RemoveBrick(br);
                    //CanBricks.Children.Remove(br.MyRect);
                    prevBr = null;
                }
                else
                {
                    br.Deselect();
                }
            }

        }

        bool CheckSelectionOK(Brick br)
        {
            int l = br.Line;
            int c = br.Column;
            int la = br.Layer;
            //0 11 12 13
            if(l == 1 && c == 1 && la == 1)
            {
                if(!allBricksL1[12].IsRemoved)
                {
                    return false;
                }
            }
            else if (l == 1 && c == 12 && la == 1)
            {
                if (!allBricksL1[13].IsRemoved)
                {
                    return false;
                }
            }
            //0 9 16 17
            else if (l == 3 && c == 2 && la == 1)
            {
                if (!allBricksL3[16].IsRemoved)
                {
                    return false;
                }
            }
            else if (l == 3 && c == 11 && la == 1)
            {
                if (!allBricksL3[17].IsRemoved)
                {
                    return false;
                }
            }
            //0 11 18 19
            else if (l == 4 && c == 1 && la == 1)
            {
                if (!allBricksL4[18].IsRemoved)
                {
                    return false;
                }
            }
            else if (l == 4 && c == 12 && la == 1)
            {
                if (!allBricksL4[19].IsRemoved)
                {
                    return false;
                }
            }
            //0 11 18 19
            if (l == 5 && c == 1 && la == 1)
            {
                if (!allBricksL5[18].IsRemoved)
                {
                    return false;
                }
            }
            else if (l == 5 && c == 12 && la == 1)
            {
                if (!allBricksL5[19].IsRemoved)
                {
                    return false;
                }
            }
            //0 9 16 17
            else if (l == 6 && c == 2 && la == 1)
            {
                if (!allBricksL6[16].IsRemoved)
                {
                    return false;
                }
            }
            else if (l == 6 && c == 11 && la == 1)
            {
                if (!allBricksL6[17].IsRemoved)
                {
                    return false;
                }
            }
            //0 11 12 13
            if (l == 8 && c == 1 && la == 1)
            {
                if (!allBricksL8[12].IsRemoved)
                {
                    return false;
                }
            }
            else if (l == 8 && c == 12 && la == 1)
            {
                if (!allBricksL8[13].IsRemoved)
                {
                    return false;
                }
            }

            return true;
        }

        bool RemoveBrick(Brick br)
        {
            int line = br.Line;
            int naboIndexLeft = -1;
            int naboIndexRight = -1;
            if (br.LeftNabour != -1)
            {
                naboIndexLeft = br.LeftNabour;
            }
            else if (br.RightNabour != -1)
            {
                naboIndexRight = br.RightNabour;
            }
            else//it is a single
            {
                return FindAndRemoveBrick(line, br);
            }
            if (line != -1)
            {
                if (line == 1)
                {
                    if (naboIndexLeft != -1)
                    {
                        allBricksL1[naboIndexLeft].RightNabour = -1;
                        allBricksL1[naboIndexLeft].IsSelectable = true;
                    }
                    else if (naboIndexRight != -1)
                    {
                        allBricksL1[naboIndexRight].LeftNabour = -1;
                        allBricksL1[naboIndexRight].IsSelectable = true;
                    }
                }
                if (line == 2)
                {
                    if (naboIndexLeft != -1)
                    {
                        allBricksL2[naboIndexLeft].RightNabour = -1;
                        allBricksL2[naboIndexLeft].IsSelectable = true;
                    }
                    else if (naboIndexRight != -1)
                    {
                        allBricksL2[naboIndexRight].LeftNabour = -1;
                        allBricksL2[naboIndexRight].IsSelectable = true;
                    }
                }
                if (line == 3)
                {
                    if (naboIndexLeft != -1)
                    {
                        allBricksL3[naboIndexLeft].RightNabour = -1;
                        allBricksL3[naboIndexLeft].IsSelectable = true;
                    }
                    else if (naboIndexRight != -1)
                    {
                        allBricksL3[naboIndexRight].LeftNabour = -1;
                        allBricksL3[naboIndexRight].IsSelectable = true;
                    }
                }
                if (line == 4)
                {
                    if (naboIndexLeft != -1)
                    {
                        allBricksL4[naboIndexLeft].RightNabour = -1;
                        allBricksL4[naboIndexLeft].IsSelectable = true;
                    }
                    else if (naboIndexRight != -1)
                    {
                        allBricksL4[naboIndexRight].LeftNabour = -1;
                        allBricksL4[naboIndexRight].IsSelectable = true;
                    }
                }
                if (line == 5)
                {
                    if (naboIndexLeft != -1)
                    {
                        allBricksL5[naboIndexLeft].RightNabour = -1;
                        allBricksL5[naboIndexLeft].IsSelectable = true;
                    }
                    else if (naboIndexRight != -1)
                    {
                        allBricksL5[naboIndexRight].LeftNabour = -1;
                        allBricksL5[naboIndexRight].IsSelectable = true;
                    }
                }
                if (line == 6)
                {
                    if (naboIndexLeft != -1)
                    {
                        allBricksL6[naboIndexLeft].RightNabour = -1;
                        allBricksL6[naboIndexLeft].IsSelectable = true;
                    }
                    else if (naboIndexRight != -1)
                    {
                        allBricksL6[naboIndexRight].LeftNabour = -1;
                        allBricksL6[naboIndexRight].IsSelectable = true;
                    }
                }
                if (line == 7)
                {
                    if (naboIndexLeft != -1)
                    {
                        allBricksL7[naboIndexLeft].RightNabour = -1;
                        allBricksL7[naboIndexLeft].IsSelectable = true;
                    }
                    else if (naboIndexRight != -1)
                    {
                        allBricksL7[naboIndexRight].LeftNabour = -1;
                        allBricksL7[naboIndexRight].IsSelectable = true;
                    }
                }
                if (line == 8)
                {
                    if (naboIndexLeft != -1)
                    {
                        allBricksL8[naboIndexLeft].RightNabour = -1;
                        allBricksL8[naboIndexLeft].IsSelectable = true;
                    }
                    else if (naboIndexRight != -1)
                    {
                        allBricksL8[naboIndexRight].LeftNabour = -1;
                        allBricksL8[naboIndexRight].IsSelectable = true;
                    }
                }
                return FindAndRemoveBrick(line, br);
            }
            else
            {
                MessageBox.Show("Line is -1");
                return false;
            }

            return true;
        }

        bool FindAndRemoveBrick(int line, Brick br)
        {
            if (line == 1)
            {
                for (int j = 0; j < allBricksL1.Count; j++)
                {
                    //finn brick
                    if (allBricksL1[j] == br)
                    {
                        if (opBr1 == null)
                        {
                            opBr1 = allBricksL1[j];
                            timerRemove.Start();
                        }
                        else if (opBr2 == null)
                        {
                            opBr2 = allBricksL1[j];
                            timerRemove.Start();
                        }
                        //CanBricks.Children.Remove(allBricksL1[j].MyRect);
                        //allBricksL1.RemoveAt(j);
                        break;
                    }
                }
            }
            else if (line == 2)
            {
                for (int j = 0; j < allBricksL2.Count; j++)
                {
                    //finn brick
                    if (allBricksL2[j] == br)
                    {
                        if (opBr1 == null)
                        {
                            opBr1 = allBricksL2[j];
                            timerRemove.Start();
                        }
                        else if (opBr2 == null)
                        {
                            opBr2 = allBricksL2[j];
                            timerRemove.Start();
                        }
                        //CanBricks.Children.Remove(allBricksL2[j].MyRect);
                        //allBricksL2.RemoveAt(j);
                        break;
                    }
                }
            }
            else if (line == 3)
            {
                for (int j = 0; j < allBricksL3.Count; j++)
                {
                    //finn brick
                    if (allBricksL3[j] == br)
                    {
                        if (opBr1 == null)
                        {
                            opBr1 = allBricksL3[j];
                            timerRemove.Start();
                        }
                        else if (opBr2 == null)
                        {
                            opBr2 = allBricksL3[j];
                            timerRemove.Start();
                        }
                        //CanBricks.Children.Remove(allBricksL3[j].MyRect);
                        //allBricksL3.RemoveAt(j);
                        break;
                    }
                }
            }
            else if (line == 4)
            {
                for (int j = 0; j < allBricksL4.Count; j++)
                {
                    //finn brick
                    if (allBricksL4[j] == br)
                    {
                        if (opBr1 == null)
                        {
                            opBr1 = allBricksL4[j];
                            timerRemove.Start();
                        }
                        else if (opBr2 == null)
                        {
                            opBr2 = allBricksL4[j];
                            timerRemove.Start();
                        }
                        //CanBricks.Children.Remove(allBricksL4[j].MyRect);
                        //allBricksL4.RemoveAt(j);
                        break;
                    }
                }
            }
            else if (line == 5)
            {
                for (int j = 0; j < allBricksL5.Count; j++)
                {
                    //finn brick
                    if (allBricksL5[j] == br)
                    {
                        if (opBr1 == null)
                        {
                            opBr1 = allBricksL5[j];
                            timerRemove.Start();
                        }
                        else if (opBr2 == null)
                        {
                            opBr2 = allBricksL5[j];
                            timerRemove.Start();
                        }
                        //CanBricks.Children.Remove(allBricksL5[j].MyRect);
                        //allBricksL5.RemoveAt(j);
                        break;
                    }
                }
            }
            else if (line == 6)
            {
                for (int j = 0; j < allBricksL6.Count; j++)
                {
                    //finn brick
                    if (allBricksL6[j] == br)
                    {
                        if (opBr1 == null)
                        {
                            opBr1 = allBricksL6[j];
                            timerRemove.Start();
                        }
                        else if (opBr2 == null)
                        {
                            opBr2 = allBricksL6[j];
                            timerRemove.Start();
                        }
                        //CanBricks.Children.Remove(allBricksL6[j].MyRect);
                        //allBricksL6.RemoveAt(j);
                        break;
                    }
                }
            }
            else if (line == 7)
            {
                for (int j = 0; j < allBricksL7.Count; j++)
                {
                    //finn brick
                    if (allBricksL7[j] == br)
                    {
                        if (opBr1 == null)
                        {
                            opBr1 = allBricksL7[j];
                            timerRemove.Start();
                        }
                        else if (opBr2 == null)
                        {
                            opBr2 = allBricksL7[j];
                            timerRemove.Start();
                        }
                        //CanBricks.Children.Remove(allBricksL7[j].MyRect);
                        //allBricksL7.RemoveAt(j);
                        break;
                    }
                }
            }
            else if (line == 8)
            {
                for (int j = 0; j < allBricksL8.Count; j++)
                {
                    //finn brick
                    if (allBricksL8[j] == br)
                    {
                        if (opBr1 == null)
                        {
                            opBr1 = allBricksL8[j];
                            timerRemove.Start();
                        }
                        else if (opBr2 == null)
                        {
                            opBr2 = allBricksL8[j];
                            timerRemove.Start();
                        }
                        //CanBricks.Children.Remove(allBricksL8[j].MyRect);
                        //allBricksL8.RemoveAt(j);
                        break;
                    }
                }
            }
            return true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CanBricks.Children.Clear();
        }

        int opStep = 20;
        Brick opBr1 = null, opBr2 = null;

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Space)
            {
                int parLeft = SelectableBricksLeft();
                TxtBlInfo.Text = parLeft + " Selectable pairs";
            }
        }

        List<Brick> allParBr = new List<Brick>();
        int SelectableBricksLeft()
        {
            for(int i=0;i<allParBr.Count;i++)
            {
                allParBr[i].SetSelColor(0);
            }
            allParBr.Clear();
            int bricksLeft = 0;
            List<Brick> allSelBr = new List<Brick>();
            for(int i=0;i< allBricksL1.Count;i++)
            {
                if(!allBricksL1[i].IsRemoved && allBricksL1[i].IsSelectable)
                {
                    allSelBr.Add(allBricksL1[i]);
                    bricksLeft++;
                }
            }
            for (int i = 0; i < allBricksL2.Count; i++)
            {
                if (!allBricksL2[i].IsRemoved && allBricksL2[i].IsSelectable)
                {
                    allSelBr.Add(allBricksL2[i]);
                    bricksLeft++;
                }
            }
            for (int i = 0; i < allBricksL3.Count; i++)
            {
                if (!allBricksL3[i].IsRemoved && allBricksL3[i].IsSelectable)
                {
                    allSelBr.Add(allBricksL3[i]);
                    bricksLeft++;
                }
            }
            for (int i = 0; i < allBricksL4.Count; i++)
            {
                if (!allBricksL4[i].IsRemoved && allBricksL4[i].IsSelectable)
                {
                    allSelBr.Add(allBricksL4[i]);
                    bricksLeft++;
                }
            }
            for (int i = 0; i < allBricksL5.Count; i++)
            {
                if (!allBricksL5[i].IsRemoved && allBricksL5[i].IsSelectable)
                {
                    allSelBr.Add(allBricksL5[i]);
                    bricksLeft++;
                }
            }
            for (int i = 0; i < allBricksL6.Count; i++)
            {
                if (!allBricksL6[i].IsRemoved && allBricksL6[i].IsSelectable)
                {
                    allSelBr.Add(allBricksL6[i]);
                    bricksLeft++;
                }
            }
            for (int i = 0; i < allBricksL7.Count; i++)
            {
                if (!allBricksL7[i].IsRemoved && allBricksL7[i].IsSelectable)
                {
                    allSelBr.Add(allBricksL7[i]);
                    bricksLeft++;
                }
            }
            for (int i = 0; i < allBricksL8.Count; i++)
            {
                if (!allBricksL8[i].IsRemoved && allBricksL8[i].IsSelectable)
                {
                    allSelBr.Add(allBricksL8[i]);
                    bricksLeft++;
                }
            }

            //check for bricks on same pos differente layer
            Start:
            for (int i = 0; i < allSelBr.Count; i++)
            {
                for(int j=0;j<allSelBr.Count;j++)
                {
                    if(allSelBr[i].Line == allSelBr[j].Line && allSelBr[i].Column == allSelBr[j].Column && allSelBr[i].Layer != allSelBr[j].Layer)
                    {
                        if(allSelBr[i].Layer > allSelBr[j].Layer)
                        {
                            allSelBr.RemoveAt(j);
                        }
                        else if (allSelBr[j].Layer > allSelBr[i].Layer)
                        {
                            allSelBr.RemoveAt(i);
                        }
                        goto Start;
                    }
                }
            }
            bricksLeft = allSelBr.Count;
            //Check for par
            int parLeft = 0;
            Start2:
            for (int i = 0; i < allSelBr.Count; i++)
            {
                for (int j = 0; j < allSelBr.Count; j++)
                {
                    if(i != j)
                    {
                        if (allSelBr[i].BrushIndex == allSelBr[j].BrushIndex || (allSelBr[i].BrushIndex > 30 && allSelBr[j].BrushIndex > 30))
                        {
                            if (showHint)
                            {
                                allSelBr[j].SetSelColor(1);
                            }
                            allParBr.Add(allSelBr[j]);
                            if (showHint)
                            {
                                allSelBr[i].SetSelColor(1);
                            }
                            allParBr.Add(allSelBr[i]);
                            Brick br1 = allSelBr[i];
                            Brick br2 = allSelBr[j];
                            parLeft++;
                            allSelBr.Remove(br1);
                            allSelBr.Remove(br2);
                            goto Start2;
                        }
                    }
                }
            }

            
            return parLeft;
        }

        bool showHint = false;
        private void ChBoHint_Checked(object sender, RoutedEventArgs e)
        {
            showHint = true;
            SelectableBricksLeft();
        }

        private void ChBoHint_Unchecked(object sender, RoutedEventArgs e)
        {
            showHint = false;
            SelectableBricksLeft();
        }

        private void timerRemove_Tick(object sender, EventArgs e)
        {
            if (opStep > 0)
            {
                opStep--;
                if (opBr1 != null)
                {
                    opBr1.SetOpacity(0.05 * opStep);
                }
                if (opBr2 != null)
                {
                    opBr2.SetOpacity(0.05 * opStep);
                }
            }
            else
            {
                timerRemove.Stop();
                opStep = 20;
                if (opBr1 != null)
                {
                    opBr1.RemoveFromCanvas();
                    numberOfBricksLeft--;
                    //CanBricks.Children.Remove(opBr1.MyRect);
                    //CanBricks.Children.Remove(opBr1.MyRectBack);
                    opBr1 = null;
                }
                if (opBr2 != null)
                {
                    opBr2.RemoveFromCanvas();
                    numberOfBricksLeft--;
                    //CanBricks.Children.Remove(opBr2.MyRect);
                    //CanBricks.Children.Remove(opBr2.MyRectBack);
                    opBr2 = null;
                }
                int parLeft = SelectableBricksLeft();
                TxtBlInfo.Text = parLeft + " Selectable pairs";
                if (numberOfBricksLeft <= 0)
                {
                    string time = "[" + DateTime.Now.ToString("yyyyMMdd_HH:mm:ss") + "]";
                    TimeSpan ts = DateTime.Now - startTime;
                    TxtBlWellDone.Visibility = Visibility.Visible;
                    TxtBlInfo.Text = "The winner, time used:" + ts.ToString() + " At date " + DateTime.Now.ToString();
                    File.AppendAllText("PlayLog.txt", "The winner, time used:" + ts.ToString() + " At date " + time + Environment.NewLine);
                    BtnReload.IsEnabled = true;
                }
                else if (parLeft==0)
                {
                    string time = "[" + DateTime.Now.ToString("yyyyMMdd_HH:mm:ss") + "]";
                    TimeSpan ts = DateTime.Now - startTime;//bricks left
                    TxtBlInfo.Text = parLeft + " Selectable pairs. Time used:" + ts.ToString() + " At date " + DateTime.Now.ToString();
                    File.AppendAllText("PlayLog.txt", "Bricks left " + numberOfBricksLeft + " and " + parLeft + " Selectable pairs. Time used:" + ts.ToString() + " At date " + time + Environment.NewLine);
                    BtnReload.IsEnabled = true;
                }
            }
        }
    }

    public delegate void GetResponseDelegate(object sender, string msg);
    class Brick
    {
        public Rectangle MyRect { get; set; }
        Ellipse ellySelecetable;
        //Polyline ln1;
        Polyline ln2;
        //public Rectangle MyRectBack { get; set; }
        Canvas MyCan;
        Brush MyBrush;
        double L, T, W, H;
        Cursor crHand;
        Brush brStrokeSel;// = Brushes.Red;
        Line borderLine01, borderLine02, borderLine03;
        public event GetResponseDelegate responseEvent;

        public Brick(double left, double top, double width, double height, Canvas can, Brush brush)
        {
            brStrokeSel = Brushes.Red;

            double shadX = 10, shadY = 10;
            L = left-5;
            T = top-5;
            W = width;
            H = height;
            MyBrush = brush;
            MyCan = can;

            /*
            //top hori and right shade
            ln1 = new Polyline();
            Point p1 = new Point(left + shadX+5, top);
            Point p2 = new Point(left + width, top);
            //Point p3 = new Point(left + shadX + width, top + height);
            PointCollection pc = new PointCollection();
            pc.Add(p1);
            pc.Add(p2);
            //pc.Add(p3);
            ln1.Points = pc;
            ln1.StrokeStartLineCap = PenLineCap.Round;
            ln1.StrokeLineJoin = PenLineJoin.Round;
            ln1.StrokeEndLineCap = PenLineCap.Round;
            ln1.Stroke = Brushes.Black;
            ln1.StrokeThickness = shadX*2;
            ln1.Opacity = 0.5;
            MyCan.Children.Add(ln1);
            */

            //right vert and bottom shade
            ln2 = new Polyline();
            Point p11 = new Point(left + width, top + 12);
            Point p21 = new Point(left + width, top + height);
            Point p31 = new Point(left + 14, top + height);
            PointCollection pc1 = new PointCollection();
            pc1.Add(p11);
            pc1.Add(p21);
            pc1.Add(p31);
            ln2.Points = pc1;
            ln2.StrokeStartLineCap = PenLineCap.Triangle;
            //ln2.StrokeLineJoin = PenLineJoin.Round;
            ln2.StrokeEndLineCap = PenLineCap.Triangle;
            ln2.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffEBE0B1"));//DABCC9 006969
            ln2.StrokeThickness = shadX*2;
            ln2.Opacity = 1.0;
            MyCan.Children.Add(ln2);

            /*
            MyRectBack = new Rectangle();
            MyRectBack.Width = width;
            MyRectBack.Height = height;
            MyRectBack.Fill = Brushes.Green;
            MyRectBack.Opacity = 1.0;
            MyRectBack.RadiusX = 10;
            MyRectBack.RadiusY = 10;
            MyRectBack.StrokeThickness = 1;
            MyRectBack.Stroke = Brushes.Black;

            MyCan.Children.Add(MyRectBack);
            Canvas.SetLeft(MyRectBack, left + shadX);
            Canvas.SetTop(MyRectBack, top + shadY);
            */

            /*
            Border border = new Border();
            border.Background = new SolidColorBrush(Colors.LightGray);
            border.BorderThickness = new Thickness(5);
            border.BorderBrush = new SolidColorBrush(Colors.Green);
            border.CornerRadius = new CornerRadius(15);
            border.Width = width;
            border.Height = height;
            */

            MyRect = new Rectangle();
            MyRect.Width = width-1;
            MyRect.Height = height-1;
            MyRect.Fill = MyBrush;
            //MyRect.RadiusX = 10;
            //MyRect.RadiusY = 10;
            MyRect.StrokeThickness = 2;
            MyRect.Stroke = Brushes.Black;
            MyRect.Opacity = 1.0;
            MyRect.MouseEnter += MyRect_MouseEnter;
            MyRect.MouseLeave += MyRect_MouseLeave;
            crHand = Cursors.Hand;
            MyRect.MouseLeftButtonUp += MyRect_MouseLeftButtonUp;

            MyCan.Children.Add(MyRect);
            Canvas.SetLeft(MyRect, left);
            Canvas.SetTop(MyRect, top);

            ellySelecetable = new Ellipse();
            ellySelecetable.Width = width;
            ellySelecetable.Height = height;
            ellySelecetable.Fill= (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFD2D200"));
            ellySelecetable.Opacity = 0.2;
            ellySelecetable.MouseEnter += MyRect_MouseEnter;
            ellySelecetable.MouseLeave += MyRect_MouseLeave;
            ellySelecetable.MouseLeftButtonUp += MyRect_MouseLeftButtonUp;
            MyCan.Children.Add(ellySelecetable);
            Canvas.SetLeft(ellySelecetable, left);
            Canvas.SetTop(ellySelecetable, top);
            ellySelecetable.Visibility = Visibility.Hidden;

            borderLine01 = new Line();
            borderLine01.X1 = left + width - 1;
            borderLine01.X2 = left + width + 10;
            borderLine01.Y1 = top + height - 1;
            borderLine01.Y2 = top + height + 10;
            borderLine01.Stroke = Brushes.Black;
            MyCan.Children.Add(borderLine01);

            borderLine02 = new Line();
            borderLine02.X1 = left + width;
            borderLine02.X2 = left + width + 10;
            borderLine02.Y1 = top;
            borderLine02.Y2 = top + 10;
            borderLine02.Stroke = Brushes.Black;
            MyCan.Children.Add(borderLine02);

            borderLine03 = new Line();
            borderLine03.X1 = left + 2;
            borderLine03.X2 = left + 12;
            borderLine03.Y1 = top + height - 2;
            borderLine03.Y2 = top + height + 8;
            borderLine03.Stroke = Brushes.Black;
            MyCan.Children.Add(borderLine03);
            
        }
        private void MyRect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsSelectable && !IsSelected)
            {
                MyRect.Stroke = brStrokeSel;
                MyRect.StrokeThickness = 2;
                IsSelected = true;
                responseEvent?.Invoke(this, BrushIndex + ":Selected");
            }
            else if (IsSelectable && IsSelected)
            {
                //MyRect.Stroke = Brushes.Blue;
                //MyRect.Stroke = null;
                MyRect.StrokeThickness = 2;
                MyRect.Stroke = Brushes.Black;
                IsSelected = false;
                responseEvent?.Invoke(this, BrushIndex + ":Diselected");
                //ellySelecetable.Visibility = Visibility.Hidden;
            }
        }
        public void SetSelColor(int how)
        {
            if(how==0)
            {
                ellySelecetable.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFD2D200"));
            }
            else
            {
                ellySelecetable.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFD20000"));
            }
        }
        public void Deselect()
        {
            IsSelected = false;
            MyRect.StrokeThickness = 2;
            MyRect.Stroke = Brushes.Black;
            //ellySelecetable.Visibility = Visibility.Hidden;
        }
        public void SetOpacity(double opacity)
        {
            MyRect.Opacity = opacity;
            ln2.Opacity = opacity;
            ellySelecetable.Opacity = opacity;
            //MyRectBack.Opacity = opacity;
        }
        public void RemoveFromCanvas()
        {
            MyCan.Children.Remove(MyRect);
            MyCan.Children.Remove(ln2);
            MyCan.Children.Remove(ellySelecetable);
            MyCan.Children.Remove(borderLine01);
            MyCan.Children.Remove(borderLine02);
            MyCan.Children.Remove(borderLine03);
            IsRemoved = true;
        }
        private void MyRect_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                string txt = "NULL";
                responseEvent?.Invoke(this, txt);
            }
            if (IsSelectable)
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void MyRect_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                string txt = "Left:" + LeftNabour.ToString() + " Right:" + RightNabour.ToString() +
                    " Index:" + MyIndex.ToString() + " BrIndex: " + BrushIndex.ToString() + " Is selectebal:" + IsSelectable.ToString();
                responseEvent?.Invoke(this, txt);
            }
            if (IsSelectable)
            {
                Mouse.OverrideCursor = crHand;
            }
        }

        bool isSelectebal = false;
        public bool IsSelectable
        {
            get
            {
                return isSelectebal;
            }
            set
            {
                isSelectebal = value;
                if(isSelectebal)
                {
                    ellySelecetable.Visibility = Visibility.Visible;
                }
                else
                {
                    ellySelecetable.Visibility = Visibility.Hidden;

                }
            }
        }
        public bool IsSelected { get; set; } = false;
        public int BrushIndex { get; set; } = -1;
        public int LeftNabour { get; set; } = -1;
        public int RightNabour { get; set; } = -1;
        public int Layer { get; set; } = -1;
        public int Line { get; set; } = -1;
        public int Column { get; set; } = -1;
        public int MyIndex { get; set; } = -1;
        public bool IsRemoved { get; set; } = false;
    }
}
