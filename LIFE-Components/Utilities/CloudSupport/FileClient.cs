using System;
using System.IO;
using System.Net.Http;
using NLog;

namespace LIFE.Components.Utilities.CloudSupport {

  public class FileClient : IFileClient {

    public static string InputDataDirectory = "./tmp/inputData";
    private readonly HttpClient _httpClient;
    private readonly Logger _logger;

    public FileClient() {
      _httpClient = new HttpClient();
      _logger = LogManager.GetCurrentClassLogger();
    }

    public string DownloadFile(string dataId) {
      var fileServiceHost = "http://file-svc";


      try {
        CreateDirectoryIfNotExist();
        var resultTask = _httpClient.GetAsync($"{fileServiceHost}/files/{dataId}");
        using (var stream = new FileStream(InputDataDirectory + $"/{dataId}", FileMode.Create, FileAccess.Write)) {
          resultTask.Result.Content.CopyToAsync(stream).Wait();
          stream.Dispose();
        }
      }
      catch (Exception exception) {
        _logger.Error("File retrieval failed: " + exception.StackTrace);
        throw new InvalidOperationException("File retrieval not possible.", exception);
      }
      return InputDataDirectory + "/" + dataId;
    }

    private void CreateDirectoryIfNotExist() {
      if (!Directory.Exists(InputDataDirectory))
        Directory.CreateDirectory(InputDataDirectory);
    }
  }
}