using PGYMiniCooper.DataModule.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Service
{
    public interface IProdigyService
    {
        bool Initialize(ConfigModel configModel);

        void StartRun();

        void StopRun();

        void Reset();
    }
}
