using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGYMiniCooper.DataModule.Interface
{
    public interface IResultModel
    {
        event EventHandler<IReadOnlyList<IFrame>> OnFramesDecoded;

        void AddFrame(IFrame frame);

        void Reset();
    }
}
