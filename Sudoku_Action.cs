using System.Text.Json.Nodes;
abstract class Sudoku_Action
{
    abstract public string ActionType { get; }
    public int Row { get; }
    public int Column { get; }

    public Sudoku_Action(int row, int column)
    {
        this.Row = row;
        this.Column = column;
    }

    public abstract JsonObject GetDTO();

}