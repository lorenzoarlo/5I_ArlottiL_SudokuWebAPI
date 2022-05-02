using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Cors;
using System.Reflection;

namespace _5I_ArlottiL_SudokuWebAPI.Controllers;

[ApiController]
[EnableCors("MyPolicy")]
[Route("api/[controller]")]
public class SudokuHelperController : ControllerBase
{
    static Func<Sudoku_Board, IEnumerable<Sudoku_Action>>[] TechniquesList = {
        Sudoku_Techniques.SingleCandidate_Technique,
        Sudoku_Techniques.SinglePosition_Technique,
        Sudoku_Techniques.CandidateLines_Technique,
        // Sudoku_Techniques.NakedPair_Technique,
        // Sudoku_Techniques.NakedTriple_Technique,
        // Sudoku_Techniques.NakedQuad_Technique,
        Sudoku_Techniques.Recursion_Technique
    };


    
    [HttpGet("{sudokuMission}")]
    public ActionResult<Sudoku_HelperDTO> NextMove(string sudokuMission)
    {
        Sudoku_Board board = new Sudoku_Board(sudokuMission);
        
        List<Sudoku_Action> elencoAzioni = new List<Sudoku_Action>();
        string technique_name = "null";
        foreach(Func<Sudoku_Board, IEnumerable<Sudoku_Action>> technique in TechniquesList){
            elencoAzioni.AddRange(technique.Invoke(board));
            technique_name = technique.Method.Name;
            if(elencoAzioni.Any()) break;
        }
        
        List<Sudoku_ActionDTO> elencoDTO = new List<Sudoku_ActionDTO>();
        foreach(Sudoku_Action action in elencoAzioni) elencoDTO.Add(action.GetDTO());

        Sudoku_HelperDTO helperDTO = new Sudoku_HelperDTO() {
            CandidateString = board.CandidatesString(),
            Technique = technique_name,
            Actions = elencoDTO
        };
        return helperDTO;
        
    }

}
