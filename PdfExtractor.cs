using System;
using System.IO;
using System.Text;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace PdfExtractor
{
    /// <summary>
    /// Extractor de TEXTO de ficheros PDF pensado para consumirse desde PowerBuilder
    /// (vía .NET DLL Importer / Assembly). Os instanciáis la clase y llamáis a
    /// <see cref="PdfToTxt"/> (vuelca a un .txt) o a <see cref="PdfToblob"/> (devuelve
    /// el texto como blob para meterlo directo en una DataWindow o variable blob de PB).
    /// </summary>
    /// <remarks>
    /// HISTORIA DIDÁCTICA: esta clase venía de iTextSharp 5, que está ABANDONADO. La
    /// reescribí entera a iText7 (paquete <c>itext7</c> 9.6.0), que es la línea viva.
    /// Fijaos en los dos cambios de fondo del salto de versión:
    /// <list type="bullet">
    /// <item>Ya no se usa <c>PdfReader</c> a secas para leer páginas: ahora hay un
    /// <see cref="PdfDocument"/> por encima del <see cref="PdfReader"/>, y se piden las
    /// páginas con <c>document.GetPage(n)</c> (las páginas empiezan en 1, no en 0).</item>
    /// <item>La estrategia de extracción (<see cref="SimpleTextExtractionStrategy"/>)
    /// TIENE ESTADO: acumula internamente el texto que va leyendo. Hay que crear una
    /// NUEVA por cada página o se os repite/acumula el texto de las anteriores. Este es
    /// EL gotcha del cambio a iText7 y la causa del bug que arrastraba el código viejo.</item>
    /// </list>
    /// </remarks>
    public class PdfExtractor

    {
        // Guardamos aquí el último mensaje de error. Como desde PowerBuilder no es cómodo
        // pillar la excepción .NET, el patrón es: relanzamos (throw) Y dejamos el texto
        // accesible con GetError() para que PB lo pueda mostrar sin pelearse con el try/catch.
        private string errorText = "";

        /// <summary>
        /// Extrae el texto de las páginas <paramref name="pageFrom"/> a
        /// <paramref name="pageTo"/> del PDF y lo escribe en <paramref name="outputFile"/>,
        /// una página por línea. Devuelve el número de páginas procesadas.
        /// </summary>
        /// <param name="inputFile">Ruta del PDF de entrada.</param>
        /// <param name="outputFile">Ruta del .txt que se genera (se sobrescribe).</param>
        /// <param name="pageFrom">Primera página a extraer (la 1 es la primera).</param>
        /// <param name="pageTo">Última página a extraer (incluida).</param>
        // Migrado de iTextSharp 5 a iText7: PdfReader + PdfDocument, y la extraccion
        // de texto via PdfTextExtractor.GetTextFromPage(page, strategy).
        public int PdfToTxt(string inputFile, string outputFile, int pageFrom, int pageTo)
        {
            try
            {
                // SetUnethicalReading(true): equivale al viejo PdfReader.unethicalreading de
                // iTextSharp. Le decimos a iText que abra PDFs protegidos solo-lectura aunque
                // no tengamos la contraseña de propietario (típico de PDFs que solo restringen
                // copiar/imprimir). Es lo que necesitamos para poder leer su texto.
                PdfReader reader = new PdfReader(inputFile).SetUnethicalReading(true);
                PdfDocument document = new PdfDocument(reader);
                int numberOfPages = 0;

                using (StreamWriter sw = new StreamWriter(outputFile))
                {
                    for (int pagenumber = pageFrom; pagenumber <= pageTo; pagenumber++)
                    {
                        // En iText7 la estrategia tiene estado: hay que crear una NUEVA por
                        // pagina, o el texto se acumula y se repite (el bug que tenia el codigo viejo).
                        var strategy = new SimpleTextExtractionStrategy();
                        string currentText = PdfTextExtractor.GetTextFromPage(document.GetPage(pagenumber), strategy);
                        sw.WriteLine(currentText);
                        numberOfPages++;
                    }
                }

                document.Close();
                return numberOfPages;
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
                throw;
            }

        }

        /// <summary>
        /// Igual que <see cref="PdfToTxt"/> pero SIN tocar disco: devuelve el texto extraído
        /// como <c>byte[]</c> en UTF-8. Pensado para PowerBuilder, donde se recoge en una
        /// variable <c>blob</c> y, si hace falta, se pasa a string con <c>String(blb, EncodingUTF8!)</c>.
        /// </summary>
        /// <param name="inputFile">Ruta del PDF de entrada.</param>
        /// <param name="pageFrom">Primera página a extraer (la 1 es la primera).</param>
        /// <param name="pageTo">Última página a extraer (incluida).</param>
        public byte[] PdfToblob(string inputFile, int pageFrom, int pageTo)
        {
            try
            {
                PdfReader reader = new PdfReader(inputFile).SetUnethicalReading(true);
                PdfDocument document = new PdfDocument(reader);

                // Acumulamos el texto de todas las páginas en un StringBuilder; es bastante
                // más eficiente que ir concatenando strings (en .NET cada concatenación crea
                // un string nuevo, porque son inmutables).
                StringBuilder pageText = new StringBuilder();

                for (int pagenumber = pageFrom; pagenumber <= pageTo; pagenumber++)
                {
                    // Una estrategia NUEVA por página: ver el porqué en el <remarks> de la clase.
                    var strategy = new SimpleTextExtractionStrategy();
                    string currentText = PdfTextExtractor.GetTextFromPage(document.GetPage(pagenumber), strategy);
                    pageText.Append(currentText);
                    pageText.Append("\n");  // Agregar nueva línea entre páginas
                }

                document.Close();
                // Convertir el texto extraído a un array de bytes usando UTF-8
                byte[] result = Encoding.UTF8.GetBytes(pageText.ToString());
                return result;
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
                throw;
            }

        }

        /// <summary>
        /// Devuelve el texto del último error capturado. Útil desde PowerBuilder: tras un
        /// fallo, llamáis a este método para enseñar el mensaje sin tener que interpretar
        /// la excepción .NET.
        /// </summary>
        public string GetError()
        {
            return errorText;
        }



    }
}
