using SQLite;

namespace AppOcorrencias.Models
{
    public class Ocorrencia
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Uid { get; set; } = Guid.NewGuid().ToString();

        // Passo 1 — Dados da Ocorrência
        public string Numero { get; set; }
        public string Natureza { get; set; }
        public string Local { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public string Hora { get; set; }
        public string Solicitante { get; set; }

        // Passo 3 — Mãe
        public string MaeNome { get; set; }
        public string MaeRG { get; set; }
        public string MaeCPF { get; set; }
        public string MaeProfissao { get; set; }

        // Passo 3 — Pai
        public string PaiNome { get; set; }
        public string PaiRG { get; set; }
        public string PaiCPF { get; set; }
        public string PaiProfissao { get; set; }

        // Passo 4 — Responsável
        public string ResponsavelNome { get; set; }
        public string ResponsavelRG { get; set; }
        public string ResponsavelProfissao { get; set; }

        // Passo 5 — Endereço Residencial
        public string EnderecoResidencial { get; set; }
        public string EnderecoResidencialNumero { get; set; }
        public string BairroResidencial { get; set; }
        public string ComplementoResidencial { get; set; }
        public string FoneResidencial { get; set; }

        // Passo 5 — Endereço Comercial
        public string EnderecoComercial { get; set; }
        public string EnderecoComercialNumero { get; set; }
        public string BairroComercial { get; set; }
        public string ComplementoComercial { get; set; }
        public string FoneComercial { get; set; }

        // Passo 6 — Histórico
        public string Historico { get; set; }

        // era: public string RegistradoPorCPF { get; set; }
        public int ConselheiroId { get; set; } // 🔹 Id do usuário logado no servidor (conselheiro)
        public bool Sincronizado { get; set; } = false;
        public DateTime CriadoEm { get; set; } = DateTime.Now;
    }
}