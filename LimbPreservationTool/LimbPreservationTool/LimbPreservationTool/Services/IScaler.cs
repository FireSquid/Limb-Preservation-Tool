using System;
using System.IO;
namespace LimbPreservationTool.Services
{
    public interface IScaler
    {

        Stream ResizeImage(byte[] imageData, float width, float height);
    }
}
