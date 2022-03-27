abstract class Sudoku_Techniques
{
    //https://www.sudokuoftheday.com/techniques
    /*
     * Single Candidate = Ok
     * Single Position  = Ok 
     * Candidate Lines  = 
     * Double Pairs     = 
     * Multiple Lines   = 
     * Naked Pair       = 
     * Hidden Pair      = 
     * Naked Triple     = 
     * Hiddent Triple   = 
     * X-Wing           = 
     * Forcing Chains   = 
     * Naked Quad       = 
     * Hidden Quad      = 
     * Swordfish        = 
     * Nishio           = X
     * Guessing         = X
     * Recursion        = Ok
     */



    /*
     * Single Candidate:
     * Se una cella ha un solo candidato possibile, il valore della cella sarà per forza quel candidato
     */
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

    /*
     * Single Position:
     * Se in una riga (o colonna, o regione), solo una cella presenta un certo candidato, il valore della cella sarà per forza quel candidato
     */
    public static List<Sudoku_Action> SinglePosition_Technique(Sudoku_Board board)
    {
        List<Sudoku_Action> actions = new List<Sudoku_Action>();

        for (int i = 0; i < Sudoku_Board.DIMENSION; i++)
        {
            actions.AddRange(SinglePosition_Scan(board.GetRow(i)));
            actions.AddRange(SinglePosition_Scan(board.GetColumn(i)));
            actions.AddRange(SinglePosition_Scan(board.GetRegion(i)));
        }


        return actions;
    }

    private static List<Sudoku_Action> SinglePosition_Scan(Sudoku_Cell[] line)
    {
        List<Sudoku_Action> actions = new List<Sudoku_Action>();
        int[] occurrencesCandidates = new int[Sudoku_Board.DIMENSION] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int[] latestIndex = new int[Sudoku_Board.DIMENSION] { -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        foreach (Sudoku_Cell cell in line)
        {
            if (cell.Value != 0) continue;
            for (int i = 0; i < Sudoku_Board.DIMENSION; i++)
            {
                if (cell.Candidates[i])
                {
                    occurrencesCandidates[i]++;
                    latestIndex[i] = i;
                }
            }
        }

        for (int i = 0; i < Sudoku_Board.DIMENSION; i++)
        {
            if (occurrencesCandidates[i] == 1)
            {
                Sudoku_Cell tmp = line[latestIndex[i]];
                actions.Add(new Sudoku_ValueAction(tmp.Row, tmp.Column, i + 1));
            }
        }
        return actions;
    }

    /*
     * Single Position:
     * Se in una riga (o colonna, o regione), solo una cella presenta un certo candidato, il valore della cella sarà per forza quel candidato
     */
    public static List<Sudoku_Action> CandidateLines_Technique(Sudoku_Board board)
    {
        List<Sudoku_Action> actions = new List<Sudoku_Action>();

        for (int regionIndex = 0; regionIndex < Sudoku_Board.DIMENSION; regionIndex++)
        {
            Sudoku_Cell[] emptyCells = board.GetRegion(regionIndex).Where(cell => cell.Value != 0).ToArray();
            if (emptyCells.Length == 0) continue;

            bool[] tryCandidates = new bool[Sudoku_Board.DIMENSION] { false, false, false, false, false, false, false, false, false };

            foreach (Sudoku_Cell cell in emptyCells) tryCandidates = ORArrayBoolean(tryCandidates, cell.Candidates);

            for (int candidateIndex = 0; candidateIndex < Sudoku_Board.DIMENSION; candidateIndex++)
            {
                if (!tryCandidates[candidateIndex]) continue;
                Sudoku_Cell[] cellsWithCandidate = emptyCells.Where(cell => cell.Candidates[candidateIndex]).ToArray();
                bool sameRow = true;
                bool sameColumn = true;
                Sudoku_Cell first = cellsWithCandidate.First();
                foreach (Sudoku_Cell cell in cellsWithCandidate)
                {
                    sameRow = sameRow && (cell.Row == first.Row);
                    sameColumn = sameColumn && (cell.Column == first.Column);
                }

                if (sameRow)
                {
                    Sudoku_Cell[] othersInRow = board.GetRow(first.Row).Where(cell => cell.Region != first.Region).ToArray();
                    foreach (Sudoku_Cell other in othersInRow) if (other.Candidates[candidateIndex]) actions.Add(new Sudoku_CandidateAction(other.Row, other.Column, candidateIndex, false));
                }
                else if (sameColumn)
                {
                    Sudoku_Cell[] othersInColumn = board.GetColumn(first.Column).Where(cell => cell.Region != first.Region).ToArray();
                    foreach (Sudoku_Cell other in othersInColumn) if (other.Candidates[candidateIndex]) actions.Add(new Sudoku_CandidateAction(other.Row, other.Column, candidateIndex, false));
                }

            }
        }



        return actions;
    }

    private static bool[] ORArrayBoolean(bool[] first, bool[] second)
    {
        if (first.Length != second.Length) throw new Exception("Array di dimensione differente");
        bool[] orArray = new bool[first.Length];

        for (int i = 0; i < Sudoku_Board.DIMENSION; i++) orArray[i] = first[i] | second[i];
        return orArray;
    }

    /*
     * Recursion:
     * Try every single possibility
     */
    public static List<Sudoku_Action> Recursion_Technique(Sudoku_Board board)
    {
        Sudoku_Board clone = new Sudoku_Board(board.ToString());
        List<Sudoku_Action> actions = new List<Sudoku_Action>();
        Recursion_Algorithm(clone, actions);
        return actions;
    }

    private static bool Recursion_Algorithm(Sudoku_Board board, List<Sudoku_Action> actions)
    {
        if (board.IsCompleted) return true;

        Sudoku_Cell next = board.GetFirstEmptyCell();

        List<Sudoku_ValueAction> guesses = next.GetPossibleActions();

        foreach (Sudoku_ValueAction guess in guesses)
        {
            if (board.IsCorrect)
            {
                board.ApplyAction(guess);
                actions.Add(guess);
                if (Recursion_Algorithm(board, actions)) return true;

            }
            board.DeApplyAction(guess);
            actions.Remove(guess);
        }
        return false;
    }



}