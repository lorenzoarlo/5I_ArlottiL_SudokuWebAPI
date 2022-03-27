class Sudoku_Cell
{
    public int Value { get; set; } = 0;

    public bool[] Candidates { get; set; } = new bool[Sudoku_Board.DIMENSION] { true, true, true, true, true, true, true, true, true };

    public string CandidatesString
    {
        get
        {
            string s = "";
            for (int i = 0; i < Sudoku_Board.DIMENSION; i++)
            {
                if (this.Candidates[i]) s += $"{i + 1}";
            }
            return s;
        }
    }

    public int Row { get; }

    public int Column { get; }

    public int Region { get { return Sudoku_Board.GetRegionIndex(this.Row, this.Column); } }

    // ----- CONSTRUCTOR -----
    public Sudoku_Cell(int row, int column, int value = 0)
    {
        this.Row = row;
        this.Column = column;
        this.Value = value;
        if (this.Value != 0) this.SetAllCandidates(false);
    }


    // ----- METHODS -----

    public void AcquireCandidates(Sudoku_Board board)
    {
        if (this.Value != 0)
        {
            this.SetAllCandidates(false);
            return;
        }

        this.SetAllCandidates(true);

        foreach (Sudoku_Cell neighbour in board.GetNeighbours(this.Row, this.Column))
        {
            if (neighbour.Value != 0) this.Candidates[neighbour.Value - 1] = false;
        }
    }

    public void SetAllCandidates(bool value)
    {
        for (int i = 0; i < Sudoku_Board.DIMENSION; i++) this.Candidates[i] = value;
    }

    public List<Sudoku_ValueAction> GetPossibleActions()
    {
        List<Sudoku_ValueAction> possibleActions = new List<Sudoku_ValueAction>();
        if (this.Value != 0) return possibleActions;

        for (int i = 0; i < Sudoku_Board.DIMENSION; i++)
        {
            if (this.Candidates[i]) possibleActions.Add(new Sudoku_ValueAction(this.Row, this.Column, i + 1));
        }
        return possibleActions;
    }

    public override string ToString()
    {
        return $"({this.Row},{this.Column}) = {this.Value}";
    }

}