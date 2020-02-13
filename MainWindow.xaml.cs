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
using Battleship.Model;
using System.Media;

namespace Battleship
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GameController ctrl = new GameController(5);
        

        List<Button> playerGridButtons = new List<Button>();
        List<Button> computerGridButtons = new List<Button>();
        bool gameRunning = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Place Ships
            ctrl.Setup();

            BuildGrid(playerPanel, playerGridButtons, false, false);
            BuildGrid(computerPanel, computerGridButtons, true, true);
        }

        private void BuildGrid(StackPanel panel, List<Button> gridButtons, bool enabled, bool hidden)
        {
            for (int row = 0; row < 5; ++row)
            {
                StackPanel rowPanel = new StackPanel()
                {
                    Orientation = Orientation.Horizontal
                };
                panel.Children.Add(rowPanel);
                for (int col = 0; col < 5; ++col)
                {
                    int btnNum = row * 3 + col;
                    Button b = MakeButton(enabled);
                    b.Tag = new Location(row, col);

                    //adding images to ships
                    Image img = new Image();
                    img.Height = b.Height;
                    img.Width = b.Width;

                    if(hidden)
                        img.Source = new BitmapImage(new Uri("Assets/water.png", UriKind.Relative));
                    else
                    {
                        if(ctrl.PlayerBoard.Cells[row, col] == CellType.Water) //this is biased to the player's view, not accurate to computer's board
                            img.Source = new BitmapImage(new Uri("Assets/water.png", UriKind.Relative));
                        else if(ctrl.PlayerBoard.Cells[row, col] == CellType.Ship)
                            img.Source = new BitmapImage(new Uri("Assets/ship.png", UriKind.Relative));
                    }

                    b.Content = img;

                    rowPanel.Children.Add(b);
                    gridButtons.Add(b);
                }
            }
        }

        // Note: The only buttons that can be clicked are those that are in the
        // enemy's grid
        private void btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            Location loc = (Location)btn.Tag;
            Image img = new Image();
            img.Height = btn.Height;
            img.Width = btn.Width;

            Image cimg = new Image();
            cimg.Height = btn.Height;
            cimg.Width = btn.Width;

            SoundPlayer player = new SoundPlayer();


            if (gameRunning) {
                // Update model to reflect player's attack
                AttackResult playerAttackResult = ctrl.AttackComputer(loc);

                if (playerAttackResult == AttackResult.Miss)
                {
                    txtFeedback.Text = "you missed";
                    img.Source = new BitmapImage(new Uri("Assets/miss.jpg", UriKind.Relative));
                    player.SoundLocation = "Assets/miss.wav";
                }
                else if (playerAttackResult == AttackResult.Hit)
                {
                    txtFeedback.Text = "Wooh! You hit a ship!!";
                    img.Source = new BitmapImage(new Uri("Assets/hit.png", UriKind.Relative));
                    player.SoundLocation = "Assets/hit.wav";
                }
                else if (playerAttackResult == AttackResult.Sink)
                {
                    txtFeedback.Text = "You managed to sink a ship, good job.";
                    img.Source = new BitmapImage(new Uri("Assets/hit.png", UriKind.Relative));
                    player.SoundLocation = "Assets/sink.wav";
                }

                if (playerAttackResult != AttackResult.repeat) //ignore instead of throwing exception
                { 
                    btn.Content = img;
                    player.Load();
                    player.Play();
                }

                if (ctrl.IsGameOver())
                {
                    player.SoundLocation = "Assets/win.wav";
                    player.Load();
                    player.Play();
                    gameRunning = false;
                    EndGame("You");
                }
            }


            if (gameRunning) {
                // Determine computer's response
                ComputerAttackResult computerAttackResult = ctrl.AttackPlayer();

                if (computerAttackResult.Result == AttackResult.Miss)
                {
                    cimg.Source = new BitmapImage(new Uri("Assets/miss.jpg", UriKind.Relative));
                }
                else if (computerAttackResult.Result == AttackResult.Hit)
                {
                    cimg.Source = new BitmapImage(new Uri("Assets/hit.png", UriKind.Relative));
                }
                else if (computerAttackResult.Result == AttackResult.Sink)
                {
                    cimg.Source = new BitmapImage(new Uri("Assets/hit.png", UriKind.Relative));
                }

                for (int i = 0; i < playerGridButtons.Count; i++)
                    if ((playerGridButtons[i].Tag as Location).Column == computerAttackResult.Col && (playerGridButtons[i].Tag as Location).Row == computerAttackResult.Row)
                    {
                        playerGridButtons[i].Content = cimg;
                       break;
                    }

                if (ctrl.IsGameOver())
                {
                    player.SoundLocation = "Assets/lose.wav";
                    player.Load();
                    player.Play();
                    gameRunning = false;
                    EndGame("The computer");
                }
            }
        }

        void EndGame(string winner)
        {
            for (int i = 0; i < playerGridButtons.Count; i++)
            {
                computerGridButtons[i].IsEnabled = false;
                playerGridButtons[i].IsEnabled = false;
            }
            MessageBox.Show(winner + " won the game!");
        }
        private Button MakeButton(bool enabled)
        {
            Button b = new Button();
            b.BorderBrush = new SolidColorBrush(Colors.Black);
            b.Width = 50;
            b.Height = 50;
            b.Margin = new Thickness(10);
            b.IsEnabled = enabled;
            b.Click += btn_Click;
            return b;
        }
    }
}
