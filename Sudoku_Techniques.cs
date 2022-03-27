abstract class Sudoku_Techniques
{
    //https://www.sudokuoftheday.com/techniques
    /*
     * Single Candidate = Ok
     * Single Position  = Ok 
     * Candidate Lines  = Ok
     * Double Pairs     = X
     * Multiple Lines   = X
     * Naked Pair       = Ok
     * Hidden Pair      = 
     * Naked Triple     = Ok
     * Hidden Triple   = 
     * X-Wing           = 
     * Forcing Chains   = 
     * Naked Quad       = Ok
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
        
        IEnumerable<Sudoku_Cell> emptyCells = line.Where(cell =>  cell.Value == 0);

        
        foreach (Sudoku_Cell cell in emptyCells)
        {
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
     * Candidate lines:
     * Se in una regione le uniche celle che presentano un certo candidato si trovano sulla stessa riga (o colonna), si può rimuovere quel candidato da tutto il resto della lnea
     */
    public static List<Sudoku_Action> CandidateLines_Technique(Sudoku_Board board)
    {
        List<Sudoku_Action> actions = new List<Sudoku_Action>();

        for (int regionIndex = 0; regionIndex < Sudoku_Board.DIMENSION; regionIndex++)
        {
            IEnumerable<Sudoku_Cell> emptyCells = board.GetRegion(regionIndex).Where(cell => cell.Value != 0);
            if (!emptyCells.Any()) continue;

            bool[] tryCandidates = new bool[Sudoku_Board.DIMENSION] { false, false, false, false, false, false, false, false, false };

            foreach (Sudoku_Cell cell in emptyCells) tryCandidates = ORArrayBoolean(tryCandidates, cell.Candidates);

            for (int candidateIndex = 0; candidateIndex < Sudoku_Board.DIMENSION; candidateIndex++)
            {
                if (!tryCandidates[candidateIndex]) continue;
                
                IEnumerable<Sudoku_Cell> cellsWithCandidate = emptyCells.Where(cell => cell.Candidates[candidateIndex]);
                Sudoku_Cell first = cellsWithCandidate.First();
                bool sameRow = cellsWithCandidate.All(cell => first.Row == cell.Row);
                bool sameColumn = cellsWithCandidate.All(cell => first.Column == cell.Column);

                if(sameRow) actions.AddRange(board.GetRow(first.Row).Where(cell => cell.Region != first.Region && cell.Candidates[candidateIndex]).Select(cell => new Sudoku_CandidateAction(cell.Row, cell.Column, candidateIndex, false)));

                if(sameColumn) actions.AddRange(board.GetColumn(first.Row).Where(cell => cell.Region != first.Region && cell.Candidates[candidateIndex]).Select(cell => new Sudoku_CandidateAction(cell.Row, cell.Column, candidateIndex, false)));

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
     * Naked_Technique:
     * Se in una linea (regione, riga o colonna) sono presenti N celle con N candidati uguali, è possibile rimuovere tali candidati dalle altre celle della linea.
     */
    public static List<Sudoku_Action> Naked_Technique(Sudoku_Board board, int nakedGoal) 
    {
        List<Sudoku_Action> actions = new List<Sudoku_Action>();
        
        for(int i = 0; i < Sudoku_Board.DIMENSION; i++) 
        {
            actions.AddRange(Naked_Scan(board.GetColumn(i), nakedGoal));
            actions.AddRange(Naked_Scan(board.GetRow(i), nakedGoal));
            actions.AddRange(Naked_Scan(board.GetRegion(i), nakedGoal));
        }
        
        
        return actions;
    }
 
    private static List<Sudoku_Action> Naked_Scan(Sudoku_Cell[] line, int goal) 
    {
        List<Sudoku_Action> actions = new List<Sudoku_Action>();

        IEnumerable<Sudoku_Cell> possibleCells = line.Where(cell => cell.Value == 0 && cell.CandidatesString.Length == goal);
        if(!possibleCells.Any()) return actions;

        IEnumerable<Sudoku_Cell> distincts = possibleCells.DistinctBy(cell => cell.CandidatesString);

        foreach(Sudoku_Cell distinct in distincts) {
            string distinctCandidateString = distinct.CandidatesString;
            int nEquals = possibleCells.Count(cell => cell.CandidatesString == distinctCandidateString);
            if(nEquals == goal) 
            {
                IEnumerable<int> indexesToRemove = distinctCandidateString.Select(c => int.Parse(c.ToString()) - 1);
                IEnumerable<Sudoku_Cell> others = possibleCells.Where(cell => cell.CandidatesString != distinct.CandidatesString);
                foreach(Sudoku_Cell other in others) 
                {
                    foreach(int candidateIndex in indexesToRemove) 
                    {
                        actions.Add(new Sudoku_CandidateAction(other.Row, other.Column, candidateIndex, false));
                    }
                }
                
            }
        }

        return actions;
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