using System;

namespace Connect4
{
    class Program
    {
        static Random random = new Random();

        const int P1 = 1;
        const int P2 = 2;

        static int curPlayer;

        const int NO_WINNER = -1;
        const int TIE = -2;

        static int winner = NO_WINNER;

        static bool isGameOver;

        const ConsoleColor EMPTY_SLOT_COLOR = ConsoleColor.White;
        const ConsoleColor P1_COLOR = ConsoleColor.Blue;
        const ConsoleColor P2_COLOR = ConsoleColor.Red;

        static ConsoleColor curPlayerColor;

        static ConsoleColor[,] board = new ConsoleColor[BOARD_ROWS, BOARD_COLS];

        const int PIECES_NEEDED_TO_WIN = 4;

        const int BOARD_ROWS = 6;
        const int BOARD_COLS = 7;
        const int BOARD_SLOTS = BOARD_ROWS * BOARD_COLS;

        const int SEARCH_STEP = 1;
        const int STARTING_PIECE = 1;

        static int piecesPlaced = 0;

        static int curRow;
        static int curCol;

        static int userCol;

        static bool isInputValid;

        const string COLS_DISPLAY_STRING = " 1  2  3  4  5  6  7";

        static void IntroduceGame()
        {
            Console.Clear();

            Console.WriteLine("Welcome to Connect 4.\nPress Enter to continue.\n");
            Console.ReadKey();

            Console.Clear();
        }

        static void DetermineStartingPlayer()
        {
            curPlayer = random.Next(P1, P2 + 1);
            curPlayerColor = curPlayer == P1 ? P1_COLOR : P2_COLOR;
        }

        static bool CurPlayerWon()
        {
            return IsHorizontalWin() || IsVerticalWin() || IsDiagonalWin();
        }

        static void Main(string[] args)
        {
            IntroduceGame();
            InitBoard();

            DetermineStartingPlayer();
            GameLoop();

            DisplayWinner();

            Console.ReadKey();
        }

        static bool IsRowValid(int row)
        {
            return row <= BOARD_ROWS - 1 && row >= 0;
        }

        static bool IsColValid(int col)
        {
            return col <= BOARD_COLS - 1 && col >= 0;
        }

        static void InitBoard()
        {
            for (int i = 0; i <= BOARD_ROWS - 1; i++)
            {
                for (int j = 0; j <= BOARD_COLS - 1; j++)
                {
                    board[i, j] = EMPTY_SLOT_COLOR;
                }
            }
        }

        static void DisplayBoard()
        {
            Console.WriteLine(COLS_DISPLAY_STRING);

            for (int i = 0; i <= BOARD_ROWS - 1; i++)
            {
                for (int j = 0; j <= BOARD_COLS - 1; j++)
                {
                    Console.ResetColor();
                    Console.ForegroundColor = board[i, j];
                    Console.Write(" O ");
                }

                Console.WriteLine();
            }

            Console.ResetColor();
        }

        static bool PlacePiece()
        {
            bool placementSuccess = false;

            curRow = BOARD_ROWS - 1;

            while (IsRowValid(curRow))
            {
                if (board[curRow, curCol] == EMPTY_SLOT_COLOR)
                {
                    board[curRow, curCol] = curPlayerColor;
                    piecesPlaced++;

                    placementSuccess = true;
                    break;
                }

                curRow--;
            }

            return placementSuccess;
        }

        static void UserInput()
        {
            Console.WriteLine();

            while (!isInputValid)
            {
                Console.Write($"P{curPlayer} COLUMN: ");
                userCol = Convert.ToInt32(Console.ReadLine());

                curCol = userCol - 1;

                isInputValid = IsColValid(curCol);

                if (isInputValid)
                {
                    isInputValid = PlacePiece();
                }
            }

            isInputValid = false;
        }

        static void PostTurnLogic()
        {
            if (winner == NO_WINNER)
            {
                SwapTurns();
            }
            else
            {
                isGameOver = true;
            }
        }

