using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using AppOcorrencias.Models;

namespace AppOcorrencias.Services
{
    public class WebDBService
    {
        private readonly SQLiteAsyncConnection _database;

        public WebDBService()
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db3");
            _database = new SQLiteAsyncConnection(dbPath);

            _database.CreateTableAsync<Usuario>().Wait();
        }

        // 🔹 Inserir usuário
        public async Task<int> InserirUsuarioAsync(Usuario usuario)
        {
            return await _database.InsertAsync(usuario);
        }

        // 🔹 Buscar usuário por CPF
        public async Task<Usuario> GetUsuarioPorCPFAsync(string cpf)
        {
            return await _database.Table<Usuario>()
                                  .Where(u => u.CPF == cpf)
                                  .FirstOrDefaultAsync();
        }

        // 🔹 Login
        public async Task<Usuario> LoginAsync(string cpf, string senha)
        {
            return await _database.Table<Usuario>()
                                  .Where(u => u.CPF == cpf && u.Senha == senha)
                                  .FirstOrDefaultAsync();
        }

        // 🔹 Listar todos
        public async Task<List<Usuario>> GetUsuariosAsync()
        {
            return await _database.Table<Usuario>().ToListAsync();
        }

        // 🔹 Deletar
        public async Task<int> DeletarUsuarioAsync(Usuario usuario)
        {
            return await _database.DeleteAsync(usuario);
        }
    }
}
