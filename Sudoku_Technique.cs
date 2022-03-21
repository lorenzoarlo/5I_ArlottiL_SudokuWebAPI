abstract class Sudoku_Techniques
{
    //https://www.sudokuoftheday.com/techniques/single-position
    public static List<Sudoku_Action> SingleCandidate_Technique(Sudoku_Board board)
    {
        List<Sudoku_Action> actions = new List<Sudoku_Action>();
        for (int i = 0; i < Sudoku_Board.DIMENSION; i++)
        {
            foreach (Sudoku_Cell cell in board.GetRow(i))
            {
                if (cell.Value != 0) continue;

                string candidateString = cell.CandidatesString;
                if (candidateString.Length == 1) actions.Add(new Sudoku_ValueAction(cell.Row, cell.Column, int.Parse(candidateString)));
            }
        }
        return actions;
    }

    // public static List<Sudoku_Action> SinglePosition_Technique(Sudoku_Board board)
    // {
    //     List<Sudoku_Action> actions = new List<Sudoku_Action>();
    //     return actions;
    // }

    // public static List<Sudoku_Action> CandidateLines_Technique(Sudoku_Board board)
    // {
    //     List<Sudoku_Action> actions = new List<Sudoku_Action>();
    //     return actions;
    // }

    // public static List<Sudoku_Action> DoublePairs_Technique(Sudoku_Board board)
    // {
    //     List<Sudoku_Action> actions = new List<Sudoku_Action>();
    //     return actions;
    // }

    // public static List<Sudoku_Action> MultipleLines_Technique(Sudoku_Board board)
    // {
    //     List<Sudoku_Action> actions = new List<Sudoku_Action>();
    //     return actions;
    // }

    public static List<Sudoku_Action> NakedPairs_Technique(Sudoku_Board board)
    {
        List<Sudoku_Action> actions = new List<Sudoku_Action>();
        Sudoku_Board boardClone = new Sudoku_Board(board.Mission);

        
        return actions;
    }



}