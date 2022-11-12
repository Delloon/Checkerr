using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CheckersGame
{
    public partial class Form1 : Form
    {
        private const int _mapSize = 8;
        private const int _cellSize = 65;

        private int currentPlayer;

        List<Button> simpleSteps = new List<Button>();

        private int countEatSteps = 0;
        Button prevButton;
        Button pressedButton;
        private bool isContinue = false;

        private bool isMoving;

        int[,] map = new int[_mapSize, _mapSize];

        Button[,] buttons = new Button[_mapSize, _mapSize];

        Image whiteChecker;
        Image whiteCheckerQuin;
        Image blackChecker;
        Image blackCheckerQuin;

        public Form1()
        {
            InitializeComponent();
            MessageBox.Show("Привет, надеюсь ты разберешся с кодом. Спрайты находятся в папке с проектом просто пропиши путь заново и всё будет ок!");
            whiteChecker = new Bitmap(new Bitmap(@"D:\download\CheckersGame-master\CheckersGame-master\CheckersGame\Sprites\w.png"), new Size(_cellSize - 10, _cellSize - 10));
            whiteCheckerQuin = new Bitmap(new Bitmap(@"D:\download\CheckersGame-master\CheckersGame-master\CheckersGame\Sprites\w-d.png"), new Size(_cellSize - 10, _cellSize - 10));
            blackChecker = new Bitmap(new Bitmap(@"D:\download\CheckersGame-master\CheckersGame-master\CheckersGame\Sprites\b.png"), new Size(_cellSize - 10, _cellSize - 10));
            blackCheckerQuin = new Bitmap(new Bitmap(@"D:\download\CheckersGame-master\CheckersGame-master\CheckersGame\Sprites\b-d.png"), new Size(_cellSize - 10, _cellSize - 10));

            this.Text = "Шашки для обучения";
            
            //GameStart();
        }

        public void GameStart()
        {
            currentPlayer = 1;
            isMoving = false;
            prevButton = null;

            map = new int[_mapSize,_mapSize] {
                { 0,1,0,1,0,1,0,1 },
                { 1,0,1,0,1,0,1,0 },
                { 0,1,0,1,0,1,0,1 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 2,0,2,0,2,0,2,0 },
                { 0,2,0,2,0,2,0,2 },
                { 2,0,2,0,2,0,2,0 }
            };

            MapCreate();
        }
        private void QuitGame()
        {
            panel1.Controls.Clear();
            MessageBox.Show("Игра завершена");
        }
        public void ResetGame()
        {
            bool player_1 = false;
            bool player_2 = false;

            for(int i = 0; i < _mapSize; i++)
            {
                for (int j = 0; j < _mapSize; j++)
                {
                    if (map[i, j] == 1)
                        player_1 = true;
                    if (map[i, j] == 2)
                        player_2 = true;
                }
            }
            if (!player_1 || !player_2)
            {
                panel1.Controls.Clear();
                MessageBox.Show("Игра окончена!");
                GameStart();
            }
        }

        public void MapCreate()
        {
            

            for(int i = 0; i < _mapSize; i++)
            {
                for (int j = 0; j < _mapSize; j++)
                {
                    Button button = new Button();
                    button.Location = new Point(j * _cellSize, i * _cellSize);
                    button.Size = new Size(_cellSize, _cellSize);
                    button.Click += new EventHandler(OnFigurePress);
                    if (map[i, j] == 1)
                        button.Image = whiteChecker;
                    else if (map[i, j] == 2) button.Image = blackChecker;

                    button.BackColor = GetPrevButtonColor(button);
                    button.ForeColor = Color.Green;

                    buttons[i, j] = button;

                    panel1.Controls.Add(button);
                }
            }
        }

        public void SwitchPlayer()
        {
            currentPlayer = currentPlayer == 1 ? 2 : 1;
            ResetGame();
        }

        public Color GetPrevButtonColor(Button prevButton)
        {
            if ((prevButton.Location.Y/_cellSize % 2) != 0)
            {
                if ((prevButton.Location.X / _cellSize % 2) == 0)
                {
                    return Color.Gray;
                }
            }
            if ((prevButton.Location.Y / _cellSize) % 2 == 0)
            {
                if ((prevButton.Location.X / _cellSize) % 2 != 0)
                {
                    return Color.Gray;
                }
            }
            return Color.White;
        }

        public void OnFigurePress(object sender, EventArgs e)
        {
            if (prevButton != null)
                prevButton.BackColor = GetPrevButtonColor(prevButton);

            pressedButton = sender as Button;

            if(map[pressedButton.Location.Y/_cellSize,pressedButton.Location.X/_cellSize] != 0 && map[pressedButton.Location.Y / _cellSize, pressedButton.Location.X / _cellSize] == currentPlayer)
            {
                CloseSteps();
                pressedButton.BackColor = Color.Green;
                DeactivateAllButtons();
                pressedButton.Enabled = true;
                countEatSteps = 0;
                if(pressedButton.Text == "D")
                ShowSteps(pressedButton.Location.Y / _cellSize, pressedButton.Location.X / _cellSize,false);
                else ShowSteps(pressedButton.Location.Y / _cellSize, pressedButton.Location.X / _cellSize);

                if (isMoving)
                {
                    CloseSteps();
                    pressedButton.BackColor = GetPrevButtonColor(pressedButton);
                    ShowPossibleSteps();
                    isMoving = false;
                }
                else
                    isMoving = true;
            }
            else
            {
                if (isMoving)
                {
                    isContinue = false;
                      if (Math.Abs(pressedButton.Location.X / _cellSize - prevButton.Location.X/_cellSize) > 1)
                    {
                        isContinue = true;
                        DeleteEaten(pressedButton, prevButton);                        
                    }
                    int temp = map[pressedButton.Location.Y / _cellSize, pressedButton.Location.X / _cellSize];
                    map[pressedButton.Location.Y / _cellSize, pressedButton.Location.X / _cellSize] = map[prevButton.Location.Y / _cellSize, prevButton.Location.X / _cellSize];
                    map[prevButton.Location.Y / _cellSize, prevButton.Location.X / _cellSize] = temp;
                    pressedButton.Image = prevButton.Image;
                    prevButton.Image = null;
                    pressedButton.Text = prevButton.Text;
                    prevButton.Text = "";
                    SwitchButtonToCheat(pressedButton);
                    countEatSteps = 0;
                    isMoving = false;                    
                    CloseSteps();
                    DeactivateAllButtons();
                    if (pressedButton.Text == "D")
                        ShowSteps(pressedButton.Location.Y / _cellSize, pressedButton.Location.X / _cellSize, false);
                    else ShowSteps(pressedButton.Location.Y / _cellSize, pressedButton.Location.X / _cellSize);
                    if (countEatSteps == 0 || !isContinue)
                    {
                        CloseSteps();
                        SwitchPlayer();
                        ShowPossibleSteps();
                        isContinue = false;
                    }else if(isContinue)
                    {
                        pressedButton.BackColor = Color.Red;
                        pressedButton.Enabled = true;
                        isMoving = true;
                    }
                }
            }

            prevButton = pressedButton;
        }

        public void ShowPossibleSteps()
        {
            bool isOneStep = true;
            bool isEatStep = false;
            DeactivateAllButtons();
            for(int i = 0; i < _mapSize; i++)
            {
                for (int j= 0; j < _mapSize; j++)
                {
                    if (map[i, j] == currentPlayer)
                    {
                        if (buttons[i, j].Text == "D")
                            isOneStep = false;
                        else isOneStep = true;
                        if (IsButtonHasEatStep(i, j, isOneStep, new int[2] { 0, 0 }))
                        {
                            isEatStep = true;
                            buttons[i, j].Enabled = true;
                        }
                    }
                }
            }
            if (!isEatStep)
                ActivateAllButtons();
        }

        public void SwitchButtonToCheat(Button button)
        {
            if (map[button.Location.Y / _cellSize, button.Location.X / _cellSize] == 1 && button.Location.Y / _cellSize == _mapSize - 1) 
            {

                button.Image = whiteCheckerQuin;
                button.Text = "D";
                
            }
            if (map[button.Location.Y / _cellSize, button.Location.X / _cellSize] == 2 && button.Location.Y / _cellSize == 0)
            {
                button.Image = blackCheckerQuin;
                button.Text = "D";
                
            }
        }

        public void DeleteEaten(Button endButton, Button startButton)
        {
            int count = Math.Abs(endButton.Location.Y / _cellSize - startButton.Location.Y / _cellSize);
            int startIndexX = endButton.Location.Y / _cellSize - startButton.Location.Y / _cellSize;
            int startIndexY = endButton.Location.X / _cellSize - startButton.Location.X / _cellSize;
            startIndexX = startIndexX < 0 ? -1 : 1;
            startIndexY = startIndexY < 0 ? -1 : 1;
            int currCount = 0;
            int i = startButton.Location.Y / _cellSize + startIndexX;
            int j = startButton.Location.X / _cellSize + startIndexY;
            while (currCount < count-1)
            {
                map[i, j] = 0;
                buttons[i, j].Image = null;
                buttons[i, j].Text = "";
                i += startIndexX;
                j += startIndexY;
                currCount++;
            }

        }

        public void ShowSteps(int iCurrFigure, int jCurrFigure,bool isOnestep = true)
        {
            simpleSteps.Clear();
            ShowDiagonal(iCurrFigure, jCurrFigure,isOnestep);
            if (countEatSteps > 0)
                CloseSimpleSteps(simpleSteps);
        }

        public void ShowDiagonal(int IcurrFigure, int JcurrFigure, bool isOneStep = false)
        {
            int j = JcurrFigure + 1;
            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (currentPlayer == 1 && isOneStep && !isContinue) break;
                if (IsInsideBorders(i, j))
                {
                    if (!DeterminePath(i, j))
                        break;
                }
                if (j < 7)
                    j++;
                else break;

                if (isOneStep)
                    break;
            }

            j = JcurrFigure - 1;
            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (currentPlayer == 1 && isOneStep && !isContinue) break;
                if (IsInsideBorders(i, j))
                {
                    if (!DeterminePath(i, j))
                        break;
                }
                if (j > 0)
                    j--;
                else break;

                if (isOneStep)
                    break;
            }

            j = JcurrFigure - 1;
            for (int i = IcurrFigure + 1; i < 8; i++)
            {
                if (currentPlayer == 2 && isOneStep && !isContinue) break;
                if (IsInsideBorders(i, j))
                {
                    if (!DeterminePath(i, j))
                        break;
                }
                if (j > 0)
                    j--;
                else break;

                if (isOneStep)
                    break;
            }

            j = JcurrFigure + 1;
            for (int i = IcurrFigure + 1; i < 8; i++)
            {
                if (currentPlayer == 2 && isOneStep && !isContinue) break;
                if (IsInsideBorders(i, j))
                {
                    if (!DeterminePath(i, j))
                        break;
                }
                if (j < 7)
                    j++;
                else break;

                if (isOneStep)
                    break;
            }
        }
        
        public bool DeterminePath(int ti,int tj)
        {
            
            if (map[ti, tj] == 0 && !isContinue)
            {
                buttons[ti, tj].BackColor = Color.Green;
                buttons[ti, tj].Enabled = true;
                simpleSteps.Add(buttons[ti, tj]);
            }else
            {
                
                if (map[ti, tj] != currentPlayer)
                {
                    if (pressedButton.Text == "D")
                        ShowProceduralEat(ti, tj, false);
                    else ShowProceduralEat(ti, tj);
                }

                return false;
            }
            return true;
        }

        public void CloseSimpleSteps(List<Button> simpleSteps)
        {
            if (simpleSteps.Count > 0)
            {
                for (int i = 0; i < simpleSteps.Count; i++)
                {
                    simpleSteps[i].BackColor = GetPrevButtonColor(simpleSteps[i]);
                    simpleSteps[i].Enabled = false;
                }
            }
        }
        public void ShowProceduralEat(int i,int j,bool isOneStep = true)
        {
            int dirX = i - pressedButton.Location.Y / _cellSize;
            int dirY = j - pressedButton.Location.X / _cellSize;
            dirX = dirX < 0 ? -1 : 1;
            dirY = dirY < 0 ? -1 : 1;
            int il = i;
            int jl = j;
            bool isEmpty = true;
            while (IsInsideBorders(il, jl))
            {
                if (map[il, jl] != 0 && map[il, jl] != currentPlayer)
                { 
                    isEmpty = false;
                    break;
                }
                il += dirX;
                jl += dirY;

                if (isOneStep)
                    break;
            }
            if (isEmpty)
                return;
            List<Button> toClose = new List<Button>();
            bool closeSimple = false;
            int ik = il + dirX;
            int jk = jl + dirY;
            while (IsInsideBorders(ik,jk))
            {
                if (map[ik, jk] == 0 )
                {
                    if (IsButtonHasEatStep(ik, jk, isOneStep, new int[2] { dirX, dirY }))
                    {
                        closeSimple = true;
                    }
                    else
                    {
                        toClose.Add(buttons[ik, jk]);
                    }
                    buttons[ik, jk].BackColor = Color.Green;
                    buttons[ik, jk].Enabled = true;
                    countEatSteps++;
                }
                else break;
                if (isOneStep)
                    break;
                jk += dirY;
                ik += dirX;
            }
            if (closeSimple && toClose.Count > 0)
            {
                CloseSimpleSteps(toClose);
            }
            
        }

        public bool IsButtonHasEatStep(int IcurrFigure, int JcurrFigure, bool isOneStep, int[] dir)
        {
            bool eatStep = false;
            int j = JcurrFigure + 1;
            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (currentPlayer == 1 && isOneStep && !isContinue) break;
                if (dir[0] == 1 && dir[1] == -1 && !isOneStep)break;
                if (IsInsideBorders(i, j))
                {
                    if (map[i, j] != 0 && map[i, j] != currentPlayer)
                    {
                        eatStep = true;
                        if (!IsInsideBorders(i - 1, j + 1))
                            eatStep = false;
                        else if (map[i - 1, j + 1] != 0)
                            eatStep = false;
                        else return eatStep;
                    }
                }
                if (j < 7)
                    j++;
                else break;

                if (isOneStep)
                    break;
            }

            j = JcurrFigure - 1;
            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (currentPlayer == 1 && isOneStep && !isContinue) break;
                if (dir[0] == 1 && dir[1] == 1 && !isOneStep) break;
                if (IsInsideBorders(i, j))
                {
                    if (map[i, j] != 0 && map[i, j] != currentPlayer)
                    {
                        eatStep = true;
                        if (!IsInsideBorders(i - 1, j - 1))
                            eatStep = false;
                        else if (map[i - 1, j - 1] != 0)
                            eatStep = false;
                        else return eatStep;
                    }
                }
                if (j > 0)
                    j--;
                else break;

                if (isOneStep)
                    break;
            }

            j = JcurrFigure - 1;
            for (int i = IcurrFigure + 1; i < 8; i++)
            {
                if (currentPlayer == 2 && isOneStep && !isContinue) break;
                if (dir[0] == -1 && dir[1] == 1 && !isOneStep) break;
                if (IsInsideBorders(i, j))
                {
                    if (map[i, j] != 0 && map[i, j] != currentPlayer)
                    {
                        eatStep = true;
                        if (!IsInsideBorders(i + 1, j - 1))
                            eatStep = false;
                        else if (map[i + 1, j - 1] != 0)
                            eatStep = false;
                        else return eatStep;
                    }
                }
                if (j > 0)
                    j--;
                else break;

                if (isOneStep)
                    break;
            }

            j = JcurrFigure + 1;
            for (int i = IcurrFigure + 1; i < 8; i++)
            {
                if (currentPlayer == 2 && isOneStep && !isContinue) break;
                if (dir[0] == -1 && dir[1] == -1 && !isOneStep) break;
                if (IsInsideBorders(i, j))
                {
                    if (map[i, j] != 0 && map[i, j] != currentPlayer)
                    {
                        eatStep = true;
                        if (!IsInsideBorders(i + 1, j + 1))
                            eatStep = false;
                        else if (map[i + 1, j + 1] != 0)
                            eatStep = false;
                        else return eatStep;
                    }
                }
                if (j < 7)
                    j++;
                else break;

                if (isOneStep)
                    break;
            }
            return eatStep;
        }

        public void CloseSteps()
        {
            for (int i = 0; i < _mapSize; i++)
            {
                for (int j = 0; j < _mapSize; j++)
                {
                    buttons[i, j].BackColor = GetPrevButtonColor(buttons[i, j]);
                }
            }
        }

        public bool IsInsideBorders(int ti,int tj)
        {
            if(ti>=_mapSize || tj >= _mapSize || ti<0 || tj < 0)
            {
                return false;
            }
            return true;
        }

        public void ActivateAllButtons()
        {
            for (int i = 0; i < _mapSize; i++)
            {
                for (int j = 0; j < _mapSize; j++)
                {
                    buttons[i, j].Enabled = true;
                }
            }
        }

        public void DeactivateAllButtons()
        {
            for (int i = 0; i < _mapSize; i++)
            {
                for (int j = 0; j < _mapSize; j++)
                {
                    buttons[i, j].Enabled = false;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GameStart();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            QuitGame();
        }
    }
}
