using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace SeparadordePDF
{
    public class Ocr
    {
        private string caminhoConfiguracaoDeIdioma = string.Format("{0}tessdata", System.IO.Path.GetFullPath(@"..\..\"));
        private string idioma = "por";
        public string RecuperarCpf(string caminhoImagem)
        {
            string cpf = string.Empty;
            try
            {
                using (TesseractEngine engine = new TesseractEngine(caminhoConfiguracaoDeIdioma, idioma, EngineMode.Default))
                {
                    using (Pix imagem = Pix.LoadFromFile(caminhoImagem))
                    {
                        using (Page pagina = engine.Process(imagem))
                        {
                            // formata texto para recuperar CPF.
                            string texto = pagina.GetText().Replace(Environment.NewLine, " ").ToLower().Replace(" ", string.Empty);
                            // Filtra conteúdo entre substrings 
                            int indexOfSubstringInicio = texto.IndexOf("cpfnomecompleto");
                            int indexOfSubstringFim = 31;
                            texto = texto.Substring(indexOfSubstringInicio, indexOfSubstringFim).Replace(".", "").Replace("-", "");
                            cpf = String.Join("", System.Text.RegularExpressions.Regex.Split(texto, @"[^\d]"));
                        }
                    }
                }
            }
            catch
            {
                cpf = string.Empty;
            }
            return cpf;
        }
    }
}
