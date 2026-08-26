using AppOcorrencias.Models;

namespace AppOcorrencias.Services
{
    internal class UserSingleton
    {
        private static UserSingleton _instance;
        public static UserSingleton Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UserSingleton();
                return _instance;
            }
        }

        private UserSingleton()
        {
            CarregarSessaoSalva();
        }

        public Usuario UsuarioLogado { get; private set; }
        public bool IsLogado => UsuarioLogado != null;

        // 🔹 Definir usuário (agora também persiste localmente)
        public void SetUsuario(Usuario usuario)
        {
            UsuarioLogado = usuario;

            Preferences.Default.Set("user_cpf", usuario.CPF ?? string.Empty);
            Preferences.Default.Set("user_nome", usuario.Nome ?? string.Empty);
            Preferences.Default.Set("user_idServidor", usuario.IdServidor);
            Preferences.Default.Set("user_token", usuario.Token ?? string.Empty);
        }

        // 🔹 Limpar sessão (logout) — agora também limpa o que foi persistido
        public void Logout()
        {
            UsuarioLogado = null;

            Preferences.Default.Remove("user_cpf");
            Preferences.Default.Remove("user_nome");
            Preferences.Default.Remove("user_idServidor");
            Preferences.Default.Remove("user_token");
        }

        // 🔹 restaura a sessão salva ao abrir o app (se o token ainda estiver salvo)
        void CarregarSessaoSalva()
        {
            var token = Preferences.Default.Get("user_token", string.Empty);
            if (string.IsNullOrWhiteSpace(token)) return;

            UsuarioLogado = new Usuario
            {
                CPF = Preferences.Default.Get("user_cpf", string.Empty),
                Nome = Preferences.Default.Get("user_nome", string.Empty),
                IdServidor = Preferences.Default.Get("user_idServidor", 0),
                Token = token
            };
        }
    }
}