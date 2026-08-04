using System;
using SQLite;

namespace AppOcorrencias.Models
{
    public class Ocorrencia
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Guid local, usado pra vincular envolvidos e evitar colisão de Id ao sincronizar
        public string Uid { get; set; } = Guid.NewGuid().ToString();

        public string Tipo { get; set; }
        public DateTime DataHora { get; set; } = DateTime.Now;
        public string Local { get; set; }
        public string Descricao { get; set; }

        // CPF de quem registrou (vem do UserSingleton)
        public string RegistradoPorCPF { get; set; }

        public bool Sincronizado { get; set; } = false;
        public DateTime CriadoEm { get; set; } = DateTime.Now;
    }
}