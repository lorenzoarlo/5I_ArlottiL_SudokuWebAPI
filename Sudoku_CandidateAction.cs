using System.Text.Json.Nodes;

class Sudoku_CandidateAction : Sudoku_Action
{
    public override string ActionType { get; } = "CandidateAction";
    public int CandidateIndex { get; }
    public bool Value { get; }
    public Sudoku_CandidateAction(int row, int column, int candidateIndex, bool value) : base(row, column)
    {
        this.CandidateIndex = candidateIndex;
        this.Value = value;
    }

    public override JsonObject GetDTO()
    {
        JsonObject tmp = new JsonObject();
        tmp.Add("ActionType", this.ActionType);
        tmp.Add("Column", this.Column);
        tmp.Add("Row", this.Row);
        tmp.Add("CandidateIndex", this.CandidateIndex);
        tmp.Add("Value", this.Value);
        return tmp;
    }
}