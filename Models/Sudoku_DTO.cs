using System.ComponentModel.DataAnnotations;
class Sudoku_DTO
{
    public enum Sudoku_Difficulty
    {
        Easy = 0,
        Medium = 1,
        Hard = 2,
        Expert = 3,
        Evil = 4
    }

    [Key]
    public int ID { get; set; }

    public string Mission { get; set; }

    public string Solution { get; set; }

    public Sudoku_Difficulty Difficulty { get; set; }

    public Sudoku_DTO(string mission, string solution, Sudoku_Difficulty difficulty)
    {
        this.Mission = mission;
        this.Solution = solution;
        this.Difficulty = difficulty;
    }

}