using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SQLite;
using AppOcorrencias.Models;

namespace AppOcorrencias.Services
{
    public class WebDBService
    {
        private readonly SQLiteAsyncConnection _database;
        private bool _initialized = false;
        private readonly SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);

        public WebDBService()
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db3");
            _database = new SQLiteAsyncConnection(dbPath);
        }

        private async Task InitAsync()
        {
            if (_initialized) return;

            await _initLock.WaitAsync();
            try
            {
                if (_initialized) return;

                await _database.CreateTableAsync<Usuario>();
                await _database.CreateTableAsync<Ocorrencia>();
                await _database.CreateTableAsync<PessoaEnvolvida>();
                await _database.CreateTableAsync<Assinatura>();
                await _database.CreateTableAsync<Adolescente>(); // 🔹 nova tabela

                _initialized = true;
            }
            finally
            {
                _initLock.Release();
            }
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
            await InitAsync();
            return await _database.Table<Usuario>()
                                  .Where(u => u.CPF == cpf)
                                  .FirstOrDefaultAsync();
        }

        // 🔹 Login
        public async Task<Usuario> LoginAsync(string cpf, string senha)
        {
            await InitAsync();
            return await _database.Table<Usuario>()
                                  .Where(u => u.CPF == cpf && u.Senha == senha)
                                  .FirstOrDefaultAsync();
        }

        // 🔹 Listar todos
        public async Task<List<Usuario>> GetUsuariosAsync()
        {
            await InitAsync();
            return await _database.Table<Usuario>().ToListAsync();
        }

        // 🔹 Deletar
        public async Task<int> DeletarUsuarioAsync(Usuario usuario)
        {
            await InitAsync();
            return await _database.DeleteAsync(usuario);
        }

        // ================== OCORRÊNCIAS ==================

        public async Task<int> SalvarOcorrenciaAsync(Ocorrencia ocorrencia)
        {
            await InitAsync();
            if (ocorrencia.Id == 0)
                return await _database.InsertAsync(ocorrencia);

            return await _database.UpdateAsync(ocorrencia);
        }

        public async Task<List<Ocorrencia>> GetOcorrenciasAsync()
        {
            await InitAsync();
            return await _database.Table<Ocorrencia>().ToListAsync();
        }

        public async Task<Ocorrencia> GetOcorrenciaPorUidAsync(string uid)
        {
            await InitAsync();
            return await _database.Table<Ocorrencia>()
                                  .Where(o => o.Uid == uid)
                                  .FirstOrDefaultAsync();
        }

        // ================== ENVOLVIDOS ==================

        public async Task<int> SalvarEnvolvidoAsync(PessoaEnvolvida pessoa)
        {
            await InitAsync();
            if (pessoa.Id == 0)
                return await _database.InsertAsync(pessoa);

            return await _database.UpdateAsync(pessoa);
        }

        public async Task<List<PessoaEnvolvida>> GetEnvolvidosAsync(string ocorrenciaUid)
        {
            await InitAsync();
            return await _database.Table<PessoaEnvolvida>()
                                  .Where(p => p.OcorrenciaUid == ocorrenciaUid)
                                  .ToListAsync();
        }

        // ================== ASSINATURAS ==================

        public async Task<int> SalvarAssinaturaAsync(Assinatura assinatura)
        {
            await InitAsync();
            if (assinatura.Id == 0)
                return await _database.InsertAsync(assinatura);

            return await _database.UpdateAsync(assinatura);
        }

        public async Task<Assinatura> GetAssinaturaPorEnvolvidoAsync(int pessoaEnvolvidaId)
        {
            await InitAsync();
            return await _database.Table<Assinatura>()
                                  .Where(a => a.PessoaEnvolvidaId == pessoaEnvolvidaId)
                                  .FirstOrDefaultAsync();
        }

        // ================== ADOLESCENTES ==================

        public async Task<int> SalvarAdolescenteAsync(Adolescente adolescente)
        {
            await InitAsync();
            if (adolescente.Id == 0)
                return await _database.InsertAsync(adolescente);

            return await _database.UpdateAsync(adolescente);
        }

        public async Task<List<Adolescente>> GetAdolescentesAsync(string ocorrenciaUid)
        {
            await InitAsync();
            return await _database.Table<Adolescente>()
                                  .Where(a => a.OcorrenciaUid == ocorrenciaUid)
                                  .ToListAsync();
        }
    }
}