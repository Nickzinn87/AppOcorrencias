using SQLite;

namespace AppOcorrencias.Models
{
    public class Usuario
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } // 🔹 Id local do SQLite (não usado hoje, já que login não passa por aqui)

        public int IdServidor { get; set; } // 🔹 Id do usuário no banco da web — é o que vai como "conselheiro" na ocorrência

        public string CPF { get; set; }
        public string Senha { get; set; }
        public string Nome { get; set; }
    }
}