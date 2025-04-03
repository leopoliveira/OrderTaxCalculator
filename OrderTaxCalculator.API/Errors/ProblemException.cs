namespace OrderTaxCalculator.API.Errors;

[Serializable]
public class ProblemException : Exception
{
    public ProblemException(string erro, string mensagem) : base(mensagem)
    {
        Erro = erro;
        Mensagem = mensagem;
    }
    
    public string Erro { get; set; }

    public string Mensagem { get; set; }
}