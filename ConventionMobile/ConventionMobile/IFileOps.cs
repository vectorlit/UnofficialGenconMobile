using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConventionMobile
{
    public interface IFileOps
    {
        bool SaveFile(byte[] bytes, string fileName);

        bool FileExists(string filePath);
        string GetFileLocation(string fileName);
    }
}
