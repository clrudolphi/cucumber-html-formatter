// See https://aka.ms/new-console-template for more information
using Cucumber.Messages;
using Io.Cucumber.Messages.Types;

if (args.Length == 0) { Console.WriteLine("Missing NDJSON file name."); return; }

var fileName = args[0];
var rootFileName = Path.GetFileNameWithoutExtension(fileName);
var filePath = Path.GetDirectoryName(fileName);
var outputFileName = rootFileName + ".html";

if (!String.IsNullOrEmpty(filePath))
    outputFileName = Path.Combine(filePath, outputFileName);

if (args.Length == 2)
{
    outputFileName = args[1];
}

var outFile = File.Create(outputFileName);
var outputStreamWriter = new StreamWriter(outFile);

var inputStream = File.OpenText(fileName).BaseStream;

var streamSerializerAction = new Action<StreamWriter, Envelope>( (sw, e) => { HtmlFormatter.NdjsonSerializer.SerializeToStream(sw.BaseStream, e);  });

using var ndjsonReader = new NdjsonMessageReader(inputStream,  HtmlFormatter.NdjsonSerializer.Deserialize);
using var htmlFormatter = new HtmlFormatter.MessagesToHtmlWriter(outputStreamWriter, streamSerializerAction);

foreach(var message in ndjsonReader)
{
    htmlFormatter.Write(message);
}
htmlFormatter.Dispose();
inputStream.Close();
outputStreamWriter.Close();
outFile.Close();