        static void GameLoop()
        {
            while (!isGameOver)
            {
                DisplayBoard();
                UserInput();

                DetermineWinner();
                PostTurnLogic();

                Console.Clear();
            }
        }

        static void DisplayWinner()
        {
            DisplayBoard();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nGAME OVER... PRESS ANY KEY TO CONTINUE");
            Console.ResetColor();

            switch (winner)
            {
                case P1:
                Console.WriteLine("P1 won - note that blue is a sad color :(");
                break;

                case P2:
                Console.WriteLine("P2 won - note that red means blood (or ketchup)!");
                break;

                default:
                Console.WriteLine("TIE - note that you probably can't tie your shoe");
                break;
            }
        }

        static int FindPiecesDiagonally(int row, int col, int rowStep, int colStep)
        {
            int pieces = 0;

            while (IsRowValid(row) &&
                IsColValid(col) &&
                board[row, col] == curPlayerColor &&
                pieces <= PIECES_NEEDED_TO_WIN)
            {
                row += rowStep;
                col += colStep;

                pieces++;
            }

            return pieces;
        }

        static bool IsDiagonalWin()
        {
            if (FindPiecesDiagonally(curRow + SEARCH_STEP, curCol - SEARCH_STEP, SEARCH_STEP, -SEARCH_STEP)
                == PIECES_NEEDED_TO_WIN - STARTING_PIECE)
            {
                return true;
            }
            else
            {
                return FindPiecesDiagonally(curRow + SEARCH_STEP, curCol + SEARCH_STEP,
                        SEARCH_STEP, SEARCH_STEP) == PIECES_NEEDED_TO_WIN - STARTING_PIECE;
            }
        }

        static int FindPiecesVertically(int startRow, int step, int curPieces)
        {
            int pieces = 0;

            for (int row = startRow; IsRowValid(row); row += step)
            {
                if (board[row, curCol] == curPlayerColor && pieces + curPieces <= PIECES_NEEDED_TO_WIN)
                {
                    pieces++;
                }
                else
                {
                    break;
                }
            }

            return pieces;
        }

        static bool IsVerticalWin()
        {
            int pieces = STARTING_PIECE;

            pieces += FindPiecesVertically(curRow - SEARCH_STEP, -SEARCH_STEP, pieces);

            if (pieces < PIECES_NEEDED_TO_WIN)
            {
                pieces += FindPiecesVertically(curRow + SEARCH_STEP, SEARCH_STEP, pieces);
            }

            return pieces == PIECES_NEEDED_TO_WIN;
        }

        static int FindPiecesHorizontally(int startCol, int step, int curPieces)
        {
            int pieces = 0;

            for (int col = startCol; IsColValid(col); col += step)
            {
                if (board[curRow, col] == curPlayerColor && pieces + curPieces <= PIECES_NEEDED_TO_WIN)
                {
                    pieces++;
                }
                else
                {
                    break;
                }
            }

            return pieces;
        }

        static bool IsHorizontalWin()
        {
            int pieces = STARTING_PIECE;

            pieces += FindPiecesHorizontally(curCol + SEARCH_STEP, SEARCH_STEP, pieces);

            if (pieces < PIECES_NEEDED_TO_WIN)
            {
                pieces += FindPiecesHorizontally(curCol - SEARCH_STEP, -SEARCH_STEP, pieces);
            }

            return pieces == PIECES_NEEDED_TO_WIN;
        }

        static void DetermineWinner()
        {
            if (CurPlayerWon())
            {
                winner = curPlayer;
            }
            else
            {
                if (piecesPlaced == BOARD_SLOTS)
                {
                    winner = TIE;
                }
            }
        }

        static void SwapTurns()
        {
            curPlayer = curPlayer == P1 ? P2 : P1;
            curPlayerColor = curPlayerColor == P1_COLOR ? P2_COLOR : P1_COLOR;
        }
    }
}