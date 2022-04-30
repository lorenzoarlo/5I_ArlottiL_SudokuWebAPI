public class Sudoku_Board
{
    // ----- CONSTANTS -----
    public const int BOARD_DIMENSION = 9;

    public const int REGION_DIMENSION = 3;

    // ----- PROPERTIES -----

    public Sudoku_Cell[,] Board { get; }
    public bool IsCompleted => !this.GetAllPossibleActions().Any();

    public string Mission { get; }

    public string Solution
    {
        get
        {
            List<Sudoku_Action> actions = Sudoku_Techniques.Recursion_Technique(this);
            Sudoku_Board clone = new Sudoku_Board(this.Mission);
            clone.ApplyActions(actions);
            return (clone.IsCompleted) ? clone.ToString() : null;
        }
    }

    public bool IsCorrect
    {
        get
        {
            for (int i = 0; i < BOARD_DIMENSION; i++)
            {
                //-> Rows
                bool[] possibleValues = new bool[BOARD_DIMENSION];
                foreach (Sudoku_Cell cell in this.GetRow(i))
                {
                    if (cell.Value == 0) continue;
                    int n = cell.Value - 1;
                    if (possibleValues[n]) return false;
                    possibleValues[n] = true;
                }

                //-> Columns
                possibleValues = new bool[BOARD_DIMENSION];
                foreach (Sudoku_Cell cell in this.GetColumn(i))
                {
                    if (cell.Value == 0) continue;
                    int n = cell.Value - 1;
                    if (possibleValues[n]) return false;
                    possibleValues[n] = true;
                }

                //-> Regions
                possibleValues = new bool[BOARD_DIMENSION];
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

    public IEnumerable<Sudoku_Cell> EmptyCells
    {
        get
        {
            List<Sudoku_Cell> cells = new List<Sudoku_Cell>();
            for (int row = 0; row < Sudoku_Board.BOARD_DIMENSION; row++)
            {
                for (int column = 0; column < Sudoku_Board.BOARD_DIMENSION; column++)
                {
                    if (this.Board[row, column].Value == 0) cells.Add(this.Board[row, column]);
                }
            }
            return cells;
        }
    }

    // ----- CONSTRUCTORS -----

    public Sudoku_Board(string mission)
    {
        this.Mission = mission;
        this.Board = new Sudoku_Cell[BOARD_DIMENSION, BOARD_DIMENSION];

        for (int row = 0; row < BOARD_DIMENSION; row++)
        {
            for (int column = 0; column < BOARD_DIMENSION; column++)
            {
                int index = row * BOARD_DIMENSION + column;
                int cellValue = int.Parse($"{this.Mission[index]}");
                this.Board[row, column] = new Sudoku_Cell(row, column, cellValue);
            }
        }

        for (int row = 0; row < BOARD_DIMENSION; row++)
        {
            for (int column = 0; column < BOARD_DIMENSION; column++)
            {
                this.Board[row, column].AcquireCandidates(this);
            }
        }

    }

    // ----- STATIC FUNCTIONS -----

    public static int GetRegionIndex(int row, int column) => (row / REGION_DIMENSION) * REGION_DIMENSION + (column / REGION_DIMENSION);

    // ----- METHODS -----

    public Sudoku_Cell[] GetRow(int rowIndex)
    {
        Sudoku_Cell[] row = new Sudoku_Cell[BOARD_DIMENSION];
        for (int column = 0; column < BOARD_DIMENSION; column++)
        {
            row[column] = this.Board[rowIndex, column];
        }
        return row;
    }

    public Sudoku_Cell[] GetColumn(int columnIndex)
    {
        Sudoku_Cell[] column = new Sudoku_Cell[BOARD_DIMENSION];
        for (int row = 0; row < BOARD_DIMENSION; row++)
        {
            column[row] = this.Board[row, columnIndex];
        }
        return column;
    }

    public Sudoku_Cell[] GetRegion(int regionIndex)
    {
        Sudoku_Cell[] region = new Sudoku_Cell[BOARD_DIMENSION];
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

    public Sudoku_Cell GetFirstEmptyCell()
    {
        for (int row = 0; row < Sudoku_Board.BOARD_DIMENSION; row++)
        {
            for (int column = 0; column < Sudoku_Board.BOARD_DIMENSION; column++)
            {
                if (this.Board[row, column].Value == 0) return this.Board[row, column];
            }
        }
        return null;
    }


    public List<Sudoku_Cell> GetNeighbours(int row, int column)
    {
        List<Sudoku_Cell> neighbours = new List<Sudoku_Cell>();

        Sudoku_Cell[] myRow = this.GetRow(row);

        Sudoku_Cell[] myColumn = this.GetColumn(column);

        Sudoku_Cell[] myRegion = this.GetRegion(Sudoku_Board.GetRegionIndex(row, column));


        for (int i = 0; i < BOARD_DIMENSION; i++)
        {
            if (i != column) neighbours.Add(myRow[i]);

            if (i != row) neighbours.Add(myColumn[i]);

            if (myRegion[i].Row != row && myRegion[i].Column != column) neighbours.Add(myRegion[i]);
        }


        return neighbours;
    }

    public List<Sudoku_Action> GetAllPossibleActions()
    {
        List<Sudoku_Action> possibleActions = new List<Sudoku_Action>();
        for (int row = 0; row < BOARD_DIMENSION; row++)
        {
            for (int column = 0; column < BOARD_DIMENSION; column++)
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
        this.Board[action.Row, action.Column].SetAllCandidates(false);

        foreach (Sudoku_Cell cell in this.GetNeighbours(action.Row, action.Column))
        {
            if (cell.Value == 0 && cell.Candidates[action.Value - 1])
            {
                cell.Candidates[action.Value - 1] = false;
                action.CandidatesModifed.Push(new Sudoku_CandidateAction(cell.Row, cell.Column, action.Value - 1, false));
            }
        }
    }

    public void ApplyActions(List<Sudoku_Action> actions)
    {
        foreach (Sudoku_Action action in actions)
        {
            if (action is Sudoku_ValueAction) this.ApplyAction(action as Sudoku_ValueAction);
            else this.ApplyAction(action as Sudoku_CandidateAction);
        }
    }

    public void DeApplyActions(List<Sudoku_Action> actions)
    {
        foreach (Sudoku_Action action in actions)
        {
            if (action is Sudoku_ValueAction) this.DeApplyAction(action as Sudoku_ValueAction);
            else this.DeApplyAction(action as Sudoku_CandidateAction);
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
        return Sudoku_DTO.Sudoku_Difficulty.Casual;
    }


    public Sudoku_DTO GetDTO()
    {
        return new Sudoku_DTO(this.Mission, this.Solution, this.RateDifficulty());
    }

    public string CandidatesString()
        {
            string toReturn = "";
            for (int row = 0; row < BOARD_DIMENSION; row++)
            {
                for (int column = 0; column < BOARD_DIMENSION; column++)
                {
                    toReturn += this.Board[row, column].AllCandidatesString;
                }
            }
            return toReturn;
        }

    public override string ToString()
    {
        string toReturn = "";
        for (int row = 0; row < BOARD_DIMENSION; row++)
        {
            for (int column = 0; column < BOARD_DIMENSION; column++)
            {
                toReturn += this.Board[row, column].Value;
            }
        }
        return toReturn;
    }
}