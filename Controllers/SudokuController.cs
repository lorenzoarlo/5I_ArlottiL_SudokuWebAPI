using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;

namespace _5I_ArlottiL_SudokuWebAPI.Controllers;

[EnableCors("MyPolicy")]
[ApiController]
[Route("api/[controller]")]
public class SudokuController : ControllerBase
{
    private readonly DbSudoku _context;

    public SudokuController(DbSudoku context)
    {
        _context = context;
    }

    
    [HttpGet("{difficulty}")]
    public async Task<ActionResult<Sudoku_DTO>> GetSudoku(string difficulty)
    {
        bool correct = Enum.TryParse(difficulty, true, out Sudoku_DTO.Sudoku_Difficulty difficultySelected);
        if (!correct) return BadRequest("Difficoltà non riconosciuta!");

        if(difficultySelected == Sudoku_DTO.Sudoku_Difficulty.Casual) {
            Random rnd = new Random();
            int rightLimit = (int) Sudoku_DTO.Sudoku_Difficulty.Evil + 1;
            difficultySelected = (Sudoku_DTO.Sudoku_Difficulty) rnd.Next(rightLimit);
        }
        List<Sudoku_DTO> records = await _context.Sudoku_Records.Where(x => x.Difficulty == difficultySelected).ToListAsync<Sudoku_DTO>();

        if (records.Count() == 0) return NotFound();

        return records.OrderBy(o => Guid.NewGuid()).First(); ;
    }

}
