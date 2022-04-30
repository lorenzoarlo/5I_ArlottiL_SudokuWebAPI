public class Sudoku_CandidateAction : Sudoku_Action
{
    public int CandidateIndex { get; }
    public bool Value { get; }
    public Sudoku_CandidateAction(int row, int column, int candidateIndex, bool value) : base(row, column)
    {
        this.CandidateIndex = candidateIndex;
        this.Value = value;
    }

    public override Sudoku_ActionDTO GetDTO() => new Sudoku_ActionDTO {
        ActionType = "Sudoku_CandidateAction",
        Row = this.Row,
        Column = this.Column,
        Value = null,
        CandidateIndex = this.CandidateIndex,
        CandidateValue = this.Value
    };

}