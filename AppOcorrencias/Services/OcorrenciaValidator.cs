using AppOcorrencias.Models;

namespace AppOcorrencias.Services
{
    public static class OcorrenciaValidator
    {
        // Se true, rejeita CPF com digito verificador invalido (ex: 111.111.111-11).
        // Deixe false em testes/apresentacoes com dados ficticios.
        const bool ValidarDigitosCpf = false;

        static string Digitos(string s) => new string((s ?? string.Empty).Where(char.IsDigit).ToArray());
        static bool Vazio(string s) => string.IsNullOrWhiteSpace(s);

        // Mostra os erros num alerta. Retorna true se havia erro (para dar return no Command).
        public static async Task<bool> BloquearSeInvalidoAsync(List<string> erros, string titulo = "Corrija os campos")
        {
            if (erros.Count == 0) return false;
            await Shell.Current.DisplayAlert(titulo, "• " + string.Join("\n• ", erros), "OK");
            return true;
        }

        public static List<string> ValidarPasso1(Ocorrencia o)
        {
            var erros = new List<string>();

            if (Vazio(o.Numero)) erros.Add("Informe o N° do registro.");
            else if (Digitos(o.Numero) != o.Numero.Trim() || o.Numero.Trim().Length > 9)
                erros.Add("O N° do registro deve conter apenas números (até 9 dígitos).");

            if (Vazio(o.Natureza)) erros.Add("Informe a natureza da ocorrência.");
            if (Vazio(o.Local)) erros.Add("Informe o local.");
            if (Vazio(o.Hora)) erros.Add("Informe a hora.");
            if (Vazio(o.Solicitante)) erros.Add("Informe o solicitante.");
            if (o.Data.Date > DateTime.Today) erros.Add("A data da ocorrência não pode ser futura.");

            return erros;
        }

        public static List<string> ValidarAdolescentes(IEnumerable<Adolescente> lista)
        {
            var erros = new List<string>();
            var itens = lista.ToList();

            if (itens.Count == 0)
            {
                erros.Add("Adicione ao menos um adolescente.");
                return erros;
            }

            foreach (var a in itens)
            {
                if (Vazio(a.Nome)) erros.Add("Há um adolescente sem nome.");
                if (a.DataNascimento.Date >= DateTime.Today)
                    erros.Add($"Informe a data de nascimento de {a.Nome}.");
            }

            return erros;
        }

        public static List<string> ValidarPasso3(Ocorrencia o)
        {
            var erros = new List<string>();

            if (Vazio(o.MaeNome) && Vazio(o.PaiNome) && Vazio(o.ResponsavelNome))
                erros.Add("Preencha ao menos um: Mãe, Pai ou Responsável.");

            ValidarCpf(o.MaeCPF, "da Mãe", erros);
            ValidarCpf(o.PaiCPF, "do Pai", erros);

            return erros;
        }

        public static List<string> ValidarPasso4(Ocorrencia o)
        {
            var erros = new List<string>();

            if (Vazio(o.EnderecoResidencial)) erros.Add("Informe o endereço residencial.");
            if (Vazio(o.BairroResidencial)) erros.Add("Informe o bairro residencial.");

            ValidarFone(o.FoneResidencial, "residencial", erros);
            ValidarFone(o.FoneComercial, "comercial", erros);

            return erros;
        }

        public static List<string> ValidarPasso5(Ocorrencia o, int assinaturasColetadas)
        {
            var erros = new List<string>();

            if (Vazio(o.Historico)) erros.Add("Preencha o histórico da ocorrência.");
            if (assinaturasColetadas < 3)
                erros.Add($"Faltam assinaturas ({assinaturasColetadas} de 3 coletadas).");

            return erros;
        }

        // Validacao final, usada ao concluir e antes de cada envio na sincronizacao
        public static List<string> ValidarCompleta(Ocorrencia o, IEnumerable<Adolescente> adolescentes, int assinaturasColetadas)
        {
            var erros = new List<string>();

            erros.AddRange(ValidarPasso1(o));
            erros.AddRange(ValidarAdolescentes(adolescentes));
            erros.AddRange(ValidarPasso3(o));
            erros.AddRange(ValidarPasso4(o));
            erros.AddRange(ValidarPasso5(o, assinaturasColetadas));

            if (o.ConselheiroId <= 0)
                erros.Add("Conselheiro não identificado. Faça login novamente.");

            return erros.Distinct().ToList();
        }

        static void ValidarCpf(string cpf, string quem, List<string> erros)
        {
            if (Vazio(cpf)) return;

            var d = Digitos(cpf);
            if (d.Length != 11)
            {
                erros.Add($"O CPF {quem} deve ter 11 dígitos.");
                return;
            }

            if (ValidarDigitosCpf && !CpfValido(d))
                erros.Add($"O CPF {quem} é inválido.");
        }

        static void ValidarFone(string fone, string tipo, List<string> erros)
        {
            if (Vazio(fone)) return;

            var n = Digitos(fone).Length;
            if (n < 8 || n > 11)
                erros.Add($"Telefone {tipo} inválido (use de 8 a 11 dígitos).");
        }

        static bool CpfValido(string d)
        {
            if (d.Distinct().Count() == 1) return false;

            int Calc(int len)
            {
                int soma = 0;
                for (int i = 0; i < len; i++)
                    soma += (d[i] - '0') * (len + 1 - i);

                int r = soma * 10 % 11;
                return r == 10 ? 0 : r;
            }

            return Calc(9) == d[9] - '0' && Calc(10) == d[10] - '0';
        }
    }
}