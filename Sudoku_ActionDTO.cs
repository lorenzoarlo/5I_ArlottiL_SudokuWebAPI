public class Sudoku_ActionDTO
{
    public string ActionType { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
    public int? Value { get; set; }
    public int? CandidateIndex { get; set; }
    public bool? CandidateValue { get; set; }

    public Sudoku_Action GetAction() {
        if(this.ActionType == "Sudoku_ValueAction") {
            return new Sudoku_ValueAction(this.Row, this.Column, (int) this.Value);
        }else if(this.ActionType == "Sudoku_CandidateAction") {
            return new Sudoku_CandidateAction(this.Row, this.Column, (int) this.CandidateIndex, (bool) this.CandidateValue);
        }
        return null;
    }
    
}