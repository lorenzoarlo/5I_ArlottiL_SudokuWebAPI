public abstract class Sudoku_Techniques
{
    //https://www.sudokuoftheday.com/techniques
    //https://www.sudoku9981.com/sudoku-solving/single.php
    /*
     * -> Subset = Riga, Colonna o Regione
     *
     * Single Candidate = Ok 
     * Se una cella ha un solo candidato possibile, il valore della cella sarà quel candidato
     * Single Position  = Ok
     * Se in un subset, un candidato è presente in una sola cella, il valore della cella sarà quel candidato
     * Candidate Lines  = Ok
     * Se in una regione le uniche celle che presentano un certo candidato si trovano sulla stessa riga (o colonna), si può rimuovere quel candidato da tutto il resto della linea
     * Double Pairs     = X
     * -
     * Multiple Lines   = X
     * -
     * Naked Technique  = Ok (Naked Pair, Triple, Quad)
     * Se in un subset sono presenti N celle con N candidati uguali, è possibile rimuovere tali candidati dalle altre celle della linea
     * Hidden Technique =  (Hidden Pair, Triple, Quad) 
     * 
     * X-Wing           = 
     * Forcing Chains   =  
     * Swordfish        = 
     * Nishio           = X
     * Guessing         = X
     * Recursion        = Ok
     */

    public static List<Sudoku_Action> SingleCandidate_Technique(Sudoku_Board board)
    {
        List<Sudoku_Action> actions = new List<Sudoku_Action>();
        
        IEnumerable<Sudoku_Cell> singleCandidateCells = board.EmptyCells.Where(cell => cell.CandidatesString.Length == 1);

        foreach(Sudoku_Cell cell in singleCandidateCells) actions.Add(new Sudoku_ValueAction(cell.Row, cell.Column, int.Parse(cell.CandidatesString)));
        
        return actions;
    }

    public static IEnumerable<Sudoku_CandidateAction> GetAllCandidatesActions(Sudoku_Board board)   
    {
        List<Sudoku_CandidateAction> actions = new List<Sudoku_CandidateAction>();
        
        foreach(Sudoku_Cell cell in board.EmptyCells) actions.AddRange(cell.Candidates.Select((value, index) => new Sudoku_CandidateAction(cell.Row, cell.Column, ((byte)index), value)));

        return actions;
    } 

    public static IEnumerable<Sudoku_Action> SinglePosition_Technique(Sudoku_Board board)
    {
        List<Sudoku_Action> actions = new List<Sudoku_Action>();

        for (int i = 0; i < Sudoku_Board.BOARD_DIMENSION; i++)
        {
            actions.AddRange(SinglePosition_Algorithm(board.GetRow(i)));
            actions.AddRange(SinglePosition_Algorithm(board.GetColumn(i)));
            actions.AddRange(SinglePosition_Algorithm(board.GetRegion(i)));
        }


        return actions;
    }

    private static List<Sudoku_Action> SinglePosition_Algorithm(IEnumerable<Sudoku_Cell> line)
    {
        List<Sudoku_Action> actions = new List<Sudoku_Action>();        
        IEnumerable<Sudoku_Cell> emptyCells = line.Where(cell => cell.Value == 0);        

        for(int candidateIndex = 0; candidateIndex < Sudoku_Board.BOARD_DIMENSION; candidateIndex++) 
        {
            IEnumerable<Sudoku_Cell> cellsWithCandidate = emptyCells.Where(cell => cell.Candidates[candidateIndex]);
            if(cellsWithCandidate.Count() == 1) {
                Sudoku_Cell tmp = cellsWithCandidate.First();
                actions.Add(new Sudoku_ValueAction(tmp.Row, tmp.Column, candidateIndex + 1));
            }
        }

        return actions;
    }

    public static List<Sudoku_Action> CandidateLines_Technique(Sudoku_Board board)
    {
        List<Sudoku_Action> actions = new List<Sudoku_Action>();

        for (int regionIndex = 0; regionIndex < Sudoku_Board.BOARD_DIMENSION; regionIndex++)
        {
            IEnumerable<Sudoku_Cell> emptyCells = board.GetRegion(regionIndex).Where(cell => cell.Value != 0);
            if (!emptyCells.Any()) continue;

            bool[] tryCandidates = new bool[Sudoku_Board.BOARD_DIMENSION] { false, false, false, false, false, false, false, false, false };

            foreach (Sudoku_Cell cell in emptyCells) tryCandidates = ORArrayBoolean(tryCandidates, cell.Candidates);

            for (int candidateIndex = 0; candidateIndex < Sudoku_Board.BOARD_DIMENSION; candidateIndex++)
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

        for (int i = 0; i < Sudoku_Board.BOARD_DIMENSION; i++) orArray[i] = first[i] | second[i];
        return orArray;
    }

    public static List<Sudoku_Action> Naked_Technique(Sudoku_Board board, int nakedGoal) 
    {
        List<Sudoku_Action> actions = new List<Sudoku_Action>();
        
        for(int i = 0; i < Sudoku_Board.BOARD_DIMENSION; i++) 
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