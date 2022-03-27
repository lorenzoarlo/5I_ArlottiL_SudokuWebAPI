using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _5I_ArlottiL_SudokuWebAPI.Controllers;

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

        List<Sudoku_DTO> records = await _context.Sudoku_Records.Where(x => x.Difficulty == difficultySelected).ToListAsync<Sudoku_DTO>();
        int nRecords = records.Count();

        if (nRecords == 0) return NotFound();

        return records.OrderBy(o => Guid.NewGuid()).First(); ;
    }

}
