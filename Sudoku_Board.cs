class Sudoku_Board
{
    // ----- CONSTANTS -----

    public const int DIMENSION = 9;

    public const int REGION_DIMENSION = 3;

    // ----- PROPERTIES -----

    public Sudoku_Cell[,] Board = new Sudoku_Cell[DIMENSION, DIMENSION];
    public bool IsCompleted
    {
        get
        {
            return this.GetAllPossibleActions().Count() == 0;
        }
    }

    public string Mission { get; }

    public string Solution
    {
        get
        {
            Sudoku_Board clone = new Sudoku_Board(this.Mission);
            bool resolved = clone.Resolve_Recursively();
            return (resolved) ? clone.ToString() : null;
        }
    }

    public bool IsCorrect
    {
        get
        {
            for (int i = 0; i < DIMENSION; i++)
            {
                //-> Rows
                bool[] possibleValues = new bool[DIMENSION];
                foreach (Sudoku_Cell cell in this.GetRow(i))
                {
                    if (cell.Value == 0) continue;
                    int n = cell.Value - 1;
                    if (possibleValues[n]) return false;
                    possibleValues[n] = true;
                }

                //-> Columns
                possibleValues = new bool[DIMENSION];
                foreach (Sudoku_Cell cell in this.GetColumn(i))
                {
                    if (cell.Value == 0) continue;
                    int n = cell.Value - 1;
                    if (possibleValues[n]) return false;
                    possibleValues[n] = true;
                }

                //-> Regions
                possibleValues = new bool[DIMENSION];
                foreach (Sudoku_Cell cell in this.GetRegion(i))
                {
                    if (cell.Value == 0) continue;
                    int n = cell.Value - 1;
                    if (possibleValues[n]) return false;
                    possibleValues[n] = true;
                }
            }
            return true;
        }
    }

    // ----- CONSTRUCTORS -----

    public Sudoku_Board(string mission)
    {
        this.Mission = mission;
        this.Initialize();
    }


    // ----- STATIC FUNCTIONS -----

    public static int GetRegionIndex(int row, int column) => (row / REGION_DIMENSION) * REGION_DIMENSION + (column / REGION_DIMENSION);

    // ----- METHODS -----

    public Sudoku_Cell[] GetRow(int rowIndex)
    {
        Sudoku_Cell[] row = new Sudoku_Cell[DIMENSION];
        for (int column = 0; column < DIMENSION; column++)
        {
            row[column] = this.Board[rowIndex, column];
        }
        return row;
    }

    public Sudoku_Cell[] GetColumn(int columnIndex)
    {
        Sudoku_Cell[] column = new Sudoku_Cell[DIMENSION];
        for (int row = 0; row < DIMENSION; row++)
        {
            column[row] = this.Board[row, columnIndex];
        }
        return column;
    }

    public Sudoku_Cell[] GetRegion(int regionIndex)
    {
        Sudoku_Cell[] region = new Sudoku_Cell[DIMENSION];
        int initialRow = (regionIndex / REGION_DIMENSION) * REGION_DIMENSION;
        int initialColumn = (regionIndex % REGION_DIMENSION) * REGION_DIMENSION;

        for (int i = 0; i < REGION_DIMENSION; i++)
        {
            for (int j = 0; j < REGION_DIMENSION; j++)
            {
                region[i * REGION_DIMENSION + j] = this.Board[initialRow + i, initialColumn + j];
            }
        }
        return region;
    }

    public Sudoku_Cell GetEmptyCell()
    {
        foreach (Sudoku_Cell cell in this.Board)
        {
            if (cell.Value == 0) return cell;
        }
        return null;
    }


    private void Initialize()
    {
        this.Board = new Sudoku_Cell[DIMENSION, DIMENSION];
        for (int row = 0; row < DIMENSION; row++)
        {
            for (int column = 0; column < DIMENSION; column++)
            {
                int index = row * DIMENSION + column;
                int cellValue = int.Parse($"{this.Mission[index]}");

                this.Board[row, column] = new Sudoku_Cell(row, column, cellValue);
            }
        }

        for (int row = 0; row < DIMENSION; row++)
        {
            for (int column = 0; column < DIMENSION; column++)
            {
                this.Board[row, column].AcquireCandidates(this);
            }
        }


    }

    public List<Sudoku_Action> GetAllPossibleActions()
    {
        List<Sudoku_Action> possibleActions = new List<Sudoku_Action>();
        for (int row = 0; row < DIMENSION; row++)
        {
            for (int column = 0; column < DIMENSION; column++)
            {
                if (this.Board[row, column].Value != 0) continue;

                possibleActions.AddRange(this.Board[row, column].GetPossibleActions());
            }
        }
        return possibleActions;
    }

    public void ApplyAction(Sudoku_ValueAction action)
    {
        this.Board[action.Row, action.Column].Value = action.Value;

        Sudoku_Cell[] myRow = this.GetRow(action.Row);

        Sudoku_Cell[] myColumn = this.GetColumn(action.Column);

        Sudoku_Cell[] myRegion = this.GetRegion(Sudoku_Board.GetRegionIndex(action.Row, action.Column));

        for (int i = 0; i < Sudoku_Board.DIMENSION; i++)
        {
            if (myRow[i].Value == 0 && myRow[i].Candidates[action.Value - 1])
            {
                myRow[i].Candidates[action.Value - 1] = false;
                action.CandidatesModifed.Push(new Sudoku_CandidateAction(myRow[i].Row, myRow[i].Column, action.Value - 1, false));
            }

            if (myColumn[i].Value == 0 && myColumn[i].Candidates[action.Value - 1])
            {
                myColumn[i].Candidates[action.Value - 1] = false;
                action.CandidatesModifed.Push(new Sudoku_CandidateAction(myColumn[i].Row, myColumn[i].Column, action.Value - 1, false));
            }

            if (myRegion[i].Value == 0 && myRegion[i].Candidates[action.Value - 1])
            {
                myRegion[i].Candidates[action.Value - 1] = false;
                action.CandidatesModifed.Push(new Sudoku_CandidateAction(myRegion[i].Row, myRegion[i].Column, action.Value - 1, false));
            }
        }

    }

    public void ApplyAction(Sudoku_CandidateAction action)
    {
        this.Board[action.Row, action.Column].Candidates[action.CandidateIndex] = action.Value;
    }

    public void DeApplyAction(Sudoku_ValueAction action)
    {
        while (action.CandidatesModifed.Any()) this.DeApplyAction(action.CandidatesModifed.Pop());

        this.Board[action.Row, action.Column].Value = 0;
        this.Board[action.Row, action.Column].AcquireCandidates(this);
    }

    public void DeApplyAction(Sudoku_CandidateAction action)
    {
        this.Board[action.Row, action.Column].Candidates[action.CandidateIndex] = !action.Value;
    }

    // Da implementare
    public Sudoku_DTO.Sudoku_Difficulty RateDifficulty()
    {
        return Sudoku_DTO.Sudoku_Difficulty.Easy;
    }
    public Sudoku_DTO GetDTO()
    {
        return new Sudoku_DTO(this.Mission, this.Solution, this.RateDifficulty());
    }

    public bool Resolve_Recursively()
    {
        if (this.IsCompleted) return true;

        Sudoku_Cell next = this.GetEmptyCell();

        List<Sudoku_ValueAction> guesses = next.GetPossibleActions();

        foreach (Sudoku_ValueAction guess in guesses)
        {
            if (this.IsCorrect)
            {
                this.ApplyAction(guess);

                if (this.Resolve_Recursively()) return true;

            }
            this.DeApplyAction(guess);
        }
        return false;
    }

    public override string ToString()
    {
        string toReturn = "";
        for (int row = 0; row < DIMENSION; row++)
        {
            for (int column = 0; column < DIMENSION; column++)
            {
                toReturn += this.Board[row, column].Value;
            }
        }
        return toReturn;
    }
}