using Microsoft.EntityFrameworkCore;

class DbSudoku : DbContext
{
    public DbSet<Sudoku_DTO> Sudoku_Records { get; set; }

    public DbSudoku(DbContextOptions<DbSudoku> options) : base(options)
    {

    }

}