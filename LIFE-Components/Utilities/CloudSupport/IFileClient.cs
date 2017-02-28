namespace LIFE.Components.Utilities.CloudSupport {

  /// <summary>
  ///   Interface for the C# File client.
  /// </summary>
  public interface IFileClient {

    /// <summary>
    ///   Get file for the given data id and return its path.
    /// </summary>
    /// <param name="dataId">Data id of the file to download</param>
    /// <returns>Path of the downloaded file</returns>
    string DownloadFile(string dataId);
  }
}