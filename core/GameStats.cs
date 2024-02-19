namespace SaturnService;

public class GameStats
{
    public string Statidcode { get; set; }
    public string Gamecode { get; set; }
    public int Teamcode { get; set; }
    public DateTime Gamedate { get; set; }
    public int Rushyds { get; set; }
    public int Rushatt { get; set; }
    public int Passyds { get; set; }
    public int Passatt { get; set; }
    public int Passcomp { get; set; }
    public int Penalties { get; set; }
    public int Penaltyyds { get; set; }
    public int Fumbleslost { get; set; }
    public int Interceptionsthrown { get; set; }
    public int Firstdowns { get; set; }
    public int Thriddownatt { get; set; }
    public int Thirddownconver { get; set; }
    public int Fourthdownatt { get; set; }
    public int Fourthdownconver { get; set; }
    public int Timeposs { get; set; }
    public int Score { get; set; }
}

public class TeamSeasonStats
{
    public List<GameStats> VisStats { get; set; }
    public List<GameStats> HomeStats { get; set; }
}
