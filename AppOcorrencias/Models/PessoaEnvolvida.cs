// Models/PessoaEnvolvida.cs
using SQLite;

namespace AppOcorrencias.Models;

public class PessoaEnvolvida
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public string OcorrenciaUid { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Papel { get; set; } = string.Empty; // Vítima, Testemunha, Policial, etc.

    public bool Assinado { get; set; } = false;
}