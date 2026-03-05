namespace OkrTracker.Application.DTOs
{
    /// <summary>
    /// Representa o resultado de uma operação de aplicação.
    /// </summary>
    public class ResultadoOperacao
    {
        /// <summary>
        /// Indica se a operação foi bem-sucedida.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mensagem descritiva do resultado (usada principalmente em erros).
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Cria um resultado de sucesso.
        /// </summary>
        public static ResultadoOperacao Sucesso() => new() { Success = true };

        /// <summary>
        /// Cria um resultado de erro com mensagem.
        /// </summary>
        public static ResultadoOperacao Erro(string mensagem) => new() { Success = false, Message = mensagem };
    }

    /// <summary>
    /// Resultado de operação com dados de retorno.
    /// </summary>
    public class ResultadoOperacao<T> : ResultadoOperacao
    {
        /// <summary>
        /// Dados retornados pela operação.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Cria um resultado de sucesso com dados.
        /// </summary>
        public static ResultadoOperacao<T> Sucesso(T data) => new() { Success = true, Data = data };

        /// <summary>
        /// Cria um resultado de erro com mensagem.
        /// </summary>
        public new static ResultadoOperacao<T> Erro(string mensagem) => new() { Success = false, Message = mensagem };
    }
}
