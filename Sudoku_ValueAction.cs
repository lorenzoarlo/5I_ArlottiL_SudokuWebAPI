using System.Text.Json.Nodes;
class Sudoku_ValueAction : Sudoku_Action
{
    public override string ActionType { get; } = "ValueAction";

    public int Value { get; }

    public Stack<Sudoku_CandidateAction> CandidatesModifed { get; set; } = new Stack<Sudoku_CandidateAction>();

    public Sudoku_ValueAction(int row, int column, int value) : base(row, column)
    {
        this.Value = value;
    }

    public override JsonObject GetDTO() 
    {
        JsonObject tmp = new JsonObject();
        tmp.Add("ActionType", this.ActionType);
        tmp.Add("Row", this.Row);
        tmp.Add("Column", this.Column);
        tmp.Add("Value", this.Value);
        return tmp;
    }

}
