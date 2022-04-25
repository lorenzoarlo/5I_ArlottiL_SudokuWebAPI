using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Cors;

namespace _5I_ArlottiL_SudokuWebAPI.Controllers;

[EnableCors("MyPolicy")]
[ApiController]
[Route("api/[controller]")]
public class SudokuHelperController : ControllerBase
{
    // [HttpGet("{sudokuMission}")]
    // public JsonArray GetSolution(string sudokuMission)
    // {
        // Sudoku_Board board = new Sudoku_Board(sudokuMission);
        // return new JsonArray(Sudoku_Techniques.Recursion_Technique(board).Select(x => x.GetDTO()).ToArray());
    // }

}
