// Models/Assinatura.cs
using SQLite;

namespace AppOcorrencias.Models;

public class Assinatura
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int PessoaEnvolvidaId { get; set; }

    // PNG em base64 (funciona 100% offline)
    public string ImagemBase64 { get; set; } = string.Empty;

    public string Hash { get; set; } = string.Empty; // SHA-256 pra integridade
    public DateTime CapturadoEm { get; set; } = DateTime.Now;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public bool Sincronizado { get; set; } = false;
}