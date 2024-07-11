using System;
using System.IO;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
//using iText.Kernel.Pdf.Canvas.Parser.Listener;
//using iText.Kernel.Pdf.Canvas.Parser;
//using iText.Kernel.Pdf;

namespace PdfExtractor
{
    public class PdfExtractor

    {
        private string errorText = "";

        //Usando ItextSharp
        public int PdfToTxt(string inputFile, string outputFile, int pageFrom, int pageTo)
        {
            try
            {
                PdfReader.unethicalreading = true;
                PdfReader reader = new PdfReader(inputFile);
                int numberOfPages = 0;
                //LocationTextExtractionStrategy strategy = new LocationTextExtractionStrategy();
                SimpleTextExtractionStrategy strategy = new SimpleTextExtractionStrategy();

                using (StreamWriter sw = new StreamWriter(outputFile))
                {
                    for (int pagenumber = pageFrom; pagenumber <= pageTo; pagenumber++)
                    {
                        string currentText = PdfTextExtractor.GetTextFromPage(reader, pagenumber, strategy);
                        sw.WriteLine(currentText);
                        numberOfPages++;
                    }
                }

                reader.Close();
                reader.Dispose();
                return numberOfPages;
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
                throw;
            }

        }

        public byte[] PdfToblob(string inputFile, int pageFrom, int pageTo)
        {
            try
            {
                PdfReader.unethicalreading = true;
                PdfReader reader = new PdfReader(inputFile);
                
                //LocationTextExtractionStrategy strategy = new LocationTextExtractionStrategy();
                SimpleTextExtractionStrategy strategy = new SimpleTextExtractionStrategy();

                // Asignar ubicación del PDF y crear un StringBuilder
                StringBuilder pageText = new StringBuilder();

                for (int pagenumber = pageFrom; pagenumber <= pageTo; pagenumber++)
                {
                    string currentText = PdfTextExtractor.GetTextFromPage(reader, pagenumber, strategy);
                    pageText.Append(currentText);
                    pageText.Append("\n");  // Agregar nueva línea entre páginas
                }


                reader.Close();
                reader.Dispose();
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

        //Versión con iText7, devuelve todo ????, pero si lo hacemos con una app de consola va bien....
        //public int PdfToTxt(string inputFile, string outputFile, int pageFrom, int pageTo)
        //{
        //    try
        //    {
        //        PdfDocument document = new PdfDocument(new PdfReader(inputFile));
        //        int numberOfPages = 0;
        //        //LocationTextExtractionStrategy strategy = new LocationTextExtractionStrategy();
        //        SimpleTextExtractionStrategy strategy = new SimpleTextExtractionStrategy();

        //        using (StreamWriter sw = new StreamWriter(outputFile))
        //        {
        //            for (int pagenumber = pageFrom; pagenumber <= pageTo; pagenumber++)
        //            {
        //                string currentText = PdfTextExtractor.GetTextFromPage(document.GetPage(pagenumber), strategy);
        //                sw.WriteLine(currentText);
        //                numberOfPages++;
        //            }
        //        }

        //        document.Close();
        //        return numberOfPages;
        //    }
        //    catch (Exception ex)
        //    {
        //        errorText = ex.Message;
        //        throw;
        //    }

        //}

        //public byte[] PdfToblob(string inputFile, int pageFrom, int pageTo)
        //{
        //    try
        //    {
        //        PdfDocument document = new PdfDocument(new PdfReader(inputFile));
        //        //LocationTextExtractionStrategy strategy = new LocationTextExtractionStrategy();
        //        SimpleTextExtractionStrategy strategy = new SimpleTextExtractionStrategy();

        //        // Asignar ubicación del PDF y crear un StringBuilder
        //        StringBuilder pageText = new StringBuilder();

        //        for (int pagenumber = pageFrom; pagenumber <= pageTo; pagenumber++)
        //        {
        //            string currentText = PdfTextExtractor.GetTextFromPage(document.GetPage(pagenumber), strategy);
        //            pageText.Append(currentText);
        //            pageText.Append("\n");  // Agregar nueva línea entre páginas
        //        }


        //        document.Close();
        //        // Convertir el texto extraído a un array de bytes usando UTF-8
        //        byte[] result = Encoding.UTF8.GetBytes(pageText.ToString());
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        errorText = ex.Message;
        //        throw;
        //    }

        //}

        public string GetError()
        {
            return errorText;
        }



    }
}
