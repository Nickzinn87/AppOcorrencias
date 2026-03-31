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

        private UserSingleton() { }

        // 🔐 Usuário logado
        public Usuario UsuarioLogado { get; private set; }

        public bool IsLogado => UsuarioLogado != null;

        // 🔹 Definir usuário
        public void SetUsuario(Usuario usuario)
        {
            UsuarioLogado = usuario;
        }

        // 🔹 Limpar sessão (logout)
        public void Logout()
        {
            UsuarioLogado = null;
        }
    }
}