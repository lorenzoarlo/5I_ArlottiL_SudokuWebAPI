public class Sudoku_ValueAction : Sudoku_Action
{
    public int Value { get; }

    public Stack<Sudoku_CandidateAction> CandidatesModifed { get; set; } = new Stack<Sudoku_CandidateAction>();

    public Sudoku_ValueAction(int row, int column, int value) : base(row, column)
    {
        this.Value = value;
    }

    public override Sudoku_ActionDTO GetDTO() => new Sudoku_ActionDTO {
        ActionType = "Sudoku_ValueAction",
        Row = this.Row,
        Column = this.Column,
        Value = this.Value,
        CandidateIndex = null,
        CandidateValue = null
    };

}
