using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Cors;

namespace _5I_ArlottiL_SudokuWebAPI.Controllers;

[ApiController]
[EnableCors("MyPolicy")]
[Route("api/[controller]")]
public class SudokuHelperController : ControllerBase
{
    [HttpGet("{sudokuMission}")]
    public ActionResult<Sudoku_HelperDTO> NextMove(string sudokuMission)
    {
        Sudoku_Board board = new Sudoku_Board(sudokuMission);
        
        List<Sudoku_Action> elencoAzioni = Sudoku_Techniques.Recursion_Technique(board).ToList();
        List<Sudoku_ActionDTO> elencoDTO = new List<Sudoku_ActionDTO>();
        foreach(Sudoku_Action action in elencoAzioni) elencoDTO.Add(action.GetDTO());

        Sudoku_HelperDTO helperDTO = new Sudoku_HelperDTO() {
            CandidateString = board.CandidatesString(),
            Actions = elencoDTO
        };
        return helperDTO;
        
    }

}
