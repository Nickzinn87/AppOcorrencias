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
        private bool _initialized = false;

        public WebDBService()
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db3");
            _database = new SQLiteAsyncConnection(dbPath);
        }

        private async Task InitAsync()
        {
            if (_initialized) return;
            await _database.CreateTableAsync<Usuario>();
            await _database.CreateTableAsync<Ocorrencia>();
            await _database.CreateTableAsync<PessoaEnvolvida>();
            await _database.CreateTableAsync<Assinatura>();
            _initialized = true;
        }

        // 🔹 Inserir usuário
        public async Task<int> InserirUsuarioAsync(Usuario usuario)
        {
            await InitAsync();
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

        // ================== OCORRÊNCIAS ==================

        // 🔹 Salvar ocorrência (insere se Id == 0, senão atualiza)
        public async Task<int> SalvarOcorrenciaAsync(Ocorrencia ocorrencia)
        {
            if (ocorrencia.Id == 0)
                return await _database.InsertAsync(ocorrencia);

            return await _database.UpdateAsync(ocorrencia);
        }

        // 🔹 Listar ocorrências
        public async Task<List<Ocorrencia>> GetOcorrenciasAsync()
        {
            return await _database.Table<Ocorrencia>().ToListAsync();
        }

        // 🔹 Buscar ocorrência por Uid
        public async Task<Ocorrencia> GetOcorrenciaPorUidAsync(string uid)
        {
            return await _database.Table<Ocorrencia>()
                                  .Where(o => o.Uid == uid)
                                  .FirstOrDefaultAsync();
        }

        // ================== ENVOLVIDOS ==================

        // 🔹 Salvar envolvido (insere se Id == 0, senão atualiza)
        public async Task<int> SalvarEnvolvidoAsync(PessoaEnvolvida pessoa)
        {
            if (pessoa.Id == 0)
                return await _database.InsertAsync(pessoa);

            return await _database.UpdateAsync(pessoa);
        }

        // 🔹 Listar envolvidos de uma ocorrência
        public async Task<List<PessoaEnvolvida>> GetEnvolvidosAsync(string ocorrenciaUid)
        {
            return await _database.Table<PessoaEnvolvida>()
                                  .Where(p => p.OcorrenciaUid == ocorrenciaUid)
                                  .ToListAsync();
        }

        // ================== ASSINATURAS ==================

        // 🔹 Salvar assinatura (insere se Id == 0, senão atualiza)
        public async Task<int> SalvarAssinaturaAsync(Assinatura assinatura)
        {
            if (assinatura.Id == 0)
                return await _database.InsertAsync(assinatura);

            return await _database.UpdateAsync(assinatura);
        }

        // 🔹 Buscar assinatura de um envolvido
        public async Task<Assinatura> GetAssinaturaPorEnvolvidoAsync(int pessoaEnvolvidaId)
        {
            return await _database.Table<Assinatura>()
                                  .Where(a => a.PessoaEnvolvidaId == pessoaEnvolvidaId)
                                  .FirstOrDefaultAsync();
        }
    }
}