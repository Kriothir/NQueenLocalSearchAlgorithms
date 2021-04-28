using System;
using System.Collections.Generic;
using System.Drawing;
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
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using Image = System.Windows.Controls.Image;
using Pen = System.Windows.Media.Pen;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public int[] chess = default;
        static Random ranObj = new Random();

        public MainWindow()
        {
            InitializeComponent();
            sizeText.Text = "6";
            DrawChessboard();
            hillRadio.IsChecked = true;


        }
        public  int[] generateRandomQueens(int size)
        {
            // Naključna postavitev kraljic v eno dimenzionalno polje, kjer je index array enak stolpcu, vrednost na tem indexu pa vrstici
            int[] queens = new int[size];
            for (int i = 0; i < queens.Length; i++)
                queens[i] = (int)(ranObj.NextDouble() * queens.Length);

            return queens;
        }
      
        public  int[] firstChoiceHillClimbing(int n, int maxNumOfIterations)
        {
            int[] originalChessboard = generateRandomQueens(n); //Generiramo nakljucno stanje
            int heuristicScore = HeuristicCalculator(originalChessboard); //izračunamo hevristiko
            int stepsTaken = 0;
            int counter = 0; // Štejemo število korakov

            while (counter < maxNumOfIterations && heuristicScore > 0) // Izvaja se dokler ni hevristika enako 0 ali dokler ne preseže maksimalno število 
                                                                       // dovoljenih premikov 
            {
                int stuckHeuristic = heuristicScore; // Za preverjanje lokalne maxime
                for (int stolpec = 0; stolpec < n; stolpec++)
                {
                    for (int vrstica = 0; vrstica < n; vrstica++)
                    {
                        // Premaknemo kraljico na novi polozaj in izracunamo hevristiko novo nastale šahovnice
                        // Če je hevristika novo nastale šahovnice manjša jo obdržimo
                        int[] newChessboard = (int[])originalChessboard.Clone();
                        //Naredimo kopijo originalne sahovnice in z isto sahovnico delamo premike
                        newChessboard[stolpec] = vrstica;
                        int newHeuristic = HeuristicCalculator(newChessboard); 
                        if (heuristicScore > newHeuristic)
                        {
                            originalChessboard[stolpec] = vrstica;  // Če je hevristika manjša, ta premik kraljice obdržimo in jo applyjamo na originalno šahovnico
                            heuristicScore = newHeuristic;
                            break;
                        }
                    }
                }

                // Če se algoritem zatakne, izračunamo novo random stanje
                if (stuckHeuristic == heuristicScore)
                {
                    originalChessboard = generateRandomQueens(n);
                }

                counter++;

            }

            if (heuristicScore == 0)
            {
                stepsTakenText.Text = Convert.ToString(counter);

                return originalChessboard;

            }
            else
            {
                stepsTakenText.Text = Convert.ToString(counter);

                return null;

            }
        }
        public  int HeuristicCalculator(int[] queens)
        {
            int heuristic = 0;

            for (int i = 0; i < queens.Length; i++)
            {
                for (int j = i + 1; j < queens.Length; j++)
                {   int deltaCol = Math.Abs(queens[i] - queens[j]);
                    int deltaRow = j - i;

                    if (queens[i] == queens[j] || deltaCol == deltaRow) //https://stackoverflow.com/questions/3209165/need-help-with-n-queens-program-checking-diagonals
                    {
                        heuristic += 1;
                    }
                }
            }
            heuristicText.Text = Convert.ToString(heuristic);
            return heuristic;
        }

        private void DrawChessboard()
        {

            chessboard.Rows = Convert.ToInt32(sizeText.Text);
            chessboard.Columns = Convert.ToInt32(sizeText.Text);
            chessboard.Children.Clear();

            for (int i = 0; i < Convert.ToInt32(sizeText.Text); i++)
            {
                for (int j = 0; j < Convert.ToInt32(sizeText.Text); j++ )
                {   // Izriše polje
                    if ((i + j) % 2 == 0)
                    {
                        Grid blackCheckers = new Grid();
                        blackCheckers.Children.Add(new Rectangle
                        {
                            Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0))
                        });

                        chessboard.Children.Add(blackCheckers);

                    }
                    else
                    {
                        Grid whiteCheckers = new Grid();
                        whiteCheckers.Children.Add(new Rectangle
                        {
                            Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(128, 128, 128))
                        });


                        chessboard.Children.Add(whiteCheckers);
                    }
                }

            }
        }
        public int[] LocalBeamSearch(int size, int maxSteps, int numberK)
        {
            int[][] arrayOfK = new int[numberK][];
            for (int i = 0; i < numberK; i++)
            {
                arrayOfK[i] = generateRandomQueens(size);
            }
            // Napolnemo array z k stevilo random stanj
            List<int[][]> arrayList = new List<int[][]>();
            int counter = 0;
            int heuristic = 0;
           
            // IZvaja se dokler ne preseze dovoljeno stevilo korakov 
            while(counter <= maxSteps || heuristic == 0)
            {
                // Nov array z velikostjo sahovnice pomnozeno z stevilom stanj
                int[][] shiftBoard = new int[size * numberK][];
                // Array, ki bo hranil naše heuristice iz ShiftBoard arraya, stem nato sortiramo
                int[] shiftedHeuristic = new int[size * numberK];
                //Sprehodimo se skozi stanja, izracunamo hevristiko in primerjamo
                // ali je enako 0 ali ni. Če ni ni nic se izvaja naprej
                
                for (int i = 0; i < numberK; i++)
                {
                     heuristic = HeuristicCalculator(arrayOfK[i]);
                    if (heuristic == 0)
                    {
                        stepsTakenText.Text = Convert.ToString(counter);
                        return arrayOfK[i];
                    }
                    for (int stolpec = 0; stolpec < size; stolpec++)
                    {
                        
                     // Sprehodimo se skozi array z novimi stanji, kraljice premikamo 
                     // hevristike novih stanj hranimo v shiftedHeuiristc
                        shiftBoard[size * i + stolpec] = shiftQueen(arrayOfK[i], stolpec, heuristic);
                     // V primeru da je vrnjeno stanje enako null vrednost, zgeneriramo novo stanje
                    // enako kot pri vzpenjanju v hrib
                        if (shiftBoard[size * i + stolpec] == null)
                        {
                            // Zgeneriramo na tistem položaju kjer je sahovnica bila nicte vrednosti
                            shiftBoard[size * i + stolpec] = generateRandomQueens(size);
                        }
                        // Hranimo hevristike
                        shiftedHeuristic[i] = HeuristicCalculator(shiftBoard[i]);

                    }
                    counter++;
                }
                // Sortiramo array z novimi stanjo po ključu hevristike
                Array.Sort(shiftedHeuristic, shiftBoard);
                // N
                Array.Copy(shiftBoard, 0, arrayOfK, 0, numberK);

            }

            return null;
        }

        private  int[] shiftQueen(int[] board, int stolpec, int heuristic)
        {

            for (int vrstica = 0; vrstica < board.Length; vrstica++)
            {
                int fooInt = board[stolpec];// Podobno kot pri ohlajanju, shranimo stanje  pred premikom v primeru da bo premik slabši

                board[stolpec] = vrstica;//Premaknemo kraljico in izračunamo
                                        // Novo hevristiko
                                        // Nato primerjamo
                int shiftedHeuristic = HeuristicCalculator(board);
                if (heuristic > shiftedHeuristic)
                {
                    board[stolpec] = vrstica;
                    return board;
                }
                board[stolpec] = fooInt; //Vrnemo v prejšno stanje

            }
            // Če nobeno stanje ni boljše, vrnemo nićlo vrednost
            return null;
        }
        private void PlaceQueens(int[] chess)
        {
            // Izriše kraljice na šahovnico
            for (int column = 0; column < Convert.ToInt32(sizeText.Text); column++)
            {
                int row = chess[column];

                Grid cell = (Grid)chessboard.Children[Convert.ToInt32(sizeText.Text) * column + row] ;
                   cell.Children.Add(new Rectangle 
                   { 
                       Fill = new SolidColorBrush(Color.FromRgb(0, 128, 0))
                   });

            }

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            coolingLabel.Visibility = Visibility.Hidden;
            temperatureLabel.Visibility = Visibility.Hidden;
            stepsText.Visibility = Visibility.Visible;
            coolingText.Visibility = Visibility.Hidden;
            temperatureText.Visibility = Visibility.Hidden;
            hillStepsLabel.Visibility = Visibility.Visible;
            maxstepsLabelBeam.Visibility = Visibility.Hidden;
            kStatesLabel.Visibility = Visibility.Hidden;
            maxIterationsText.Visibility = Visibility.Hidden;
            kStatesText.Visibility = Visibility.Hidden;
        }

        private void annealRadio_Checked(object sender, RoutedEventArgs e)
        {
            coolingLabel.Visibility = Visibility.Visible;
            temperatureLabel.Visibility = Visibility.Visible;
            coolingText.Visibility = Visibility.Visible;
            temperatureText.Visibility = Visibility.Visible;
            stepsText.Visibility = Visibility.Hidden;
            hillStepsLabel.Visibility = Visibility.Hidden;
            maxstepsLabelBeam.Visibility = Visibility.Hidden;
            kStatesLabel.Visibility = Visibility.Hidden;
            maxIterationsText.Visibility = Visibility.Hidden;
            kStatesText.Visibility = Visibility.Hidden;
        }
        public  int[] SimulatedAnnealing(int size, double temperature, double coolingFactor)
        {
            int counter = 0;
            int[] originalChessboard = generateRandomQueens(size);
          

            int heuristic = HeuristicCalculator(originalChessboard);

            // 
            while (temperature >= 0 || heuristic != 0) // Program izvaja dokler ni temperatura enaka ali manjša 0
            {
                originalChessboard = makeMoveAnneal(originalChessboard, heuristic, temperature); // Premaknemo kraljico, zračunamo novo heuristiko in zmanjšamo temperaturo
                heuristic = HeuristicCalculator(originalChessboard);
                temperature -= coolingFactor; // Zmanjšamo temperaturo z ohladitvenim faktorjem
                counter++;
            }
            stepsTakenText.Text = Convert.ToString(counter);
            if(heuristic == 0)
            {
                return originalChessboard;
            }
            else
            {
                return null;
            }
        }

        private  int[] makeMoveAnneal(int[] originalChessboard, int heuristics, double temperature)
        {

            while (true)
            {
                List<int[]> listNew = new List<int[]>();
                int newStolpec = (int)(ranObj.NextDouble() * originalChessboard.Length);
               // Izračun novih pozicij
                int newVrstica = (int)(ranObj.NextDouble() * originalChessboard.Length);

                int fooVrstica = originalChessboard[newStolpec]; // Hranimo stanje pred premikom
                originalChessboard[newStolpec] = newVrstica; // Premaknemo kraljico in ponovno izračunamo hevristiko

                int newHeuristics = HeuristicCalculator(originalChessboard); // Če je nova hevristika boljša, jo obdržimo
                if (newHeuristics < heuristics)
                {
                    return originalChessboard;
                }
                int deltaHeuristics = heuristics - newHeuristics; // Če ni boljša, uporabimo formulo iz psevdokode in če je verjetnost večja od nakljucne vrednosti, vrnemo novo sahovnico
                double probability = Math.Pow(Math.E,(-(double)deltaHeuristics / temperature));

                if (ranObj.NextDouble() < probability)
                {
                    return originalChessboard;
                }

                originalChessboard[newStolpec] = fooVrstica; // Drugače spet premaknemo nazaj na isto mesto

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) // Generiraj in nariši šahovnico
        {
            DrawChessboard();
            int[] generated = generateRandomQueens(Convert.ToInt32(sizeText.Text));
            chess = generated;
            heuristicText.Text = Convert.ToString(HeuristicCalculator(chess));

            PlaceQueens(generated);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) // Solve button
        {
            DrawChessboard();
            if (hillRadio.IsChecked == true)
            {
                chess = firstChoiceHillClimbing(Convert.ToInt32(sizeText.Text), Convert.ToInt32(stepsText.Text));
                if ( chess != null)
                {
                    PlaceQueens(chess);
                    heuristicText.Text = Convert.ToString(HeuristicCalculator(chess));
                }
                else
                {
                    MessageBox.Show("No solution found in specified steps");
                }

            }
            else if (annealRadio.IsChecked == true)
            {
                chess = SimulatedAnnealing(Convert.ToInt32(sizeText.Text), Convert.ToInt32(temperatureText.Text), Convert.ToDouble(coolingText.Text));
                if(chess != null)
                {
                    PlaceQueens(chess);
                    heuristicText.Text = Convert.ToString(HeuristicCalculator(chess));
                }
                else
                {
                    MessageBox.Show("No solution found!");

                }

            }
            else
            {
                chess = LocalBeamSearch(Convert.ToInt32(sizeText.Text), Convert.ToInt32(maxIterationsText.Text), Convert.ToInt32(kStatesText.Text));
                if(chess != null)
                {
                    PlaceQueens(chess);
                    heuristicText.Text = Convert.ToString(HeuristicCalculator(chess));
                }
                else
                {
                    MessageBox.Show("No solution found!");

                }
            }
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            coolingLabel.Visibility = Visibility.Hidden;
            temperatureLabel.Visibility = Visibility.Hidden;
            coolingText.Visibility = Visibility.Hidden;
            temperatureText.Visibility = Visibility.Hidden;
            stepsText.Visibility = Visibility.Hidden;
            hillStepsLabel.Visibility = Visibility.Hidden;
            maxstepsLabelBeam.Visibility = Visibility.Visible;
            kStatesLabel.Visibility = Visibility.Visible;
            maxIterationsText.Visibility = Visibility.Visible;
            kStatesText.Visibility = Visibility.Visible;
        }
    }
}
