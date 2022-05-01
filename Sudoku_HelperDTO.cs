public class Sudoku_HelperDTO {
    public string CandidateString { get; set; }
    public string Technique { get; set; }
    public IEnumerable<Sudoku_ActionDTO> Actions { get; set; }
}