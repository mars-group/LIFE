using System.Net;
using CommonTypes.TypeEnums;

namespace NodeRegistry.Interface
{
    /// <summary>
    /// 
    /// </summary>
    public interface INodeEndpoint
    {
        IPAddress GetIpAddress();

        int GetPort();

        NodeType GetNodeType();
    }
}