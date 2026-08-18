using AppOcorrencias.Models;

namespace AppOcorrencias.Services
{
    internal class NovaOcorrenciaSingleton
    {
        private static NovaOcorrenciaSingleton _instance;
        public static NovaOcorrenciaSingleton Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new NovaOcorrenciaSingleton();
                return _instance;
            }
        }

        private NovaOcorrenciaSingleton() { }

        public Ocorrencia OcorrenciaAtual { get; private set; } = new Ocorrencia();
        public List<Adolescente> Adolescentes { get; private set; } = new List<Adolescente>();
        public List<PessoaEnvolvida> Envolvidos { get; private set; } = new List<PessoaEnvolvida>();

        // 🔹 chamado ao entrar no Passo 1, garante que começa do zero
        public void IniciarNova()
        {
            OcorrenciaAtual = new Ocorrencia
            {
                ConselheiroId = UserSingleton.Instance.UsuarioLogado?.IdServidor ?? 0
            };
            Adolescentes = new List<Adolescente>();
            Envolvidos = new List<PessoaEnvolvida>();
        }

        // 🔹 chamado depois de salvar tudo com sucesso no Passo 6
        public void Limpar()
        {
            OcorrenciaAtual = new Ocorrencia
            {
                ConselheiroId = UserSingleton.Instance.UsuarioLogado?.IdServidor ?? 0
            };
            Adolescentes = new List<Adolescente>();
            Envolvidos = new List<PessoaEnvolvida>();
        }
    }
}