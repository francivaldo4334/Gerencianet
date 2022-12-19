namespace Gerencianet.Dtos{
    public class GerencianetPagarComCartao{
        public string NomeProduto { get; set; }
        public int ValorProduto { get; set; }
        public int QuantidadeParcelas {get;set;}
        public string TokenCartao { get; set; }
        public string NomeClient { get;set;}
        public string EmailClient { get; set; }
        public string CpfClient { get; set; }
        public string TelefoneClient { get; set;}
    }
}