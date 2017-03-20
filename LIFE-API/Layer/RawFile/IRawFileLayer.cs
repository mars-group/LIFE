using System.IO;

namespace LIFE.API.Layer.RawFile
{
    public interface IRawFileLayer : ILayer
    {
        FileStream GetFileStream();
    }
}