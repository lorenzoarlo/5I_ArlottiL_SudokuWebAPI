class Sudoku_CandidateAction : Sudoku_Action
{
    public int CandidateIndex { get; }
    public bool Value { get; }
    public Sudoku_CandidateAction(int row, int column, int candidateIndex, bool value) : base(row, column)
    {
        this.CandidateIndex = candidateIndex;
        this.Value = value;
    }
}