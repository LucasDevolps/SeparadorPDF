using System;
using System.IO;

namespace SeparadordePDF
{
    public class Program    
    {
        static void Main(string[] args)
        {

            //Console.Write("Por favor informe onde o PDF está: ");
            string dirTodosInformesPdf = @"C:\Users\lucas\Desktop\PDFs\Randonprev_Informe de Rendimentos_2023_RFB.pdf"; //Console.ReadLine();
            //Console.Write("Por favor informe onde o PDF será salvo: ");
            string dirPastaRetornoProcessamento = @"C:\Users\lucas\Desktop\Teste";//Console.ReadLine();
            //Console.Write("Por favor, informe quantas páginas serão por aquivo, caso não queira aperte enter...:");
            string rangeProcessamentoPagina = "2";//Console.ReadLine();
            //Console.Write("Complemento (exemplo: '-2020-01'): ");
            string complemento = "-2023-01";
            try
            {
                int paginas = 2;
                if (!String.IsNullOrEmpty(rangeProcessamentoPagina))
                {
                    paginas = Convert.ToInt32(rangeProcessamentoPagina);
                }

                if (!Directory.Exists(dirPastaRetornoProcessamento))
                {
                    Console.WriteLine("Diretório inexistente");
                }
                else{
                    GerarInformes(dirTodosInformesPdf, dirPastaRetornoProcessamento,paginas,complemento);
                }
            }
            catch(Exception ex) { Console.WriteLine("Erro: " + ex.Message); }            
            
        }

        private static void GerarInformes(string dirTodosInformesPdf, string dirPastaRetornoProcessamento, int RangePaginas,string complemento)
        {
            try
            {
                int rangeDePaginasPorDeclaracao = RangePaginas;
                SeparadorPDF informeDeRendimento = new SeparadorPDF(dirTodosInformesPdf, rangeDePaginasPorDeclaracao);
                Console.WriteLine("Criando informe de rendimento...");
                informeDeRendimento.CriarInformes(dirPastaRetornoProcessamento,complemento);
                Console.WriteLine("Processo concluído !");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao executar o processo " + ex.Message);
            }
        }
    }
}
