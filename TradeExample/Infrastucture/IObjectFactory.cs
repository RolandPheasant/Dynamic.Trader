using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeExample.Infrastucture
{
    public interface IObjectProvider
    {
        T Get<T>();
    }
}
