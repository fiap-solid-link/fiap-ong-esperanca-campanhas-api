namespace Esperanca.Campanha.Domain.Campanhas;

public static class CampanhaErros
{
    public const string TituloObrigatorio                    = "Campanha:101";
    public const string DescricaoObrigatoria                 = "Campanha:102";
    public const string DataFimMenorQueAgora                 = "Campanha:103";
    public const string DataInicioMaiorQueDataFim            = "Campanha:104";
    public const string MetaFinanceiraInvalida               = "Campanha:105";
    public const string EdicaoSomenteEmCadastrada            = "Campanha:201";
    public const string AtivacaoSomenteEmCadastrada          = "Campanha:202";
    public const string ProrrogacaoSomenteEmAndamento        = "Campanha:203";
    public const string ProrrogacaoExigeNovaDataFimMaior     = "Campanha:204";
    public const string CancelamentoSomenteEmAndamento       = "Campanha:205";
    public const string ConclusaoSomenteEmAndamento          = "Campanha:206";
    public const string ConclusaoPorDataExigeDataFimExpirada = "Campanha:207";
    public const string ConclusaoPorDataExigeModoCompativel  = "Campanha:208";
    public const string ConclusaoPorMetaExigeMetaAtingida    = "Campanha:209";
    public const string ConclusaoPorMetaExigeModoCompativel  = "Campanha:210";
    public const string ArrecadacaoSomenteEmAndamento        = "Campanha:301";
    public const string ArrecadacaoExigeValorPositivo        = "Campanha:302";
}
