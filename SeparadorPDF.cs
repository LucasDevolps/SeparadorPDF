using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.IO;
using Ghostscript.NET;

namespace SeparadordePDF
{
    public class SeparadorPDF
    {
        private PdfReader _docPdfParaDividirArquivo = null;
        private int _rangeDePaginasPorDeclaracao = 0;
        private string _diretorioImagemTemporaria = string.Format("{0}PaginaPdfComoImagem.png", System.IO.Path.GetFullPath(@"..\..\"));

        public SeparadorPDF(string diretorioPdf, int rangeDePaginasPorDeclaracao)
        {
            _docPdfParaDividirArquivo = new PdfReader(diretorioPdf);
            _rangeDePaginasPorDeclaracao = rangeDePaginasPorDeclaracao;
        }

        public void CriarInformes(string diretorioParaGeracaoDePdf, string complemento)
        {
            try
            {
                //if (Directory.Exists(diretorioParaGeracaoDePdf))
                    //Directory.Delete(diretorioParaGeracaoDePdf, true);
                Directory.CreateDirectory(diretorioParaGeracaoDePdf);
                Ocr ocr = new Ocr();
                Document pagina = null;
                PdfCopy novoArquivoPdf = null;
                int PageNum = _docPdfParaDividirArquivo.NumberOfPages;
                int quantidadeDeArquivosGerados = 0;
                int contadorDePaginasGeradas = 0;
                string nomeArquivoPdf = string.Empty;
                bool reajustarContadorLoop = false;
                for (int i = 0; i <= PageNum; i++)
                {
                    try
                    {
                        if (reajustarContadorLoop)
                            i--;
                        if (contadorDePaginasGeradas <= _rangeDePaginasPorDeclaracao - 1)
                        {
                            if (novoArquivoPdf == null)
                            {
                                this.ConverterPaginaEmImagem(i + 1);
                                string cpf = ocr.RecuperarCpf(_diretorioImagemTemporaria);
                                string nomeArquivo = this.CriarNomeDeArquivoBaseadoEmCpf(diretorioParaGeracaoDePdf, cpf);
                                nomeArquivo += complemento;
                                pagina = new Document(_docPdfParaDividirArquivo.GetPageSizeWithRotation(i + 1));
                                novoArquivoPdf = new PdfCopy(pagina, new System.IO.FileStream(string.Format(@"{0}\{1}.pdf", diretorioParaGeracaoDePdf, nomeArquivo), System.IO.FileMode.Create));
                                pagina.Open();
                            }
                            novoArquivoPdf.AddPage(novoArquivoPdf.GetImportedPage(_docPdfParaDividirArquivo, i + 1));
                            contadorDePaginasGeradas++;
                            reajustarContadorLoop = false;
                        }
                        else
                        {
                            pagina.Close();
                            pagina = null;
                            novoArquivoPdf = null;
                            quantidadeDeArquivosGerados++;
                            contadorDePaginasGeradas = 0;
                            reajustarContadorLoop = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("erro: "+ex.Message);
                    }
                }
                Console.WriteLine(string.Format("Quantidade de páginas no PDF original :{0}", PageNum));
                Console.WriteLine(string.Format("Quantidade de arquivos PDF gerados :{0}", quantidadeDeArquivosGerados));
                Console.ReadKey();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void ConverterPaginaEmImagem(int indicePagina)
        {
            File.Delete(_diretorioImagemTemporaria);
            File.Delete(_diretorioImagemTemporaria.Replace(".png", ".pdf"));
            Document pagina = null;
            PdfCopy novoArquivoPdf = null;
            pagina = new Document(_docPdfParaDividirArquivo.GetPageSizeWithRotation(indicePagina));
            novoArquivoPdf = new PdfCopy(pagina, new System.IO.FileStream(_diretorioImagemTemporaria.Replace(".png", ".pdf"), System.IO.FileMode.Create));
            pagina.Open();
            novoArquivoPdf.AddPage(novoArquivoPdf.GetImportedPage(_docPdfParaDividirArquivo, indicePagina));
            pagina.Close();
            GhostscriptPngDevice imagem = new GhostscriptPngDevice(GhostscriptPngDeviceType.Png16m);
            imagem.GraphicsAlphaBits = GhostscriptImageDeviceAlphaBits.V_4;
            imagem.TextAlphaBits = GhostscriptImageDeviceAlphaBits.V_4;
            imagem.ResolutionXY = new GhostscriptImageDeviceResolution(200, 200);
            imagem.InputFiles.Add(_diretorioImagemTemporaria.Replace(".png", ".pdf"));
            imagem.Pdf.FirstPage = 1;
            imagem.Pdf.LastPage = 1;
            imagem.PostScript = string.Empty;
            imagem.OutputPath = _diretorioImagemTemporaria;
            imagem.Process();
        }

        private string CriarNomeDeArquivoBaseadoEmCpf(string diretorioParaGeracaoDePdf, string cpf)
        {
            int quantidadeDeOcorrencias = Directory.GetFiles(diretorioParaGeracaoDePdf, string.Format("{0}*", cpf), SearchOption.AllDirectories).Length;
            if (quantidadeDeOcorrencias > 0)
            {
                return string.Format("{0}_{1}", cpf, quantidadeDeOcorrencias + 1);
            }
            else
                return cpf;
        }

    }
}
