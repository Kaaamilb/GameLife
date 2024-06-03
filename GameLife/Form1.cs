using System;
using System.Drawing;
using System.Windows.Forms;

namespace GameLife
{
    public partial class Form1 : Form
    {
        private const int MapSize = 30;
        private const int CellSize = 20;
        private const int Offset = 25;
        private const int TimerInterval = 100;

        private int[,] currentState = new int[MapSize, MapSize];
        private int[,] nextState = new int[MapSize, MapSize];
        private Button[,] cells = new Button[MapSize, MapSize];
        private bool isPlaying = false;
        private Timer mainTimer;

        public Form1()
        {
            InitializeComponent();
            SetFormSize();
            BuildMenu();
            InitializeGame();
        }

        private void SetFormSize()
        {
            Width = (MapSize + 1) * CellSize;
            Height = (MapSize + 1) * CellSize + 40;
        }

        private void InitializeGame()
        {
            isPlaying = false;
            InitializeTimer();
            currentState = InitializeMap();
            nextState = InitializeMap();
            InitializeCells();
        }

        private void InitializeTimer()
        {
            mainTimer = new Timer
            {
                Interval = TimerInterval
            };
            mainTimer.Tick += UpdateStates;
        }

        private void ClearGame()
        {
            isPlaying = false;
            InitializeTimer();
            currentState = InitializeMap();
            nextState = InitializeMap();
            ResetCells();
        }

        private void ResetCells()
        {
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    cells[i, j].BackColor = Color.White;
                }
            }
        }

        private void BuildMenu()
        {
            var menu = new MenuStrip();
            var restartMenuItem = new ToolStripMenuItem("Начать заного", null, RestartGame);
            var playMenuItem = new ToolStripMenuItem("Начать симуляцию", null, StartSimulation);

            menu.Items.AddRange(new ToolStripItem[] { playMenuItem, restartMenuItem });
            Controls.Add(menu);
        }

        private void RestartGame(object sender, EventArgs e)
        {
            mainTimer.Stop();
            ClearGame();
        }

        private void StartSimulation(object sender, EventArgs e)
        {
            if (!isPlaying)
            {
                isPlaying = true;
                mainTimer.Start();
            }
        }

        private void UpdateStates(object sender, EventArgs e)
        {
            CalculateNextState();
            DisplayMap();
            if (IsGenerationDead())
            {
                mainTimer.Stop();
                MessageBox.Show(":(");
            }
        }

        private bool IsGenerationDead()
        {
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    if (currentState[i, j] == 1)
                        return false;
                }
            }
            return true;
        }

        private void CalculateNextState()
        {
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    int neighborsCount = CountNeighbors(i, j);
                    bool isAlive = currentState[i, j] == 1;

                    nextState[i, j] = isAlive
                        ? (neighborsCount == 2 || neighborsCount == 3 ? 1 : 0)
                        : (neighborsCount == 3 ? 1 : 0);
                }
            }

            currentState = nextState;
            nextState = InitializeMap();
        }

        private void DisplayMap()
        {
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    cells[i, j].BackColor = currentState[i, j] == 1 ? Color.Black : Color.White;
                }
            }
        }

        private int CountNeighbors(int x, int y)
        {
            int count = 0;
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (IsInsideMap(i, j) && !(i == x && j == y) && currentState[i, j] == 1)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        private bool IsInsideMap(int x, int y)
        {
            return x >= 0 && x < MapSize && y >= 0 && y < MapSize;
        }

        private int[,] InitializeMap()
        {
            return new int[MapSize, MapSize];
        }

        private void InitializeCells()
        {
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    var button = new Button
                    {
                        Size = new Size(CellSize, CellSize),
                        BackColor = Color.White,
                        Location = new Point(j * CellSize, i * CellSize + Offset)
                    };
                    button.Click += CellClicked;
                    Controls.Add(button);
                    cells[i, j] = button;
                }
            }
        }

        private void CellClicked(object sender, EventArgs e)
        {
            if (!isPlaying)
            {
                var button = sender as Button;
                var x = (button.Location.Y - Offset) / CellSize;
                var y = button.Location.X / CellSize;

                currentState[x, y] ^= 1;
                button.BackColor = currentState[x, y] == 1 ? Color.Black : Color.White;
            }
        }
    }
}
