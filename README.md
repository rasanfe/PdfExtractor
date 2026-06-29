# 📑 PdfExtractor

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=flat-square&logo=csharp&logoColor=white)
![iText7](https://img.shields.io/badge/iText7-9.6-007E33?style=flat-square)
![Blog](https://img.shields.io/badge/blog-rsrsystem-FF5722?style=flat-square&logo=blogger&logoColor=white)

> Librería **.NET 10** para **extraer el texto de un PDF** (a fichero `.txt` o a `byte[]`) desde PowerBuilder.

## 📋 ¿Qué es esto?

Le pasas un PDF y un rango de páginas y te devuelve el **texto plano**: ideal para indexar, buscar o
volcar contenido a PowerBuilder.

```csharp
public class PdfExtractor
{
    int    PdfToTxt(string inputFile, string outputFile, int pageFrom, int pageTo); // → .txt
    byte[] PdfToblob(string inputFile, int pageFrom, int pageTo);                   // → byte[]
    string GetError();
}
```

## 🧩 Dependencias

| Paquete | Versión |
|---------|---------|
| [itext7](https://www.nuget.org/packages/itext7) | `9.6.0` |

> 🆕 **Migración a .NET 10:** reescrito de **iTextSharp 5** (abandonado, AGPL) a **iText7 9.6.0**.
> Ojo: en iText7 la estrategia de extracción **tiene estado**, así que se crea una nueva por página.
> Para la versión nacida ya en iText7 (con app de consola de prueba), mira el ejemplo hermano
> **`PdfExtractor8`**.

## 🛠️ Requisitos

- **.NET SDK 10.0** o superior

## 🚀 Compilar

```bat
dotnet build PdfExtractor.csproj -c Release
```

---

📨 **Blog:** <https://rsrsystem.blogspot.com/>

> ¡Nos vemos en el próximo artículo! Y recuerda: en PowerBuilder, los límites solo están en nuestra imaginación. 🚀
